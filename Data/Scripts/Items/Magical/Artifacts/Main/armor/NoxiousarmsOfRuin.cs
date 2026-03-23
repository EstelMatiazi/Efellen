
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class NoxiousarmsOfRuin : GiftDragonArms
	{
		[Constructable]
		public NoxiousarmsOfRuin()
		{
            Name = "Noxious Arms of Ruin";
			Hue = 1285;
			SkillBonuses.SetValues( 0, SkillName.Fencing, 10);
			SkillBonuses.SetValues( 1, SkillName.Poisoning, 10);
			Attributes.AttackChance = 10;
			Attributes.WeaponSpeed = 10;
			Attributes.BonusStr = 10;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "" );
		}

		public NoxiousarmsOfRuin( Serial serial ) : base( serial )
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
