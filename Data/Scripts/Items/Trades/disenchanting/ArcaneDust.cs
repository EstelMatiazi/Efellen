using System;
using Server.Items;

namespace Server.Items
{
    public class ArcaneDust : Item
    {
        [Constructable]
        public ArcaneDust() : this(1)
        {
        }

        [Constructable]
        public ArcaneDust(int amount) : base(12265)
        {
            Name = "arcane dust";
            Hue = 33;
            Stackable = true;
            Amount = amount;
            Weight = 0.01;
        }
        public override string DefaultDescription{ get{ return"Esta é a essência mágica condensada usada na criação de um item mágico. Artesãos de guilda qualificados podem usá-lo para aprimorar as propriedades de itens mágicos."; } }

        public ArcaneDust(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}