using System.Collections;
using UnityEngine;

public class BossController : EntityComponent
{
    [SerializeField] private BossPhaseConfig[] phases;
    private int currentPhaseIndex = 0;
    private BossPhaseConfig currentPhase;

    private void Start()
    {
        StartPhase(0);
    }

    private void StartPhase(int index)
    {
        if (index >= phases.Length)
        {
            OnBossDefeated();
            return;
        }

        currentPhase = phases[index];
        currentPhase.behaviour.Begin(this);
    }

    private void Update()
    {
        if (currentPhase != null)
        {
            if (currentPhase.behaviour.Update(this))
            {
                // phase finished
                StartCoroutine(TransitionToNextPhase());
            }
        }
    }

    private IEnumerator TransitionToNextPhase()
    {
        yield return currentPhase.DoTransitionOut();
        currentPhaseIndex++;
        StartPhase(currentPhaseIndex);
    }

    private void OnBossDefeated()
    {
        // spawn fireworks etc.
    }
}
