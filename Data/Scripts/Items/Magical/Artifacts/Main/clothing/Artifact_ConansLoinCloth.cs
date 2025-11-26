using System;
using Server;

namespace Server.Items
{
	public class Artifact_ConansLoinCloth : GiftBelt
	{
		[Constructable]
		public Artifact_ConansLoinCloth()
		{
			Hue = 0x978;
			ItemID = 0x2B68;
			Name = "Loin Cloth of the Cimmerian";
			Attributes.BonusStr = 10;
			Attributes.Luck = 70;
			SkillBonuses.SetValues( 0, SkillName.MagicResist, 20 );
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Conan's Loin Cloth - 10% Spell Reflect" );
		}

		public Artifact_ConansLoinCloth( Serial serial ) : base( serial )
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