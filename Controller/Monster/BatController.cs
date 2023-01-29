using System;
using BehaviorTree;
using System.Collections.Generic;
using UnityEngine;

public class BatController : MonsterController
{
    protected override void SetTree()
    {
        aiComponent = new AIComponent(this);
        var objTransform = transform;

        aiComponent.Root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new Selector(new List<Node>()
                {
                    // 유저의 몬스터 스폰 기능이 추가되면 주석 해제
                    // new CheckEnemyInDetectRange(objTransform, aiComponent),
                    new CheckEnemyBuild(objTransform, aiComponent),
                }),
                new TaskGoToTarget(objTransform, aiComponent)
            }),
            new Sequence(new List<Node>
            {
                new CheckEnemyInAttackRange(objTransform, aiComponent),
                new TaskAttack(objTransform, aiComponent, Attack),
            }),
        });
    }

    public override void Attack()
    {
        var target = aiComponent.GetData("target");
        if (target != null)
        {
            var targetTransform = target as Transform;
            if (targetTransform.CompareTag("Building"))
            {
                if (targetTransform != null)
                    targetTransform.GetComponent<Build>().Attacked(AttackPoint);
            }
            else
            {
                if (targetTransform != null)
                    targetTransform.GetComponent<MonsterController>().Attacked(AttackPoint);
            }


        }
    }

    public override void Attacked(float attackDamage)
    {
        Hp = Mathf.Clamp(Hp - attackDamage, 0, MaxHp);
        Debug.Log($"Attacked bat{transform.name}: {Hp}");
    }

    public override void Die()
    {
        if (Hp <= 0)
        {
            if (transform.CompareTag("Enemy"))
            {
                BattleManager.instance.MonsterCount--;
            }

            Destroy(gameObject);
        }
    }
    public override void Move() { }
    public override void Skill() { }
}
