using System;

namespace Server.Companions.Core
{
    public class CompanionExperience
    {
        private const int MaxLevel = 20;
        
        private static readonly long[] ExperienceTable = new long[]
        {
            0,      // Level 1 (starting level)
            1000,   // Level 2
            3000,   // Level 3
            6000,   // Level 4
            10000,  // Level 5
            15000,  // Level 6
            21000,  // Level 7
            28000,  // Level 8
            36000,  // Level 9
            45000,  // Level 10
            55000,  // Level 11
            66000,  // Level 12
            78000,  // Level 13
            91000,  // Level 14
            105000, // Level 15
            120000, // Level 16
            136000, // Level 17
            153000, // Level 18
            171000, // Level 19
            190000  // Level 20
        };

        public static long GetExperienceForLevel(int level)
        {
            if (level < 1 || level > MaxLevel)
                return 0;

            return ExperienceTable[level - 1];
        }

        public static long GetExperienceForNextLevel(int currentLevel)
        {
            if (currentLevel >= MaxLevel)
                return long.MaxValue;

            return GetExperienceForLevel(currentLevel + 1);
        }

        public static int GetLevelFromExperience(long experience)
        {
            for (int i = MaxLevel; i >= 1; i--)
            {
                if (experience >= GetExperienceForLevel(i))
                    return i;
            }

            return 1;
        }

        public static long GetExperienceToNextLevel(int currentLevel, long currentExperience)
        {
            if (currentLevel >= MaxLevel)
                return 0;

            long nextLevelExp = GetExperienceForNextLevel(currentLevel);
            return Math.Max(0, nextLevelExp - currentExperience);
        }

        public static double GetLevelProgress(int currentLevel, long currentExperience)
        {
            if (currentLevel >= MaxLevel)
                return 1.0;

            long currentLevelExp = GetExperienceForLevel(currentLevel);
            long nextLevelExp = GetExperienceForNextLevel(currentLevel);
            long experienceInCurrentLevel = currentExperience - currentLevelExp;
            long experienceNeededForLevel = nextLevelExp - currentLevelExp;

            if (experienceNeededForLevel <= 0)
                return 1.0;

            return Math.Max(0.0, Math.Min(1.0, (double)experienceInCurrentLevel / experienceNeededForLevel));
        }

        public static bool ShouldLevelUp(int currentLevel, long currentExperience)
        {
            if (currentLevel >= MaxLevel)
                return false;

            return currentExperience >= GetExperienceForNextLevel(currentLevel);
        }
    }
}