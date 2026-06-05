using Server;
using Server.Mobiles;
using System;

namespace Scripts.Mythik.Systems.Achievements
{
    public class ItemCraftedAchievement : BaseAchievement
    {
        private readonly Type m_Item;

        public ItemCraftedAchievement(int id, int catid, int itemIcon, bool hiddenTillComplete, BaseAchievement prereq, int total, string title, string desc, short rewardPoints, Type item, params Type[] rewards)
            : base(id, catid, itemIcon, hiddenTillComplete, prereq, title, desc, rewardPoints, total, rewards)
        {
            m_Item = item;
            EventSink.CraftSuccess += EventSink_CraftSuccess; ;
        }

        private void EventSink_CraftSuccess(CraftSuccessArgs e)
        {
            if (e.Crafter is PlayerMobile && e.CraftedItem.GetType() == m_Item)
            {
                AchievementSystem.SetAchievementStatus(e.Crafter as PlayerMobile, this, e.CraftedItem.Amount);
            }
        }
    }
}
