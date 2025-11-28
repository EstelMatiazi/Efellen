using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class Artifact_BladeDance : GiftRuneBlade
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_BladeDance()
		{
			Name = "Blade Dance";
			Hue = 0x66C;
			Attributes.BonusMana = 10;
			Attributes.SpellChanneling = 1;
			Attributes.WeaponDamage = 30;
			WeaponAttributes.HitLeechMana = 20;
			WeaponAttributes.UseBestSkill = 1;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Drains energy" );
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
                    int mana = Utility.RandomMinMax(10, 35);
				    attacker.Mana += mana;
				    attacker.SendMessage(33, "Blade Dance devour the energy of the fallen enemy!");
                }
            }
        }

		public Artifact_BladeDance( Serial serial ) : base( serial )
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