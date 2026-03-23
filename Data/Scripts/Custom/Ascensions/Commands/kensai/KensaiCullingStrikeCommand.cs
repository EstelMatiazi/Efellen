using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class KensaiCullingStrikeCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "KensaiCullingStrike",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            KensaiCullingStrikeAbility ability = new KensaiCullingStrikeAbility();
            ability.Execute(pm);
        }
    }
}
