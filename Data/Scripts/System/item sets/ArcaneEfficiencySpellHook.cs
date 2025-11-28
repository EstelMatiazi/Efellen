using System;
using Server;
using Server.SpellEffects;

namespace Server
{
    public class ArcaneEfficiencySpellHook
    {
        public static void Initialize()
        {
            EventSink.CastSpellRequest += new CastSpellRequestEventHandler(OnCast);
        }

        private static void OnCast(CastSpellRequestEventArgs e)
        {
            Mobile caster = e.Mobile;
            int spellID = e.SpellID;

            // DelayCall ensures spell finishes casting before refund triggers
            Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(ApplyCallback), new object[] { caster, spellID });
        }

        private static void ApplyCallback(object state)
        {
            object[] o = (object[])state;
            Mobile caster = (Mobile)o[0];
            int spellID = (int)o[1];

            ArcaneEfficiencySystem.TryApply(caster, spellID);
        }
    }
}
