using System;

namespace Server.Custom.Ascensions
{
    public static class AscensionTypeHelper
    {
        public static string GetDisplayName(AscensionType type)
        {
            //parsing the enuns for correct text display
            switch (type)
            {
                case AscensionType.ArcaneArcher: return "Arcane Archer";
                default: return type.ToString();
            }
        }
    }
}