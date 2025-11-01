using Pixelplacement;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static MobileUIJoystick;

public class PlayerInputController : Singleton<PlayerInputController>
{
    public Vector2 moveInput = Vector2.zero;
    public bool attacking = false;
    public bool lockToggle = true;

    public void OnAttack(InputValue inputValue)
    {
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

    public void OnJoystickMove(Direction8 direction8, Vector2 vector)
    {
        moveInput = vector;
    }
}
