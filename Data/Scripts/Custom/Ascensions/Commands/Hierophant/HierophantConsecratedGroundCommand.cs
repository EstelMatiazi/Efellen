using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class HierophantConsecratedGroundCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "HierophantConsecratedGround",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            HierophantConsecratedGroundAbility ability = new HierophantConsecratedGroundAbility();
            ability.Execute(pm);
        }
    }
}
