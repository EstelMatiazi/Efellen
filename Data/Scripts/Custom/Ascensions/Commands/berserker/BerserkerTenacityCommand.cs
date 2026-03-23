using System;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class BerserkerTenacityCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "BerserkerTenacity",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;
            if (pm == null)
                return;

            BerserkerTenacityAbility ability = new BerserkerTenacityAbility();
            ability.Execute(pm);
        }
    }
}
