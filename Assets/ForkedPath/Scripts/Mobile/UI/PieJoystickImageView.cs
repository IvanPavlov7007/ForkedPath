using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class PieJoystickImageView : MonoBehaviour
{
    [Header("Slice Visuals")]
    [Tooltip("Sprite used for each pie slice. Any round UI sprite works (e.g., Unity's default UI sprite).")]
    public Sprite sliceSprite;

    [Tooltip("Base color for all slices.")]
    public Color baseColor = new Color(1f, 1f, 1f, 0.2f);

    [Tooltip("Color for the selected slice.")]
    public Color highlightColor = new Color(1f, 1f, 1f, 0.9f);

    [Tooltip("Optional background circle sprite (under the slices).")]
    public Sprite backgroundSprite;

    [Tooltip("Color of the background circle.")]
    public Color backgroundColor = new Color(1f, 1f, 1f, 0.08f);

    [Header("Dead Zone (visual only)")]
    [Tooltip("Show a faint inner disc to indicate dead zone.")]
    public bool showDeadZone = true;

    [Tooltip("Dead zone radius as a fraction of the joystick radius (for visuals only). Match MobileUIJoystick setting manually).")]
    [Range(0f, 0.49f)]
    public float deadZoneRadiusNormalized = 0.15f;

    [Tooltip("Color of the dead zone circle.")]
    public Color deadZoneColor = new Color(1f, 0f, 0f, 0.12f);

    [Header("Build")]
    [Tooltip("Automatically (re)build the 8 slices on Awake.")]
    public bool buildOnAwake = true;

    [Tooltip("Sort order: background at bottom, then slices, then dead zone on top (if enabled).")]
    public bool deadZoneOnTop = false;

    [Header("Animation")]
    [Tooltip("Animate the selected slice with a small pulse.")]
    public bool animateSelected = true;

    [Tooltip("Pulse speed for the selected slice.")]
    public float pulseSpeed = 6f;

    [Tooltip("Pulse amplitude (scale). 0.05 = 5% larger at peak.")]
    [Range(0f, 0.2f)]
    public float pulseScale = 0.05f;

    private readonly List<Image> _slices = new List<Image>(8);
    private Image _background;
    private Image _deadZone;
    private int _selectedIndex = -1;
    private float _baseSliceScale = 1f;

    private RectTransform _rt;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        if (buildOnAwake)
            Rebuild();
    }

    private void Update()
    {
        if (!animateSelected)
            return;

        for (int i = 0; i < _slices.Count; i++)
        {
            var rt = _slices[i].rectTransform;
            if (i == _selectedIndex)
            {
                float s = _baseSliceScale * (1f + Mathf.Sin(Time.unscaledTime * pulseSpeed) * pulseScale);
                rt.localScale = new Vector3(s, s, 1f);
            }
            else
            {
                if (rt.localScale.x != _baseSliceScale)
                    rt.localScale = new Vector3(_baseSliceScale, _baseSliceScale, 1f);
            }
        }

        if (_deadZone != null)
        {
            // Keep dead zone scaled with parent rect (safe each frame in case layout changes).
            float d = Mathf.Clamp01(deadZoneRadiusNormalized);
            _deadZone.rectTransform.localScale = new Vector3(d * 2f, d * 2f, 1f);
        }
    }

    public void Rebuild()
    {
        ClearChildren();

        // Background
        if (backgroundSprite != null)
        {
            _background = CreateChildImage("Background", 0, Quaternion.identity);
            _background.sprite = backgroundSprite;
            _background.type = Image.Type.Sliced; // Sliced or Simple depending on your sprite
            _background.color = backgroundColor;
            _background.raycastTarget = false;
        }

        // Slices
        for (int i = 0; i < 8; i++)
        {
            Image img = CreateSlice(i);
            _slices.Add(img);
        }

        // Dead Zone (Top or below slices depending on preference)
        if (showDeadZone)
        {
            _deadZone = CreateChildImage("DeadZone", deadZoneOnTop ? 3 : 1, Quaternion.identity);
            _deadZone.sprite = sliceSprite;
            _deadZone.type = Image.Type.Filled;
            _deadZone.fillMethod = Image.FillMethod.Radial360;
            _deadZone.fillAmount = 1f;
            _deadZone.color = deadZoneColor;
            _deadZone.raycastTarget = false;

            float d = Mathf.Clamp01(deadZoneRadiusNormalized);
            _deadZone.rectTransform.localScale = new Vector3(d * 2f, d * 2f, 1f);
        }

        SetSelectedIndex(-1);
    }

    public void SetSelectedIndex(int index)
    {
        _selectedIndex = index;

        for (int i = 0; i < _slices.Count; i++)
        {
            var img = _slices[i];
            img.color = (i == index) ? highlightColor : baseColor;
            if (!animateSelected)
            {
                // Snap scale if animation is off
                img.rectTransform.localScale = Vector3.one;
            }
        }
    }

    // Hook this up to MobileUIJoystick.onDirectionChanged in the Inspector.
    public void OnJoystickDirectionChanged(MobileUIJoystick.Direction8 direction, Vector2 vector)
    {
        int idx = direction == MobileUIJoystick.Direction8.None ? -1 : (int)direction;
        SetSelectedIndex(idx);
    }

    private void ClearChildren()
    {
        _slices.Clear();
        _background = null;
        _deadZone = null;

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.Undo.DestroyObjectImmediate(child.gameObject);
            else
                Destroy(child.gameObject);
#else
            Destroy(child.gameObject);
#endif
        }
    }

    private Image CreateSlice(int index)
    {
        float angle = index * 45f;
        var img = CreateChildImage($"Slice {index}", 2, Quaternion.Euler(0f, 0f, angle));

        img.sprite = sliceSprite;
        img.type = Image.Type.Filled;
        img.fillMethod = Image.FillMethod.Radial360;
        img.fillOrigin = 0;            // 0 = Top (Up), matching joystick mapping
        img.fillAmount = 0.125f;       // 360 * 0.125 = 45 degrees
        img.fillClockwise = true;
        img.color = baseColor;
        img.raycastTarget = false;     // Let MobileUIJoystick receive the input

        return img;
    }

    private Image CreateChildImage(string name, int siblingLayer, Quaternion rotation)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.layer = gameObject.layer;
        go.transform.SetParent(transform, false);

        var rt = (RectTransform)go.transform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = Vector2.zero;
        rt.localRotation = rotation;
        rt.localScale = Vector3.one;

        var img = go.GetComponent<Image>();
        go.transform.SetSiblingIndex(Mathf.Clamp(siblingLayer, 0, transform.childCount));

        return img;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Maintain visual consistency in edit mode.
        if (!Application.isPlaying && buildOnAwake)
        {
            if (_rt == null) _rt = GetComponent<RectTransform>();
            Rebuild();
        }
    }
#endif
}