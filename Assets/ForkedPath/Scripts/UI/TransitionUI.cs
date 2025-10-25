using System.Collections;
using UnityEngine;
using Coffee.UISoftMask;
using Pixelplacement;

public class TransitionUI : Singleton<TransitionUI>
{
    public SoftMask softMask;

    ITransparencyController transparencyController;

    const float duration = 0.6f;

    private void Awake()
    {
        transparencyController = Transparency.GetController(softMask);
    }

    /// <summary>
    /// becoming visible
    /// </summary>
    public void FadeIn()
    {
        Tween.Value(1f, 0f, x => transparencyController.Alpha = x, duration, 0f);
    }

    /// <summary>
    /// becoming invisible
    /// </summary>
    public void FadeOut()
    {
        Tween.Value(0f, 1f, x => transparencyController.Alpha = x, duration, 0f);
    }
}