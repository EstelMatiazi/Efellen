using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class SkaldWarChantCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "SkaldWarChant",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            SkaldWarChantAbility ability = new SkaldWarChantAbility();
            ability.Execute(pm);
        }
    }
}
