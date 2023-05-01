using System;

namespace VO.Building
{
    [Serializable]
    public class BuildingVo
    {
        public string name;
        public string type;
        public int xSize;
        public int ySize;
        public string description;
        // public List<level> levels;
    }
    
    public class LevelVo
    {
        public int hp;
        public int upgradeCool;
        public int upgradeCost;
        public string prefabPath;
    }
}