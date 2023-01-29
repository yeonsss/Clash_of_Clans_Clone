using UnityEngine;
using static Define;
public class BuildCancelHandler : MonoBehaviour
{
    public void ClickEvent()
    {
        var bd = GetComponentInParent<BuildFinalChoice>();
        
        if (bd.obj != null)
        {
            UIManager.instance.UIActiveSetting(UI.GAMEUI, true);
            
            Destroy(bd.obj);
            Destroy(transform.parent.gameObject);
        }
    }
}
