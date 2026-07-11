using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Gumps;

namespace Server.Items
{
    public class ArtifactBook : Container
    {
        public override int DefaultMaxItems { get { return 10000; } }

        public override int MaxWeight { get { return 0; } }

        public override bool DisplaysContent { get { return false; } }

        [Constructable]
        public ArtifactBook() : base(0xFF4)
        {
            Name = "Artifact Codex";
            Hue = 1154;
            LootType = LootType.Blessed;
        }

        public ArtifactBook(Serial serial) : base(serial) { }

		public override int GetTotal( TotalType type )
		{
			return 0;
		}

        public override void OnDoubleClick(Mobile from)
        {
            from.CloseGump(typeof(ArtifactBookGump));
            from.SendGump(new ArtifactBookGump(from, this, 0, ""));
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped == null || dropped.Deleted || dropped == this || dropped.ArtifactLevel == 0)
                return false;
            
            if(this.Items.FindIndex(x => x.Name == dropped.Name) != -1)
            {
                from.SendMessage("This artifact is already stored in the book.");
                return false;
            }

            if (!base.OnDragDrop(from, dropped))
                return false;

            from.SendMessage("Artifact stored.");

            if (from.HasGump(typeof(ArtifactBookGump)))
            {
                from.CloseGump(typeof(ArtifactBookGump));
                from.SendGump(new ArtifactBookGump(from, this, 0, ""));  
            }

            return true;
        }

        public bool Retrieve(Mobile from, Item entry)
        {
            if (entry == null || entry.Deleted)
                return false;

            if (from.Backpack == null)
                return false;

            from.Backpack.DropItem(entry);

            from.SendMessage("Artifact retrieved.");
            return true;
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