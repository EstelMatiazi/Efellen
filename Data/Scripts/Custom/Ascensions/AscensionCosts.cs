using System.Collections.Generic;

public static class AscensionCosts
{
    private static Dictionary<int, int> m_GoldCosts = new Dictionary<int, int>()
    {
        {2, 2000}, {3, 4000}, {4, 8000}, {5, 14000},
        {6, 22000}, {7, 31000}, {8, 40000}, {9, 50000},
        {10, 60000}, {11, 75000}, {12, 90000}, {13, 110000},
        {14, 140000}, {15, 180000}, {16, 220000},
        {17, 260000}, {18, 320000}, {19, 440000},
        {20, 500000}
    };

    private static Dictionary<int, int> m_ScrollCosts = new Dictionary<int, int>()
    {
        {2,2},{3,3},{4,3},{5,5},{6,5},{7,5},{8,5},
        {9,8},{10,10},{11,12},{12,15},{13,20},
        {14,25},{15,30},{16,35},{17,40},{18,45},
        {19,50},{20,100}
    };

    private static Dictionary<int, int> m_DustCosts = new Dictionary<int, int>()
    {
        {2, 1000}, {3, 2000}, {4, 4000}, {5, 7000},
        {6, 11000}, {7, 15500}, {8, 20000},
        {9, 25000}, {10, 30000}, {11, 37500},
        {12, 45000}, {13, 55000},
        {14, 70000}, {15, 90000}, {16, 110000},
        {17, 130000}, {18, 160000},
        {19, 220000}, {20, 250000}
    };


    private static Dictionary<int, int> m_ExperienceRequired = new Dictionary<int, int>()
    {
        {2,2000},{3,6000},{4,12000},{5,20000},
        {6,30000},{7,42000},{8,56000},{9,72000},
        {10,90000},{11,110000},{12,132000},
        {13,156000},{14,182000},{15,210000},
        {16,240000},{17,272000},{18,306000},
        {19,342000},{20,380000}
    };

    public static int GetDustCost(int level)
    {
        return m_DustCosts.ContainsKey(level) ? m_DustCosts[level] : 0;
    }


    public static int GetGoldCost(int level)
    {
        return m_GoldCosts.ContainsKey(level) ? m_GoldCosts[level] : 0;
    }

    public static int GetScrollCost(int level)
    {
        return m_ScrollCosts.ContainsKey(level) ? m_ScrollCosts[level] : 0;
    }

    public static int GetRequiredExperience(int level)
    {
        return m_ExperienceRequired.ContainsKey(level) ? m_ExperienceRequired[level] : 0;
    }
}
