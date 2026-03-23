using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class PalemasterUndyingHordesCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("PalemasterUndyingHordes", AccessLevel.Player, new CommandEventHandler(OnCommand));
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            PalemasterUndyingHordesAbility ability = new PalemasterUndyingHordesAbility();
            ability.Execute(pm);
        }
    }
}
