using UnityEngine;
using static Define;
public class BuildInputComponent : InputComponent
{
    protected override void MouseDown(GameObject obj)
    {
        // ���� Ŭ�� �� ���콺�� offset�� ���� ����� ���´�.
        mZcoord = Camera.main.WorldToScreenPoint(obj.transform.position).z;
        _mousePosOffset = obj.transform.position - GetMouseWorldPosition();
    }

    protected override void MouseDrag(GameObject obj)
    {
        // �ݶ��̴� üũ ����
        
        // �ٴڿ� ���� �� �ִ� ���� (4����) ǥ��
        
        // ���콺 Ŀ�� ��ġ�� x, z ��ǥ�� �ٲ��� �Ѵ�. �׷����� �����ɽ�Ʈ�ؼ� �� �ٴ��� ��ǥ�� ��ġ���Ѿ� �ϳ�??
        
        // �������� 1ĭ�� �����̴� ������ �ٲ�� �Ѵ�.
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
