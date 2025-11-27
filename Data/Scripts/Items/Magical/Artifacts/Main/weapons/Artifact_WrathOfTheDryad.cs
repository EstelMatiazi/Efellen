using System;
using Server;

namespace Server.Items
{
	public class Artifact_WrathOfTheDryad : GiftGnarledStaff
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_WrathOfTheDryad()
		{
			Name = "Wrath of the Dryad";
			Hue = 0x29C;
			WeaponAttributes.HitLeechMana = 30;
			WeaponAttributes.HitLightning = 30;
			Attributes.AttackChance = 15;
			Attributes.SpellChanneling = 1;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Nature's wrathful scorn" );
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
                    int mana = Utility.RandomMinMax(15, 35);
                    attacker.Mana += mana;

                    attacker.SendMessage(33, "Wrath empowers you!");
                }
            }
        }

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			pois = 100;

			cold = fire = phys = nrgy = chaos = direct = 0;
		}

		public Artifact_WrathOfTheDryad( Serial serial ) : base( serial )
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
		}
	}
}