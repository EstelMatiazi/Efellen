using System;
using Server;
using Server.Spells.Magical;
using Server.Targeting;

namespace Server.Items
{
	public class Artifact_BowofthePhoenix : GiftElvenCompositeLongbow
	{
		public DateTime TimeUsed;
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_BowofthePhoenix()
		{
			Hue = 0x489;
			ItemID = 0x2D1E;
			Name = "Bow of the Phoenix";
			AosElementDamages.Fire = 100; 
			WeaponAttributes.HitFireball = 100;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Calls forth a Phoenix" );
		}

		public override void OnDoubleClick( Mobile from )
		{
			DateTime TimeNow = DateTime.Now;
			long ticksThen = TimeUsed.Ticks;
			long ticksNow = TimeNow.Ticks;
			int minsThen = (int)TimeSpan.FromTicks(ticksThen).TotalMinutes;
			int minsNow = (int)TimeSpan.FromTicks(ticksNow).TotalMinutes;
			int CanUseMagic = 60 - ( minsNow - minsThen );

			if ( Parent != from )
			{
				from.SendMessage( "You must be holding the bow to call a Phoenix." );
			}
			else if ( CanUseMagic > 0 )
			{
				TimeSpan t = TimeSpan.FromMinutes( CanUseMagic );
				string wait = string.Format("{0:D1} hours and {1:D2} minutes", 
								t.Hours, 
								t.Minutes);
				from.SendMessage( "You can use the magic again in " + wait + "." );
			}
			else
			{
				new SummonPhoenixSpell( from, this ).Cast();
				TimeUsed = DateTime.Now;
			}
		}

		public Artifact_BowofthePhoenix( Serial serial ) : base( serial )
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