using Pixelplacement;
using UnityEngine;

public class HamburgerFightController : Singleton<HamburgerFightController>
{
    public Rect worldBoundariesMoverRect = new Rect(-5, -3, 10, 6);
    HamburgerController hamburgerController;
    public PlayerEnterTrigger playerEnterTrigger;

    private void Awake()
    {
        hamburgerController = FindFirstObjectByType<HamburgerController>();
    }

    private void OnEnable()
    {
        GameEvents.Instance.OnPlayerEnterTrigger += playerEnteredTrigger;
    }

    bool started = false;
    void playerEnteredTrigger(PlayerEnterTrigger playerEnterTrigger)
    {
        if(this.playerEnterTrigger != playerEnterTrigger || started) return;
        started = true;
        hamburgerController.StartFight();
    }


#if UNITY_EDITOR
    // Visualize boundaries in editor and during play (when selected)
    void OnDrawGizmosSelected()
    {
        var rect = worldBoundariesMoverRect;
        Vector3 a = new Vector3(rect.xMin, rect.yMin, 0);
        Vector3 b = new Vector3(rect.xMax, rect.yMin, 0);
        Vector3 c = new Vector3(rect.xMax, rect.yMax, 0);
        Vector3 d = new Vector3(rect.xMin, rect.yMax, 0);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);
    }
#endif
}