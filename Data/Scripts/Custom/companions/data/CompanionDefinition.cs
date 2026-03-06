using System;
using System.Collections.Generic;
using Server;

namespace Server.Companions.Data
{
    public class CompanionDefinition
    {
        private CompanionClass m_Class;
        private int m_BaseStr;
        private int m_BaseDex;
        private int m_BaseInt;
        private int m_StrPerLevel;
        private int m_DexPerLevel;
        private int m_IntPerLevel;
        private int m_HpCap;
        private List<SkillName> m_FastSkills;
        private List<SkillName> m_MediumSkills;
        private List<CompanionAlignment> m_AllowedAlignments;
        
        // Damage scaling
        private int m_BaseDamageMin;
        private int m_BaseDamageMax;
        private int m_DamageLevelsPerPoint;
        
        // Resists
        private List<ResistanceType> m_GoodResists;
        private List<ResistanceType> m_MediumResists;
        private List<ResistanceType> m_PoorResists;

        public CompanionClass Class { get { return m_Class; } set { m_Class = value; } }
        public int BaseStr { get { return m_BaseStr; } set { m_BaseStr = value; } }
        public int BaseDex { get { return m_BaseDex; } set { m_BaseDex = value; } }
        public int BaseInt { get { return m_BaseInt; } set { m_BaseInt = value; } }
        public int StrPerLevel { get { return m_StrPerLevel; } set { m_StrPerLevel = value; } }
        public int DexPerLevel { get { return m_DexPerLevel; } set { m_DexPerLevel = value; } }
        public int IntPerLevel { get { return m_IntPerLevel; } set { m_IntPerLevel = value; } }
        public int HpCap { get { return m_HpCap; } set { m_HpCap = value; } }
        public List<SkillName> FastSkills { get { return m_FastSkills; } }
        public List<SkillName> MediumSkills { get { return m_MediumSkills; } }
        public List<CompanionAlignment> AllowedAlignments { get { return m_AllowedAlignments; } }
        
        public int BaseDamageMin { get { return m_BaseDamageMin; } set { m_BaseDamageMin = value; } }
        public int BaseDamageMax { get { return m_BaseDamageMax; } set { m_BaseDamageMax = value; } }
        public int DamageLevelsPerPoint { get { return m_DamageLevelsPerPoint; } set { m_DamageLevelsPerPoint = value; } }
        
        public List<ResistanceType> GoodResists { get { return m_GoodResists; } }
        public List<ResistanceType> MediumResists { get { return m_MediumResists; } }
        public List<ResistanceType> PoorResists { get { return m_PoorResists; } }

        private static Dictionary<CompanionClass, CompanionDefinition> m_Definitions;

        public CompanionDefinition()
        {
            m_FastSkills = new List<SkillName>();
            m_MediumSkills = new List<SkillName>();
            m_AllowedAlignments = new List<CompanionAlignment>();
            m_GoodResists = new List<ResistanceType>();
            m_MediumResists = new List<ResistanceType>();
            m_PoorResists = new List<ResistanceType>();
        }

