using System;
using Server;
using Server.CustomEffects;

namespace Server.Items
{
	public class Artifact_FangOfRactus : GiftKryss
	{
		private DateTime m_NextArtifactAttackAllowed;
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		public override bool CanFortify{ get{ return false; } }

		[Constructable]
		public Artifact_FangOfRactus()
		{
			Name = "Fang of Ractus";
			Hue = 0x117;

			Attributes.SpellChanneling = 1;
			Attributes.AttackChance = 10;
			SkillBonuses.SetValues( 0, SkillName.Poisoning, 20 );
			WeaponAttributes.HitPoisonArea = 20;
			WeaponAttributes.ResistPoisonBonus = 15;
			Slayer = SlayerName.Repond;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "The blade oozes with malice" );
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

			double skill = attacker.Skills[SkillName.Poisoning].Value;
			int duration = 4 + (int)(skill / 25.0);

			if (duration < 4) duration = 4;
			if (duration > 9) duration = 9;

			DotEffect.ApplyDot(defender, duration, attacker, 4);

			attacker.SendMessage(33, "The Fang of Ractus envenoms your enemy!");
			attacker.PlaySound(0x208);

			m_NextArtifactAttackAllowed = DateTime.UtcNow + TimeSpan.FromMinutes(2);
		}

		public Artifact_FangOfRactus( Serial serial ) : base( serial )
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
