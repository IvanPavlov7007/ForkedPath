using System.Collections;
using UnityEngine;
public class FoodUpgradeEventData 
{
    public string foodName;
    public bool wasReset;

    public FoodUpgradeEventData(string foodName)
    {
        this.foodName = foodName;
    }

    public FoodUpgradeEventData(bool wasReset)
    {
        this.wasReset = wasReset;
    }
}