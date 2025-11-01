using UnityEngine;

public class HamburgerBossAnimator : SimpleEntityAnimatorController
{
    private int TransformToHamburgerHash = Animator.StringToHash("TransformToHamburger");
    private int TransformToCherryHash = Animator.StringToHash("TransformToCherry");

    protected override void Update()
    {
        
    }
    
    public void TriggerTransformToHamburger()
    {
        anim.SetTrigger(TransformToHamburgerHash);
    }

    public void TriggerTransformToCherry()
    {
        anim.SetTrigger(TransformToCherryHash);
    }
}