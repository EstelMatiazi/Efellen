using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class Artifact_SoulSeeker : GiftRadiantScimitar
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_SoulSeeker()
		{
			Name = "Soul Seeker";
			Hue = 0x38C;

			WeaponAttributes.HitLeechStam = 30;
			WeaponAttributes.HitLeechMana = 30;
			WeaponAttributes.HitLeechHits = 30;
			Slayer = SlayerName.Repond;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Soulseeker Devours the weak." );
		}

		public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            if (attacker == null || defender == null)
                return;

            if (defender.Hits > 0 && defender.Hits < (defender.HitsMax / 10))
            {
                int extra = (int)(defender.HitsMax * 0.25);
                if (extra < 1)
                    extra = 1;
				if (extra > 50)
					extra = 50;

                defender.Damage(extra, attacker);

                attacker.FixedParticles(0x3728, 10, 10, 5052, 0, 0, EffectLayer.Head);
                attacker.PlaySound(0x1F1);
            }

            base.OnHit(attacker, defender, damageBonus);
        }

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			cold = 100;

			pois = fire = phys = nrgy = chaos = direct = 0;
		}

		public Artifact_SoulSeeker( Serial serial ) : base( serial )
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