using UnityEngine;
using static Define;
public class BuildCancelHandler : MonoBehaviour
{
    public void ClickEvent()
    {
        var bd = GetComponentInParent<BuildFinalChoice>();
        
        if (bd.obj != null)
        {
            UIManager.instance.TransformUI(UI.GAMEUI);
            
            Destroy(bd.obj);
            Destroy(transform.parent.gameObject);
        }
    }
}
