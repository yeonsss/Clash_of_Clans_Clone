using static Define;
using UnityEngine;
using System;

public class InputComponent : IComponent
{
    protected Vector3 _mousePosOffset;
    protected float mZcoord;

    public void OnEventExecute(object sender, EventArgs arg)
    {
        //TODO: 배튼 씬이면 동작 안되게 한다.
        var evt = arg as CustomInputEvent;

        if (evt != null)
        {
            switch (evt.type)
            {
                case InputEventType.MouseDown :
                    MouseDown(evt.obj);
                    break;
                case InputEventType.MouseDrag :
                    MouseDrag(evt.obj);
                    break;
                case InputEventType.MouseUp :
                    MouseUp(evt.obj);
                    break;
            }    
        }
    }

    protected virtual void MouseDown(GameObject obj) { }
    protected virtual void MouseDrag(GameObject obj) { }
    protected virtual void MouseUp(GameObject obj) { }
}
