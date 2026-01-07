using System;
using Server;
using Server.Companions.Core;

namespace Server.Companions.Abilities
{
    public abstract class BasePassiveFeat : BaseFeat
    {
        protected BasePassiveFeat(int tier)
            : base(tier)
        {
        }

        public override FeatType Type
        {
            get { return FeatType.Passive; }
        }

        public override TimeSpan GetCooldown()
        {
            return TimeSpan.Zero;
        }

        public override bool CanUse(CompanionMobile companion)
        {
            return false;
        }

        public override void Use(CompanionMobile companion, Mobile target)
        {
        }
    }
}
