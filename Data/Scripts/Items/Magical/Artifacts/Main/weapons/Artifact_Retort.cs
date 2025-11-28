using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class Artifact_Retort : GiftWarFork
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_Retort()
		{
			Name = "Retort";
			Hue = 910;
			WeaponAttributes.HitLeechHits = 40;
			WeaponAttributes.HitLowerDefend = 40;
			Attributes.BonusDex = 10;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Has the last word" );
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
                    int stam = Utility.RandomMinMax(10, 35);
				    attacker.Stam += stam;
				    attacker.SendMessage(33, "Retort saps the will of the fallen enemy!");
                }
            }
        }

		public Artifact_Retort( Serial serial ) : base( serial )
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
