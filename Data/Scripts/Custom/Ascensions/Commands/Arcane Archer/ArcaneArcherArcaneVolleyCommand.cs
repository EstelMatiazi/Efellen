using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class ArcaneArcherArcaneVolleyCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "ArcaneArcherArcaneVolley",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            ArcaneArcherArcaneVolleyAbility ability = new ArcaneArcherArcaneVolleyAbility();
            ability.Execute(pm);
        }
    }
}
