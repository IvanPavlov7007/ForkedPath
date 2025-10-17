using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using System;
using Pixelplacement;
using NUnit.Framework;
using System.Collections.Generic;

public sealed class CameraManager : Singleton<CameraManager>
{
    [SerializeField]
    int highPriority = 1;
    [SerializeField]
    ManagedCameraPosition[] allCameraPositions;

    LinkedList<ManagedCameraPosition> cameraPositionsList;
    LinkedListNode<ManagedCameraPosition> nextCameraPosition = null;
    ManagedCamera currentCamera = null;

    private void Awake()
    {
        cameraPositionsList = new LinkedList<ManagedCameraPosition>(allCameraPositions);
        nextCameraPosition = cameraPositionsList.First;
        if(nextCameraPosition != null)
        {
            checkCompletion(nextCameraPosition.Value.triggerConditions);
        }
    }

    private void OnEnable()
    {
        GameEvents.Instance.OnPlayerEnterTrigger += OnPlayerTriggerEntered;
        GameEvents.Instance.OnDeath += OnEntityDeath;
    }

    private void Update()
    {
        if (nextCameraPosition != null && nextCameraPosition.Value.triggerConditions.completed)
        {
            prioritizeCamera(nextCameraPosition.Value.camera);
            nextCameraPosition = nextCameraPosition.Next;
        }
    }

    private void prioritizeCamera(ManagedCamera managedCamera)
    {
        if(currentCamera != null)
        {
            currentCamera.Deactivate();
            currentCamera.cam.Priority = 0;
        }

        currentCamera = managedCamera;
        currentCamera.cam.Priority = highPriority;
        currentCamera.Activate();
    }

    public void OnPlayerTriggerEntered(PlayerEnterTrigger playerEnterTrigger)
    {
        var node = nextCameraPosition;
        while (node != null)
        {
            if (!node.Value.triggerConditions.completed)
            {
                if (!node.Value.triggerConditions.checkOnlyIfNext || node == nextCameraPosition)
                {
                    bool found = false;
                    foreach (var triggerCondition in node.Value.triggerConditions.triggersToEnterConditions)
                    {
                        if (triggerCondition.PlayerEnterTrigger == playerEnterTrigger)
                        {
                            triggerCondition.completed = true;
                            found = true;
                        }
                    }
                    if (found)
                    {
                        checkCompletion(node.Value.triggerConditions);
                    }
                }
            }
            node = node.Next;
        }
    }

    public void OnEntityDeath(DeathEventData deathEventData)
    {
        var node = nextCameraPosition;
        while (node != null)
        {
            if (!node.Value.triggerConditions.completed)
            {
                if (!node.Value.triggerConditions.checkOnlyIfNext || node == nextCameraPosition)
                {
                    bool found = false;
                    foreach (var enemyDeathTracker in node.Value.triggerConditions.enemiesDeadConditions)
                    {
                        if (enemyDeathTracker.enemyID == deathEventData.entity.Config.entityID)
                        {
                            enemyDeathTracker.deathCount++;
                            enemyDeathTracker.completed = enemyDeathTracker.deathCount >= enemyDeathTracker.requiredDeathCount;
                            found = true;
                        }
                    }
                    if(found)
                    {
                        checkCompletion(node.Value.triggerConditions);
                    }
                }
            }
            node = node.Next;
        }
    }



    private void checkCompletion(CameraTriggerConditions conditions)
    {
        bool allCompleted = true;
        foreach (var trigger in conditions.triggersToEnterConditions)
        {
            if (!trigger.completed)
            {
                allCompleted = false;
                break;
            }
        }
        foreach(var enemyDeathTracker in conditions.enemiesDeadConditions)
        {
            if (!enemyDeathTracker.completed)
            {
                allCompleted = false;
                break;
            }
        }
        conditions.completed = allCompleted;
    }


    [Serializable]
    public class ManagedCameraPosition
    {
        public ManagedCamera camera;
        public CameraTriggerConditions triggerConditions;
    }
    [Serializable]
    public class CameraTriggerConditions
    {
        public CameraTriggerConditions() 
        {
            checkOnlyIfNext = true;
            triggersToEnterConditions = new PlayerEnterTriggerCondition[0];
            enemiesDeadConditions = new EnemyDeathTrackerCondition[0];
        }

        public bool checkOnlyIfNext = true;
        public PlayerEnterTriggerCondition[] triggersToEnterConditions;
        public EnemyDeathTrackerCondition[] enemiesDeadConditions;
        [HideInInspector]
        public bool completed;
    }

    public abstract class CameraTriggerCondition
    {
        [HideInInspector]
        public bool completed;
    }

    [Serializable]
    public class PlayerEnterTriggerCondition : CameraTriggerCondition
    {
        public PlayerEnterTrigger PlayerEnterTrigger;
    }

    [Serializable]
    public class EnemyDeathTrackerCondition : CameraTriggerCondition
    {
        public string enemyID;
        public int requiredDeathCount = 1;
        [HideInInspector]
        public int deathCount = 0;
    }
}