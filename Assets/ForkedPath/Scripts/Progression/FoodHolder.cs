using System.Collections;
using UnityEngine;
[DisallowMultipleComponent]
public class FoodHolder : MonoBehaviour
{
    public FoodComboTracker FoodComboTracker { get; private set; }

    public void Initialize(FoodComboTracker comboTracker)
    {
        FoodComboTracker = comboTracker;
    }
}