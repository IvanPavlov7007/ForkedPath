using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Pixelplacement;

public class MobileUI : Singleton<MobileUI>
{
    CanvasGroup canvasGroup;
    MobileUIJoystick joystick;
    [SerializeField]
    Button shootButton;
    [SerializeField]
    CustomToggle lockToggle;

    UIPushButton shootPushButton;

    public MobileUIJoystick Joystick => joystick;
    public UIPushButton ShootButton => shootPushButton;
    public Toggle LockToggle => lockToggle.toggle;


    private void Awake()
    {
        joystick = GetComponentInChildren<MobileUIJoystick>();
        canvasGroup = GetComponent<CanvasGroup>();

        shootPushButton = shootButton.GetComponent<UIPushButton>();
        if(shootPushButton == null)
        {
            shootPushButton = shootButton.gameObject.AddComponent<UIPushButton>();
        }
    }


    public void ActivateMobileUI(bool setActive)
    {
        canvasGroup.alpha = setActive ? 1f : 0f;
        canvasGroup.blocksRaycasts = setActive;
        canvasGroup.interactable = setActive;
    }
}