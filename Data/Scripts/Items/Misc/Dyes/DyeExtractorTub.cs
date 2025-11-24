using System;
using Server;
using Server.Targeting;
using Server.Network;

namespace Server.Items
{
    public class DyeExtractorTub : DyeTub
    {
        private int m_ExtractedHue;

        [CommandProperty(AccessLevel.GameMaster)]
        public int ExtractedHue
        {
            get { return m_ExtractedHue; }
            set 
            { 
                m_ExtractedHue = value; 
                Hue = value;
                DyedHue = value;
                InvalidateProperties();
            }
        }

        [Constructable]
        public DyeExtractorTub()
        {
            m_ExtractedHue = 0;
            Hue = 0;
            Name = "Dye Extractor Tub";
            Redyable = false;
        }

        public DyeExtractorTub(Serial serial) : base(serial)
        {
        }
        public override string DefaultDescription{ get{ return "This item can consume a piece of clothing to extract the color from it, which can then be transfered to another dye tub."; } }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);
            
            if (m_ExtractedHue > 0)
            {
                list.Add(1060658, "Extracted Color\t{0}", m_ExtractedHue.ToString());
            }
            else
            {
                list.Add(1070722, "Empty - Ready to Extract");
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 2))
            {
                if (m_ExtractedHue > 0)
                {
                    from.SendMessage("Select another dye tub to transfer the extracted color to.");
                    from.Target = new TransferTarget(this);
                }
                else
                {
                    from.SendMessage("Select an item to extract color from. The item will be destroyed.");
                    from.Target = new ExtractTarget(this);
                }
            }
            else
            {
                from.SendLocalizedMessage(500446); 
            }
        }

        private class ExtractTarget : Target
        {
            private DyeExtractorTub m_Tub;

            public ExtractTarget(DyeExtractorTub tub) : base(1, false, TargetFlags.None)
            {
                m_Tub = tub;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Tub == null || m_Tub.Deleted)
                    return;

                if (m_Tub.ExtractedHue > 0)
                {
                    from.SendMessage("This tub already contains an extracted color. Transfer it first.");
                    return;
                }

                Item item = targeted as Item;

                if (item == null)
                {
                    from.SendMessage("You can only extract color from an item.");
                    return;
                }

                if (!item.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(502437); // Must be in backpack
                    return;
                }

                if (item.Hue == 0)
                {
                    from.SendMessage("This item has no color to extract.");
                    return;
                }

                if (item is DyeTub || item is DyeExtractorTub)
                {
                    from.SendMessage("You cannot extract color from a dye tub.");
                    return;
                }

                // Extract color and destroy item
                int extractedHue = item.Hue;
                item.Delete();

                m_Tub.ExtractedHue = extractedHue;

                from.SendMessage("Color extracted successfully! The item has been consumed.");
                from.SendMessage("Now select another dye tub to apply this color.");
            }
        }

        private class TransferTarget : Target
        {
            private DyeExtractorTub m_SourceTub;

            public TransferTarget(DyeExtractorTub tub) : base(1, false, TargetFlags.None)
            {
                m_SourceTub = tub;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_SourceTub == null || m_SourceTub.Deleted)
                    return;

                if (m_SourceTub.ExtractedHue == 0)
                {
                    from.SendMessage("This tub does not contain any extracted color.");
                    return;
                }

                DyeTub targetTub = targeted as DyeTub;

                if (targetTub == null)
                {
                    from.SendMessage("You must select a dye tub.");
                    return;
                }

                if (!targetTub.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(502437); 
                    return;
                }

                if (targetTub is DyeExtractorTub)
                {
                    from.SendMessage("You cannot transfer color to another extractor tub.");
                    return;
                }

                int colorToTransfer = m_SourceTub.ExtractedHue;

                targetTub.Hue = colorToTransfer;
                targetTub.DyedHue = colorToTransfer;

                m_SourceTub.ExtractedHue = 0;

                from.SendMessage("Color successfully transferred!");
                from.PlaySound(0x23E);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
            writer.Write(m_ExtractedHue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
                m_ExtractedHue = reader.ReadInt();
            else
                m_ExtractedHue = 0;
        }
    }
}
