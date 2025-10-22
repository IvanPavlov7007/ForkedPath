using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressionUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI foodLevelText;
    [SerializeField]
    TextMeshProUGUI foodCountText;
    [SerializeField]
    Image roundsInfo;
    [SerializeField]
    Image foodTypeUI;
    [SerializeField]
    Image foodTypeIcon;

    [SerializeField]
    Sprite meatIcon;
    [SerializeField]
    Sprite veggieIcon;
    [SerializeField]
    Sprite normalIcon;

    [SerializeField, Range(0f,1f)]
    float roundInfoMaxFill = 0.5f;
    [SerializeField]
    Color normalColor = Color.blue;
    [SerializeField]
    Color meatColor = new Color(1f, 0.4f, 0.0f);
    [SerializeField]
    Color veggieColor = Color.green;

    private void OnEnable()
    {
        GameEvents.Instance.OnPlayerFoodConsumed += onPlayerFoodConsumed;
        GameEvents.Instance.OnPlayerUpgraded += onPlayerUpgraded;
        GameEvents.Instance.OnPlayerDowngraded += onPlayerDowngraded;
        GameEvents.Instance.OnPlayerUpgradeReset += onFoodUpgradeReset;
        ProgressionManager.Instance.onAmoChanged += onRemainingAmoChanged;
    }

    private void OnDisable()
    {
        GameEvents.Instance.OnPlayerFoodConsumed -= onPlayerFoodConsumed;
        GameEvents.Instance.OnPlayerUpgraded -= onPlayerUpgraded;
        GameEvents.Instance.OnPlayerDowngraded -= onPlayerDowngraded;
        GameEvents.Instance.OnPlayerUpgradeReset -= onFoodUpgradeReset;
        ProgressionManager.Instance.onAmoChanged -= onRemainingAmoChanged;
    }

    void onPlayerFoodConsumed()
    {
        if (ProgressionManager.Instance.CurrentComboTracker.TryPeekNextThreshold(out int threshold, out int remaining))
        {
            foodTypeUI.fillAmount = (float)(threshold - remaining) / threshold;
        }
        else
        {
            foodTypeUI.fillAmount = 1f;
        }
        redrawFood();
        
    }

    void onPlayerUpgraded(ProgressionLevel level)
    {
        foodLevelText.text = "Level " + level.level;
    }

    void onPlayerDowngraded(ProgressionLevel level)
    {
        foodLevelText.text = "Level " + level.level;
    }

    void onRemainingAmoChanged(bool capped, int remaining, int max)
    {
        if (capped)
        {
            roundsInfo.fillAmount = roundInfoMaxFill * (float)remaining / max;
        }
        else
            roundsInfo.fillAmount = 0f;
    }


    void onFoodUpgradeReset(Entity entity)
    {
        redrawFood();
        foodLevelText.text = "Level " + 0;
    }

    void redrawFood()
    {
        Color color = normalColor;
        Sprite sprite = normalIcon;
        switch (ProgressionManager.Instance.CurrentComboTracker.CurrentType)
        {
            case EntityFoodType.Vegetable:
                color = veggieColor;
                sprite = veggieIcon;
                break;
            case EntityFoodType.Meat:
                color = meatColor;
                sprite = meatIcon;
                break;
            default:
                break;
        }
        foodTypeIcon.sprite = sprite;
        foodTypeUI.color = color;
        roundsInfo.color = color;
        int count = ProgressionManager.Instance.CurrentComboTracker.CurrentCount;
        foodCountText.text = count.ToString();
    }
}