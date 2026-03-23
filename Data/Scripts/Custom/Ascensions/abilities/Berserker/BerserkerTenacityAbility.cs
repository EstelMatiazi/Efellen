using System;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public class BerserkerTenacityAbility : AscensionAbility
    {
        public override string Name { get { return "Tenacity"; } }
        public override string        DisplayName { get { return "Tenacity"; } }

        public override AscensionType Ascension
        {
            get { return AscensionType.Berserker; }
        }

        public override int RequiredLevel { get { return 18; } }

        public override bool IsPassive { get { return false; } }

        public override TimeSpan Cooldown
        {
            get { return TimeSpan.FromMinutes(2); }
        }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
            {
                pm.SendMessage("You cannot use your tenacity now.");
                return;                
            }


            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("Tenacity is still on cooldown.");
                return;
            }

            pm.SendMessage("Your tenacity surges!");

            StartTenacity(pm);

            pm.SetAbilityCooldown(Name, Cooldown);

            Timer.DelayCall(
                Cooldown,
                delegate
                {
                    if (pm != null && !pm.Deleted)
                        pm.SendMessage("Your Tenacity can be called upon again.");
                }
            );
        }

        private static void StartTenacity(PlayerMobile pm)
        {
            if (pm == null || pm.Deleted)
                return;

            AscensionProgress prog = pm.AscensionProfile.Get(AscensionType.Berserker);
            int level = prog.Level;

            new TenacityTimer(pm, level).Start();
        }

        private class TenacityTimer : Timer
        {
            private PlayerMobile m_PM;
            private int m_Level;
            private int m_Ticks;

            public TenacityTimer(PlayerMobile pm, int level)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(10))
            {
                m_PM = pm;
                m_Level = level;
                m_Ticks = 0;
            }

            protected override void OnTick()
            {
                if (m_PM == null || m_PM.Deleted)
                {
                    Stop();
                    return;
                }

                if (m_Ticks >= 3)
                {
                    Stop();
                    m_PM.SendMessage("Your tenacity fades.");
                    return;
                }

                m_PM.Hits = m_PM.HitsMax;

                if (m_Level >= 20)
                {
                    m_PM.Stam = m_PM.StamMax;
                }

                PlayPulseEffect(m_PM);

                m_Ticks++;
            }
        }

        private static void PlayPulseEffect(PlayerMobile pm)
        {
            pm.FixedParticles(
                0x376A,
                10,
                30,
                5052,
                0x0F1,
                0,
                EffectLayer.Waist
            );

            pm.PlaySound(0x1F2);
        }
    }
}