        public static void Initialize()
        {
            m_Definitions = new Dictionary<CompanionClass, CompanionDefinition>();

            CompanionDefinition wizard = new CompanionDefinition();
            wizard.Class = CompanionClass.Wizard;
            wizard.BaseStr = 30; wizard.BaseDex = 40; wizard.BaseInt = 80;
            wizard.StrPerLevel = 5; wizard.DexPerLevel = 5; wizard.IntPerLevel = 25;
            wizard.HpCap = 400;
            wizard.BaseDamageMin = 2; wizard.BaseDamageMax = 7; wizard.DamageLevelsPerPoint = 4;
            wizard.GoodResists.Add(ResistanceType.Energy);
            wizard.MediumResists.Add(ResistanceType.Fire);
            wizard.MediumResists.Add(ResistanceType.Cold);
            wizard.PoorResists.Add(ResistanceType.Poison);
            wizard.PoorResists.Add(ResistanceType.Physical);
            wizard.FastSkills.Add(SkillName.Magery);
            wizard.FastSkills.Add(SkillName.Psychology);
            wizard.FastSkills.Add(SkillName.Meditation);
            wizard.MediumSkills.Add(SkillName.MagicResist);
            wizard.MediumSkills.Add(SkillName.Inscribe);
            wizard.MediumSkills.Add(SkillName.Bludgeoning);
            wizard.AllowedAlignments.AddRange(GetAllAlignments());
            m_Definitions[CompanionClass.Wizard] = wizard;

            CompanionDefinition fighter = new CompanionDefinition();
            fighter.Class = CompanionClass.Fighter;
            fighter.BaseStr = 70; fighter.BaseDex = 50; fighter.BaseInt = 30;
            fighter.StrPerLevel = 15; fighter.DexPerLevel = 15; fighter.IntPerLevel = 5;
            fighter.HpCap = 1000;
            fighter.BaseDamageMin = 4; fighter.BaseDamageMax = 9; fighter.DamageLevelsPerPoint = 2;
            fighter.GoodResists.Add(ResistanceType.Physical);
            fighter.MediumResists.Add(ResistanceType.Fire);
            fighter.MediumResists.Add(ResistanceType.Energy);
            fighter.MediumResists.Add(ResistanceType.Cold);
            fighter.PoorResists.Add(ResistanceType.Poison);
            fighter.FastSkills.Add(SkillName.Swords);
            fighter.FastSkills.Add(SkillName.Bludgeoning);
            fighter.FastSkills.Add(SkillName.Tactics);
            fighter.FastSkills.Add(SkillName.Parry);
            fighter.MediumSkills.Add(SkillName.MagicResist);
            fighter.MediumSkills.Add(SkillName.Healing);
            fighter.MediumSkills.Add(SkillName.Anatomy);
            fighter.AllowedAlignments.AddRange(GetAllAlignments());
            m_Definitions[CompanionClass.Fighter] = fighter;

            CompanionDefinition druid = new CompanionDefinition();
            druid.Class = CompanionClass.Druid;
            druid.BaseStr = 50; druid.BaseDex = 40; druid.BaseInt = 60;
            druid.StrPerLevel = 10; druid.DexPerLevel = 10; druid.IntPerLevel = 15;
            druid.HpCap = 800;
            druid.BaseDamageMin = 3; druid.BaseDamageMax = 8; druid.DamageLevelsPerPoint = 3;
            druid.GoodResists.Add(ResistanceType.Poison);
            druid.MediumResists.Add(ResistanceType.Physical);
            druid.MediumResists.Add(ResistanceType.Energy);
            druid.PoorResists.Add(ResistanceType.Fire);
            druid.PoorResists.Add(ResistanceType.Cold);
            druid.FastSkills.Add(SkillName.Druidism);
            druid.FastSkills.Add(SkillName.Spiritualism);
            druid.FastSkills.Add(SkillName.Magery);
            druid.MediumSkills.Add(SkillName.Meditation);
            druid.MediumSkills.Add(SkillName.Healing);
            druid.MediumSkills.Add(SkillName.Bludgeoning);
            druid.MediumSkills.Add(SkillName.Swords);
            druid.MediumSkills.Add(SkillName.Tactics);
            druid.MediumSkills.Add(SkillName.Parry);
            druid.AllowedAlignments.AddRange(GetNeutralAlignments());
            m_Definitions[CompanionClass.Druid] = druid;

            CompanionDefinition rogue = new CompanionDefinition();
            rogue.Class = CompanionClass.Rogue;
            rogue.BaseStr = 40; rogue.BaseDex = 60; rogue.BaseInt = 50;
            rogue.StrPerLevel = 10; rogue.DexPerLevel = 15; rogue.IntPerLevel = 10;
            rogue.HpCap = 600;
            rogue.BaseDamageMin = 3; rogue.BaseDamageMax = 8; rogue.DamageLevelsPerPoint = 3;
            rogue.GoodResists.Add(ResistanceType.Fire);
            rogue.MediumResists.Add(ResistanceType.Physical);
            rogue.MediumResists.Add(ResistanceType.Poison);
            rogue.PoorResists.Add(ResistanceType.Cold);
            rogue.PoorResists.Add(ResistanceType.Energy);
            rogue.FastSkills.Add(SkillName.Fencing);
            rogue.FastSkills.Add(SkillName.Hiding);
            rogue.FastSkills.Add(SkillName.Searching);
            rogue.FastSkills.Add(SkillName.Stealth);
            rogue.MediumSkills.Add(SkillName.Lockpicking);
            rogue.MediumSkills.Add(SkillName.Tactics);
            rogue.MediumSkills.Add(SkillName.Healing);
            rogue.MediumSkills.Add(SkillName.Poisoning);
            rogue.AllowedAlignments.AddRange(GetAllAlignments());
            m_Definitions[CompanionClass.Rogue] = rogue;

            CompanionDefinition monk = new CompanionDefinition();
            monk.Class = CompanionClass.Monk;
            monk.BaseStr = 50; monk.BaseDex = 50; monk.BaseInt = 50;
            monk.StrPerLevel = 10; monk.DexPerLevel = 15; monk.IntPerLevel = 10;
            monk.HpCap = 800;
            monk.BaseDamageMin = 3; monk.BaseDamageMax = 8; monk.DamageLevelsPerPoint = 3;
            monk.MediumResists.Add(ResistanceType.Energy);
            monk.MediumResists.Add(ResistanceType.Poison);
            monk.MediumResists.Add(ResistanceType.Physical);
            monk.MediumResists.Add(ResistanceType.Fire);
            monk.MediumResists.Add(ResistanceType.Cold);
            monk.FastSkills.Add(SkillName.FistFighting);
            monk.FastSkills.Add(SkillName.Tactics);
            monk.FastSkills.Add(SkillName.Healing);
            monk.MediumSkills.Add(SkillName.Anatomy);
            monk.MediumSkills.Add(SkillName.MagicResist);
            monk.MediumSkills.Add(SkillName.Spiritualism);
            monk.AllowedAlignments.AddRange(GetLawfulAlignments());
            m_Definitions[CompanionClass.Monk] = monk;

            CompanionDefinition barbarian = new CompanionDefinition();
            barbarian.Class = CompanionClass.Barbarian;
            barbarian.BaseStr = 80; barbarian.BaseDex = 50; barbarian.BaseInt = 10;
            barbarian.StrPerLevel = 20; barbarian.DexPerLevel = 10; barbarian.IntPerLevel = 5;
            barbarian.HpCap = 1200;
            barbarian.BaseDamageMin = 4; barbarian.BaseDamageMax = 9; barbarian.DamageLevelsPerPoint = 2;
            barbarian.GoodResists.Add(ResistanceType.Physical);
            barbarian.MediumResists.Add(ResistanceType.Fire);
            barbarian.MediumResists.Add(ResistanceType.Poison);
            barbarian.MediumResists.Add(ResistanceType.Energy);
            barbarian.PoorResists.Add(ResistanceType.Cold);
            barbarian.FastSkills.Add(SkillName.Swords);
            barbarian.FastSkills.Add(SkillName.Tactics);
            barbarian.FastSkills.Add(SkillName.MagicResist);
            barbarian.MediumSkills.Add(SkillName.Healing);
            barbarian.MediumSkills.Add(SkillName.Anatomy);
            barbarian.AllowedAlignments.AddRange(GetChaoticAlignments());
            m_Definitions[CompanionClass.Barbarian] = barbarian;

            CompanionDefinition ranger = new CompanionDefinition();
            ranger.Class = CompanionClass.Ranger;
            ranger.BaseStr = 50; ranger.BaseDex = 70; ranger.BaseInt = 30;
            ranger.StrPerLevel = 10; ranger.DexPerLevel = 15; ranger.IntPerLevel = 10;
            ranger.HpCap = 800;
            ranger.BaseDamageMin = 4; ranger.BaseDamageMax = 9; ranger.DamageLevelsPerPoint = 2;
            ranger.GoodResists.Add(ResistanceType.Cold);
            ranger.GoodResists.Add(ResistanceType.Fire);
            ranger.MediumResists.Add(ResistanceType.Physical);
            ranger.MediumResists.Add(ResistanceType.Poison);
            ranger.PoorResists.Add(ResistanceType.Energy);
            ranger.FastSkills.Add(SkillName.Marksmanship);
            ranger.FastSkills.Add(SkillName.Tactics);
            ranger.FastSkills.Add(SkillName.Tracking);
            ranger.MediumSkills.Add(SkillName.Healing);
            ranger.MediumSkills.Add(SkillName.Anatomy);
            ranger.MediumSkills.Add(SkillName.MagicResist);
            ranger.MediumSkills.Add(SkillName.Searching);
            ranger.AllowedAlignments.AddRange(GetAllAlignments());
            m_Definitions[CompanionClass.Ranger] = ranger;

            CompanionDefinition paladin = new CompanionDefinition();
            paladin.Class = CompanionClass.Paladin;
            paladin.BaseStr = 70; paladin.BaseDex = 30; paladin.BaseInt = 50;
            paladin.StrPerLevel = 15; paladin.DexPerLevel = 5; paladin.IntPerLevel = 15;
            paladin.HpCap = 1000;
            paladin.BaseDamageMin = 4; paladin.BaseDamageMax = 9; paladin.DamageLevelsPerPoint = 2;
            paladin.GoodResists.Add(ResistanceType.Physical);
            paladin.MediumResists.Add(ResistanceType.Poison);
            paladin.MediumResists.Add(ResistanceType.Energy);
            paladin.MediumResists.Add(ResistanceType.Fire);
            paladin.MediumResists.Add(ResistanceType.Cold);
            paladin.FastSkills.Add(SkillName.Swords);
            paladin.FastSkills.Add(SkillName.Bludgeoning);
            paladin.FastSkills.Add(SkillName.Fencing);
            paladin.FastSkills.Add(SkillName.Tactics);
            paladin.FastSkills.Add(SkillName.Knightship);
            paladin.FastSkills.Add(SkillName.MagicResist);
            paladin.MediumSkills.Add(SkillName.Parry);
            paladin.MediumSkills.Add(SkillName.ArmsLore);
            paladin.AllowedAlignments.Add(CompanionAlignment.GetLawfulGood());
            m_Definitions[CompanionClass.Paladin] = paladin;

            CompanionDefinition cleric = new CompanionDefinition();
            cleric.Class = CompanionClass.Cleric;
            cleric.BaseStr = 55; cleric.BaseDex = 35; cleric.BaseInt = 60;
            cleric.StrPerLevel = 10; cleric.DexPerLevel = 5; cleric.IntPerLevel = 15;
            cleric.HpCap = 800;
            cleric.BaseDamageMin = 3; cleric.BaseDamageMax = 8; cleric.DamageLevelsPerPoint = 3;
            cleric.GoodResists.Add(ResistanceType.Energy);
            cleric.MediumResists.Add(ResistanceType.Physical);
            cleric.MediumResists.Add(ResistanceType.Fire);
            cleric.MediumResists.Add(ResistanceType.Cold);
            cleric.PoorResists.Add(ResistanceType.Poison);
            cleric.FastSkills.Add(SkillName.Bludgeoning);
            cleric.FastSkills.Add(SkillName.Healing);
            cleric.FastSkills.Add(SkillName.Magery);
            cleric.FastSkills.Add(SkillName.Meditation);
            cleric.MediumSkills.Add(SkillName.MagicResist);
            cleric.MediumSkills.Add(SkillName.Parry);
            cleric.MediumSkills.Add(SkillName.Spiritualism);
            cleric.AllowedAlignments.AddRange(GetAllAlignments());
            m_Definitions[CompanionClass.Cleric] = cleric;

            CompanionDefinition sorcerer = new CompanionDefinition();
            sorcerer.Class = CompanionClass.Sorcerer;
            sorcerer.BaseStr = 35; sorcerer.BaseDex = 45; sorcerer.BaseInt = 70;
            sorcerer.StrPerLevel = 5; sorcerer.DexPerLevel = 10; sorcerer.IntPerLevel = 20;
            sorcerer.HpCap = 400;
            sorcerer.BaseDamageMin = 2; sorcerer.BaseDamageMax = 7; sorcerer.DamageLevelsPerPoint = 4;
            sorcerer.GoodResists.Add(ResistanceType.Energy);
            sorcerer.MediumResists.Add(ResistanceType.Fire);
            sorcerer.MediumResists.Add(ResistanceType.Cold);
            sorcerer.PoorResists.Add(ResistanceType.Poison);
            sorcerer.PoorResists.Add(ResistanceType.Physical);
            sorcerer.FastSkills.Add(SkillName.Magery);
            sorcerer.FastSkills.Add(SkillName.Psychology);
            sorcerer.FastSkills.Add(SkillName.Meditation);
            sorcerer.MediumSkills.Add(SkillName.Bludgeoning);
            sorcerer.MediumSkills.Add(SkillName.Tactics);
            sorcerer.MediumSkills.Add(SkillName.Inscribe);
            sorcerer.AllowedAlignments.AddRange(GetAllAlignments());
            m_Definitions[CompanionClass.Sorcerer] = sorcerer;

            CompanionDefinition bard = new CompanionDefinition();
            bard.Class = CompanionClass.Bard;
            bard.BaseStr = 40; bard.BaseDex = 60; bard.BaseInt = 50;
            bard.StrPerLevel = 10; bard.DexPerLevel = 10; bard.IntPerLevel = 15;
            bard.HpCap = 600;
            bard.BaseDamageMin = 3; bard.BaseDamageMax = 8; bard.DamageLevelsPerPoint = 3;
            bard.MediumResists.Add(ResistanceType.Fire);
            bard.MediumResists.Add(ResistanceType.Cold);
            bard.MediumResists.Add(ResistanceType.Energy);
            bard.PoorResists.Add(ResistanceType.Poison);
            bard.PoorResists.Add(ResistanceType.Physical);
            bard.FastSkills.Add(SkillName.Musicianship);
            bard.FastSkills.Add(SkillName.Peacemaking);
            bard.FastSkills.Add(SkillName.Discordance);
            bard.FastSkills.Add(SkillName.Provocation);
            bard.MediumSkills.Add(SkillName.Swords);
            bard.MediumSkills.Add(SkillName.Bludgeoning);
            bard.MediumSkills.Add(SkillName.Fencing);
            bard.MediumSkills.Add(SkillName.Tactics);
            bard.MediumSkills.Add(SkillName.Healing);
            bard.MediumSkills.Add(SkillName.Magery);
            bard.MediumSkills.Add(SkillName.Meditation);
            bard.AllowedAlignments.AddRange(GetAllAlignments());
            m_Definitions[CompanionClass.Bard] = bard;
        }

