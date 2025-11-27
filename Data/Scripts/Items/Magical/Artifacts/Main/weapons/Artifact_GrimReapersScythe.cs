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

		public override void OnHit(Mobile attacker, Mobile defender, double damage)
        {
            base.OnHit(attacker, defender, damage);

            if (attacker == null || defender == null)
                return;

            if (!defender.Alive || defender.Hits <= 0)
            {
                if (Utility.Random(100) < 15)
                {
                    int hits = Utility.RandomMinMax(5, 25);
                    int mana = Utility.RandomMinMax(5, 15);

                    attacker.Hits += hits;
                    attacker.Mana += mana;

                    attacker.SendMessage(33, "Grim Reaper's Scythe devours the enemy's soul!");
                }
            }
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