using System;
using Server;
using Server.CustomEffects;

namespace Server.Items
{
	public class Artifact_WildfireBow : GiftElvenCompositeLongbow
	{
		private DateTime m_NextWildfireAllowed;
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_WildfireBow() : base()
		{
			Hue = 0x489;
			Name = "Wildfire Bow";
			ItemID = 0x2D1E;
			
			SkillBonuses.SetValues( 0, SkillName.Marksmanship, 10 );
			WeaponAttributes.ResistFireBonus = 25;
			Attributes.WeaponDamage = 29;
			Velocity = 15;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "" );
			m_NextWildfireAllowed = DateTime.MinValue;
		}

		public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
		{
			base.OnHit(attacker, defender, damageBonus);

			if (attacker == null || defender == null || defender.Deleted)
				return;

			if (DateTime.UtcNow < m_NextWildfireAllowed)
				return;

			if (Utility.RandomDouble() > 0.15)
				return;

			double skill = attacker.Skills[SkillName.Marksmanship].Value;
			int duration = 4 + (int)(skill / 25.0);

			if (duration < 4) duration = 4;
			if (duration > 9) duration = 9;

			BurningEffect.ApplyBurn(defender, duration, attacker);

			attacker.SendMessage(33, "The Wildfire Bow sets your enemy ablaze!");
			attacker.PlaySound(0x208);

			m_NextWildfireAllowed = DateTime.UtcNow + TimeSpan.FromMinutes(1);
		}

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			phys = cold = pois = nrgy = chaos = direct = 0;
			fire = 100;
		}

		public Artifact_WildfireBow( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
			writer.Write(m_NextWildfireAllowed);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			if (version >= 0)
				m_NextWildfireAllowed = reader.ReadDateTime();

			ArtifactLevel = 2;
		}
	}
}
