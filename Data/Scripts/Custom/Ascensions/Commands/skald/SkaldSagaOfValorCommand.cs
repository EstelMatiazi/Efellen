using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class SkaldSagaOfValorCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "SkaldSagaOfValor",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            SkaldSagaOfValorAbility ability = new SkaldSagaOfValorAbility();
            ability.Execute(pm);
        }
    }
}
