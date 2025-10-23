using Pixelplacement;
using System;
using System.Collections;
using UnityEngine;

public sealed class ProgressionManager : Singleton<ProgressionManager>
{
    public FoodRulesConfig FoodRulesConfig => foodRulesConfig;
    public ProgressionLevel CurrentProgressionLevel { get; private set; }
    public FoodComboTracker CurrentComboTracker { get; private set; }
    public int currentAmmo { get; private set; }
    public bool currentAmmoCapped { get; private set; }

    public Action<bool, int, int> onAmoChanged;


    [Header("Food Progression")]
    [SerializeField] FoodRulesConfig foodRulesConfig;
    [SerializeField] ProgressionConfig foodProgressionConfig;
    [Header("On Death")]
    [Range(0f, 1f)]
    [SerializeField] float foodPartKeep = 0.3f;

    private void OnEnable()
    {
        GameEvents.Instance.OnPlayerRespawned += onPlayerRespawned;
        GameEvents.Instance.OnPlayerDeath += onPlayerDeath;
        GameEvents.Instance.OnEntityEaten += onEat;
    }

    private void OnDisable()
    {
        GameEvents.Instance.OnPlayerRespawned -= onPlayerRespawned;
        GameEvents.Instance.OnPlayerDeath -= onPlayerDeath;
        GameEvents.Instance.OnEntityEaten -= onEat;
    }

    void onPlayerRespawned(Entity entity)
    {
        var holder = entity.gameObject.AddComponent<FoodHolder>();
        //entity.gameObject.AddComponent<CharacterColor>();
        createNewFoodTracker();
        holder.Initialize(CurrentComboTracker);
        onFoodReset();//base state;
        entity.GetComponent<AutomaticShooter>().OnShoot += AutomaticShooterOnShot;
        
    }

    void createNewFoodTracker()
    {
        CurrentComboTracker = new FoodComboTracker(foodRulesConfig);
        CurrentComboTracker.OnUpgraded += onFoodUpgraded;
        CurrentComboTracker.OnDowngraded += onFoodDowngraded;
        CurrentComboTracker.OnReset += onFoodReset;
        CurrentComboTracker.OnHeal += onHeal;
    }

    void releaseCurrentComboTracker()
    {
        CurrentComboTracker.OnUpgraded -= onFoodUpgraded;
        CurrentComboTracker.OnDowngraded -= onFoodDowngraded;
        CurrentComboTracker.OnReset -= onFoodReset;
        CurrentComboTracker.OnHeal -= onHeal;
    }

    void onPlayerDeath(Entity entity)
    {
        releaseCurrentComboTracker();
        entity.GetComponent<AutomaticShooter>().OnShoot -= AutomaticShooterOnShot;
    }

    void onEat(EatingEventData e)
    {
        if (!Player.IsEntityActivePlayer(e.eater) || e.prey == null) return;
        var otherHolder = e.prey.GetComponent<FoodHolder>();

        EntityFoodType foodType;
        int foodCount = 1;

        if (otherHolder != null && otherHolder.FoodComboTracker != null)
        {
            var otherType = otherHolder.FoodComboTracker.CurrentType;
            foodType = otherType != EntityFoodType.None ? otherType : e.prey.foodType;
            foodCount = Mathf.Max(1, Mathf.FloorToInt(otherHolder.FoodComboTracker.OverallCollectedCount() * foodPartKeep));
        }
        else
        {
            foodType = e.prey.foodType;
        }

        CurrentComboTracker.Collect(foodType, foodCount, null);
        e.eater.foodType = foodType;//setting last eaten food type, for a case player dies
        GameEvents.Instance.OnPlayerFoodConsumed?.Invoke();

    }


    void AutomaticShooterOnShot()
    {
        if (currentAmmoCapped)
        {
            currentAmmo--;
            onAmoChanged?.Invoke(currentAmmoCapped, currentAmmo, CurrentProgressionLevel.maxAmmo);
        }
        if (currentAmmo <= 0)
        {
            CurrentComboTracker.Degrade();
            GameEvents.Instance.OnPlayerFoodConsumed?.Invoke();
        }
    }
    void onFoodUpgraded(EntityFoodType type, int level)
    {
        setProgressionState(type, level);
        GameEvents.Instance.OnPlayerUpgraded?.Invoke(CurrentProgressionLevel);
    }

    void onFoodDowngraded(EntityFoodType type, int level)
    {
        setProgressionState(type,level);
        GameEvents.Instance.OnPlayerDowngraded?.Invoke(CurrentProgressionLevel);
    }

    void onFoodReset()
    {
        setProgressionState(EntityFoodType.None, 0);//base state;
        GameEvents.Instance.OnPlayerUpgradeReset?.Invoke(Player.Instance.CurrentAvatar);
    }

    void onHeal()
    {
        Player.Instance.healPlayer(1);
    }

    void setProgressionState(EntityFoodType foodType, int level)
    {
        CurrentProgressionLevel = foodProgressionConfig.GetProgressionLevel(foodType, level);
        if (CurrentProgressionLevel == null)
        {
            Debug.LogError("ProgressionManager: No progression level found for food type " + foodType + " at level " + level);
            return;
        }

        //Player.Instance.CurrentAvatar.GetComponent<CharacterColor>()?.SetColor(CurrentProgressionLevel.color);
        AutomaticShooter.ReloadAutomaticShooter(Player.Instance.CurrentAvatar.gameObject, CurrentProgressionLevel.projectilesPattern);
        currentAmmoCapped = CurrentProgressionLevel.amoCapped;
        currentAmmo = currentAmmoCapped ? CurrentProgressionLevel.maxAmmo : int.MaxValue;
        onAmoChanged?.Invoke(currentAmmoCapped, currentAmmo, CurrentProgressionLevel.maxAmmo);
    }
}