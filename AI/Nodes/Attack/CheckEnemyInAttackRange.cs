using BehaviorTree;
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
        
        GameObject target = rootTree.GetData("target") as GameObject;

        if (target == null || target.activeSelf == false)
        {
            rootTree.ClearData("target");
            Anim.SetBool("attackState", false);
            return NodeState.FAILURE;
        }

        var pos = _transform.position;
        var targetPos = target.transform.position;

        var width = pos.x - targetPos.x;
        var height = pos.z - targetPos.z;

        var radian = Mathf.Atan2(height, width);

        var localScale = target.transform.localScale;
        var targetWidth = localScale.x / 2;
        var targetHeight = localScale.z / 2;

        var radius = Mathf.Sqrt(targetWidth * targetWidth + targetHeight * targetHeight);

        var xPos = (Mathf.Cos(radian) * radius) + targetPos.x;
        var yPos = (Mathf.Sin(radian) * radius) + targetPos.z;

        var hitPoint = new Vector3(xPos, targetPos.y, yPos);

        if (Vector3.Distance(_transform.position, hitPoint) <= rootTree.monsterData.AttackRange)
        {
            Anim.SetFloat("speed", 0);
            return NodeState.SUCCESS;
        }
        
        Anim.SetBool("attackState", false);
        return NodeState.FAILURE;
    }
}
