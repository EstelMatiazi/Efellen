using System;
using Server;

namespace Server.Items
{
	public class Artifact_HolyLance : GiftLance
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_HolyLance()
		{
			Name = "Holy Lance";
			Hue = 0x47E;
			SkillBonuses.SetValues( 0, SkillName.Knightship, 10.0 );
			Attributes.BonusStr = 15;
			Attributes.WeaponDamage = 20;
			WeaponAttributes.UseBestSkill = 1;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Culls Evil" );
		}

		public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            if (attacker == null || defender == null)
                return;

            if (defender.Hits > 0 && defender.Hits < (defender.HitsMax / 8) && defender.Karma < 1000)
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

		public Artifact_HolyLance( Serial serial ) : base( serial )
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