using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class EntityStateDebugOverlay : MonoBehaviour
{
    [Header("Overlay Settings")]
    public float minimalStateDisplayTime = 0.1f;
    public float overlayAlpha = 0.4f;
    public Sprite overlaySprite; // Optional; if null, a shared white sprite will be generated.

    private Entity entity;
    private GameObject overlayGO;
    private SpriteRenderer overlayRenderer;
    private Coroutine colorCoroutine;
    private EntityState lastState;
    private float lastStateChangeTime;

    private static Sprite s_DefaultOverlaySprite;

    private static readonly Color AliveColor       = new Color(0f,   1f,   0f,   1f);   // Green
    private static readonly Color HitColor         = new Color(1f,   1f,   0f,   1f);   // Yellow
    private static readonly Color DeadColor        = new Color(1f,   0f,   0f,   1f);   // Red
    private static readonly Color FallingColor     = new Color(0.5f, 0f,   0.5f, 1f);   // Purple
    private static readonly Color InvincibleColor  = new Color(0f,   0.5f, 1f,   1f);   // Blue
    private static readonly Color DeadFallingColor = new Color(1f,   0.4f, 0.7f, 1f);   // Pink

    void Awake()
    {
        entity = GetComponent<Entity>();
        CreateOverlay();
    }

    void OnEnable()
    {
        entity.StateChanged += OnEntityStateChanged;
        overlayGO.SetActive(true);
        OnEntityStateChanged(entity.CurrentState, default);
    }

    void OnDisable()
    {
        entity.StateChanged -= OnEntityStateChanged;
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
        if (overlayGO != null)
        {
            Destroy(overlayGO);
            overlayGO = null;
        }
    }

    private void CreateOverlay()
    {
        overlayGO = new GameObject("EntityStateDebugOverlay");
        overlayGO.transform.SetParent(transform, false);
        overlayRenderer = overlayGO.AddComponent<SpriteRenderer>();
        overlayRenderer.sprite = overlaySprite != null ? overlaySprite : GenerateDefaultSprite();
        overlayRenderer.sortingOrder = 1000; // Draw above everything
        overlayRenderer.color = Color.clear;
        overlayGO.SetActive(false);

        var bounds = GetEntityBounds();
        overlayGO.transform.localScale = new Vector3(bounds.size.x, bounds.size.y, 1f);
    }

    private static Sprite GenerateDefaultSprite()
    {
        if (s_DefaultOverlaySprite != null) return s_DefaultOverlaySprite;

        const int size = 16;
        var tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        var pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();

        s_DefaultOverlaySprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        return s_DefaultOverlaySprite;
    }

    private Bounds GetEntityBounds()
    {
        var renderers = GetComponentsInChildren<SpriteRenderer>();
        if (renderers.Length == 0) return new Bounds(transform.position, Vector3.one);
        var bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
        return bounds;
    }

    private void OnEntityStateChanged(EntityState state, EntityStateChangeData changeData)
    {
        float timeSinceLastChange = Time.time - lastStateChangeTime;
        lastStateChangeTime = Time.time;

        if (colorCoroutine != null) StopCoroutine(colorCoroutine);

        if (timeSinceLastChange < minimalStateDisplayTime && lastState != state)
        {
            colorCoroutine = StartCoroutine(DelayedColorChange(state, minimalStateDisplayTime - timeSinceLastChange));
        }
        else
        {
            SetOverlayColor(state);
        }

        Debug.Log($"[EntityStateDebugOverlay] {entity.name} state changed to {state}");
        lastState = state;
    }

    private IEnumerator DelayedColorChange(EntityState state, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetOverlayColor(state);
        lastState = state;
    }

    private void SetOverlayColor(EntityState state)
    {
        overlayGO.SetActive(true);
        var col = GetColorForState(state);
        col.a = overlayAlpha;
        overlayRenderer.color = col;
    }

    private Color GetColorForState(EntityState state)
    {
        switch (state)
        {
            case EntityState.Alive:       return AliveColor;
            case EntityState.Hit:         return HitColor;
            case EntityState.Dead:        return DeadColor;
            case EntityState.Falling:     return FallingColor;
            case EntityState.Invincible:  return InvincibleColor;
            case EntityState.DeadFalling: return DeadFallingColor;
            default:                      return Color.clear;
        }
    }
}