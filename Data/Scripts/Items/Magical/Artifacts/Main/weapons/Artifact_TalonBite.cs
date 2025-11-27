using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class Artifact_TalonBite : GiftOrnateAxe
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_TalonBite()
		{
			ItemID = 0x2D34;
			Hue = 0x47E;
			Name = "Talon Bite";
			SkillBonuses.SetValues( 0, SkillName.Tactics, 10.0 );
			SkillBonuses.SetValues( 0, SkillName.MagicResist, 10.0 );
			Attributes.BonusDex = 10;
			Attributes.WeaponSpeed = 20;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Ancient dwarven masterpiece" );
		}
		
		public override void OnHit(Mobile attacker, Mobile defender, double damage)
        {
            base.OnHit(attacker, defender, damage);

            if (attacker == null || defender == null)
                return;

            if (!defender.Alive || defender.Hits <= 0)
            {
                if (Utility.Random(100) < 15)
                {
                    int stam = Utility.RandomMinMax(5, 25);
                    int mana = Utility.RandomMinMax(5, 15);

                    attacker.Stam += stam;
                    attacker.Mana += mana;

                    attacker.SendMessage(33, "The Talon Bite empowers you!");
                }
            }
        }

		public Artifact_TalonBite( Serial serial ) : base( serial )
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