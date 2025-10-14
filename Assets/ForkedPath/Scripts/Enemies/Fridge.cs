using UnityEngine;
using Pixelplacement;

public class Fridge : EntityComponent
{
    [SerializeField] Transform collisionChild;

    protected override void OnDeath(DeathEventData deathEventData)
    {
        SpawnDrop();
        Destroy(collisionChild.gameObject);
        if(spriteRenderer != null)
        {
            Tween.Color(spriteRenderer, Color.clear, 0.4f, 0f);
        }
        Destroy(gameObject, 0.5f);
    }

    private void SpawnDrop()
    {
        var fridgeConfig = entity.Config as FridgeConfig;
        if (fridgeConfig == null || fridgeConfig.possibleDrops == null || fridgeConfig.possibleDrops.Length == 0) return;
        var dropConfig = fridgeConfig.possibleDrops[Random.Range(0, fridgeConfig.possibleDrops.Length)];
        if (dropConfig == null) return;
        var dropPosition = entity.transform.position;
        EntitiesSpawnManager.Instance.SpawnEntity(dropConfig, dropPosition);
    }
}
