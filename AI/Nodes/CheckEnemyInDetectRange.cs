using BehaviorTree;
using UnityEngine;

public class CheckEnemyInDetectRange : Node
{
    Transform _transform;
    private AIComponent rootTree;

    public CheckEnemyInDetectRange(Transform transform, AIComponent tree)
    {
        _transform = transform;
        rootTree = tree;
    }

    public override NodeState Evaluate()
    {
        if (rootTree.CheckData("target") == true)
        {
            return NodeState.SUCCESS;
        }

        LayerMask mask = LayerMask.GetMask("Object");
        
        Collider[] hitCollider = Physics.OverlapSphere(_transform.position, rootTree.monsterData.DetectRange, mask);

        foreach (Collider co in hitCollider)
        {
            if (co.transform == _transform) continue;
            if (_transform.CompareTag("Enemy"))
            {
                if (co.CompareTag("Ally"))
                {
                    rootTree.SetData("target", co.transform);
                    return NodeState.SUCCESS;
                }
            }
            else
            {
                if (co.CompareTag("Enemy"))
                {
                    rootTree.SetData("target", co.transform);
                    return NodeState.SUCCESS;
                }
            }
            
        }
        return NodeState.FAILURE;
    }
}
