using System;
using System.Collections;
using Server;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public static class KensaiFullHealthTracker
    {
        private static readonly Hashtable m_Table = new Hashtable();

        private static int MakeKey(Mobile attacker, Mobile target)
        {
            return (attacker.Serial.Value << 16) ^ target.Serial.Value;
        }

        public static void RecordHit(Mobile attacker, Mobile target)
        {
            bool full = (target.Hits >= target.HitsMax);
            m_Table[MakeKey(attacker, target)] = full;
        }

        public static bool WasFullHealth(Mobile attacker, Mobile target)
        {
            object val = m_Table[MakeKey(attacker, target)];

            if (val is bool)
                return (bool)val;

            return false;
        }

        public static void Clear(Mobile attacker, Mobile target)
        {
            m_Table.Remove(MakeKey(attacker, target));
        }
    }
}