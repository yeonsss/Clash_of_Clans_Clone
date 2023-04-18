using System;
using BehaviorTree;
using System.Collections.Generic;
using UnityEngine;

public class BatController : MonsterController
{
    protected override void SetTree()
    {
        base.SetTree();
        var objTransform = transform;

        aiComponent.Root = new Sequence(new List<Node>
        {
            new SearchBuildIgnoreWall(objTransform, aiComponent),
            
            new Selector(new List<Node>
            {
                new Sequence(new List<Node>()
                {
                    new CheckEnemyInAttackRange(objTransform, aiComponent),    
                    new AttackTarget(objTransform, aiComponent, Attack),
                }),
                new Sequence(new List<Node>()
                {
                    new MoveToTarget(objTransform, aiComponent),
                }), 
            }),
        });
    }

    protected override void Attack()
    {
        var target = aiComponent.GetData("target") as GameObject;
        if (target != null)
        {
            if (target.CompareTag("Building"))
                target.GetComponent<Build>().Attacked(AttackPoint);
            else
                target.GetComponent<MonsterController>().Attacked(AttackPoint);
        }
    }

    // public override void Attacked(float attackDamage)
    // {
    //     Hp = Mathf.Clamp(Hp - attackDamage, 0, MaxHp);
    //     Debug.Log($"Attacked bat{transform.name}: {Hp}");
    // }
    //
    // protected override void Die()
    // {
    //     base.Die();
    // }
    protected override void Move() { }
    protected override void Skill() { }
}
