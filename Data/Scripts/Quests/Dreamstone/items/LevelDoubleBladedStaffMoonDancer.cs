using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
    public class LevelDoubleBladedStaffMoonDancer : BaseLevelSpear
	{
        private string m_Owner;

		[CommandProperty(AccessLevel.GameMaster)]
		public string Owner
		{
			get { return m_Owner; }
			set { m_Owner = value; InvalidateProperties(); }
		}

		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.WhirlwindAttack; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		public override WeaponAbility ThirdAbility{ get{ return WeaponAbility.TalonStrike; } }
		public override WeaponAbility FourthAbility{ get{ return WeaponAbility.BleedAttack; } }
		public override WeaponAbility FifthAbility{ get{ return WeaponAbility.MagicProtection2; } }

		public override int AosStrengthReq{ get{ return 50; } }
		public override int AosMinDamage{ get{ return (int)(17 * GetDamageScaling()); } }
		public override int AosMaxDamage{ get{ return (int)(21 * GetDamageScaling()); } }
		public override int AosSpeed{ get{ return 49; } }
		public override float MlSpeed{ get{ return 2.25f; } }

		public override int OldStrengthReq{ get{ return 50; } }
		public override int OldMinDamage{ get{ return 12; } }
		public override int OldMaxDamage{ get{ return 13; } }
		public override int OldSpeed{ get{ return 49; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 80; } }

		[Constructable]
		public LevelDoubleBladedStaffMoonDancer(string ownerName) : base( 0x2678 )
		{
			Weight = 9.0;
			Name = "Moon Dancer";
            Layer = Layer.TwoHanded;
            Hue = 0x373;
			ItemID = 0x2678;
            Attributes.WeaponDamage = 35;
            Attributes.WeaponSpeed = 60;
			m_Owner = ownerName;
		}

		[Constructable]
		public LevelDoubleBladedStaffMoonDancer() : this("unknown")
		{
		}
        public override bool OnEquip(Mobile from)
		{
			if (m_Owner != null && m_Owner.Length > 0 && from.Name != m_Owner && from.Skills[SkillName.Tactics].Base < 111.0)
			{
				from.SendMessage("You are not worthy of the Moon Dancer.");
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
        public LevelDoubleBladedStaffMoonDancer( Serial serial ) : base( serial )
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