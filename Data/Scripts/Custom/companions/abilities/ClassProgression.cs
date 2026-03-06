using System.Collections.Generic;
using Server.Companions.Data;

namespace Server.Companions.Abilities
{
    public class ClassProgression
    {
        public CompanionClass Class;
        public Dictionary<int, int> MartialFeatsPerLevel;

        public int GetFeatsAtLevel(int level)
        {
            int value;
            if (MartialFeatsPerLevel.TryGetValue(level, out value))
                return value;

            return 0;
        }
    }
}
