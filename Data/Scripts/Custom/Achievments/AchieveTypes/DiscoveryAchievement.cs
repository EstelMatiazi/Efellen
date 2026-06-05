using Server;
using Server.Mobiles;
using System;

namespace Scripts.Mythik.Systems.Achievements
{
    public class DiscoveryAchievement : BaseAchievement
    {
        public readonly string Region;

        public DiscoveryAchievement(int id, int catid, int itemIcon, bool hiddenTillComplete, BaseAchievement prereq, string title, string desc, short rewardPoints, string region, params Type[] rewards)
            : base(id, catid, itemIcon, hiddenTillComplete, prereq, title, desc, rewardPoints, 1, rewards)
        {
            Region = region;
            CompletionTotal = 1;
            EventSink.OnEnterRegion += EventSink_OnEnterRegion;
        }

        private void EventSink_OnEnterRegion(OnEnterRegionArgs e)
        {
            if (e == null || e.NewRegion == null || e.From == null || e.NewRegion.Name == null) return;

            var player = e.From as PlayerMobile;
            if (e.NewRegion.Name == Region && player != null)
            {
                AchievementSystem.SetAchievementStatus(player, this, 1);
            }
        }
    }
}
