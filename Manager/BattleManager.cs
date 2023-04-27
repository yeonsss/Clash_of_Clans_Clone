using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = System.Random;
using static Define;

public class BattleManager : Singleton<BattleManager>
{
    public bool isBattleStart;
    public string spawnType;
    public string spawnName;
    public string targetId;

    public Dictionary<string, int> selectMonsterMap;
    public Dictionary<string, int> selectMagicMap;
    private List<GameObject> monsterList;

    public bool[,] boardForPath = new bool[50, 50];
    public GameObject[,] wallMap = new GameObject[50, 50];
    public GameObject mainHall;

    public UnityEvent<string> battleDoneEvent;
    public UnityEvent<ResponseGetRivalDTO> rivalChangeEvent;
    public UnityEvent<ResponseGetArmyDTO> setMyArmyEvent;

    private bool _battleDone = false;

    public override void Init()
    {
        battleDoneEvent = new UnityEvent<string>();
        rivalChangeEvent = new UnityEvent<ResponseGetRivalDTO>();
        setMyArmyEvent = new UnityEvent<ResponseGetArmyDTO>();
        monsterList = new List<GameObject>();
    }

    // 매니저에서 관리중인 배틀 관련 데이터 초기화
    public void BattleStateInit()
    {
        isBattleStart = false;
        targetId = "";
        mainHall = null;
        _battleDone = false;
        monsterList.Clear();
    }

    public void SetHpGaugeActive()
    {
        
    }
    
    public async void ChangeRival()
    {
        var rival = await NetworkManager.instance.Get<ResponseGetRivalDTO>($"/rival/{targetId}");
        rivalChangeEvent.Invoke(rival);
        
        targetId = rival.rivalInfo.userId;
        var buildList = rival.rivalInfo.buildList;
        var enemyBuild = GameObject.Find("enemyBuild");
        if (GameObject.Find("enemyBuild") == null)
        {
            enemyBuild = new GameObject();
            enemyBuild.name = "enemyBuild";    
        }

        for (int i = 0; i < enemyBuild.transform.childCount; i++)
        {
            Destroy(enemyBuild.transform.GetChild(i).gameObject);
        }

        foreach (var bi in buildList)
        {
            var b = SpawnManager.instance.SpawnRivalBuild(bi);
            b.transform.parent = enemyBuild.transform;
            if (bi.name == "Hall") mainHall = b;
        }
    }
    
    public void DeletaBuildInBoard(GameObject build)
    {
        var centerPos = build.transform.position;
        Wtor(centerPos.x, centerPos.z, (int)WIDTH, out var rx, out var ry);

        var buildInfo = build.GetComponent<Build>();
        var width = buildInfo.XSize;
        var height = buildInfo.YSize;
        
        var minX = (int)Mathf.Ceil(rx - (width / 2));
        var maxX = (int)Mathf.Floor(rx + (width / 2));
        var minY = (int)Mathf.Ceil(ry - (height / 2));
        var maxY = (int)Mathf.Floor(ry + (height / 2));

        for (int i = minY; i <= maxY; i++)
        {
            for (int j = minX; j <= maxX; j++)
            {
                boardForPath[i, j] = false;
                wallMap[i, j] = null;
            }
        }
    }
    
    public int GetDestroyCost(GameObject origin, int x, int y)
    {
        if (wallMap[y, x] == null) return 0;
        
        var buildInfo = wallMap[y, x].GetComponent<Build>();
        var monsterInfo = origin.GetComponent<MonsterController>();
        
        var eliminateTime = (buildInfo.Hp / monsterInfo.AttackPoint) * monsterInfo.AttackCooldown * 3;
        return (int)eliminateTime;
    }

    public async void EnterBattlePage()
    {
        BattleStateInit();
        var army = await NetworkManager.instance.Get<ResponseGetArmyDTO>("/army");
        
        UpdateMyData(army.armyInfo.selectMonsterMap, army.armyInfo.selectMagicMap);
        setMyArmyEvent.Invoke(army);
        ClearSpawnState();
        
        ChangeRival();
    }

    public void UpdateMyData(Dictionary<string, int> selectMonster, Dictionary<string, int> selectMagic)
    {
        selectMonsterMap = selectMonster;
        selectMagicMap = selectMagic;
    }

    public void SetSpawnState(string type, string objName)
    {
        spawnType = type;
        spawnName = objName;
    }
    
    public void ClearSpawnState()
    {
        spawnType = "";
        spawnName = "";
    }
    
