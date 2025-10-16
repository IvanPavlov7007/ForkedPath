using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class MobileJoystickManager : MonoBehaviour
{
    MobileUIJoystick joystick;
    private void Awake()
    {
        joystick = Object.FindFirstObjectByType<MobileUIJoystick>();
        joystick.onDirectionChanged.AddListener(PlayerInputController.Instance.OnJoystickMove);
    }
}