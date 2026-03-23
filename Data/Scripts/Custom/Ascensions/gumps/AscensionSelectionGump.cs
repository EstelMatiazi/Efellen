using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public class AscensionSelectionGump : Gump
    {
        private PlayerMobile m_Player;

        public AscensionSelectionGump(PlayerMobile pm) : base(100, 100)
        {
            m_Player = pm;

            Closable = true;
            Dragable = true;
            Disposable = true;

            AddPage(0);
            AddBackground(0, 0, 650, 800, 9270);
            AddAlphaRegion(10, 10, 630, 780);
            AddLabel(260, 20, 1152, "Ascensions");

            int y = 60;

            foreach (AscensionType type in Enum.GetValues(typeof(AscensionType)))
            {
                if (type == AscensionType.None)
                    continue;

                AddEntry(type, y);
                y += 50;
            }
        }

        private void AddEntry(AscensionType type, int y)
        {
            AscensionProgress prog = m_Player.AscensionProfile.Get(type);
            AddLabel(30, y, 1152, AscensionTypeHelper.GetDisplayName(type)); // was type.ToString()
            if (prog.Unlocked)
            {
                AddLabel(200, y, 68, "Level: " + prog.Level);
                int required = prog.GetRequiredExperience();
                string expText = required > 0
                    ? "Experience: " + prog.Experience + " / " + required
                    : "Experience: MAX";
                AddLabel(280, y, 1152, expText);
                AddButton(540, y, 4005, 4007, 3000 + (int)type, GumpButtonType.Reply, 0);
                AddLabel(575, y, 68, "Activate");
            }
            else
            {
                AddButton(200, y, 4005, 4007, 1000 + (int)type, GumpButtonType.Reply, 0);
                AddLabel(235, y, 33, "Unlock");
            }
            AddButton(480, y, 4011, 4013, 2000 + (int)type, GumpButtonType.Reply, 0);
            AddLabel(515, y, 1152, "?");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 0)
                return;

            // Unlock button
            if (info.ButtonID >= 1000 && info.ButtonID < 2000)
            {
                AscensionType type = (AscensionType)(info.ButtonID - 1000);

                if (AscensionUnlocking.TryUnlock(m_Player, type))
                {
                    m_Player.SendMessage(0x55, "You have unlocked the " + AscensionTypeHelper.GetDisplayName(type) + " ascension."); // was type.ToString()
                }

                m_Player.SendGump(new AscensionSelectionGump(m_Player));
                return;
            }
            // info
            if (info.ButtonID >= 2000 && info.ButtonID < 3000)
            {
                AscensionType type = (AscensionType)(info.ButtonID - 2000);
                m_Player.SendGump(new AscensionDetailGump(m_Player, type));
                return;
            }
            // activate
            if (info.ButtonID >= 3000)
            {
                AscensionType type = (AscensionType)(info.ButtonID - 3000);

                if (m_Player.AscensionProfile.IsUnlocked(type))
                {
                    m_Player.ActivateAscension(type);
                }
                else
                {
                    m_Player.SendMessage("You must unlock this ascension first.");
                }
                m_Player.SendGump(new AscensionSelectionGump(m_Player));
            }
        }
    }
}
