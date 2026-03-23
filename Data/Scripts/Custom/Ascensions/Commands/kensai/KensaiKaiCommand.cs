using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class KensaiKaiCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "KensaiKai",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            KensaiKaiAbility ability = new KensaiKaiAbility();
            ability.Execute(pm);
        }
    }
}
