using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using TMPro;
using UnityEngine.UI;

public class BattleUI : BaseUI
{
    private enum Panels
    {
        SinglePlayPanel,
        PracticePanel,
        MultiPlayPanel
    }
    
    private enum Buttons
    {
        ExitButton,
        PracticeButton,
        SingleButton,
        MultiButton,
        MatchButton,
    }

    void Awake()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Transform>(typeof(Panels));
        
        Get<Button>((int)Buttons.ExitButton).gameObject.BindEvent(ExitButtonHandler);
        Get<Button>((int)Buttons.MatchButton).gameObject.BindEvent(MatchButtonHandler);
        Get<Button>((int)Buttons.PracticeButton).gameObject.BindEvent(PracticeButtonHandler);
        Get<Button>((int)Buttons.SingleButton).gameObject.BindEvent(SingleButtonHandler);
        Get<Button>((int)Buttons.MultiButton).gameObject.BindEvent(MultiButtonHandler);
        
        TransformPanel(Panels.MultiPlayPanel);
    }
    
    private void TransformPanel(Panels changePanel)
    {
        foreach (int idx in Enum.GetValues(typeof(Panels)))
        {
            if (idx == (int)changePanel)
                Get<Transform>(idx).gameObject.SetActive(true);
            else 
                Get<Transform>(idx).gameObject.SetActive(false);
        }
    }

    private void PracticeButtonHandler()
    {
        TransformPanel(Panels.PracticePanel);
    }

    private void SingleButtonHandler()
    {
        TransformPanel(Panels.SinglePlayPanel);
    }

    private void MultiButtonHandler()
    {
        TransformPanel(Panels.MultiPlayPanel);
    }

    private async void MatchButtonHandler()
    {
        var response = await NetworkManager.instance.Get<ResponseGetBattlePossibleDTO>("/army/battle");
        if (response.state == false) return;
        if (response.possible == false) return;
        
        UIManager.instance.TransformUI(UI.None);
        SpawnManager.instance.SetAciveMyBuild(false);
        C_SceneManager.instance.SwitchScene("BattleScene");
    }

    private void ExitButtonHandler()
    {
        UIManager.instance.TransformUI(UI.GAMEUI);
    }
}
