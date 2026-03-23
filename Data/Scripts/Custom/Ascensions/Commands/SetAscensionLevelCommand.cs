using System;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class SetAscensionLevelCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register(
                "SetAscensionLevel",
                AccessLevel.GameMaster,
                new CommandEventHandler(OnCommand)
            );
        }

        private static void OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (!(from is PlayerMobile))
                return;

            if (e.Length < 2)
            {
                from.SendMessage("Usage: [SetAscensionLevel <AscensionType> <Level>");
                return;
            }

            PlayerMobile pm = (PlayerMobile)from;

            AscensionType type;
            if (!Enum.TryParse<AscensionType>(e.GetString(0), true, out type))
            {
                from.SendMessage("Invalid ascension type.");
                return;
            }

            int level;
            if (!int.TryParse(e.GetString(1), out level))
            {
                from.SendMessage("Invalid level.");
                return;
            }

            if (level < 0)
                level = 0;

            if (level > 20)
                level = 20;

            if (pm.AscensionProfile == null)
            {
                from.SendMessage("You do not have an ascension profile.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(type);

            prog.Unlocked = true; 
            prog.Level = level;

            from.SendMessage(
                String.Format("Your {0} ascension is now level {1}.", type, level)
            );
        }
    }
}
