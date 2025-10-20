using System;
using System.Collections;
using UnityEngine;
public class Health : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int MaxHealth = 5;

    public bool IsDead { get; private set; }
    public bool IsInvincible { get; private set; }
    public bool IsFallingToDeath { get; private set; }
    public bool IsCorpseFalling { get; private set; }

    int currentHealth;

    public int CurrentHealth => currentHealth;

    Coroutine _invCoroutine;
    float _invEndsAt;

    void Awake()
    {
        currentHealth = MaxHealth;
    }

    public void InitializeAsCorpse()
    {
        IsDead = true;
    }

    public void TakeDamage(int amount, string context, Vector2 hitPoint, Vector2 hitDir, ScriptableObject source)
    {
        if (IsDead) return;
        if (IsInvincible) return;
        if (IsFallingToDeath) return; // ignore damage during fall phase

        currentHealth -= amount;

        DamageEventData damageEventData = new(this, context, amount, hitPoint, hitDir, source);
        GameEvents.Instance.OnDamage?.Invoke(damageEventData);

        if (currentHealth <= 0)
        {
            Die(damageEventData);
        }
    }

    // Alive -> Falling (multi-phase), not dead yet.
    public void BeginFallingToDeath()
    {
        if (IsDead || IsFallingToDeath) return;

        // cancel invincibility for clarity
        if (_invCoroutine != null) StopCoroutine(_invCoroutine);
        _invCoroutine = null;
        _invEndsAt = 0f;
        if (IsInvincible) SetInvincible(false, InvincibilityReason.Script, 0f);

        IsFallingToDeath = true;
        GameEvents.Instance.OnFallingToDeathStarted?.Invoke(new FallingEventData(GetComponent<Entity>()));
    }

    // Dead -> DeadFalling (corpse starts falling).
    public void BeginCorpseFalling()
    {
        if (!IsDead || IsCorpseFalling) return;
        IsCorpseFalling = true;
        GameEvents.Instance.OnFallingToDeathStarted?.Invoke(new FallingEventData(GetComponent<Entity>()));
    }

    // Alive fall completes: finalize with death.
    // Call when the fall completes (e.g., hit kill volume or de-spawn trigger).
    public void LandAndDie()
    {
        if (IsDead) return;
        IsFallingToDeath = false;
        InstantDie(fallenToDeath: true);
    }

    // Corpse fall completes: return to Dead state.
    public void CorpseLanded()
    {
        if (!IsDead) return;
        if (!IsCorpseFalling) return;
        IsCorpseFalling = false;
        GameEvents.Instance.OnCorpseLanded?.Invoke(new CorpseLandedEventData(GetComponent<Entity>()));
    }

    public void InstantDie(bool fallenToDeath = false)
    {
        // cancel invincibility cleanly (mirror Die)
        if (_invCoroutine != null) StopCoroutine(_invCoroutine);
        _invCoroutine = null;
        _invEndsAt = 0f;
        if (IsInvincible) SetInvincible(false, InvincibilityReason.Script, 0f);

        IsFallingToDeath = false;
        IsCorpseFalling = false;
        currentHealth = 0;
        IsDead = true;
        GameEvents.Instance.OnDeath?.Invoke(new DeathEventData(GetComponent<Entity>(), fallenToDeath: fallenToDeath));
    }

    public void BeginInvincibility(float duration, InvincibilityReason reason = InvincibilityReason.AfterHit)
    {
        if (IsDead || duration <= 0f) return;

        var endsAt = Time.time + duration;
        if (endsAt > _invEndsAt) _invEndsAt = endsAt;

        if (_invCoroutine == null)
        {
            SetInvincible(true, reason, duration);
            _invCoroutine = StartCoroutine(InvincibilityTimer(reason));
        }
    }

    IEnumerator InvincibilityTimer(InvincibilityReason reason)
    {
        while (Time.time < _invEndsAt) yield return null;
        _invCoroutine = null;
        _invEndsAt = 0f;
        SetInvincible(false, reason, 0f);
    }

    void SetInvincible(bool value, InvincibilityReason reason, float duration)
    {
        if (IsInvincible == value) return;
        IsInvincible = value;
        GameEvents.Instance.OnInvincibilityChanged?.Invoke(
            new InvincibilityEventData(GetComponent<Entity>(), value, duration, reason));
    }

    void OnDisable()
    {
        if (_invCoroutine != null) StopCoroutine(_invCoroutine);
        _invCoroutine = null;
        _invEndsAt = 0f;
        if (IsInvincible) SetInvincible(false, InvincibilityReason.Script, 0f);
        IsFallingToDeath = false;
        IsCorpseFalling = false;
    }

    void Die(DamageEventData damageEventData)
    {
        if (_invCoroutine != null) StopCoroutine(_invCoroutine);
        _invCoroutine = null;
        _invEndsAt = 0f;
        if (IsInvincible) SetInvincible(false, InvincibilityReason.Script, 0f);
        IsFallingToDeath = false;
        IsCorpseFalling = false;
        IsDead = true;
        GameEvents.Instance.OnDeath?.Invoke(new DeathEventData(GetComponent<Entity>(), damageEventData));
    }

    public void SetMaxHealth(int maxHealth)
    {
        this.MaxHealth = maxHealth;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    public void ResetHealth()
    {
        currentHealth = MaxHealth;
        IsDead = false;
        IsFallingToDeath = false;
        IsCorpseFalling = false;
    }

    [Obsolete("Use BeginInvincibility instead")]
    internal void SetInvincible(bool v)
    {
        IsInvincible = v;
    }
}