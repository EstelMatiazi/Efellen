using Server;
using Server.Mobiles;
using Server.Companions.Data;

namespace Server.Companions.Core
{
    public static class CompanionEffects
    {
        public static void PlayBandageEffect(CompanionMobile mob)
        {
            if (mob == null || mob.Deleted)
                return;

            mob.PlaySound(0x57); // bandage sound
            mob.FixedEffect(0x376A, 9, 32); // white healing sparkle
        }

        public static void PlayPotionEffect(CompanionMobile mob)
        {
            if (mob == null || mob.Deleted)
                return;

            mob.PlaySound(0x1F2); // drink potion
            mob.FixedEffect(0x375A, 10, 15); // small burst
        }

        public static void PlaySpiritualismEffect(CompanionMobile mob)
        {
            if (mob == null || mob.Deleted)
                return;

            CompanionAlignment a = mob.Alignment;

            int hue = GetSpiritualismHue(a);
            if(a.GetIsGood())
            {
              mob.PlaySound( 0x24A ); 
            }
            else if (a.GetIsEvil())
            {
                mob.PlaySound( 0x481 );
            }
            else
            {
                mob.PlaySound(0x213);                
            }

            mob.FixedEffect(0x37C4, 1, 36, hue, 0);
        }

        private static int GetSpiritualismHue(CompanionAlignment a)
        {
            if (a.GetIsGood())
                return 0x482; 
            if (a.GetIsEvil())
                return 0x497; 
            if (a.GetIsChaotic())
                return 0x455; 

            return 0x59B;
        }
    }
}
