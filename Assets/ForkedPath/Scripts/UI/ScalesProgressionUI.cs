using System;
using TMPro;
using UnityEngine;

public class ScalesProgressionUI : MonoBehaviour
{
    [SerializeField]
    ScalesUI scalesUI;
    [SerializeField]
    TMP_Text ammoCount;

    private void OnEnable()
    {
        GameEvents.Instance.OnPlayerUpgraded += onPlayerProgressionChanged;
        GameEvents.Instance.OnPlayerDowngraded += onPlayerProgressionChanged;
        GameEvents.Instance.OnPlayerUpgradeReset += onPlayerUpgradeReset;
        GameEvents.Instance.OnPlayerFoodConsumed += onPlayerFoodConsumed;
        ProgressionManager.Instance.onAmmoChanged += onAmmoChanged;

        redraw();
    }

    private void onAmmoChanged(bool capped, int remaining, int max)
    {
        if (ammoCount == null)
        {
            ammoCount.text = "";
            return;
        }

        // Infinite ammo (ProgressionManager sets remaining to int.MaxValue when uncapped)
        if (!capped)
        {
            ammoCount.text = "∞";
            return;
        }

        // Defensive: avoid showing "/0" or negative values if something is misconfigured
        if (max <= 0)
        {
            ammoCount.text = Mathf.Max(0, remaining).ToString();
            return;
        }

        remaining = Mathf.Clamp(remaining, 0, max);
        ammoCount.text = remaining + "\n/" + max;
    }

    private void onPlayerProgressionChanged(ProgressionLevel level)
    {
        redraw();
    }

    private void onPlayerUpgradeReset(Entity entity)
    {
        redraw();
    }

    private void onPlayerFoodConsumed()
    {
        redraw();
    }

    private void redraw()
    {
        if (scalesUI == null)
            return;

        var tracker = ProgressionManager.Instance.CurrentComboTracker;
        if (tracker == null || tracker.CurrentType == EntityFoodType.None)
        {
            scalesUI.ResetValues();
            return;
        }

        int value = tracker.OverallCollectedCount();

        switch (tracker.CurrentType)
        {
            case EntityFoodType.Meat:
                scalesUI.SetUpValue(value, ProgressionManager.Instance.MaxCollectedMeat());
                break;

            case EntityFoodType.Vegetable:
                scalesUI.SetDownValue(value, ProgressionManager.Instance.MaxCollectedVeggies());
                break;

            default:
                scalesUI.ResetValues();
                break;
        }
    }

    private void OnDisable()
    {
        GameEvents.Instance.OnPlayerUpgraded -= onPlayerProgressionChanged;
        GameEvents.Instance.OnPlayerDowngraded -= onPlayerProgressionChanged;
        GameEvents.Instance.OnPlayerUpgradeReset -= onPlayerUpgradeReset;
        GameEvents.Instance.OnPlayerFoodConsumed -= onPlayerFoodConsumed;
        ProgressionManager.Instance.onAmmoChanged -= onAmmoChanged;
    }
}