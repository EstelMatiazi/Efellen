using System;
using System.Collections.Generic;
using Server;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public class TomeOfAscension : Item
    {
        private Dictionary<AscensionType, int> m_Entries;

        [CommandProperty(AccessLevel.GameMaster)]
        public Dictionary<AscensionType, int> Entries
        {
            get { return m_Entries; }
        }

        [Constructable]
        public TomeOfAscension()
            : base(0x2259)
        {
            Weight = 1.0;
            Hue = 0x439;
            Name = "Tome of Ascension";
            LootType = LootType.Blessed;

            m_Entries = new Dictionary<AscensionType, int>();
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.CheckAlive() && IsChildOf(from.Backpack))
            {
                list.Add(new OrganizeScrollsEntry(from, this));
            }
        }

        private class OrganizeScrollsEntry : ContextMenuEntry
        {
            private Mobile m_From;
            private TomeOfAscension m_Tome;

            public OrganizeScrollsEntry(Mobile from, TomeOfAscension tome)
                : base(0097)
            {
                m_From = from;
                m_Tome = tome;
            }

            public override void OnClick()
            {
                if (m_From.CheckAlive() && m_Tome.IsChildOf(m_From.Backpack))
                {
                    int totalAdded = m_Tome.CollectAscensionScrolls(m_From);

                    if (totalAdded > 0)
                    {
                        m_From.SendSound(0x42, m_Tome.GetWorldLocation());
                        m_From.SendMessage("You have organized {0} ascension scroll{1} into the tome.", totalAdded, totalAdded != 1 ? "s" : "");
                        if (m_From is PlayerMobile)
                            m_From.SendGump(new TomeOfAscensionGump((PlayerMobile)m_From, m_Tome));
                    }
                    else
                    {
                        m_From.SendMessage("There are no ascension scrolls in your backpack to collect.");
                    }
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }

            if (m_Entries.Count == 0)
            {
                from.SendMessage("The tome is empty.");
                return;
            }

            if (from is PlayerMobile)
            {
                from.SendGump(new TomeOfAscensionGump((PlayerMobile)from, this));
            }
        }

        public int CollectAscensionScrolls(Mobile from)
        {
            if (from == null || from.Backpack == null)
                return 0;
            List<Item> toDelete = new List<Item>();
            int totalAdded = 0;

            // Collect all ascension scrolls from backpack and nested containers
            List<Item> found = new List<Item>();
            GetAllScrollsInBackpack(from.Backpack, found);

            foreach (Item item in found)
            {
                if (item == null || item.Deleted)
                    continue;

                AscensionScroll scroll = item as AscensionScroll;
                if (scroll == null)
                    continue;

                AscensionType type = scroll.Ascension;

                if (m_Entries.ContainsKey(type))
                    m_Entries[type] += scroll.Amount;
                else
                    m_Entries[type] = scroll.Amount;

                totalAdded += scroll.Amount;
                toDelete.Add(item);
            }

            foreach (Item item in toDelete)
            {
                item.Delete();
            }

            if (totalAdded > 0)
                InvalidateProperties();

            return totalAdded;
        }

        private void GetAllScrollsInBackpack(Container container, List<Item> list)
        {
            if (container == null)
                return;

            foreach (Item item in container.Items)
            {
                if (item == null || item.Deleted)
                    continue;

                if (item is Container)
                    GetAllScrollsInBackpack((Container)item, list);
                else if (item is AscensionScroll)
                    list.Add(item);
            }
        }

        private AscensionScroll FindExistingScrollStack(Container pack, AscensionType type)
        {
            if (pack == null)
                return null;

            for (int i = 0; i < pack.Items.Count; i++)
            {
                AscensionScroll scroll = pack.Items[i] as AscensionScroll;
                if (scroll != null && scroll.Ascension == type)
                    return scroll;
            }

            return null;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            AscensionScroll scroll = dropped as AscensionScroll;

            if (scroll == null)
                return false;

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("The tome must be in your backpack to add scrolls to it.");
                return false;
            }

            AscensionType type = scroll.Ascension;
            int amount = scroll.Amount;

            if (m_Entries.ContainsKey(type))
                m_Entries[type] += amount;
            else
                m_Entries[type] = amount;

            scroll.Delete();
            InvalidateProperties();
            from.SendSound(0x42, GetWorldLocation());
            from.SendMessage("Ascension scroll added to the tome.");

            if (from is PlayerMobile)
                from.SendGump(new TomeOfAscensionGump((PlayerMobile)from, this));

            return true;
        }

        public void WithdrawScrolls(Mobile from, AscensionType type, int amount)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("The tome must be in your backpack to withdraw scrolls.");
                return;
            }

            if (!m_Entries.ContainsKey(type))
            {
                from.SendMessage("There are no scrolls of that type in the tome.");
                return;
            }

            if (amount <= 0)
            {
                from.SendMessage("Invalid amount.");
                return;
            }

            int available = m_Entries[type];
            if (amount > available)
            {
                from.SendMessage("You only have {0} scroll{1} of that type.", available, available != 1 ? "s" : "");
                return;
            }

            Container pack = from.Backpack;
            if (pack == null)
                return;

            AscensionScroll existing = FindExistingScrollStack(pack, type);

            if (existing != null)
            {
                existing.Amount += amount;
            }
            else
            {
                AscensionScroll scroll = new AscensionScroll(type);
                scroll.Amount = amount;
                pack.DropItem(scroll);
            }

            m_Entries[type] -= amount;
            if (m_Entries[type] <= 0)
                m_Entries.Remove(type);

            InvalidateProperties();
            from.SendSound(0x55, GetWorldLocation());
            from.SendMessage("You have withdrawn {0} ascension scroll{1} from the tome.", amount, amount != 1 ? "s" : "");

            if (from is PlayerMobile)
                from.SendGump(new TomeOfAscensionGump((PlayerMobile)from, this));
        }

        public TomeOfAscension(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.WriteEncodedInt(m_Entries.Count);
            foreach (KeyValuePair<AscensionType, int> kvp in m_Entries)
            {
                writer.Write((int)kvp.Key);
                writer.Write((int)kvp.Value);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        int count = reader.ReadEncodedInt();
                        m_Entries = new Dictionary<AscensionType, int>();

                        for (int i = 0; i < count; ++i)
                        {
                            AscensionType type = (AscensionType)reader.ReadInt();
                            int amount = reader.ReadInt();
                            m_Entries[type] = amount;
                        }

                        break;
                    }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            int totalScrolls = 0;
            foreach (KeyValuePair<AscensionType, int> kvp in m_Entries)
            {
                totalScrolls += kvp.Value;
            }

            list.Add("Ascension Scrolls: {0}", totalScrolls);
        }
    }

    public class TomeOfAscensionGump : Gump
    {
        private PlayerMobile m_From;
        private TomeOfAscension m_Tome;
        private List<KeyValuePair<AscensionType, int>> m_SortedEntries;

        public TomeOfAscensionGump(PlayerMobile from, TomeOfAscension tome)
            : base(80, 60)
        {
            m_From = from;
            m_Tome = tome;

            m_SortedEntries = new List<KeyValuePair<AscensionType, int>>(tome.Entries);
            m_SortedEntries.Sort((a, b) => a.Key.ToString().CompareTo(b.Key.ToString()));

            BuildGump();
        }

        private void BuildGump()
        {
            int entryCount = m_SortedEntries.Count;
            int rowsHeight = Math.Max(1, entryCount) * 48;
            int totalHeight = Math.Min(560, Math.Max(260, 95 + rowsHeight + 25));

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            const int gumpWidth = 520;

            AddPage(0);

           AddBackground(0, 0, gumpWidth, totalHeight, 0x1453);
            AddImageTiled(10, 10, gumpWidth - 20, totalHeight - 20, 0xA40);
            AddAlphaRegion(10, 10, gumpWidth - 20, totalHeight - 20);

            // Icon
            AddBackground(20, 16, 44, 44, 0x2422);
            AddImageTiled(22, 18, 40, 40, 0xA40);
            AddAlphaRegion(22, 18, 40, 40);
            AddItem(25, 20, 0x2259, 0x439);

            // Title and Counter
            AddHtml(75, 18, 240, 22, "<BIG><BASEFONT COLOR=#F4D03F>Tome of Ascension</BASEFONT></BIG>", false, false);
            AddHtml(75, 40, 240, 20, "<BASEFONT COLOR=#BDC3C7><SMALL>Arcane Tome of ancestral scrolls</SMALL></BASEFONT>", false, false);

            AddHtml(310, 24, 180, 20, $"<RIGHT><BASEFONT COLOR=#82E0AA>Total: {GetTotalScrollCount()} scroll{(GetTotalScrollCount() != 1 ? "s" : "")}</BASEFONT></RIGHT>", false, false);

            // Subtle Divider
            AddImageTiled(20, 68, gumpWidth - 40, 2, 0xBB8);

            // Scroll List
            int yPos = 78;

            if (entryCount == 0)
            {
                AddHtml(20, 120, gumpWidth - 40, 24, "<CENTER><BASEFONT COLOR=#8E999E><I>No scroll stored in the tome.</I></BASEFONT></CENTER>", false, false);
            }
            else
            {
                for (int i = 0; i < entryCount; i++)
                {
                    if (yPos + 44 > totalHeight - 15)
                        break; 

                    KeyValuePair<AscensionType, int> entry = m_SortedEntries[i];
                    AscensionType type = entry.Key;

                    // Row Background
                    AddBackground(20, yPos, gumpWidth - 40, 42, 0x2422);
                    AddImageTiled(23, yPos + 3, gumpWidth - 46, 36, 0xA40);
                    AddAlphaRegion(23, yPos + 3, gumpWidth - 46, 36);

                AscensionVisuals.DrawClassIcon(this, 26, yPos + 4, type, 34);

                AddItem(66, yPos + 7, 0xEF3, AscensionVisuals.GetItemHue(type));

                    // Class name and Quantity
                    string hexColor = AscensionVisuals.GetColorHex(type);
                    string displayName = AscensionTypeHelper.GetDisplayName(type);

                    AddHtml(100, yPos + 10, 220, 20, $"<BASEFONT COLOR={hexColor}>{displayName}</BASEFONT>", false, false);
                    AddHtml(310, yPos + 10, 90, 20, $"<BASEFONT COLOR=#F4D03F>x{entry.Value}</BASEFONT>", false, false);

                    // Withdraw Button
                    AddButton(415, yPos + 8, 0xFA5, 0xFA7, i + 1, GumpButtonType.Reply, 0);
                    AddHtml(450, yPos + 10, 40, 20, "<BASEFONT COLOR=#90EE90>Withdraw</BASEFONT>", false, false);

                    yPos += 46;
                }
            }
        }

        private int GetTotalScrollCount()
        {
            int total = 0;
            foreach (KeyValuePair<AscensionType, int> kvp in m_SortedEntries)
                total += kvp.Value;
            return total;
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 0)
                return;

            int index = info.ButtonID - 1;
            if (index < 0 || index >= m_SortedEntries.Count)
                return;

            KeyValuePair<AscensionType, int> entry = m_SortedEntries[index];

            if (entry.Value == 1)
            {
                m_Tome.WithdrawScrolls(m_From, entry.Key, 1);
            }
            else
            {
                m_From.SendGump(new TomeOfAscensionWithdrawGump(m_From, m_Tome, entry.Key, entry.Value));
            }
        }
    }

    public class TomeOfAscensionWithdrawGump : Gump
    {
        private PlayerMobile m_From;
        private TomeOfAscension m_Tome;
        private AscensionType m_Type;
        private int m_Available;

        public TomeOfAscensionWithdrawGump(PlayerMobile from, TomeOfAscension tome, AscensionType type, int available)
            : base(150, 150)
        {
            m_From = from;
            m_Tome = tome;
            m_Type = type;
            m_Available = available;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            const int width = 380;
            const int height = 230;

            AddPage(0);

            AddBackground(0, 0, width, height, 0x1453);
            AddImageTiled(10, 10, width - 20, height - 20, 0xA40);
            AddAlphaRegion(10, 10, width - 20, height - 20);

            AscensionVisuals.DrawClassIcon(this, 25, 18, type, 42);
            AddItem(72, y: 22, itemID: 0xEF3, hue: AscensionVisuals.GetItemHue(type));

            string hexColor = AscensionVisuals.GetColorHex(type);
            string displayName = AscensionTypeHelper.GetDisplayName(type);

            AddHtml(110, 18, 240, 20, $"<BIG><BASEFONT COLOR={hexColor}>Withdraw Scrolls</BASEFONT></BIG>", false, false);
            AddHtml(110, 38, 240, 20, $"<BASEFONT COLOR=#BDC3C7>{displayName}</BASEFONT>", false, false);

            AddImageTiled(20, 68, width - 40, 2, 0xBB8);

            string message = $"You have <BASEFONT COLOR=#F4D03F>{available}</BASEFONT> scroll{(available != 1 ? "s" : "")} stored.<br>How many do you want to withdraw to your backpack?";
            AddHtml(25, 78, width - 50, 42, $"<BASEFONT COLOR=#E5E7E9>{message}</BASEFONT>", false, false);

            AddHtml(25, 128, 110, 20, "<BASEFONT COLOR=#BDC3C7>Amount:</BASEFONT>", false, false);
            AddBackground(135, 125, 100, 26, 0x2422);
            AddTextEntry(140, 127, 90, 22, 0, 0, available.ToString());

            // Subtle divider
            AddImageTiled(20, 165, width - 40, 1, 0xBB8);

            // Withdraw and Cancel Buttons
            AddButton(60, 180, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
            AddHtml(95, 182, 80, 20, "<BASEFONT COLOR=#2ECC71>Withdraw</BASEFONT>", false, false);

            AddButton(210, 180, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
            AddHtml(245, 182, 80, 20, "<BASEFONT COLOR=#E74C3C>Cancel</BASEFONT>", false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID != 1)
            {
                m_From.SendGump(new TomeOfAscensionGump(m_From, m_Tome));
                return;
            }

            TextRelay entry = info.GetTextEntry(0);
            if (entry == null || String.IsNullOrEmpty(entry.Text))
            {
                m_From.SendMessage("Invalid amount. Please enter a valid number.");
                m_From.SendGump(new TomeOfAscensionGump(m_From, m_Tome));
                return;
            }

            int amount;
            if (!int.TryParse(entry.Text, out amount) || amount <= 0 || amount > m_Available)
            {
                m_From.SendMessage("Invalid amount. Please enter a number between 1 and {0}.", m_Available);
                m_From.SendGump(new TomeOfAscensionGump(m_From, m_Tome));
                return;
            }

            m_Tome.WithdrawScrolls(m_From, m_Type, amount);
        }
    }
}
