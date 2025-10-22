using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class CharacterColor : EntityComponent
{
    public void SetColor(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }

    protected override void InstantDie()
    {
    }
}