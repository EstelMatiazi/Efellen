using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class HierophantDivinePowerCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "HierophantDivinePower",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            HierophantDivinePowerAbility ability = new HierophantDivinePowerAbility();
            ability.Execute(pm);
        }
    }
}
