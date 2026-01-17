using System.Collections;
using UnityEngine;
using System;


[RequireComponent(typeof(Rigidbody2D), typeof(AutomaticEater))]
public class PlayerController : EntityComponent, IMovementProvider, IFacingDirectionProvider
{
    [Header("Movement")]
    public float moveSpeed = 5;
    public float stopThreshold = 0f;

    [Header("Input")]
    public InputScheme inputScheme => GameConfig.Instance.InputScheme;

    public event Action OnFixedUpdated;

    private FacingDirection lastDirection = FacingDirection.Down;
    private Rigidbody2D rb;

    // Cached continuous facing direction for Continuous scheme (normalized or zero).
    private Vector2 lastContinuousDirection = Vector2.down;
    public FacingDirection CurrentFacingDistinctDirection { get; private set; } = FacingDirection.None;

    // For compatibility: Direction is 8-way for discrete schemes, continuous for Continuous.
    public Vector2 Direction
    {
        get
        {
            switch (inputScheme)
            {
                case InputScheme.Old8Directional:
                case InputScheme.New8Directional:
                    // Existing behavior: snapped to 8 directions via facing enum
                    return DirectionToVector(CurrentFacingDistinctDirection);

                case InputScheme.Continuous:
                    // In continuous mode, use the cached continuous direction that
                    // respects lockToggle (updated in HandleFacing_Continuous).
                    if (lastContinuousDirection.sqrMagnitude > 0.0001f)
                        return lastContinuousDirection;

                    // Fallbacks if somehow we don't have a cached value.
                    var input = PlayerInputController.Instance;
                    if (input != null)
                    {
                        Vector2 aim = input.aimInput;
                        if (aim.sqrMagnitude > 0.0001f)
                            return aim.normalized;
                    }

                    if (Velocity.sqrMagnitude > 0.0001f)
                        return Velocity.normalized;

                    return Vector2.zero;

                default:
                    return DirectionToVector(CurrentFacingDistinctDirection);
            }
        }
    }

    public bool IsMoving { get; private set; }
    public Vector2 Velocity { get; private set; }

    public bool shooting = false;
    public bool startedShootingThisFrame = false;
    AutomaticEater automaticEater;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        automaticEater = GetComponent<AutomaticEater>();

