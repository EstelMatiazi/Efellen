using System;
using Server;
using Server.Items;
using Server.Companions.Core;
using Server.Companions.Data;

namespace Server.Companions.Abilities
{
    public class Flurry : BaseStrike
    {
        public Flurry(int tier) : base(tier)
        {
        }

        public override FeatCategory Category
        {
            get { return FeatCategory.Martial; }
        }

        public override int MaxTier
        {
            get { return 3; }
        }

        public override string FeatKey
        {
            get { return "Flurry"; }
        }

        public override string GetName()
        {
            return "Flurry " + Tier;
        }

        public override string GetDescription()
        {
            return "Increases attack speed for a short duration.";
        }

        public override int GetRequiredLevel()
        {
            if (Tier == 1) return 1;
            if (Tier == 2) return 9;
            return 18;
        }

        public override TimeSpan GetCooldown()
        {
            return TimeSpan.FromSeconds(90);
        }

        protected override void OnStrike(CompanionMobile companion, Mobile target)
        {
            companion.SayAligned("*performs a flurry of blows!*");

            int bonus;
            if (Tier == 1) bonus = 20;
            else if (Tier == 2) bonus = 30;
            else bonus = 40;

            TimeSpan duration =
                TimeSpan.FromSeconds(10 + (companion.Level * 2));

            AosAttributeMod speedMod =
                new AosAttributeMod(AosAttribute.WeaponSpeed, bonus);

            companion.AddAosAttributeMod(speedMod);

            new FlurryEffectTimer(
                companion,
                speedMod,
                duration
            ).Start();
        }


        private static void ApplyFlurryBuff(
            CompanionMobile companion,
            int swingSpeedBonus,
            TimeSpan duration
        )
        {
            string modName = "FlurrySwingSpeed";

            companion.RemoveStatMod(modName);

            StatMod mod = new StatMod(
                StatType.Dex,
                modName,
                swingSpeedBonus,
                duration
            );

            companion.AddStatMod(mod);

            new FlurryEffectTimer(
                companion,
                duration
            ).Start();
        }
    }

    public class FlurryEffectTimer : Timer
    {
        private CompanionMobile m_Companion;
        private AosAttributeMod m_Mod;
        private DateTime m_End;

        public FlurryEffectTimer(
            CompanionMobile companion,
            AosAttributeMod mod,
            TimeSpan duration
        ) : base(TimeSpan.Zero, TimeSpan.FromSeconds(1))
        {
            m_Companion = companion;
            m_Mod = mod;
            m_End = DateTime.UtcNow + duration;
        }

        protected override void OnTick()
        {
            if (m_Companion == null || m_Companion.Deleted || DateTime.UtcNow >= m_End)
            {
                if (m_Companion != null)
                    m_Companion.RemoveAosAttributeMod(m_Mod);

                Stop();
                return;
            }

            m_Companion.FixedEffect(
                0x37B9,
                10,
                16,
                AlignmentHueUtility.GetHue(m_Companion.Alignment),
                0
            );
        }
    }

}
