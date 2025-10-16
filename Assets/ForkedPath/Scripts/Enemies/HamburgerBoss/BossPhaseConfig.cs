using System.Collections;
using UnityEngine;

public abstract class BossPhaseConfig : ScriptableObject
{
    public float endHealth;
    public float timeLimit;
    public float tickTime;

    public abstract void OnPhaseStart(HamburgerController controller);
    public abstract void OnPhaseEnd(HamburgerController controller);
    // Return true to end the phase
    public abstract bool OnFixedUpdate(HamburgerController controller);
}