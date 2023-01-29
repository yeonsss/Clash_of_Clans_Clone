using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using static Define;

public class TaskGoToTarget : Node
{
    private Rigidbody rigid;
    private Transform transform;
    
    private Animator Anim;
    private AIComponent rootTree;

    public TaskGoToTarget(Transform transform, AIComponent tree)
    {
        this.transform = transform;
        rigid = transform.GetComponent<Rigidbody>();
        Anim = transform.GetComponent<Animator>();
        rootTree = tree;
    }

    public override NodeState Evaluate()
    {

        Transform target = (Transform)rootTree.GetData("target");

        if (target == null)
        {
            Anim.SetFloat("speed", 0);
            rootTree.ClearData("target");
            return NodeState.FAILURE;
        }

        if (Vector3.Distance(transform.position, target.position) > rootTree.monsterData.AttackRange)
        {
            // 타깃이 벽이면 그냥 다가간다. A* 필요없음
            var cPos = transform.position;
            float posX = cPos.x;
            float posY = cPos.z;
            var newPosition = new Vector3(0, cPos.y, 0);
            
            float targetPosX = target.position.x;
            float targetPosY = target.position.z;   
            
            //여기서 이제 A* 알고리즘 사용
            if (target.CompareTag("Building"))
            {
                if(target.GetComponent<Build>().bType == BuildType.Wall)
                {
                    Wtor(posX, posY, (int)WIDTH, out var posRX, out var posRY);
                    Wtor(targetPosX, targetPosY, (int)WIDTH, out var targetPosRX, out var targetPosRY);
            
                    PathManager.instance.AStarPath(posRX, posRY, targetPosRX, targetPosRY, new Vector2Int(targetPosRX, targetPosRY));

                    if (PathManager.instance.Root.Count < 2)
                    {
                        Anim.SetFloat("speed", 0);
                        return NodeState.FAILURE;
                    }
                    var path = PathManager.instance.Root[1];
                    Rtow(path.x, path.y, (int)WIDTH, out var pathX, out var pathY);
                    newPosition.x = pathX;
                    newPosition.z = pathY;
                    
                    Anim.SetFloat("speed", rootTree.monsterData.MoveSpeed * Time.deltaTime);
                    transform.position = Vector3.MoveTowards(
                        transform.position, newPosition, rootTree.monsterData.MoveSpeed * Time.deltaTime);
                    transform.LookAt(newPosition);
                    state = NodeState.RUNNING;
                }
                else
                {

                    Wtor(posX, posY, (int)WIDTH, out var posRX, out var posRY);
                    Wtor(targetPosX, targetPosY, (int)WIDTH, out var targetPosRX, out var targetPosRY);
            
                    PathManager.instance.AStarPath(posRX, posRY, targetPosRX, targetPosRY, null);

                    if (PathManager.instance.Root.Count < 2)
                    {
                        Anim.SetFloat("speed", 0);
                        return NodeState.FAILURE;
                    }
                    var path = PathManager.instance.Root[1];
                    Rtow(path.x, path.y, (int)WIDTH, out var pathX, out var pathY);
                    newPosition.x = pathX;
                    newPosition.z = pathY;
            

                    Anim.SetFloat("speed", rootTree.monsterData.MoveSpeed * Time.deltaTime);
                    transform.position = Vector3.MoveTowards(
                        transform.position, newPosition, rootTree.monsterData.MoveSpeed * Time.deltaTime);
                    transform.LookAt(newPosition);
                    state = NodeState.RUNNING;
                }
            }
            else
            {

                Wtor(posX, posY, (int)WIDTH, out var posRX, out var posRY);
                Wtor(targetPosX, targetPosY, (int)WIDTH, out var targetPosRX, out var targetPosRY);
            
                PathManager.instance.AStarPath(posRX, posRY, targetPosRX, targetPosRY, null);

                if (PathManager.instance.Root.Count < 2)
                {
                    Anim.SetFloat("speed", 0);
                    return NodeState.FAILURE;
                }
                var path = PathManager.instance.Root[1];
                Rtow(path.x, path.y, (int)WIDTH, out var pathX, out var pathY);
                newPosition.x = pathX;
                newPosition.z = pathY;
            

                Anim.SetFloat("speed", rootTree.monsterData.MoveSpeed * Time.deltaTime);
                transform.position = Vector3.MoveTowards(
                    transform.position, newPosition, rootTree.monsterData.MoveSpeed * Time.deltaTime);
                transform.LookAt(newPosition);
                state = NodeState.RUNNING;
            }
        }
        else
        {
            Anim.SetFloat("speed", 0);
            state = NodeState.FAILURE;
        }

        

        return state;
    }

}
