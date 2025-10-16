using System.Collections;
using UnityEngine;
using System;

[DisallowMultipleComponent]
public sealed class BetweenBoundariesMover : MonoBehaviour
{
    [SerializeField]
    BoundariesType boundariesType = BoundariesType.globalBoundaries;

    [Tooltip("If 'Global', this is a world-space Rect.\nIf 'Camera Boundaries', only the size is used and the rect is centered on the main camera at runtime.")]
    [SerializeField]
    Rect boundaries = new Rect(-5, -3, 10, 6);

    Rigidbody2D rb;

    public event Action destinationReached;

    Vector2 destination; // in world coordinates
    AnimationCurve easeCurve;
    bool moving = false;

    // tween state
    Vector2 startPos;
    float startTime;
    float duration;

    // External provider(s)
    Func<Rect> rectProvider; // If set, overrides serialized settings.

    const float Epsilon = 1e-5f;

    enum BoundariesType
    {
        globalBoundaries,
        cameraBoundaries // rect should be centered on camera
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Move toward the furthest intersection of the ray (from current position and the given direction)
    /// with the configured boundaries. 'speed' is units/sec. 'easeCurve' maps 0..1 -> 0..1.
    /// Intersections behind the direction are ignored; if parallel to an edge, the perpendicular edge is chosen.
    /// </summary>
    public void Move(Vector2 direction, float speed, AnimationCurve easeCurve)
    {
        if (easeCurve == null) easeCurve = AnimationCurve.Linear(0, 0, 1, 1);
        if (speed <= 0f) speed = 0.001f;

        this.easeCurve = easeCurve;

        // Compute destination
        var worldRect = GetWorldRect();
        Vector2 origin = rb != null ? rb.position : (Vector2)transform.position;
        Vector2 dir = direction;
        if (dir.sqrMagnitude < Epsilon) return;
        dir.Normalize();

        if (!TryRaycastRect(origin, dir, worldRect, out destination))
        {
            // No hit; nothing to do
            moving = false;
            return;
        }

        startPos = origin;
        float distance = Vector2.Distance(startPos, destination);
        duration = Mathf.Max(Epsilon, distance / speed);
        startTime = Time.fixedTime;
        moving = true;
    }

    [Obsolete("Use Move(Vector2 direction, float speed, AnimationCurve easeCurve)")]
    public void Move(float directionDegrees, float speed, AnimationCurve easeCurve)
    {
        var dir = new Vector2(Mathf.Cos(directionDegrees * Mathf.Deg2Rad), Mathf.Sin(directionDegrees * Mathf.Deg2Rad));
        Move(dir, speed, easeCurve);
    }

    public void Stop()
    {
        moving = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    void FixedUpdate()
    {
        if (!moving) return;
        if (rb == null)
        {
            moving = false;
            return;
        }

        float t = Mathf.Clamp01((Time.fixedTime - startTime) / duration);
        float eased = easeCurve != null ? easeCurve.Evaluate(t) : t;
        Vector2 target = Vector2.LerpUnclamped(startPos, destination, eased);

        rb.MovePosition(target);

        if (t >= 1f - Mathf.Epsilon)
        {
            rb.MovePosition(destination);
            rb.linearVelocity = Vector2.zero;
            moving = false;
            destinationReached?.Invoke();
        }
    }

    Rect GetWorldRect()
    {
        // External provider takes precedence if set:
        if (rectProvider != null) return rectProvider();

        if (boundariesType == BoundariesType.globalBoundaries)
        {
            return boundaries;
        }

        // Camera-centered rect: boundaries.size is used; center on main camera (XY).
        var cam = Camera.main;
        Vector2 center = cam ? (Vector2)cam.transform.position : Vector2.zero;
        var size = boundaries.size;
        return new Rect(center - size * 0.5f, size);
    }

    static bool TryRaycastRect(Vector2 origin, Vector2 dir, Rect rect, out Vector2 hitPoint)
    {
        hitPoint = default;

        // Collect intersections with rectangle edges: x = xMin/xMax, y = yMin/yMax
        float bestT = float.NegativeInfinity;
        bool found = false;

        float xMin = rect.xMin, xMax = rect.xMax;
        float yMin = rect.yMin, yMax = rect.yMax;

        // Vertical edges
        if (Mathf.Abs(dir.x) > Epsilon)
        {
            // x = xMin
            float t = (xMin - origin.x) / dir.x;
            if (t >= 0f)
            {
                float y = origin.y + dir.y * t;
                if (y >= yMin - Epsilon && y <= yMax + Epsilon)
                {
                    if (t > bestT) { bestT = t; hitPoint = new Vector2(xMin, y); found = true; }
                }
            }

            // x = xMax
            t = (xMax - origin.x) / dir.x;
            if (t >= 0f)
            {
                float y = origin.y + dir.y * t;
                if (y >= yMin - Epsilon && y <= yMax + Epsilon)
                {
                    if (t > bestT) { bestT = t; hitPoint = new Vector2(xMax, y); found = true; }
                }
            }
        }

        // Horizontal edges
        if (Mathf.Abs(dir.y) > Epsilon)
        {
            // y = yMin
            float t = (yMin - origin.y) / dir.y;
            if (t >= 0f)
            {
                float x = origin.x + dir.x * t;
                if (x >= xMin - Epsilon && x <= xMax + Epsilon)
                {
                    if (t > bestT) { bestT = t; hitPoint = new Vector2(x, yMin); found = true; }
                }
            }

            // y = yMax
            t = (yMax - origin.y) / dir.y;
            if (t >= 0f)
            {
                float x = origin.x + dir.x * t;
                if (x >= xMin - Epsilon && x <= xMax + Epsilon)
                {
                    if (t > bestT) { bestT = t; hitPoint = new Vector2(x, yMax); found = true; }
                }
            }
        }

        return found;
    }

    // -------- Boundaries injection API (best practice for config-driven setups) --------

    /// <summary>
    /// Use a fixed world-space Rect for boundaries (injected by code/config).
    /// </summary>
    public void SetBoundaries(Rect rect)
    {
        rectProvider = () => rect;
    }

    /// <summary>
    /// Use a camera-centered Rect of a given size; center updates dynamically each call.
    /// </summary>
    public void SetCameraBoundaries(Vector2 size, Camera cam = null)
    {
        rectProvider = () =>
        {
            var c = cam != null ? cam : Camera.main;
            Vector2 center = c ? (Vector2)c.transform.position : Vector2.zero;
            return new Rect(center - size * 0.5f, size);
        };
    }

    /// <summary>
    /// Removes externally injected boundaries so the component falls back to its serialized settings.
    /// </summary>
    public void ClearExternalBoundaries()
    {
        rectProvider = null;
    }

#if UNITY_EDITOR
    // Visualize boundaries in editor and during play (when selected)
    void OnDrawGizmosSelected()
    {
        var rect = GetWorldRect();
        Vector3 a = new Vector3(rect.xMin, rect.yMin, 0);
        Vector3 b = new Vector3(rect.xMax, rect.yMin, 0);
        Vector3 c = new Vector3(rect.xMax, rect.yMax, 0);
        Vector3 d = new Vector3(rect.xMin, rect.yMax, 0);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);
    }
#endif
}