using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class ArchmageConfluxCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "ArchmageConflux",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            ArchmageConfluxAbility ability = new ArchmageConfluxAbility();
            ability.Execute(pm);
        }
    }
}
