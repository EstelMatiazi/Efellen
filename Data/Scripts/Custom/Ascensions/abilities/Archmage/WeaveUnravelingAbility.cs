using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public static class WeaveUnravelingAbility
    {
        public static void Trigger(PlayerMobile caster, Mobile center)
        {
            if (center == null || center.Map == null)
                return;

            Map map        = center.Map;
            Point3D origin = center.Location;
            int count      = Utility.RandomMinMax(6, 12);
            int duration   = Utility.RandomMinMax(12, 22);

            List<WeaveDisruption> spawned = new List<WeaveDisruption>();

            for (int i = 0; i < 20 && spawned.Count < count; i++)
            {
                Point3D loc = new Point3D(
                    origin.X + Utility.RandomMinMax(-7, 7),
                    origin.Y + Utility.RandomMinMax(-7, 7),
                    origin.Z
                );

                if (!map.CanFit(loc, 16, false, false))
                    continue;

                if (HasDisruptionAt(map, loc))
                    continue;

                spawned.Add(new WeaveDisruption(loc, map, caster, TimeSpan.FromSeconds(duration)));
            }
        }

        private static bool HasDisruptionAt(Map map, Point3D loc)
        {
            IPooledEnumerable eable = map.GetItemsInRange(loc, 0);

            try
            {
                foreach (Item item in eable)
                {
                    if (item is WeaveDisruption)
                        return true;
                }
            }
            finally
            {
                eable.Free();
            }

            return false;
        }
    }

    public class WeaveDisruption : Item
    {
        private PlayerMobile m_Caster;
        private Timer m_DamageTimer;

        public override bool BlocksFit { get { return false; } }

        public WeaveDisruption(Point3D loc, Map map, PlayerMobile caster, TimeSpan duration)
            : base(0x3946)
        {
            Movable = false;
            Visible = true;
            Light   = LightType.Circle300;
            Hue     = 0x0213;

            MoveToWorld(loc, map);

            m_Caster      = caster;
            m_DamageTimer = new DamageTimer(this, duration);
            m_DamageTimer.Start();

            Effects.PlaySound(loc, map, 0x20B);
        }

        public WeaveDisruption(Serial serial) : base(serial) { }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_DamageTimer != null)
                m_DamageTimer.Stop();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            Delete();
        }

        private class DamageTimer : Timer
        {
            private WeaveDisruption m_Field;
            private int m_TicksRemaining;
            private int m_BonusDamage;

            public DamageTimer(WeaveDisruption field, TimeSpan duration)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                Priority         = TimerPriority.OneSecond;
                m_Field          = field;
                m_TicksRemaining = (int)duration.TotalSeconds;
                m_BonusDamage    = field.m_Caster != null ? field.m_Caster.Int / 15 : 0;
            }

            protected override void OnTick()
            {
                if (m_Field.Deleted || m_Field.Map == null || m_Field.m_Caster == null)
                {
                    Stop();
                    return;
                }

                IPooledEnumerable eable = m_Field.Map.GetMobilesInRange(m_Field.Location, 0);

                try
                {
                    foreach (Mobile m in eable)
                    {
                        if (m == m_Field.m_Caster || !m_Field.m_Caster.CanBeHarmful(m))
                            continue;

                        m_Field.m_Caster.DoHarmful(m);

                        AOS.Damage(m, m_Field.m_Caster, Utility.RandomMinMax(14, 22) + m_BonusDamage, 0, 0, 0, 0, 100);

                        Effects.SendLocationParticles(
                            EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration),
                            0x376A, 9, 10, 1161, 0, 5051, 0
                        );
                    }
                }
                finally
                {
                    eable.Free();
                }

                m_TicksRemaining--;

                if (m_TicksRemaining <= 0)
                    m_Field.Delete();
            }
        }
    }
}