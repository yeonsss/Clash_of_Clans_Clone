using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckEnemyInAttackRange : Node
{
    Transform _transform;
    private Animator Anim;
    private AIComponent rootTree;

    public CheckEnemyInAttackRange(Transform transform, AIComponent tree) {
        _transform = transform;
        Anim = transform.GetComponent<Animator>();
        rootTree = tree;
    }

    public override NodeState Evaluate()
    {
        
        Transform target = (Transform)rootTree.GetData("target");

        if (target == null)
        {
            Anim.SetBool("attackState", false);
            return NodeState.FAILURE;
        }

        if (Vector3.Distance(_transform.position, target.position) <= rootTree.monsterData.AttackRange)
        {
            Anim.SetBool("attackState", true);
            return NodeState.RUNNING;
        }
        else
        {
            Anim.SetBool("attackState", false);
            return NodeState.FAILURE;
        }
    }


}
