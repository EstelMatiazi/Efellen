using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class KensaiBattleMeditationCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "KensaiBattleMeditation",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            KensaiBattleMeditationAbility ability = new KensaiBattleMeditationAbility();
            ability.Execute(pm);
        }
    }
}
