using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class YSortManager : Singleton<YSortManager>
{
    private Dictionary<int, List<YSortable>> sortables = new Dictionary<int, List<YSortable>>();

    public void Register(YSortable sortable)
    {
        var layerValue = SortingLayer.GetLayerValueFromID(sortable.spriteRenderer.sortingLayerID);
        if (!sortables.ContainsKey(layerValue))
        {
            sortables[layerValue] = new List<YSortable>();
        }
        if(!sortables[layerValue].Contains(sortable))
        {
            sortables[layerValue].Add(sortable);
        }
    }

    public void OnSortingLayerChange(YSortable sortable)
    {
        Unregister(sortable);
        Register(sortable);
    }

    public void Unregister(YSortable sortable)
    {
        var layerValue = SortingLayer.GetLayerValueFromID(sortable.spriteRenderer.sortingLayerID);
        if (!sortables.ContainsKey(layerValue))
        {
            return;
        }
        sortables[layerValue].Remove(sortable);
    }

    void LateUpdate()
    {
        foreach (var layer in sortables.Keys)
        {
            // Sort by Y (lower = in front)
            sortables[layer].Sort((a, b) => b.transform.position.y.CompareTo(a.transform.position.y));

            int order = 0;
            foreach (var s in sortables[layer])
            {
                s.spriteRenderer.sortingOrder = order + s.BaseOffset;
                order += 1;
            }
        }
    }
}