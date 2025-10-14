using Pixelplacement;
using System.Collections.Generic;
using UnityEngine;

public class CorpseHintManager : Singleton<CorpseHintManager>
{
    [Header("Icons per Food Type")]
    [Tooltip("Icon for EntityFoodType.None")]
    public Sprite iconNone;
    [Tooltip("Icon for EntityFoodType.Vegetable")]
    public Sprite iconVegetable;
    [Tooltip("Icon for EntityFoodType.Meat")]
    public Sprite iconMeat;
    [Tooltip("Icon for EntityFoodType.NotEdible")]
    public Sprite iconNotEdible;

    [Header("Icon Visuals")]
    [Tooltip("Local offset from the entity position where the icon should appear.")]
    public Vector3 worldOffset = new Vector3(0f, 0.75f, 0f);
    [Tooltip("Uniform scale of the icon object.")]
    public float iconScale = 1f;
    [Tooltip("Sorting layer for the icon renderer.")]
    public string sortingLayerName = "Default";
    [Tooltip("Sorting order for the icon renderer (higher renders on top).")]
    public int sortingOrder = 1000;
    [Tooltip("Tint color for the icon.")]
    public Color color = Color.white;

    private readonly Dictionary<Entity, GameObject> _icons = new Dictionary<Entity, GameObject>();

    private void OnEnable()
    {
        GameEvents.Instance.OnDeath += OnEntityDeath;
        GameEvents.Instance.OnEntitySpawned += OnEntitySpawn;
        // If a corpse lands after falling, show the icon then.
        GameEvents.Instance.OnCorpseLanded += OnCorpseLanded;
    }

    private void OnDisable()
    {
        GameEvents.Instance.OnDeath -= OnEntityDeath;
        GameEvents.Instance.OnEntitySpawned -= OnEntitySpawn;
        GameEvents.Instance.OnCorpseLanded -= OnCorpseLanded;

        // Clean up any icons we created
        foreach (var kvp in _icons)
        {
            if (kvp.Value != null) Destroy(kvp.Value);
        }
        _icons.Clear();
    }

    void OnEntityDeath(DeathEventData deathEventData)
    {
        var entity = deathEventData != null ? deathEventData.entity : null;
        if (entity == null) return;
        TryCreateIcon(entity);
    }

    void OnEntitySpawn(EntitySpawnedEventData entitySpawnData)
    {
        var entity = entitySpawnData != null ? entitySpawnData.entity : null;
        if (entity == null) return;

        if (IsEntityDead(entity))
        {
            TryCreateIcon(entity);
        }
        else
        {
            RemoveIcon(entity);
        }
    }

    void OnCorpseLanded(CorpseLandedEventData e)
    {
        if (e == null || e.entity == null) return;
        TryCreateIcon(e.entity);
    }

    private bool IsEntityDead(Entity entity)
    {
        if (entity == null) return false;
        // Prefer Health flag, but also check state to be safe
        if (entity.Health != null && entity.Health.IsDead) return true;
        return entity.CurrentState == EntityState.Dead || entity.CurrentState == EntityState.DeadFalling;
    }

    private void TryCreateIcon(Entity entity)
    {
        if (entity == null) return;

        // If we already created one, ensure it's using the correct sprite and bail.
        if (_icons.TryGetValue(entity, out var existing) && existing != null)
        {
            UpdateIconSprite(existing, GetSpriteForFoodType(entity.foodType));
            return;
        }

        var sprite = GetSpriteForFoodType(entity.foodType);
        if (sprite == null) return; // Nothing to show for this type.

        var go = new GameObject("CorpseIcon");
        go.transform.SetParent(entity.transform, false);
        go.transform.localPosition = worldOffset;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one * Mathf.Max(0.0001f, iconScale);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = color;
        sr.sortingLayerName = sortingLayerName;
        sr.sortingOrder = sortingOrder;
        
        go.AddComponent<IconAnimation>();

        _icons[entity] = go;
    }

    private void RemoveIcon(Entity entity)
    {
        if (entity == null) return;
        if (_icons.TryGetValue(entity, out var icon))
        {
            if (icon != null) Destroy(icon);
            _icons.Remove(entity);
        }
    }

    private void UpdateIconSprite(GameObject iconGo, Sprite sprite)
    {
        if (iconGo == null) return;
        var sr = iconGo.GetComponent<SpriteRenderer>();
        if (sr == null) return;
        sr.sprite = sprite;
    }

    private Sprite GetSpriteForFoodType(EntityFoodType type)
    {
        switch (type)
        {
            case EntityFoodType.None: return iconNone;
            case EntityFoodType.Vegetable: return iconVegetable;
            case EntityFoodType.Meat: return iconMeat;
            case EntityFoodType.NotEdible: return iconNotEdible;
            default: return null;
        }
    }
}