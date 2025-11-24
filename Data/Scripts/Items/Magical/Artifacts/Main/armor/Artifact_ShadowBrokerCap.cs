using System;
using Server;

namespace Server.Items
{
	public class Artifact_ShadowBrokerCap : GiftLeatherCap
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		public override int BasePhysicalResistance{ get{ return 10; } }
		public override int BasePoisonResistance{ get{ return 13; } }
		public override int BaseEnergyResistance{ get{ return 13; } }

		[Constructable]
		public Artifact_ShadowBrokerCap()
		{
			Name = "Shadow Broker Cap";
			Hue = 0x455;
			SkillBonuses.SetValues( 0, SkillName.Mercantile, 10.0 );
			SkillBonuses.SetValues( 1, SkillName.Stealing, 10.0 );
			Attributes.BonusDex = 5;
			Attributes.Luck = 95;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, 10, "" );
		}

		public Artifact_ShadowBrokerCap( Serial serial ) : base( serial )
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