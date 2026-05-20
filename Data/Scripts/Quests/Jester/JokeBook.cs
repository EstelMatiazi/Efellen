using Server;
using System;
using System.Collections;
using Server.Network;
using Server.Misc;
using Server.Mobiles;

namespace Server.Items
{
	public class JokeBook : Item
	{
		[Constructable]
		public JokeBook() : base( 0x1A98 )
		{
			Weight = 1.0;
			Name = RandomThings.MagicWandOwner() + " Book of Jokes";
			Hue = 0xAFF;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from is PlayerMobile )
			{
				switch ( Utility.RandomMinMax( 0, 8 ) ) 
				{
					case 0: from.PlaySound( from.Female ? 801 : 1073 );		from.Say( "*ri*" );	break;
					case 1: from.PlaySound( from.Female ? 801 : 1073 );		from.Say( "Boa essa!" );	break;
					case 2: from.PlaySound( from.Female ? 801 : 1073 );		from.Say( "Nunca tinha ouvido essa antes!" );	break;
					case 3: from.PlaySound( from.Female ? 801 : 1073 );		from.Say( "Sempre gosto de uma boa risada!" );	break;
					case 4: from.PlaySound( from.Female ? 801 : 1073 );		from.Say( "Isso me fez chorar de rir!" );	break;
					case 5: from.Say( "Não entendi." );							break;
					case 6: from.Say( "O que isso significa?" );				break;
					case 7: from.Say( "Isso é pra ser engraçado?" );			break;
					case 8: from.Say( "Um orc e um elfo entram numa taverna?" );	break;
				}
			}
		}

		public JokeBook( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( ( int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}