using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class AssassinCripplingPoisonCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "AssassinCripplingPoison",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            AssassinCripplingPoisonAbility ability = new AssassinCripplingPoisonAbility();
            ability.Execute(pm);
        }
    }
}
