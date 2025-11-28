using System;
using Server;
using Server.CustomEffects;

namespace Server.Items
{
	public class Artifact_TitansHammer : GiftWarHammer
	{
		private DateTime m_NextArtifactAttackAllowed;
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_TitansHammer()
		{
			Name = "Titan's Hammer";
			Hue = 0x482;
			ItemID = 0x267C;
			WeaponAttributes.HitEnergyArea = 50;
			Attributes.BonusStr = 15;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "" );
			Server.Misc.Arty.ArtySetup( this, "Lightning dances in its wake" );
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

			double skill = attacker.Skills[SkillName.Bludgeoning].Value;
			int duration = 4 + (int)(skill / 25.0);

			if (duration < 4) duration = 4;
			if (duration > 9) duration = 9;

			DotEffect.ApplyDot(defender, duration, attacker,5);

			attacker.SendMessage(33, "The Titan's Hammer cracks your foe with electricity!");
			attacker.PlaySound(0x208);

			m_NextArtifactAttackAllowed = DateTime.UtcNow + TimeSpan.FromMinutes(2);
		}


		public Artifact_TitansHammer( Serial serial ) : base( serial )
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