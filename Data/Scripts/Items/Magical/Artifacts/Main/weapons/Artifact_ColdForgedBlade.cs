using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class Artifact_ColdForgedBlade : GiftElvenSpellblade
	{
		private DateTime m_NextArtifactBuff;
		private bool m_BuffActive; 

		[Constructable]
		public Artifact_ColdForgedBlade()
		{
			Name = "Cold Forged Blade";
			WeaponAttributes.HitHarm = 40;
			Attributes.SpellChanneling = 1;
			Slayer = SlayerName.Fey;
			Attributes.WeaponSpeed = 20;

			Hue = this.GetElementalDamageHue();
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Bane of Fey kind" );
			m_NextArtifactBuff = DateTime.MinValue;
		}

		public override void OnHit(Mobile attacker, Mobile defender, double damage)
		{
			base.OnHit(attacker, defender, damage);

			if (attacker == null || defender == null)
				return;

			if (defender.Alive || defender.Hits > 0)
				return;

			if (!IsSlayerEffective(defender))
				return;

			if (Utility.RandomDouble() > 0.15)
				return;

			if (DateTime.UtcNow < m_NextArtifactBuff)
				return;

			if (m_BuffActive)
				return;

			ApplyArtifactBuff(attacker);

			attacker.SendMessage(33, "The fallen fey empowers you!");
			attacker.PlaySound(0x1E9);

			m_NextArtifactBuff = DateTime.UtcNow + TimeSpan.FromMinutes(5.0);
		}

		private bool IsSlayerEffective(Mobile m)
		{
			if (Slayer == SlayerName.Fey)
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName(Slayer);

				if (entry != null && entry.Slays(m))
					return true;
			}
			return false;
		}

		private void ApplyArtifactBuff(Mobile m)
		{
			m_BuffActive = true;
			m.AddStatMod(new StatMod(StatType.Int, "ArtifactSlayerInt", 10, TimeSpan.FromMinutes(3)));
			m.AddStatMod(new StatMod(StatType.Dex, "ArtifactSlayerDex", 10, TimeSpan.FromMinutes(3)));
			new ArtifactBuffEndTimer(this, m).Start();
		}

		private class ArtifactBuffEndTimer : Timer
		{
			private Artifact_ColdForgedBlade m_Item;
			private Mobile m_Mobile;

			public ArtifactBuffEndTimer(Artifact_ColdForgedBlade item, Mobile mob)
				: base(TimeSpan.FromMinutes(2.0))
			{
				m_Item = item;
				m_Mobile = mob;
				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
				if (m_Mobile != null)
				{
					m_Mobile.RemoveStatMod("ArtifactSlayerInt");
					m_Mobile.RemoveStatMod("ArtifactSlayerDex");
				}

				if (m_Item != null)
					m_Item.m_BuffActive = false;
			}
		}

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			phys = fire = pois = nrgy = chaos = direct = 0;
			cold = 100;
		}

		public Artifact_ColdForgedBlade( Serial serial ) : base( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);

			writer.Write(m_NextArtifactBuff);
			writer.Write(m_BuffActive);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			ArtifactLevel = 2;

			int version = reader.ReadInt();

			m_NextArtifactBuff = reader.ReadDateTime();
			m_BuffActive = reader.ReadBool();

			if (Slayer == SlayerName.None)
				Slayer = SlayerName.Fey;
		}
	}
}