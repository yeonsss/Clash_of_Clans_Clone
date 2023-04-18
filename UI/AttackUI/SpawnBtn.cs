using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnBtn : BaseUI
{
    private enum Texts
    {
        Count,
        Name
    }
    
    private enum Images
    {
        Image
    }

    private bool _select = false;
    private string _type;
    private string _name;
    
    public override void Init()
    {
        Bind<TMP_Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
    }

    public void SetSelectState(bool state)
    {
        _select = state;
    }

    public void ItemInit(string text, string objName, string type)
    {
        Get<TMP_Text>((int)Texts.Count).text = text;
        Get<TMP_Text>((int)Texts.Name).text = objName;
        _type = type;
        _name = objName;
    }

    private void Update()
    {
        if (_select == false)
        {
            Get<Image>((int)Images.Image).color = Color.white;
        }
        else
        {
            Get<Image>((int)Images.Image).color = Color.yellow;
        }

        if (_type == "Monster")
        {
            Get<TMP_Text>((int)Texts.Count).text = BattleManager.instance.selectMonsterMap[_name].ToString();
        }
        else
        {
            Get<TMP_Text>((int)Texts.Count).text = BattleManager.instance.selectMagicMap[_name].ToString();
        }
    }
}
