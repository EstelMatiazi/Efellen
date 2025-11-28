using System;
using Server;

namespace Server.Items
{
	public class Artifact_TheDryadBow : GiftBow
	{
		private DateTime m_NextParalyze;
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_TheDryadBow()
		{
			Name = "Dryad Bow";
			ItemID = 0x13B1;
			Hue = 0x48F;
			Attributes.WeaponSpeed = 20;
			WeaponAttributes.ResistPoisonBonus = 20;
			SkillBonuses.SetValues( 0, SkillName.Druidism, 10 );
			SkillBonuses.SetValues( 1, SkillName.Taming, 10 );
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Entangles nature's foes" );
		}

		public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

            if (DateTime.Now < m_NextParalyze)
                return;

            bool validTarget = false;
            SlayerEntry repond = SlayerGroup.GetEntryByName(SlayerName.Repond);
			SlayerEntry undead = SlayerGroup.GetEntryByName(SlayerName.Silver);
			if (repond != null && repond.Slays(defender))
                validTarget = true;
			if (undead != null && undead.Slays(defender))
                validTarget = true;
            if (!validTarget)
                return;

            if (Utility.RandomDouble() < 0.25)
            {
                if (defender != null && defender.Alive && !defender.Paralyzed)
                {
                    defender.Paralyze(TimeSpan.FromSeconds(6));
                    attacker.SendMessage("Your arrow immobilizes your foe!");
                    m_NextParalyze = DateTime.Now + TimeSpan.FromSeconds(30);
                }
            }
        }

		public Artifact_TheDryadBow( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			ArtifactLevel = 2;
			int version = reader.ReadInt();
		}
	}
}
