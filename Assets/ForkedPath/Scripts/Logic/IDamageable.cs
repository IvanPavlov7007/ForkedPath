using System.Collections;
using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(int amount, string context, Vector2 hitPoint, Vector2 hitDir, Vector2 hitNormal, ScriptableObject source);
    public bool IsDead { get; }
}