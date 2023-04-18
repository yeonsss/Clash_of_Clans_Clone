using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using TMPro;
using UnityEngine.UI;
using Utils;

public class AttackUI : BaseUI
{
    private TMP_Text Name;
    private TMP_Text TierPoint;
    private TMP_Text CreditPoint;
    private BattleSceneInitializer _initializer;
    private Dictionary<string, GameObject> monsterBtnList;
    private Dictionary<string, GameObject> magicBtnList;
    private string _prevSelectedName;
    private bool _changeDone;
    
    private enum Buttons
    {
        NextUserButton,
        GiveUpButton,
        CancelButton,
    }
    
    private enum Panels
    {
        SpawnPanel,
        UserPanel,
        AlertPanel
    }
    
    private enum Texts
    {
        UserName,
        TierPoint,
        CreditPoint,
        AlertMessage
    }

    public override void Init()
    {
        monsterBtnList = new ();
        magicBtnList = new();
            
        Bind<Button>(typeof(Buttons));
        Bind<Transform>(typeof(Panels));
        Bind<TMP_Text>(typeof(Texts));
        
        Get<Button>((int)Buttons.NextUserButton).gameObject.BindEvent(NextUserHandler);
        Get<Button>((int)Buttons.CancelButton).gameObject.BindEvent(BattleCancelHandler);
        Get<Button>((int)Buttons.GiveUpButton).gameObject.BindEvent(GiveUpHandler);
        
        Get<Transform>((int)Panels.AlertPanel).gameObject.SetActive(false);
        Get<Button>((int)Buttons.GiveUpButton).gameObject.SetActive(false);
        
        BattleManager.instance.battleDoneEvent.AddListener(SetAlertMessage);
        BattleManager.instance.rivalChangeEvent.AddListener(SetRivalInfo);
        BattleManager.instance.setMyArmyEvent.AddListener(SetSpawnButton);
    }

    private void NextUserHandler()
    {
        if (_changeDone == true)
        {
            _changeDone = false;
            BattleManager.instance.ChangeRival();    
        }
    }
    
    private void GiveUpHandler()
    {
        StartCoroutine(BattleManager.instance.BattleDone("you lose", false));
        SpawnManager.instance.SetAciveMyBuild(true);
        BattleManager.instance.BattleStateInit();
        C_SceneManager.instance.SwitchScene("HomeGround");
    }
    
    private void BattleCancelHandler()
    {
        if (BattleManager.instance.isBattleStart == true) return;
        SpawnManager.instance.SetAciveMyBuild(true);
        BattleManager.instance.BattleStateInit();
        C_SceneManager.instance.SwitchScene("HomeGround");
    }

    public void SetAlertMessage(string message)
    {
        Get<Transform>((int)Panels.AlertPanel).gameObject.SetActive(true);
        Get<TMP_Text>((int)Texts.AlertMessage).text = message;
    }
    
    public void SetRivalInfo(ResponseGetRivalDTO response)
    {
        Get<TMP_Text>((int)Texts.UserName).text = response.rivalInfo.userName;
        Get<TMP_Text>((int)Texts.TierPoint).text = response.rivalInfo.tierPoint.ToString();
        Get<TMP_Text>((int)Texts.CreditPoint).text = response.rivalInfo.credit.ToString();
        _changeDone = true;
    }

    private void AllSpawnButtonStateFalse()
    {
        foreach (var btn in monsterBtnList)
        {
            btn.Value.GetComponent<SpawnBtn>().SetSelectState(false);
        }
    }

    private Action SpawnBtnClickHandler(string monsterName)
    {
        return () =>
        {
            monsterBtnList.TryGetValue(monsterName, out var btn);
            if (btn == null) return;
            
            if (_prevSelectedName == null)
            {
                _prevSelectedName = monsterName;
                btn.GetComponent<SpawnBtn>().SetSelectState(true);
                BattleManager.instance.SetSpawnState("Monster", monsterName);
            }

            else if (_prevSelectedName == monsterName)
            {
                _prevSelectedName = "";
                btn.GetComponent<SpawnBtn>().SetSelectState(false);
                BattleManager.instance.ClearSpawnState();
            }

            else if (_prevSelectedName != monsterName)
            {
                _prevSelectedName = monsterName;
                AllSpawnButtonStateFalse();
                btn.GetComponent<SpawnBtn>().SetSelectState(true);
                BattleManager.instance.SetSpawnState("Monster", monsterName);
            }
        };
    }

    public void SetSpawnButton(ResponseGetArmyDTO response)
    {
        var info = response.armyInfo;
        var spawnPanel = Get<Transform>((int)Panels.SpawnPanel);
        
        foreach (var monster in info.selectMonsterMap)
        {
            var btn = ResourceManager.instance.Instantiate("UI/ATTACK/SpawnBtn", spawnPanel);
            btn.GetComponent<SpawnBtn>().ItemInit(monster.Value.ToString(), monster.Key, "Monster");
            btn.BindEvent(SpawnBtnClickHandler(monster.Key));
            monsterBtnList.Add(monster.Key, btn);
        }
        // foreach (var magic in info.selectMagicMap)
        // {
        //     var btn = ResourceManager.instance.Instantiate("UI/ATTACK/SpawnBtn", spawnPanel);
        //     btn.GetComponent<SpawnBtn>().ItemInit(magic.Value.ToString(), magic.Key, "Magic");
        // }
    }
    
    private void Update()
    {
        if (BattleManager.instance.isBattleStart == true)
        {
            Get<Button>((int)Buttons.GiveUpButton).gameObject.SetActive(true);
            Get<Button>((int)Buttons.CancelButton).gameObject.SetActive(false);
        }
        else
        {
            Get<Button>((int)Buttons.GiveUpButton).gameObject.SetActive(false);
            Get<Button>((int)Buttons.CancelButton).gameObject.SetActive(true);
        }
    }

    
}
