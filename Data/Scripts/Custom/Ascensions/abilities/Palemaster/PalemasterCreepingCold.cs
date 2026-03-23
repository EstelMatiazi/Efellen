using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Spells.Necromancy;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public static class PalemasterCreepingCold
    {
        public static bool TryTrigger(PlayerMobile pm)
        {
            if (pm == null || pm.Deleted)
                return false;

            if (!pm.HasActiveAscension || pm.ActiveAscension != AscensionType.Palemaster)
                return false;

            AscensionProgress prog = pm.AscensionProfile.Get(AscensionType.Palemaster);

            if (prog == null || prog.Level < 14)
                return false;

            int level = prog.Level;

            double chance = 0.025 * level;

            if (Utility.RandomDouble() > chance)
                return false;

            pm.PublicOverheadMessage(
                MessageType.Regular,
                0x480,
                false,
                "*creeping cold*"
            );

            if (level >= 19)
                TryTriggerEnervate(pm, level);

            return true;
        }

        private static void TryTriggerEnervate(PlayerMobile pm, int level)
        {
            double chance = 0.025 * level;

            if (Utility.RandomDouble() > chance)
                return;

            PaleMasterEnervateAbility ability = new PaleMasterEnervateAbility();

            AscensionProgress prog = pm.AscensionProfile.Get(AscensionType.Palemaster);

            if (prog == null)
                return;

            int enervateLevel = prog.Level;

            ability.ForceExecute(pm, enervateLevel);
        }
    }
}