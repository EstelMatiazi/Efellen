using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class SkaldDirgeOfTheFallenCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "SkaldDirgeOfTheFallen",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            SkaldDirgeOfTheFallenAbility ability = new SkaldDirgeOfTheFallenAbility();
            ability.Execute(pm);
        }
    }
}
