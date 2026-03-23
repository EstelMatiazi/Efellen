using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class BerserkerRageCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("BerserkerRage", AccessLevel.Player, OnCommand);
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            AscensionAbility ability = AscensionAbilityRegistry.GetAbilityByName("Berserker Rage");

            if (ability != null)
                ability.Execute(pm);
        }
    }
}
