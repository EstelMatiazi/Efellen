using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Engines.PartySystem;
using Server.CustomEffects;

namespace Server.Items
{
	public class Artifact_Frostbringer : GiftBow
	{
		private DateTime m_NextArtifactAttackAllowed;
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_Frostbringer()
		{
			Name = "Frostbringer";
			Hue = 0x4F2;
			ItemID = 0x13B2;
			WeaponAttributes.ResistColdBonus = 25;
			Attributes.WeaponDamage = 30;
			Velocity = 15;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Frost dances in its wake" );
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

			double skill = attacker.Skills[SkillName.Marksmanship].Value;
			int duration = 4 + (int)(skill / 25.0);

			if (duration < 4) duration = 4;
			if (duration > 9) duration = 9;

			DotEffect.ApplyDot(defender, duration, attacker,3);

			attacker.SendMessage(33, "The Frostbringer Bow shivers your enemy!");
			attacker.PlaySound(0x208);

			m_NextArtifactAttackAllowed = DateTime.UtcNow + TimeSpan.FromMinutes(2);
		}

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			phys = fire = pois = nrgy = chaos = direct = 0;
			cold = 100;
		}

		public Artifact_Frostbringer( Serial serial ) : base( serial )
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