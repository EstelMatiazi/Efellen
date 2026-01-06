using System;
using System.Collections.Generic;
using Server.Companions.Data;
using Server.Mobiles;
using Server.Regions;

namespace Server.Companions.Core
{
    public class CompanionTimerHelper
    {
        private CompanionContract m_Contract;
        private const int GoldPerLevelPerHour = 60;
        private static readonly TimeSpan MaxDuration = TimeSpan.FromHours(6);
        private static readonly TimeSpan WarningThreshold1 = TimeSpan.FromHours(2);
        private static readonly TimeSpan WarningThreshold2 = TimeSpan.FromHours(1);
        private static readonly TimeSpan WarningThreshold3 = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan WarningThreshold4 = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan FinalWarning = TimeSpan.FromMinutes(5);

        // Track warnings with List instead of HashSet
        private List<double> m_WarnedThresholdHours;

        public CompanionTimerHelper(CompanionContract contract)
        {
            m_Contract = contract;
            m_WarnedThresholdHours = new List<double>();
        }

        public int GoldPerHour
        {
            get
            {
                CompanionMobile companion = m_Contract.GetCompanion();
                if (companion == null)
                    return GoldPerLevelPerHour;

                return GoldPerLevelPerHour * companion.Level;
            }
        }

        public TimeSpan RemainingTime
        {
            get { return m_Contract.RemainingTime; }
        }

        public bool IsExpired
        {
            get { return RemainingTime <= TimeSpan.Zero; }
        }

        public void Tick(TimeSpan elapsed)
        {
            if (m_Contract.IsPaused)
                return;

            CompanionMobile companion = m_Contract.GetCompanion();
            if (companion == null)
                return;

            m_Contract.RemainingTime -= elapsed;

            if (m_Contract.RemainingTime < TimeSpan.Zero)
                m_Contract.RemainingTime = TimeSpan.Zero;

            m_Contract.UpdateTooltipTime();
            
            string message;
            if (ShouldWarn(out message))
            {
                if (!companion.Deleted)
                    companion.Say(message);
            }

            if (IsExpired)
            {
                OnExpired();
            }
        }

        public bool AddTime(int goldAmount, out int goldUsed)
        {
            goldUsed = 0;

            if (goldAmount <= 0)
                return false;

            int goldPerHour = GoldPerHour;
            if (goldPerHour <= 0)
                return false;

            double hoursAvailable = (MaxDuration - m_Contract.RemainingTime).TotalHours;

            if (hoursAvailable <= 0)
            {
                goldUsed = 0;
                return false;
            }

            double hoursPaid = (double)goldAmount / goldPerHour;
            double hoursAdded = Math.Min(hoursPaid, hoursAvailable);

            TimeSpan timeAdded = TimeSpan.FromHours(hoursAdded);
            m_Contract.RemainingTime += timeAdded;

            goldUsed = (int)Math.Ceiling(hoursAdded * goldPerHour);

            return true;
        }

        public bool AddTime(int goldAmount)
        {
            int dummy;
            return AddTime(goldAmount, out dummy);
        }



        public int GetGoldToFill()
        {
            TimeSpan remaining = MaxDuration - m_Contract.RemainingTime;
            return (int)Math.Ceiling(remaining.TotalHours * GoldPerHour);
        }

        public void ClearWarnings()
        {
            m_WarnedThresholdHours.Clear();
        }

        private bool HasWarnedAt(double hours)
        {
            for (int i = 0; i < m_WarnedThresholdHours.Count; i++)
            {
                if (Math.Abs(m_WarnedThresholdHours[i] - hours) < 0.01)
                    return true;
            }
            return false;
        }

        private void MarkWarnedAt(double hours)
        {
            if (!HasWarnedAt(hours))
                m_WarnedThresholdHours.Add(hours);
        }

