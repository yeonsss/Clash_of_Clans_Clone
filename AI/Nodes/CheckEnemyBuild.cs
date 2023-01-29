using BehaviorTree;
using UnityEngine;
using static Define;

public class CheckEnemyBuild : Node
{
    Transform transform;
    private AIComponent rootTree;
    
    public CheckEnemyBuild(Transform transform, AIComponent tree)
    {
        this.transform = transform;
        rootTree = tree;
    }

    public override NodeState Evaluate()
    {
        var cPos = transform.position;
        float posX = cPos.x;
        float posY = cPos.z;
        var buildList = BattleManager.instance.NeerBuildingList(posX, posY);

        if (rootTree.CheckData("target"))
        {
            return NodeState.SUCCESS;
        }
        
        // builds���� ���� �ƴ� �ǹ��鸸 �����;� �Ѵ�.
        foreach (var build in buildList)
        {
            var position = build.Item2.transform.position;
            var targetPosX = position.x;
            var targetPosY = position.z;
            Wtor(posX, posY, (int)WIDTH, out var posRX, out var posRY);
            Wtor(targetPosX, targetPosY, (int)WIDTH, out var targetPosRX, out var targetPosRY);
            
            var check = PathManager.instance.AStarPath(posRX, posRY, targetPosRX, targetPosRY, null);
            if (check == false) continue;

            rootTree.SetData("target", build.Item2.transform);
            return NodeState.SUCCESS;
        }

        if (buildList.Count > 0)
        {
            LayerMask mask = LayerMask.GetMask("Object");
            var nearTargetPosition = buildList[0].Item2.transform.position;
            
            // ��� ��ġ - �� ��ġ
            if (Physics.Raycast(cPos, (nearTargetPosition - cPos).normalized, out var hit,  Mathf.Infinity, mask))
            {
                if (hit.transform.CompareTag("Building"))
                {
                    rootTree.SetData("target", hit.transform);
                    return NodeState.SUCCESS;    
                }
            }
        }
        
        return NodeState.FAILURE;
    }
}
