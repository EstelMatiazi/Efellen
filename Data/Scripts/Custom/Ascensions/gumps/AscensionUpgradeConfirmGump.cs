using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Custom.Ascensions;

namespace Server.Custom.Ascensions
{
    public class AscensionUpgradeConfirmGump : Gump
    {
        private PlayerMobile m_Player;
        private AscensionType m_Type;

        public AscensionUpgradeConfirmGump(PlayerMobile pm, AscensionType type)
            : base(150, 150)
        {
            m_Player = pm;
            m_Type = type;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            const int width = 440;
            const int height = 260;

            AddPage(0);

            // Main Frame
            AddBackground(0, 0, width, height, 0x1453);
            AddImageTiled(10, 10, width - 20, height - 20, 0xA40);
            AddAlphaRegion(10, 10, width - 20, height - 20);

            // Class Icon
            AscensionVisuals.DrawClassIcon(this, 25, 20, type, 46);

            AscensionProgress prog = pm.AscensionProfile.Get(type);
            bool isUnlock = !prog.Unlocked || prog.Level == 0;
            int nextLevel = prog.Level + 1;

            string hexColor = AscensionVisuals.GetColorHex(type);
            string displayName = AscensionTypeHelper.GetDisplayName(type);

            string titleText = isUnlock ? "Ascension Unlock" : "Level Evolution";
            AddHtml(80, 20, 330, 22, $"<BIG><BASEFONT COLOR={hexColor}>{titleText}</BASEFONT></BIG>", false, false);

            string promptText = isUnlock
                ? $"Do you want to unlock the {displayName} class?"
                : $"Do you want to evolve {displayName} to Level {nextLevel}?";
            AddHtml(80, 42, 330, 20, $"<BASEFONT COLOR=#BDC3C7>{promptText}</BASEFONT>", false, false);

            // Subtle divider
            AddImageTiled(20, 74, width - 40, 2, 0xBB8);

            // Costs
            int gold, dust, scrolls;
            if (isUnlock)
            {
                AscensionUnlocking.GetUnlockCost(pm, out gold, out dust, out scrolls);
            }
            else
            {
                gold = AscensionCosts.GetGoldCost(nextLevel);
                dust = AscensionCosts.GetDustCost(nextLevel);
                scrolls = AscensionCosts.GetScrollCost(nextLevel);
            }

            // Display 3 resources in styled cards
            int resY = 90;

            // Gold
            AddBackground(25, resY, 120, 80, 0x2422);
            AddImageTiled(28, resY + 3, 114, 74, 0xA40);
            AddAlphaRegion(28, resY + 3, 114, 74);
            AddItem(68, resY + 12, 0xEEF);
            bool hasGold = AscensionUnlocking.HasGold(pm, gold);
            string goldColor = hasGold ? "#2ECC71" : "#E74C3C";
            AddHtml(25, resY + 52, 120, 20, $"<CENTER><BASEFONT COLOR={goldColor}>{gold} Gold</BASEFONT></CENTER>", false, false);

            // Arcane Dust
            AddBackground(160, resY, 120, 80, 0x2422);
            AddImageTiled(163, resY + 3, 114, 74, 0xA40);
            AddAlphaRegion(163, resY + 3, 114, 74);
            AddItem(203, resY + 12, 0x0F8F);
            bool hasDust = AscensionUnlocking.HasArcaneDust(pm, dust);
            string dustColor = hasDust ? "#2ECC71" : "#E74C3C";
            AddHtml(160, resY + 52, 120, 20, $"<CENTER><BASEFONT COLOR={dustColor}>{dust} Arcane Dust</BASEFONT></CENTER>", false, false);

            // Scrolls
            AddBackground(295, resY, 120, 80, 0x2422);
            AddImageTiled(298, resY + 3, 114, 74, 0xA40);
            AddAlphaRegion(298, resY + 3, 114, 74);
            AddItem(338, resY + 12, 0xEF3, AscensionVisuals.GetItemHue(type));
            bool hasScrolls = AscensionUnlocking.HasScrolls(pm, type, scrolls);
            string scrollColor = hasScrolls ? "#2ECC71" : "#E74C3C";
            AddHtml(295, resY + 52, 120, 20, $"<CENTER><BASEFONT COLOR={scrollColor}>{scrolls} Scroll{(scrolls != 1 ? "s" : "")}</BASEFONT></CENTER>", false, false);

            // Subtle divider
            AddImageTiled(20, 185, width - 40, 1, 0xBB8);

            // Confirm and Cancel Buttons
            int btnY = 202;

            // Confirm
            AddButton(70, btnY, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
            AddHtml(105, btnY + 2, 90, 20, "<BASEFONT COLOR=#2ECC71>Confirm</BASEFONT>", false, false);

            // Cancel
            AddButton(250, btnY, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
            AddHtml(285, btnY + 2, 90, 20, "<BASEFONT COLOR=#E74C3C>Cancel</BASEFONT>", false, false);
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                if (m_Player.AscensionProfile.IsUnlocked(m_Type))
                {
                    AscensionUpgradeSystem.TryUpgrade(m_Player, m_Type);
                }
                else
                {
                    AscensionUnlocking.TryUnlock(m_Player, m_Type);
                }
            }

            m_Player.SendGump(new AscensionDetailGump(m_Player, m_Type));
        }
    }
}