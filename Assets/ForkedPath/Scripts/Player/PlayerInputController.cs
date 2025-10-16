using Pixelplacement;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static MobileUIJoystick;

public class PlayerInputController : Singleton<PlayerInputController>
{
    public Vector2 moveInput = Vector2.zero;
    public bool attacking = false;

    public void OnAttack(InputValue inputValue)
    {
        attacking = inputValue.isPressed;
    }

    public void OnMove(InputValue inputValue)
    {
        moveInput = inputValue.Get<Vector2>();
    }

    public void OnJoystickMove(Direction8 direction8, Vector2 vector)
    {
        moveInput = vector;
    }
}
