using System;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class DeckUI : MonoBehaviour
{
    private Button ExitButton;
    private Transform Content;
    
    private void Awake()
    {
        var itemPrefab = Resources.Load("Prefabs/UI/Item");
        var cList = GameManager.instance.deck._cardList;
        
        ExitButton = transform.Find("ExitButton").GetComponent<Button>();
        Content = transform.Find("Scroll View").Find("Viewport").Find("Content");

        foreach (var code in cList)
        {
            var item = Instantiate(itemPrefab, Content) as GameObject;
            if (item != null) item.GetComponent<ItemHandler>().CardCode = code;
        }
        
        ExitButton.onClick.AddListener(ExitButtonHandler);
    }

    private void ExitButtonHandler()
    {
        UIManager.instance.UIActiveSetting(UI.DECKUI, false);
        UIManager.instance.UIActiveSetting(UI.GAMEUI, true);
    }
    
}
