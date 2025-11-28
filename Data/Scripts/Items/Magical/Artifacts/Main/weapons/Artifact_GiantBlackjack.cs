using System;
using Server;
using Server.CustomEffects;

namespace Server.Items
{
	public class Artifact_GiantBlackjack : GiftClub
	{
		private DateTime m_NextArtifactAttackAllowed;
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_GiantBlackjack()
		{
			Hue = 0x497;
			ItemID = 0x13B4;
			Name = "Giant Blackjack";
			Attributes.BonusStr = 25;
			SkillBonuses.SetValues( 0, SkillName.Bludgeoning, 20 );
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Shatters foes" );
			m_NextArtifactAttackAllowed = DateTime.MinValue;
		}

		public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
		{
			base.OnHit(attacker, defender, damageBonus);

			if (attacker == null || defender == null || defender.Deleted)
				return;

			if (DateTime.UtcNow < m_NextArtifactAttackAllowed)
				return;

			if (Utility.RandomDouble() > 0.15)
				return;

			double skill = attacker.Skills[SkillName.Swords].Value;
			int duration = 4 + (int)(skill / 25.0);

			if (duration < 4) duration = 4;
			if (duration > 9) duration = 9;

			DotEffect.ApplyDot(defender, duration, attacker,1);

			attacker.SendMessage(33, "The Giant Blackjack shatters your enemy!");
			attacker.PlaySound(0x208);

			m_NextArtifactAttackAllowed = DateTime.UtcNow + TimeSpan.FromMinutes(2);
		}


		public Artifact_GiantBlackjack( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
			writer.Write(m_NextArtifactAttackAllowed);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			if (version >= 0)
				m_NextArtifactAttackAllowed = reader.ReadDateTime();

			ArtifactLevel = 2;
		}
	}
}