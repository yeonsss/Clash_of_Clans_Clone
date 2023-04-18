using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : BaseController
{
    // BaseInfo
    public MonsterName type;
    protected AIComponent aiComponent;
    public bool aiActive = true;
    public int currentlevel = 0;
    
    public float AttackPoint { get; set; }
    public float MoveSpeed { get; set; }
    public float AttackRange { get; set; }
    public float DetectRange { get; set; }
    public float AttackCooldown { get; set; }
    public float SkillCooldown { get; set; }
    
    protected override void Start()
    {
        DataManager.instance.monsterDict.TryGetValue(type, out var info);
        if (info == null) return;
        AttackCooldown = info.AttackCooldown;
        Hp = info.Levels[currentlevel].Hp;
        MaxHp = info.Levels[currentlevel].Hp;
        AttackPoint = info.Levels[currentlevel].Attack;
        MoveSpeed = info.MoveSpeed;
        AttackRange = info.AttackRange;
        SkillCooldown = info.SkillCooldown;
        SetTree();
    }
    
    IEnumerator DisplayAndDelete()
    {
        var check = aiComponent.CheckData("displayTargetingImage");
        if (!check)
            yield break;

        var bestObj = aiComponent.GetData("target") as GameObject;
        if (bestObj == null) yield break;
        
        var targetPos = bestObj.transform.position;
        var image =ResourceManager.instance.Instantiate("WorldSpaceUI/TargetingImage");
        image.transform.position = new Vector3(targetPos.x + 0.5f, targetPos.y + 0.5f, targetPos.z + 0.5f);
        aiComponent.ClearData("displayTargetingImage");
        yield return new WaitForSeconds(2.0f);
        Destroy(image);
    }

    protected override void Update()
    {
        if (aiComponent != null && aiActive == true) aiComponent.Generate();
        StartCoroutine(DisplayAndDelete());
        base.Update();
    }

    protected virtual void SetTree()
    {
        aiComponent = new AIComponent(this);
        aiComponent.SetData("displayTargetingImage", 1);
    }

    public override void Attacked(float attackDamage)
    {
        base.Attacked(attackDamage);
    }

    protected override void Die()
    {
        base.Die();
    }
    
    protected virtual void Attack() { }
    protected virtual void Move() { }
    protected virtual void Skill() { }
}
