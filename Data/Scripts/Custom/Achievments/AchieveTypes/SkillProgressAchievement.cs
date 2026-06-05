using Server;
using Server.Mobiles;
using System;

namespace Scripts.Mythik.Systems.Achievements
{
    /// <summary>
    /// Achievement to handle hitting X in skill Y
    /// Comment out if you are missing the SkillGain eventsink
    /// </summary>
    class SkillProgressAchievement : BaseAchievement
    {
        private readonly SkillName m_Skill;

        public SkillProgressAchievement(int id, int catid, int itemIcon, bool hiddenTillComplete, BaseAchievement prereq, int total, string title, string desc, SkillName skill, short rewardPoints, params Type[] rewards)
            : base(id, catid, itemIcon, hiddenTillComplete, prereq, title, desc, rewardPoints, total, rewards)
        {
            m_Skill = skill;

            EventSink.SkillGain += EventSink_SkillGain;
        }

        private void EventSink_SkillGain(SkillGainArgs e)
        {
            if (e.From is PlayerMobile && e.Skill.SkillID == (int)m_Skill)
            {
                if (e.Skill.Base >= CompletionTotal)
                    AchievementSystem.SetAchievementStatus(e.From as PlayerMobile, this, e.Skill.BaseFixedPoint);
            }
        }
    }
}
