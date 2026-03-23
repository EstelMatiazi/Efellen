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
                pm.SendMessage("You do not have enough experience.");
                return false;
            }

            int nextLevel = prog.GetNextLevel();

            int goldCost = AscensionCosts.GetGoldCost(nextLevel);
            int scrollCost = AscensionCosts.GetScrollCost(nextLevel);
            int dustCost = AscensionCosts.GetDustCost(nextLevel);
            
            if (prog.Level >= AscensionConstants.MaxAscensionLevel)
            {
                pm.SendMessage("You already mastered this ascension.");
                return false;
            }


            if (!AscensionUnlocking.HasGold(pm, goldCost))
            {
                pm.SendMessage("You do not have enough gold.");
                return false;
            }

            if (!AscensionUnlocking.HasArcaneDust(pm, dustCost))
            {
                pm.SendMessage("You do not have enough arcane dust.");
                return false;
            }

            if (!AscensionUnlocking.HasScrolls(pm, type, scrollCost))
            {
                pm.SendMessage("You do not have enough ascension scrolls.");
                return false;
            }

            AscensionUnlocking.ConsumeGold(pm, goldCost);
            AscensionUnlocking.ConsumeArcaneDust(pm, dustCost);
            AscensionUnlocking.ConsumeScrolls(pm, type, scrollCost);

            prog.Level = nextLevel;

            pm.SendMessage("Your " + type.ToString() + " ascension has reached level " + nextLevel + "!");

            return true;
        }
    }
}
