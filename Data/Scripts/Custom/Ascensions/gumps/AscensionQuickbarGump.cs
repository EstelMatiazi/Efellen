using Server.Gumps;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Custom.Ascensions
{
    public class AscensionQuickbarGump : Gump
    {
        private PlayerMobile _pm;

        private const int GumpWidth  = 250;
        private const int RowHeight  = 32;
        private const int TopPadding = 15;
        private const int XPSection  = 20;

        public AscensionQuickbarGump(PlayerMobile pm) : base(20, 200)
        {
            _pm = pm;

            List<AscensionAbility> usable = GetUsableAbilities(pm);

            int height = TopPadding + XPSection + (usable.Count * RowHeight) + 10;

            AddBackground(0, 0, GumpWidth, height, 9270);

            AscensionProgress prog = pm.AscensionProfile.Get(pm.ActiveAscension);

            int required = prog.GetRequiredExperience();
            string xpText = required > 0
                ? "Experience: " + prog.Experience + " / " + required
                : "Experience: MAX";

            AddLabel(15, TopPadding, 1152, xpText);

            int y        = TopPadding + XPSection;
            int buttonID = 1;

            foreach (AscensionAbility ability in usable)
            {
                AddButton(10, y, 4005, 4007, buttonID, GumpButtonType.Reply, 0);
                AddLabel(45, y + 3, 1152, ability.DisplayName);

                y        += RowHeight;
                buttonID++;
            }
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (_pm == null || _pm.Deleted)
                return;

            List<AscensionAbility> usable = GetUsableAbilities(_pm);

            int index = info.ButtonID - 1;

            if (index >= 0 && index < usable.Count)
            {
                usable[index].Execute(_pm);

                _pm.CloseGump(typeof(AscensionQuickbarGump));
                _pm.SendGump(new AscensionQuickbarGump(_pm));
            }
        }

        private static List<AscensionAbility> GetUsableAbilities(PlayerMobile pm)
        {
            List<AscensionAbility> abilities = AscensionAbilityRegistry.GetAbilities(pm.ActiveAscension);
            List<AscensionAbility> usable    = new List<AscensionAbility>();

            foreach (AscensionAbility ability in abilities)
            {
                if (ability.IsPassive)
                    continue;

                if (!ability.CanUse(pm))
                    continue;

                usable.Add(ability);
            }

            return usable;
        }
    }
}