using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class ReaverExsanguinateCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "ReaverExsanguinate",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            ReaverExsanguinateAbility ability = new ReaverExsanguinateAbility();
            ability.Execute(pm);
        }
    }
}
