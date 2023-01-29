using System;
using UnityEngine;
using static Define;
using TMPro;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    private TMP_Text GoldText;
    private Button CardButton;
    private Button BattleButton;
    private Button DeckOpenButton;
    private Button SettingButton;
    private Button AwardButton;

    private void Awake()
    {
        GoldText = transform.Find("GoldBox").Find("Gold").GetComponent<TMP_Text>();
        CardButton = transform.Find("CardButton").GetComponent<Button>();
        BattleButton = transform.Find("BattleButton").GetComponent<Button>();
        DeckOpenButton = transform.Find("DeckOpenButton").GetComponent<Button>();
        SettingButton = transform.Find("SettingButton").GetComponent<Button>();
        AwardButton = transform.Find("AwardButton").GetComponent<Button>();

        GoldText.text = GameManager.instance.GetResource("Gold").ToString();
        CardButton.onClick.AddListener(CardButtonHandler);
        BattleButton.onClick.AddListener(BattleButtonHandler);
        DeckOpenButton.onClick.AddListener(DeckOpenButtonHandler);
        SettingButton.onClick.AddListener(SettingButtonHandler);
        AwardButton.onClick.AddListener(AwardButtonHandler);
    }

    private void Update()
    {
        GoldText.text = GameManager.instance.GetResource("Gold").ToString();
    }

    private void DeckOpenButtonHandler()
    {
        UIManager.instance.UIActiveSetting(UI.GAMEUI, false);
        UIManager.instance.UIActiveSetting(UI.HANDUI, false);
        UIManager.instance.UIActiveSetting(UI.DECKUI, true);
    }
    
    private void CardButtonHandler()
    {
        UIManager.instance.UIActiveSetting(UI.GAMEUI, false);
        UIManager.instance.UIActiveSetting(UI.DECKUI, false);
        UIManager.instance.UIActiveSetting(UI.HANDUI, true);
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
        BattleManager.instance.BattleStart();
        Debug.Log("배틀 씬 전환");
    }
    
}
