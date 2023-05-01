using System.Collections.Generic;

namespace VO.Building
{
    public class DefenceCanonVo : BuildingVo
    {
        public float attackCooldown;
        public int minDistance;
        public int maxDistance;
        public List<DefenceCanonLevelVo> levels;
    }

    public class DefenceCanonLevelVo : LevelVo
    {
        public int townLevel;
        public float attack;
    }
}