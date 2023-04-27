using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using static Define;

public class LoginSceneInitializer : Initializer
{
    public override void Init()
    {
        if (Camera.main == null)
        {
            ResourceManager.instance.InstantiateDontDistroy("Main Camera");    
        }
    }
}
