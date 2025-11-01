using UnityEngine;
using Pixelplacement;
using UnityEngine.InputSystem;


public class G : Singleton<G>
{
    public PlayerInput PlayerInput;
    public HamburgerFightController HamburgerFightController;
    public CanvasGroup stageStartUIGroup;
    public CanvasGroup deathUIGroup;
}
