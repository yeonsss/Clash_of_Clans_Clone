using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BehaviorTree;
using UnityEngine;
using static Define;

public class SearchBuildIgnoreWall : Node
{
    private Transform _transform;
    private AIComponent _rootTree;
    
    public SearchBuildIgnoreWall(Transform transform, AIComponent tree)
    {
        _transform = transform;
        _rootTree = tree;
    }

    public override NodeState Evaluate()
    {
        // var prevTarget = rootTree.GetData("target") as GameObject;
        // if (prevTarget != null && prevTarget.activeSelf == true)
        //     return NodeState.SUCCESS;
        
        var cPos = _transform.position;
        var posX = cPos.x;
        var posY = cPos.z;
        
        // ���� Ÿ���� ������ ���� ����� 3�༮�� ã�´�.
        var targetList = BattleManager.instance.GetNearBuild(posX, posY);
        
        if (targetList.Count < 1) return NodeState.FAILURE;

        GameObject bestObj = null;
        int minPoint = Int32.MaxValue;
        PathVecter2Int nextPos = null;

        foreach (var item in targetList.Select((value, index) => (value, index)))
        {
            // 3�༮ �� ���� �켱������ ���� �༮�� ���� Ÿ������ �����Ѵ�.
            if (item.index > 2) break;
            PathManager.instance.AStarIgnoreWall(_transform.gameObject, item.value);
            
            if (minPoint > PathManager.instance.pathCost)
            {
                minPoint = PathManager.instance.pathCost;
                bestObj = item.value;
                if (PathManager.instance.Root.Count > 1)
                {
                    nextPos = PathManager.instance.Root[1];    
                }
            }
        }

        if (_rootTree.CheckData("nextPos"))
        {
            Wtor(cPos.x, cPos.z, (int)WIDTH, out var rx, out var ry);
            var objCurrentPos = new Vector3(rx, cPos.y, ry);
            
            var prevVector = _rootTree.GetData("nextPos") as PathVecter2Int;
            var prevPos = new Vector3(prevVector.X, cPos.y, prevVector.Y);

            if (Vector3.Distance(objCurrentPos, prevPos) < 0.1)
            {
                SetTarget(bestObj, nextPos);
            }
        }
        else
        {
            SetTarget(bestObj, nextPos);
        }

        return NodeState.SUCCESS;
    }

    private void SetTarget(GameObject bestObj, PathVecter2Int nextPos)
    {
        if (bestObj != null) _rootTree.SetData("target", bestObj);
        _rootTree.SetData("nextPos", nextPos);  
    }
}
