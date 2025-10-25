using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public abstract class BossPhaseConfig : ScriptableObject
{
    [Title("Phase End Conditions")]
    public float endDamage;
    public float timeLimit;
    public float tickTime;

    protected int initHealth;
    protected bool initHealthDifferenceCondition(HamburgerController controller)
    {
        return controller.entity.Health.CurrentHealth <= initHealth - endDamage;
    }

    public virtual void OnPhaseStart(HamburgerController controller)
    {
        initHealth = controller.entity.Health.CurrentHealth;
    }
    public abstract void OnPhaseEnd(HamburgerController controller);
    // Return true to end the phase
    public abstract bool OnFixedUpdate(HamburgerController controller);
}