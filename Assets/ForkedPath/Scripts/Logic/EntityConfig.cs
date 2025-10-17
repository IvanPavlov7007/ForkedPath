using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/EntityConfig")]
public class EntityConfig : ScriptableObject
{
    public GameObject entityPrefab;
    public string entityID;
    public EntityFoodType initialFoodType = EntityFoodType.None;
    public int maxHealth = 5;
    public float moveSpeed = 5f;
    public int collisionDamage = 1;
    public GameObject deathFX;
    public AudioClip deathSound;
    public int scoreValue = 1; // maybe food value
    public bool corpseOnSpawn = false;

    [Header("Invincibility")]
    public bool invincibleAfterHit = false;
    public float invincibilityDuration = 1f;

    [Header("Hit Reaction")]
    public float hitStunDuration = 0f; // 0 = immediate return to Alive

    [Header("Interaction")]
    public LayerMask interactWithAliveLayers;
}