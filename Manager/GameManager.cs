using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unity.VisualScripting;
using static Define;
using Vector2 = System.Numerics.Vector2;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : Singleton<GameManager>
{
    private Dictionary<ResourceType, int> ResourceDict = new Dictionary<ResourceType, int>();
    
    // 시리얼 코드, 개수
    public Dictionary<int, int> OwnedMonsterDict = new Dictionary<int, int>();
    
    // 시리얼 코드, 개수
    public Dictionary<int, int> OwnedMagicDict = new Dictionary<int, int>();

    public static GameObject[,] m_Board = new GameObject[50, 50];
    public int currentChapter = 1;
    public int currentRound = 1;
    public static int idCount = 0;
    private float worldPosY;
    private float worldPosX;
    
    private GameObject path;

    public Deck deck;

    public static int IssueId()
    {
        idCount++;
        return idCount;
    }

    public static Area GetArea(int x, int y)
    {
        if (x < 0 || x >= WIDTH) return null;
        if (y < 0 || y >= HEIGHT) return null;
        if (m_Board[y, x] == null) return null;
        return m_Board[y, x].GetComponent<Area>();
    }
    
    public static void SetAreaType(int x, int y, AreaType t)
    {
        var area = GetArea(x, y);
        if (area != null && area.type == t) return;
        area.type = t;
    }

    public void InitResource()
    {
        foreach (var name in Enum.GetNames(typeof(ResourceType)))
        {
            if (name == "None") continue;
            ResourceType resType;
            Enum.TryParse<ResourceType>(name, out resType);
            ResourceDict.Add(resType, 1000);    
            // UIManager.instance.SetTextUI(resType, ResourceDict[resType].ToString());
        }
    }

    public void Init()
    {
        InitResource();
        
        path = Resources.Load("Prefabs/Plane") as GameObject;

        CreateBoard();
        
        deck = new Deck();

        var hallPrefab = Resources.Load("Prefabs/Building/Hall");
        var hall = Instantiate(hallPrefab) as GameObject;
        if (hall != null)
        {
            var hb = hall.GetComponent<Build>(); ;
            hb.serialCode = 3;
            hb.justSpawn = true;
        }

        deck.AddCard(1);
        deck.AddCard(2);
        deck.AddCard(3);
        deck.AddCard(4);
        deck.AddCard(5);
        
        // SpawnWall(-3, 3);
        // SpawnWall(-3, -3);
        // SpawnWall(3, -3);
        // SpawnWall(3, 3);
        //
        // SpawnWall(-3, 2);
        // SpawnWall(-3, 1);
        // SpawnWall(-3, 0);
        // SpawnWall(-3, -1);
        // SpawnWall(-3, -2);
        //
        // SpawnWall(3, 2);
        // SpawnWall(3, 1);
        // SpawnWall(3, 0);
        // SpawnWall(3, -1);
        // SpawnWall(3, -2);
        //
        // SpawnWall(2, 3);
        // SpawnWall(1, 3);
        // SpawnWall(0, 3);
        // SpawnWall(-1, 3);
        // SpawnWall(-2, 3);
        //
        // SpawnWall(2, -3);
        // SpawnWall(1, -3);
        // SpawnWall(0, -3);
        // SpawnWall(-1, -3);
        // SpawnWall(-2, -3);
        //
        // SpawnWall(-3, 2);
        // SpawnWall(-3, 1);
        // SpawnWall(-3, 0);
        // SpawnWall(-3, -1);
        // SpawnWall(-3, -2);
        //
        // SpawnWall(3, 2);
        // SpawnWall(3, 1);
        // SpawnWall(3, 0);
        // SpawnWall(3, -1);
        // SpawnWall(3, -2);
        //
        // SpawnWall(2, 3);
        // SpawnWall(1, 3);
        // SpawnWall(0, 3);
        // SpawnWall(-1, 3);
        // SpawnWall(-2, 3);
        //
        // SpawnWall(2, -3);
        // SpawnWall(1, -3);
        // SpawnWall(0, -3);
        // SpawnWall(-1, -3);
        // SpawnWall(-2, -3);
    }

    public void SpawnWall(float x, float y)
    {
        var wallPrefab = Resources.Load("Prefabs/Building/Wall");
        var wallObj =Instantiate(wallPrefab) as GameObject;
        var wallBuild = wallObj.GetComponent<Build>();
        wallBuild.justSpawn = true;
        wallBuild.serialCode = 4;
        wallObj.transform.position = new Vector3(x, 0, y);
    }

    public void GainResource(string type, int value)
    {
        ResourceType resType;
        resType = Enum.Parse<ResourceType>(type);
        ResourceDict[resType] = Mathf.Clamp(ResourceDict[resType] + value, 0, int.MaxValue);
    }

    public int GetResource(string type)
    {
        ResourceType resType;
        resType = Enum.Parse<ResourceType>(type);
        ResourceDict.TryGetValue(resType, out var value);
        return value;
    }
    
    public void UseResource(string type, int mount)
    {
        ResourceType resType;
        resType = Enum.Parse<ResourceType>(type);
        ResourceDict[resType] = Mathf.Clamp(ResourceDict[resType] - mount, 0, int.MaxValue);
    }
    
    public float WorldPosY
    {
        get
        {
            return worldPosY;
        }
        set
        {
            worldPosY = -1 * (value - (Define.HEIGHT / 2));
        }
    }
    
    public float WorldPosX
    {
        get
        {
            return worldPosX;
        }
        set
        {
            worldPosX = value - (Define.WIDTH / 2);
        }
    }

    public Vector2Int GetWorldPos(int x, int y)
    {
        return new Vector2Int(x - ((int)WIDTH / 2), -1 * (y - ((int)HEIGHT / 2)));
    }

    private void CreateBoard()
    {
        GameObject p = new("Board");
        p.AddComponent<CameraController>();
        GameObject checkState = null;
        for (int i = 0; i < m_Board.GetLength(0); i++)
        {
            WorldPosY = (float)i;
            for(int j = 0; j < m_Board.GetLength(1); j++)
            {
                WorldPosX = (float)j;
                Rtow(j, i, (int)WIDTH, out var wx, out var wy);
                GameObject obj = Instantiate(path, new Vector3(wx, -0.5f, wy), Quaternion.identity, p.transform);
                obj.name = $"{-1 * (i - (HEIGHT / 2))}-{j - (WIDTH / 2)}";
                m_Board[i, j] = obj;
            }
        }
    }
}
