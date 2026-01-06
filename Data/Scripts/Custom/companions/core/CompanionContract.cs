using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.ContextMenus;
using Server.Items;
using Server.Companions.Data;
using Server.Companions.Systems;

namespace Server.Companions.Core
{
    public class CompanionContract : Item
    {
        private Mobile m_Owner;

        // Stored companion data (no live mobile reference)
        private CompanionClass m_CompanionClass;
        private OrderAxis m_OrderAxis;
        private MoralAxis m_MoralAxis;
        private int m_Level;
        private long m_Experience;
        private bool m_IsUnique;
        private string m_CompanionName;

        // Stored stats
        private int m_BaseStrength;
        private int m_BaseDexterity;
        private int m_BaseIntelligence;

        // Stored appearance
        private bool m_Female;
        private int m_BodyValue;
        private int m_SpeechHue;
        private int m_Hue;
        private int m_HairItemID;
        private int m_HairHue;
        private int m_FacialHairItemID;
        private int m_FacialHairHue;

        private int m_CompanionGearTier;
        private int m_CompanionWeaponID;
        private int m_CompanionShieldID;
        private int m_CompanionHelmID;
        private int m_CompanionArmorType;
        private int m_CompanionWeaponType;
        private int m_CompanionCloak;
        private int m_CompanionCloakColor;
        private int m_CompanionGearColor;


        // Active companion reference (only when summoned)
        private Serial m_ActiveCompanionSerial;

        private TimeSpan m_RemainingTime;
        private DateTime m_LastTickTime;
        private bool m_IsPaused;
        private bool m_IsDead;
        private CompanionTimerHelper m_Timer;

