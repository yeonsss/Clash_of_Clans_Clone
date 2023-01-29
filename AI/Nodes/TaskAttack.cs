using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;
using JetBrains.Annotations;
using Random = UnityEngine.Random;

public class TaskAttack : Node
{
    private Animator Anim;
    private AIComponent rootTree;
    private Transform objTransform;
    [CanBeNull] private Action AttackFunc;

    private int GetRandomAtatckPos(int posCount)
    {
        return Random.Range(1, posCount);
    }

    public TaskAttack(Transform transform, AIComponent tree, Action func)
    {
        objTransform = transform;
        Anim = transform.GetComponent<Animator>();
        rootTree = tree;
        AttackFunc = func;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)rootTree.GetData("target");
        if (target == null)
        {
            Anim.SetBool("attackState", false);
            rootTree.ClearData("target");
            return NodeState.SUCCESS;
        }
        
        if (rootTree.CoolCheck[AIComponent.COOLID.attack] == true) {
            rootTree.SetCoolFalse(AIComponent.COOLID.attack);
            Debug.Log("attack!!");
            objTransform.LookAt(target);
            if (AttackFunc != null) AttackFunc();
        }

        state = NodeState.RUNNING;
        return state;
    }

}
