using System;
using System.Collections.Generic;
using Server.Companions.Core;

namespace Server.Companions.Abilities
{
    public static class MartialFeatRegistry
    {
        private static readonly List<Func<BaseFeat>> m_AllFeats =
            new List<Func<BaseFeat>>
            {
                () => new PowerAttack(1),
                () => new PowerAttack(2),
                () => new PowerAttack(3),

                () => new Flurry(1),
                () => new Flurry(2),
                () => new Flurry(3),
            };

        public static List<ICompanionAbility> GetAvailable(
            CompanionMobile companion
        )
        {
            List<ICompanionAbility> list =
                new List<ICompanionAbility>();

            for (int i = 0; i < m_AllFeats.Count; i++)
            {
                BaseFeat feat = m_AllFeats[i]();

                if (!IsFeatEligible(companion, feat))
                    continue;

                list.Add(feat);
            }

            return list;
        }

        public static Dictionary<int, List<BaseFeat>> GetAvailableByTier(
            CompanionMobile companion
        )
        {
            Dictionary<int, List<BaseFeat>> map =
                new Dictionary<int, List<BaseFeat>>();

            List<ICompanionAbility> list = GetAvailable(companion);

            for (int i = 0; i < list.Count; i++)
            {
                BaseFeat feat = list[i] as BaseFeat;
                if (feat == null)
                    continue;

                int tier = feat.Tier;

                if (!map.ContainsKey(tier))
                    map[tier] = new List<BaseFeat>();

                map[tier].Add(feat);
            }

            return map;
        }

        private static bool IsFeatEligible(
            CompanionMobile companion,
            BaseFeat feat
        )
        {
            if (companion.Level < feat.GetRequiredLevel())
                return false;

            if (!feat.RequiresClass(companion.Class))
                return false;

            if (!feat.RequiresAlignment(companion.Alignment))
                return false;

            Type prereq = feat.RequiredPreviousFeat;
            if (prereq != null)
            {
                if (!HasFeat(companion, prereq))
                    return false;
            }

            return true;
        }

        private static bool HasFeat(
            CompanionMobile companion,
            Type featType
        )
        {
            AbilityManager mgr = companion.AbilityManager;
            if (mgr == null)
                return false;

            return mgr.HasAbility(featType);
        }
    }
}
