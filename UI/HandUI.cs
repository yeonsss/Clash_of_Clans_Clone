using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;
using System.Collections.Generic;

public class HandUI : MonoBehaviour
{
    private TMP_Text GoldText;
    private Button RerollButton;
    private Button ExitButton;
    private Transform Content;

    private List<GameObject> CardObj;

    private void Awake()
    {
        GoldText = transform.Find("GoldBox").Find("Gold").GetComponent<TMP_Text>();
        RerollButton = transform.Find("RerollButton").GetComponent<Button>();
        ExitButton = transform.Find("ExitButton").GetComponent<Button>();
        Content = transform.Find("Hand").Find("Viewport").Find("Content");

        GoldText.text = GameManager.instance.GetResource("Gold").ToString();
        RerollButton.onClick.AddListener(RerollButtonHandler);
        ExitButton.onClick.AddListener(HandExitButtonHandler);
        
        CardObj = new List<GameObject>();
        FillCard();
    }

    private void Update()
    {
        GoldText.text = GameManager.instance.GetResource("Gold").ToString();
    }

    private void FillCard()
    {
        var contentBox = Resources.Load("Prefabs/UI/ContentBox") as GameObject;
        GameManager.instance.deck.Shuffle();
        int[] cardCodeList = GameManager.instance.deck.Draw();

        foreach (var cardCode in cardCodeList)
        {
            DataManager.instance.cardDict.TryGetValue(cardCode, out var cardInfo);
            if (cardInfo == null) continue;
            
            GameObject obj = Instantiate(contentBox, Content);
            obj.name = "card";
            var cc = obj.GetComponent<CardController>();
            if (cardInfo.Type == 1) cc.type = CardType.BuildCard;
            else if (cardInfo.Type == 2) cc.type = CardType.MonsterCard;
            else if (cardInfo.Type == 3) cc.type = CardType.MagicCard;
            else return;

            cc.SerialCode = cardInfo.SerialCode;
            cc.Name = cardInfo.Name;
            cc.Description = cardInfo.Description;
            cc.Cost = cardInfo.Cost;
            
            CardObj.Add(obj);  
        }
    }

    private void RerollButtonHandler()
    {
        GameManager.instance.deck.Shuffle();
        int[] cardCodeList = GameManager.instance.deck.Draw();
        

        for (int i = 0; i < CardObj.Count; i++)
        {
            CardObj[i].gameObject.SetActive(true);

            var cc = CardObj[i].GetComponent<CardController>();
            
            var code = cardCodeList[i];
            DataManager.instance.cardDict.TryGetValue(code, out var cardInfo);
            if (cardInfo == null) continue;
            
            if (cardInfo.Type == 1) cc.type = CardType.BuildCard;
            else if (cardInfo.Type == 2) cc.type = CardType.MonsterCard;
            else if (cardInfo.Type == 3) cc.type = CardType.MagicCard;
            else return;
            
            cc.SerialCode = cardInfo.SerialCode;
            cc.Name = cardInfo.Name;
            cc.Description = cardInfo.Description;
            cc.Cost = cardInfo.Cost;
            
            cc.DisplayUpdate();
        }
    }

    private void HandExitButtonHandler()
    {
        UIManager.instance.UIActiveSetting(UI.GAMEUI, true);
        UIManager.instance.UIActiveSetting(UI.HANDUI, false);
    }
   
}
