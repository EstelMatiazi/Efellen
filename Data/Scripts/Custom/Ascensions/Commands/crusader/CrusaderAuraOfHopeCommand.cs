using System;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class CrusaderAuraOfHopeCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "CrusaderAuraOfHope",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;
            if (pm == null)
                return;

            CrusaderAuraOfHopeAbility ability = new CrusaderAuraOfHopeAbility();
            ability.Execute(pm);
        }
    }
}