        private bool ShouldWarn(out string message)
        {
            message = null;

            CompanionMobile companion = m_Contract.GetCompanion();
            if (companion == null)
                return false;

            TimeSpan remaining = RemainingTime;
            CompanionAlignment alignment = companion.Alignment;

            if (remaining <= FinalWarning && !HasWarnedAt(FinalWarning.TotalHours))
            {
                message = GetWarningMessage(alignment, WarningLevel.Final);
                MarkWarnedAt(FinalWarning.TotalHours);
                return true;
            }
            else if (remaining <= WarningThreshold4 && !HasWarnedAt(WarningThreshold4.TotalHours))
            {
                message = GetWarningMessage(alignment, WarningLevel.Urgent);
                MarkWarnedAt(WarningThreshold4.TotalHours);
                return true;
            }
            else if (remaining <= WarningThreshold3 && !HasWarnedAt(WarningThreshold3.TotalHours))
            {
                message = GetWarningMessage(alignment, WarningLevel.Concerned);
                MarkWarnedAt(WarningThreshold3.TotalHours);
                return true;
            }
            else if (remaining <= WarningThreshold2 && !HasWarnedAt(WarningThreshold2.TotalHours))
            {
                message = GetWarningMessage(alignment, WarningLevel.Reminder);
                MarkWarnedAt(WarningThreshold2.TotalHours);
                return true;
            }
            else if (remaining <= WarningThreshold1 && !HasWarnedAt(WarningThreshold1.TotalHours))
            {
                message = GetWarningMessage(alignment, WarningLevel.Casual);
                MarkWarnedAt(WarningThreshold1.TotalHours);
                return true;
            }

            return false;
        }

        private enum WarningLevel
        {
            Casual,
            Reminder,
            Concerned,
            Urgent,
            Final
        }

        private string GetWarningMessage(CompanionAlignment alignment, WarningLevel level)
        {
            string baseMessage;
            
            switch (level)
            {
                case WarningLevel.Casual:
                    baseMessage = "I could use some treasure soon.";
                    break;
                case WarningLevel.Reminder:
                    baseMessage = "Don't forget about my share of the treasure.";
                    break;
                case WarningLevel.Concerned:
                    baseMessage = "My patience for unpaid work is wearing thin.";
                    break;
                case WarningLevel.Urgent:
                    baseMessage = "I need my payment NOW!";
                    break;
                case WarningLevel.Final:
                    baseMessage = "This is your last warning - pay me or I'm leaving!";
                    break;
                default:
                    baseMessage = "I need treasure.";
                    break;
            }

            if (alignment.GetIsLawful() && level == WarningLevel.Final)
                return "Our contract is about to expire. Honor your obligations!";
            if (alignment.GetIsChaotic() && level == WarningLevel.Urgent)
                return "Oi! Where's my gold?! I'm not doing this for free!";
            if (alignment.GetIsGood() && level == WarningLevel.Casual)
                return "Friend, I hate to mention it, but I do need my share soon.";
            if (alignment.GetIsEvil() && level == WarningLevel.Final)
                return "You've wasted my time long enough. Goodbye, fool.";

            return baseMessage;
        }

        private string GetMaxTimeMessage(CompanionAlignment alignment)
        {
            if (alignment.GetIsLawful())
                return "I cannot accept more than our contract allows.";
            if (alignment.GetIsChaotic())
                return "Whoa, that's more than I can carry!";
            if (alignment.GetIsGood())
                return "Thank you, but I can't accept any more treasure right now.";

            return "I can't carry any more treasure.";
        }

        private void OnExpired()
        {
            CompanionMobile companion = m_Contract.GetCompanion();
            if (companion == null || companion.Deleted)
                return;

            string farewellMessage = GetFarewellMessage(companion.Alignment);
            companion.Say(farewellMessage);

            m_Contract.DismissCompanion(true);
        }

        private string GetFarewellMessage(CompanionAlignment alignment)
        {
            if (alignment.GetIsLawfulGood())
                return "I'm disappointed in your lack of honor. I must take my leave.";
            if (alignment.GetIsLawful())
                return "Our contract has expired. Farewell.";
            if (alignment.GetIsChaotic() && alignment.GetIsGood())
                return "Sorry friend, but I've got bills to pay. Maybe next time!";
            if (alignment.GetIsChaotic())
                return "I'm outta here! Find yourself another sucker!";
            if (alignment.GetIsGood())
                return "I'm sorry, but I can't continue without proper compensation.";
            if (alignment.GetIsEvil())
                return "You're not worth my time. Pathetic.";

            return "My time is up. Goodbye.";
        }

        public static bool IsInSafeZone(Mobile owner)
        {
            if (owner == null || owner.Region == null)
                return false;
            bool house = false;
			if ( owner.Region is HouseRegion )
			    if (((HouseRegion)owner.Region).House.IsOwner(owner))
					house = true;
			
			if ( owner.Region.GetLogoutDelay( owner ) == TimeSpan.Zero || house ) 
				return true;

			return false;           
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(m_WarnedThresholdHours.Count);
            for (int i = 0; i < m_WarnedThresholdHours.Count; i++)
            {
                writer.Write(m_WarnedThresholdHours[i]);
            }
        }

        public void Deserialize(GenericReader reader)
        {
            m_WarnedThresholdHours = new List<double>();
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                m_WarnedThresholdHours.Add(reader.ReadDouble());
            }
        }
    }
}