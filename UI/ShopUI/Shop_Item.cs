using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class Shop_Item : BaseUI
{

    private enum Panels
    {
        DescPanel,
        ItemPanel
    }

    private enum Buttons
    {
        ExitButton,
        Info
    }

    private enum Texts
    {
        Description,
        Cost,
        Name
    }

    private enum Images
    {
        BuildImage
    }

    private BuildName _type;
    private int _buildCost;

    public void ItemInit(object info)
    {
        Building bInfo = info as Building;
        if (bInfo == null) return;
        
        _type = Enum.Parse<BuildName>(bInfo.Name);
        _buildCost = bInfo.BuildCost;

        Bind<Button>(typeof(Buttons));
        Bind<Transform>(typeof(Panels));
        Bind<TMP_Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        Get<TMP_Text>((int)Texts.Cost).text = bInfo.BuildCost.ToString();
        Get<TMP_Text>((int)Texts.Description).text = bInfo.Description;
        Get<TMP_Text>((int)Texts.Name).text = bInfo.Name;
        Get<Button>((int)Buttons.ExitButton).gameObject.BindEvent(DescExitHandler);
        Get<Button>((int)Buttons.Info).gameObject.BindEvent(InfoHandler);
        Get<Transform>((int)Panels.ItemPanel).gameObject.BindEvent(ItemClickHandler);
        
        Get<Transform>((int)Panels.DescPanel).gameObject.SetActive(false);
    }

    private void DescExitHandler()
    {
        Get<Transform>((int)Panels.DescPanel).gameObject.SetActive(false);
        Get<Transform>((int)Panels.ItemPanel).gameObject.SetActive(true);
    }

    private void InfoHandler()
    {
        Get<Transform>((int)Panels.DescPanel).gameObject.SetActive(true);
        Get<Transform>((int)Panels.ItemPanel).gameObject.SetActive(false);
    }
    private void ItemClickHandler()
    {
        if (GameManager.instance.GetResource(ResourceType.Gold) < _buildCost) return;
        // TODO: 내 카메라 중앙에 스폰
        SpawnManager.instance.SpawnBuild(_type, 0, 0);
    }
}
