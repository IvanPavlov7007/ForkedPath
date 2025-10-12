using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class SimpleEntityAnimatorController : EntityVisualsBase
{
    protected PlayerController playerController;
    protected Animator anim;
    protected int deadHash = Animator.StringToHash("Dead");
    protected int shootHash = Animator.StringToHash("Shoot");
    protected int walkingHash = Animator.StringToHash("Walking");
    protected int XHash = Animator.StringToHash("X");
    protected int YHash = Animator.StringToHash("Y");


    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    protected virtual void Update()
    {
        anim.SetFloat(XHash, playerController.CurrentDirectionVector.x);
        anim.SetFloat(YHash, playerController.CurrentDirectionVector.y);
        anim.SetBool(walkingHash, playerController.moving);
    }

    public virtual void Shoot()
    {
        anim.SetTrigger(shootHash);
    }

    protected override void OnDeath(DeathEventData deathEventData)
    {
        anim.SetBool(deadHash, true);
    }
}