        // Optional: for mouse-based aiming.
        if (PlayerInputController.Instance != null)
        {
            PlayerInputController.Instance.RegisterPlayer(transform);
        }
    }

    protected override void OnDeath(DeathEventData deathEventData)
    {
        InstantDie();
    }

    private void FixedUpdate()
    {
        var input = PlayerInputController.Instance;
        Vector2 moveInput = input.moveInput;
        Vector2 aimInput = input.aimInput;

        Debug.Log("Move Input: " + moveInput + ", Aim Input: " + aimInput);

        if(!shooting && input.attacking)
        {
            startedShootingThisFrame = true;
        }
        else
        {
            startedShootingThisFrame = false;
        }
        shooting = input.attacking;
        bool lockShootDirection = input.lockToggle;

        // --- Calculate movement & velocity based on scheme ---
        switch (inputScheme)
        {
            case InputScheme.Old8Directional:
                HandleMovement_Old8Directional(moveInput);
                break;

            case InputScheme.New8Directional:
                HandleMovement_New8Directional(moveInput);
                break;

            case InputScheme.Continuous:
                HandleMovement_Continuous(moveInput);
                break;
        }

        // --- Calculate facing / aiming based on scheme ---
        switch (inputScheme)
        {
            case InputScheme.Old8Directional:
                HandleFacing_Old8Directional(moveInput, lockShootDirection);
                break;

            case InputScheme.New8Directional:
                HandleFacing_New8Directional(moveInput, aimInput, lockShootDirection, startedShootingThisFrame);
                break;

            case InputScheme.Continuous:
                HandleFacing_Continuous(moveInput, aimInput, lockShootDirection, startedShootingThisFrame);
                break;
        }

        // Nasty fix for when porky is respawned while player is shooting,
        // so that he doesn't end up facing none
        if (CurrentFacingDistinctDirection == FacingDirection.None)
        {
            CurrentFacingDistinctDirection = lastDirection;
        }

        OnFixedUpdated?.Invoke();

        if (automaticEater != null)
        {
            automaticEater.EatingEnabled = !IsMoving && !shooting;
        }
    }

    // -------- Movement handlers --------

    void HandleMovement_Old8Directional(Vector2 moveInput)
    {
        // Old behavior: snap movement to 8 directions
        var inputFacingDirection = GetDirectionFromInput(moveInput);
        Vector2 moveDir = DirectionToVector(inputFacingDirection);

        IsMoving = moveInput.magnitude > stopThreshold && moveDir.sqrMagnitude > 0f;
        rb.linearVelocity = moveDir * (IsMoving ? moveSpeed : 0f);
        Velocity = rb.linearVelocity;
    }

    void HandleMovement_New8Directional(Vector2 moveInput)
    {
        // 8-dir movement, but aim may come from separate input
        var inputFacingDirection = GetDirectionFromInput(moveInput);
        Vector2 moveDir = DirectionToVector(inputFacingDirection);

        IsMoving = moveInput.magnitude > stopThreshold && moveDir.sqrMagnitude > 0f;
        rb.linearVelocity = moveDir * (IsMoving ? moveSpeed : 0f);
        Velocity = rb.linearVelocity;
    }

    void HandleMovement_Continuous(Vector2 moveInput)
    {
        // Continuous movement: do not snap, but keep stopThreshold
        float mag = moveInput.magnitude;
        IsMoving = mag > stopThreshold;

        Vector2 moveDir = mag > 1e-4f ? moveInput / mag : Vector2.zero;
        rb.linearVelocity = moveInput * moveSpeed;//moveDir * (IsMoving ? moveSpeed : 0f);
        Velocity = rb.linearVelocity;
    }

    // -------- Facing / aiming handlers --------

    void HandleFacing_Old8Directional(Vector2 moveInput, bool lockShootDirection)
    {
        var inputFacingDirection = GetDirectionFromInput(moveInput);

        if (!shooting || !lockShootDirection)
        {
            if (inputFacingDirection != FacingDirection.None)
            {
                CurrentFacingDistinctDirection = inputFacingDirection;
                lastDirection = CurrentFacingDistinctDirection;
            }
            else
            {
                CurrentFacingDistinctDirection = lastDirection;
            }
        }
    }

    void HandleFacing_New8Directional(Vector2 moveInput, Vector2 aimInput, bool lockShootDirection, bool startedShootingThisFrame)
    {
        // Prefer aimInput if any; otherwise fall back to movement input.
        Vector2 source = aimInput.sqrMagnitude > 0.0001f ? aimInput : moveInput;
        var inputFacingDirection = GetDirectionFromInput(source);
        inputFacingDirection = inputFacingDirection == FacingDirection.None ? lastDirection : inputFacingDirection;

        if (shooting)
        {
            if (lockShootDirection && !startedShootingThisFrame)
            {
                CurrentFacingDistinctDirection = lastDirection;
            }
            else
            {
                CurrentFacingDistinctDirection = inputFacingDirection;
                lastDirection = CurrentFacingDistinctDirection;
            }
        }
        else
        {
            CurrentFacingDistinctDirection = inputFacingDirection;
            lastDirection = CurrentFacingDistinctDirection;
        }
    }



    void HandleFacing_Continuous(Vector2 moveInput, Vector2 aimInput, bool lockShootDirection, bool startedShootingThisFrame)
    {
        // Source for facing: aim preferred, otherwise movement.
        Vector2 source = aimInput.sqrMagnitude > 0.05f ? aimInput : moveInput;

        // Still maintain 8-way facing enum for animation, etc.
        var inputFacingDirection = GetDirectionFromInput(source);
        //check shooting and lock
        //Debug.Log("Shooting: " + shooting + ", LockShootDirection: " + lockShootDirection + " Source: " + source);

        if (source.sqrMagnitude > 0.05f)
        {
            lastContinuousDirection = source.normalized;
            CurrentFacingDistinctDirection = inputFacingDirection;
        }


        if(CurrentFacingDistinctDirection == FacingDirection.None)
        {
            CurrentFacingDistinctDirection = lastDirection;
            lastContinuousDirection = Vector2.down;
        }

    }

    private FacingDirection GetDirectionFromInput(Vector2 input)
    {
        if (input == Vector2.zero)
            return FacingDirection.None;

        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        angle = (angle + 360) % 360; // Normalize angle to [0,360)

        if (angle >= 337.5f || angle < 22.5f)
            return FacingDirection.Right;
        if (angle >= 22.5f && angle < 67.5f)
            return FacingDirection.UpRight;
        if (angle >= 67.5f && angle < 112.5f)
            return FacingDirection.Up;
        if (angle >= 112.5f && angle < 157.5f)
            return FacingDirection.UpLeft;
        if (angle >= 157.5f && angle < 202.5f)
            return FacingDirection.Left;
        if (angle >= 202.5f && angle < 247.5f)
            return FacingDirection.DownLeft;
        if (angle >= 247.5f && angle < 292.5f)
            return FacingDirection.Down;
        if (angle >= 292.5f && angle < 337.5f)
            return FacingDirection.DownRight;

        return FacingDirection.None;
    }

    // Returns a normalized Vector2 for the given FacingDirection
    public static Vector2 DirectionToVector(FacingDirection direction)
    {
        switch (direction)
        {
            case FacingDirection.Up:        return Vector2.up;
            case FacingDirection.UpRight:   return new Vector2(1, 1).normalized;
            case FacingDirection.Right:     return Vector2.right;
            case FacingDirection.DownRight: return new Vector2(1, -1).normalized;
            case FacingDirection.Down:      return Vector2.down;
            case FacingDirection.DownLeft:  return new Vector2(-1, -1).normalized;
            case FacingDirection.Left:      return Vector2.left;
            case FacingDirection.UpLeft:    return new Vector2(-1, 1).normalized;
            default:                        return Vector2.zero;
        }
    }

    protected override void InstantDie()
    {
        rb.linearDamping = 10;
        enabled = false;
        //TODO clean up automatic eater and other components. Maybe create a base class?
        automaticEater.EatingEnabled = false;
        automaticEater.enabled = false;
    }
}

[Serializable]
public enum InputScheme
{
    Old8Directional,        // 1) old (8-dir movement = aim, lockable)
    New8Directional,        // 2) new 8-dir movement + 8-dir aim, lockable
    Continuous              // 3) continuous move + continuous aim, lockable
}

public enum FacingDirection
{
    Up,
    UpRight,
    Right,
    DownRight,
    Down,
    DownLeft,
    Left,
    UpLeft,
    None
}