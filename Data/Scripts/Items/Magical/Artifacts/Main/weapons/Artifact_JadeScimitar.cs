using System;
using Server.Network;
using Server.Items;
using Server.Targeting;
using Server.CustomEffects;

namespace Server.Items
{
	public class Artifact_JadeScimitar : GiftScimitar
	{
		private DateTime m_NextArtifactAttackAllowed;
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_JadeScimitar()
		{
			Name = "Jade Scimitar";
			Hue = 2964;
			ItemID = 0x13B6;
			WeaponAttributes.HitColdArea = 20;
			WeaponAttributes.HitEnergyArea = 20;
			WeaponAttributes.HitFireArea = 20;
			WeaponAttributes.HitPhysicalArea = 20;
			WeaponAttributes.HitPoisonArea = 20;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "" );
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


			DotEffect.ApplyDot(defender, duration, attacker, Utility.RandomMinMax(1, 5));

			attacker.SendMessage(33, "The Jade Scimitar bursts with power!");
			attacker.PlaySound(0x208);

			m_NextArtifactAttackAllowed = DateTime.UtcNow + TimeSpan.FromMinutes(2);
		}

		public Artifact_JadeScimitar( Serial serial ) : base( serial )
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
