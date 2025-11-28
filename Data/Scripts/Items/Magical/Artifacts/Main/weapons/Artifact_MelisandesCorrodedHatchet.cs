using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class Artifact_MelisandesCorrodedHatchet : GiftHatchet
	{
		[Constructable]
		public Artifact_MelisandesCorrodedHatchet()
		{
			Hue = 0x494;
			Name = "Melisande's Corroded Hatchet";
			ItemID = 0xF43;
			SkillBonuses.SetValues( 0, SkillName.Lumberjacking, 20.0 );
			Attributes.WeaponSpeed = 30;
			Attributes.WeaponDamage = -50;
			WeaponAttributes.SelfRepair = 10;
			WeaponAttributes.HitPoisonArea = 40;
			WeaponAttributes.HitLowerDefend = 40;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "" );
		}

		public Artifact_MelisandesCorrodedHatchet( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			ArtifactLevel = 2;

			int version = reader.ReadEncodedInt();
		}
	}
}