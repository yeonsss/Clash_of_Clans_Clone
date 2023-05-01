using System.Collections.Generic;

namespace VO.Building
{
    public class GoldMineVo : BuildingVo
    {
        public List<GoldMineLevelVo> levels;
    }

    public class GoldMineLevelVo : LevelVo
    {
        public int townLevel;
        public int minCapacity;
        public int maxCapacity;
        public int outputPerHour;
    }
}