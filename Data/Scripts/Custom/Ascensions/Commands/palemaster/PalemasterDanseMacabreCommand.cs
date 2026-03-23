using System;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class PalemasterDanseMacabreCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("PalemasterDanseMacabre", AccessLevel.Player, new CommandEventHandler(OnCommand));
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            PalemasterDanseMacabreAbility ability = new PalemasterDanseMacabreAbility();
            ability.Execute(pm);
        }
    }
}