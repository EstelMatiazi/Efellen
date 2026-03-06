using Server.Companions.Data;

namespace Server.Companions.Data
{
    public static class AlignmentHueUtility
    {
        private static readonly int[] LawfulGoodHues =
        {
            1170, 2406, 2410, 1111, 2413
        };

        private static readonly int[] LawfulNeutralHues =
        {
            1109, 1111, 2401, 2305, 2413
        };

        private static readonly int[] LawfulEvilHues =
        {
            1107, 1115, 1175, 1908, 2419
        };

        private static readonly int[] NeutralGoodHues =
        {
            2125, 2213, 2407, 2414, 2422
        };

        private static readonly int[] TrueNeutralHues =
        {
            2101, 2115, 2309, 2403, 2420
        };

        private static readonly int[] NeutralEvilHues =
        {
            1905, 1910, 2109, 2307, 2425
        };

        private static readonly int[] ChaoticGoodHues =
        {
            1266, 1272, 1359, 1365, 1281, 1153
        };

        private static readonly int[] ChaoticNeutralHues =
        {
            1184, 1903, 2107, 2210, 2427
        };

        private static readonly int[] ChaoticEvilHues =
        {
            1645, 1909, 1912, 1108, 1176, 2429
        };

        public static int GetHue(CompanionAlignment alignment)
        {
            int[] table = GetTable(alignment);
            if (table == null || table.Length == 0)
                return 0;

            return table[Utility.Random(table.Length)];
        }

        private static int[] GetTable(CompanionAlignment alignment)
        {
            if (alignment.Order == OrderAxis.Lawful)
            {
                if (alignment.Moral == MoralAxis.Good) return LawfulGoodHues;
                if (alignment.Moral == MoralAxis.Neutral) return LawfulNeutralHues;
                return LawfulEvilHues;
            }

            if (alignment.Order == OrderAxis.Chaotic)
            {
                if (alignment.Moral == MoralAxis.Good) return ChaoticGoodHues;
                if (alignment.Moral == MoralAxis.Neutral) return ChaoticNeutralHues;
                return ChaoticEvilHues;
            }

            if (alignment.Moral == MoralAxis.Good) return NeutralGoodHues;
            if (alignment.Moral == MoralAxis.Evil) return NeutralEvilHues;

            return TrueNeutralHues;
        }
    }
}
