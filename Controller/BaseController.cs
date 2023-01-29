using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseController : MonoBehaviour
{
    protected event EventHandler m_EvtListener;

    protected List<IComponent> m_ComponentList;
    
    protected virtual void SetComponent()
    {
        // m_ComponentList.Add();
    }

    protected void SetEvent()
    {
        foreach (var component in m_ComponentList)
        {
            m_EvtListener += component.OnEventExecute;
        }
    }

    protected virtual void Awake()
    {
        m_ComponentList = new List<IComponent>();
        SetComponent();
        SetEvent();
    }

    protected virtual void Update() { }
    protected virtual void Start() { }

    protected virtual void OnMouseDown()
    {
        if (m_EvtListener == null) return;
        m_EvtListener(this, new CustomInputEvent(InputEventType.MouseDown, gameObject));
    }
    
    protected virtual void OnMouseDrag()
    {
        if (m_EvtListener == null) return;
        m_EvtListener(this, new CustomInputEvent(InputEventType.MouseDrag, gameObject));
    }
    
    protected virtual void OnMouseUp()
    {
        if (m_EvtListener == null) return;
        m_EvtListener(this, new CustomInputEvent(InputEventType.MouseUp, gameObject));
    }
}
