using Pixelplacement;
using System.Collections;
using UnityEngine;

public class HitAndBlinkSpriteVisuals : EntityComponent
{
    protected override void InstantDie()
    {
    }

    protected override void OnHit(DamageEventData damageEventData)
    {
        if (body == null || spriteRenderer == null) return;
        //activeTweens.Add(Tween.Shake(body, body.localPosition, new Vector2(1f, 0.2f), 0.1f, 0f));
        activeTweens.Add(Tween.Color(spriteRenderer, Color.red, 0.05f, 0f));
        activeTweens.Add(Tween.Color(spriteRenderer, Color.white, 0.05f, 0.05f));
    }

    protected override void OnInvincibility(InvincibilityEventData e)
    {
        if (spriteRenderer == null) return;

        float blinkInterval = 0.1f;
        int blinkCount = Mathf.CeilToInt(e.Duration / blinkInterval);

        IEnumerator BlinkCoroutine()
        {
            for (int i = 0; i < blinkCount; i++)
            {
                spriteRenderer.enabled = false;
                yield return new WaitForSeconds(blinkInterval / 2f);
                spriteRenderer.enabled = true;
                yield return new WaitForSeconds(blinkInterval / 2f);
            }
            spriteRenderer.enabled = true;
        }

        StartCoroutine(BlinkCoroutine());
    }

}