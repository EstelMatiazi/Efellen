using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public class AscensionSelectionGump : Gump
    {
        private PlayerMobile m_Player;

        public AscensionSelectionGump(PlayerMobile pm) : base(60, 40)
        {
            m_Player = pm;

            Closable = true;
            Dragable = true;
            Disposable = true;
            Resizable = false;

            // Valid Classes
            List<AscensionType> classes = new List<AscensionType>();
            foreach (AscensionType type in Enum.GetValues(typeof(AscensionType)))
            {
                if (type != AscensionType.None)
                    classes.Add(type);
            }

            const int itemsPerPage = 7;
            int totalPages = (int)Math.Ceiling((double)classes.Count / itemsPerPage);
            if (totalPages < 1)
                totalPages = 1;

            // Página 0: Main Frame and Headers
            AddPage(0);

            // Dark background with refined border
            AddBackground(0, 0, 700, 620, 0x1453);
            AddImageTiled(10, 10, 680, 600, 0xA40);
            AddAlphaRegion(10, 10, 680, 600);

            // Styled Header
            AddHtml(20, 16, 660, 24, "<CENTER><BIG><BASEFONT COLOR=#F4D03F><B>ASCENSIONS</B></BASEFONT></BIG></CENTER>", false, false);
            AddHtml(20, 38, 660, 18, "<CENTER><BASEFONT COLOR=#BDC3C7><SMALL>Awaken the ancestral powers and raise the potential of your spirit</SMALL></BASEFONT></CENTER>", false, false);

            // Subtle divider
            AddImageTiled(20, 58, 660, 2, 0xBB8);

            // Page creation
            for (int page = 1; page <= totalPages; page++)
            {
                AddPage(page);

                int startIdx = (page - 1) * itemsPerPage;
                int endIdx = Math.Min(startIdx + itemsPerPage, classes.Count);

                int y = 68;

                for (int i = startIdx; i < endIdx; i++)
                {
                    AddClassEntry(classes[i], y);
                    y += 70;
                }

                // Page Footer
                int footerY = 574;
                AddImageTiled(20, footerY - 8, 660, 1, 0xBB8);

                AddHtml(260, footerY, 180, 20, $"<CENTER><BASEFONT COLOR=#BDC3C7>Page {page} of {totalPages}</BASEFONT></CENTER>", false, false);

                if (page > 1)
                {
                    AddButton(30, footerY - 2, 0x15E3, 0x15E7, 0, GumpButtonType.Page, page - 1);
                    AddHtml(50, footerY, 120, 20, "<BASEFONT COLOR=#E5E7E9><SMALL>Previous Page</SMALL></BASEFONT>", false, false);
                }

                if (page < totalPages)
                {
                    AddButton(645, footerY - 2, 0x15E1, 0x15E5, 0, GumpButtonType.Page, page + 1);
                    AddHtml(530, footerY, 110, 20, "<RIGHT><BASEFONT COLOR=#E5E7E9><SMALL>Next Page</SMALL></BASEFONT></RIGHT>", false, false);
                }
            }
        }

        private void AddClassEntry(AscensionType type, int y)
        {
            AscensionProgress prog = m_Player.AscensionProfile.Get(type);
            bool isActive = m_Player.AscensionProfile.ActiveAscension == type;

            // Card
            AddBackground(20, y, 660, 64, 0x2422);
            AddImageTiled(24, y + 4, 652, 56, 0xA40);
            AddAlphaRegion(24, y + 4, 652, 56);

            // Active Class Highlight
            if (isActive)
            {
                AddImageTiled(20, y, 660, 2, 0x805);
                AddImageTiled(20, y + 62, 660, 2, 0x805);
            }

            // Class Icon with Slot
            AscensionVisuals.DrawClassIcon(this, 30, y + 9, type, 46);

            // Name and Subtitle
            string hexColor = AscensionVisuals.GetColorHex(type);
            string displayName = AscensionTypeHelper.GetDisplayName(type);
            string subTitle = AscensionVisuals.GetSubTitle(type);

            AddHtml(85, y + 10, 220, 22, $"<BASEFONT COLOR={hexColor}>{displayName}</BASEFONT>", false, false);
            AddHtml(85, y + 30, 220, 20, $"<BASEFONT COLOR=#8E999E><SMALL><I>{subTitle}</I></SMALL></BASEFONT>", false, false);
            if (prog.Unlocked)
            {
                // Level and XP Bar
                int required = prog.GetRequiredExperience();
                double pct = required > 0 ? Math.Min(1.0, (double)prog.Experience / required) : 1.0;
                string xpText = required > 0 ? $"{prog.Experience} / {required}" : "MAX";

                AddHtml(305, y + 8, 180, 20, $"<BASEFONT COLOR=#82E0AA>Level {prog.Level}</BASEFONT>", false, false);
                AscensionVisuals.DrawProgressBar(this, 305, y + 28, 170, 16, pct, xpText);

                // Status / Activate Button
                if (isActive)
                {
                    AddHtml(495, y + 20, 90, 24, "<CENTER><BASEFONT COLOR=#F4D03F>★ ACTIVE</BASEFONT></CENTER>", false, false);
                }
                else
                {
                    AddButton(495, y + 18, 0xFA5, 0xFA7, 3000 + (int)type, GumpButtonType.Reply, 0);
                    AddHtml(530, y + 20, 60, 20, "<BASEFONT COLOR=#90EE90>Activate</BASEFONT>", false, false);
                }
            }
            else
            {
                AddHtml(310, y + 20, 140, 20, "<BASEFONT COLOR=#7F8C8D><I>Locked</I></BASEFONT>", false, false);

                // Unlock Button
                AddButton(480, y + 18, 0xFA5, 0xFA7, 1000 + (int)type, GumpButtonType.Reply, 0);
                AddHtml(515, y + 20, 90, 20, "<BASEFONT COLOR=#E59866>Unlock</BASEFONT>", false, false);
            }
            AddButton(615, y + 18, 0x15E1, 0x15E5, 2000 + (int)type, GumpButtonType.Reply, 0);
            AddHtml(636, y + 19, 30, 20, "<BASEFONT COLOR=#BDC3C7><SMALL>View</SMALL></BASEFONT>", false, false);
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
