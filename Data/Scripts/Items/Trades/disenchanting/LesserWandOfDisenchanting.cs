using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Misc;

namespace Server.Items
{
    public class LesserWandOfDisenchanting : Item
    {
        private int m_Charges;

        [Constructable]
        public LesserWandOfDisenchanting() : base(0xDF5)
        {
            Name = "Lesser wand of disenchanting";
            Hue = 33;
            Weight = 1.0;
            m_Charges = 25;
        }

        public LesserWandOfDisenchanting(Serial serial) : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; InvalidateProperties(); }
        }
        
        public override string DefaultDescription{ get{ return "This wand is used to unravel magical items and strip them of their essence. It can target a single item, or a bag or container filled with items, doing so will destroy them, and award you with Arcane Dust that skilled guild crafters can use to enhance items, and that is also of great value to powerful wizards. Artifacts and unidentified items can never be disenchanted."; } }
        
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("charges remaining: {0}", m_Charges);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("The wand must be in your backpack to use it.");
                return;
            }

            if (m_Charges <= 0)
            {
                from.SendMessage("The wand has no remaining charges.");
                return;
            }

            from.SendMessage("Select the item or container you wish to disenchant.");
            from.Target = new DisenchantTarget(this);
        }

        private class DisenchantTarget : Target
        {
            private LesserWandOfDisenchanting m_Wand;

            public DisenchantTarget(LesserWandOfDisenchanting wand) : base(10, false, TargetFlags.None)
            {
                m_Wand = wand;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
              Server.Misc.DisenchantingSystem.HandleTarget(from, targeted as Item, m_Wand);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            m_Charges = reader.ReadInt();
        }
    }
}
