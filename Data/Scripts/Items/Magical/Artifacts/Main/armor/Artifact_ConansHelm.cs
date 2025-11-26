using System;
using Server;

namespace Server.Items
{
	public class Artifact_ConansHelm : GiftPlateHelm
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_ConansHelm()
		{
			ItemID = 0x2645;
			Hue = 0x835;
			Name = "Helm of the Cimmerian";

			Attributes.BonusStam = 10;
			SkillBonuses.SetValues( 0, SkillName.MagicResist, 10 );
			Attributes.DefendChance = 20;
			PhysicalBonus = 15;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Conan's Lost Helm - 10% Spell Reflect" );
		}

		public Artifact_ConansHelm( Serial serial ) : base( serial )
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