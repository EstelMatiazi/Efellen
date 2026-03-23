using System;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class CrusaderChargeCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "CrusaderCharge",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;
            if (pm == null)
                return;

            CrusaderChargeAbility ability = new CrusaderChargeAbility();
            ability.Execute(pm);
        }
    }
}
