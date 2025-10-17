#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LinearTrackDolly))]
public class LinearTrackDollyEditor : Editor
{
    SerializedProperty _trackStartProp;
    SerializedProperty _trackEndProp;
    SerializedProperty _startColorProp;
    SerializedProperty _endColorProp;
    SerializedProperty _handleSizeProp;

    void OnEnable()
    {
        _trackStartProp = serializedObject.FindProperty("TrackStart");
        _trackEndProp = serializedObject.FindProperty("TrackEnd");
        _startColorProp = serializedObject.FindProperty("startColor");
        _endColorProp = serializedObject.FindProperty("endColor");
        _handleSizeProp = serializedObject.FindProperty("handleSize");
    }

    public void OnSceneGUI()
    {
        serializedObject.Update();

        var dolly = (LinearTrackDolly)target;

        Vector2 start2 = _trackStartProp.vector2Value;
        Vector2 end2 = _trackEndProp.vector2Value;

        float zPlane = dolly.transform.position.z; // keep handles on the object's Z plane
        Vector3 start = new Vector3(start2.x, start2.y, zPlane);
        Vector3 end = new Vector3(end2.x, end2.y, zPlane);

        float sizeMul = Mathf.Max(0.01f, _handleSizeProp.floatValue);
        float startSize = HandleUtility.GetHandleSize(start) * sizeMul;
        float endSize = HandleUtility.GetHandleSize(end) * sizeMul;

        // Draw anti-aliased line for better visibility while selected
        Handles.color = new Color(1f, 1f, 1f, 0.9f);
        Handles.DrawAAPolyLine(2f, start, end);

        // Determine if snapping is active and the snap increments we should use
        bool snapActive = IsGridSnapEnabled() || EditorGUI.actionKey; // toolbar Snap or Ctrl/Cmd
        Vector3 snapIncrements = GetMoveSnap();

        EditorGUI.BeginChangeCheck();

        // Start handle (Slider2D replaces deprecated FreeMoveHandle)
        Handles.color = _startColorProp.colorValue;
        Vector3 newStart = Handles.Slider2D(
            start,
            Vector3.forward, // plane normal (XY plane)
            Vector3.right,   // X axis
            Vector3.up,      // Y axis
            startSize,
            Handles.SphereHandleCap,
            0f               // we'll do per-axis snapping manually
        );
        Handles.Label(newStart + Vector3.up * (startSize * 1.2f), "Start");

        // End handle
        Handles.color = _endColorProp.colorValue;
        Vector3 newEnd = Handles.Slider2D(
            end,
            Vector3.forward,
            Vector3.right,
            Vector3.up,
            endSize,
            Handles.SphereHandleCap,
            0f
        );
        Handles.Label(newEnd + Vector3.up * (endSize * 1.2f), "End");

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(dolly, "Move Track Endpoints");

            // Constrain to XY plane (keep Z from the dolly)
            newStart.z = zPlane;
            newEnd.z = zPlane;

            if (snapActive)
            {
                newStart.x = SnapAxis(newStart.x, snapIncrements.x);
                newStart.y = SnapAxis(newStart.y, snapIncrements.y);
                newEnd.x = SnapAxis(newEnd.x, snapIncrements.x);
                newEnd.y = SnapAxis(newEnd.y, snapIncrements.y);
            }

            _trackStartProp.vector2Value = new Vector2(newStart.x, newStart.y);
            _trackEndProp.vector2Value = new Vector2(newEnd.x, newEnd.y);

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(dolly);
        }
    }

    static float SnapAxis(float value, float increment)
    {
        if (increment <= 0f) return value;
        // Use Handles.SnapValue for consistency with Unity's own tools
        return Handles.SnapValue(value, increment);
    }

    static Vector3 GetMoveSnap()
    {
        // EditorSnapSettings.move exists in modern Unity; default to (1,1,1) if unavailable
        try
        {
            return EditorSnapSettings.move;
        }
        catch
        {
            return Vector3.one;
        }
    }

    static bool IsGridSnapEnabled()
    {
        // Prefer EditorSnapSettings.gridSnapEnabled when available; fallback to older names; else false
        var t = typeof(EditorSnapSettings);
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        var p = t.GetProperty("gridSnapEnabled", flags);
        if (p != null && p.PropertyType == typeof(bool))
        {
            return (bool)p.GetValue(null, null);
        }

        p = t.GetProperty("snapEnabled", flags) ?? t.GetProperty("enabled", flags);
        if (p != null && p.PropertyType == typeof(bool))
        {
            return (bool)p.GetValue(null, null);
        }

        return false;
    }
}
#endif