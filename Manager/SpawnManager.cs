using System;
using UnityEngine;
using static Define;

public class SpawnManager : Singleton<SpawnManager>
{
    private GameObject board;
    private GameObject objCollection;
    public static GameObject[,] m_Board = new GameObject[50, 50];
    
    private float worldPosY;
    private float worldPosX;
    
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

    public void Init()
    {
        board = ResourceManager.instance.InstantiateEmtObj("Board");
        // board.AddComponent<CameraController>();
        objCollection = ResourceManager.instance.InstantiateEmtObj("ObjCollection");
    }

    public void SetAciveMyBuild(bool active)
    {
        if (objCollection == null) return;
        objCollection.SetActive(active);
    }
    
    public void SpawnInit(ResponseGetMyDataDTO response)
    {
        if (objCollection.transform.childCount > 0) return;

        var buildList = response.userInfo.buildList;
        foreach (var bi in buildList)
        {
            SpawnMyBuild(bi);
        }
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
    
    public void SpawnBoard()
    {
        for (int i = 0; i < m_Board.GetLength(0); i++)
        {
            WorldPosY = (float)i;
            for(int j = 0; j < m_Board.GetLength(1); j++)
            {
                WorldPosX = (float)j;
                Rtow(j, i, (int)WIDTH, out var wx, out var wy);
                GameObject obj = ResourceManager.instance.Instantiate("Plane", new Vector3(wx, -0.5f, wy), Quaternion.identity, board.transform);
                if ((i + j) % 2 == 1)
                {
                    obj.GetComponent<Area>().EmptyType = 1;
                }
                else
                {
                    obj.GetComponent<Area>().EmptyType = 0;
                }
                obj.name = $"{-1 * (i - (HEIGHT / 2))}-{j - (WIDTH / 2)}";
                
                m_Board[i, j] = obj;
            }
            
        }
    }

    public GameObject SpawnMonster(MonsterName type, float posX, float posY, string objname)
    {
        if (DataManager.instance.monsterDict.TryGetValue(type, out var data) == false) return null;
        var obj = ResourceManager.instance.Instantiate(data.Levels[0].PrefabPath);
        if (obj != null)
        {
            obj.tag = "Enemy";
            obj.name = objname;
            obj.transform.position = new Vector3(posX, obj.transform.position.y, posY);
            obj.GetComponent<MonsterController>().type = type;
        }

        return obj;
    }

    public void SpawnMyBuild(MyBuildInfo info)
    {
        var type = (BuildName)Enum.Parse(typeof(BuildName), info.name);
        var build = SpawnBuild(type, info.posX, info.posY, true);
        build.name = info.name;
        build.GetComponent<Build>().buildId = info._id;
        if (info.buildType == "Resource")
        {
            build.GetComponent<ResourceTower>().GetResourceFull = info.isFull;
        }
        
        if (info.active == false)
        {
            var barPrefab = Resources.Load("Prefabs/WorldSpaceUI/BuildSliderBar");
            var bar = Instantiate(barPrefab, build.transform) as GameObject;
            if (bar != null) bar.GetComponent<BuildTimeSliderHandler>().Init(build, info.remainingTime);
        }
    }

    public GameObject SpawnRivalBuild(RivalBuildInfo info)
    {
        var type = (BuildName)Enum.Parse(typeof(BuildName), info.name);
        var build = SpawnBuildForJustSpawn(type, info.posX, info.posY);
        build.name = info.name;
        return build;
    }
    
    public GameObject SpawnBuildForJustSpawn(BuildName type, float posX, float posY)
    {
        Building data;
        var pos = Vector3.zero;
        if (DataManager.instance.buildingDict.TryGetValue(type, out data) == false)
        {
            return null;
        }
        
        GameObject bd = ResourceManager.instance.Instantiate(data.Levels[0].PrefabPath, new Vector3(pos.x + posX, 0, pos.z + posY),
            Quaternion.identity, null);
        bd.name = type.ToString();
        if (bd != null)
        {
            var bdScript = bd.GetComponent<Build>(); ;
            bdScript.type = type;
            bdScript.justSpawn = true;
        }
        return bd;
    }

    public GameObject SpawnBuild(BuildName type, float posX, float posY, bool justSpawn = false)
    {
        var pos = Vector3.zero;
        if (DataManager.instance.buildingDict.TryGetValue(type, out var data) == false)
        {
            return null;
        }
        GameObject bd = null;
        if (justSpawn == true)
        {
            bd = ResourceManager.instance.Instantiate(data.Levels[0].PrefabPath, new Vector3(pos.x + posX, 0, pos.z + posY),
                Quaternion.identity, objCollection.transform);
            bd.name = type.ToString();
            // bd.transform.localScale = new Vector3(data.XSize, data.XSize, data.XSize);
            if (bd != null)
            {
                var bdScript = bd.GetComponent<Build>(); ;
                bdScript.type = type;
                bdScript.justSpawn = true;
            }
            return bd;
        }
        
        var uiPrefab = Resources.Load("Prefabs/WorldSpaceUI/BuildAcceptCancel");
        if (uiPrefab == null) return null;
        
        UIManager.instance.UIActiveSetting(UI.SHOPUI, false);
        
        if (data.XSize % 2 == 0 || data.YSize % 2 == 0)
        {
            bd = ResourceManager.instance.Instantiate(data.Levels[0].PrefabPath, new Vector3(-0.5f, 0, -0.5f),
                Quaternion.identity, objCollection.transform);
        }
        else
        {
            bd = ResourceManager.instance.Instantiate(data.Levels[0].PrefabPath, new Vector3(0, 0, 0),
                Quaternion.identity, objCollection.transform);
        }
        
        if (bd == null) return null;
        bd.name = type.ToString();
        bd.GetComponent<Build>().type = type;

        var ui = ResourceManager.instance.Instantiate("WorldSpaceUI/BuildAcceptCancel", bd.transform);
        ui.GetComponent<BuildFinalChoice>().Init(bd, data.BuildCost);
        pos = bd.transform.position;
        bd.transform.position = new Vector3(pos.x + posX, 0, pos.z + posY);

        return bd;
    }

    public void SpawnWall(float originPosX, float originPosY)
    {
        float[] xl = new []{ -1f, 1f, 0f, 0f };
        float[] yl = new []{ 0f, 0f, 1f, -1f };

        //TODO: 여기서 이제 내가 설치한 곳을 기록한다든지 해서 방향을 예측
        
        for(int i = 0; i < xl.Length; i++)
        {
            var x = originPosX + xl[i];
            var y = originPosY + yl[i];
            
            if (x < X_MIN + 1 || x >= X_MAX - 1)
            {
                continue;
            }
            if (y < Y_MIN + 1 || y >= Y_MAX - 1)
            {
                continue;
            }

            Wtor(x, y, (int)WIDTH, out var rx, out var ry);
            if (m_Board[ry, rx].GetComponent<Area>().type == AreaType.Building)
            {
                continue;
            }
            
            SpawnBuild(BuildName.Wall, originPosX+xl[i], originPosY+yl[i]);
            break;
        }
    }
    
}
