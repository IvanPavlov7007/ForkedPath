using UnityEngine;
using System.Linq;

public sealed class FoodComboTracker
{
    public EntityFoodType CurrentType { get; private set; } = EntityFoodType.None;
    public int CurrentCount { get; private set; } = 0;
    public int CurrentLevel { get; private set; } = 0;

    FoodRulesConfig foodRulesConfig;
    Entity entity;

    public FoodComboTracker(Entity entity, FoodRulesConfig foodSchemeConfig)
    {
        this.entity = entity;
        this.foodRulesConfig = foodSchemeConfig;
    }

    public void Collect(EntityFoodType foodType, int count)
    {
        for(int i = 0; i < count; i++)
        {
            Collect(foodType);
        }
    }

    public void Degrade()
    {
        if (CurrentLevel > 0)
        {
            CurrentLevel--;
            GameEvents.Instance.OnFoodUpgrade?.Invoke(entity, CurrentType, CurrentLevel);
        }
        else
        {
            Debug.LogError("Cannot degrade food level below 0");
        }
        CheckForUpgrade(); // to check if we still can get back using up the current count
    }

    public void Collect(EntityFoodType foodType)
    {
        if (CurrentType == EntityFoodType.None)
        {
            // Start new streak
            CurrentType = foodType;
            CurrentCount = 1;
            return;
        }

        // Mixing case
        if (foodType != CurrentType)
        {
            if (CurrentCount >= 1) // collected one of each
                GameEvents.Instance.OnLifeReplenish?.Invoke(entity);

            StateReset();
            return;
        }

        // Same type
        CurrentCount++;
        CheckForUpgrade();
    }

    public int OverallCollectedCount()
    {
        int total = 0;
        var rule = foodRulesConfig.Rules.First(r => r.Type == CurrentType);
        if (rule != null)
        {
            for (int i = 0; i < CurrentLevel; i++)
            {
                total += rule.Thresholds[i];
            }
            total += CurrentCount;
        }
        return total;
    }

    private void CheckForUpgrade()
    {
        var rule = foodRulesConfig.Rules.First(r => r.Type == CurrentType);
        if (rule == null) return;

        if (CurrentLevel < rule.Thresholds.Length &&
            CurrentCount >= rule.Thresholds[CurrentLevel])
        {
            CurrentCount -= rule.Thresholds[CurrentLevel];
            CurrentLevel++;
            GameEvents.Instance.OnFoodUpgrade?.Invoke(entity, CurrentType, foodRulesConfig, CurrentLevel);
        }
    }

    private void StateReset()
    {
        CurrentType = EntityFoodType.None;
        CurrentCount = 0;
        CurrentLevel = 0;
        GameEvents.Instance.OnFoodUpgradeReset?.Invoke(entity);
    }

}