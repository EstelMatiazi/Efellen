using System.Collections.Generic;
using Server.Companions.Data;

namespace Server.Companions.Abilities
{
    public static class ProgressionRegistry
    {
        private static Dictionary<CompanionClass, ClassProgression> m_Registry;

        static ProgressionRegistry()
        {
            m_Registry = new Dictionary<CompanionClass, ClassProgression>();

            ClassProgression fighter = new ClassProgression();
            fighter.Class = CompanionClass.Fighter;
            fighter.MartialFeatsPerLevel = new Dictionary<int, int>
            {
                {1,1},{2,1},{3,1},{4,1},
                {6,2},
                {8,1},{9,1},{10,1},
                {12,1},{14,1},{15,1},
                {16,1},{18,1},{20,1}
            };

            m_Registry[CompanionClass.Fighter] = fighter;
        }

        public static ClassProgression Get(CompanionClass cls)
        {
            ClassProgression prog;
            m_Registry.TryGetValue(cls, out prog);
            return prog;
        }
    }
}
