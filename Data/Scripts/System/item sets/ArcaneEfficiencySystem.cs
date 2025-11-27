using System;
using Server;
using Server.Items;

namespace Server.SpellEffects
{
    public class ArcaneEfficiencySystem
    {
        public static void TryApply(Mobile caster, int spellID)
        {
            // only magery spells for arcane refund
            if ( spellID <= 0 && spellID > 63 )
                return;
            if (caster == null)
                return;

            Item hand = caster.FindItemOnLayer(Layer.TwoHanded);
            
            if (!(hand is Artifact_StaffOfTheMagi))
                return;

            double magery = caster.Skills.Magery.Value;
            double insc = caster.Skills.Inscribe.Value;

            double chance = (magery / 10.0) + (insc / 10.0);
            if (chance > 25.0)
                chance = 25.0;
            if (Utility.RandomDouble() * 100.0 > chance)
                return;

            int circle = spellID / 8;

            int baseCost = (circle + 1) * 4;

            double refundPct = 25.0 + (magery / 10.0) + (insc / 10.0);
            if (refundPct > 50.0)
                refundPct = 50.0;
            int manaBack = (int)(baseCost * (refundPct / 100.0));
            if (manaBack < 1)
                manaBack = 1;
            //better safe than sorry
            if (caster.Mana + manaBack > caster.ManaMax)
            {
                caster.Mana = caster.ManaMax;
            }
            else
            {
                caster.Mana += manaBack;
            }
            caster.SendMessage("The Staff brims with power and empowers you!");
            caster.SendMessage("You recover " + manaBack + " mana.");
        }
    }
}
