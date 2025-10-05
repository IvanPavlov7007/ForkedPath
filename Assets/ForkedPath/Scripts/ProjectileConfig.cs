﻿using System.Collections;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/ProjectileConfig")]
public class ProjectileConfig : ScriptableObject
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;
    public float maxLifetime = 5f;
    public LayerMask layerMask;
    public int damage = 1;

    [Header("FX")]
    public GameObject spawnFX;
    public GameObject impactFX;
    public AudioClip spawnSFX;
}