using System;
using Server.Network;
using Server.Items;
using Server.Engines.Harvest;

namespace Server.Items
{
	public class Artifact_GrimReapersScythe : GiftScythe
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_GrimReapersScythe()
		{
			Hue = 0x47E;
			Name = "Grim Reaper's Scythe";
			ItemID = 0x2690;
			WeaponAttributes.HitLeechHits = 50;
			Attributes.SpellChanneling = 1;
			AccuracyLevel = WeaponAccuracyLevel.Supremely;
 	 	    Slayer = SlayerName.Repond;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Reaps souls." );
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
				else if (extra > 25)
					extra = 25;

                attacker.Heal(extra, attacker);

                attacker.FixedParticles(0x3728, 10, 10, 5052, 0, 0, EffectLayer.Head);
                attacker.PlaySound(0x1F1);
            }

            base.OnHit(attacker, defender, damageBonus);
        }

		public Artifact_GrimReapersScythe( Serial serial ) : base( serial )
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