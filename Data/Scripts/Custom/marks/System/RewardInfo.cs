using System;
using Server;
using Server.Items;

namespace Server.Custom.DefenderOfTheRealm
{
    public class RewardInfo
    {
        public Type     ItemType;
        public int      Cost;
        public int      ItemID;
        public string   Name;
        public bool     Hueable;
        public int      Hue;
        public object[] Args;

        public RewardInfo(Type type, int cost, int itemID, string name, bool hueable, int hue, params object[] args)
        {
            ItemType = type;
            Cost     = cost;
            ItemID   = itemID;
            Name     = name;
            Hueable  = hueable;
            Hue      = hue;
            Args     = args;
        }

        public Item CreateItem(int factionType)
        {
            Item item = (Item)Activator.CreateInstance(ItemType, Args);

            if (Hueable)
            {
                switch (factionType)
                {
                    case 1: item.Hue = 53;     break;
                    case 2: item.Hue = 37;     break;
                    case 3: item.Hue = 1109;   break;
                    case 4: item.Hue = 669;    break;
                    case 5: item.Hue = 0x9C2;  break;
                    case 6: item.Hue = 0x213;  break;
                }
            }
            else if (Hue != 0)
            {
                item.Hue = Hue;
            }

            return item;
        }
    }
}