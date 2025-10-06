﻿using System.Collections;
using UnityEngine;
using System;
using Pixelplacement;

public class GameEvents : Singleton<GameEvents>
{
    public Action<FXEventData> OnFX;
    public Action<DamageEventData> OnDamage;
    public Action<DeathEventData> OnDeath;
    public Action<PlayerEnterTrigger> OnPlayerEnterTrigger;
}