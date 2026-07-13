using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Items
{
    public class ArtifactBookGump : Gump
    {
        private enum ItemFilter
        {
            All,
            Weapons,
            Armor,
            Jewelry,
            Clothing,
            Shields
        }

        private enum SortColumn
        {
            None,
            Name,
            Str,
            Dex,
            Int,
            Hits,
            Stam,
            Mana,
            HitsRegen,
            StamRegen,
            ManaRegen,
            DI,
            HCI,
            DCI,
            SSI,
            FC,
            FCR,
            LMC,
            LRC,
            SDI,
            PhysRes,
            FireRes,
            ColdRes,
            PoisRes,
            EnergyRes,
            Luck,
            ReflectPhys,
            EnhancePotions,
            NightSight,
            LifeLeech,
            ManaLeech,
            StaminaLeech
        }

        private enum SortDirection
        {
            Ascending,
            Descending
        }

        private class ColumnDef
        {
            public string Header;
            public string Description;
            public int X;
            public SortColumn Sort;
            public Func<Item, int> Value;

            public ColumnDef(string header, string description, int x, SortColumn sort, Func<Item, int> value)
            {
                Header = header;
                Description = description;
                X = x;
                Sort = sort;
                Value = value;
            }
        }

        // Cliloc 1070722 = ("~1_val~")
        private const int TooltipCliloc = 1070722;

        private const int ItemFilterCount = 6;

        private Mobile m_From;
        private ArtifactBook m_Book;
        private int m_Page;
        private string m_Search;

        private ItemFilter m_Filter = ItemFilter.All;
        private SortColumn m_SortColumn = SortColumn.None;
        private SortDirection m_SortDir = SortDirection.Descending;

        private const int RowHeight = 40;
        private const int PageSize = 10;

        private const int ColIconX = 20;
        private const int ColNameX = 62;
        private const int ColDataStartX = ColNameX + NameColumnWidth + 10;
        private const int ColWidth = 48;
        private const int ColGetOffset = 16;
        private const int NameColumnWidth = 180;
        private const int HeaderTextOffset = 0;
        private const int HeaderArrowWidth = 20;

        private const int HeaderButtonY = 90;
        private const int HeaderY = 114;
        private const int RowStartY = 138;

        private const int BtnPrev = 1;
        private const int BtnNext = 2;
        private const int BtnFilter = 10;
        private const int BtnSearch = 12;
        private const int BtnSortBase = 20;
        private const int BtnRetrieveBase = 1000;

        public ArtifactBookGump(Mobile from, ArtifactBook book, int page, string search)
            : this(from, book, page, search, ItemFilter.All, SortColumn.None, SortDirection.Descending)
        {
        }

        private ArtifactBookGump(Mobile from, ArtifactBook book, int page, string search, ItemFilter filter, SortColumn sortColumn, SortDirection sortDir)
            : base(50, 50)
        {
            m_From = from;
            m_Book = book;
            m_Page = page;
            m_Search = search ?? "";
            m_Filter = filter;
            m_SortColumn = sortColumn;
            m_SortDir = sortDir;

            Closable = true;
            Disposable = true;
            Dragable = true;

            Build();
        }

        private List<ColumnDef> BuildColumns()
        {
            List<ColumnDef> cols = new List<ColumnDef>();
            int x = ColDataStartX;

            cols.Add(new ColumnDef("STR", "Bonus Strength", x, SortColumn.Str, item => GetAttr(item, a => a.BonusStr))); x += ColWidth;
            cols.Add(new ColumnDef("DEX", "Bonus Dexterity", x, SortColumn.Dex, item => GetAttr(item, a => a.BonusDex))); x += ColWidth;
            cols.Add(new ColumnDef("INT", "Bonus Intelligence", x, SortColumn.Int, item => GetAttr(item, a => a.BonusInt))); x += ColWidth;
            cols.Add(new ColumnDef("HP", "Bonus Hit Points", x, SortColumn.Hits, item => GetAttr(item, a => a.BonusHits))); x += ColWidth;
            cols.Add(new ColumnDef("STA", "Bonus Stamina", x, SortColumn.Stam, item => GetAttr(item, a => a.BonusStam))); x += ColWidth;
            cols.Add(new ColumnDef("MP", "Bonus Mana", x, SortColumn.Mana, item => GetAttr(item, a => a.BonusMana))); x += ColWidth;
            cols.Add(new ColumnDef("HPR", "Hit Point Regeneration", x, SortColumn.HitsRegen, item => GetAttr(item, a => a.RegenHits))); x += ColWidth;
            cols.Add(new ColumnDef("SPR", "Stamina Regeneration", x, SortColumn.StamRegen, item => GetAttr(item, a => a.RegenStam))); x += ColWidth;
            cols.Add(new ColumnDef("MPR", "Mana Regeneration", x, SortColumn.ManaRegen, item => GetAttr(item, a => a.RegenMana))); x += ColWidth;
            cols.Add(new ColumnDef("DI", "Damage Increase", x, SortColumn.DI, item => GetAttr(item, a => a.WeaponDamage))); x += ColWidth;
            cols.Add(new ColumnDef("HCI", "Hit Chance Increase", x, SortColumn.HCI, item => GetAttr(item, a => a.AttackChance))); x += ColWidth;
            cols.Add(new ColumnDef("DCI", "Defense Chance Increase", x, SortColumn.DCI, item => GetAttr(item, a => a.DefendChance))); x += ColWidth;
            cols.Add(new ColumnDef("SSI", "Swing Speed Increase", x, SortColumn.SSI, item => GetAttr(item, a => a.WeaponSpeed))); x += ColWidth;
            cols.Add(new ColumnDef("FC", "Faster Casting", x, SortColumn.FC, item => GetAttr(item, a => a.CastSpeed))); x += ColWidth;
            cols.Add(new ColumnDef("FCR", "Faster Cast Recovery", x, SortColumn.FCR, item => GetAttr(item, a => a.CastRecovery))); x += ColWidth;
            cols.Add(new ColumnDef("LMC", "Lower Mana Cost", x, SortColumn.LMC, item => GetAttr(item, a => a.LowerManaCost))); x += ColWidth;
            cols.Add(new ColumnDef("LRC", "Lower Reagent Cost", x, SortColumn.LRC, item => GetAttr(item, a => a.LowerRegCost))); x += ColWidth;
            cols.Add(new ColumnDef("SDI", "Spell Damage Increase", x, SortColumn.SDI, item => GetAttr(item, a => a.SpellDamage))); x += ColWidth;
            cols.Add(new ColumnDef("PhR", "Physical Resistance", x, SortColumn.PhysRes, GetPhysRes)); x += ColWidth;
            cols.Add(new ColumnDef("FiR", "Fire Resistance", x, SortColumn.FireRes, GetFireRes)); x += ColWidth;
            cols.Add(new ColumnDef("CoR", "Cold Resistance", x, SortColumn.ColdRes, GetColdRes)); x += ColWidth;
            cols.Add(new ColumnDef("PoR", "Poison Resistance", x, SortColumn.PoisRes, GetPoisRes)); x += ColWidth;
            cols.Add(new ColumnDef("EnR", "Energy Resistance", x, SortColumn.EnergyRes, GetEnergyRes)); x += ColWidth;
            cols.Add(new ColumnDef("Luck", "Luck", x, SortColumn.Luck, item => GetAttr(item, a => a.Luck))); x += ColWidth;
            cols.Add(new ColumnDef("RP", "Reflect Physical Damage", x, SortColumn.ReflectPhys, item => GetAttr(item, a => a.ReflectPhysical))); x += ColWidth;
            cols.Add(new ColumnDef("EP", "Enhance Potions", x, SortColumn.EnhancePotions, item => GetAttr(item, a => a.EnhancePotions))); x += ColWidth;
            cols.Add(new ColumnDef("NS", "Night Sight", x, SortColumn.NightSight, item => GetAttr(item, a => a.NightSight))); x += ColWidth;
            cols.Add(new ColumnDef("LL", "Hit Life Leech", x, SortColumn.LifeLeech, GetLifeLeechValue)); x += ColWidth;
            cols.Add(new ColumnDef("ML", "Hit Mana Leech", x, SortColumn.ManaLeech, GetManaLeechValue)); x += ColWidth;
            cols.Add(new ColumnDef("SL", "Hit Stamina Leech", x, SortColumn.StaminaLeech, GetStaminaLeechValue)); x += ColWidth;

            return cols;
        }

        private void Build()
        {
            List<ColumnDef> columns = BuildColumns();
            int colGetX = columns[columns.Count - 1].X + ColWidth + ColGetOffset;
            int gumpWidth = colGetX + 60;

            List<Item> list = GetFiltered();
            SortList(list, columns);

            int maxPage = list.Count == 0 ? 0 : (list.Count - 1) / PageSize;
            if (m_Page > maxPage)
                m_Page = maxPage;
            if (m_Page < 0)
                m_Page = 0;

            int gumpHeight = RowStartY + PageSize * RowHeight + 60;

            AddBackground(0, 0, gumpWidth, gumpHeight, 9270);

            AddLabel(gumpWidth / 2 - 60, 10, 1152, "Artifact Codex");

            AddLabel(20, 40, 1152, "Search:");
            AddTextEntry(80, 40, 90, 20, 0, 0, m_Search);
            AddButton(178, 40, 4005, 4007, BtnSearch, GumpButtonType.Reply, 0);

            AddLabel(220, 40, 1152, "Type:");
            AddButton(265, 40, 2117, 2118, BtnFilter, GumpButtonType.Reply, 0);
            AddLabel(295, 40, 1152, m_Filter.ToString());

            AddLabel(gumpWidth - 120, 40, 1152, String.Format("Page {0}/{1}", m_Page + 1, maxPage + 1));

            // Table header
            AddImageTiled(20, HeaderButtonY - 4, gumpWidth - 40, 44, 9274);

            AddSortHeader("Name", null, ColNameX, NameColumnWidth, SortColumn.Name);

            foreach (ColumnDef col in columns)
                AddSortHeader(col.Header, col.Description, col.X, ColWidth, col.Sort);

            // Table rows
            int y = RowStartY;

            int start = m_Page * PageSize;
            int end = Math.Min(start + PageSize, list.Count);

            for (int i = start; i < end; i++)
            {
                Item item = list[i];

                if (item == null || item.Deleted)
                    continue;

                AddImageTiled(20, y, gumpWidth - 40, RowHeight, 2624);

                AddItem(ColIconX, y + (RowHeight / 2) - 20, item.ItemID, item.Hue);

                string name = item.Name ?? "";
                if (name.Length > 22)
                    name = name.Substring(0, 20) + "..";

                int textY = y + (RowHeight / 2) - 7;

                AddLabel(ColNameX, textY, 1152, name);

                foreach (ColumnDef col in columns)
                {
                    int value = col.Value(item);
                    AddLabel(col.X, textY, value > 0 ? 68 : 900, value > 0 ? value.ToString() : "-");
                }

                AddButton(colGetX, textY - 2, 4005, 4007, BtnRetrieveBase + (i - start), GumpButtonType.Reply, 0);

                y += RowHeight;
            }

            int footerY = RowStartY + PageSize * RowHeight + 15;
            AddButton(gumpWidth / 2 - 60, footerY, 4014, 4016, BtnPrev, GumpButtonType.Reply, 0);
            AddButton(gumpWidth / 2 + 40, footerY, 4005, 4007, BtnNext, GumpButtonType.Reply, 0);
        }

        private void AddSortHeader(string text, string description, int x, int columnWidth, SortColumn column)
        {
            int hue = (m_SortColumn == column) ? 68 : 149;

            AddLabel(x + HeaderTextOffset, HeaderY, hue, text);

            if (!String.IsNullOrEmpty(description))
                AddTooltip(TooltipCliloc, description);

            int normal = 2117;
            int pressed = 2118;

            int buttonX = x + (columnWidth / 4) - (HeaderArrowWidth / 2);

            AddButton(
                buttonX,
                HeaderButtonY,
                normal,
                pressed,
                BtnSortBase + (int)column,
                GumpButtonType.Reply,
                0);

            if (!String.IsNullOrEmpty(description))
                AddTooltip(TooltipCliloc, description);
        }

        private List<Item> GetFiltered()
        {
            List<Item> list = new List<Item>();

            foreach (Item e in m_Book.Items)
            {
                if (e == null || e.Deleted)
                    continue;

                // search
                if (!String.IsNullOrEmpty(m_Search))
                {
                    if (e.Name == null ||
                        e.Name.ToLower().IndexOf(m_Search.ToLower()) < 0)
                        continue;
                }

                // type filter
                if (!PassTypeFilter(e))
                    continue;

                list.Add(e);
            }

            return list;
        }

        private void SortList(List<Item> list, List<ColumnDef> columns)
        {
            if (m_SortColumn == SortColumn.None)
                return;

            Func<Item, int> valueGetter = null;

            if (m_SortColumn != SortColumn.Name)
            {
                foreach (ColumnDef col in columns)
                {
                    if (col.Sort == m_SortColumn)
                    {
                        valueGetter = col.Value;
                        break;
                    }
                }
            }

            list.Sort((a, b) =>
            {
                int cmp;

                if (m_SortColumn == SortColumn.Name)
                    cmp = String.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
                else if (valueGetter != null)
                    cmp = valueGetter(a).CompareTo(valueGetter(b));
                else
                    cmp = 0;

                return m_SortDir == SortDirection.Descending ? -cmp : cmp;
            });
        }

        private bool PassTypeFilter(Item item)
        {
            switch (m_Filter)
            {
                case ItemFilter.All:
                    return true;

                case ItemFilter.Weapons:
                    return item is BaseWeapon;

                case ItemFilter.Armor:
                    return item is BaseArmor;

                 case ItemFilter.Jewelry:
                     return item is BaseGiftJewel;

                case ItemFilter.Clothing:
                    return item is BaseClothing;

                case ItemFilter.Shields:
                    return item is BaseShield;
            }

            return true;
        }

        private AosAttributes GetAttributes(Item item)
        {
            if (item is BaseWeapon)
                return ((BaseWeapon)item).Attributes;

            if (item is BaseArmor)
                return ((BaseArmor)item).Attributes;

            if (item is BaseClothing)
                return ((BaseClothing)item).Attributes;

             if (item is BaseGiftJewel)
                 return ((BaseGiftJewel)item).Attributes;

            return null;
        }

        private int GetAttr(Item item, Func<AosAttributes, int> selector)
        {
            AosAttributes attrs = GetAttributes(item);
            return attrs != null ? selector(attrs) : 0;
        }

        private int GetLifeLeechValue(Item item)
        {
            BaseWeapon bw = item as BaseWeapon;
            return bw != null ? bw.WeaponAttributes.HitLeechHits : 0;
        }

        private int GetManaLeechValue(Item item)
        {
            BaseWeapon bw = item as BaseWeapon;
            return bw != null ? bw.WeaponAttributes.HitLeechMana : 0;
        }

        private int GetStaminaLeechValue(Item item)
        {
            BaseWeapon bw = item as BaseWeapon;
            return bw != null ? bw.WeaponAttributes.HitLeechStam : 0;
        }

        private int GetPhysRes(Item item)
        {
            BaseArmor ba = item as BaseArmor;
            return ba != null ? ba.PhysicalResistance : 0;
        }

        private int GetFireRes(Item item)
        {
            BaseArmor ba = item as BaseArmor;
            return ba != null ? ba.FireResistance : 0;
        }

        private int GetColdRes(Item item)
        {
            BaseArmor ba = item as BaseArmor;
            return ba != null ? ba.ColdResistance : 0;
        }

        private int GetPoisRes(Item item)
        {
            BaseArmor ba = item as BaseArmor;
            return ba != null ? ba.PoisonResistance : 0;
        }

        private int GetEnergyRes(Item item)
        {
            BaseArmor ba = item as BaseArmor;
            return ba != null ? ba.EnergyResistance : 0;
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 0)
                return;

            TextRelay searchEntry = info.GetTextEntry(0);

            if (info.ButtonID == BtnSearch)
            {
                string newSearch = searchEntry != null ? searchEntry.Text : m_Search;
                m_From.SendGump(new ArtifactBookGump(m_From, m_Book, 0, newSearch, m_Filter, m_SortColumn, m_SortDir));
                return;
            }

            if (info.ButtonID == BtnFilter)
            {
                ItemFilter newFilter = (ItemFilter)(((int)m_Filter + 1) % ItemFilterCount);
                m_From.SendGump(new ArtifactBookGump(m_From, m_Book, 0, m_Search, newFilter, m_SortColumn, m_SortDir));
                return;
            }

            if (info.ButtonID >= BtnSortBase && info.ButtonID < BtnRetrieveBase)
            {
                SortColumn clickedColumn = (SortColumn)(info.ButtonID - BtnSortBase);

                if (Enum.IsDefined(typeof(SortColumn), clickedColumn) && clickedColumn != SortColumn.None)
                {
                    SortDirection newDir;

                    if (m_SortColumn == clickedColumn)
                        newDir = m_SortDir == SortDirection.Descending ? SortDirection.Ascending : SortDirection.Descending;
                    else
                        newDir = clickedColumn == SortColumn.Name ? SortDirection.Ascending : SortDirection.Descending;

                    m_From.SendGump(new ArtifactBookGump(m_From, m_Book, m_Page, m_Search, m_Filter, clickedColumn, newDir));
                    return;
                }
            }

            if (info.ButtonID == BtnPrev)
            {
                int newPage = m_Page > 0 ? m_Page - 1 : 0;
                m_From.SendGump(new ArtifactBookGump(m_From, m_Book, newPage, m_Search, m_Filter, m_SortColumn, m_SortDir));
                return;
            }

            if (info.ButtonID == BtnNext)
            {
                m_From.SendGump(new ArtifactBookGump(m_From, m_Book, m_Page + 1, m_Search, m_Filter, m_SortColumn, m_SortDir));
                return;
            }

            if (info.ButtonID >= BtnRetrieveBase)
            {
                int index = info.ButtonID - BtnRetrieveBase;

                List<ColumnDef> columns = BuildColumns();
                List<Item> list = GetFiltered();
                SortList(list, columns);

                int realIndex = m_Page * PageSize + index;

                if (realIndex >= 0 && realIndex < list.Count)
                {
                    Item entry = list[realIndex];

                    if (entry != null)
                        m_Book.Retrieve(m_From, entry);
                }

                m_From.SendGump(new ArtifactBookGump(m_From, m_Book, m_Page, m_Search, m_Filter, m_SortColumn, m_SortDir));
                return;
            }

            // Unrecognized button -  reopen the gump
            m_From.SendGump(new ArtifactBookGump(m_From, m_Book, m_Page, m_Search, m_Filter, m_SortColumn, m_SortDir));
        }
    }
}