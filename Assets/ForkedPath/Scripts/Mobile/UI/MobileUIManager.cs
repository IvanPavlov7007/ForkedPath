using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Pixelplacement;
public sealed class MobileUIManager : Singleton<MobileUIManager>
{
    public static bool mobileUIActive;
    [Header("Debug")]
    [SerializeField]
    private bool debugForceMobileUIActive = true;
    [SerializeField]
    private bool startWithMobileUIActive = true;

    private void Awake()
    {
#if UNITY_EDITOR
        if(debugForceMobileUIActive)
        {
            mobileUIActive = startWithMobileUIActive;
        }
#endif
    }

    private void Start()
    {
        MobileUI.Instance.ActivateMobileUI(mobileUIActive);
        MobileUI.Instance.Joystick.onDirectionChanged.AddListener(PlayerInputController.Instance.OnJoystickMove);
        MobileUI.Instance.ShootButton.onButtonPressed += PlayerInputController.Instance.OnAttackPressed;
        MobileUI.Instance.ShootButton.onButtonReleased += PlayerInputController.Instance.OnAttackReleased;
    }
}