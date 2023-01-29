using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Define;
public class BuildAcceptHandler : MonoBehaviour
{
    private void Update()
    {
        var bd = GetComponentInParent<BuildFinalChoice>();
        var button = GetComponentInChildren<Button>();
        if (bd.obj.GetComponent<Build>().m_CollisionCheck == false)
        {
            button.enabled = true;
        }
        else
        {
            button.enabled = false;
        }
    }

    public void ClickEvent()
    {
        var bd = GetComponentInParent<BuildFinalChoice>();
        if (bd.obj != null)
        {
            UIManager.instance.UIActiveSetting(UI.GAMEUI, true);
            
            var barPrefab = Resources.Load("Prefabs/WorldSpaceUI/BuildSliderBar");
            var bar = Instantiate(barPrefab, bd.obj.transform);
            bar.GetComponent<BuildTimeSliderHandler>().Init(bd.obj);
            bd.obj.GetComponent<Build>().buildActive = true;
            bd.obj.GetComponent<Build>().m_BuildChoiceComplete = true;
            Destroy(transform.parent.gameObject);
            // 자원 계산
        }
    }
}
