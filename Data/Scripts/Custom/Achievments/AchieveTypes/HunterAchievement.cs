using Server;
using Server.Mobiles;
using System;

namespace Scripts.Mythik.Systems.Achievements
{
    public class HunterAchievement : BaseAchievement
    {
        public readonly Type EnemyType;

        public HunterAchievement(int id, int catid, int itemIcon, bool hiddenTillComplete, BaseAchievement prereq, int total, string title, string desc, short rewardPoints, Type targets, params Type[] rewards)
            : base(id, catid, itemIcon, hiddenTillComplete, prereq, title, desc, rewardPoints, total, rewards)
        {
            EnemyType = targets;
            EventSink.OnKilledBy += EventSink_OnKilledBy;
        }

        private void EventSink_OnKilledBy(OnKilledByArgs e)
        {
            var player = e.KilledBy as PlayerMobile;
            if (player != null && e.Killed.GetType() == EnemyType)
            {
                AchievementSystem.SetAchievementStatus(player, this, 1);
            }
        }
    }
}
