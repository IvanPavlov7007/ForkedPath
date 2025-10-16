using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName ="Game/Boss/CherryPhase")]
public class CherryPhaseConfig : BossPhaseConfig
{
    public ProjectilesPattern pattern;
    public float shootTime = 0.5f;
    public float waitTime;


    AutomaticShooter shooter;
    SimpleTimer skipTimer = new SimpleTimer();
    bool shooting = false;
    public override bool OnFixedUpdate(HamburgerController controller)
    {
        if(shooting)
        {
            if (shooter == null) Debug.LogError($"{name}'s shooter is null");
            else
            {
                shooter.Shoot(controller.Direction, controller.CherryLocalPosition);
            }
        }

        return (skipTimer.isSet() && skipTimer.tick(Time.deltaTime))
            || controller.entity.Health.CurrentHealth < endHealth ;
    }

    public override void OnPhaseEnd(HamburgerController controller)
    {
        controller.StopAllCoroutines();
        if(shooter != null)
        {
            shooter.StopShooting();
        }
        shooting = false;
        skipTimer.clear();
    }

    public override void OnPhaseStart(HamburgerController controller)
    {
        controller.StartCoroutine(PhaseSequence(controller));
    }

    IEnumerator PhaseSequence(HamburgerController controller)
    {
        yield return controller.TransitionToCherryState();

        if (timeLimit > 0)
        {
            skipTimer.setOnce(timeLimit);
        }
        shooter = AutomaticShooter.ReloadAutomaticShooter(controller.gameObject, pattern);
        while (true)
        {
            yield return Wait();
            yield return Shoot(controller, shooter);
        }
    }

    IEnumerator Shoot(HamburgerController controller, AutomaticShooter shooter)
    {
        shooting = true;
        yield return new WaitForSeconds(shootTime);
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
    }
}