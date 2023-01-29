using System;
using UnityEngine;
using static Define;

public class CustomInputEvent : EventArgs
{
    public InputEventType type;
    public GameObject obj;

    public CustomInputEvent(InputEventType t, GameObject target)
    {
        type = t;
        obj = target;
    }
}
