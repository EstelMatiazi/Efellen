using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class SkaldSongOfThunderCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "SkaldSongOfThunder",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            SkaldSongOfThunderAbility ability = new SkaldSongOfThunderAbility();
            ability.Execute(pm);
        }
    }
}
