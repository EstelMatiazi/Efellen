using System;
using Server;

namespace Server.Items
{
	public class Artifact_VampireKiller : GiftWhips
	{
        private DateTime m_NextParalyze;
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_VampireKiller()
		{
			Hue = 0x986;
			Name = "Vampire Killer";
			ItemID = 0x6453;
			Attributes.BonusStr = 10;
			Attributes.AttackChance = 10;
            Slayer = SlayerName.Silver;
            Slayer2 = SlayerName.Exorcism;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Belmonts's Whip - chance to immobilize undead on hit" );
            m_NextParalyze = DateTime.MinValue;
		}

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

            if (DateTime.Now < m_NextParalyze)
                return;

            bool validTarget = false;
            SlayerEntry silver = SlayerGroup.GetEntryByName(SlayerName.Silver);
            SlayerEntry exorcism = SlayerGroup.GetEntryByName(SlayerName.Exorcism);
            if (silver != null && silver.Slays(defender))
                validTarget = true;
            if (exorcism != null && exorcism.Slays(defender))
                validTarget = true;
            if (!validTarget)
                return;

            if (Utility.RandomDouble() < 0.25)
            {
                if (defender != null && defender.Alive && !defender.Paralyzed)
                {
                    defender.Paralyze(TimeSpan.FromSeconds(6));
                    attacker.SendMessage("Your whip immobilizes your foe!");
                    m_NextParalyze = DateTime.Now + TimeSpan.FromSeconds(30);
                }
            }
        }

		public Artifact_VampireKiller( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
            writer.Write(m_NextParalyze);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			ArtifactLevel = 2;
			int version = reader.ReadInt();
            if (version >= 1)
                m_NextParalyze = reader.ReadDateTime();
		}
	}
}