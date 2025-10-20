using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Configs/FridgeConfig")]
public class FridgeConfig : EntityConfig
{
    [Header("Fridge Config")]
    public EntityConfig[] possibleDrops;

    //private void OnValidate()
    //{
    //    if (possibleDrops == null) return;
    //    foreach(var drop in possibleDrops)
    //    {
    //        if (drop != null && !drop.corpseOnSpawn)
    //        {
    //            Debug.LogWarning($"Fridge possible drop {drop.name} does not have corpseOnSpawn enabled. This may lead to creating a living enemy.");
    //        }
    //    }
    //}
}