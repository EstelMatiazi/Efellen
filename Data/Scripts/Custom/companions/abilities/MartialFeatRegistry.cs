using System.Collections.Generic;
using Server.Companions.Core;

namespace Server.Companions.Abilities
{
    public static class MartialFeatRegistry
    {
        private static List<ICompanionAbility> m_All;

        static MartialFeatRegistry()
        {
            m_All = new List<ICompanionAbility>();

            m_All.Add(new PowerAttackI());
           // m_All.Add(new FlurryI());
           // m_All.Add(new BullRushI());
        }

        public static List<ICompanionAbility> GetAvailable(CompanionMobile comp)
        {
            List<ICompanionAbility> list = new List<ICompanionAbility>();

            for (int i = 0; i < m_All.Count; i++)
            {
                ICompanionAbility a = m_All[i];

                if (a.GetRequiredLevel() > comp.Level)
                    continue;

                list.Add(a);
            }

            return list;
        }
    }
}
