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
            : base(200, 200)
        {
            m_Player = pm;
            m_Type = type;

            AddBackground(0, 0, 350, 200, 9270);
       

            AscensionProgress prog = pm.AscensionProfile.Get(type);
            bool isUnlock = prog.Level == 0;
            int nextLevel = prog.Level + 1;

            AddLabel(80, 20, 1152, isUnlock 
            ? "Unlock " + AscensionTypeHelper.GetDisplayName(type) + "?"
            : "Upgrade " + AscensionTypeHelper.GetDisplayName(type) + " to Level " + nextLevel + "?");


            int gold = AscensionCosts.GetGoldCost(nextLevel);
            int dust = AscensionCosts.GetDustCost(nextLevel);
            int scrolls = AscensionCosts.GetScrollCost(nextLevel);

            AddLabel(40, 60, 1153, "Gold: " + gold);
            AddLabel(40, 80, 1153, "Arcane Dust: " + dust);
            AddLabel(40, 100, 1153, "Scrolls: " + scrolls);

            AddButton(60, 130, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddLabel(95, 130, 68, "Yes");

            AddButton(200, 130, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddLabel(235, 130, 33, "No");
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                AscensionUpgradeSystem.TryUpgrade(m_Player, m_Type);
            }

            m_Player.SendGump(new AscensionDetailGump(m_Player, m_Type));
        }
    }
}