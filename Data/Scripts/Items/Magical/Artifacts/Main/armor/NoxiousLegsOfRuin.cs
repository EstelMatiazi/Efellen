
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class NoxiousLegsOfRuin : GiftDragonLegs
	{
		[Constructable]
		public NoxiousLegsOfRuin()
		{
            Name = "Noxious Legs of Ruin";
			Hue = 1285;
			SkillBonuses.SetValues( 0, SkillName.Fencing, 10);
			SkillBonuses.SetValues( 1, SkillName.Poisoning, 10);
			Attributes.BonusStr = 10;
			Attributes.BonusStam = 10;
			Attributes.DefendChance = 15;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "" );
		}

		public NoxiousLegsOfRuin( Serial serial ) : base( serial )
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
