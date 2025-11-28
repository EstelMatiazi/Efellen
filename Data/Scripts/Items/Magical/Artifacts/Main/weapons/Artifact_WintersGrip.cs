using System;
using Server;
using Server.CustomEffects;

namespace Server.Items
{
	public class Artifact_WintersGrip : GiftWarMace
	{
		private DateTime m_NextArtifactAttackAllowed;
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_WintersGrip()
		{
			Name = "Winter's Grip";
			Hue = 0xB3E;
			ItemID = 0x1407;
			WeaponAttributes.HitHarm = 50;
			Attributes.WeaponSpeed = 30;
			WeaponAttributes.ResistColdBonus = 10;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Cold unending" );
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

			DotEffect.ApplyDot(defender, duration, attacker,3);

			attacker.SendMessage(33, "The Winter's grip shivers your enemy!");
			attacker.PlaySound(0x208);

			m_NextArtifactAttackAllowed = DateTime.UtcNow + TimeSpan.FromMinutes(2);
		}

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			cold = 100;
			phys = pois = fire = nrgy = chaos = direct = 0;
		}

		public Artifact_WintersGrip( Serial serial ) : base( serial )
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