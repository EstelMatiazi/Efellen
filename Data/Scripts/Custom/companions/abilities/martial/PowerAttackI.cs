using System;
using Server;
using Server.Mobiles;
using Server.Companions.Core;
using Server.Companions.Data;

namespace Server.Companions.Abilities
{
    public class PowerAttackI : BaseCompanionAbility
    {
        public override bool IsMartialSpecial
        {
            get { return true; }
        }

        public override string GetName()
        {
            return "Power Attack I";
        }

        public override string GetDescription()
        {
            return "A powerful strike that deals additional damage based on strength.";
        }

        public override int GetRequiredLevel()
        {
            return 1;
        }

        public override TimeSpan GetCooldown()
        {
            return TimeSpan.FromSeconds(90);
        }

        private int GetStaminaCost(CompanionMobile companion)
        {
            int cost = companion.Level * 4;
            if (cost < 25)
                cost = 25;

            return cost;
        }

        public override bool CanUse(CompanionMobile companion)
        {
            if (!base.CanUse(companion))
                return false;

            if (companion.Stam < GetStaminaCost(companion))
                return false;

            if (companion.Combatant == null)
                return false;

            if (!companion.InRange(companion.Combatant, 1))
                return false;

            return true;
        }

        public override void Use(CompanionMobile companion, Mobile target)
        {
            if (companion == null || target == null)
                return;

            int staminaCost = GetStaminaCost(companion);

            companion.Stam -= staminaCost;

            double bonusDamage = companion.Str / 20.0;

            int damage = (int)Math.Round(bonusDamage);

            companion.DoHarmful(target);

            companion.SayAligned("*performs a powerful attack!*");
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
                100,
                0,
                0,
                0,
                0
            );
        }
    }
}
