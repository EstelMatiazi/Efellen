using System;
using Server;
using Server.Items;
using System.Text;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	public class DesmonaDiary : Item
	{
		[Constructable]
		public DesmonaDiary( ) : base( 0xE34 )
		{
			Weight = 1.0;
			Hue = 0x0213;
			Name = "a dusty scroll";
			ItemID = 0x14EE;
		}

		public class ClueGump : Gump
		{
			public ClueGump( Mobile from ): base( 100, 100 )
			{
				from.PlaySound( 0x249 );
                sstring sText = "...Tsareth, meu senhor, meu temido e amado coração—que sofrimento ainda resta antes que eu possa estar contigo novamente?<br>Por longas eras cuidei de nosso santuário e seus sombrios tomos, e ainda assim não posso desfazer o decreto perverso que nos separou, quando a Ordem nos expulsou por nosso amor à magia e um ao outro—temendo a grandeza que estávamos tão perto de alcançar.<br>Então veio aquela calamidade rastejante que corroeu o próprio Tecido, uma maré arruinada que os homens agora ousam não nomear. Naquela hora não me abandonaste, mas com mão deliberada e vontade amorosa ligaste minha alma à não-morte, para que eu fosse poupado de seu hálito devastador. Esta negra bênção foi teu presente para mim, minha mais verdadeira misericórdia.<br>Contudo, nem mesmo a ti a praga poupou por completo. Senti o momento em que tua luz foi arrancada do mundo, e desde aquele dia mantive vigília dentro de nossos salões, guardando nossa biblioteca e nosso lar tanto como tumba quanto templo, para que nenhuma mão profanasse o que outrora foi teu.<br>Serás tu minha ferida aberta, amor?<br>Para cuidar até o fim dos tempos,<br>para atar quando tua ausência me fizer sofrer,<br>acalmar quando tua memória me queimar em carne viva,<br>tocar quando esta fome roedora voltar minha aversão contra mim mesmo?<br>";


				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);

				AddImage(0, 0, 10901, 2786);
				AddImage(0, 0, 10899, 2117);
				AddHtml( 45, 78, 386, 218, @"<BODY><BASEFONT Color=#d9c781>" + sText + "</BASEFONT></BODY>", (bool)false, (bool)true);
			}

			public override void OnResponse( NetState state, RelayInfo info ) 
			{
				Mobile from = state.Mobile; 
				from.PlaySound( 0x249 );
			}
		}

		public override void OnDoubleClick( Mobile m )
		{
			if ( m.InRange( this.GetWorldLocation(), 2 ) )
			{
				m.SendGump( new ClueGump( m ) );
				m.PlaySound( 0x249 );
			}
			else
			{
				m.SendLocalizedMessage( 502138 ); // That is too far away for you to use
			}
		}

		public DesmonaDiary(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}