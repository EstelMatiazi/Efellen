using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Custom.Ascensions;

namespace Server.Custom.Ascensions
{
    public class AscensionCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("Ascension", AccessLevel.Player, OnCommand);
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            pm.CloseGump(typeof(AscensionSelectionGump));
            pm.SendGump(new AscensionSelectionGump(pm));
        }
    }
}
