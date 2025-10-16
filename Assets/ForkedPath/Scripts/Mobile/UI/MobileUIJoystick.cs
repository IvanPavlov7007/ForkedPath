using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class MobileUIJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEndDragHandler, ICanvasRaycastFilter
{
    public enum Direction8
    {
        None = -1,
        Up = 0,
        UpRight = 1,
        Right = 2,
        DownRight = 3,
        Down = 4,
        DownLeft = 5,
        Left = 6,
        UpLeft = 7
    }

    [Serializable]
    public sealed class DirectionChangedEvent : UnityEvent<Direction8, Vector2> { }

    [Header("Setup")]
    [Tooltip("Optional. If null, the RectTransform on this GameObject is used.")]
    [SerializeField] private RectTransform joystickRect;

    [Tooltip("Only consider pointer presses that begin within the circular area.")]
    [SerializeField] private bool requirePressInsideCircle = true;

    [Header("Behavior")]
    [Tooltip("First finger that presses inside the joystick captures it until release. Other touches are ignored.")]
    [SerializeField] private bool lockToFirstTouch = true;

    [Tooltip("Release and reset when the active finger leaves the circle.")]
    [SerializeField] private bool releaseOnPointerExit = false;

    [Tooltip("Dead zone radius as a fraction of the joystick radius. Inside this zone, no direction is selected.")]
    [Range(0f, 0.49f)]
    [SerializeField] private float deadZoneRadiusNormalized = 0.15f;

    [Tooltip("Boundary tolerance in degrees. If the touch is within this angle from a slice boundary, the last non-boundary direction is kept to avoid flicker.")]
    [Range(0f, 5f)]
    [SerializeField] private float sliceBoundaryEpsilonDegrees = 0.2f;

    [Header("Events")]
    [SerializeField] private DirectionChangedEvent onDirectionChanged;

    public Direction8 CurrentDirection { get; private set; } = Direction8.None;

    // Unit vector of the current direction. Diagonals are normalized (~0.707, 0.707). Zero when None.
    public Vector2 CurrentVector { get; private set; } = Vector2.zero;

    // The raw local stick vector (center to touch, clamped to radius). Useful if you want magnitude too.
    public Vector2 RawLocalVector { get; private set; } = Vector2.zero;

    public bool IsPressed { get; private set; }
    public int ActivePointerId { get; private set; } = -1;

    private RectTransform _rt;
    private Canvas _canvas;
    private Camera _eventCamera;

    private float RadiusPixels
    {
        get
        {
            var rect = _rt.rect;
            return 0.5f * Mathf.Min(rect.width, rect.height);
        }
    }

    private Vector2 CenterLocal
    {
        get
        {
            // Convert from pivot-relative coordinates to the rect center.
            var rect = _rt.rect;
            return new Vector2((0.5f - _rt.pivot.x) * rect.width, (0.5f - _rt.pivot.y) * rect.height);
        }
    }

    private static readonly Vector2[] DirectionVectors =
    {
        new Vector2(0f, 1f),                                   // Up
        new Vector2(1f, 1f).normalized,                        // UpRight
        new Vector2(1f, 0f),                                   // Right
        new Vector2(1f, -1f).normalized,                       // DownRight
        new Vector2(0f, -1f),                                  // Down
        new Vector2(-1f, -1f).normalized,                      // DownLeft
        new Vector2(-1f, 0f),                                  // Left
        new Vector2(-1f, 1f).normalized                        // UpLeft
    };

    private void Awake()
    {
        _rt = joystickRect ? joystickRect : GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _eventCamera = _canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? (_canvas.worldCamera != null ? _canvas.worldCamera : Camera.main)
            : null;
    }

    private void OnDisable()
    {
        ResetState();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (lockToFirstTouch && ActivePointerId != -1 && ActivePointerId != eventData.pointerId)
            return;

        if (!TryScreenToLocal(eventData.position, out var local))
            return;

        var center = CenterLocal;
        var delta = local - center;

        if (requirePressInsideCircle && delta.magnitude > RadiusPixels)
            return;

        CapturePointer(eventData.pointerId);
        UpdateFromLocal(delta);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsActivePointer(eventData.pointerId))
            return;

        if (!TryScreenToLocal(eventData.position, out var local))
            return;

        var delta = local - CenterLocal;

        bool inside = delta.magnitude <= RadiusPixels + 0.0001f;
        if (!inside && releaseOnPointerExit)
        {
            ResetState();
            return;
        }

        UpdateFromLocal(delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsActivePointer(eventData.pointerId))
            ResetState();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (IsActivePointer(eventData.pointerId))
            ResetState();
    }

    // Raycast filter so only the circular region is clickable in the UI, even if the Image is rectangular.
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (!requirePressInsideCircle)
            return true;

        if (!TryScreenToLocal(sp, out var local))
            return false;

        var delta = local - CenterLocal;
        return delta.sqrMagnitude <= RadiusPixels * RadiusPixels + 0.001f;
    }

    // Allows querying direction from an arbitrary screen position without altering state.
    public bool TryGetDirectionFromScreenPoint(Vector2 screenPoint, out Direction8 dir, out Vector2 dirVector)
    {
        dir = Direction8.None;
        dirVector = Vector2.zero;

        if (!TryScreenToLocal(screenPoint, out var local))
            return false;

        var delta = local - CenterLocal;
        return TryComputeDirection(delta, CurrentDirection, out dir, out dirVector);
    }

    private void CapturePointer(int pointerId)
    {
        ActivePointerId = pointerId;
        IsPressed = true;
    }

    private bool IsActivePointer(int pointerId) => IsPressed && ActivePointerId == pointerId;

    private void ResetState()
    {
        ActivePointerId = -1;
        IsPressed = false;
        RawLocalVector = Vector2.zero;
        SetDirection(Direction8.None, Vector2.zero);
    }

    private bool TryScreenToLocal(Vector2 screenPoint, out Vector2 localPoint)
    {
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(_rt, screenPoint, _eventCamera, out localPoint);
    }

    private void UpdateFromLocal(Vector2 localDelta)
    {
        // Clamp to the circle radius
        var radius = RadiusPixels;
        var clamped = localDelta;
        if (clamped.sqrMagnitude > radius * radius)
            clamped = clamped.normalized * radius;

        RawLocalVector = clamped;

        if (TryComputeDirection(clamped, CurrentDirection, out var newDir, out var vec))
        {
            SetDirection(newDir, vec);
        }
    }

    private bool TryComputeDirection(Vector2 localDelta, Direction8 lastDirection, out Direction8 dir, out Vector2 dirVector)
    {
        var radius = RadiusPixels;
        var deadRadius = Mathf.Clamp01(deadZoneRadiusNormalized) * radius;

        float mag = localDelta.magnitude;

        if (mag <= deadRadius)
        {
            dir = Direction8.None;
            dirVector = Vector2.zero;
            return dir != CurrentDirection; // changed only if previously not None
        }

        // Angle relative to Up (0 degrees at Up, clockwise positive)
        // atan2(x, y) gives 0 at Up, 90 at Right, 180 at Down, -90 at Left
        float angle = Mathf.Atan2(localDelta.x, localDelta.y) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;

        float iFloat = angle / 45f; // 8 slices
        int iRounded = Mathf.RoundToInt(iFloat) % 8;

        // Boundary stabilization: if close to a boundary, keep the last direction (prevents flicker).
        float nearestMultiple = Mathf.Round(iFloat);
        float deltaToBoundaryDegrees = Mathf.Abs(iFloat - nearestMultiple) * 45f;

        if (deltaToBoundaryDegrees <= sliceBoundaryEpsilonDegrees && lastDirection != Direction8.None)
        {
            dir = lastDirection;
            dirVector = DirectionToVector(lastDirection);
        }
        else
        {
            dir = IndexToDirection(iRounded);
            dirVector = DirectionVectors[iRounded];
        }

        return dir != CurrentDirection;
    }

    private static Direction8 IndexToDirection(int index)
    {
        switch (index)
        {
            case 0: return Direction8.Up;
            case 1: return Direction8.UpRight;
            case 2: return Direction8.Right;
            case 3: return Direction8.DownRight;
            case 4: return Direction8.Down;
            case 5: return Direction8.DownLeft;
            case 6: return Direction8.Left;
            case 7: return Direction8.UpLeft;
            default: return Direction8.None;
        }
    }

    private static Vector2 DirectionToVector(Direction8 d)
    {
        if (d == Direction8.None) return Vector2.zero;
        return DirectionVectors[(int)d];
    }

    private void SetDirection(Direction8 newDir, Vector2 newVector)
    {
        if (newDir == CurrentDirection && newVector == CurrentVector)
            return;

        CurrentDirection = newDir;
        CurrentVector = newVector;
        onDirectionChanged?.Invoke(CurrentDirection, CurrentVector);
    }

