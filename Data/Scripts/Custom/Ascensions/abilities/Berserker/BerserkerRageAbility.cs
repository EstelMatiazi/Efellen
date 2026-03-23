using System;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public class BerserkerRageAbility : AscensionAbility
    {
        public override string Name
        {
            get { return "BerserkerRage"; }
        }

        public override string        DisplayName { get { return "Berserker Rage"; } }

        public override AscensionType Ascension
        {
            get { return AscensionType.Berserker; }
        }

        public override int RequiredLevel
        {
            get { return 1; }
        }

        public override bool IsPassive
        {
            get { return false; }
        }

        public override TimeSpan Cooldown
        {
            get { return TimeSpan.FromMinutes(1); }
        }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
            {
                pm.SendMessage("You cannot use your Rage now.");
                return;                
            }
            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);

            if (prog == null)
                return;

            int level = prog.Level;

            int bonusStr = 15 + level;
            int durationSeconds = 10 + level;
            TimeSpan duration = TimeSpan.FromSeconds(durationSeconds);

            pm.AddAscensionEffect("BerserkerRage", duration, level);

            pm.PublicOverheadMessage(
                MessageType.Regular,
                0x22,
                false,
                "*RAGES*"
            );

            pm.FixedParticles(0x3709, 10, 30, 5052, 0x21, 0, EffectLayer.Waist);
            pm.PlaySound(0x208);

            new RageEffectTimer(pm).Start();

            pm.AddStatMod(
                new StatMod(
                    StatType.Str,
                    "BerserkerRageStr",
                    bonusStr,
                    duration
                )
            );

            pm.SendMessage("You enter a berserker rage!");

            pm.SetAbilityCooldown(Name, Cooldown);

            Timer.DelayCall(
                duration,
                delegate
                {
                    if (pm != null && !pm.Deleted)
                    {
                        pm.Stam -= pm.Stam / 2;

                        pm.PublicOverheadMessage(
                            MessageType.Regular,
                            0x3B2,
                            false,
                            "*calms down*"
                        );

                        pm.SendMessage("Your rage subsides.");
                    }
                }
            );

            Timer.DelayCall(
                Cooldown,
                delegate
                {
                    if (pm != null && !pm.Deleted)
                        pm.SendMessage("You can rage again.");
                }
            );
        }

        private class RageEffectTimer : Timer
        {
            private PlayerMobile m_Mobile;

            public RageEffectTimer(PlayerMobile pm)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(3.0))
            {
                m_Mobile = pm;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (m_Mobile == null || m_Mobile.Deleted)
                {
                    Stop();
                    return;
                }

                if (!m_Mobile.HasAscensionEffect("BerserkerRage"))
                {
                    Stop();
                    return;
                }

                m_Mobile.FixedParticles(
                    0x36BD, 
                    1,
                    20,
                    5044,
                    0x21,
                    0,
                    EffectLayer.Waist
                );

                m_Mobile.FixedParticles(
                    0x3709,
                    1,
                    15,
                    5052,
                    0xF1,      
                    0,
                    EffectLayer.Head
                );

                m_Mobile.PlaySound(0x1F5);
            }
        }
    }
}
