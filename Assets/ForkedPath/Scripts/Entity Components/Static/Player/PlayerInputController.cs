using Pixelplacement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static MobileUIJoystick;

public class PlayerInputController : Singleton<PlayerInputController>
{
    public Vector2 moveInput = Vector2.zero;
    public Vector2 aimInput = Vector2.zero;

    public bool attacking = false;
    public bool lockToggle = true;

    // Optional: reference to the player transform so we can compute mouse-relative aim
    [SerializeField] private Transform playerTransform;

    // Threshold to consider aim "non-zero"
    const float AimFireThresholdSqr = 0.01f * 0.01f;

    InputScheme CurrentScheme => GameConfig.Instance != null ? GameConfig.Instance.InputScheme : InputScheme.Old8Directional;

    public void OnAttack(InputValue inputValue)
    {
        // Explicit button should always win for schemes that use it.
        //if(Mouse.current != null)
        //{
        //    Vector2 screenPos = Mouse.current.position.ReadValue();
        //    if (IsPointerOverUI(screenPos))
        //    {
        //        attacking = false;
        //        aimInput = Vector2.zero;
        //        return;
        //    }
        //}
        attacking = inputValue.isPressed;
    }

    public void OnInteract(InputValue inputValue)
    {
        lockToggle = !lockToggle;
    }

    public void OnMove(InputValue inputValue)
    {
        moveInput = inputValue.Get<Vector2>();
    }

    public void OnMove(Vector2 inputVector)
    {
        moveInput = inputVector;
    }

    /// <summary>
    /// Called from a second stick / gamepad / mouse-aim action.
    /// For gamepad twin-stick this is typically bound to "Right Stick" 2D vector.
    /// </summary>
    public void OnAim(InputValue inputValue)
    {
        aimInput = inputValue.Get<Vector2>();
        UpdateAttackingFromAim();
    }

    public void OnAim(Vector2 inputVector)
    {
        aimInput = inputVector;
        UpdateAttackingFromAim();
    }

    /// <summary>
    /// Mouse/touch-based aim: bound to a Vector2 screen-position action (e.g. "Point").
    /// On mobile with two fingers, ignore positions over UI so the movement joystick finger
    /// doesn't drive aim.
    /// </summary>
    public void OnMouseAim(InputValue inputValue)
    {

        if (playerTransform == null)
            return;

        Vector2 screenPos = inputValue.Get<Vector2>();

        // Robust UI check using raycast instead of relying solely on IsPointerOverGameObject
        //if (IsPointerOverUI(screenPos))
        //{
        //    aimInput = Vector2.zero;
        //    return;
        //}

            //Debug.Log("Mouse Aim Screen Position: " + screenPos);

        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -cam.transform.position.z));
        Vector2 direction = (Vector2)(worldPos - playerTransform.position);
        //Debug.Log("Mouse Aim Direction: " + direction);
        aimInput = direction.normalized;
        //UpdateAttackingFromAim();
    }

    bool IsPointerOverUI(Vector2 screenPosition)
    {
        if (EventSystem.current == null) return false;

        var eventData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    public void OnAttackPressed()
    {
        attacking = true;
    }

    public void OnAttackReleased()
    {
        attacking = false;
    }

    public void OnLockToggle(bool isOn)
    {
        lockToggle = isOn;
    }

    /// <summary>
    /// Called by the movement joystick (existing behavior).
    /// </summary>
    public void OnJoystickMove(Direction8 direction8, Vector2 vector)
    {
        moveInput = vector;
    }

    /// <summary>
    /// Called by a second joystick used for aiming (discrete 8-dir).
    /// </summary>
    public void OnAimJoystickMove(Direction8 direction8, Vector2 vector)
    {
        aimInput = vector;
        UpdateAttackingFromAim();
    }

    /// <summary>
    /// Let other scripts set the player transform at runtime if needed.
    /// </summary>
    public void RegisterPlayer(Transform t)
    {
        playerTransform = t;
    }

    /// <summary>
    /// For schemes without an attack button, treat non-zero aim as "attacking".
    /// </summary>
    void UpdateAttackingFromAim()
    {
        switch (CurrentScheme)
        {
            case InputScheme.Continuous:
                // No attack button: aim stick / mouse direction == fire.
                attacking = aimInput.sqrMagnitude > AimFireThresholdSqr;
                break;

            case InputScheme.New8Directional:
                // Choose behavior:
                //  - If you want buttonless firing here too, uncomment next line.
                //  - Otherwise, do nothing and keep button-only behavior.
                 attacking = aimInput.sqrMagnitude > AimFireThresholdSqr;
                break;

            case InputScheme.Old8Directional:
            default:
                // Old scheme uses button only.
                break;
        }
    }
}
