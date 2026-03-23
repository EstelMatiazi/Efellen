using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class ReaverGorgeCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "ReaverGorge",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            ReaverGorgeAbility ability = new ReaverGorgeAbility();
            ability.Execute(pm);
        }
    }
}
