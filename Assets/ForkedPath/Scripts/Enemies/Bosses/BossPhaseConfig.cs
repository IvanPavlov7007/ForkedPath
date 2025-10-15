using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Boss/Phase Config")]
public class BossPhaseConfig : ScriptableObject
{
    public string phaseName;
    public BossBehaviour behaviour;   // Scriptable behavior definition
    public float timeLimit = 0f; // 0 means no time limit
    public float endHealth = 0f; // Health threshold to end phase, 0 means no health limit
}