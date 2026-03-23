using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Custom.Ascensions;
namespace Server.Custom.Ascensions
{
    public class AscensionDetailGump : Gump
    {
        private PlayerMobile m_From;
        private string m_Key;
        private AscensionProgress m_Progress;

        private const int Button_Unlock = 1;
        private const int Button_LevelUp = 2;
        private const int Button_Back = 3;

        private AscensionType m_Type;

        public AscensionDetailGump(PlayerMobile from, AscensionType type): base(100, 100)
        {
            m_From = from;
            m_Type = type;

            m_Progress = from.AscensionProfile.Get(type);

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            BuildGump();
        }

        private void BuildGump()
        {
            AddPage(0);

            AddBackground(0, 0, 520, 610, 9270);
            AddAlphaRegion(10, 10, 510, 600);

            AddLabel(140, 20, 1152, AscensionTypeHelper.GetDisplayName(m_Type));

            AddLabel(30, 85, 88, "Level:");
            AddLabel(200, 85, 1152, m_Progress.Level.ToString());

            AddLabel(30, 110, 88, "Experience:");
            AddLabel(200, 110, 1152, m_Progress.Experience.ToString());

            int required = m_Progress.GetRequiredExperience();

            AddLabel(30, 135, 88, "Next Level XP:");
            AddLabel(200, 135, 1152, required.ToString());

            AddLabel(30, 200, 1152, "Description:");
            AddHtml(30, 230, 460, 80, AscensionDefinitions.GetDescription(m_Type), true, true);

            AddLabel(30, 340, 1152, "Granted Abilities:");
            AddHtml(30, 360, 460, 140, AscensionDefinitions.GetAbilities(m_Type), true, true);

            int bottomY = 560;

            if (m_From.AscensionProfile.ActiveAscension == m_Type)
            {
                AddLabel(320, bottomY, 63, "Currently Active");
            }

            AddButton(40, bottomY, 4014, 4016, Button_Back, GumpButtonType.Reply, 0);
            AddLabel(75, bottomY, 1152, "Back");

            if (m_Progress.CanLevelUp())
            {
                int next = m_Progress.GetNextLevel();
                int gold = AscensionCosts.GetGoldCost(next);
                int dust = AscensionCosts.GetDustCost(next);
                int scrolls = AscensionCosts.GetScrollCost(next);

                int upgradeY = 510;

                AddLabel(260, upgradeY, 1153,
                    "Cost: " + gold + "g / " +
                    dust + " dust / " +
                    scrolls + " scrolls");

                AddButton(260, upgradeY + 25, 4005, 4007, 3000 + (int)m_Type, GumpButtonType.Reply, 0);
                AddLabel(295, upgradeY + 25, 68, "Upgrade");
            }

            if (m_Progress.Level > 0)
            {
                AddButton(320, bottomY, 4005, 4007, 2, GumpButtonType.Reply, 0);
                AddLabel(355, bottomY, 1152, "Activate Ascension");
            }
        }
        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case Button_Back:
                    m_From.SendGump(new AscensionSelectionGump(m_From));
                    return;

                case 2:
                    m_From.ActivateAscension(m_Type);
                    Refresh();
                    return;
            }

            if (info.ButtonID >= 3000)
            {
                AscensionType type = (AscensionType)(info.ButtonID - 3000);
                m_From.SendGump(new AscensionUpgradeConfirmGump(m_From, type));
            }
        }


        private void Refresh()
        {
            m_From.SendGump(new AscensionDetailGump(m_From, m_Type));
        }
    }
}
