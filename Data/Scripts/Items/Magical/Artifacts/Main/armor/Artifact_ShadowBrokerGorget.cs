using System;
using Server;

namespace Server.Items
{
	public class Artifact_ShadowBrokerGorget : GiftLeatherGorget
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		public override int BasePhysicalResistance{ get{ return 15; } }
		public override int BasePoisonResistance{ get{ return 17; } }
		public override int BaseEnergyResistance{ get{ return 17; } }

		[Constructable]
		public Artifact_ShadowBrokerGorget()
		{
			Name = "Shadow Broker Gorget";
			Hue = 0x455;
			ItemID = 0x13C7;
			SkillBonuses.SetValues( 0, SkillName.Mercantile, 10.0 );
			SkillBonuses.SetValues( 1, SkillName.Stealing, 10.0 );
			Attributes.BonusDex = 3;
			Attributes.Luck = 105;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, 10, "" );
		}

		public Artifact_ShadowBrokerGorget( Serial serial ) : base( serial )
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