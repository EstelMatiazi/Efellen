using System;
using Server;
using Server.Spells.Sixth;
using Server.Targeting;

namespace Server.Items
{
	public class Artifact_CloakOfTheRogue : GiftCloak
	{
        public DateTime TimeUsed;

		[Constructable]
		public Artifact_CloakOfTheRogue()
		{
			Name = "Cloak of the Rogue";
			Hue = 0x967;
			SkillBonuses.SetValues( 0, SkillName.Stealing, 25 );
			SkillBonuses.SetValues( 1, SkillName.Snooping, 25 );
			SkillBonuses.SetValues( 2, SkillName.RemoveTrap, 50 );
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Casts Invisibility" );
		}

		public override void OnDoubleClick( Mobile from )
		{
            DateTime TimeNow = DateTime.Now;
			long ticksThen = TimeUsed.Ticks;
			long ticksNow = TimeNow.Ticks;
			int minsThen = (int)TimeSpan.FromTicks(ticksThen).TotalMinutes;
			int minsNow = (int)TimeSpan.FromTicks(ticksNow).TotalMinutes;
			int CanUseMagic = 15 - ( minsNow - minsThen );

			if ( Parent != from )
			{
				from.SendMessage( "You must be wearing the cloak to use its power." );
			}
            else if ( CanUseMagic > 0 )
			{
				TimeSpan t = TimeSpan.FromMinutes( CanUseMagic );
				string wait = string.Format("{0:D1} hours and {1:D2} minutes", 
								t.Hours, 
								t.Minutes);
				from.SendMessage( "You can use the magic in " + wait + "." );
			}
			else
			{
				new InvisibilitySpell( from, this ).Cast();
				TimeUsed = DateTime.Now;
			}
			return;
		}

		public Artifact_CloakOfTheRogue( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			ArtifactLevel = 2;
			int version = reader.ReadInt();
		}
	}
}