using BehaviorTree;
using System.Collections.Generic;
using UnityEngine;

public class MageController : MonsterController
{
    protected override void SetTree()
    {
        base.SetTree();
        var objTransform = transform;
        
        aiComponent.Root = new Sequence( new List<Node> {
            new SearchBuildLongRange(objTransform, aiComponent),
            
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
        var target = aiComponent.GetData("target");
        if (target != null)
        {
            var targetObj = target as GameObject;
            transform.LookAt(targetObj.transform);
            var missile = ResourceManager.instance.Instantiate("Particle/MageMissile");
            var mc = missile.GetComponent<HowitzerController>();
            mc.SetInfo(targetObj.transform, transform, AttackPoint, 5.0f);
        }
    }
    // public override void Attacked(float attackDamage)
    // {
    //     Hp = Mathf.Clamp(Hp - attackDamage, 0, MaxHp);
    //     Debug.Log($"Attacked : {Hp}");
    //     base.Attacked(attackDamage);
    // }

    // protected override void Die()
    // {
    //     base.Die();
    // }
    protected override void Move() { }
    protected override void Skill() { }
}
