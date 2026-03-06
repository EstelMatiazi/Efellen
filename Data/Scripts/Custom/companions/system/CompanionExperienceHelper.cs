using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Companions.Systems;
using Server.Companions.Data;

namespace Server.Companions.Core
{
    public static class CompanionExperienceHelper
    {
        public static void TryGrantCompanionXP(Mobile killer, int fame)
        {
            if (killer == null || fame <= 0)
                return;

            // Only players can grant companion XP
            if (!(killer is PlayerMobile))
                return;

            List<CompanionMobile> companions =
                CompanionSpawner.GetActiveCompanions(killer);

            if (companions == null || companions.Count == 0)
                return;

            int minXP = (int)Math.Floor(fame * 0.8 / 100.0);
            int maxXP = (int)Math.Floor(fame * 1.6 / 100.0);

            if (maxXP <= 0)
                return;

            int totalXP = Utility.RandomMinMax(minXP, maxXP);

            if (totalXP <= 0)
                return;

            int xpPerCompanion = totalXP / companions.Count;

            if (xpPerCompanion <= 0)
                return;

            for (int i = 0; i < companions.Count; i++)
            {
                CompanionMobile comp = companions[i];
                if (comp.GetContract() == null)
                    continue;
                if (!comp.InRange(killer, 50))
                    continue;
                if (comp != null && !comp.Deleted)
                    comp.GainExperience(xpPerCompanion);
            }
        }
    }
}
