using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;
using static Define;

public class BattleManager : Singleton<BattleManager>
{
    public bool rountStart = false;
    public int MonsterCount { get; set; } = 0;
    public bool[,] boardForPath = new bool[50, 50];
    public Dictionary<int, GameObject> walls;
    public Dictionary<int, GameObject> builds;

    public void Init()
    {
        walls = new Dictionary<int, GameObject>();
        builds = new Dictionary<int, GameObject>();
    }

    public GameObject CheckWallInPos(float x, float y)
    {
        var wallList = walls.Select(kvp => kvp.Value).ToList();;
        if (wallList.Count == 0) return null;

        foreach (var wall in wallList)
        {
            var pos = wall.transform.position;
            if (pos.x == x && pos.z == y)
            {
                return wall;
            }
        }

        return null;
    }

    public void DeleteBuildInArray(int instanceId)
    {
        if (builds.ContainsKey(instanceId) == true)
        {
            builds.Remove(instanceId);
        }
    }
    
    public void DeleteWallInArray(int instanceId)
    {
        if (walls.ContainsKey(instanceId) == true)
        {
            var wallPos = walls[instanceId].transform.position;
            Wtor(wallPos.x, wallPos.z, (int)WIDTH, out var rx, out var ry);
            boardForPath[ry, rx] = false;
            walls.Remove(instanceId);
        }
    }
    
    public bool GetArea(int x, int y)
    {
        if (x < 0 || x >= WIDTH) return false;
        if (y < 0 || y >= HEIGHT) return false;
        return boardForPath[y, x];
    }
    
    public bool CheckMovePossible(int nextX, int nextY)
    {
        var canGo = GetArea(nextX, nextY);
        if (canGo == false) return true;
        return false;
    }

    public void BattleStart()
    {
        if (rountStart == true) return;
        rountStart = true;
        FieldSetting();
        GenMonster();
        UIManager.instance.UIActiveSetting(UI.GAMEUI, false);
        UIManager.instance.UIActiveSetting(UI.DECKUI, false);
        UIManager.instance.UIActiveSetting(UI.HANDUI, false);
    }

    public void BattleEnd()
    {
        rountStart = false;
        CulcResource();
        UIManager.instance.UIActiveSetting(UI.GAMEUI, true);
    }

    private void Update()
    {
        if (rountStart == true && CheckGameEnd() == true)
        {
            BattleEnd();
        }
    }


    public void CulcPath()
    {
    }

    public bool CheckGameEnd()
    {
        if (MonsterCount < 1)
        {
            Debug.Log("GameEnd");
            return true;
        }
        return false;
    }

    public void FieldSetting()
    {
        
        var buildList = GameObject.FindGameObjectsWithTag("Building");

        foreach (var build in buildList)
        {
            if (build.GetComponent<Build>().bType == BuildType.Wall)
            {
                var position = build.transform.position;
                var x = position.x;
                var y = position.z;

                Wtor(x, y, (int)WIDTH, out var rx, out var ry);

                boardForPath[ry, rx] = true;
                walls.Add(build.GetComponent<Build>().instanceId, build);
            }
            else
            {
                Debug.Log(build.GetComponent<Build>().instanceId.ToString());
                builds.Add(build.GetComponent<Build>().instanceId, build);
            }
        } 
        
    }

    public List<Tuple<float, GameObject>> NeerBuildingList(float posX, float posY)
    {
        var buildList = builds.Select(kvp => kvp.Value).ToList();;
        List<Tuple<float, GameObject>> canMoveAreaList = new List<Tuple<float, GameObject>>();
        foreach (var build in buildList)
        {
            if (build == null) continue;
            var buildPos = build.transform.position;
            var culcX = Mathf.Abs(posX - buildPos.x);
            var culcY = Mathf.Abs(posY - buildPos.z);

            var maxD = culcX + culcY;
            canMoveAreaList.Add(new Tuple<float, GameObject>(maxD, build));
        }
        canMoveAreaList.Sort((elem, elem2) => elem.Item1.CompareTo(elem2.Item1));
        
        return canMoveAreaList;
    }

    public void GenMonster()
    {
        string key = $"{GameManager.instance.currentChapter}-{GameManager.instance.currentRound}";
        var data = DataManager.instance.ChapterDict[key];
        Random random = new Random();
        List<Transform> spawnPos = new List<Transform>();
        int typeCount = data.ChapterObjects.Length;
        int totalSpawnCount = 0;
        int[] spawnList = new int[typeCount];
        
        foreach (var board in GameManager.m_Board)
        {
            if (board.GetComponent<Area>().type == Define.AreaType.Empty)
            {
                spawnPos.Add(board.transform);
            }            
        }

        for (int i = 0; i < typeCount; i++)
        {
            spawnList[i] = data.ChapterObjects[i].Count;
            totalSpawnCount += data.ChapterObjects[i].Count;
        }

        for (int j = 0; j < totalSpawnCount; j++)
        {
            int choicePos = random.Next(0, spawnPos.Count);
            int choiceType = random.Next(0, typeCount);
            if (spawnList[choiceType] == 0)
            {
                for (int k = 0; k < typeCount; k++)
                {
                    if (spawnList[k] != 0)
                    {
                        spawnList[k] -= 1;
                        SpawnMonster(data.ChapterObjects[k].SerialCode, spawnPos[choicePos].position.x,
                            spawnPos[choicePos].position.z, $"{j}");
                        break;    
                    }
                }
            }
            else
            {
                spawnList[choiceType] -= 1;
                SpawnMonster(data.ChapterObjects[choiceType].SerialCode, spawnPos[choicePos].position.x,
                    spawnPos[choicePos].position.z, $"{j}");
            }
        }
    }

    public void SpawnMonster(int code, float x, float y, string name)
    {
        if (DataManager.instance.monsterDict.TryGetValue(code, out var data) == false) return;

        var monsterPrefab = Resources.Load(data.Levels[0].PrefabPath);
        if (monsterPrefab == null) return;
        var obj = Instantiate(monsterPrefab) as GameObject;
        if (obj != null)
        {
            obj.tag = "Enemy";
            obj.name = name;
            obj.transform.position = new Vector3(x, 0, y);
            obj.GetComponent<MonsterController>().serialCode = code;
            MonsterCount++;
        }
    }

    public void CulcResource()
    {
        // 자원 건물 계산해서 골드 획득
        var building = GameObject.FindGameObjectsWithTag("Building");
    }
}
