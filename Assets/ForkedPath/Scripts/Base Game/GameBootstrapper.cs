using Pixelplacement;
using System.Collections;
using UnityEngine;

public class GameBootstrapper : Singleton<GameBootstrapper>
{
    private void Awake()
    {
        GameConfig.Load();
    }
}