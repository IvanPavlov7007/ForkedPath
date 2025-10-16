using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Game/ProjectilesPattern")]
public class ProjectilesPattern : ScriptableObject
{
    public ProjectileWave[] projectileWaves;

    public float totalDuration
    {
        get
        {
            float total = 0f;
            foreach (var wave in projectileWaves)
            {
                total += wave.delayAfterWave;
            }
            return total;
        }
    }
}

[System.Serializable]
public class ProjectileWave
{
    public float delayAfterWave = 0.5f;
    public ProjectileConfig projectileConfig;

    // New fields for multiple projectiles
    [Header("Multiple Projectiles Settings")]
    public int projectileCount = 1; // How many projectiles to shoot in this wave
    public float angleSpread = 0f;  // Total angle spread in degrees (centered on main direction)
    public float angleOffset = 0f;  // Fixed angle offset per wave
    public Vector2 offset = Vector2.zero;         // Fixed offset per wave
    // New fields
    [Header("Randomization Settings")]
    public float randomAngleRange = 0f;           // Randomize angle +/- this value
    public float randomOffsetRadius = 0f;         // Randomize spawn position within this radius
}