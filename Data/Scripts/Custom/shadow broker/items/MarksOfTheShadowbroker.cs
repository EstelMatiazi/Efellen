using System;
using Server;

namespace Server.Items
{
    public class MarksOfTheShadowbroker : Item
    {
        [Constructable]
        public MarksOfTheShadowbroker() : this(1)
        {
        }
        
        public override string DefaultDescription{ get{ return "A Mark of the Shadowbroker represents your prowess as a thief. It can be aqquired by burglars as they adventure and filfer the pockets of their victims. The guildmaster of the thieves guild can offer many trinkets for those that would speak of rewards with them."; } }

        [Constructable]
        public MarksOfTheShadowbroker(int amount) : base(0x2ff8)
        {
            Stackable = true;
            Weight = 0.1;
            Hue = 0x455;
            Amount = amount;
            Name = "Mark of the Shadow Broker";
        }

        public MarksOfTheShadowbroker(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
