using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseController : MonoBehaviour
{
    public float Hp { get; set; }
    public float MaxHp { get; set; }
    
    protected event EventHandler m_EvtListener;
    protected List<IComponent> m_ComponentList;
    protected HpBarController HpBar;
    
    
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
        SetHpBar();
    }

    private void SetHpBar()
    {
        var obj = ResourceManager.instance.Instantiate("WorldSpaceUI/HpBar");
        HpBar = obj.GetComponent<HpBarController>();
        HpBar.SetHpTarget(gameObject.transform);
        obj.SetActive(false);
    }
    
    protected void UpdateHpBar()
    {
        if (MaxHp > Hp)
        {
            if (HpBar.gameObject.activeSelf == false) HpBar.gameObject.SetActive(true);
        }
        
        HpBar.SetHpValue(MaxHp, Hp);
    }

    protected virtual void Update()
    {
        Die();
    }
    protected virtual void Start() { }

    public virtual void Attacked(float attackDamage)
    {
        Hp = Mathf.Clamp(Hp - attackDamage, 0, MaxHp);
        Debug.Log($"Attacked bat{transform.name}: {Hp}");
        UpdateHpBar();
    }

    protected virtual void Die()
    {
        if (Hp <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
