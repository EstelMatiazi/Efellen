using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class ReaverBloodstormCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "ReaverBloodstorm",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            ReaverBloodstormAbility ability = new ReaverBloodstormAbility();
            ability.Execute(pm);
        }
    }
}
