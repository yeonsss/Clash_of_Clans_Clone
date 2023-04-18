using System;
using System.Linq;
using BehaviorTree;
using UnityEngine;
using static Define;

public class SearchBuildLongRange : Node
{
    private Transform _transform;
    private AIComponent _rootTree;

    public SearchBuildLongRange(Transform obj, AIComponent root)
    {
        _transform = obj;
        _rootTree = root;
    }

    public override NodeState Evaluate()
    {
        var cPos = _transform.position;
        var posX = cPos.x;
        var posY = cPos.z;
        
        var targetList = BattleManager.instance.GetNearBuild(posX, posY);
        if (targetList.Count < 1) return NodeState.FAILURE;
        
        GameObject bestObj = null;
        int minPoint = Int32.MaxValue;
        PathVecter2Int nextPos = null;

        var range = _transform.GetComponent<MonsterController>().AttackRange;
        
        foreach (var item in targetList.Select((value, index) => (value, index)))
        {
            // 3녀석 중 가장 우선순위가 높은 녀석을 메인 타깃으로 설정한다.
            if (item.index > 2) break;
            PathManager.instance.AStarRootVerArea(_transform.gameObject, item.value, range);
            
            if (minPoint > PathManager.instance.pathCost)
            {
                minPoint = PathManager.instance.pathCost;
                bestObj = PathManager.instance.targetList[0];
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
