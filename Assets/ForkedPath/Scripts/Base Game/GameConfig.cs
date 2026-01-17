using System.Collections;
using UnityEngine;
using Pixelplacement;

[CreateAssetMenu(menuName ="Game/GameConfig")]
public class GameConfig : SingletonScriptableObject<GameConfig>
{
    public EntityConfig EntityFallbackConfig;
    public ProjectileConfig ProjectileConfig;
    public InputScheme InputScheme = InputScheme.Continuous;
}