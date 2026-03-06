using System;
using Server;

namespace Server.Custom
{
    public class CompanionInit
    {
        public static void Initialize()
        {
            EventSink.WorldLoad += new WorldLoadEventHandler(OnLoad);
        }

        private static void OnLoad()
        {
            Server.Companions.Systems.CompanionSystem.Initialize();
            Server.Companions.Commands.CompanionCommands.Initialize();
        }
    }
}