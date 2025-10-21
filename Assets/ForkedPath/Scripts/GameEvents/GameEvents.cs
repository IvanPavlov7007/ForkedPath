using System.Collections;
using UnityEngine;
using System;
using Pixelplacement;

public class GameEvents : Singleton<GameEvents>
{
    public Action<FXEventData> OnFX;
    public Action<DamageEventData> OnDamage;
    public Action<DeathEventData> OnDeath;
    public Action<PlayerEnterTrigger> OnPlayerEnterTrigger;
    public Action<InvincibilityEventData> OnInvincibilityChanged;
    public Action<FallingEventData> OnFallingToDeathStarted;
    public Action<CorpseLandedEventData> OnCorpseLanded;
    public Action<EntitySpawnedEventData> OnEntitySpawned;
    public Action<EatingEventData> OnEntityEaten;

    //Food related events
    public Action<Entity, FoodRulesConfig, EntityFoodType, int> OnFoodUpgrade;
    public Action<Entity> OnLifeReplenish;
    public Action<Entity> OnFoodUpgradeReset;

    // TODO expand later
    public Action OnPlayerLifeChange;
}