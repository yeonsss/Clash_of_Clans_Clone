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
[Serializable]
public class Session
{
    public string sessionID { get; set; }
}

#region Building Data Class

[Serializable]
public class Building
{
    public int XSize;
    public int YSize;
    public string Name;
    public int BuildCost;
    public int BuildTime;
    public string BuildType;
    public string Description;
    public float AttackCooldown;
    public List<level> Levels;
}

[Serializable]
public class level {
    public int Level;
    public int StorageCapacity;
    public float Hp;
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
    public string Description;
    public float MoveSpeed;
    public float AttackRange;
    public float AttackCooldown;
    public float SkillCooldown;
    public int SummonCapacity;
    public float SpawnTime;
    public string Name;
    public List<MLevel> Levels;
}

public class MLevel
{
    public int Level;
    public float Hp;
    public float Attack;
    public string PrefabPath;
}

public class MonsterData
{
    public List<Monster> Monsters = new List<Monster>();
}
#endregion

public class RoundData
{
    public List<Round> Rounds = new List<Round>();
}

public class Round
{
    public int round { get; set; }
    public int Bat { get; set; }
    public int Mage { get; set; }
}

public class DataManager : Singleton<DataManager>
{
    public Dictionary<BuildName, Building> buildingDict { get; private set; } = new ();
    public Dictionary<MonsterName, Monster> monsterDict { get; private set; } = new ();
    public Dictionary<int, Round> roundDict { get; private set; } = new ();

    public T DataInitialize<T>(string path)
    {
        TextAsset tAsset = Resources.Load<TextAsset>(path);
        T dataMapJson = JsonConvert.DeserializeObject<T>(tAsset.text);
        return dataMapJson;
    }

    public void SaveData<T>(string path, T data)
    {
        string jsonText = JsonUtility.ToJson(data);
        System.IO.File.WriteAllText(path, jsonText);
    }

    public T GetData<T>(string path)
    {
        var data = DataInitialize<T>(path);
        return data;
    }

    public Monster GetMonsterData(string monsterName)
    {
        if (Enum.TryParse<MonsterName>(monsterName, out var type) == false) return null;
        return monsterDict[type];
    }

    public void BuildingDataInit()
    {
        var jsonData = DataInitialize<BuildingData>("Data/Building");
        foreach (var data in jsonData.Buildings)
        {
            buildingDict.Add((BuildName)Enum.Parse(typeof(BuildName), data.Name), data);
        }
    }
    
    public void MonsterDataInit()
    {
        var jsonData = DataInitialize<MonsterData>("Data/Monster");
        foreach (var data in jsonData.Monsters)
        {   
            monsterDict.Add((MonsterName)Enum.Parse(typeof(MonsterName), data.Name), data);
        }
    }
    
    public void RoundDataInit()
    {
        var jsonData = DataInitialize<RoundData>("Data/Round");
        foreach (var data in jsonData.Rounds)
        {   
            roundDict.Add(data.round, data);
        }
    }

    public void Init()
    {
        BuildingDataInit();
        MonsterDataInit();
        RoundDataInit();
    }
}


