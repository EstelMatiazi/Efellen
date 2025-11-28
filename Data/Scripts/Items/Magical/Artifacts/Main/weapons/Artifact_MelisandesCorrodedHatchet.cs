using System;
using Server.Network;
using Server.Items;
using Server.CustomEffects;

namespace Server.Items
{
	public class Artifact_MelisandesCorrodedHatchet : GiftHatchet
	{
		private DateTime m_NextArtifactAttackAllowed;
		[Constructable]
		public Artifact_MelisandesCorrodedHatchet()
		{
			Hue = 0x494;
			Name = "Melisande's Corroded Hatchet";
			ItemID = 0xF43;
			SkillBonuses.SetValues( 0, SkillName.Lumberjacking, 20.0 );
			Attributes.WeaponSpeed = 30;
			Attributes.WeaponDamage = -50;
			WeaponAttributes.SelfRepair = 10;
			WeaponAttributes.HitPoisonArea = 40;
			WeaponAttributes.HitLowerDefend = 40;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Thirsts for metal to dissolve" );
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

			DotEffect.ApplyDot(defender, duration, attacker,4);

			attacker.SendMessage(33, "The Corroded Hatches rusts your enemy!");
			attacker.PlaySound(0x208);

			m_NextArtifactAttackAllowed = DateTime.UtcNow + TimeSpan.FromMinutes(2);
		}

		public Artifact_MelisandesCorrodedHatchet( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
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