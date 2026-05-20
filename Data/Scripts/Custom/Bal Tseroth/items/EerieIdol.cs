using System;
using Server;

namespace Server.Items
{
    public class EerieIdol : Item
    {
        [Constructable]
        public EerieIdol() : this(1)
        {
        }

        [Constructable]
        public EerieIdol(int amount) : base(0x4688)
        {
            Stackable = true;
            Weight = 0.01;
            Hue = 0x0213;
            Amount = amount;
            Name = "Eerie Idol";
        }

        public override string DefaultDescription{ get{ return "Este ídolo antigo brilha com uma energia estranha e distorcida. Pollo pode estar procurando por estes na entrada da escavação."; } }

        public EerieIdol(Serial serial) : base(serial)
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
