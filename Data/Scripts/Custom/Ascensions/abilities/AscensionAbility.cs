using System;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public abstract class AscensionAbility
    {
        public abstract string Name { get; }
        public virtual string DisplayName { get { return Name; } }
        public abstract AscensionType Ascension { get; }
        public abstract int RequiredLevel { get; }
        public abstract bool IsPassive { get; }

        public virtual TimeSpan Cooldown
        {
            get { return TimeSpan.Zero; }
        }

        public virtual bool CanUse(PlayerMobile pm)
        {
            if (pm == null)
                return false;

            if (!pm.HasActiveAscension)
                return false;

            if (pm.ActiveAscension != Ascension)
                return false;

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);

            if (prog == null || prog.Level < RequiredLevel)
                return false;

            return true;
        }

        public abstract void Execute(PlayerMobile pm);

        public virtual void OnPassiveEvent(PlayerMobile pm, object context)
        {
        }
    }
}
