using BehaviorTree;
using System.Collections.Generic;
using UnityEngine;

public class MageController : MonsterController
{
    protected override void SetTree()
    {
        aiComponent = new AIComponent(this);
        var objTransform = transform;
        
        aiComponent.Root = new Selector( new List<Node> {
            new Sequence( new List<Node> {
                new CheckEnemyInDetectRange(objTransform, aiComponent),
                new TaskGoToTarget(objTransform, aiComponent)
            }),
            new Sequence( new List<Node> { 
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
            transform.LookAt(targetTransform);
            var magicPrefabs = Resources.Load("Prefabs/Particle/MagicMissilePink") as GameObject;
            var maginObj = Instantiate(magicPrefabs, transform.position, Quaternion.identity);
            var mc = maginObj.GetComponent<ProjectileController>();
            mc.target = targetTransform;
            mc.Attacker = transform;
            mc.AttackPoint = AttackPoint;
        }
    }
    public override void Attacked(float attackDamage)
    {
        Hp = Mathf.Clamp(Hp - attackDamage, 0, MaxHp);
        Debug.Log($"Attacked : {Hp}");
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
