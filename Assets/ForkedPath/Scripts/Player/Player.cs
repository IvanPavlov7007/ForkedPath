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

    void OnEnable()
    {
        // Listen to global death events so we can detect when the player's avatar dies
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnDeath += onPlayerDeath;
        }
    }

    void OnDisable()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnDeath -= onPlayerDeath;
        }
    }

    void Start()
    {
        // Spawn initial avatar if not present in the scene
        if (currentAvatar == null)
        {
            SpawnBaseAvatar(spawnPoint != null ? spawnPoint.position : Vector3.zero);
        }
    }

    private void SpawnBaseAvatar(Vector3 vector3)
    {
        var newEntity = EntitiesSpawnManager.Instance.SpawnEntity(basePlayerConfig, vector3);
        newEntity.transform.parent = this.transform;
        currentAvatar = newEntity;
    }

    public bool ColliderIsPlayer(Collider2D collider)
    {
        if (currentAvatar == null || collider == null) return false;
        var entity = collider.GetComponentInParent<Entity>();
        return entity != null && entity == currentAvatar;
    }

    void onPlayerDeath(DeathEventData e)
    {
        if (e == null || e.entity == null) return;
        if (currentAvatar == null) return;

        // Only react if the dead entity is the player's current avatar
        if (e.entity != currentAvatar) return;

        // Let the corpse persist to be eaten according to your Health/corpse pipeline.
        currentAvatar = null;

        if (lives > 0)
        {
            lives--;
            StartCoroutine(RespawnAfterDelay());
        }
        else
        {
            // TODO: trigger game over flow (UI, input lock, etc.)
            Debug.Log("Player: no lives left. Game Over.");
        }
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnBaseAvatar(spawnPoint != null ? spawnPoint.position : Vector3.zero);
    }
}