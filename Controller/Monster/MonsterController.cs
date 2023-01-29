using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : BaseController
{
    // BaseInfo
    public int serialCode = 1;
    protected AIComponent aiComponent;
    public bool aiActive = true;
    public int currentlevel = 0;

    public float Hp { get; set; }
    public float MaxHp { get; set; }
    public float AttackPoint { get; set; }
    public float MoveSpeed { get; set; }
    public float AttackRange { get; set; }
    public float DetectRange { get; set; }
    public float AttackCooldown { get; set; }
    public float SkillCooldown { get; set; }

    protected override void Start()
    {
        DataManager.instance.monsterDict.TryGetValue(serialCode, out var info);
        if (info == null) return;
        AttackCooldown = info.AttackCooldown;
        Hp = info.Levels[currentlevel].Hp;
        MaxHp = info.Levels[currentlevel].MaxHp;
        AttackPoint = info.Levels[currentlevel].Attack;
        MoveSpeed = info.MoveSpeed;
        AttackRange = info.AttackRange;
        DetectRange = info.DetectRange;
        SkillCooldown = info.SkillCooldown;
        SetTree();
    }

    protected override void Update()
    {
        if (aiComponent != null && aiActive == true) aiComponent.Generate();
        Die();
    }

    protected virtual void SetTree() { }
    public virtual void Attack() { }
    public virtual void Attacked(float attackDamage) { }

    public virtual void Die()
    {
    }
    public virtual void Move() { }
    public virtual void Skill() { }
    
}
