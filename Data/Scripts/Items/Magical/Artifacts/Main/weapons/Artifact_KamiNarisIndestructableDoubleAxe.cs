using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class Artifact_KamiNarisIndestructableDoubleAxe : GiftDoubleAxe
	{
		public override int InitMinHits{ get{ return 250; } }
		public override int InitMaxHits{ get{ return 250; } }

		[Constructable]
		public Artifact_KamiNarisIndestructableDoubleAxe()
		{
			Name = "Kami-Naris Indestructable Axe";
			Hue = 1161;
			ItemID = 0xF4B;
			WeaponAttributes.HitFireArea = 30;
			WeaponAttributes.HitHarm = 20;
			WeaponAttributes.HitLightning = 30;
			WeaponAttributes.SelfRepair = 10;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "" );
		}

		public Artifact_KamiNarisIndestructableDoubleAxe( Serial serial ) : base( serial )
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