#if UNITY_EDITOR
    // Scene view visualization for the slices and deadzone.
    private void OnDrawGizmosSelected()
    {
        if (_rt == null) _rt = joystickRect ? joystickRect : GetComponent<RectTransform>();
        if (_rt == null) return;

        var center = _rt.TransformPoint(CenterLocal);
        float r = RadiusPixels * _rt.lossyScale.x;

        // Draw circle
        UnityEditor.Handles.color = new Color(1f, 1f, 1f, 0.6f);
        UnityEditor.Handles.DrawWireDisc(center, Vector3.forward, r);

        // Dead zone
        float dead = Mathf.Clamp01(deadZoneRadiusNormalized) * r;
        UnityEditor.Handles.color = new Color(1f, 0f, 0f, 0.25f);
        UnityEditor.Handles.DrawSolidDisc(center, Vector3.forward, dead);

        // Slice lines every 45 degrees
        UnityEditor.Handles.color = new Color(0f, 1f, 0f, 0.4f);
        for (int i = 0; i < 8; i++)
        {
            float deg = i * 45f;
            // Convert "0 at Up" to standard math angle: world-space uses Up as +Y
            float rad = Mathf.Deg2Rad * deg;
            // angle from Up clockwise: direction = (sin, cos)
            Vector2 dir = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
            Vector3 end = center + (Vector3)(dir * r);
            UnityEditor.Handles.DrawLine(center, end);
        }
    }
#endif

    // Edge case policy summary:
    // - Multiple touches: first touch inside the joystick captures control; others are ignored until it releases.
    // - Dead zone: touch inside dead zone selects None.
    // - Slice boundaries: if very close to a boundary, keep the last direction to avoid flickering.
    // - Leaving the circle while dragging:
    //      - If releaseOnPointerExit = true, joystick resets immediately.
    //      - If false (default), direction continues to update based on the clamped edge.
}
