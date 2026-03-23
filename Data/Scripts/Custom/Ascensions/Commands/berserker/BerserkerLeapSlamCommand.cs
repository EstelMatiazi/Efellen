using System;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class BerserkerLeapSlamCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "BerserkerLeapSlam",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;
            if (pm == null)
                return;

            BerserkerLeapSlamAbility ability = new BerserkerLeapSlamAbility();
            ability.Execute(pm);
        }
    }
}
