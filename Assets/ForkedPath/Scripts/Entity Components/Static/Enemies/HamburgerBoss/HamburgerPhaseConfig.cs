using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Boss/HamburgerPhase")]
public class HamburgerPhaseConfig : BossPhaseConfig
{
    public int attacksCount;
    public AnimationCurve moveAttackCurve;
    public float moveAttackSpeed;
    public float waitTime;

    public override bool OnFixedUpdate(HamburgerController controller)
    {
        return initHealthDifferenceCondition(controller);
    }

    public override void OnPhaseEnd(HamburgerController controller)
    {
        controller.StopAllCoroutines();
        BetweenBoundariesMover mover = controller.GetComponent<BetweenBoundariesMover>();
        if (mover != null)
        {
            mover.Stop();
            Destroy(mover);
        }
    }

    public override void OnPhaseStart(HamburgerController controller)
    {
        base.OnPhaseStart(controller);
        initHealth = controller.entity.Health.CurrentHealth;
        controller.StartCoroutine(PhaseSequence(controller));
    }

    IEnumerator PhaseSequence(HamburgerController controller)
    {
        yield return controller.TransitionToHamburgerState();
        BetweenBoundariesMover betweenBoundariesMover;
        if(!controller.TryGetComponent<BetweenBoundariesMover>(out betweenBoundariesMover))
        {
            betweenBoundariesMover = controller.AddComponent<BetweenBoundariesMover>();
        }
        betweenBoundariesMover.SetBoundaries(HamburgerFightController.Instance.worldBoundariesMoverRect);

        while (true)
        {
            yield return Wait();
            for(int i = 0; i < attacksCount; i++)
            {
                yield return MoveAttack(controller, betweenBoundariesMover);
            }
        }
    }

    IEnumerator MoveAttack(HamburgerController controller, BetweenBoundariesMover mover)
    {
        bool destinationReached = false;
        Action onDestinationReached = () => destinationReached = true;
        mover.destinationReached += onDestinationReached;
        Vector2? distToPlayer = null;
        while(distToPlayer == null)
        {
            distToPlayer = controller.DistanceToPlayer();
            yield return null;
        }
        mover.Move(distToPlayer.Value, moveAttackSpeed, moveAttackCurve);
        yield return new WaitUntil(() => destinationReached);
        mover.destinationReached -= onDestinationReached;
    }

    IEnumerator Wait()
    {
        // FX of preparing to attack
        yield return new WaitForSeconds(waitTime);
    }
}