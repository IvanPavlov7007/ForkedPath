using System.Collections;
using UnityEngine;
using System;


[RequireComponent(typeof(Rigidbody2D),typeof(AutomaticEater))]
public class PlayerController : EntityComponent, IMovementProvider, IFacingDirectionProvider
{
    public float moveSpeed = 5;
    public event Action fixedUpdated;
    public float stopThreshold = 0.5f;

    private FacingDirection lastDirection = FacingDirection.Down;
    private Rigidbody2D rb;

    public FacingDirection CurrentFacingDistinctDirection { get; private set; } = FacingDirection.None;
    public Vector2 Direction => DirectionToVector(CurrentFacingDistinctDirection);

    public bool IsMoving { get; private set; }

    public Vector2 Velocity { get; private set; }

    

    public bool shooting = false;
    AutomaticEater automaticEater;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        automaticEater = GetComponent<AutomaticEater>();
    }

    protected override void OnDeath(DeathEventData deathEventData)
    {
        InstantDie();
    }

    private void FixedUpdate()
    {
        Vector2 _input = PlayerInputController.Instance.moveInput;
        shooting = PlayerInputController.Instance.attacking;
        bool lockShootDirection = PlayerInputController.Instance.lockToogle;
        var inputFacingDirection = GetDirectionFromInput(_input);
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
        IsMoving = _input.magnitude > stopThreshold;
        rb.linearVelocity = DirectionToVector(inputFacingDirection) * moveSpeed;
        Velocity = rb.linearVelocity;
        fixedUpdated?.Invoke();

        if (automaticEater != null)
        {
            automaticEater.EatingEnabled = !IsMoving && !shooting;
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
        //TODO clean up atuomatic eater and other components. Maybe create a base class?
        automaticEater.EatingEnabled = false;
        automaticEater.enabled = false;
    }
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