        public static CompanionDefinition Get(CompanionClass classType)
        {
            if (m_Definitions == null)
                Initialize();

            if (m_Definitions.ContainsKey(classType))
                return m_Definitions[classType];

            return null;
        }

        private static List<CompanionAlignment> GetAllAlignments()
        {
            List<CompanionAlignment> list = new List<CompanionAlignment>();
            list.Add(CompanionAlignment.GetLawfulGood());
            list.Add(CompanionAlignment.GetLawfulNeutral());
            list.Add(CompanionAlignment.GetLawfulEvil());
            list.Add(CompanionAlignment.GetNeutralGood());
            list.Add(CompanionAlignment.GetTrueNeutral());
            list.Add(CompanionAlignment.GetNeutralEvil());
            list.Add(CompanionAlignment.GetChaoticGood());
            list.Add(CompanionAlignment.GetChaoticNeutral());
            list.Add(CompanionAlignment.GetChaoticEvil());
            return list;
        }

        private static List<CompanionAlignment> GetNeutralAlignments()
        {
            List<CompanionAlignment> list = new List<CompanionAlignment>();
            list.Add(CompanionAlignment.GetTrueNeutral());
            list.Add(CompanionAlignment.GetNeutralGood());
            list.Add(CompanionAlignment.GetNeutralEvil());
            list.Add(CompanionAlignment.GetChaoticNeutral());
            list.Add(CompanionAlignment.GetLawfulNeutral());
            return list;
        }

