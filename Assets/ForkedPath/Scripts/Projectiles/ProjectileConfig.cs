using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/ProjectileConfig")]
public class ProjectileConfig : BaseConfig
{
    [Title("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;
    public float maxLifetime = 5f;
    public LayerMask layerMask;
    public int damage = 1;

    [Header("Additional FX")]
    public Color color = Color.white;

    public override BaseConfig FallbackConfig => GameConfig.Instance.ProjectileConfig;
}