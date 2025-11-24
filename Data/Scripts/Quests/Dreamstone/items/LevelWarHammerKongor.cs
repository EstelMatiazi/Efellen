using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class LevelWarHammerKongor : BaseLevelBashing
	{
		private string m_Owner;

		[CommandProperty(AccessLevel.GameMaster)]
		public string Owner
		{
			get { return m_Owner; }
			set { m_Owner = value; InvalidateProperties(); }
		}

		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.WhirlwindAttack; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.CrushingBlow; } }
		public override WeaponAbility ThirdAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		public override WeaponAbility FourthAbility{ get{ return WeaponAbility.DoubleStrike; } }
		public override WeaponAbility FifthAbility{ get{ return WeaponAbility.DeathBlow; } }

		public override int AosStrengthReq{ get{ return 125; } }
		public override int AosMinDamage{ get{ return (int)(22 * GetDamageScaling()); } }
		public override int AosMaxDamage{ get{ return (int)(26 * GetDamageScaling()); } }
		public override int AosSpeed{ get{ return 28; } }
		public override float MlSpeed{ get{ return 3.75f; } }

		public override int OldStrengthReq{ get{ return 40; } }
		public override int OldMinDamage{ get{ return 8; } }
		public override int OldMaxDamage{ get{ return 36; } }
		public override int OldSpeed{ get{ return 31; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 110; } }

		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Bash2H; } }

		[Constructable]
		public LevelWarHammerKongor(string ownerName) : base( 0x267C )
		{
			Weight = 10.0;
            Hue = 0x373;
			Layer = Layer.TwoHanded;
			Name = "Kongor's Undying Rage";
			ItemID = 0x267C;
            Attributes.WeaponDamage = 50;
            WeaponAttributes.HitPhysicalArea = 100;
			m_Owner = ownerName;
		}
		[Constructable]
		public LevelWarHammerKongor() : this("unknown")
		{
		}

		public override bool OnEquip(Mobile from)
		{
			if (m_Owner != null && m_Owner.Length > 0 && from.Name != m_Owner && from.Skills[SkillName.MagicResist].Base < 111)
			{
				from.SendMessage("You are not worthy of Kongor's Undying Rage.");
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

		public LevelWarHammerKongor( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
			writer.Write((string)m_Owner);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			if (version >= 1)
				m_Owner = reader.ReadString();
		}
	}
}