using System;
using Server;

namespace Server.Companions.Data
{
    public static class CompanionStats
    {
        public const int MaxStamina = 175;
        public const int MaxMana = 175;
        public const int MaxSkillCap = 125;
        public const int FastSkillStart = 49;
        public const int MediumSkillStart = 38;
        public const int FastSkillGainPerLevel = 4;
        public const int MediumSkillGainPerLevel = 3;

        public static int RollStatGain(int baseGain)
        {
            double minMultiplier = 0.9;
            double maxMultiplier = 1.15;
            
            double min = baseGain * minMultiplier;
            double max = baseGain * maxMultiplier;
            
            return Utility.RandomMinMax((int)Math.Floor(min), (int)Math.Ceiling(max));
        }

        public static int CalculateHP(int strength, int cap)
        {
            int hp = 100 + (2 * strength);
            return Math.Min(hp, cap);
        }

        public static double GetFastSkillValue(int level)
        {
            if (level < 1) 
                return 0;
            return FastSkillStart + (FastSkillGainPerLevel * (level - 1));
        }

        public static double GetMediumSkillValue(int level)
        {
            if (level < 1) 
                return 0;
            return MediumSkillStart + (MediumSkillGainPerLevel * (level - 1));
        }

        public static int GetStamina(int dexterity)
        {
            return Math.Min(dexterity, MaxStamina);
        }

        public static int GetMana(int intelligence)
        {
            return Math.Min(intelligence, MaxMana);
        }

        public static int GetResistValue(int level, ResistQuality quality)
        {
            int baseResist = 30;
            
            if (quality == ResistQuality.Good)
            {
                // Reaches 70 at level 10: 30 + (40 / 9) per level
                if (level >= 10)
                    return 70;
                return baseResist + ((level - 1) * 40 / 9);
            }
            else if (quality == ResistQuality.Medium)
            {
                // Reaches 70 at level 15: 30 + (40 / 14) per level
                if (level >= 15)
                    return 70;
                return baseResist + ((level - 1) * 40 / 14);
            }
            else // Poor
            {
                // Reaches 60 at level 20: 30 + (30 / 19) per level
                return baseResist + ((level - 1) * 30 / 19);
            }
        }

        public static void GetDamageRange(int level, int baseDamageMin, int baseDamageMax, int levelsPerPoint, out int minDamage, out int maxDamage)
        {
            int bonusPoints = (level - 1) / levelsPerPoint;
            minDamage = baseDamageMin + bonusPoints;
            maxDamage = baseDamageMax + bonusPoints;
        }
    }

    public enum ResistQuality
    {
        Good,
        Medium,
        Poor
    }
}