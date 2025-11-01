using System.Collections;
using UnityEngine;
using Pixelplacement;
using System;

public class Player : Singleton<Player>
{
    [Header("Meta")]
    public int lives = 3;

    [Header("Spawning")]
    [SerializeField] EntityConfig basePlayerConfig;
    public Transform spawnPoint;
    [SerializeField] float respawnDelay = 1.25f;

    [Header("Runtime")]
    [SerializeField] Entity currentAvatar;

    public Entity CurrentAvatar => currentAvatar;

    public static bool IsEntityActivePlayer(Entity entity)
    {
        return Instance != null && Instance.currentAvatar == entity;
    }

    public void healPlayer(int amount)
    {
        if(currentAvatar != null && currentAvatar.Health != null)
        {
            currentAvatar.Health.Heal(amount);
            GameEvents.Instance?.OnPlayerHealed?.Invoke(amount);
        }
    }

    void OnEnable()
    {
        // Listen to global death events so we can detect when the player's avatar dies
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnDeath += onEntityDeath;
            GameEvents.Instance.OnDamage += onEntityHit;
        }
    }

    void OnDisable()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnDeath -= onEntityDeath;
            GameEvents.Instance.OnDamage -= onEntityHit;
        }
    }

    void Start()
    {
        // Spawn initial avatar if not present in the scene
        if (currentAvatar == null)
        {
            SpawnBaseAvatar(spawnPoint != null ? (Vector2) spawnPoint.position : Vector3.zero);
        }
        else
            GameEvents.Instance?.OnPlayerRespawned?.Invoke(currentAvatar);
    }

    private void SpawnBaseAvatar(Vector3 vector3)
    {
        var newEntity = EntitiesSpawnManager.Instance.SpawnEntity(basePlayerConfig, vector3);
        newEntity.transform.parent = this.transform;
        currentAvatar = newEntity;
        GameEvents.Instance?.OnPlayerRespawned?.Invoke(newEntity);
    }

    public bool ColliderIsPlayer(Collider2D collider)
    {
        if (currentAvatar == null || collider == null) return false;
        var entity = collider.GetComponentInParent<Entity>();
        return entity != null && entity == currentAvatar;
    }

    void onEntityHit(DamageEventData e)
    {
        if(e== null || e.target == null) return;
        if(currentAvatar == null) return;

        if(e.target != currentAvatar.Health as IDamageable) return;

        GameEvents.Instance?.OnPlayerHit?.Invoke(currentAvatar.Health.CurrentHealth);
    }

    void onEntityDeath(DeathEventData e)
    {
        if (e == null || e.entity == null) return;
        if (currentAvatar == null) return;

        // Only react if the dead entity is the player's current avatar
        if (e.entity != currentAvatar) return;

        GameEvents.Instance.OnPlayerDeath?.Invoke(e.entity);

        // Let the corpse persist to be eaten according to your Health/corpse pipeline.
        currentAvatar = null;

        if (lives > 0)
        {
            lives--;
            GameEvents.Instance?.OnPlayerLivesChanged?.Invoke(lives);
            StartCoroutine(RespawnAfterDelay());
        }
        else
        {
            GameEvents.Instance?.OnPlayerGameOver?.Invoke();
        }
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnBaseAvatar(spawnPoint != null ? spawnPoint.position : Vector3.zero);
    }
}