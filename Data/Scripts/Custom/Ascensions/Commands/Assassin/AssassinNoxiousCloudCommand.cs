using System;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class AssassinNoxiousCloudCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("AssassinNoxiousCloudAbility", AccessLevel.Player, new CommandEventHandler(OnCommand));
        }

        private static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            AssassinNoxiousCloudAbility ability = new AssassinNoxiousCloudAbility();
            ability.Execute(pm);
        }
    }
}