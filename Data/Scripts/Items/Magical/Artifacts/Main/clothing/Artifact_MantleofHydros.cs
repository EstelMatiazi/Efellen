using System;
using Server;

namespace Server.Items
{
	public class Artifact_MantleofHydros : GiftWizardsHat
	{
		[Constructable]
		public Artifact_MantleofHydros()
		{
			ItemID = 0x5C14;
			Name = "Mantle of the Lurker";
			Hue = 0x97F;
			Resistances.Cold = 20;
			Attributes.CastRecovery = 1;
			Attributes.CastSpeed = 1;
			Attributes.LowerManaCost = 5;
			Attributes.LowerRegCost = 5;
			Attributes.RegenStam = 5;
			SkillBonuses.SetValues(0, SkillName.Elementalism, 10);
			SkillBonuses.SetValues(1, SkillName.Focus, 10);
			SkillBonuses.SetValues(2, SkillName.Meditation, 10);
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Hydros' Enchanted Hood" );
		}

		public Artifact_MantleofHydros( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			ArtifactLevel = 2;
			int version = reader.ReadInt();
		}
	}
}