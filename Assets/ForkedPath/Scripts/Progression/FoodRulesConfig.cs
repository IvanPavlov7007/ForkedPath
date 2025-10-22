using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName ="Game/FoodRules")]
public class FoodRulesConfig : ScriptableObject
{
    public FoodUpgradeRule[] Rules;
}

[System.Serializable]
public class FoodUpgradeRule
{
    public EntityFoodType Type;
    public int[] Thresholds;
}