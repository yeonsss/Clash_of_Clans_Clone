using System.Collections.Generic;

namespace VO.Building
{
    public class TrainingStationVo : BuildingVo
    {
        public List<TrainingStationLevelVo> levels;
    }

    public class TrainingStationLevelVo : LevelVo
    {
        public int townLevel;
    }
}