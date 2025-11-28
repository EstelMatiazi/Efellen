using System;
using Server;

namespace Server.Items
{
	public class Artifact_ColdBlood : GiftCleaver
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_ColdBlood()
		{
			Hue = 0x4F2;
			ItemID = 0x2AB6;
			Name = "Cold Blood";
			Attributes.WeaponSpeed = 40;
			Attributes.BonusHits = 10;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Feasts on the fallen" );
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
                    int hits = Utility.RandomMinMax(10, 35);
                    attacker.Hits += hits;
                    attacker.SendMessage(33, "Cold Blood feasts on the fallen enemy!");
                }
            }
        }

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			cold = 100;
			fire = phys = pois = nrgy = chaos = direct = 0;
		}

		public Artifact_ColdBlood( Serial serial ) : base( serial )
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