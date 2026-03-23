using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class BlackguardDeathsAdvanceCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "BlackguardDeathsAdvance",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            BlackguardDeathsAdvanceAbility ability = new BlackguardDeathsAdvanceAbility();
            ability.Execute(pm);
        }
    }
}
