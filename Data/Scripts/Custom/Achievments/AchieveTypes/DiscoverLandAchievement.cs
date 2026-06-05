using Server;
using Server.Mobiles;
using System;

namespace Scripts.Mythik.Systems.Achievements
{
    public class DiscoverLandAchievement : BaseAchievement
    {
        public readonly Land Land;

        public DiscoverLandAchievement(int id, int catid, int itemIcon, bool hiddenTillComplete, BaseAchievement prereq, string title, string desc, short rewardPoints, Land land, params Type[] rewards)
            : base(id, catid, itemIcon, hiddenTillComplete, prereq, title, desc, rewardPoints, 1, rewards)
        {
            HideDesc = true;
            Land = land;
            CompletionTotal = 1;
            CustomEventSink.LandChanged += EventSink_OnLandChanged;
            EventSink.Login += EventSink_OnLogin; // Ensure Players always have their Land discovered
        }

        private void EventSink_OnLogin(LoginEventArgs e)
        {
            if (e == null || e.Mobile == null) return;

            var player = e.Mobile as PlayerMobile;
            if (player == null) return;

            if (player.Land == Land)
            {
                AchievementSystem.SetAchievementStatus(player, this, 1);
            }
        }


        private void EventSink_OnLandChanged(LandChangedArgs e)
        {
            if (e == null || e.Mobile == null) return;

            if (e.NewLand == Land)
            {
                AchievementSystem.SetAchievementStatus(e.Mobile, this, 1);
            }
        }
    }
}
