using Server;
using Server.Mobiles;
using System;

namespace Scripts.Mythik.Systems.Achievements
{
    public class HarvestAchievement : BaseAchievement
    {
        private readonly Func<Item, bool> m_Predicate;

        public HarvestAchievement(int id, int catid, int itemIcon, bool hiddenTillComplete, BaseAchievement prereq, int total, string title, string desc, short rewardPoints, Func<Item, bool> predicate, params Type[] rewards)
            : base(id, catid, itemIcon, hiddenTillComplete, prereq, title, desc, rewardPoints, total, rewards)
        {
            m_Predicate = predicate;
            EventSink.ResourceHarvestSuccess += EventSink_ResourceHarvestSuccess;
        }

        private void EventSink_ResourceHarvestSuccess(ResourceHarvestSuccessArgs e)
        {
            var player = e.Harvester as PlayerMobile;
            if (m_Predicate(e.Resource))
            {
                AchievementSystem.SetAchievementStatus(player, this, e.Resource.Amount);
            }
        }
    }
}
