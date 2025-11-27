using System;
using Server;

namespace Server.Items
{
	public class Artifact_TheDragonSlayer : GiftLance
	{
		private DateTime m_NextDragonBuff;
		private bool m_BuffActive;  

		public override int InitMinHits { get { return 80; } }
		public override int InitMaxHits { get { return 160; } }

		[Constructable]
		public Artifact_TheDragonSlayer()
		{
			Name = "Slayer of Dragons";
			Hue = 0x530;
			Slayer = SlayerName.DragonSlaying;
			Attributes.WeaponDamage = 30;
			WeaponAttributes.ResistFireBonus = 30;
			WeaponAttributes.ResistEnergyBonus = 30;
			ArtifactLevel = 2;

			Server.Misc.Arty.ArtySetup(this, "Bane of Dragonkind");

			m_NextDragonBuff = DateTime.MinValue;
		}

		public override void OnHit(Mobile attacker, Mobile defender, double damage)
		{
			base.OnHit(attacker, defender, damage);

			if (attacker == null || defender == null)
				return;

			if (defender.Alive || defender.Hits > 0)
				return;

			if (!IsDragonSlayerEffective(defender))
				return;

			if (Utility.RandomDouble() > 0.15)
				return;

			if (DateTime.UtcNow < m_NextDragonBuff)
				return;

			if (m_BuffActive)
				return;

			ApplyDragonBuff(attacker);

			attacker.SendMessage(33, "The fallen dragon empowers you!");
			attacker.PlaySound(0x1E9);

			m_NextDragonBuff = DateTime.UtcNow + TimeSpan.FromMinutes(5.0);
		}

		private bool IsDragonSlayerEffective(Mobile m)
		{
			if (Slayer == SlayerName.DragonSlaying)
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName(Slayer);

				if (entry != null && entry.Slays(m))
					return true;
			}
			return false;
		}

		private void ApplyDragonBuff(Mobile m)
		{
			m_BuffActive = true;
			m.AddStatMod(new StatMod(StatType.Str, "DragonSlayerStr", 10, TimeSpan.FromMinutes(3)));
			m.AddStatMod(new StatMod(StatType.Dex, "DragonSlayerDex", 10, TimeSpan.FromMinutes(3)));
			new DragonBuffEndTimer(this, m).Start();
		}

		private class DragonBuffEndTimer : Timer
		{
			private Artifact_TheDragonSlayer m_Item;
			private Mobile m_Mobile;

			public DragonBuffEndTimer(Artifact_TheDragonSlayer item, Mobile mob)
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
					m_Mobile.RemoveStatMod("DragonSlayerStr");
					m_Mobile.RemoveStatMod("DragonSlayerDex");
				}

				if (m_Item != null)
					m_Item.m_BuffActive = false;
			}
		}

		public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
		{
			phys = fire = cold = pois = chaos = direct = 0;
			nrgy = 100;
		}

		public Artifact_TheDragonSlayer(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);

			writer.Write(m_NextDragonBuff);
			writer.Write(m_BuffActive);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			ArtifactLevel = 2;

			int version = reader.ReadInt();

			m_NextDragonBuff = reader.ReadDateTime();
			m_BuffActive = reader.ReadBool();

			if (Slayer == SlayerName.None)
				Slayer = SlayerName.DragonSlaying;
		}
	}
}
