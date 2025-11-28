using System;
using Server.Network;
using Server.Items;
using Server.CustomEffects;

namespace Server.Items
{
	public class Artifact_LuminousRuneBlade : GiftRuneBlade
	{
		private DateTime m_NextArtifactAttackAllowed;
		[Constructable]
		public Artifact_LuminousRuneBlade()
		{
			Name = "Luminous Rune Blade";

			WeaponAttributes.HitLightning = 40;
			WeaponAttributes.SelfRepair = 10;
			Attributes.WeaponSpeed = 10;
			Attributes.WeaponDamage = 20;
			Hue = this.GetElementalDamageHue();
			ArtifactLevel = 2;
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

			attacker.SendMessage(33, "The Rune Blade cracks your foe with electricity!");
			attacker.PlaySound(0x208);

			m_NextArtifactAttackAllowed = DateTime.UtcNow + TimeSpan.FromMinutes(2);
		}

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			phys = fire = cold = pois = chaos = direct = 0;
			nrgy = 100;
		}

		public Artifact_LuminousRuneBlade( Serial serial ) : base( serial )
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