using System;
using Server;

namespace Server.Items
{
	public class Artifact_CaptainQuacklebushsCutlass : GiftCutlass
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_CaptainQuacklebushsCutlass()
		{
			Name = "Captain's Cutlass";
			Hue = 0x66C;
			ItemID = 0x1441;
			Attributes.BonusDex = 15;
			Attributes.AttackChance = 10;
			Attributes.WeaponSpeed = 20;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Powerful criticals" );
		}

		public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
		{
		    if (Utility.RandomDouble() < 0.15)
		    {
		        damageBonus += 0.35;
		        attacker.SendMessage("Your strike pierces through your enemy!");
		        attacker.PlaySound(0x20F);
		    }
		    base.OnHit(attacker, defender, damageBonus);
		}

		public Artifact_CaptainQuacklebushsCutlass( Serial serial ) : base( serial )
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