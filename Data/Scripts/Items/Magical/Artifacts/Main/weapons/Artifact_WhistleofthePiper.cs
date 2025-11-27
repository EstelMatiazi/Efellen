using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class Artifact_WhistleofthePiper : GiftWhips
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_WhistleofthePiper()
		{
			Name = "Whistle of the Pied Piper";
			Hue = 0x668;
			SkillBonuses.SetValues( 1, SkillName.Herding,  20.0);
			WeaponAttributes.HitLeechStam = 40;
            Attributes.WeaponSpeed = 10;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Immobilizes animals" );
		}

		public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

            if (DateTime.Now < m_NextParalyze)
                return;

            bool validTarget = false;
            SlayerEntry animal = SlayerGroup.GetEntryByName(SlayerName.AnimalHunter);
			if (animal != null && animal.Slays(defender))
                validTarget = true;
			if (!validTarget)
                return;

            if (Utility.RandomDouble() < 0.25)
            {
                if (defender != null && defender.Alive && !defender.Paralyzed)
                {
                    defender.Paralyze(TimeSpan.FromSeconds(5));
                    attacker.SendMessage("Your blow immobilizes your foe!");
                    m_NextParalyze = DateTime.Now + TimeSpan.FromSeconds(30);
                }
            }
        }

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			cold = 100;

			pois = fire = phys = nrgy = chaos = direct = 0;
		}

		public Artifact_WhistleofthePiper( Serial serial ) : base( serial )
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