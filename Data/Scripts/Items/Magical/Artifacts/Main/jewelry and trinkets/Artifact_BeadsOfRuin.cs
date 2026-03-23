using System;
using Server;

namespace Server.Items
{
	public class Artifact_BeadsOfRuin : GiftGoldBeadNecklace
	{
		[Constructable]
		public Artifact_BeadsOfRuin()
		{
			Name = "Beads of Ruin";
			Hue = 1285;
			SkillBonuses.SetValues( 0, SkillName.Fencing, 10);
			SkillBonuses.SetValues( 1, SkillName.Poisoning, 10);
			Attributes.WeaponDamage = 20;
			Resistances.Poison = 25;
			Attributes.BonusDex = 8;
			Attributes.RegenStam = 5;
			Attributes.BonusStam = 5;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "" );
		}

		public Artifact_BeadsOfRuin( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			ArtifactLevel = 2;

			int version = reader.ReadInt();

			if ( Hue == 0x12B )
				Hue = 0x554;
		}
	}
}