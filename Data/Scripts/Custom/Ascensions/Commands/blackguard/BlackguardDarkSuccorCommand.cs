using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class BlackguardDarkSuccorCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "BlackguardDarkSuccor",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            BlackguardDarkSuccorAbility ability = new BlackguardDarkSuccorAbility();
            ability.Execute(pm);
        }
    }
}
