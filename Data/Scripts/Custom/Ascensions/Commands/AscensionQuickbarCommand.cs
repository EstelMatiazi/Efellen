using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Custom.Ascensions;

namespace Server.Custom.Ascensions
{
    public class AscensionQuickbarCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("AscensionQuickbar", AccessLevel.Player, OnCommand);
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            if (!pm.HasActiveAscension)
            {
                pm.SendMessage("You do not have an active ascension.");
                return;
            }

            pm.CloseGump(typeof(AscensionQuickbarGump));
            pm.SendGump(new AscensionQuickbarGump(pm));
        }
    }
}
