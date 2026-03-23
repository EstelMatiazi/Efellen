using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class AssassinToxicSurgeCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "AssassinToxicSurge",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            AssassinToxicSurgeAbility ability = new AssassinToxicSurgeAbility();
            ability.Execute(pm);
        }
    }
}
