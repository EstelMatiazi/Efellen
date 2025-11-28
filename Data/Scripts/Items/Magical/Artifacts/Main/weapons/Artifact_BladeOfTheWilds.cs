using System;
using Server;

namespace Server.Items
{
	public class Artifact_BladeOfTheWilds : GiftLongsword
	{
		private DateTime m_NextArtifactBuff;
		private bool m_BuffActive;  
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_BladeOfTheWilds()
		{
			Hue = 0x21F;
			Name = "Blade of the Wilds";
			Slayer = SlayerName.Repond;
			WeaponAttributes.HitLeechHits = 25;
			WeaponAttributes.HitDispel = 25;
			Attributes.BonusHits = 10;
			Attributes.WeaponDamage = 10;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "" );
			Server.Misc.Arty.ArtySetup(this, "Bane of civilization");

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

			attacker.SendMessage(33, "The fallen humanoid empowers you!");
			attacker.PlaySound(0x1E9);

			m_NextArtifactBuff = DateTime.UtcNow + TimeSpan.FromMinutes(5.0);
		}

		private bool IsSlayerEffective(Mobile m)
		{
			if (Slayer == SlayerName.Repond)
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
			m.AddStatMod(new StatMod(StatType.Str, "ArtifactSlayerStr", 10, TimeSpan.FromMinutes(3)));
			m.AddStatMod(new StatMod(StatType.Dex, "ArtifactSlayerDex", 10, TimeSpan.FromMinutes(3)));
			new ArtifactBuffEndTimer(this, m).Start();
		}

		private class ArtifactBuffEndTimer : Timer
		{
			private Artifact_BladeOfTheWilds m_Item;
			private Mobile m_Mobile;

			public ArtifactBuffEndTimer(Artifact_BladeOfTheWilds item, Mobile mob)
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
					m_Mobile.RemoveStatMod("ArtifactSlayerStr");
					m_Mobile.RemoveStatMod("ArtifactSlayerDex");
				}

				if (m_Item != null)
					m_Item.m_BuffActive = false;
			}
		}

		public Artifact_BladeOfTheWilds( Serial serial ) : base( serial )
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
				Slayer = SlayerName.Repond;
		}
	}
}