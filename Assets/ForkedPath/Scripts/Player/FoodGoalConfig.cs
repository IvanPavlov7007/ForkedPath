using System.Collections;
using UnityEngine;
using Sirenix;
using Sirenix.OdinInspector;

public class FoodGoalConfig : ScriptableObject
{
    public string goalName;
    public EntityFoodType[] combination;
    public FoodGoalConfig nextGoal;
}