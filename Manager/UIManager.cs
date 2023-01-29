using System;
using static Define;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private Dictionary<UI, GameObject> UiDict = new Dictionary<UI, GameObject>();
    public event EventHandler txtEvent;
    
    public void Init()
    {
        foreach (var uiName in Enum.GetNames(typeof(UI)))
        {
            UI uiType;
            if (uiName == "None") continue;
            
            var ui = GameObject.FindWithTag(uiName);
            if (ui == null)
            {
                var uiPrefab = Resources.Load($"Prefabs/UI/{uiName}");
                if (uiPrefab == null) continue;
                ui = Instantiate(uiPrefab) as GameObject;
                if (ui != null) ui.SetActive(false);
            }
            
            Enum.TryParse<UI>(uiName, out uiType);
            UiDict.Add(uiType, ui);
        }
    }

    public GameObject GetUIObject(UI ui)
    {
        UiDict.TryGetValue(ui, out var obj);
        if (obj == null) return null;
        return obj;
    }

    public void UIActiveSetting(UI uiType, bool active)
    {
        UiDict[uiType].SetActive(active);
    }
    
}
