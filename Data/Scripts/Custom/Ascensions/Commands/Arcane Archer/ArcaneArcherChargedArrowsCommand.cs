using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class ArcaneArcherChargedArrowsCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "ArcaneArcherChargedArrows",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            ArcaneArcherChargedArrowsAbility ability = new ArcaneArcherChargedArrowsAbility();
            ability.Execute(pm);
        }
    }
}
