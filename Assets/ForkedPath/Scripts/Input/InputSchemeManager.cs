using UnityEngine;
using Pixelplacement;

public class InputSchemeManager : Singleton<InputSchemeManager>
{
    public event System.Action<InputScheme> OnInputSchemeChanged;

    public InputScheme CycleInputScheme()
    {
        var config = GameConfig.Instance;
        if (config == null)
        {
            Debug.LogError("GameConfig instance is null. Cannot cycle input scheme.");
            return default; // Return a default value
        }

        var next = (int)config.InputScheme + 1;
        if (next > (int)InputScheme.Continuous) next = 0;
        config.InputScheme = (InputScheme)next;
        OnInputSchemeChanged?.Invoke(config.InputScheme);
        return config.InputScheme;
    }

    public InputScheme GetCurrentInputScheme()
    {
        var config = GameConfig.Instance;
        if (config == null)
        {
            Debug.LogError("GameConfig instance is null. Cannot get current input scheme.");
            return default; // Return a default value
        }
        return config.InputScheme;
    }
}