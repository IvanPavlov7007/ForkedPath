using Pixelplacement;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


[DisallowMultipleComponent]
public sealed partial class HamburgerController : BaseShooterProvider, IFacingDirectionProvider, IMovementProvider
{
    [SerializeField]
    Vector2 cherryLocalPosition;
    [SerializeField]
    Transform hamburgerColliders;
    [SerializeField]
    Transform cherryColliders;
    

    Rigidbody2D rb;
    HamburgerBossAnimator animator;
    BossPhaseConfig[] phases;

    public Vector2 CherryLocalPosition => cherryLocalPosition;

    public Vector2 Direction
    {
        get
        { // looking at player if possible
            var distToPlayer = DistanceToPlayer();
            if(distToPlayer == null || distToPlayer.Value.sqrMagnitude < 0.0001f)
            {
                return Vector2.down;
            }
            return distToPlayer.Value.normalized;
        }
    }

    public bool IsMoving => rb.linearVelocity.sqrMagnitude > 0.1f;

    public Vector2 Velocity => rb.linearVelocity;

    public HamburgerBossConfig Config => entity.Config as HamburgerBossConfig;

    public enum HamburgerBossState{ Hamburger, Cherry}
    public HamburgerBossState currentState = HamburgerBossState.Hamburger;

    int currentPhase = 0;


    override protected void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<HamburgerBossAnimator>();
    }

    public void StartFight()
    {
        phases = clonePhases(Config.phases);
#if UNITY_EDITOR
        if (Config.lowLivesPerPhases)
        {
            int maxHealth = 3 * phases.Length + 1;
            entity.Health.SetMaxHealth(maxHealth);
            for (int i = 0; i < phases.Length - 1; i++)
            {
                phases[i].endDamage = 3;
            }
            phases[phases.Length - 1].endDamage = 200; //big number to ensure death
        }
#endif

        phases[currentPhase].OnPhaseStart(this);
    }

    private BossPhaseConfig[] clonePhases(BossPhaseConfig[] original)
    {
        BossPhaseConfig[] clone = new BossPhaseConfig[original.Length];
        for(int i = 0; i < original.Length; i++)
        {
            clone[i] = Instantiate(original[i]);
        }
        return clone;
    }

    private void FixedUpdate()
    {
        if(phases == null || currentPhase >= phases.Length || ended)
        {
            return;
        }
        if (phases[currentPhase].OnFixedUpdate(this))
        {
            phases[currentPhase].OnPhaseEnd(this);
            currentPhase++;
            if (currentPhase >= phases.Length)
            {
                //maybe shouldn't be reached, since on death should be called before last phase ends
                Debug.LogError("Hamburger Boss: Reached end of phases without dying");
                return;
            }
            phases[currentPhase].OnPhaseStart(this);
        }
    }

    protected override void OnDeath(DeathEventData deathEventData)
    {
        endFight();
    }
    bool ended = false;
    private void endFight()
    {
        if (ended) return;
        ended = true;
        if(currentPhase < phases.Length)
        {
            phases[currentPhase].OnPhaseEnd(this);
        }
        StartCoroutine(dramaticDeath());
    }

    IEnumerator dramaticDeath()
    {
        GameEvents.Instance.OnFX(new FXEventData(transform.position,"Explosions", src: Config, fx: Config.deathFX, sfx: Config.deathSound, parent: transform));
        yield return new WaitForSeconds(Config.deathAnimationDuration);
        Tween.Color(GetComponentInChildren<SpriteRenderer>(), Color.clear, 1f, 0f);
        Destroy(gameObject, 1.5f);
    }

    public Vector2? DistanceToPlayer()
    {
        if(Player.Instance == null || Player.Instance.CurrentAvatar == null) 
            return null;
        return (Vector2)Player.Instance.CurrentAvatar.transform.position - (Vector2)transform.position;
    }

    public IEnumerator TransitionToHamburgerState()
    {
        if(currentState == HamburgerBossState.Hamburger) yield break;
        currentState = HamburgerBossState.Hamburger;
        GameEvents.Instance.OnFX(new FXEventData(transform.position,"Sound",sfx: Config.openingSound));
        yield return new WaitForSeconds(0.2f);
        animator.TriggerTransformToHamburger();
        resetColliders();
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator TransitionToCherryState()
    {
        if(currentState == HamburgerBossState.Cherry) yield break;
        currentState = HamburgerBossState.Cherry;
        GameEvents.Instance.OnFX(new FXEventData(transform.position, "Sound", sfx: Config.closingSound));
        yield return new WaitForSeconds(0.2f);
        animator.TriggerTransformToCherry();
        resetColliders();
        yield return new WaitForSeconds(0.5f);
    }

    private void resetColliders()
    {
        switch (currentState)
        {
            case HamburgerBossState.Hamburger:
                cherryColliders.gameObject.SetActive(false);
                hamburgerColliders.gameObject.SetActive(true);
                rb.bodyType = RigidbodyType2D.Dynamic;
                break;
            case HamburgerBossState.Cherry:
                hamburgerColliders.gameObject.SetActive(false);
                cherryColliders.gameObject.SetActive(true);
                rb.bodyType = RigidbodyType2D.Kinematic;
                break;
            default:
                break;
        }
    }

    protected override void InstantDie()
    {
        Debug.Log("Hamburger Boss Instant Die");
    }
}
