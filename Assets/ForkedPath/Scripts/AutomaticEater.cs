using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public sealed class AutomaticEater : MonoBehaviour, IEatingProvider
{
    private bool eatingEnabled = false;
    public bool EatingEnabled {
        get => eatingEnabled;
        set
        {
            if (value == eatingEnabled) return;
            eatingEnabled = value;
            if (eatingEnabled)
                ResetTimers(); // start fresh when enabling
        }
    }

    //TODO maybe make this configurable for each eater/prey
    public static readonly float stayTargetTime = 0.5f;

    public bool IsEating{ get; private set; }

    [SerializeField] CustomTrigger2D eatingTrigger;

    Dictionary<Entity, Timer> timers = new Dictionary<Entity, Timer>();
    Entity owner;

    // reuse buffer to avoid allocations
    static readonly List<Entity> _cleanup = new List<Entity>(8);

    public void ResetTimers()
    {
        foreach (var timer in timers.Values)
        {
            timer.elapsedTime = 0f;
            timer.triggered = false;
        }
    }

    private void Awake()
    {
        owner = GetComponent<Entity>();
        eatingTrigger = GetComponentInChildren<CustomTrigger2D>();
        eatingTrigger.onEnter.AddListener(objectEntered);
        eatingTrigger.onStay.AddListener(objectStayed);
        eatingTrigger.onExit.AddListener(objectExited);
    }

    private void OnDisable()
    {
        if (eatingTrigger != null)
        {
            eatingTrigger.onEnter.RemoveListener(objectEntered);
            eatingTrigger.onStay.RemoveListener(objectStayed);
            eatingTrigger.onExit.RemoveListener(objectExited);
        }
    }

    private void LateUpdate()
    {
        // Prune entries that won’t necessarily fire OnTriggerExit:
        // - destroyed (== null)
        // - disabled/inactive (!isActiveAndEnabled)
        // - despawned or not eatable anymore
        if (timers.Count > 0)
        {
            _cleanup.Clear();
            foreach (var kv in timers)
            {
                var e = kv.Key;
                if (e == null || !e.isActiveAndEnabled || e.CurrentState == EntityState.Despawned || !e.isEatable)
                    _cleanup.Add(e);
            }
            for (int i = 0; i < _cleanup.Count; i++)
                timers.Remove(_cleanup[i]);
        }

        IsEating = eatingEnabled && timers.Count > 0;
    }

    void objectEntered(Collider2D collision)
    {
        var entity = collision.GetComponentInParent<Entity>();
        if (entity == null || !entity.isEatable)
            return;
        if (!timers.ContainsKey(entity))
            timers.Add(entity, new Timer());
    }
    void objectStayed(Collider2D collision)
    {
        var entity = collision.GetComponentInParent<Entity>();
        if (entity == null)
            return;
        
        if (!entity.isEatable)
        {
            timers.Remove(entity);
        }
        else if (eatingEnabled)
        {
            if (!timers.ContainsKey(entity))
            {
                Debug.Log(collision + " - unregistered, but eatable and stays at trigger : " + ToString());
            }
            else
            {
                var t = timers[entity];
                t.elapsedTime += Time.deltaTime;
                if (t.elapsedTime >= stayTargetTime && !t.triggered)
                {
                    t.triggered = true;
                    // Prevent re-trigger this frame and after: remove immediately
                    timers.Remove(entity);
                    entity.Eat(owner);
                }
            }
        }
        
    }
    void objectExited(Collider2D collision)
    {
        var entity = collision.GetComponentInParent<Entity>();
        if (entity == null)
            return;
        timers.Remove(entity);
    }

    sealed class Timer
    {
        public bool triggered;
        public float elapsedTime;
    }
}