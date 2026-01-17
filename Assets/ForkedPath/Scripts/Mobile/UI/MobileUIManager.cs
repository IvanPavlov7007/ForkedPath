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
#if PLATFORM_ANDROID
        mobileUIActive = true;
#endif
        Initialize();
    }

    private void Initialize()
    {
        MobileUI.Instance.ActivateMobileUI(mobileUIActive);

        // old system
        var oldUI = MobileUI.Instance.oldSystemUIElements;
        oldUI.movementJoystick.onDirectionChanged.AddListener(PlayerInputController.Instance.OnJoystickMove);
        oldUI.shootButton.onButtonPressed += PlayerInputController.Instance.OnAttackPressed;
        oldUI.shootButton.onButtonReleased += PlayerInputController.Instance.OnAttackReleased;
        // new discrete system
        var discreteUI = MobileUI.Instance.newDiscreteUIElements;
        discreteUI.movementJoystick.onDirectionChanged.AddListener(PlayerInputController.Instance.OnJoystickMove);
        discreteUI.aimJoystick.onDirectionChanged.AddListener(PlayerInputController.Instance.OnAimJoystickMove);
        // new continuous system
        var continuousUI = MobileUI.Instance.newContinuousUIElements;
        continuousUI.movementJoystick.joystickOutputEvent.AddListener(PlayerInputController.Instance.OnMove);
        continuousUI.aimJoystick.joystickOutputEvent.AddListener(PlayerInputController.Instance.OnAim);
        
        InputSchemeManager.Instance.OnInputSchemeChanged += MobileUI.Instance.SetInputScheme;
        MobileUI.Instance.SetInputScheme(InputSchemeManager.Instance.GetCurrentInputScheme());
    }
}