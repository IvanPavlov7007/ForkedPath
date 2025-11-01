using UnityEngine;

[CreateAssetMenu(menuName = "Game/EntityConfigs/HamburgerBoss")]
public class HamburgerBossConfig : EntityConfig
{
    [Header("Hamburger Boss Specific")]
    public BossPhaseConfig[] phases;
    public AudioClip openingSound;
    public AudioClip closingSound;
    public float deathAnimationDuration = 2f;
    public float deathExplosionRate = 5f;
    [Header("Debug")]
    public bool lowLivesPerPhases = true;
}