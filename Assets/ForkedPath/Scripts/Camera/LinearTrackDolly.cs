using Pixelplacement;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LinearTrackDolly : MonoBehaviour, ICameraTrackDolly
{
    [Header("Track")]
    [Tooltip("World-space start of the linear track.")]
    [SerializeField] Vector2 TrackStart;
    [Tooltip("World-space end of the linear track.")]
    [SerializeField] Vector2 TrackEnd = Vector2.right * 5f;

    [Header("Editor Gizmos")]
    [Tooltip("Color for the Start handle and gizmo.")]
    [SerializeField] Color startColor = new Color(0.2f, 1f, 0.2f, 1f);
    [Tooltip("Color for the End handle and gizmo.")]
    [SerializeField] Color endColor = new Color(1f, 0.3f, 0.3f, 1f);
    [Tooltip("Handle size multiplier in the Scene view.")]
    [SerializeField] float handleSize = 0.2f;
    [Tooltip("Gizmo sphere radius for endpoints.")]
    [SerializeField] float gizmoRadius = 0.1f;
    [Tooltip("Color of the always-visible line between Start and End.")]
    [SerializeField] Color lineColor = new Color(1f, 1f, 1f, 0.9f);

    public Transform Dolly => transform;

    public Vector2 Direction => (TrackEnd - TrackStart).normalized;

    Transform playerTarget
    {
        get
        {
            var avatar = Player.Instance.CurrentAvatar;
            if (avatar != null)
            {
                return avatar.transform;
            }
            else
                return null;
        }
    }

    [ContextMenu("Relocate segment start position")]
    public void RelocateSegment()
    {
        // Keep segment direction and length, move start to this transform.
        Vector2 oldStart = TrackStart;
        Vector2 oldEnd = TrackEnd;
        Vector2 segment = oldEnd - oldStart;

        #if UNITY_EDITOR
        Undo.RecordObject(this, "Relocate Segment Start Position");
        #endif

        TrackStart = transform.position;
        TrackEnd = TrackStart + segment;

        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        #endif
    }

    private void FixedUpdate()
    {
        var target = playerTarget;
        if (target == null)
            return;
        Vector2 position = target.position;

        Vector2 segment = TrackEnd - TrackStart;
        Vector2 segmentDirection = segment.normalized;
        Vector2 distToTarget = position - TrackStart;
        float projectionOnSegment = Vector2.Dot(distToTarget, segmentDirection);
        float segmentLength = segment.magnitude;

        float clampedProjection = Mathf.Clamp(projectionOnSegment, 0, segmentLength);
        transform.position = TrackStart + segmentDirection * clampedProjection;
    }

    // Always show the line and endpoints in the Scene view.
    private void OnDrawGizmos()
    {
        // Draw connecting line
        Gizmos.color = lineColor;
        Gizmos.DrawLine(new Vector3(TrackStart.x, TrackStart.y, 0f), new Vector3(TrackEnd.x, TrackEnd.y, 0f));

        // Draw endpoints
        Gizmos.color = startColor;
        Gizmos.DrawSphere(new Vector3(TrackStart.x, TrackStart.y, 0f), Mathf.Max(0.0001f, gizmoRadius));
        Gizmos.color = endColor;
        Gizmos.DrawSphere(new Vector3(TrackEnd.x, TrackEnd.y, 0f), Mathf.Max(0.0001f, gizmoRadius));
    }

    // Expose read-only accessors for the editor (optional but convenient)
    public Vector2 GetTrackStart() => TrackStart;
    public Vector2 GetTrackEnd() => TrackEnd;
    public Color GetStartColor() => startColor;
    public Color GetEndColor() => endColor;
    public float GetHandleSize() => handleSize;
}