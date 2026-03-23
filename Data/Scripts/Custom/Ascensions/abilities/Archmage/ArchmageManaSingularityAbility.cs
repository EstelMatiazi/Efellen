using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public class ArchmageManaSingularityAbility : AscensionAbility
    {
        public override AscensionType Ascension  { get { return AscensionType.Archmage; } }
        public override int RequiredLevel        { get { return 11; } }
        public override string Name              { get { return "ManaSingularity"; } }

        public override string        DisplayName { get { return "Mana Singularity"; } }
        public override bool IsPassive           { get { return false; } }

        public override TimeSpan Cooldown
        {
            get { return TimeSpan.FromSeconds(60); }
        }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
            {
                pm.SendMessage("You cannot use this ability.");
                return;
            }

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            if (pm.Mana < 50)
            {
                pm.SendMessage("You do not have enough mana.");
                return;
            }

            pm.SendMessage("Select a location for the singularity.");
            pm.Target = new SingularityTarget(this);
        }

        private class SingularityTarget : Target
        {
            private ArchmageManaSingularityAbility m_Ability;

            public SingularityTarget(ArchmageManaSingularityAbility ability)
                : base(12, true, TargetFlags.Harmful)
            {
                m_Ability = ability;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                PlayerMobile pm = from as PlayerMobile;
                if (pm == null)
                    return;

                IPoint3D p = targeted as IPoint3D;
                if (p == null)
                {
                    pm.SendMessage("That is not a valid location.");
                    return;
                }

                Map map = pm.Map;
                if (map == null)
                    return;

                m_Ability.CreateSingularity(pm, new Point3D(p), map);
            }
        }

        public void CreateSingularity(PlayerMobile caster, Point3D loc, Map map)
        {
            AscensionProgress prog = caster.AscensionProfile.Get(Ascension);
            if (prog == null)
                return;

            int level    = prog.Level;
            int maxRange = 2 + (level / 3);

            if (caster.GetDistanceToSqrt(loc) > maxRange)
            {
                caster.SendMessage("That location is too far away.");
                return;
            }

            caster.Mana -= 50;
            caster.SetAbilityCooldown(Name, Cooldown);

            BeginVortex(caster, loc, map, level);
        }

        public void CreateSingularityPassive(PlayerMobile caster, Point3D loc, Map map)
        {
            AscensionProgress prog = caster.AscensionProfile.Get(Ascension);
            if (prog == null)
                return;

            BeginVortex(caster, loc, map, prog.Level);
        }

        private static void BeginVortex(PlayerMobile caster, Point3D loc, Map map, int level)
        {
            new VortexTimer(caster, loc, map, level).Start();
        }

        private class VortexTimer : Timer
        {
            private PlayerMobile m_Caster;
            private Point3D      m_Location;
            private Map          m_Map;
            private int          m_Level;
            private int          m_Tick;

            public VortexTimer(PlayerMobile caster, Point3D loc, Map map, int level)
                : base(TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500))
            {
                m_Caster   = caster;
                m_Location = loc;
                m_Map      = map;
                m_Level    = level;
                m_Tick     = 0;
                Priority   = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (m_Caster == null || m_Caster.Deleted || m_Map == null)
                {
                    Stop();
                    return;
                }

                m_Tick++;

                Effects.PlaySound(m_Location, m_Map, 0x5C9);

                Effects.SendLocationEffect(m_Location, m_Map, 0x3709, 20, 10, 2023, 0);
                Effects.SendLocationEffect(m_Location, m_Map, 0x376A, 20, 10, 1109, 0);

                SendSpiralParticles();

                if (m_Tick >= 5)
                {
                    Stop();
                    Explode();
                }
            }

            private void SendSpiralParticles()
            {
                for (int i = 0; i < 8; i++)
                {
                    double angle = (Math.PI * 2.0 / 8) * i;

                    Point3D start = new Point3D(
                        m_Location.X + (int)(Math.Cos(angle) * 2),
                        m_Location.Y + (int)(Math.Sin(angle) * 2),
                        m_Location.Z
                    );

                    Effects.SendMovingParticles(
                        new Entity(Serial.Zero, start, m_Map),
                        new Entity(Serial.Zero, m_Location, m_Map),
                        0x36D4, 5, 0, false, false, 1109, 0, 9502, 1, 0, EffectLayer.Waist, 0
                    );
                }
            }

            private void Explode()
            {
                Effects.PlaySound(m_Location, m_Map, 0x307);
                Effects.SendLocationEffect(m_Location, m_Map, 0x36BD, 30, 10, 2053, 0);

                int bonusDamage = (m_Caster.Int / 10) + (m_Level / 2);
                int manaDrain   = 20 + m_Level;
                int manaReturn  = 0;
                bool canFreeze  = m_Level >= 16;

                IPooledEnumerable eable = m_Map.GetMobilesInRange(m_Location, 2);

                try
                {
                    foreach (Mobile m in eable)
                    {
                        if (m == m_Caster || !m_Caster.CanBeHarmful(m))
                            continue;

                        m_Caster.DoHarmful(m);

                        AOS.Damage(m, m_Caster, Utility.RandomMinMax(25, 40) + bonusDamage, 0, 0, 0, 0, 100);

                        m.Mana = Math.Max(0, m.Mana - manaDrain);

                        manaReturn += 4;

                        if (canFreeze)
                        {
                            m.Stam = Math.Max(0, m.Stam - (20 + m_Level));
                            m.Freeze(TimeSpan.FromSeconds(6));
                        }
                    }
                }
                finally
                {
                    eable.Free();
                }

                if (manaReturn > 0)
                {
                    m_Caster.Mana += manaReturn;
                    m_Caster.SendMessage("You absorb arcane energy from your enemies!");
                }
            }
        }
    }
}