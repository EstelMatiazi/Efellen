using System;
using Server;

namespace Server.Items
{
	public class NoxiousScalesOfRuin : GiftScalemailShield
	{
		public override int BasePhysicalResistance{ get{ return 5; } }
		public override int BaseFireResistance{ get{ return 0; } }
		public override int BaseColdResistance{ get{ return 0; } }
		public override int BasePoisonResistance{ get{ return 10; } }
		public override int BaseEnergyResistance{ get{ return 0; } }

		public override int InitMinHits{ get{ return 200; } }
		public override int InitMaxHits{ get{ return 200; } }

		[Constructable]
		public NoxiousScalesOfRuin()
		{
			Name = "Noxious Scales of Ruin";
			Hue = 1285;
			SkillBonuses.SetValues( 0, SkillName.Parry, 10);
			SkillBonuses.SetValues( 1, SkillName.Poisoning, 10);
			Attributes.SpellChanneling = 1;
			Attributes.DefendChance = 10;
			Attributes.BonusDex = 10;
			Attributes.WeaponDamage = 20;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "" );
		}

		public NoxiousScalesOfRuin( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			ArtifactLevel = 2;
			int version = reader.ReadInt();
		}
	}
}