        private int m_LastTooltipMinute = -1;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CompanionClass CompanionClass
        {
            get { return m_CompanionClass; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CompanionLevel
        {
            get { return m_Level; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan RemainingTime
        {
            get { return m_RemainingTime; }
            set { m_RemainingTime = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsPaused
        {
            get { return m_IsPaused; }
            set { m_IsPaused = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsDead
        {
            get { return m_IsDead; }
            set { m_IsDead = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsActive
        {
            get
            {
                if (m_ActiveCompanionSerial == Serial.MinusOne)
                    return false;

                Mobile mob = World.FindMobile(m_ActiveCompanionSerial);
                return mob != null && !mob.Deleted;
            }
        }

        public OrderAxis OrderAxis
        {
            get { return m_OrderAxis; }
        }
        
        public MoralAxis MoralAxis
        {
            get { return m_MoralAxis; }
        }

        public int CompanionGearTier { get { return m_CompanionGearTier; } set { m_CompanionGearTier = value; } }
        public int CompanionWeaponID { get { return m_CompanionWeaponID; } set { m_CompanionWeaponID = value; } }
        public int CompanionShieldID { get { return m_CompanionShieldID; } set { m_CompanionShieldID = value; } }
        public int CompanionHelmID { get { return m_CompanionHelmID; } set { m_CompanionHelmID = value; } }
        public int CompanionArmorType { get { return m_CompanionArmorType; } set { m_CompanionArmorType = value; } }
        public int CompanionWeaponType { get { return m_CompanionWeaponType; } set { m_CompanionWeaponType = value; } }
        public int CompanionCloak { get { return m_CompanionCloak; } set { m_CompanionCloak = value; } }
        public int CompanionCloakColor { get { return m_CompanionCloakColor; } set { m_CompanionCloakColor = value; } }
        public int CompanionGearColor { get { return m_CompanionGearColor; } set { m_CompanionGearColor = value; } }


        [Constructable]
        public CompanionContract() : base(0x14F0)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
            Movable = true;
            Name = "Companion Contract";

            m_ActiveCompanionSerial = Serial.MinusOne;
            m_RemainingTime = TimeSpan.FromHours(6);
            m_LastTickTime = DateTime.UtcNow;
            m_IsPaused = true;
            m_IsDead = false;
            m_Level = 1;
            m_Experience = 0;

            m_Timer = new CompanionTimerHelper(this);
        }

        public CompanionContract(Serial serial) : base(serial)
        {
        }

        public void InitializeCompanion(Mobile owner, CompanionClass companionClass, CompanionAlignment alignment, bool isUnique, string customName)
        {
            m_Owner = owner;
            m_CompanionClass = companionClass;
            m_OrderAxis = alignment.Order;
            m_MoralAxis = alignment.Moral;
            m_IsUnique = isUnique;
            m_Level = 1;
            m_Experience = 0;
            m_ActiveCompanionSerial = Serial.MinusOne;

            CompanionDefinition def = CompanionDefinition.Get(companionClass);
            if (def != null)
            {
                m_BaseStrength = def.BaseStr;
                m_BaseDexterity = def.BaseDex;
                m_BaseIntelligence = def.BaseInt;
            }

            // Generate appearance
            m_SpeechHue = Utility.RandomTalkHue();
            m_Hue = Utility.RandomSkinHue();
            m_Female = Utility.RandomBool();

            if (m_Female)
            {
                m_BodyValue = 0x191;
                if (string.IsNullOrEmpty(customName))
                    m_CompanionName = NameList.RandomName("female");
                else
                    m_CompanionName = customName;

                m_HairItemID = Utility.RandomList(0x203B, 0x203C, 0x203D, 0x2045, 0x204A, 0x2046, 0x2049);
                m_HairHue = Utility.RandomHairHue();
                m_FacialHairItemID = 0;
                m_FacialHairHue = 0;
            }
            else
            {
                m_BodyValue = 0x190;
                if (string.IsNullOrEmpty(customName))
                    m_CompanionName = NameList.RandomName("male");
                else
                    m_CompanionName = customName;

                m_HairItemID = Utility.RandomList(0x203B, 0x203C, 0x203D, 0x2044, 0x2045, 0x2047, 0x2048);
                m_HairHue = Utility.RandomHairHue();
                m_FacialHairItemID = Utility.RandomList(0, 8254, 8255, 8256, 8257, 8267, 8268, 8269);
                m_FacialHairHue = m_HairHue;
            }

            Name = m_CompanionName + " the " + companionClass.ToString() + " Contract";

            m_Timer = new CompanionTimerHelper(this);
        }

        public CompanionMobile GetCompanion()
        {
            if (m_ActiveCompanionSerial == Serial.MinusOne)
                return null;

            Mobile mob = World.FindMobile(m_ActiveCompanionSerial);
            return mob as CompanionMobile;
        }

        public bool CanSummon(Mobile from)
        {
            int followersSlots = from.FollowersMax - from.Followers;
            if (followersSlots < 1)
            {
                from.SendMessage("You already have enough followers in your group.");
                return false;
            }
            if (from != m_Owner)
            {
                from.SendMessage("This contract does not belong to you.");
                return false;
            }

            if (m_CompanionClass == 0)
            {
                from.SendMessage("This contract has no companion data.");
                return false;
            }

            if (m_IsDead)
            {
                from.SendMessage("Your companion must be resurrected before you can summon them.");
                return false;
            }

            if (IsActive)
            {
                from.SendMessage("Your companion is already active.");
                return false;
            }

            if (m_RemainingTime <= TimeSpan.Zero)
            {
                from.SendMessage("Your companion demands payment before returning to service.");
                return false;
            }

            if (!CompanionTimerHelper.IsInSafeZone(from))
            {
                from.SendMessage("You must be in a safe zone (house, inn, or bank) to summon your companion.");
                return false;
            }

            string message;
            if (!CompanionSpawner.ValidateSummon(from, out message))
            {
                from.SendMessage(message);
                return false;
            }

            if (CompanionSpawner.HasActiveCompanionOfClass(from, m_CompanionClass))
            {
                from.SendMessage("You already have a companion of this class");
                return false;
            }

            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }

            if (CanSummon(from))
            {
                SummonCompanion();
            }
        }

        public void SummonCompanion()
        {
            if (m_Owner == null)
                return;

            // Create new companion from stored data
            CompanionAlignment alignment = new CompanionAlignment(m_OrderAxis, m_MoralAxis);
            CompanionMobile companion = new CompanionMobile(m_CompanionClass, alignment, m_Owner);

            // Restore companion state
            companion.Level = m_Level;
            companion.Experience = m_Experience;
            companion.IsUnique = m_IsUnique;
            companion.CompanionName = m_CompanionName;
            companion.Name = m_CompanionName;
            companion.Title = "the " + m_CompanionClass.ToString();
            companion.ContractSerial = this.Serial;

            // Restore stats
            companion.SetBaseStrength(m_BaseStrength);
            companion.SetBaseDexterity(m_BaseDexterity);
            companion.SetBaseIntelligence(m_BaseIntelligence);

            // Restore appearance
            companion.Female = m_Female;
            companion.Body = m_BodyValue;
            companion.SpeechHue = m_SpeechHue;
            companion.Hue = m_Hue;
            companion.HairItemID = m_HairItemID;
            companion.HairHue = m_HairHue;
            companion.FacialHairItemID = m_FacialHairItemID;
            companion.FacialHairHue = m_FacialHairHue;

            // Update all stats/skills/resists based on current level
            companion.UpdateFromContract();
            // suit'em up
            CompanionEquipment.ApplyEquipment(companion, this);


            // Place in world
            companion.MoveToWorld(m_Owner.Location, m_Owner.Map);
            companion.ControlMaster = m_Owner;
            companion.Controlled = true;

            // Set to follow mode automatically
            companion.ControlTarget = m_Owner;
            companion.ControlOrder = OrderType.Follow;
            companion.ControlSlots = 1;
            companion.Loyalty = 100;

            m_ActiveCompanionSerial = companion.Serial;
            m_IsPaused = false;
            m_LastTickTime = DateTime.UtcNow;
            m_Timer.ClearWarnings();

            m_Owner.SendMessage("You summon " + companion.Name + "!");

            Visible = false;
        }

        public void DismissCompanion(bool timerExpired)
        {
            CompanionMobile companion = GetCompanion();
            if (companion == null || companion.Deleted)
            {
                m_ActiveCompanionSerial = Serial.MinusOne;
                Visible = true;
                m_IsPaused = true;
                return;
            }

            // Store current state before deleting
            StoreCompanionState(companion);

            Visible = true;

            companion.Delete();
            m_ActiveCompanionSerial = Serial.MinusOne;

            m_IsPaused = true;

            if (!timerExpired && m_Owner != null)
                m_Owner.SendMessage(companion.Name + " returns to their contract.");
        }

        public void OnCompanionDeath()
        {
            CompanionMobile companion = GetCompanion();

            if (companion != null)
                StoreCompanionState(companion);

            m_IsDead = true;
            m_IsPaused = true;
            m_ActiveCompanionSerial = Serial.MinusOne;
            Visible = true;

            if (m_Owner != null)
                m_Owner.SendMessage(m_CompanionName + " has fallen! Take the contract to a healer for resurrection.");
        }

        public void OnCompanionGainedExperience(long newExperience)
        {
            m_Experience = newExperience;
        }

        public void OnCompanionLevelUp(int newLevel, int newStr, int newDex, int newInt)
        {
            m_Level = newLevel;
            m_BaseStrength = newStr;
            m_BaseDexterity = newDex;
            m_BaseIntelligence = newInt;
        }

        private void StoreCompanionState(CompanionMobile companion)
        {
            if (companion == null)
                return;

            m_Level = companion.Level;
            m_Experience = companion.Experience;
            m_BaseStrength = companion.GetBaseStrength();
            m_BaseDexterity = companion.GetBaseDexterity();
            m_BaseIntelligence = companion.GetBaseIntelligence();
        }

        public bool Resurrect(Mobile healer)
        {
            if (!m_IsDead)
                return false;

            int cost = GetResurrectionCost();

            if (m_Owner == null)
                return false;

            Container bank = m_Owner.BankBox;
            Container pack = m_Owner.Backpack;

            bool hasMoney = false;
            if (bank != null && bank.ConsumeTotal(typeof(Gold), cost))
            {
                hasMoney = true;
            }
            else if (pack != null && pack.ConsumeTotal(typeof(Gold), cost))
            {
                hasMoney = true;
            }

            if (!hasMoney)
            {
                m_Owner.SendMessage("You need " + cost.ToString() + " gold to resurrect " + m_CompanionName + ".");
                return false;
            }

            m_IsDead = false;

            m_Owner.SendMessage(m_CompanionName + " has been resurrected for " + cost.ToString() + " gold.");

            return true;
        }

        public int GetResurrectionCost()
        {
            return 30 * m_Level;
        }

        public bool AddGold(int amount)
        {
            if (amount <= 0)
                return false;

            int goldUsed;
            bool success = m_Timer.AddTime(amount, out goldUsed);

            int refund = amount - goldUsed;

            if (m_Owner != null)
            {
                if (goldUsed > 0)
                {
                    double hoursAdded = (double)goldUsed / m_Timer.GoldPerHour;
                    m_Owner.SendMessage(
                        m_CompanionName + " shall adventure with you for " +
                        hoursAdded.ToString("F1") + " more hours."
                    );
                }

                if (refund > 0)
                {
                    m_Owner.AddToBackpack(new Gold(refund));
                    m_Owner.SendMessage("You receive " + refund + " gold back.");
                }

                if (!success && goldUsed == 0)
                {
                    m_Owner.SendMessage(m_CompanionName + " cannot carry any more treasure.");
                }
            }

            return goldUsed > 0;
        }

        public void UpdateTooltipTime()
        {
            int currentMinute = (int)m_RemainingTime.TotalMinutes;
        
            if (currentMinute != m_LastTooltipMinute)
            {
                m_LastTooltipMinute = currentMinute;
                InvalidateProperties();
            }
        }



        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is Gold)
            {
                Gold gold = (Gold)dropped;

                if (from != m_Owner)
                {
                    from.SendMessage("This is not your companion's contract.");
                    return false;
                }

                AddGold(gold.Amount);
                gold.Delete();
                return true;
            }

            return base.OnDragDrop(from, dropped);
        }

        public void Tick()
        {
            if (!IsActive)
                return;

            if (CompanionTimerHelper.IsInSafeZone(m_Owner))
            {
                if (!m_IsPaused)
                {
                    m_IsPaused = true;
                    if (m_Owner != null)
                        m_Owner.SendMessage("Your companion's timer is paused while in a safe zone.");
                }
                return;
            }

            if (m_IsPaused)
            {
                m_IsPaused = false;
                m_LastTickTime = DateTime.UtcNow;
                return;
            }

            DateTime now = DateTime.UtcNow;
            TimeSpan elapsed = now - m_LastTickTime;
            m_LastTickTime = now;

            m_Timer.Tick(elapsed);
        }

        public void RecalculateTimerCost()
        {
            if (m_Owner != null)
            {
                m_Owner.SendMessage(m_CompanionName + " now requires " + m_Timer.GoldPerHour.ToString() + " gold per hour.");
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from == m_Owner)
            {
                list.Add(new ExamineEntry(this));

                if (IsActive)
                    list.Add(new DismissEntry(this));
            }
        }

        private class ExamineEntry : ContextMenuEntry
        {
            private CompanionContract m_Contract;

            public ExamineEntry(CompanionContract contract) : base(6104, 3)
            {
                m_Contract = contract;
            }

            public override void OnClick()
            {
                if (m_Contract.m_Owner != null)
                {
                    m_Contract.m_Owner.SendMessage("Character sheet gump not yet implemented.");
                }
            }
        }

        private class DismissEntry : ContextMenuEntry
        {
            private CompanionContract m_Contract;

            public DismissEntry(CompanionContract contract) : base(6108, 3)
            {
                m_Contract = contract;
            }

            public override void OnClick()
            {
                m_Contract.DismissCompanion(false);
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add("Level " + m_Level.ToString() + " " + m_CompanionClass.ToString());
            list.Add("Experience: " + m_Experience.ToString());

            if (m_Level < 20)
            {
                long remaining = CompanionExperience.GetExperienceToNextLevel(m_Level, m_Experience);
                list.Add("Next Level: " + remaining.ToString() + " exp");
            }

            list.Add("Time Remaining: " + m_RemainingTime.Hours.ToString() + "h " + m_RemainingTime.Minutes.ToString() + "m");

            if (m_IsDead)
                list.Add(1061169, GetResurrectionCost().ToString());

            if (m_Owner != null)
                list.Add("Owner: " + m_Owner.Name);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write(m_Owner);
            writer.Write((int)m_CompanionClass);
            writer.Write((int)m_OrderAxis);
            writer.Write((int)m_MoralAxis);
            writer.Write(m_Level);
            writer.Write(m_Experience);
            writer.Write(m_IsUnique);
            writer.Write(m_CompanionName);

            writer.Write(m_BaseStrength);
            writer.Write(m_BaseDexterity);
            writer.Write(m_BaseIntelligence);

            writer.Write(m_Female);
            writer.Write(m_BodyValue);
            writer.Write(m_SpeechHue);
            writer.Write(m_Hue);
            writer.Write(m_HairItemID);
            writer.Write(m_HairHue);
            writer.Write(m_FacialHairItemID);
            writer.Write(m_FacialHairHue);

            writer.Write((int)m_ActiveCompanionSerial);
            writer.Write(m_RemainingTime);
            writer.Write(m_LastTickTime);
            writer.Write(m_IsPaused);
            writer.Write(m_IsDead);

            writer.Write(m_CompanionGearTier);
            writer.Write(m_CompanionWeaponID);
            writer.Write(m_CompanionShieldID);
            writer.Write(m_CompanionHelmID);
            writer.Write(m_CompanionArmorType);
            writer.Write(m_CompanionWeaponType);
            writer.Write(m_CompanionCloak);
            writer.Write(m_CompanionCloakColor);
            writer.Write(m_CompanionGearColor);

            m_Timer.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Owner = reader.ReadMobile();
            m_CompanionClass = (CompanionClass)reader.ReadInt();
            m_OrderAxis = (OrderAxis)reader.ReadInt();
            m_MoralAxis = (MoralAxis)reader.ReadInt();
            m_Level = reader.ReadInt();
            m_Experience = reader.ReadLong();
            m_IsUnique = reader.ReadBool();
            m_CompanionName = reader.ReadString();

            m_BaseStrength = reader.ReadInt();
            m_BaseDexterity = reader.ReadInt();
            m_BaseIntelligence = reader.ReadInt();

            m_Female = reader.ReadBool();
            m_BodyValue = reader.ReadInt();
            m_SpeechHue = reader.ReadInt();
            m_Hue = reader.ReadInt();
            m_HairItemID = reader.ReadInt();
            m_HairHue = reader.ReadInt();
            m_FacialHairItemID = reader.ReadInt();
            m_FacialHairHue = reader.ReadInt();

            m_ActiveCompanionSerial = (Serial)reader.ReadInt();
            m_RemainingTime = reader.ReadTimeSpan();
            m_LastTickTime = reader.ReadDateTime();
            m_IsPaused = reader.ReadBool();
            m_IsDead = reader.ReadBool();

            m_CompanionGearTier = reader.ReadInt();
            m_CompanionWeaponID = reader.ReadInt();
            m_CompanionShieldID = reader.ReadInt();
            m_CompanionHelmID = reader.ReadInt();
            m_CompanionArmorType = reader.ReadInt();
            m_CompanionWeaponType = reader.ReadInt();
            m_CompanionCloak = reader.ReadInt();
            m_CompanionCloakColor = reader.ReadInt();
            m_CompanionGearColor = reader.ReadInt();

            m_Timer = new CompanionTimerHelper(this);
            m_Timer.Deserialize(reader);
        }
    }
}