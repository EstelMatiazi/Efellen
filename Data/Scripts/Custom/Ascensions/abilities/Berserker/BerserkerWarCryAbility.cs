using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Items;

namespace Server.Custom.Ascensions
{
    public class BerserkerWarCryAbility : AscensionAbility
    {
        public override string Name { get { return "Warcry"; } }
        public override string        DisplayName { get { return "Warcry"; } }

        public override AscensionType Ascension
        {
            get { return AscensionType.Berserker; }
        }

        public override int RequiredLevel { get { return 11; } }

        public override bool IsPassive { get { return false; } }

        public override TimeSpan Cooldown
        {
            get { return TimeSpan.FromMinutes(1); }
        }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
            {
                pm.SendMessage("You cannot use your warcry now");
                return;                
            }


            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            int level = prog.Level;

            int radius = 3 + (level / 5);

            PlayPulseEffect(pm, level);

            foreach (Mobile m in pm.GetMobilesInRange(radius))
            {
                if (m == pm)
                    continue;

                if (pm.CanBeHarmful(m))
                {
                    ApplyEnemyDebuff(pm, m, level);
                }
                else
                {
                    ApplyAllyRegen(m, level);
                }
            }

            pm.SetAbilityCooldown(Name, Cooldown);
            pm.SendMessage("You unleash a terrifying warcry!");
            Timer.DelayCall(
                Cooldown,
                delegate
                {
                    if (pm != null && !pm.Deleted)
                        pm.SendMessage("You can use your Warcry again.");
                }
            );
        }

        private static void ApplyEnemyDebuff(PlayerMobile caster, Mobile target, int level)
        {
            caster.DoHarmful(target);

            int strLoss = 25 + level;
            int dexLoss = 10 + level;
            int intLoss = 20 + level;
            int duration = 20 + level;

            target.AddStatMod(new StatMod(StatType.Str, "WarcryStr", -strLoss, TimeSpan.FromSeconds(duration)));
            target.AddStatMod(new StatMod(StatType.Dex, "WarcryDex", -dexLoss, TimeSpan.FromSeconds(duration)));
            target.AddStatMod(new StatMod(StatType.Int, "WarcryInt", -intLoss, TimeSpan.FromSeconds(duration)));

            if (level >= 16)
            {
                int immediateDrain = 20 + (2 * level);
                target.Stam -= immediateDrain;

                new WarcryStaminaDrainTimer(target, level).Start();
            }

            target.FixedParticles(0x36BD, 10, 20, 5052, 0x0F1, 0, EffectLayer.Head);
        }

        private static void ApplyAllyRegen(Mobile target, int level)
        {
            new WarcryRegenTimer(target, level).Start();
        }

         private static void PlayPulseEffect(PlayerMobile pm, int level)
        {
            Effects.SendLocationParticles(
                EffectItem.Create(pm.Location, pm.Map, EffectItem.DefaultDuration),
                0x376A,
                10,
                30,
                0x0F1,
                0,
                5029,
                0
            );

            pm.PlaySound(0x2F3);

            if (level >= 16)
            {
                Effects.SendLocationParticles(
                    EffectItem.Create(pm.Location, pm.Map, EffectItem.DefaultDuration),
                    0x3709,
                    15,
                    40,
                    0x0F1,
                    0,
                    5052,
                    0
                );
            }
        }

        private class WarcryRegenTimer : Timer
        {
            private Mobile m_Target;
            private int m_Level;
            private int m_Ticks;

            public WarcryRegenTimer(Mobile m, int level)
                : base(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2))
            {
                m_Target = m;
                m_Level = level;
                m_Ticks = 0;
            }

            protected override void OnTick()
            {
                if (m_Target == null || m_Target.Deleted)
                {
                    Stop();
                    return;
                }

                int duration = 10 + m_Level;

                if (m_Ticks >= duration / 2)
                {
                    Stop();
                    return;
                }

                m_Target.Stam += 2;
                m_Ticks++;
            }
        }

        private class WarcryStaminaDrainTimer : Timer
        {
            private Mobile m_Target;
            private int m_Level;
            private int m_Ticks;

            public WarcryStaminaDrainTimer(Mobile m, int level)
                : base(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2))
            {
                m_Target = m;
                m_Level = level;
                m_Ticks = 0;
            }

            protected override void OnTick()
            {
                if (m_Target == null || m_Target.Deleted)
                {
                    Stop();
                    return;
                }

                int duration = 10 + m_Level;

                if (m_Ticks >= duration / 2)
                {
                    Stop();
                    return;
                }

                int drain = 2 + m_Level;
                m_Target.Stam -= drain;

                m_Ticks++;
            }
        }
    }
}
