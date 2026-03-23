using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class AssassinCleansingAnnihilationCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "AssassinCleansingAnnihilation",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            AssassinCleansingAnnihilationAbility ability = new AssassinCleansingAnnihilationAbility();
            ability.Execute(pm);
        }
    }
}
