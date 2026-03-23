using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public sealed class ReaverDeepCutsBleed : Timer
    {
        private static readonly Hashtable m_BleedTable = new Hashtable();

        public static bool IsDeepCutsBleeding(Mobile m)
        {
            return m_BleedTable.ContainsKey(m);
        }

        private static void Register(Mobile m, ReaverDeepCutsBleed timer)
        {
            m_BleedTable[m] = timer;
        }

        private static void Unregister(Mobile m)
        {
            m_BleedTable.Remove(m);
        }

        private readonly Mobile       m_Target;
        private readonly PlayerMobile m_Caster;
        private readonly int          m_Level;
        private          int          m_TicksRemaining;

        private readonly double m_OriginalActiveSpeed;
        private readonly double m_OriginalPassiveSpeed;
        private          bool   m_Slowed;

        public ReaverDeepCutsBleed(Mobile target, PlayerMobile caster, int level, int durationSeconds)
            : base(TimeSpan.FromSeconds(3.0), TimeSpan.FromSeconds(3.0))
        {
            m_Target         = target;
            m_Caster         = caster;
            m_Level          = level;
            m_TicksRemaining = durationSeconds / 3;
            Priority         = TimerPriority.OneSecond;

            BaseCreature bc = target as BaseCreature;

            if (bc != null)
            {
                m_OriginalActiveSpeed  = bc.ActiveSpeed;
                m_OriginalPassiveSpeed = bc.PassiveSpeed;
                m_Slowed               = true;

                if (bc.ActiveSpeed  > 0.1) bc.ActiveSpeed  = 0.1;
                if (bc.PassiveSpeed > 0.2) bc.PassiveSpeed = 0.2;
            }

            Register(target, this);
        }

        protected override void OnTick()
        {
            if (m_Target == null || m_Target.Deleted || !m_Target.Alive)
            {
                Cleanup();
                Stop();
                return;
            }

            m_TicksRemaining--;

            int strBonus    = m_Caster.Str / 10;
            int bleedDamage = Utility.RandomMinMax(52, 72) + strBonus;

            m_Target.FixedParticles(0x377A, 10, 15, 5030, 0x675, 0, EffectLayer.Waist);
            m_Target.PlaySound(0x1DD);

            AOS.Damage(m_Target, m_Caster, bleedDamage, 100, 0, 0, 0, 0);

            if (m_TicksRemaining <= 0)
            {
                Cleanup();
                Stop();
            }
        }

        private void Cleanup()
        {
            if (m_Slowed)
            {
                BaseCreature bc = m_Target as BaseCreature;

                if (bc != null && !bc.Deleted)
                {
                    bc.ActiveSpeed  = m_OriginalActiveSpeed;
                    bc.PassiveSpeed = m_OriginalPassiveSpeed;
                }

                m_Slowed = false;
            }

            Unregister(m_Target);
        }
    }
}