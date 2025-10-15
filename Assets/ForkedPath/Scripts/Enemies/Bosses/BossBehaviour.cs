using System.Collections;
using UnityEngine;
public abstract class BossBehaviour : ScriptableObject
{
    public abstract void Begin(BossController boss);
    public abstract void End(BossController boss);

    public abstract bool Update(BossController boss);
}