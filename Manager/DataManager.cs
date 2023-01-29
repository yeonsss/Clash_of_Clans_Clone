using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using UnityEngine;
using TextAsset = UnityEngine.TextAsset;
using static Define;


// [Serializable] => 메모리에 있는 정보가 파일로 변환될 수 있다는 뜻으로 표시
// class {} => 파일로 변환 가능한 클래스


#region Building Data Class

[Serializable]
public class Building
{
    public int SerialCode;
    public int XSize;
    public int YSize;
    public string Name;
    public int BuildCost;
    public string Description;
    public float AttackCooldown;
    public List<level> Levels;
}

[Serializable]
public class level {
    public int Level;
    public int GainPoint;
    public int CoolTime;
    public float Hp;
    public float MaxHp;
    public float Attack;
    public int UpgradeCost;
    public string PrefabPath;
}

public class BuildingData
{
    public List<Building> Buildings = new List<Building>();
}

#endregion


#region Monster Data Class

[Serializable]

public class Monster
{
    public int SerialCode;
    public string Description;
    public float MoveSpeed;
    public float AttackRange;
    public float DetectRange;
    public float AttackCooldown;
    public float SkillCooldown;
    public List<MLevel> Levels;
}

public class MLevel
{
    public int Level;
    public float Hp;
    public float MaxHp;
    public float Attack;
    public float SpawnCost;
    public string PrefabPath;
}

public class MonsterData
{
    public List<Monster> Monsters = new List<Monster>();
}
#endregion


#region Card Data class

public class Card
{
    public string Name;
    public string Description;
    public int Cost;
    public int CardCode;
    public int SerialCode;
    public int Type;
}

public class CardData
{
    public List<Card> Cards = new List<Card>();
}

#endregion


#region Chapter Data class

public class Chapter
{
    public int ChapterNum;
    public int RoundNum;
    public ChapterObject[] ChapterObjects;
}

public class ChaptersData
{
    public Chapter[] Chapters;
}

public class ChapterObject
{
    public int SerialCode;
    public int Count;
}

#endregion


public class DataManager : Singleton<DataManager>
{
    public Dictionary<int, Building> buildingDict { get; private set; } = new Dictionary<int, Building>();
    public Dictionary<int, Monster> monsterDict { get; private set; } = new Dictionary<int, Monster>();
    public Dictionary<int, Card> cardDict { get; private set; } = new Dictionary<int, Card>();
    public Dictionary<string, Chapter> ChapterDict { get; private set; } = new Dictionary<string, Chapter>();

    public T DataInitialize<T>(string path)
    {
        TextAsset tAsset = Resources.Load<TextAsset>(path);
        T dataMapJson = JsonConvert.DeserializeObject<T>(tAsset.text);
        return dataMapJson;
    }

    public void BuildingDataInit()
    {
        var jsonData = DataInitialize<BuildingData>("Data/Building");
        foreach (var data in jsonData.Buildings)
        {   
            buildingDict.Add(data.SerialCode, data);
        }
    }
    
    public void MonsterDataInit()
    {
        var jsonData = DataInitialize<MonsterData>("Data/Monster");
        foreach (var data in jsonData.Monsters)
        {   
            monsterDict.Add(data.SerialCode, data);
        }
    }
    
    public void CardDataInit()
    {
        var jsonData = DataInitialize<CardData>("Data/Card");
        foreach (var data in jsonData.Cards)
        {   
            cardDict.Add(data.CardCode, data);
        }
    }
    
    public void ChapterDataInit()
    {
        var jsonData = DataInitialize<ChaptersData>("Data/Chapters");
        foreach (var data in jsonData.Chapters)
        {
            string key = $"{data.ChapterNum}-{data.RoundNum}";
            ChapterDict.Add(key, data);
        }
    }

    public void Init()
    {
        BuildingDataInit();
        MonsterDataInit();
        CardDataInit();
        ChapterDataInit();
    }
}