        private static List<CompanionAlignment> GetLawfulAlignments()
        {
            List<CompanionAlignment> list = new List<CompanionAlignment>();
            list.Add(CompanionAlignment.GetLawfulGood());
            list.Add(CompanionAlignment.GetLawfulNeutral());
            list.Add(CompanionAlignment.GetLawfulEvil());
            return list;
        }

        private static List<CompanionAlignment> GetChaoticAlignments()
        {
            List<CompanionAlignment> list = new List<CompanionAlignment>();
            list.Add(CompanionAlignment.GetChaoticGood());
            list.Add(CompanionAlignment.GetChaoticNeutral());
            list.Add(CompanionAlignment.GetChaoticEvil());
            return list;
        }

        private static List<CompanionAlignment> GetGoodAlignments()
        {
            List<CompanionAlignment> list = new List<CompanionAlignment>();
            list.Add(CompanionAlignment.GetChaoticGood());
            list.Add(CompanionAlignment.GetLawfulGood());
            list.Add(CompanionAlignment.GetNeutralGood());
            return list;
        }

        private static List<CompanionAlignment> GetEvilAlignments()
        {
            List<CompanionAlignment> list = new List<CompanionAlignment>();
            list.Add(CompanionAlignment.GetChaoticEvil());
            list.Add(CompanionAlignment.GetLawfulEvil());
            list.Add(CompanionAlignment.GetNeutralEvil());
            return list;
        }

        public bool IsAlignmentAllowed(CompanionAlignment alignment)
        {
            for (int i = 0; i < m_AllowedAlignments.Count; i++)
            {
                CompanionAlignment allowed = m_AllowedAlignments[i];
                if (allowed.Order == alignment.Order && allowed.Moral == alignment.Moral)
                    return true;
            }
            return false;
        }
    }
}