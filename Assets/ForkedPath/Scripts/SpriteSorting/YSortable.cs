using System.Collections;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class YSortable : MonoBehaviour
{
    public int BaseOffset;
    [HideInInspector] public SpriteRenderer spriteRenderer;

    public int sortingLayer
    {
        get => spriteRenderer.sortingLayerID;
        set
        {
            if (spriteRenderer.sortingLayerID != value)
            {
                spriteRenderer.sortingLayerID = value;
                YSortManager.Instance?.OnSortingLayerChange(this);
            }
        }
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        YSortManager.Instance?.Register(this);
    }

    void OnDestroy() => YSortManager.Instance?.Unregister(this);
}