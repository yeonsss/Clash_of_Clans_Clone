using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static Define;
using Object = UnityEngine.Object;

 public class Build : BaseController
{
    public bool m_BuildActivete = false;
    public BuildName type = BuildName.None;
    public string buildId;

    public bool buildActive
    {
        get { return m_BuildActivete; }
        set
        {
            if (value == false)
            {
                area.SetActive(true);
                ClearArea(transform.position);
            }
            else
            {
                area.SetActive(false);
                DecisionPos();
            }

            m_BuildActivete = value;
        }
    }

    public bool m_CollisionCheck = false;
    public bool m_CooldownComplete = false;
    public bool m_BuildChoiceComplete = false;
    public bool justSpawn = false;


    public int currentLevel = 0;
    public float Attack { get; set; }
    public float AttackCooldown { get; set; }
    public int XSize { get; set; }
    public int YSize { get; set; }


    private float rangeY = -0.49f;
    private Vector3 prevPos;
    public int instanceId;
    public GameObject area;
    private List<Tuple<int, int, AreaType>> AreaList = new List<Tuple<int, int, AreaType>>();

    protected override void Start()
    {
        base.Start();
        instanceId = GameManager.IssueId();
        DataManager.instance.buildingDict.TryGetValue(type, out var info);

        if (info == null) return;
        Attack = info.Levels[currentLevel].Attack;
        Hp = info.Levels[currentLevel].Hp;
        MaxHp = info.Levels[currentLevel].Hp;
        AttackCooldown = info.AttackCooldown;
        XSize = info.XSize;
        YSize = info.YSize;

        if (justSpawn == false)
        {
            CulcArea();
            buildActive = false;
            GetCollision();
        }
        else
        {
            CulcArea();
            buildActive = true;
            m_CollisionCheck = false;
            m_BuildChoiceComplete = true;
            m_CooldownComplete = true;
        }
    }

    protected override void Update()
    {
        if (buildActive == false)
        {
            GetCollision();
        }
        base.Update();
    }

    public void ClearArea(Vector3 pos)
    {
        if (XSize % 2 == 0 || YSize % 2 == 0)
        {
            pos.x += 0.5f;
            pos.z += 0.5f;
        }
        
        if (AreaList.Count < 0) return;
        foreach (var tuple in AreaList)
        {
            
            Wtor(pos.x + tuple.Item1, pos.z + tuple.Item2, (int)WIDTH, out var rx, out var ry);
            
            var ar = SpawnManager.GetArea(rx, ry);
            if (ar == null) return;

            ar.PopArea(instanceId);
        }
    }

    public void DecisionPos()
    {
        var pos = transform.position;
        if (XSize % 2 == 0 || YSize % 2 == 0)
        {
            pos.x += 0.5f;
            pos.z += 0.5f;
        }
        
        if (AreaList.Count < 0) return;
        foreach (var tuple in AreaList)
        {
            
            Wtor(pos.x + tuple.Item1, pos.z + tuple.Item2, (int)WIDTH, out var rx, out var ry);
            
            var ar = SpawnManager.GetArea(rx, ry);
            if (ar == null) return;

            ar.PushArea(tuple.Item3, instanceId);
        }
    }

    public void GetCollision()
    {
        var br = GetComponentsInChildren<BuildArea>();
        foreach (var range in br)
        {
            if (range.isCollision == true)
            {
                m_CollisionCheck = true;
                return;
            }
        }

        m_CollisionCheck = false;
    }

    protected override void SetComponent() { }

    public void CulcArea()
    {
        Object range = Resources.Load("Prefabs/Range");

        area = new GameObject();
        area.name = "Area";
        
        int areaX = (int)Mathf.Floor(XSize + 2);
        int areaY = (int)Mathf.Floor(YSize + 2);

        int midX = (int)Mathf.Floor((XSize + 2) / 2);
        int midY = (int)Mathf.Floor((YSize + 2) / 2);
        
        for (int i = 0; i < areaY; i++)
        {
            for (int j = 0; j < areaX; j++)
            {
                if (i == 0 || i == areaY - 1)
                {
                    AreaList.Add(new Tuple<int, int, AreaType>(j - midX, i - midY, AreaType.NoSpawnArea));
                }
                else
                {
                    if (j == 0 || j == areaX - 1)
                    {
                        AreaList.Add(new Tuple<int, int, AreaType>(j - midX, i - midY, AreaType.NoSpawnArea));
                    }
                    else
                    {
                        AreaList.Add(new Tuple<int, int, AreaType>(j - midX, i - midY, AreaType.Building));
                        Instantiate(range, new Vector3(j - midX, rangeY, i - midY), Quaternion.identity, area.transform);
                    }
                }
            }        
        }

        area.transform.parent = transform;
        
        if (areaX % 2 != 0 && areaY % 2 != 0)
        {
            area.transform.localPosition = new Vector3(0, 0, 0);
        }
        else
        {
            area.transform.localPosition = new Vector3(0.5f, 0, 0.5f);
        }
        area.SetActive(false);
    }

    public List<Tuple<float, Vector2>> GetCanMoveNearArea(float x, float y)
    {
        List<Tuple<float, Vector2>> canMoveAreaList = new List<Tuple<float, Vector2>>();
        foreach (var tuple in AreaList.FindAll(elem => elem.Item3 == AreaType.NoSpawnArea))
        {
            var areaPosX = (transform.position.x + tuple.Item1);
            var areaPosY = (transform.position.z + tuple.Item2);

            Wtor(areaPosX, areaPosY, (int)WIDTH, out var areaRPosX, out var areaRPosY);

            if (SpawnManager.GetArea(areaRPosX, areaRPosY).type == AreaType.Building)
            {
                continue;
            }

            var result = Mathf.Abs(x - areaPosX) + Mathf.Abs(y - areaPosY);
            Vector2 vec = new Vector2(0, 0);
            vec.x = areaPosX;
            vec.y = areaPosY;
            
            canMoveAreaList.Add(new Tuple<float, Vector2>(result, vec));
        }

        canMoveAreaList.Sort((elem, elem2) => elem.Item1.CompareTo(elem2.Item1));
        return canMoveAreaList;
    }

    public void OnDisable()
    {
        // 씬 전환 전 건물 비활성화
        if (buildActive == true && m_CooldownComplete == true)
        {
            // 건물을 비활성화 시킬때 바닥도 제거
            ClearArea(transform.position);
        }
    }

    public void OnEnable()
    {
        // 씬 전환 후 비활성화 했던 건물들 다시 활성화
        if (buildActive == true && m_CooldownComplete == true)
        {
            // 건물을 활성화 시킬때 바닥 다시 설치
            DecisionPos();
        }
    }

    protected override void Die()
    {
        if (Hp <= 0)
        {
            BattleManager.instance.DeletaBuildInBoard(gameObject);
            gameObject.SetActive(false);
            // Destroy(gameObject);
        }
    }

    protected virtual void UseSkill() { }
}
