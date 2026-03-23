
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class NoxiousHelmOfRuin : GiftDragonHelm
	{
		[Constructable]
		public NoxiousHelmOfRuin()
		{
            Name = "Noxious Helm of Ruin";
			Hue = 1285;
			SkillBonuses.SetValues( 0, SkillName.MagicResist, 10);
			SkillBonuses.SetValues( 1, SkillName.Poisoning, 10);
			Attributes.BonusDex = 10;
			Attributes.WeaponDamage = 20;
			Attributes.BonusStam = 10;
			Attributes.WeaponSpeed = 20;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "" );
		}

		public NoxiousHelmOfRuin( Serial serial ) : base( serial )
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
