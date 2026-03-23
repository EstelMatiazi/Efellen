using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;

namespace Server.Custom.Ascensions
{
    public class HierophantConsecratedGroundAbility : AscensionAbility
    {
        public override AscensionType Ascension     { get { return AscensionType.Hierophant; } }
        public override int           RequiredLevel { get { return 11; } }
        public override string        Name          { get { return "ConsecratedGround"; } }
        public override string        DisplayName { get { return "Consecrated Ground"; } }
        public override bool          IsPassive     { get { return false; } }
        public override TimeSpan      Cooldown      { get { return TimeSpan.FromMinutes(3); } }

        // needs static for free trigger with capstone
        public static void DoConsecratedGround(PlayerMobile pm, int level)
        {
            pm.PublicOverheadMessage(MessageType.Regular, 0x439, false, "*Consecrated Ground*");
            pm.FixedParticles(0x376A, 9, 32, 5030, 0x439, 0, EffectLayer.Waist);
            pm.PlaySound(0x1F5);

            int radius   = 4;
            int duration = level / 3;
            if (duration < 1) duration = 1;

            new ConsecratedGroundTimer(pm, level, radius, duration).Start();
        }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
                return;

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            if (pm.Mana < 55)
            {
                pm.SendMessage("You do not have enough mana.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            if (prog == null)
                return;

            pm.Mana -= 55;
            pm.SetAbilityCooldown(Name, Cooldown);

            DoConsecratedGround(pm, prog.Level);

            new CooldownNotifyTimer(pm, Cooldown).Start();
        }

        private sealed class ConsecratedGroundTimer : Timer
        {
            private readonly PlayerMobile m_Caster;
            private readonly int          m_Level;
            private readonly int          m_Radius;
            private readonly Point3D      m_Origin;
            private readonly Map          m_Map;
            private          int          m_TicksRemaining;

            private readonly ArrayList m_GlimmerTiles;

            public ConsecratedGroundTimer(PlayerMobile caster, int level, int radius, int durationSeconds)
                : base(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0))
            {
                m_Caster         = caster;
                m_Level          = level;
                m_Radius         = radius;
                m_Origin         = caster.Location;
                m_Map            = caster.Map;
                m_TicksRemaining = durationSeconds / 2;
                m_GlimmerTiles   = new ArrayList();
                Priority         = TimerPriority.OneSecond;

                PlaceGlimmerTiles();
            }

            private void PlaceGlimmerTiles()
            {
                for (int x = -m_Radius; x <= m_Radius; x++)
                {
                    for (int y = -m_Radius; y <= m_Radius; y++)
                    {
                        if (x * x + y * y > m_Radius * m_Radius)
                            continue;

                        int nx = m_Origin.X + x;
                        int ny = m_Origin.Y + y;
                        int nz = m_Map.GetAverageZ(nx, ny);

                        Static glow = new Static(0x1647);
                        glow.Hue   = 0x439;
                        glow.MoveToWorld(new Point3D(nx, ny, nz), m_Map);
                        m_GlimmerTiles.Add(glow);
                    }
                }
            }

            protected override void OnTick()
            {
                m_TicksRemaining--;

                bool expiring = m_TicksRemaining <= 0;

                ArrayList inRange = new ArrayList();

                IPooledEnumerable eable = m_Map.GetMobilesInRange(m_Origin, m_Radius);

                try
                {
                    foreach (Mobile m in eable)
                    {
                        if (m == null || m.Deleted || !m.Alive)
                            continue;

                        inRange.Add(m);
                    }
                }
                finally
                {
                    eable.Free();
                }

                int healHits = 12 + (m_Level / 3);
                int healMana = 5  + (m_Level / 3);
                int fireDmg  = Utility.RandomMinMax(22, 32) + (m_Level / 3);

                for (int i = 0; i < inRange.Count; i++)
                {
                    Mobile m = (Mobile)inRange[i];

                    if (m.Deleted || !m.Alive)
                        continue;

                    if (m.Karma >= 0)
                    {
                        m.Hits = Math.Min(m.HitsMax, m.Hits + healHits);
                        m.Mana = Math.Min(m.ManaMax, m.Mana + healMana);
                        m.FixedParticles(0x376A, 9, 32, 5030, 0x439, 0, EffectLayer.Waist);
                    }
                    else if (m_Caster.CanBeHarmful(m, false))
                    {
                        m_Caster.DoHarmful(m);
                        m.FixedParticles(0x3709, 10, 30, 5052, 0x439, 0, EffectLayer.LeftFoot);
                        m.PlaySound(0x208);
                        AOS.Damage(m, m_Caster, fireDmg, 0, 100, 0, 0, 0);
                    }
                }

                if (expiring)
                {
                    RemoveGlimmerTiles();
                    Stop();
                }
            }

            private void RemoveGlimmerTiles()
            {
                for (int i = 0; i < m_GlimmerTiles.Count; i++)
                {
                    Item tile = (Item)m_GlimmerTiles[i];
                    if (tile != null && !tile.Deleted)
                        tile.Delete();
                }

                m_GlimmerTiles.Clear();
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
                m_Player.SendMessage(0x439, "Consecrated Ground can be used again.");
            }
        }
    }
}