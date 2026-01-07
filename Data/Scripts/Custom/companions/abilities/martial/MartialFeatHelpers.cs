using Server.Companions.Core;

namespace Server.Companions.Abilities
{
    public static class MartialFeatHelpers
    {
        public static int GetStaminaCost(CompanionMobile companion)
        {
            int cost = companion.Level * 4;

            if (cost < 25)
                cost = 25;

            return cost;
        }
    }
}
