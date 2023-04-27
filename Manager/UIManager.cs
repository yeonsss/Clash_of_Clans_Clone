using System;
using static Define;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private Dictionary<UI, GameObject> UiDict = new Dictionary<UI, GameObject>();
    public event EventHandler txtEvent;

    public bool isUiActive = false;

    public override void Init()
    {
        foreach (var uiName in Enum.GetNames(typeof(UI)))
        {
            UI uiType;
            if (uiName == "None") continue;
            
            var ui = GameObject.FindWithTag(uiName);
            if (ui == null)
            {
                ui = ResourceManager.instance.InstantiateDontDistroy($"UI/{uiName}");
                if (ui != null) ui.SetActive(false);
            }
            
            Enum.TryParse<UI>(uiName, out uiType);
            UiDict.Add(uiType, ui);
        }
    }

    public GameObject GetUIObject(UI ui)
    {
        UiDict.TryGetValue(ui, out var obj);
        return obj == null ? null : obj;
    }

    public void UIActiveSetting(UI uiType, bool active)
    {
        try
        {
            UiDict[uiType].SetActive(active);
        }
        catch (Exception e)
        {
            Debug.Log(e.StackTrace);
        }
        
    }

    public void TransformUI(UI uiType)
    {
        foreach (var uiT in UiDict.Keys)
        {
            if (uiT == uiType) UiDict[uiT].SetActive(true);
            else UiDict[uiT].SetActive(false);
        }
    }
    
}
