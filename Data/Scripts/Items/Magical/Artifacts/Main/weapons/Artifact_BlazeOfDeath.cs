using System;
using Server;
using Server.CustomEffects;


namespace Server.Items
{
	public class Artifact_BlazeOfDeath : GiftHalberd
	{
		private DateTime m_NextArtifactAttackAllowed;
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_BlazeOfDeath()
		{
			Name = "Blaze of Death";
			Hue = 0x501;
			ItemID = 0x143E;
			WeaponAttributes.HitFireArea = 50;
			Attributes.WeaponSpeed = 20;
			WeaponAttributes.ResistFireBonus = 10;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Fire made metal" );
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

			DotEffect.ApplyDot(defender, duration, attacker,4);

			attacker.SendMessage(33, "The Blaze of death sets your enemy on fire!");
			attacker.PlaySound(0x208);

			m_NextArtifactAttackAllowed = DateTime.UtcNow + TimeSpan.FromMinutes(2);
		}

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			fire = 50;
			phys = 50;

			cold = pois = nrgy = chaos = direct = 0;
		}

		public Artifact_BlazeOfDeath( Serial serial ) : base( serial )
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