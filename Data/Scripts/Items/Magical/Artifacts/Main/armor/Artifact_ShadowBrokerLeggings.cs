using System;
using Server;

namespace Server.Items
{
	public class Artifact_ShadowBrokerLeggings : GiftLeatherLegs
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		public override int BasePhysicalResistance{ get{ return 17; } }
		public override int BasePoisonResistance{ get{ return 18; } }
		public override int BaseEnergyResistance{ get{ return 18; } }

		[Constructable]
		public Artifact_ShadowBrokerLeggings()
		{
			Name = "Shadow Broker Leggings";
			ItemID = 0x13D2;
			Hue = 0x455;
			SkillBonuses.SetValues( 0, SkillName.Mercantile, 20.0 );
			SkillBonuses.SetValues( 1, SkillName.Stealing, 20.0 );
			Attributes.Luck = 90;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, 10, "" );
		}

		public Artifact_ShadowBrokerLeggings( Serial serial ) : base( serial )
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