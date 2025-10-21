using UnityEngine;
using Pixelplacement;

public class CorpseManager : Singleton<CorpseManager>
{
    [SerializeField] float hideDuration = 0.2f;
    private void OnEnable()
    {
        GameEvents.Instance.OnEntityEaten += onEaten;
    }

    private void OnDisable()
    {
        GameEvents.Instance.OnEntityEaten -= onEaten;
    }

    void onEaten(EatingEventData e)
    {
        if (e == null || e.prey == null) return;

        Destroy(e.prey.gameObject, hideDuration);
    }
}
