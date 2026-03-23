using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Custom.Ascensions
{
    public class HierophantDivineWrathAbility : AscensionAbility
    {
        public override AscensionType Ascension     { get { return AscensionType.Hierophant; } }
        public override int           RequiredLevel { get { return 1; } }
        public override string        Name          { get { return "DivineWrath"; } }
        public override string        DisplayName { get { return "Divine Wrath"; } }
        public override bool          IsPassive     { get { return false; } }
        public override TimeSpan      Cooldown      { get { return TimeSpan.FromMinutes(1); } }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
                return;

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            if (pm.Mana < 35)
            {
                pm.SendMessage("You do not have enough mana.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            if (prog == null)
                return;

            pm.SendMessage(0x439, "Choose a target location.");
            pm.Target = new DivineWrathTarget(pm, prog.Level);
        }

        private sealed class DivineWrathTarget : Target
        {
            private readonly PlayerMobile m_Caster;
            private readonly int          m_Level;

            public DivineWrathTarget(PlayerMobile pm, int level)
                : base(12, true, TargetFlags.None)
            {
                m_Caster = pm;
                m_Level  = level;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D p = targeted as IPoint3D;
                if (p == null) return;

                Map map = m_Caster.Map;
                if (map == null) return;

                m_Caster.Mana -= 35;
                m_Caster.SetAbilityCooldown("DivineWrath", TimeSpan.FromMinutes(1));

                m_Caster.PublicOverheadMessage(MessageType.Regular, 0x439, false, "*Divine Wrath*");

                Point3D loc = new Point3D(p.X, p.Y, p.Z);

                DoWrath(m_Caster, m_Level, loc, map);

                if (m_Level >= 20 && Utility.Random(100) < 20)
                    DoWrath(m_Caster, m_Level, loc, map);

                new CooldownNotifyTimer(m_Caster, TimeSpan.FromMinutes(1)).Start();
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType) { }
        }

        public static void DoWrath(PlayerMobile pm, int level, Point3D loc, Map map)
        {
            if (map == null || map == Map.Internal)
                return;

            Effects.SendLocationParticles(
                EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                0x551A, 10, 30, 0x439, 0, 0, 0
            );
            Effects.PlaySound(loc, map, 0x208);

            int karmaBonus = pm.Karma / 1000;
            int baseDamage = Utility.RandomMinMax(30, 48) + karmaBonus;

            ArrayList targets = new ArrayList();

            IPooledEnumerable eable = map.GetMobilesInRange(loc, 2);

            try
            {
                foreach (Mobile m in eable)
                {
                    if (m == null || m.Deleted || !m.Alive || m == pm)
                        continue;

                    if (!pm.CanBeHarmful(m, false))
                        continue;

                    targets.Add(m);
                }
            }
            finally
            {
                eable.Free();
            }

            for (int i = 0; i < targets.Count; i++)
            {
                Mobile m = (Mobile)targets[i];

                if (m.Deleted || !m.Alive)
                    continue;

                int damage = baseDamage;

                bool isEvil = (m.Karma < 0);

                if (isEvil)
                {
                    int evilBonus = (level >= 10) ? 25 : 15;
                    damage = (damage * (100 + evilBonus)) / 100;
                }

                pm.DoHarmful(m);

                m.FixedParticles(0x551A, 10, 15, 0, 0x439, 0, EffectLayer.Waist);
                AOS.Damage(m, pm, damage, 0, 100, 0, 0, 0);

                if (level >= 15 && isEvil && m.Alive)
                {
                    m.Paralyze(TimeSpan.FromSeconds(4));

                    int dotDuration = 12 + (level / 3);
                    new DivineWrathDoTTimer(m, pm, dotDuration).Start();
                }
            }
        }

        private sealed class DivineWrathDoTTimer : Timer
        {
            private readonly Mobile       m_Target;
            private readonly PlayerMobile m_Caster;
            private          int          m_TicksRemaining;

            public DivineWrathDoTTimer(Mobile target, PlayerMobile caster, int durationSeconds)
                : base(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0))
            {
                m_Target         = target;
                m_Caster         = caster;
                m_TicksRemaining = durationSeconds / 2;
                Priority         = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (m_Target == null || m_Target.Deleted || !m_Target.Alive)
                {
                    Stop();
                    return;
                }

                m_TicksRemaining--;

                int dotDamage = Utility.RandomMinMax(15, 24);

                m_Target.FixedParticles(0x551A, 10, 15, 0, 0x439, 0, EffectLayer.Waist);
                m_Target.PlaySound(0x208);

                AOS.Damage(m_Target, m_Caster, dotDamage, 0, 100, 0, 0, 0);

                if (m_TicksRemaining <= 0)
                    Stop();
            }
        }

        private sealed class CooldownNotifyTimer : Timer
        {
            private readonly PlayerMobile m_Player;

            public CooldownNotifyTimer(PlayerMobile pm, TimeSpan delay)
                : base(delay)
            {
                m_Player = pm;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (m_Player == null || m_Player.Deleted) return;
                m_Player.SendMessage(0x439, "Divine Wrath can be used again.");
            }
        }
    }
}