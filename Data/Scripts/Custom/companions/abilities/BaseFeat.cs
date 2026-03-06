using System;
using Server;
using Server.Companions.Core;
using Server.Companions.Data;

namespace Server.Companions.Abilities
{
    public abstract class BaseFeat : BaseCompanionAbility
    {
        public abstract string FeatKey { get; }

        private int m_Tier;

        protected BaseFeat(int tier)
        {
            m_Tier = tier;
        }

        public int Tier
        {
           get { return m_Tier; }
        }

        public abstract FeatCategory Category { get; }
        public abstract FeatType Type { get; }

        public virtual int MaxTier
        {
             get { return 1; }
        }

        public virtual bool CanUpgradeTo(int newTier)
        {
            return newTier == m_Tier + 1 && newTier <= MaxTier;
        }

        public virtual Type RequiredPreviousFeat
        {
            get { return null; }
        }

        public virtual bool RequiresClass(CompanionClass cls)
        {
            return true;
        }

        public virtual bool RequiresAlignment(CompanionAlignment alignment)
        {
            return true;
        }

        public override bool CanUse(CompanionMobile companion)
        {
            if (!base.CanUse(companion))
                return false;

            if (!RequiresClass(companion.Class))
                return false;

            if (!RequiresAlignment(companion.Alignment))
                return false;

            return true;
        }
    }
}
