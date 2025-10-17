using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CameraOnATrack : ManagedCamera
{
    ICameraTrackDolly dolly;
    CinemachinePositionComposer positionComposer;

    static readonly DirectionalSettings upSettings = new DirectionalSettings
    {
        screenPosition = new Vector2(0, 0.2f),
        deadZone = new Vector2(0f, 0.4f)
    };
    static readonly DirectionalSettings downSettings = new DirectionalSettings
    {
        screenPosition = new Vector2(0, -0.2f),
        deadZone = new Vector2(0f, 0.4f)
    };
    static readonly DirectionalSettings rightSettings = new DirectionalSettings
    {
        screenPosition = new Vector2(-0.2f, 0),
        deadZone = new Vector2(0.4f, 0f)
    };
    static readonly DirectionalSettings leftSettings = new DirectionalSettings
    {
        screenPosition = new Vector2(0.2f, 0),
        deadZone = new Vector2(0.4f, 0f)
    };


    protected override void Awake()
    {
        base.Awake();
        positionComposer = GetComponentInChildren<CinemachinePositionComposer>();
        dolly = GetComponentInChildren<ICameraTrackDolly>();
        cam.Target.TrackingTarget = dolly.Dolly;
    }

    private void Start()
    {
        applySetting(selectSettingByDirection(dolly.Direction));
    }

    DirectionalSettings selectSettingByDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Horizontal movement
            if (direction.x > 0)
                return rightSettings;
            else
                return leftSettings;
        }
        else
        {
            // Vertical movement
            if (direction.y > 0)
                return upSettings;
            else
                return downSettings;
        }
    }

    void applySetting(DirectionalSettings settings)
    {
        positionComposer.Composition.ScreenPosition = settings.screenPosition;
        positionComposer.Composition.DeadZone.Size = settings.deadZone;
        positionComposer.Composition.HardLimits.Size = settings.deadZone;
    }

    struct DirectionalSettings
    {
        public Vector2 screenPosition;
        public Vector2 deadZone;
    }
}

public interface ICameraTrackDolly
{
    Transform Dolly { get; }
    Vector2 Direction { get; }
}