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

    // Player events
    public Action<Entity> OnPlayerRespawned;
    public Action<int> OnPlayerHit;
    public Action<int> OnPlayerHealed;
    public Action<Entity> OnPlayerDeath;
    public Action<int> OnPlayerLivesChanged;
    public Action OnPlayerFoodConsumed;
    public Action<ProgressionLevel> OnPlayerUpgraded;
    public Action<ProgressionLevel> OnPlayerDowngraded;
    public Action<Entity> OnPlayerUpgradeReset;
    public Action<Entity> OnLifeReplenish;
}