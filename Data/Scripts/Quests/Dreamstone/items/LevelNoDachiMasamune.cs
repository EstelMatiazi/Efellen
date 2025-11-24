using System;
using Server;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute(0x27A2, 0x27ED)]
	public class LevelNoDachiMasamune : BaseLevelSword
	{
		private string m_Owner;

		[CommandProperty(AccessLevel.GameMaster)]
		public string Owner
		{
			get { return m_Owner; }
			set { m_Owner = value; InvalidateProperties(); }
		}

		public override WeaponAbility PrimaryAbility { get { return WeaponAbility.WhirlwindAttack; } }
		public override WeaponAbility SecondaryAbility { get { return WeaponAbility.ArmorIgnore; } }
		public override WeaponAbility ThirdAbility { get { return WeaponAbility.DeathBlow; } }
		public override WeaponAbility FourthAbility { get { return WeaponAbility.DefenseMastery; } }
		public override WeaponAbility FifthAbility { get { return WeaponAbility.MagicProtection2; } }

		public override int AosStrengthReq { get { return 40; } }
		public override int AosMinDamage { get { return (int)(21 * GetDamageScaling()); } }
		public override int AosMaxDamage { get { return (int)(26 * GetDamageScaling()); } }
		public override int AosSpeed { get { return 35; } }
		public override float MlSpeed { get { return 3.50f; } }

		public override int OldStrengthReq { get { return 40; } }
		public override int OldMinDamage { get { return 16; } }
		public override int OldMaxDamage { get { return 18; } }
		public override int OldSpeed { get { return 35; } }

		public override int DefHitSound { get { return 0x23B; } }
		public override int DefMissSound { get { return 0x23A; } }

		public override int InitMinHits { get { return 31; } }
		public override int InitMaxHits { get { return 90; } }

		[Constructable]
		public LevelNoDachiMasamune(string ownerName) : base(0x27A2)
		{
			Name = "Masamune";
			Hue = 0x373;
			Weight = 10.0;
			Layer = Layer.TwoHanded;
			Slayer = SlayerName.Repond;
			Attributes.WeaponDamage = 100;
			m_Owner = ownerName;
		}

		[Constructable]
		public LevelNoDachiMasamune() : this("unknown")
		{
		}

		public override bool OnEquip(Mobile from)
		{
			if (m_Owner != null && m_Owner.Length > 0 && from.Name != m_Owner && from.Karma < 0)
			{
				from.SendMessage("You are not worthy of the Masamune.");
				return false;
			}

			return base.OnEquip(from);
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (m_Owner != null && m_Owner.Length > 0 && m_Owner != "unknown")
				list.Add("Belongs to {0}", m_Owner);
		}

		public LevelNoDachiMasamune(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)1); // version
			writer.Write((string)m_Owner);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			if (version >= 1)
				m_Owner = reader.ReadString();
		}
	}
}
