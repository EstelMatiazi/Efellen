using System;
using Server;

namespace Server.Items
{
    public class MarksOfBhaal : Item
    {
        [Constructable]
        public MarksOfBhaal() : this(1)
        {
        }
        
        public override string DefaultDescription{ get{ return "A Mark of Bhaal represents your devotion to the god of murder. It can be aqquired by assassins as they strike from the shadows and poison their foes. The guildmaster of the assassins guild can offer many trinkets for those that would speak of rewards with them."; } }

        [Constructable]
        public MarksOfBhaal(int amount) : base(0x2FE1)
        {
            Stackable = true;
            Weight = 0.01;
            Hue = 0x047E;
            Amount = amount;
            Name = "Mark of Bhaal";
        }

        public MarksOfBhaal(Serial serial) : base(serial)
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
