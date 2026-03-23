using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class PaleMasterEnervateCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "PalemasterEnervate",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            PaleMasterEnervateAbility ability = new PaleMasterEnervateAbility();
            ability.Execute(pm);
        }
    }
}
