using System;
using UnityEngine;
using UnityEngine.UI;
using static Define;
public class RadeUI : BaseUI
{
    private enum Buttons
    {
        ExitButton,
        ArmyTab,
        MonsterTab,
        MagicTab,
    }

    private enum Panels
    {
        MonsterPanel,
        ArmyPanel,
        MagicPanel
    }

    private Panels _lastOpenPanel;

    private void Awake()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Transform>(typeof(Panels));

        Get<Button>((int)Buttons.ExitButton).gameObject.BindEvent(ExitButtonHandler);
        Get<Button>((int)Buttons.ArmyTab).gameObject.BindEvent(ArmyTabHandler);
        Get<Button>((int)Buttons.MonsterTab).gameObject.BindEvent(MonsterTabHandler);
        Get<Button>((int)Buttons.MagicTab).gameObject.BindEvent(MagicTabHandler);
    }
    
    private void Start()
    {
        TransformPanel(Panels.ArmyPanel);
    }

    private void OnEnable()
    {
        UpdatePanel(_lastOpenPanel);
    }

    private void OnDisable()
    {
        foreach (int idx in Enum.GetValues(typeof(Panels)))
        {
            var selectPanel = Get<Transform>(idx);
            if (selectPanel.gameObject.activeSelf == true)
            {
                _lastOpenPanel = (Panels)idx;
            }
        }
    }

    private void UpdatePanel(Panels targetPanel)
    {
        var selectPanel = Get<Transform>((int)targetPanel);
        if (selectPanel == null) return;
        
        selectPanel.gameObject.SetActive(true);
        switch (targetPanel)
        {
            case Panels.ArmyPanel :
                selectPanel.GetComponent<ArmyPanel>().UpdateArmyPanel();
                break;
        
            case Panels.MagicPanel :
                // selectPanel.GetComponent<MagicPanel>().UpdateArmyPanel();
                break;
        
            case Panels.MonsterPanel :
                selectPanel.GetComponent<MonsterPanel>().UpdateMonsterPanel();
                break;
        }
    }

    private void TransformPanel(Panels targetPanel)
    {
        foreach (int idx in Enum.GetValues(typeof(Panels)))
        {
            if (idx == (int)targetPanel)
            {
                UpdatePanel(targetPanel);
            }
            else 
                Get<Transform>(idx).gameObject.SetActive(false);
        }
    }

    private void ExitButtonHandler()
    {
        UIManager.instance.TransformUI(UI.GAMEUI);
    }

    private void ArmyTabHandler()
    {
        TransformPanel(Panels.ArmyPanel);
    }

    private void MonsterTabHandler()
    {
        TransformPanel(Panels.MonsterPanel);
    }

    private void MagicTabHandler()
    {
        return;
        // TransformPanel(Panel.MagicPanel);
    }
}
