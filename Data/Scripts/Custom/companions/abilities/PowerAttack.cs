using System;
using Server;
using Server.Companions.Data;
using Server.Companions.Core;

namespace Server.Companions.Abilities
{
    public class PowerAttack : BaseStrike
    {
        public PowerAttack(int tier) : base(tier)
        {
        }

        public override string FeatKey
        {
            get { return "PowerAttack"; }
        }


        public override FeatCategory Category
        {
            get { return FeatCategory.Martial; }
        }

        public override int MaxTier
        {
            get { return 3; }
        }

        public override string GetName()
        {
            return "Power Attack " + Tier;
        }

        public override string GetDescription()
        {
            return "A heavy strike that converts Strength into damage.";
        }

        public override int GetRequiredLevel()
        {
            if (Tier == 1) return 1;
            if (Tier == 2) return 6;
            return 12;
        }

        public override TimeSpan GetCooldown()
        {
            return TimeSpan.FromSeconds(90);
        }

        protected override void OnStrike(
            CompanionMobile companion,
            Mobile target
        )
        {
            int staminaCost = MartialFeatHelpers.GetStaminaCost(companion);

            companion.Stam -= staminaCost;

            double scale;

            if (Tier == 1) scale = 20.0;
            else if (Tier == 2) scale = 15.0;
            else scale = 10.0;

            int damage = (int)Math.Round(
                companion.Str / scale
            );

            if (damage > 50)
                damage = 50;

            companion.DoHarmful(target);

            companion.SayAligned("*Performs a powerful attack!*");

            companion.FixedEffect(
                0x37B9,
                10,
                16,
                AlignmentHueUtility.GetHue(companion.Alignment),
                0
            );

            AOS.Damage(
                target,
                companion,
                damage,
                100, 0, 0, 0, 0
            );
        }
    }
}
