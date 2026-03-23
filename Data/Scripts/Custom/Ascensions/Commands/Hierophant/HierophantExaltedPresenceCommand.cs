using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class HierophantExaltedPresenceCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "HierophantExaltedPresence",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            HierophantExaltedPresenceAbility ability = new HierophantExaltedPresenceAbility();
            ability.Execute(pm);
        }
    }
}
