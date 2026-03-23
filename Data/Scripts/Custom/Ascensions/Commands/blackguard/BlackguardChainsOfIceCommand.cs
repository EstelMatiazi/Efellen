using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class BlackguardChainsOfIceCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "BlackguardChainsOfIce",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            BlackguardChainsOfIceAbility ability = new BlackguardChainsOfIceAbility();
            ability.Execute(pm);
        }
    }
}
