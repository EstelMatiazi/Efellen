using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Companions.Data;
using Server.Companions.Abilities;

namespace Server.Companions.Core
{
    public class CompanionMobile : BaseCreature
    {
        private CompanionClass m_Class;
        private OrderAxis m_OrderAxis;
        private MoralAxis m_MoralAxis;
        private int m_Level;
        private long m_Experience;
        private Mobile m_Owner;
        private bool m_IsUnique;
        private string m_CompanionName;

        private int m_BaseStrength;
        private int m_BaseDexterity;
        private int m_BaseIntelligence;

        private Serial m_ContractSerial;

        private AbilityManager m_Abilities;

        [CommandProperty(AccessLevel.GameMaster)]
        public CompanionClass Class
        {
            get { return m_Class; }
            set { m_Class = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CompanionAlignment Alignment
        {
            get { return new CompanionAlignment(m_OrderAxis, m_MoralAxis); }
            set
            {
                m_OrderAxis = value.Order;
                m_MoralAxis = value.Moral;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Level
        {
            get { return m_Level; }
            set
            {
                if (m_Level != value)
                {
                    m_Level = Math.Max(1, Math.Min(20, value));
                    OnLevelChanged();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public long Experience
        {
            get { return m_Experience; }
            set
            {
                m_Experience = Math.Max(0, value);
                CheckLevelUp();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsUnique
        {
            get { return m_IsUnique; }
            set { m_IsUnique = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string CompanionName
        {
            get { return m_CompanionName; }
            set { m_CompanionName = value; }
        }

        public AbilityManager Abilities
        {
            get { return m_Abilities; }
        }

        public Serial ContractSerial
        {
            get { return m_ContractSerial; }
            set { m_ContractSerial = value; }
        }

        [Constructable]
        public CompanionMobile() : this(CompanionClass.Fighter, CompanionAlignment.GetTrueNeutral(), null)
        {
        }

        public CompanionMobile(Serial serial) : base(serial)
        {
        }

        public CompanionMobile(CompanionClass companionClass, CompanionAlignment alignment, Mobile owner)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            m_Class = companionClass;
            m_OrderAxis = alignment.Order;
            m_MoralAxis = alignment.Moral;
            m_Owner = owner;
            m_Level = 1;
            m_Experience = 0;
            m_IsUnique = false;
            m_ContractSerial = Serial.MinusOne;

            m_Abilities = new AbilityManager(this);

            CompanionDefinition def = CompanionDefinition.Get(companionClass);
            if (def != null)
            {
                m_BaseStrength = def.BaseStr;
                m_BaseDexterity = def.BaseDex;
                m_BaseIntelligence = def.BaseInt;
            }

            Controlled = true;
            ControlMaster = owner;
            ControlSlots = 1;

            if (owner != null)
            {
                Loyalty = MaxLoyalty;
            }

            m_Abilities.Initialize(m_Class);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (ContractSerial == Serial.MinusOne)
                return;

            CompanionContract contract =
                World.FindItem(ContractSerial) as CompanionContract;

            if (contract != null)
                contract.Tick();
        }


        public void UpdateFromContract()
        {
            UpdateAllStats();
            UpdateAllSkills();
            UpdateDamage();
            UpdateResists();
        }

        public void SetBaseStrength(int value)
        {
            m_BaseStrength = value;
        }

        public void SetBaseDexterity(int value)
        {
            m_BaseDexterity = value;
        }

        public void SetBaseIntelligence(int value)
        {
            m_BaseIntelligence = value;
        }

        public int GetBaseStrength()
        {
            return m_BaseStrength;
        }

        public int GetBaseDexterity()
        {
            return m_BaseDexterity;
        }

        public int GetBaseIntelligence()
        {
            return m_BaseIntelligence;
        }

        public void GainExperience(int amount)
        {
            if (amount <= 0 || m_Level >= 20)
                return;

            m_Experience += amount;

            CompanionContract contract = GetContract();
            if (contract != null)
                contract.OnCompanionGainedExperience(m_Experience);

            CheckLevelUp();
            InvalidateProperties();
        }

        private void CheckLevelUp()
        {
            while (CompanionExperience.ShouldLevelUp(m_Level, m_Experience) && m_Level < 20)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            m_Level++;

            if (m_Owner != null)
                m_Owner.SendMessage("Your companion " + Name + " has reached level " + m_Level.ToString() + "!");

            CompanionDefinition def = CompanionDefinition.Get(m_Class);
            if (def != null)
            {
                m_BaseStrength += CompanionStats.RollStatGain(def.StrPerLevel);
                m_BaseDexterity += CompanionStats.RollStatGain(def.DexPerLevel);
                m_BaseIntelligence += CompanionStats.RollStatGain(def.IntPerLevel);
            }

            UpdateAllStats();
            UpdateAllSkills();
            UpdateDamage();
            UpdateResists();

            m_Abilities.OnLevelUp(m_Level);

            UpdateEquipment();

            CompanionContract contract = GetContract();
            if (contract != null)
            {
                contract.OnCompanionLevelUp(m_Level, m_BaseStrength, m_BaseDexterity, m_BaseIntelligence);
                contract.RecalculateTimerCost();
            }

            PlayLevelUpEffect();
            PlayLevelUpSound();

            OnLevelChanged();
            InvalidateProperties();
        }

        private int GetLevelUpHue(CompanionAlignment alignment)
        {
             if (alignment.Order == OrderAxis.Lawful && alignment.Moral == MoralAxis.Good)
                return 0x47E; // gold-white

            if (alignment.Order == OrderAxis.Lawful && alignment.Moral == MoralAxis.Neutral)
                return 0x47E; // gold-white

            if (alignment.Order == OrderAxis.Lawful && alignment.Moral == MoralAxis.Evil)
               return 0x47E; // gold-white

            // Neutral
            if (alignment.Order == OrderAxis.Neutral && alignment.Moral == MoralAxis.Good)
                return 0x47E; // gold-white

            if (alignment.Order == OrderAxis.Neutral && alignment.Moral == MoralAxis.Neutral)
                return 0x47E; // gold-white

            if (alignment.Order == OrderAxis.Neutral && alignment.Moral == MoralAxis.Evil)
               return 0x47E; // gold-white

            // Chaotic
            if (alignment.Order == OrderAxis.Chaotic && alignment.Moral == MoralAxis.Good)
              return 0x47E; // gold-white

            if (alignment.Order == OrderAxis.Chaotic && alignment.Moral == MoralAxis.Neutral)
               return 0x47E; // gold-white

            if (alignment.Order == OrderAxis.Chaotic && alignment.Moral == MoralAxis.Evil)
               return 0x47E; // gold-white

            return 0;
        }

        private void PlayLevelUpEffect()
        {
            CompanionAlignment alignment = Alignment;
            int hue = GetLevelUpHue(alignment);

            Effects.SendTargetParticles(
                this,
                0x373A,      // PS particle effect
                35,          // speed
                90,          // duration
                5052,        // effect ID 
                hue,
                0,
                EffectLayer.Waist,
                0x100
            );

            Effects.SendLocationEffect(
                Location,
                Map,
                0x376A,      // ground ring
                20,
                hue
            );
        }

        private void PlayLevelUpSound()
        {
            PlaySound(0x1F2);
        }




        private void UpdateAllStats()
        {
            CompanionDefinition def = CompanionDefinition.Get(m_Class);
            if (def == null)
                return;

            SetStr(m_BaseStrength);
            SetDex(m_BaseDexterity);
            SetInt(m_BaseIntelligence);

            int maxHits = CompanionStats.CalculateHP(m_BaseStrength, def.HpCap);
            SetHits(maxHits);

            SetStam(CompanionStats.GetStamina(m_BaseDexterity));
            SetMana(CompanionStats.GetMana(m_BaseIntelligence));
        }

        private void UpdateAllSkills()
        {
            CompanionDefinition def = CompanionDefinition.Get(m_Class);
            if (def == null)
                return;

            for (int i = 0; i < def.FastSkills.Count; i++)
            {
                SkillName skill = def.FastSkills[i];
                double value = CompanionStats.GetFastSkillValue(m_Level);
                SetSkill(skill, value, value);
            }

            for (int i = 0; i < def.MediumSkills.Count; i++)
            {
                SkillName skill = def.MediumSkills[i];
                double value = CompanionStats.GetMediumSkillValue(m_Level);
                SetSkill(skill, value, value);
            }
        }

        private void UpdateEquipment()
        {
            // TODO: Implement equipment changes based on level
        }

        private void UpdateDamage()
        {
            CompanionDefinition def = CompanionDefinition.Get(m_Class);
            if (def == null)
                return;

            int minDmg, maxDmg;
            CompanionStats.GetDamageRange(m_Level, def.BaseDamageMin, def.BaseDamageMax, def.DamageLevelsPerPoint, out minDmg, out maxDmg);
            SetDamage(minDmg, maxDmg);
        }

        private void UpdateResists()
        {
            CompanionDefinition def = CompanionDefinition.Get(m_Class);
            if (def == null)
                return;

            SetResistance(ResistanceType.Physical, 30);
            SetResistance(ResistanceType.Fire, 30);
            SetResistance(ResistanceType.Cold, 30);
            SetResistance(ResistanceType.Poison, 30);
            SetResistance(ResistanceType.Energy, 30);

            for (int i = 0; i < def.GoodResists.Count; i++)
            {
                ResistanceType type = def.GoodResists[i];
                int value = CompanionStats.GetResistValue(m_Level, ResistQuality.Good);
                SetResistance(type, value);
            }

            for (int i = 0; i < def.MediumResists.Count; i++)
            {
                ResistanceType type = def.MediumResists[i];
                int value = CompanionStats.GetResistValue(m_Level, ResistQuality.Medium);
                SetResistance(type, value);
            }

            for (int i = 0; i < def.PoorResists.Count; i++)
            {
                ResistanceType type = def.PoorResists[i];
                int value = CompanionStats.GetResistValue(m_Level, ResistQuality.Poor);
                SetResistance(type, value);
            }
        }
        private void OnLevelChanged()
        {
            InvalidateProperties();
        }

        public CompanionContract GetContract()
        {
            if (m_ContractSerial == Serial.MinusOne)
                return null;

            Item item = World.FindItem(m_ContractSerial);
            return item as CompanionContract;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (from == null || dropped == null)
                return false;

            CompanionContract contract = GetContract();
            if (contract == null)
                return false;

            int goldValue = GetGoldValue(dropped);

            if (goldValue <= 0)
                return false;

            bool success = contract.AddGold(goldValue);

            if (success)
            {
                Say(GetPaymentAcceptedMessage(Alignment));
                dropped.Delete();
            }
            else
            {
                Say(GetPaymentRefusedMessage(Alignment));
            }


            return true;
        }

        private int GetGoldValue(Item dropped)
        {
            int nAmount = dropped.Amount;
            int nGold = 0;

            if (dropped is DDSilver)
                nGold = (int)Math.Floor(nAmount / 5.0);
            else if (dropped is DDCopper)
                nGold = (int)Math.Floor(nAmount / 10.0);
            else if (dropped is DDJewels)
                nGold = nAmount * 2;
            else if (dropped is DDXormite)
                nGold = nAmount * 3;
            else if (dropped is Crystals)
                nGold = nAmount * 5;
            else if (dropped is Gold)
                nGold = nAmount;
            else if (dropped is DDGemstones)
                nGold = nAmount * 2;
            else if (dropped is DDGoldNuggets)
                nGold = nAmount;

            return nGold;
        }

        private string GetPaymentAcceptedMessage(CompanionAlignment alignment)
        {
            if (alignment.GetIsLawful()  && alignment.GetIsGood())
                return "Thank you. A contract honored is a bond strengthened.";

            if (alignment.GetIsLawful() && alignment.GetIsNeutral())
                return "Payment acknowledged with gratitude.";

            if (alignment.GetIsLawful() && alignment.GetIsEvil())
                return "Good. Order is maintained… for now.";

            if (alignment.GetIsNeutral() && alignment.GetIsGood())
                return "I appreciate it. Let’s keep going.";

            if (alignment.GetIsNeutral() && alignment.GetIsEvil())
                return "Hmph. You’ll do… for now.";

            if (alignment.GetIsChaotic() && alignment.GetIsGood())
                return "Nice! Let’s make this worth it!";

            if (alignment.GetIsChaotic() && alignment.GetIsNeutral())
                return "Heh! That’ll keep me around!";

            if (alignment.GetIsChaotic() && alignment.GetIsEvil())
                return "Gold talks. You live… for now.";
            
            if (alignment.GetIsNeutral())
                return "Fair enough. Let’s continue.";

            return "Payment received.";
        }


        private string GetPaymentRefusedMessage(CompanionAlignment alignment)
        {
            // Lawful
            if (alignment.GetIsLawful()  && alignment.GetIsGood())
                return "I cannot accept more than our agreement allows.";

            if (alignment.GetIsLawful() && alignment.GetIsNeutral())
                return "The contract is at its limit.";

            if (alignment.GetIsLawful() && alignment.GetIsEvil())
                return "No excess. Waste nothing.";

            // Neutral
            if (alignment.GetIsNeutral() && alignment.GetIsGood())
                return "I’m set for now, but thank you.";

            if (alignment.GetIsNeutral() && alignment.GetIsEvil())
                return "Keep it. I don’t need it… yet.";

            // Chaotic
            if (alignment.GetIsChaotic() && alignment.GetIsGood())
                return "Whoa! I’m good for now!";

            if (alignment.GetIsChaotic() && alignment.GetIsNeutral())
                return "Easy there! No more!";

            if (alignment.GetIsChaotic() && alignment.GetIsEvil())
                return "I don’t need charity.";

            if (alignment.GetIsNeutral())
                return "That’s enough for now.";

            return "I can’t take any more.";
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            CompanionContract contract = GetContract();
            if (contract != null && !contract.Deleted)
            {
                contract.OnCompanionDeath();
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add("Level " + m_Level.ToString() + " " + m_Class.ToString());

            long nextLevelExp = CompanionExperience.GetExperienceForNextLevel(m_Level);
            double progress = CompanionExperience.GetLevelProgress(m_Level, m_Experience);
            int percent = (int)(progress * 100.0);
            if (m_Level >= 20 || nextLevelExp == long.MaxValue)
            {
                list.Add("Experience: " + m_Experience.ToString() + " (MAX)");
            }
            else
            {
                list.Add(
                "Experience: " +
                m_Experience +
                " / " +
                nextLevelExp +
                " (" + percent + "%)"
            );
            }


            if (m_Owner != null)
                list.Add("Party Leader: " + m_Owner.Name);

        }

        public override bool CanBeControlledBy(Mobile m)
        {
            return m == m_Owner || base.CanBeControlledBy(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write((int)m_Class);
            writer.Write((int)m_OrderAxis);
            writer.Write((int)m_MoralAxis);
            writer.Write(m_Level);
            writer.Write(m_Experience);
            writer.Write(m_Owner);
            writer.Write(m_IsUnique);
            writer.Write(m_CompanionName);

            writer.Write(m_BaseStrength);
            writer.Write(m_BaseDexterity);
            writer.Write(m_BaseIntelligence);

            writer.Write((int)m_ContractSerial);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Class = (CompanionClass)reader.ReadInt();
            m_OrderAxis = (OrderAxis)reader.ReadInt();
            m_MoralAxis = (MoralAxis)reader.ReadInt();
            m_Level = reader.ReadInt();
            m_Experience = reader.ReadLong();
            m_Owner = reader.ReadMobile();
            m_IsUnique = reader.ReadBool();
            m_CompanionName = reader.ReadString();

            m_BaseStrength = reader.ReadInt();
            m_BaseDexterity = reader.ReadInt();
            m_BaseIntelligence = reader.ReadInt();

            m_ContractSerial = (Serial)reader.ReadInt();

            m_Abilities = new AbilityManager(this);
            m_Abilities.Initialize(m_Class);
        }
    }
}