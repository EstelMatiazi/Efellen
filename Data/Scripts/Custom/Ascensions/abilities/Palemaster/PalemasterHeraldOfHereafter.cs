using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public static class PalemasterHeraldOfHereafter
    {
        public static void TryTrigger(PlayerMobile pm)
        {
            if (pm == null || pm.Deleted)
                return;

            if (!pm.HasActiveAscension || pm.ActiveAscension != AscensionType.Palemaster)
                return;

            AscensionProgress prog = pm.AscensionProfile.Get(AscensionType.Palemaster);

            if (prog == null || prog.Level < 20)
                return;

            if (Utility.RandomDouble() > 0.0025 * prog.Level)
                return;

            SpawnCorruptions(pm, prog.Level);
        }

        private static void SpawnCorruptions(PlayerMobile caster, int level)
        {
            Map map = caster.Map;

            if (map == null)
                return;

            Point3D origin        = caster.Location;
            int count             = Utility.RandomMinMax(6, 12);
            int durationSeconds   = Utility.RandomMinMax(12, 22);
            int spawned           = 0;

            for (int i = 0; i < 20 && spawned < count; i++)
            {
                Point3D loc = new Point3D(
                    origin.X + Utility.RandomMinMax(-7, 7),
                    origin.Y + Utility.RandomMinMax(-7, 7),
                    origin.Z
                );

                if (!map.CanFit(loc, 16, false, false))
                    continue;

                if (HasFieldAt(map, loc))
                    continue;

                int itemID = Utility.RandomBool() ? 0x3915 : 0x3922;

                new CorruptionField(itemID, loc, caster, map, TimeSpan.FromSeconds(durationSeconds));

                spawned++;
            }

            Effects.PlaySound(origin, map, 0x20B);
        }

        private static bool HasFieldAt(Map map, Point3D loc)
        {
            IPooledEnumerable eable = map.GetItemsInRange(loc, 0);

            try
            {
                foreach (Item item in eable)
                {
                    if (item is CorruptionField)
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

    public class CorruptionField : Item
    {
        private PlayerMobile m_Caster;
        private Timer m_DamageTimer;

        public override bool BlocksFit { get { return false; } }

        public CorruptionField(int itemID, Point3D loc, PlayerMobile caster, Map map, TimeSpan duration)
            : base(itemID)
        {
            Movable = false;
            Visible = true;
            Hue     = 0xB97;
            Light   = LightType.Circle300;

            MoveToWorld(loc, map);

            m_Caster      = caster;
            m_DamageTimer = new DamageTimer(this, duration);
            m_DamageTimer.Start();
        }

        public CorruptionField(Serial serial) : base(serial) { }

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
            private CorruptionField m_Field;
            private int m_TicksRemaining;
            private int m_BonusDamage;

            public DamageTimer(CorruptionField field, TimeSpan duration)
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
                        if (m == m_Field.m_Caster)
                            continue;

                        if (!m_Field.m_Caster.CanBeHarmful(m))
                            continue;

                        m_Field.m_Caster.DoHarmful(m);

                        AOS.Damage(m, m_Field.m_Caster, Utility.RandomMinMax(14, 22) + m_BonusDamage, 0, 0, 0, 0, 100);

                        Effects.SendLocationParticles(
                            EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration),
                            0x376A, 9, 10, 0xB97, 0, 5051, 0
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