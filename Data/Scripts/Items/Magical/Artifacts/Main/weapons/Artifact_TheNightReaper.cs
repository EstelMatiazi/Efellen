using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class Artifact_TheNightReaper : GiftRepeatingCrossbow
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_TheNightReaper()
		{
			Name = "Night Reaper";
			ItemID = 0x26CD;
			Hue = 0x41C;

			Slayer = SlayerName.Exorcism;
			Attributes.WeaponSpeed = 25;
			Attributes.WeaponDamage = 25;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Culls undead" );
		}

		public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            if (attacker == null || defender == null)
                return;

			bool validTarget = false;
			SlayerEntry undead = SlayerGroup.GetEntryByName(SlayerName.Silver);
			if (undead != null && undead.Slays(defender))
                validTarget = true;
			if (!validTarget)
                return;

            if (defender.Hits > 0 && defender.Hits < (defender.HitsMax / 9))
            {
                int extra = (int)(defender.HitsMax * 0.25);
                if (extra < 1)
                    extra = 1;
				if (extra > 100)
					extra = 100;

                defender.Damage(extra, attacker);

                attacker.FixedParticles(0x3728, 10, 10, 5052, 0, 0, EffectLayer.Head);
                attacker.PlaySound(0x1F1);
            }

            base.OnHit(attacker, defender, damageBonus);
        }

		public Artifact_TheNightReaper( Serial serial ) : base( serial )
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