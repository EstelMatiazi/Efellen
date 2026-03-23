using System;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class PalemasterCircleOfDeathCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("PalemasterCircleOfDeath", AccessLevel.Player, new CommandEventHandler(OnCommand));
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            PalemasterCircleOfDeathAbility ability = new PalemasterCircleOfDeathAbility();
            ability.Execute(pm);
        }
    }
}