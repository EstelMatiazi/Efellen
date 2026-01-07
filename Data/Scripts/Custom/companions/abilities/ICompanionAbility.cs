using System;
using Server;
using Server.Companions.Core;

namespace Server.Companions.Abilities
{
    public interface ICompanionAbility
    {
        string GetName();
        string GetDescription();
        int GetRequiredLevel();
        TimeSpan GetCooldown();
        DateTime GetLastUsed();
        void SetLastUsed(DateTime value);

        bool IsMartialSpecial { get; }

        bool CanUse(CompanionMobile companion);
        void Use(CompanionMobile companion, Mobile target);
    }

    public abstract class BaseCompanionAbility : ICompanionAbility
    {
        private DateTime m_LastUsed;

        public virtual bool IsMartialSpecial
        {
            get { return false; }
        }

        public abstract string GetName();
        public abstract string GetDescription();
        public abstract int GetRequiredLevel();
        public abstract TimeSpan GetCooldown();

        public DateTime GetLastUsed()
        {
            return m_LastUsed;
        }

        public void SetLastUsed(DateTime value)
        {
            m_LastUsed = value;
        }

        public BaseCompanionAbility()
        {
            m_LastUsed = DateTime.MinValue;
        }

        public virtual bool CanUse(CompanionMobile companion)
        {
            if (companion == null || companion.Deleted)
                return false;

            if (companion.Level < GetRequiredLevel())
                return false;

            if (DateTime.UtcNow < m_LastUsed + GetCooldown())
                return false;

            return true;
        }

        public abstract void Use(CompanionMobile companion, Mobile target);

        protected bool CheckCooldown()
        {
            if (DateTime.UtcNow < m_LastUsed + GetCooldown())
                return false;

            m_LastUsed = DateTime.UtcNow;
            return true;
        }
    }
}
