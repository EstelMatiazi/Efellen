using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Misc;
using Server.Custom.Ascensions;

namespace Server.Items
{
    public class HoardPiles : Item
    {
        private int      m_Uses;
        private DateTime m_DecayTime;
        private Timer    m_DecayTimer;

        public string HoardName;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Uses { get { return m_Uses; } set { m_Uses = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.Owner)]
        public string Hoard_Name { get { return HoardName; } set { HoardName = value; InvalidateProperties(); } }

        public virtual TimeSpan DecayDelay { get { return TimeSpan.FromMinutes(10.0); } }

        [Constructable]
        public HoardPiles() : base(0x0879)
        {
            Movable = false;
            Name    = "treasure hoard";
            Light   = LightType.Circle225;
            ItemID  = Utility.RandomList(0x0879, 0x08AD);
        }

        public HoardPiles(Serial serial) : base(serial)
        {
        }

        public virtual void RefreshDecay(bool setDecayTime)
        {
            if (Deleted)
                return;

            if (m_DecayTimer != null)
                m_DecayTimer.Stop();

            if (setDecayTime)
                m_DecayTime = DateTime.Now + DecayDelay;

            TimeSpan ts = m_DecayTime - DateTime.Now;

            if (ts < TimeSpan.FromMinutes(2.0))
                ts = TimeSpan.FromMinutes(2.0);

            m_DecayTimer = Timer.DelayCall(ts, new TimerCallback(Delete));
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Blessed)
            {
                from.SendMessage("You cannot look through that while in this state.");
                return;
            }

            if (!from.InRange(GetWorldLocation(), 3))
            {
                from.SendMessage("You will have to get closer to it!");
                return;
            }

            if (m_Uses >= 5)
            {
                from.SendMessage("There is nothing else worth taking from this pile!");
                Delete();
                return;
            }

            m_Uses++;

            if (GetPlayerInfo.LuckyPlayer(from.Luck) && Utility.RandomBool())
                m_Uses--;

            from.PlaySound(0x2E5);
            from.SendMessage("You pull something from the treasure hoard!");

            Item item = GetHoardItem(from);

            if (item is Container)
                item.MoveToWorld(from.Location, from.Map);
            else
                from.AddToBackpack(item);
        }

        private Item GetHoardItem(Mobile from)
        {
            int enchantPower = Math.Min(500, 300 + (from.Luck * 200 / 2000));

            switch (Utility.Random(17))
            {
                case 0:
                    if (Utility.RandomDouble() < 0.33)
                        return Loot.RandomArty();

                    return LootPackEntry.Enchant(from, 500, Loot.RandomMagicalItem(LootPackEntry.playOrient(from)));

                case 1:
                case 2:
                    if (Utility.RandomDouble() < 0.44)
                        return Loot.RandomSArty(LootPackEntry.playOrient(from), from);

                    return LootPackEntry.Enchant(from, 500, Loot.RandomMagicalItem(LootPackEntry.playOrient(from)));
                case 3:
                    return Loot.RandomRelic(from);
                case 4:
                    return Loot.RandomRare(Utility.RandomMinMax(6, 12), from);
                case 5:
                    return Loot.RandomBooks(Utility.RandomMinMax(6, 12));
                case 6:
                case 7:
					 return AscensionScrollFactory.CreateRandom();
				case 8:
                case 9:
                case 10:
                    return MakeCurrencyDrop(from);
                case 11:
                case 12:
                    return LootPackEntry.Enchant(from, enchantPower, Loot.RandomMagicalItem(LootPackEntry.playOrient(from)));

                case 13:
                    return LootPackEntry.Enchant(from, enchantPower, Loot.RandomInstrument());

                case 14:
                    return Loot.RandomPotion(Utility.RandomMinMax(6, 12), true);

                case 15:
                    return new MagicalWand(Utility.RandomMinMax(6, 8));

                case 16:
                    return MakeLootChest(from);

                default:
                    return MakeCurrencyDrop(from);
            }
        }

        private static Item MakeCurrencyDrop(Mobile from)
        {
            int luckMod = Math.Min(from.Luck, 2000);

            if (Region.Find(from.Location, from.Map).IsPartOf("the Mines of Morinia"))
                return new Crystals(luckMod + Utility.RandomMinMax(200, 400));

            if (from.Land == Land.Underworld)
                return new DDJewels(luckMod + Utility.RandomMinMax(500, 1000));

            return new Gold(luckMod + Utility.RandomMinMax(1000, 2000));
        }

        private Item MakeLootChest(Mobile from)
        {
            m_Uses = 6; // Stop giving loot once a container is awarded

            int chestLuck  = Math.Max(3, Math.Min(8, GetPlayerInfo.LuckyPlayerArtifacts(from.Luck)));
            int chestLevel = Utility.RandomMinMax(3, chestLuck);

            Item item = new LootChest(chestLevel);

            item.ItemID = Utility.RandomList(0x9AB, 0xE40, 0xE41, 0xE7C);
            item.Hue    = Utility.RandomList(
                0x961, 0x962, 0x963, 0x964, 0x965, 0x966, 0x967, 0x968,
                0x969, 0x96A, 0x96B, 0x96C, 0x96D, 0x96E, 0x96F, 0x970,
                0x971, 0x972, 0x973, 0x974, 0x975, 0x976, 0x977, 0x978,
                0x979, 0x97A, 0x97B, 0x97C, 0x97D, 0x97E, 0x4AA
            );

            string[] boxNames = new string[]
            {
                "hoard chest", "treasure chest", "secret chest",  "fabled chest",
                "legendary chest", "mythical chest", "lost chest", "stolen chest"
            };

            string box = boxNames[Utility.Random(boxNames.Length)];

            item.Name = Utility.RandomBool()
                ? box + " from " + Worlds.GetRegionName(from.Map, from.Location)
                : box + " of " + HoardName;

            LootPackChange.AddGoldToContainer(Utility.RandomMinMax(5000, 8000), (LootChest)item, from, chestLevel);

            int artyChance = GetPlayerInfo.LuckyPlayerArtifacts(from.Luck) + 10;
            if (Utility.RandomMinMax(0, 90) < artyChance)
                ((LootChest)item).DropItem(Loot.RandomArty());

            return item;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_DecayTime);
            writer.WriteEncodedInt(m_Uses);
            writer.Write(HoardName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    m_DecayTime = reader.ReadDateTime();
                    RefreshDecay(false);
                    break;
            }

            m_Uses    = reader.ReadEncodedInt();
            HoardName = reader.ReadString();
        }
    }
}