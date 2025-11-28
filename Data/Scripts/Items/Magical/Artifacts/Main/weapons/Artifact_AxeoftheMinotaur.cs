using System;
using Server;
using Server.CustomEffects;

namespace Server.Items
{
	public class Artifact_AxeoftheMinotaur : GiftLargeBattleAxe
	{
		private DateTime m_NextArtifactAttackAllowed;
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_AxeoftheMinotaur()
		{
			Hue = 0x485;
			Name = "Axe of the Minotaur";
			ItemID = 0x13FB;
			Attributes.AttackChance = 10;
			Attributes.WeaponDamage = 10;
			WeaponAttributes.HitPhysicalArea = 50;
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

			attacker.SendMessage(33, "The Axe of the Minotaur shatters your enemy!");
			attacker.PlaySound(0x208);

			m_NextArtifactAttackAllowed = DateTime.UtcNow + TimeSpan.FromMinutes(2);
		}

		public Artifact_AxeoftheMinotaur( Serial serial ) : base( serial )
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