using System;
using Server;
using Server.Items;

namespace Server.Custom.DefenderOfTheRealm
{
    public class RewardInfo
    {
        public Type ItemType;
        public int Cost;
        public int ItemID;
        public string Name;
        public bool Hueable;
        public int Hue;
        public object[] Args;

        public RewardInfo(Type type, int cost, int itemID, string name, bool hueable, int hue,params object[] args)
        {
            ItemType = type;
            Cost = cost;
            ItemID = itemID;
            Name = name;
            Hueable = hueable;
            Hue = hue;
            Args = args; //amount
        }

        public Item CreateItem(bool isDefender)
        {
            Item item = (Item)Activator.CreateInstance(ItemType, Args);

            if (Hueable)
            {
                item.Hue = isDefender ? 0x35 : 0x25;
            }
            return item;
        }
    }
}
