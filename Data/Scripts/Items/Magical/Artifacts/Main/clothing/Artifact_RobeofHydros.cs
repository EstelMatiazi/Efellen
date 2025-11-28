using System;
using Server;

namespace Server.Items
{
	public class Artifact_RobeofHydros : GiftRobe
	{
		[Constructable]
		public Artifact_RobeofHydros()
		{
			ItemID = 0x0289;
			Name = "Robe of the Lurker";
			Hue = 0x97F;
			Resistances.Cold = 20;
			Attributes.CastRecovery = 1;
			Attributes.CastSpeed = 1;
			Attributes.LowerManaCost = 10;
			Attributes.LowerRegCost = 10;
			SkillBonuses.SetValues(0, SkillName.Elementalism, 20);
			SkillBonuses.SetValues(1, SkillName.Focus, 10);
			SkillBonuses.SetValues(2, SkillName.Meditation, 10);
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Hydros' Enchanted Robe" );
		}

		public Artifact_RobeofHydros( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			ArtifactLevel = 2;
			int version = reader.ReadInt();
		}
	}
}