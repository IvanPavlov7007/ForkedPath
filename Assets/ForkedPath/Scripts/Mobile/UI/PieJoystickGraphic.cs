using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Pie Joystick Graphic")]
public class PieJoystickGraphic : MaskableGraphic
{
    [Header("Colors")]
    public Color baseColor = new Color(1f, 1f, 1f, 0.2f);
    public Color highlightColor = new Color(1f, 1f, 1f, 0.9f);
    public bool showBackground = true;
    public Color backgroundColor = new Color(1f, 1f, 1f, 0.08f);

    [Header("Dead Zone (visual)")]
    public bool showDeadZone = true;
    [Range(0f, 0.49f)] public float deadZoneRadiusNormalized = 0.15f;
    public bool deadZoneOnTop = false;
    public Color deadZoneColor = new Color(1f, 0f, 0f, 0.12f);

    [Header("Geometry")]
    [Tooltip("Higher = smoother circle, more verts. 6-10 is usually fine.")]
    [Range(2, 32)] public int segmentsPerSlice = 8;

    [Header("Animation")]
    public bool animateSelected = true;
    [Range(0f, 0.2f)] public float pulseScale = 0.05f;
    public float pulseSpeed = 6f;

    [Header("Binding")]
    [Tooltip("Auto-bind to a MobileUIJoystick on the same GameObject.")]
    public bool autoBindToJoystick = true;
    public MobileUIJoystick joystick;

    [Tooltip("Manual override. -1 = none, otherwise 0..7")]
    [Range(-1, 7)] public int selectedIndex = -1;

    private MobileUIJoystick.Direction8 _lastDir = MobileUIJoystick.Direction8.None;
    private float _lastPulseScaleApplied;

    protected override void Awake()
    {
        base.Awake();
        if (autoBindToJoystick && joystick == null)
            joystick = GetComponent<MobileUIJoystick>();
        raycastTarget = true; // ensures pointer events reach this UI element
    }

    private void Update()
    {
        // Track joystick direction (optional)
        if (joystick != null)
        {
            var d = joystick.CurrentDirection;
            if (d != _lastDir)
            {
                _lastDir = d;
                selectedIndex = (d == MobileUIJoystick.Direction8.None) ? -1 : (int)d;
                SetVerticesDirty();
            }
        }

        if (animateSelected && selectedIndex >= 0)
        {
            float pulse = 1f + Mathf.Sin(Time.unscaledTime * pulseSpeed) * pulseScale;
            if (!Mathf.Approximately(pulse, _lastPulseScaleApplied))
            {
                _lastPulseScaleApplied = pulse;
                SetVerticesDirty();
            }
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Rect rect = GetPixelAdjustedRect();
        Vector2 center = rect.center;
        float radius = 0.5f * Mathf.Min(rect.width, rect.height);

        // Background disc
        if (showBackground)
        {
            AddDisc(vh, center, radius, 8 * segmentsPerSlice, backgroundColor);
        }

        // Dead zone disc (under slices if desired)
        if (showDeadZone && !deadZoneOnTop)
        {
            float rDead = Mathf.Clamp01(deadZoneRadiusNormalized) * radius;
            AddDisc(vh, center, rDead, 6 * segmentsPerSlice, deadZoneColor);
        }

        // 8 slices, aligned with joystick math:
        // 0 deg = Up, clockwise positive.
        for (int i = 0; i < 8; i++)
        {
            float centerDeg = i * 45f;
            float startDeg = centerDeg - 22.5f;
            float endDeg = centerDeg + 22.5f;

            // Pulse only the selected wedge by slightly increasing its outer radius.
            float rOuter = radius;
            if (animateSelected && i == selectedIndex)
            {
                float pulse = 1f + Mathf.Sin(Time.unscaledTime * pulseSpeed) * pulseScale;
                rOuter = radius * pulse;
            }

            Color col = (i == selectedIndex) ? highlightColor : baseColor;
            AddSector(vh, center, 0f, rOuter, startDeg, endDeg, segmentsPerSlice, col);
        }

        // Dead zone disc (on top if desired)
        if (showDeadZone && deadZoneOnTop)
        {
            float rDead = Mathf.Clamp01(deadZoneRadiusNormalized) * radius;
            AddDisc(vh, center, rDead, 6 * segmentsPerSlice, deadZoneColor);
        }
    }

    // Draws a filled circular sector (ring from innerRadius to outerRadius). For innerRadius=0 it’s a pie wedge.
    private static void AddSector(VertexHelper vh, Vector2 center, float innerRadius, float outerRadius,
                                  float startDeg, float endDeg, int segments, Color color)
    {
        // Ensure proper ordering (clockwise from Up)
        float total = endDeg - startDeg;
        if (segments < 1) segments = 1;
        float step = total / segments;

        // Precompute center vertex if innerRadius == 0
        bool useCenter = innerRadius <= 0.0001f;
        int baseIndex = vh.currentVertCount;

        for (int s = 0; s < segments; s++)
        {
            float a0 = startDeg + s * step;
            float a1 = startDeg + (s + 1) * step;

            Vector2 o0 = center + DirFromDeg(a0) * outerRadius;
            Vector2 o1 = center + DirFromDeg(a1) * outerRadius;

            if (useCenter)
            {
                // Triangle: center, o0, o1
                AddVert(vh, center, color);
                AddVert(vh, o0, color);
                AddVert(vh, o1, color);
                int i0 = baseIndex + s * 3;
                vh.AddTriangle(i0 + 0, i0 + 1, i0 + 2);
            }
            else
            {
                Vector2 i0v = center + DirFromDeg(a0) * innerRadius;
                Vector2 i1v = center + DirFromDeg(a1) * innerRadius;

                // Quad (two triangles): i0v, o0, o1, i1v
                AddVert(vh, i0v, color);
                AddVert(vh, o0, color);
                AddVert(vh, o1, color);
                AddVert(vh, i1v, color);

                int i = baseIndex + s * 4;
                vh.AddTriangle(i + 0, i + 1, i + 2);
                vh.AddTriangle(i + 0, i + 2, i + 3);
            }
        }
    }

    // Draws a filled disc by fan
    private static void AddDisc(VertexHelper vh, Vector2 center, float radius, int segments, Color color)
    {
        if (segments < 3) segments = 3;
        int baseIndex = vh.currentVertCount;
        AddVert(vh, center, color);

        for (int s = 0; s <= segments; s++)
        {
            float deg = s * (360f / segments);
            Vector2 p = center + DirFromDeg(deg) * radius;
            AddVert(vh, p, color);
        }

        for (int s = 0; s < segments; s++)
        {
            int i0 = baseIndex;
            int i1 = baseIndex + 1 + s;
            int i2 = baseIndex + 2 + s;
            vh.AddTriangle(i0, i1, i2);
        }
    }

    private static void AddVert(VertexHelper vh, Vector2 pos, Color color)
    {
        var v = UIVertex.simpleVert;
        v.color = color;
        v.position = pos;
        vh.AddVert(v);
    }

    // 0° at Up, clockwise positive: dir = (sin, cos)
    private static Vector2 DirFromDeg(float deg)
    {
        float rad = deg * Mathf.Deg2Rad;
        return new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (autoBindToJoystick && joystick == null)
            joystick = GetComponent<MobileUIJoystick>();
        SetVerticesDirty(); // redraw without creating/destroying children
    }
#endif
}