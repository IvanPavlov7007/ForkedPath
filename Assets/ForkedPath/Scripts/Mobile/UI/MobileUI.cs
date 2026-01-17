using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Pixelplacement;
using System;

public class MobileUI : Singleton<MobileUI>
{
    CanvasGroup canvasGroup;

    public OldSystemUIElements oldSystemUIElements = new OldSystemUIElements();
    public NewDiscreteUIElements newDiscreteUIElements = new NewDiscreteUIElements();
    public NewContinuousUIElements newContinuousUIElements = new NewContinuousUIElements();

    protected override void OnRegistration()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ActivateMobileUI(bool setActive)
    {
        canvasGroup.alpha = setActive ? 1f : 0f;
        canvasGroup.blocksRaycasts = setActive;
    }

    public void SetInputScheme(InputScheme inputScheme)
    {
        foreach (var uiElements in new SystemUIElements[] { oldSystemUIElements, newDiscreteUIElements, newContinuousUIElements })
        {
            uiElements.canvasGroup.alpha = 0f;
            uiElements.canvasGroup.blocksRaycasts = false;
        }

        switch (inputScheme)
        {
            case InputScheme.Old8Directional:
                oldSystemUIElements.canvasGroup.alpha = 1f;
                oldSystemUIElements.canvasGroup.blocksRaycasts = true;
                break;
            case InputScheme.New8Directional:
                newDiscreteUIElements.canvasGroup.alpha = 1f;
                newDiscreteUIElements.canvasGroup.blocksRaycasts = true;
                break;
            case InputScheme.Continuous:
                newContinuousUIElements.canvasGroup.alpha = 1f;
                newContinuousUIElements.canvasGroup.blocksRaycasts = true;
                break;
            default:
                break;
        }
    }


    public class SystemUIElements
    {
        public CanvasGroup canvasGroup;
    }

    [Serializable]
    public class OldSystemUIElements : SystemUIElements
    {
        public MobileUIJoystick movementJoystick;
        public UIPushButton shootButton;
    }

    [Serializable]
    public class NewDiscreteUIElements : SystemUIElements
    {
        public MobileUIJoystick movementJoystick;
        public MobileUIJoystick aimJoystick;
    }

    [Serializable]
    public class NewContinuousUIElements : SystemUIElements
    {
        public CustomVirtualJoystick movementJoystick;
        public CustomVirtualJoystick aimJoystick;
    }
}