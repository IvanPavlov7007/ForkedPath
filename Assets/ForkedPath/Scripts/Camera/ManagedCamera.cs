using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class ManagedCamera : MonoBehaviour
{
    [HideInInspector]
    public CinemachineCamera cam;

    protected virtual void Awake()
    {
        cam = GetComponentInChildren<CinemachineCamera>();
    }

    public virtual void Activate(){}

    public virtual void Deactivate() { }
}