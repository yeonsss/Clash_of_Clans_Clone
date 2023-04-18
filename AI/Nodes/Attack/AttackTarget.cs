using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;
using JetBrains.Annotations;
using Random = UnityEngine.Random;

public class AttackTarget : Node
{
    private Animator Anim;
    private AIComponent rootTree;
    private Transform objTransform;
    [CanBeNull] private Action AttackFunc;

    public AttackTarget(Transform transform, AIComponent tree, Action func)
    {
        objTransform = transform;
        Anim = transform.GetComponent<Animator>();
        rootTree = tree;
        AttackFunc = func;
    }

    public override NodeState Evaluate()
    {
        GameObject target = rootTree.GetData("target") as GameObject;
        if (target == null || target.activeSelf == false)
        {
            Anim.SetBool("attackState", false);
            rootTree.ClearData("target");
            return NodeState.FAILURE;
        }
        
        if (rootTree.CoolCheck[AIComponent.COOLID.attack] == true) {
            rootTree.SetCoolFalse(AIComponent.COOLID.attack);
            Debug.Log("attack!!");
            Anim.SetBool("attackState", true);
            objTransform.LookAt(target.transform);
            if (AttackFunc != null) AttackFunc();
        }
        
        return NodeState.SUCCESS;
    }

}
