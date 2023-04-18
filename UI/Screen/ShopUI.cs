using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;
using UnityEngine.Events;

public class ShopUI : BaseUI
{
    private TMP_Text GoldText;
    private Button ExitButton;
    private Transform Content;
    
    private enum Texts
    {
        Gold
    }
    
    private enum Buttons
    {
        ExitButton
    }
    
    private enum Scrolls
    {
        Content
    }

    public override void Init()
    {
        Bind<TMP_Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<Transform>(typeof(Scrolls));

        Get<Button>((int)Buttons.ExitButton).gameObject.BindEvent(HandExitButtonHandler);

        FillContent();
    }

    private void Update()
    {
        Get<TMP_Text>((int)Texts.Gold).text = GameManager.instance.GetResource(ResourceType.Gold).ToString();
    }
    
    private void FillContent()
    {

        var content = Get<Transform>((int)Scrolls.Content);
        
        foreach (var info in DataManager.instance.buildingDict)
        {
            if (info.Key == BuildName.Hall) continue;
            var item = ResourceManager.instance.Instantiate("UI/SHOP/Item", content);
            item.GetComponent<Shop_Item>().ItemInit(info.Value);
        }
    }

    private void HandExitButtonHandler()
    {
        UIManager.instance.TransformUI(UI.GAMEUI);
    }
   
}
