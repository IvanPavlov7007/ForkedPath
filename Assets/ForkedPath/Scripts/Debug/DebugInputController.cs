using Pixelplacement;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DebugInputController : Singleton<DebugInputController>
{
#if UNITY_EDITOR

    private void Awake()
    {
        var debugMap = G.Instance.PlayerInput.actions.FindActionMap("Debug", true);
        debugMap.Enable();

    }

    public void OnReloadScene(InputValue value)
    {
        if (value.isPressed)
        {
            GameManager.RestartLevel();
        }
    }

    public void OnSwitchDebugOverlay(InputValue value)
    {
        if(value.isPressed)
            EntityStateDebugOverlayManager.Toggle();
    }
#endif
}