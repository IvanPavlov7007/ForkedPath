using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Pixelplacement;

public class EndingUI : Singleton<EndingUI>
{
    [SerializeField]
    TextMeshProUGUI EndText;
    
    
    Button restartButton;
    Image buttonImage;
    TextMeshProUGUI buttonText;

    CanvasGroup canvasGroup;

    ITransparencyController groupTransparency;
    ITransparencyController buttonImageTransparency;
    ITransparencyController buttonTextTransparency;
    ITransparencyController endTextTransparency;

    private void Awake()
    {
        restartButton = GetComponentInChildren<Button>();
        buttonImage = restartButton.GetComponent<Image>();
        buttonText = restartButton.GetComponentInChildren<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
        restartButton.onClick.AddListener(()=>GameManager.RestartLevel());
        setActive(false);

        groupTransparency = Transparency.GetController(canvasGroup);
        buttonImageTransparency = Transparency.GetController(buttonImage);
        buttonTextTransparency = Transparency.GetController(buttonText);
        endTextTransparency = Transparency.GetController(EndText);

        groupTransparency.Alpha = 0;
        buttonImageTransparency.Alpha = 0;
        buttonTextTransparency.Alpha = 0;
        endTextTransparency.Alpha = 0;
    }

    void setActive(bool active)
    {
        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
    }

    public Coroutine ShowEndingUI()
    {
        return StartCoroutine(FadeInUI());
    }

    IEnumerator FadeInUI()
    {
        Tween.Value(0f,1f, value => groupTransparency.Alpha = value, 0.2f, 0f, Tween.EaseInOutStrong);
        yield return new WaitForSeconds(1f);
        Tween.Value(0f, 1f, value => endTextTransparency.Alpha = value, 0.2f,0f, Tween.EaseInOutStrong);
        setActive(true);
        Tween.Value(0f, 1f, value => buttonImageTransparency.Alpha = value, 0.2f, 0f, Tween.EaseInOutStrong);
        Tween.Value(0f, 1f, value => buttonTextTransparency.Alpha = value, 0.2f, 0f, Tween.EaseInOutStrong);
    }


}