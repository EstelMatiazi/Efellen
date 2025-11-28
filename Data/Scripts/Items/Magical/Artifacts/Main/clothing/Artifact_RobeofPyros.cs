using System;
using Server;

namespace Server.Items
{
	public class Artifact_RobeofPyros : GiftRobe
	{
		[Constructable]
		public Artifact_RobeofPyros()
		{
			ItemID = 0x2B69;
			Name = "Robe of the Daemon King";
			Hue = 0x981;
			Resistances.Fire = 20;
			Attributes.CastRecovery = 1;
			Attributes.CastSpeed = 1;
			Attributes.LowerManaCost = 10;
			Attributes.LowerRegCost = 10;
			SkillBonuses.SetValues(0, SkillName.Elementalism, 20);
			SkillBonuses.SetValues(1, SkillName.Focus, 10);
			SkillBonuses.SetValues(2, SkillName.Meditation, 10);
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Pyros' Vile Robe" );
		}

		public Artifact_RobeofPyros( Serial serial ) : base( serial )
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