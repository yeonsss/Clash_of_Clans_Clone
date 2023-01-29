using UnityEngine;
using static Define;
public class BuildInputComponent : InputComponent
{
    protected override void MouseDown(GameObject obj)
    {
        // 최초 클릭 시 마우스의 offset을 구해 기록해 놓는다.
        mZcoord = Camera.main.WorldToScreenPoint(obj.transform.position).z;
        _mousePosOffset = obj.transform.position - GetMouseWorldPosition();
    }

    protected override void MouseDrag(GameObject obj)
    {
        // 콜라이더 체크 해제
        
        // 바닥에 놓을 수 있는 범위 (4각형) 표시
        
        // 마우스 커서 위치로 x, z 좌표가 바뀌어야 한다. 그럴려면 레이케스트해서 그 바닥의 좌표를 위치시켜야 하나??
        
        // 움직임을 1칸씩 움직이는 것으로 바꿔야 한다.
        Vector3 pos = GetMouseWorldPosition() + _mousePosOffset;
        pos.y = 0;
        var xsize = obj.GetComponent<Build>().XSize;
        var ysize = obj.GetComponent<Build>().YSize;

        if (xsize % 2 == 0 || ysize % 2 == 0)
        {
            pos.x = Mathf.Floor(pos.x) - 0.5f;
            pos.z = Mathf.Floor(pos.z) - 0.5f;
        }
        else
        {
            pos.x = Mathf.Floor(pos.x);
            pos.z = Mathf.Floor(pos.z);
        }
        
        
        if (pos.z < Y_MIN + 1 || pos.z >= Y_MAX - 1)
        {
            return;
        }
        
        if (pos.x < X_MIN + 1 || pos.x >= X_MAX - 1)
        {
            return;
        }
        
        obj.transform.position = pos;
    }
    
    protected override void MouseUp(GameObject obj)
    {
        Debug.Log("up");
        // BuildRange range = GetComponentInChildren<BuildRange>();
        // if (!range.isCollision)
        // {
        //     _active = true;
        // }
    }

}
