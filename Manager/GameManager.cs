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
    
    // �ø��� �ڵ�, ����
    public Dictionary<string, int> OwnedMonsterDict = new ();
    
    // �ø��� �ڵ�, ����
    public Dictionary<string, int> OwnedMagicDict = new ();

    public ArmyInfo info;
    
    public int currentChapter = 1;
    public int currentRound = 1;
    public static int idCount = 0;
    public bool isInit = false;

    public static int IssueId()
    {
        idCount++;
        return idCount;
    }

    public void InitResource(ResourceType resType, int credit)
    {
        // foreach (var name in Enum.GetNames(typeof(ResourceType)))
        // {
        //     if (name == "None") continue;
        //     ResourceType resType;
        //     Enum.TryParse<ResourceType>(name, out resType);
        //     ResourceDict.Add(resType, 1000);
        // }
        ResourceDict.Add(resType, credit);
    }

    public void GainResource(ResourceType type, int value)
    {
        // TODO: 서버한테서 받기
        // TODO: 건물 리스트 순회해서 골드 알람 없애기
        ResourceDict[type] = Mathf.Clamp(value, 0, int.MaxValue);
    }

    public int GetResource(ResourceType type)
    {
        ResourceDict.TryGetValue(type, out var value);
        return value;
    }
    
    public void UseResource(ResourceType type, int mount)
    {
        ResourceDict[type] = Mathf.Clamp(ResourceDict[type] - mount, 0, int.MaxValue);
    }
}
