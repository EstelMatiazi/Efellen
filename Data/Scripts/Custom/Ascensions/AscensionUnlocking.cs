using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom.Ascensions
{
    public static class AscensionUnlocking
    {
        public static void GetUnlockCost(PlayerMobile pm, out int gold, out int dust, out int scrolls)
        {
            int totalLevels = pm.AscensionProfile.GetTotalLevels();

            if (totalLevels == 0)
            {
                gold    = 500;
                dust    = 250;
                scrolls = 3;
                return;
            }

            gold    = totalLevels * 1000;
            dust    = gold / 2;
            scrolls = 3;
        }

        public static bool TryUnlock(PlayerMobile pm, AscensionType type)
        {
            if (pm.AscensionProfile.IsUnlocked(type))
                return false;

            int gold, dust, scrolls;
            GetUnlockCost(pm, out gold, out dust, out scrolls);

            if (!HasGold(pm, gold))
            {
                pm.SendMessage("You do not have enough gold.");
                return false;
            }

            if (!HasArcaneDust(pm, dust))
            {
                pm.SendMessage("You do not have enough arcane dust.");
                return false;
            }

            if (!HasScrolls(pm, type, scrolls))
            {
                pm.SendMessage("You do not have enough " + type.ToString() + " ascension scrolls.");
                return false;
            }

            ConsumeGold(pm, gold);
            ConsumeArcaneDust(pm, dust);
            ConsumeScrolls(pm, type, scrolls);

            AscensionProgress prog = pm.AscensionProfile.Get(type);
            prog.Unlocked    = true;
            prog.Level       = 1;
            prog.Experience  = 0;

            pm.SendMessage("You have unlocked the " + type.ToString() + " ascension.");

            return true;
        }

        internal static bool HasGold(PlayerMobile pm, int amount)
        {
            if (pm.Backpack != null && pm.Backpack.GetAmount(typeof(Gold)) >= amount)
                return true;

            if (pm.BankBox != null && pm.BankBox.GetAmount(typeof(Gold)) >= amount)
                return true;

            return false;
        }

        internal static void ConsumeGold(PlayerMobile pm, int amount)
        {
            if (pm.Backpack != null && pm.Backpack.ConsumeTotal(typeof(Gold), amount))
                return;

            if (pm.BankBox != null)
                pm.BankBox.ConsumeTotal(typeof(Gold), amount);
        }

        internal static bool HasArcaneDust(PlayerMobile pm, int amount)
        {
            int total = 0;

            if (pm.Backpack != null)
                total += pm.Backpack.GetAmount(typeof(ArcaneDust));

            if (total >= amount)
                return true;

            if (pm.BankBox != null)
                total += pm.BankBox.GetAmount(typeof(ArcaneDust));

            return total >= amount;
        }

        internal static void ConsumeArcaneDust(PlayerMobile pm, int amount)
        {
            int remaining = amount;

            if (pm.Backpack != null)
            {
                int inPack = pm.Backpack.GetAmount(typeof(ArcaneDust));

                if (inPack >= remaining)
                {
                    pm.Backpack.ConsumeTotal(typeof(ArcaneDust), remaining);
                    return;
                }

                if (inPack > 0)
                {
                    pm.Backpack.ConsumeTotal(typeof(ArcaneDust), inPack);
                    remaining -= inPack;
                }
            }

            if (pm.BankBox != null && remaining > 0)
                pm.BankBox.ConsumeTotal(typeof(ArcaneDust), remaining);
        }

        
        internal static bool HasScrolls(PlayerMobile pm, AscensionType type, int amount)
        {
            if (pm == null)
                return false;

            int total = CountScrolls(pm.Backpack, type);

            if (total >= amount)
                return true;

            total += CountScrolls(pm.BankBox, type);

            return total >= amount;
        }

        internal static void ConsumeScrolls(PlayerMobile pm, AscensionType type, int amount)
        {
            if (pm == null)
                return;

            int remaining = ConsumeScrollsFromContainer(pm.Backpack, type, amount);

            if (remaining > 0)
                ConsumeScrollsFromContainer(pm.BankBox, type, remaining);
        }

        
        private static int CountScrolls(Container container, AscensionType type)
        {
            if (container == null)
                return 0;

            int total = 0;

            foreach (Item item in container.Items)
            {
                AscensionScroll scroll = item as AscensionScroll;

                if (scroll != null && scroll.Ascension == type)
                    total += scroll.Amount;
            }

            return total;
        }

        private static int ConsumeScrollsFromContainer(Container container, AscensionType type, int amount)
        {
            if (container == null)
                return amount;

            int remaining = amount;

            for (int i = container.Items.Count - 1; i >= 0; i--)
            {
                AscensionScroll scroll = container.Items[i] as AscensionScroll;

                if (scroll == null || scroll.Ascension != type)
                    continue;

                if (scroll.Amount <= remaining)
                {
                    remaining -= scroll.Amount;
                    scroll.Delete();
                }
                else
                {
                    scroll.Amount -= remaining;
                    remaining      = 0;
                }

                if (remaining <= 0)
                    break;
            }

            return remaining;
        }
    }
}