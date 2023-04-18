using System;
using System.Collections;
using UnityEngine;
using static Define;
using TMPro;
using UnityEngine.UI;

public class GameUI : BaseUI
{
    private enum Buttons
    {
        ShopButton,
        BattleButton,
        RadeButton,
        DetailExitButton,
    }
    
    private enum Panels
    {
        BuildOperPanel,
        DetailPanel,
        DescPanel,
        GaugePanel
    }
    
    private enum Texts
    {
        Gold
    }

    public override void Init()
    {
        Bind<TMP_Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<Transform>(typeof(Panels));

        Get<TMP_Text>((int)Texts.Gold).text =
            GameManager.instance.GetResource(ResourceType.Gold).ToString();
                
        Get<Button>((int)Buttons.ShopButton).gameObject.BindEvent(ShopOpenButtonHandler);
        Get<Button>((int)Buttons.BattleButton).gameObject.BindEvent(BattleButtonHandler);
        Get<Button>((int)Buttons.RadeButton).gameObject.BindEvent(RadeButtonHandler);
        Get<Button>((int)Buttons.DetailExitButton).gameObject.BindEvent(DetailExitHandler);
        
        Get<Transform>((int)Panels.DetailPanel).gameObject.SetActive(false);
        Get<Transform>((int)Panels.BuildOperPanel).gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        SocketManager.instance.Send(SocketEvent.GET_BUILD_STORAGE, null);
    }

    private void Update()
    {
        Get<TMP_Text>((int)Texts.Gold).text =
            GameManager.instance.GetResource(ResourceType.Gold).ToString();
    }

    private void DetailExitHandler()
    {
        Get<Button>((int)Buttons.DetailExitButton).gameObject.SetParentActive(false);
    }

    public void DisplayBuildPanel()
    {
        
    }
    
    public void UnDisplayBuildPanel()
    {
        
    }

    private void ShopOpenButtonHandler()
    {
        UIManager.instance.TransformUI(UI.SHOPUI);
    }
    
    private void RadeButtonHandler()
    {
        UIManager.instance.TransformUI(UI.RADEUI);
    }

    private void SettingButtonHandler()
    {
        Debug.Log("추가 예정");
    }
    
    private void AwardButtonHandler()
    {
        Debug.Log("추가 예정");
    }

    private void BattleButtonHandler()
    {
        UIManager.instance.TransformUI(UI.BATTLEUI);
    }
    
}
