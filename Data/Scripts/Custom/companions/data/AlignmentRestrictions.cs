using System;

namespace Server.Companions.Data
{
    [Flags]
    public enum AlignmentRestrictions
    {
        None        = 0,
        NonGood     = 1 << 0,
        NonEvil     = 1 << 1,
        NonLawful   = 1 << 2,
        NonChaotic  = 1 << 3,
        NonNeutral  = 1 << 4
    }
}
