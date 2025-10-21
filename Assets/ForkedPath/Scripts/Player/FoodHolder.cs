using System.Collections;
using UnityEngine;
public class FoodHolder : MonoBehaviour
{
    Entity entity;
    public FoodComboTracker foodComboTracker { get; private set; }

    private readonly static float foodPartKeep = 0.3f;

    public void Initialize(Entity entity, FoodRulesConfig foodRulesConfig)
    {
        foodComboTracker = new FoodComboTracker(entity, foodRulesConfig);
    }

    private void OnEnable()
    {
        GameEvents.Instance.OnEntityEaten += OnEntityEaten;
    }

    private void OnDisable()
    {
        GameEvents.Instance.OnEntityEaten -= OnEntityEaten;
    }

    void OnEntityEaten(EatingEventData e)
    {
        if(e.eater != entity || e.prey == null) return;
        var otherHolder = e.prey.GetComponent<FoodHolder>();

        EntityFoodType foodType;
        int foodCount = 1;

        if (otherHolder != null && otherHolder.foodComboTracker != null)
        {
            var otherType = otherHolder.foodComboTracker.CurrentType;
            foodType = otherType != EntityFoodType.None ? otherType : e.prey.foodType;
            foodCount = Mathf.Max(1, Mathf.FloorToInt(otherHolder.foodComboTracker.OverallCollectedCount() * foodPartKeep));
        }
        else
        {
            foodType = e.prey.foodType;
        }
        foodComboTracker.Collect(foodType, foodCount);
    }
}