using System;
using Server;
using Server.Mobiles;

namespace Server.CustomEffects
{
    public class DotEffect
    {
        /*
         Damage type mapping:
         1 - physical
         2 - fire
         3 - cold
         4 - poison
         5 - energy
        */
        private static string GetDotID(int type)
        {
            return "DotEffect_" + type;
        }

        public static bool IsDotted(Mobile m, int type)
        {
            return m != null && m.GetStatMod(GetDotID(type)) != null;
        }

        public static void ApplyDot(Mobile target, int seconds, Mobile from, int type)
        {
            if (target == null || target.Deleted || seconds <= 0)
                return;

            string id = GetDotID(type);
            StatMod existing = target.GetStatMod(id);

            if (existing != null)
            {
                int oldDuration = existing.Offset;
                if (seconds <= oldDuration)
                    return;

                target.RemoveStatMod(id);
            }
            target.AddStatMod(new StatMod(StatType.Str, id, seconds, TimeSpan.FromSeconds(seconds)));
            new DotTimer(target, seconds, from, type, id).Start();
        }

        private class DotTimer : Timer
        {
            private Mobile m_Target;
            private Mobile m_From;
            private int m_Remaining;
            private int m_Type;
            private string m_ID;

            public DotTimer(Mobile target, int duration, Mobile from, int type, string id)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                m_Target = target;
                m_From = from;
                m_Remaining = duration;
                m_Type = type;
                m_ID = id;

                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (m_Target == null || m_Target.Deleted || m_Remaining <= 0)
                {
                    Cleanup();
                    return;
                }
                m_Remaining--;
                int dmg = Utility.RandomMinMax(5, 15);
                ApplyDamageType(m_Target, m_From, dmg, m_Type);

                if (m_Remaining <= 0)
                    Cleanup();
            }

            private void Cleanup()
            {
                if (m_Target != null)
                    m_Target.RemoveStatMod(m_ID);

                Stop();
            }
        }

        private static void ApplyDamageType(Mobile target, Mobile from, int amount, int type)
        {
            int phys = 0, fire = 0, cold = 0, pois = 0, nrgy = 0;

            switch (type)
            {
                case 1: phys = 100; break;
                case 2: fire = 100; break;
                case 3: cold = 100; break;
                case 4: pois = 100; break;
                case 5: nrgy = 100; break;
                default: fire = 100; break; // sanity
            }

            AOS.Damage(target, from, amount, phys, fire, cold, pois, nrgy);
        }
    }
}
