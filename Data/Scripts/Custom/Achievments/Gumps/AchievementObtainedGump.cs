using Server;
using Server.Gumps;
using Server.Utilities;

namespace Scripts.Mythik.Systems.Achievements.Gumps
{
    class AchievementObtainedGump : Gump
    {
        public AchievementObtainedGump(BaseAchievement ach) : base(300, 25)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;
            AddPage(0);
            AddBackground(0, 13, 350, 100, 3600);
            TextDefinition.AddHtmlText(this, 0, 0, 350, 16, "<CENTER>ACHIEVEMENT COMPLETED</CENTER>", false, false, AchievementGump.COLOR_LOCALIZED, AchievementGump.COLOR_HTML);

            if (ach.ItemIcon > 0)
				GumpUtilities.AddCenteredItemToGump(this, ach.ItemIcon, 15, 27, 67, 72);

            AddLabel(82, 31, 49, ach.Title);
            TextDefinition.AddHtmlText(this, 81, 56, 250, 41, ach.Desc, false, false, AchievementGump.COLOR_LOCALIZED, AchievementGump.COLOR_HTML);
        }
    }
}
