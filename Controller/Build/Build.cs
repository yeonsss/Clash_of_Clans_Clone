using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Object = UnityEngine.Object;

 public class Build : BaseController
{
    public int serialCode;
    public bool m_BuildActivete = false;
    public BuildType bType = BuildType.None;
    
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

    private int currentLevel = 0;
    public float Hp { get; set; }
    public float MaxHp { get; set; }
    public float Attack { get; set; }
    public float AttackCooldown { get; set; }
    public int XSize { get; set; }
    public int YSize { get; set; }
    public float CoolTime { get; set; }
    

    private float rangeY = -0.49f;
    private Vector3 prevPos;
    public int instanceId;
    public GameObject area;
    private List<Tuple<int, int, AreaType>> AreaList = new List<Tuple<int, int, AreaType>>();
    
    protected virtual void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        instanceId = GameManager.IssueId();
        DataManager.instance.buildingDict.TryGetValue(serialCode, out var info);
        bType = serialCode == 4 ? BuildType.Wall : BuildType.NonWall;
        
        if (info == null) return;
        Attack = info.Levels[currentLevel].Attack;
        Hp = info.Levels[currentLevel].Hp;
        MaxHp = info.Levels[currentLevel].MaxHp;
        AttackCooldown = info.AttackCooldown;
        XSize = info.XSize;
        YSize = info.YSize;
        CoolTime = info.Levels[currentLevel].CoolTime;

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
            
            var ar = GameManager.GetArea(rx, ry);
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
            
            var ar = GameManager.GetArea(rx, ry);
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

    protected override void SetComponent()
    {
        m_ComponentList.Add(new BuildInputComponent());
    }

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
        if (justSpawn == true)
        {
            area.transform.position = transform.position;
        }
        else
        {
            area.transform.position = new Vector3(0, 0, 0);    
        }
        
        // if (areaX % 2 == 0 && areaY % 2 == 0)
        // {
        //     area.transform.position = new Vector3(0.5f, 0, 0.5f);
        // }
        // else
        // {
        //     
        // }

        
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

            if (GameManager.GetArea(areaRPosX, areaRPosY).type == AreaType.Building)
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

    protected override void OnMouseDown()
    {
        // 건물을 클릭해서 자원을 흭득하지 않고 라운드 클리어 후 흭득하도록 변경 
        // 그에 따른 해당 코드 주석 처리
        // if (buildActive == true && m_CooldownComplete == true) UseSkill();
        if (m_BuildChoiceComplete == true)
        {
            buildActive = false;
        }
        base.OnMouseDown();
    }

    protected override void OnMouseDrag()
    {
        if (prevPos != transform.position)
        {
            GetCollision();
        }
        base.OnMouseDrag();
    }
    
    protected override void OnMouseUp()
    {
        if (m_CollisionCheck == false && m_BuildChoiceComplete)
        {
            if (buildActive == false) buildActive = true;
            if (prevPos != transform.position)
            {
                prevPos = transform.position;    
            }
        }
        base.OnMouseUp();
    }

    public void Attacked(float attackDamage)
    {
        Hp = Mathf.Clamp(Hp - attackDamage, 0, MaxHp);
        Debug.Log($"Attacked bat{transform.name}: {Hp}");
        Die();
    }

    protected void Die()
    {
        if (Hp <= 0)
        {
            if (bType == BuildType.NonWall)
            {
                BattleManager.instance.DeleteBuildInArray(instanceId);
            }
            else if (bType == BuildType.Wall)
            {
                BattleManager.instance.DeleteWallInArray(instanceId);
            }
            Destroy(gameObject);
        }
    }

    protected virtual void UseSkill() { }
}
