using Pixelplacement;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
public class Stage1 : MonoBehaviour
{
    private void Awake()
    {
        G.Instance.stageStartUIGroup.alpha = 0f;
        G.Instance.deathUIGroup.alpha = 0f;
        G.Instance.deathUIGroup.interactable = false;
        G.Instance.deathUIGroup.blocksRaycasts = false;
        GameEvents.Instance.OnPlayerGameOver += OnPlayerGameOver;
        GameEvents.Instance.OnDeath += onEntityDeath;
    }

    private void OnDisable()
    {
        GameEvents.Instance.OnPlayerGameOver -= OnPlayerGameOver;
        GameEvents.Instance.OnDeath -= onEntityDeath;
    }

    IEnumerator preStart()
    {
        var stageAlpha = Transparency.GetController(G.Instance.stageStartUIGroup);
        Tween.Value(0f, 1f, x => stageAlpha.Alpha = x, 0.2f, 1f);
        yield return new WaitForSeconds(2f);
        Tween.Value(1f, 0f, x => stageAlpha.Alpha = x, 0.2f, 0f);
        yield return new WaitForSeconds(0.2f);
    }

    IEnumerator death()
    {
        var deathAlpha = Transparency.GetController(G.Instance.deathUIGroup);
        G.Instance.deathUIGroup.interactable = true;
        G.Instance.deathUIGroup.blocksRaycasts = true;
        Tween.Value(0f, 1f, x => deathAlpha.Alpha = x, 0.2f, 0f);
        yield return new WaitForSeconds(2f);
    }

    IEnumerator Start()
    {
        yield return preStart();
        TransitionUI.Instance.FadeOut();
    }


    //convert this into a trigger

    void onEntityDeath(DeathEventData e)
    {
        if (e == null || e.entity == null) return;
        if (e.entity.Config.entityID.ToLowerInvariant().Equals("hamburger"))
        {
            StartCoroutine(END());
        }
    }

    IEnumerator END()
    {
        yield return new WaitForSeconds(4f);
        yield return EndingUI.Instance.ShowEndingUI();
    }


    void OnPlayerGameOver()
    {
        StartCoroutine(death());
    }

}