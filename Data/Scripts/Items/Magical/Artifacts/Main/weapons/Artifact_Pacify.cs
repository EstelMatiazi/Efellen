using System;
using Server;

namespace Server.Items
{
	public class Artifact_Pacify : GiftPike
	{
		private DateTime m_NextParalyze;
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		public override bool CanFortify{ get{ return false; } }

		[Constructable]
		public Artifact_Pacify()
		{
			Name = "Pacify";
			Hue = 0x835;

			Attributes.SpellChanneling = 1;
			Attributes.WeaponSpeed = 25;
			WeaponAttributes.HitLeechHits = 50;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Immobilizes foes" );
		}

		public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

            if (DateTime.Now < m_NextParalyze)
                return;


            if (Utility.RandomDouble() < 0.25)
            {
                if (defender != null && defender.Alive && !defender.Paralyzed)
                {
                    defender.Paralyze(TimeSpan.FromSeconds(9));
                    attacker.SendMessage("Your blow immobilizes your foe!");
                    m_NextParalyze = DateTime.Now + TimeSpan.FromSeconds(90);
                }
            }
        }

		public Artifact_Pacify( Serial serial ) : base( serial )
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
