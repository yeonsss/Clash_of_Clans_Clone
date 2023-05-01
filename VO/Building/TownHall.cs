using System.Collections.Generic;

namespace VO.Building
{
    public class TownHallVo : BuildingVo
    {
        public List<TownHallLevelVo> levels;
    }

    public class TownHallLevelVo : LevelVo
    {
        public int master;
        public Dictionary<string, int> lvCondition;
        public Dictionary<string, int> buildPossible;
    }
}