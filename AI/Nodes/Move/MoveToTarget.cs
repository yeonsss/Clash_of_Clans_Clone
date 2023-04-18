using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using static Define;

public class MoveToTarget : Node
{
    private Transform _transform;
    private Animator _anim;
    private AIComponent _rootTree;

    public MoveToTarget(Transform transform, AIComponent tree)
    {
        this._transform = transform;
        _anim = transform.GetComponent<Animator>();
        _rootTree = tree;
    }

    public override NodeState Evaluate()
    {
        var target = _rootTree.GetData("target") as GameObject;

        if (target == null || target.activeSelf == false)
        {
            _anim.SetFloat("speed", 0);
            _rootTree.ClearData("nextPos");
            _rootTree.ClearData("target");
            return NodeState.SUCCESS;
        }

        if (_rootTree.CheckData("nextPos"))
        {
            var targetRoot = _rootTree.GetData("nextPos") as PathVecter2Int;

            Rtow(targetRoot.X, targetRoot.Y, (int)WIDTH, out var posX, out var posY);
        
            var newPosition = new Vector3(posX, _transform.position.y, posY);

            if (Vector3.Distance(_transform.position, newPosition) > 0)
            {
                _anim.SetFloat("speed", _rootTree.monsterData.MoveSpeed * Time.deltaTime);
                _transform.position = Vector3.MoveTowards(
                    _transform.position, newPosition, _rootTree.monsterData.MoveSpeed * Time.deltaTime);
                _transform.LookAt(newPosition);
                return NodeState.RUNNING;
            }
            
            return NodeState.SUCCESS;
        }
        
        return NodeState.FAILURE;
    }
}
