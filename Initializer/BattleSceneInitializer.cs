using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class BattleSceneInitializer : Initializer
{
    public override void Init()
    {
        BattleManager.instance.EnterBattlePage();
    }
}
