using System.Collections.Generic;

public class CreateTask
{
    public string _id { get; set; }
    public string type { get; set; }
    public string name { get; set; }
    public bool isStart { get; set; }
    public int remainingTime { get; set; }
    public bool done { get; set; }
}

public class UserInfo
{
    public string name { get; set; }
    public int credit { get; set; }
    public int tierPoint { get; set; }
    public int lv { get; set; }
    public int hallLv { get; set; }
    public int armyCapacity { get; set; }
    public Dictionary<string, int> buildMap { get; set; }
    public List<MyBuildInfo> buildList { get; set; }
}

public class RivalInfo
{
    public string userName { get; set; }
    public int credit { get; set; }
    public int tierPoint { get; set; }
    public string userId { get; set; }
    public Dictionary<string, int> buildMap { get; set; }
    public List<RivalBuildInfo> buildList { get; set; }
}

public class RivalBuildInfo
{
    public string _id { get; set; }
    public bool active { get; set; }
    public string name { get; set; }
    public float posX { get; set; }
    public float posY { get; set; }
    public int lv { get; set; }
}

public class MyResourceBuildInfo
{
    public string _id { get; set; }
    public string name { get; set; }
    public int stored { get; set; }
}

public class MyBuildInfo
{
    public string _id { get; set; }
    public bool active { get; set; }
    public string name { get; set; }
    public float posX { get; set; }
    public float posY { get; set; }
    public bool isFull { get; set; }
    public int stored { get; set; }
    public string buildType { get; set; }
    public int lv { get; set; }
    public float remainingTime { get; set; }
}

public class ArmyInfo
{
    public int monsterProdMaxCount { get; set; }
    public int monsterProdCurCount { get; set; }
    public int magicProdMaxCount { get; set; }
    public int magicProdCurCount { get; set; }

    public Dictionary<string, int> monsterCountMap { get; set; }
    public Dictionary<string, int> monsterLevelMap { get; set; }
    public Dictionary<string, int> magicCountMap { get; set; }
    public Dictionary<string, int> magicLevelMap { get; set; }
    
    public int selectMagicCount { get; set; }
    public int selectMonsterCount { get; set; }
    public Dictionary<string, int> selectMonsterMap { get; set; }
    public Dictionary<string, int> selectMagicMap { get; set; }
}