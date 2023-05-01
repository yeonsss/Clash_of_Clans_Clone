using System.Collections.Generic;

namespace VO.Building
{
    public class DefenceTowerVo : BuildingVo
    { 
        public float attackCooldown;
        public int minDistance;
        public int maxDistance;
        public List<DefenceTowerLevelVo> levels;
    }

    public class DefenceTowerLevelVo : LevelVo
    {
        public int townLevel;
        public float attack;
    }
}