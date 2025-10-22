using UnityEngine;
using System.Linq;
using System;

public sealed class FoodComboTracker
{
    public Action<EntityFoodType,int> OnUpgraded;
    public Action<EntityFoodType, int> OnDowngraded;
    public Action OnReset;
    public Action OnHeal;

    public EntityFoodType CurrentType { get; private set; } = EntityFoodType.None;
    public int CurrentCount { get; private set; } = 0;
    public int CurrentLevel { get; private set; } = 0;

    public readonly FoodRulesConfig foodRulesConfig;

    public FoodComboTracker(FoodRulesConfig foodSchemeConfig)
    {
        this.foodRulesConfig = foodSchemeConfig;
    }

    public void Degrade()
    {
        if (CurrentLevel <= 0)
        {
            Debug.LogWarning("Cannot degrade food level below 0");
            return;
        }

        CurrentLevel--;
        if(CurrentLevel < 1 && CurrentCount < 1)
            CurrentType = EntityFoodType.None;
        OnDowngraded?.Invoke(CurrentType, CurrentLevel);

        if (CurrentCount > 0)
            while (CheckForUpgrade()) { }
    }

    public void Collect(EntityFoodType foodType, int count, Action OnCollect)
    {
        if(count <= 0)
        {
            Debug.LogWarning("Collect called with non-positive count: " + count);
            return;
        }

        if (CurrentType == EntityFoodType.None)
        {
            // Start new streak
            CurrentType = foodType;
        }

        // Mixing case
        if (foodType != CurrentType)
        {
            if (CurrentCount > 0)
            {
                OnHeal?.Invoke();
                StateReset();
                if (count > 1)
                {
                    Collect(foodType, count - 1, OnCollect);
                }
            }
            else
            {
                StateReset();
                Collect(foodType, count, OnCollect);
            }
            return;
        }

        // Same type
        CurrentCount += count;
        OnCollect?.Invoke();
        while (CheckForUpgrade()) { }
    }

    public int OverallCollectedCount()
    {
        int total = 0;
        var rule = foodRulesConfig.Rules.FirstOrDefault(r => r.Type == CurrentType);
        if (rule != null)
        {
            for (int i = 0; i < CurrentLevel; i++)
            {
                total += rule.Thresholds[i];
            }
            total += CurrentCount;
        }
        else
            Debug.Log("No food rule found for type " + CurrentType);
        return total;
    }

    /// <summary>
    /// Returns the next threshold value for the current type, and how many more items are needed.
    /// Returns false if there is no higher level available.
    /// </summary>
    public bool TryPeekNextThreshold(out int nextThreshold, out int remainingToNext)
    {
        nextThreshold = 0;
        remainingToNext = 0;

        // No active combo → no threshold to peek
        if (CurrentType == EntityFoodType.None)
            return false;

        // Lookup rule safely
        var rule = foodRulesConfig.Rules.FirstOrDefault(r => r.Type == CurrentType);
        if (rule == null) return false;

        // Already at max level
        if (CurrentLevel >= rule.Thresholds.Length)
            return false;

        nextThreshold = rule.Thresholds[CurrentLevel];
        remainingToNext = Mathf.Max(0, nextThreshold - CurrentCount);
        return true;
    }

    private bool CheckForUpgrade()
    {
        var rule = foodRulesConfig.Rules.FirstOrDefault(r => r.Type == CurrentType);
        if (rule == null) return false;

        if (CurrentLevel < rule.Thresholds.Length &&
            CurrentCount >= rule.Thresholds[CurrentLevel])
        {
            CurrentCount -= rule.Thresholds[CurrentLevel];
            CurrentLevel++;
            OnUpgraded?.Invoke(CurrentType, CurrentLevel);
            return true;
        }
        return false;
    }

    private void StateReset()
    {
        CurrentType = EntityFoodType.None;
        CurrentCount = 0;
        CurrentLevel = 0;
        OnReset?.Invoke();
    }

}