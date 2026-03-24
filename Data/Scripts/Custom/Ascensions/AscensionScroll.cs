using System;
using Server;
using Server.Items;

namespace Server.Custom.Ascensions
{
    public class AscensionScroll : Item
    {
        private AscensionType m_Ascension;

        [CommandProperty(AccessLevel.GameMaster)]
        public AscensionType Ascension
        {
            get { return m_Ascension; }
            set { m_Ascension = value; Hue = GetHueForAscension(value); InvalidateProperties(); }
        }

        [Constructable]
        public AscensionScroll() : this(AscensionScrollFactory.GetRandom())
        {
        }

        [Constructable]
        public AscensionScroll(AscensionType type) : base(0x2D9E)
        {
            Weight    = 0.2;
            LootType  = LootType.Regular;
            Stackable = true;
            Amount    = 1;
            m_Ascension = type;
            Hue  = GetHueForAscension(type);
            Name = AscensionTypeHelper.GetDisplayName(type) + " Ascension Scroll";
        }

        public override string DefaultDescription{ get{ return "Pergaminhos de ascensão são uma forma de moeda usada para desbloquear habilidades poderosas. Cada pergaminho está ligado a uma forma específica de ascensão, e cada ascensão tem seu próprio conjunto de pré-requisitos. Use [Ascension para mais informações."; } }

        public AscensionScroll(Serial serial) : base(serial)
        {
        }

        private static int GetHueForAscension(AscensionType type)
        {
            switch (type)
            {
                case AscensionType.Berserker:       return 0x0F1;
                case AscensionType.Archmage:        return 0x213;
                case AscensionType.Palemaster:      return 0xB97;
                case AscensionType.Crusader:        return 0x0F8;
                case AscensionType.Assassin:        return 0x233;
                case AscensionType.Blackguard:      return 0x47E;
                case AscensionType.Skald:           return 0x445;
                case AscensionType.Reaver:          return 0x675;
                case AscensionType.Kensai:          return 0x448;
                case AscensionType.Hierophant:      return 0x439;
                case AscensionType.ArcaneArcher:    return 0x48F;
                default:                            return 0;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Necessário para desbloquear e avançar na Ascensão " + AscensionTypeHelper.GetDisplayName(m_Ascension) + ".");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write((int)m_Ascension);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Ascension = (AscensionType)reader.ReadInt();
            Hue = GetHueForAscension(m_Ascension);
        }
    }
}