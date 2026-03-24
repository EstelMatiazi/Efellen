using Server;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public static class AscensionUpgradeSystem
    {
        public static bool TryUpgrade(PlayerMobile pm, AscensionType type)
        {
            AscensionProgress prog = pm.AscensionProfile.Get(type);

            if (!prog.CanLevelUp())
            {
                pm.SendMessage("Você não tem experiência suficiente.");
                return false;
            }

            int nextLevel = prog.GetNextLevel();

            int goldCost = AscensionCosts.GetGoldCost(nextLevel);
            int scrollCost = AscensionCosts.GetScrollCost(nextLevel);
            int dustCost = AscensionCosts.GetDustCost(nextLevel);
            
            if (prog.Level >= AscensionConstants.MaxAscensionLevel)
            {
                pm.SendMessage("Você já dominou esta ascensão.");
                return false;
            }


            if (!AscensionUnlocking.HasGold(pm, goldCost))
            {
                pm.SendMessage("Você não tem ouro suficiente.");
                return false;
            }

            if (!AscensionUnlocking.HasArcaneDust(pm, dustCost))
            {
                pm.SendMessage("Você não tem pó arcano suficiente.");
                return false;
            }

            if (!AscensionUnlocking.HasScrolls(pm, type, scrollCost))
            {
                pm.SendMessage("Você não tem pergaminhos de ascensão suficientes.");
                return false;
            }

            AscensionUnlocking.ConsumeGold(pm, goldCost);
            AscensionUnlocking.ConsumeArcaneDust(pm, dustCost);
            AscensionUnlocking.ConsumeScrolls(pm, type, scrollCost);

            prog.Level = nextLevel;

            pm.SendMessage("Sua ascensão " + type.ToString() + " alcançou o nível " + nextLevel + "!");

            return true;
        }
    }
}
