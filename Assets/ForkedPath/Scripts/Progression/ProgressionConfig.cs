using System.Collections;
using System.Linq;
using UnityEngine;
using Sirenix;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName ="Game/ProgressionConfig")]
public class ProgressionConfig : ScriptableObject
{
    public ProgressionLevel baseLevel;
    public ProgressionBranch[] progressionBranches;

    public ProgressionLevel GetProgressionLevel(EntityFoodType type, int level)
    {
        if(level == 0)
            return baseLevel;

        foreach (var step in progressionBranches.FirstOrDefault(x=>x.foodType == type).levels)
        {
            if(step.level == level)
            {
                return step;
            }
        }
        return null;
    }
}

[System.Serializable]
public class ProgressionBranch
{
    public EntityFoodType foodType;
    public ProgressionLevel[] levels;
}

[System.Serializable]
public class ProgressionLevel
{
    public int level;
    public ProjectilesPattern projectilesPattern;
    public Color color = Color.white;
    public bool amoCapped;
    [ShowIf("amoCapped")]
    public int maxAmmo;
}