    public IEnumerator BattleDone(string message, bool win)
    {
        battleDoneEvent.Invoke(message);
        var dto = new RequestBattleDoneDTO()
        {
            win = win,
            rivalId = targetId,
            selectMagicMap = selectMagicMap,
            selectMonsterMap = selectMonsterMap,
        };
        
        yield return NetworkManager.instance.Post<RequestBattleDoneDTO, ResponseBattleDoneDTO>("/battle/done", dto);
        yield return new WaitForSeconds(2);
        
        SpawnManager.instance.SetAciveMyBuild(true);
        C_SceneManager.instance.SwitchScene("HomeGround");
    }
    
    IEnumerator BattleStart()
    {
        SetRivalBuildInfo();
        var dto = new RequestBattleStartDTO() { targetId = targetId };
        yield return NetworkManager.instance.Post<RequestBattleStartDTO, ResponseBattleStartDTO>("/battle/start",
            dto); 
    }
    
    public void SetRivalBuildInfo()
    {
        var buildList = GameObject.FindGameObjectsWithTag("Building");

        foreach (var build in buildList)
        {
            var position = build.transform.position;
            var x = position.x;
            var y = position.z;

            Wtor(x, y, (int)WIDTH, out var rx, out var ry);

            if (build.GetComponent<Build>().type == BuildName.Wall)
            {
                wallMap[ry, rx] = build;
            }
            else
            {
                boardForPath[ry, rx] = true;    
            }
        }
    }
    
    public List<GameObject> GetBuildListExcludeType(BuildName type)
    {
        var result = new List<GameObject>(); 
        var buildList = GameObject.FindGameObjectsWithTag("Building");

        foreach (var build in buildList)
        {
            if (build.GetComponent<Build>().type != type)
            {
                result.Add(build);
            }
        }

        return result;
    }

    public List<GameObject> GetNearBuild(float posX, float posY)
    {
        var bList = GetBuildListExcludeType(BuildName.Wall);
        if (bList.Count < 1) return null;

        Dictionary<GameObject, float> buildDict = new Dictionary<GameObject, float>();
        foreach (var build in bList)
        {
            if (build == null) continue;
            var buildPos = build.transform.position;
            var culcX = Mathf.Abs(posX - buildPos.x);
            var culcY = Mathf.Abs(posY - buildPos.z);

            var maxD = culcX + culcY;
            buildDict.Add(build, maxD);
        }
        
        var items = from pair in buildDict
            orderby pair.Value
            select pair;
        
        var sortList = items.ToList();
        var resultList = new List<GameObject>();

        if (sortList.Count > 0)
        {
            var top3List = sortList.GetRange(0, Mathf.Clamp(sortList.Count, 1, 3));
            foreach (var item in top3List)
            {
                resultList.Add(item.Key);
            }
        }

        return resultList;
    }

    // EventHandler for InputManager
    public void TouchSpawnEventHandler(float x, float y)
    {
        if(spawnType == "" || spawnName == "") return;
        
        if (spawnType == "Monster")
        {
            if (isBattleStart == false)
            {
                isBattleStart = true;
                StartCoroutine(BattleStart());
            }

            if (selectMonsterMap[spawnName] < 1) return;
            var type = Enum.Parse<MonsterName>(spawnName);
            var obj = SpawnManager.instance.SpawnMonster(type, x, y, spawnName);
            if (obj != null) monsterList.Add(obj);
            selectMonsterMap[spawnName] -= 1;
            
        }
        else
        {
            if (isBattleStart == false)
            {
                isBattleStart = true;
                StartCoroutine(BattleStart());
            }
        }
    }
    
    public bool GetArea(int x, int y)
    {
        if (x < 0 || x >= WIDTH) return false;
        if (y < 0 || y >= HEIGHT) return false;
        return boardForPath[y, x];
    }

    public bool CheckMovePossible(int nextX, int nextY, int destX, int destY)
    {
        if (nextX == destX && nextY == destY)
        {
            return true;
        }
        
        var canGo = GetArea(nextX, nextY);
        if (canGo == false) return true;
        return false;
    }
    
    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "BattleScene") return;
        if (mainHall == null) return;

        if (mainHall.activeSelf == false)
        {
            // 게임 종료
            if (_battleDone == false)
            {
                _battleDone = true;
                StartCoroutine(BattleDone(" You Win ", true));
            }
        }

        if (mainHall.activeSelf)
        {
            int spawnPossible = 0;
            foreach (var m in selectMonsterMap)
            {
                spawnPossible += m.Value;
            }

            if (spawnPossible == 0)
            {
                int activeMonsterCount = 0; 
                foreach (var monster in monsterList)
                {
                    if (monster.activeSelf)
                    {
                        activeMonsterCount += 1;
                        break;
                    }
                }

                if (activeMonsterCount == 0)
                {
                    if (_battleDone == false)
                    {
                        _battleDone = true;
                        StartCoroutine(BattleDone(" You Lose ", false));
                    }
                }
            }
        }
    }
}
