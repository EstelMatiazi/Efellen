using System;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class CrusaderHeavenlyGateCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "CrusaderHeavenlyGate",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;
            if (pm == null)
                return;

            CrusaderHeavenlyGateAbility ability = new CrusaderHeavenlyGateAbility();
            ability.Execute(pm);
        }
    }
}
