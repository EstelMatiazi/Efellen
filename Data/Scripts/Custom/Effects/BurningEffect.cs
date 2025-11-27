using System;
using Server;
using Server.Mobiles;

namespace Server.CustomEffects
{
    public class BurningEffect
    {
        public static bool IsBurning(Mobile m)
        {
            return m != null && m.GetStatMod("WildfireBurn") != null;
        }

        public static void ApplyBurn(Mobile target, int seconds, Mobile from)
        {
            if (target == null || target.Deleted || seconds <= 0)
                return;

            StatMod existing = target.GetStatMod("WildfireBurn");
            if (existing != null)
            {
                int oldDuration = existing.Offset;
                if (seconds <= oldDuration)
                    return;

                target.RemoveStatMod("WildfireBurn");
            }

            target.AddStatMod(new StatMod(StatType.Str, "WildfireBurn", seconds, TimeSpan.FromSeconds(seconds)));
            new BurnTimer(target, seconds, from).Start();
        }

        private class BurnTimer : Timer
        {
            private Mobile m_Target;
            private Mobile m_From;
            private int m_Remaining;

            public BurnTimer(Mobile target, int duration, Mobile from)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                m_Target = target;
                m_From = from;
                m_Remaining = duration;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (m_Target == null || m_Target.Deleted || m_Remaining <= 0)
                {
                    Stop();
                    if (m_Target != null)
                        m_Target.RemoveStatMod("WildfireBurn");
                    return;
                }

                m_Remaining--;

                int dmg = Utility.RandomMinMax(5, 15);

                AOS.Damage(m_Target, m_From, dmg, 0, 100, 0, 0, 0);

                if (m_Remaining <= 0)
                {
                    m_Target.RemoveStatMod("WildfireBurn");
                    Stop();
                }
            }
        }
    }
}
