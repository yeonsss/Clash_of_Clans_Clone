using System;
using System.Collections.Generic;

namespace VO
{
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
}