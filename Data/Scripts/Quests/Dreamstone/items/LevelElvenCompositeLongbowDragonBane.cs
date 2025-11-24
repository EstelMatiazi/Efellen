using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class LevelElvenCompositeLongbowDragonBane : BaseLevelRanged
	{
		private string m_Owner;

		[CommandProperty(AccessLevel.GameMaster)]
		public string Owner
		{
			get { return m_Owner; }
			set { m_Owner = value; InvalidateProperties(); }
		}
		public override int EffectID{ get{ return 0xF42; } }
		public override Type AmmoType{ get{ return typeof( Arrow ); } }
		public override Item Ammo{ get{ return new Arrow(); } }

		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ForceArrow; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.SerpentArrow; } }
		public override WeaponAbility ThirdAbility{ get{ return WeaponAbility.BleedAttack; } }
		public override WeaponAbility FourthAbility{ get{ return WeaponAbility.DoubleShot; } }
		public override WeaponAbility FifthAbility{ get{ return WeaponAbility.ArmorIgnore; } }

		public override int AosStrengthReq{ get{ return 45; } }
		public override int AosMinDamage{ get{ return (int)(17 * GetDamageScaling()); } }
		public override int AosMaxDamage{ get{ return (int)(24 * GetDamageScaling()); } }
		public override int AosSpeed{ get{ return 27; } }
		public override float MlSpeed{ get{ return 4.00f; } }

		public override int OldStrengthReq{ get{ return 45; } }
		public override int OldMinDamage{ get{ return 12; } }
		public override int OldMaxDamage{ get{ return 16; } }
		public override int OldSpeed{ get{ return 27; } }

		public override int DefMaxRange{ get{ return 10; } }

		public override int InitMinHits{ get{ return 41; } }
		public override int InitMaxHits{ get{ return 90; } }

		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.ShootBow; } }

		[Constructable]
		public LevelElvenCompositeLongbowDragonBane(string ownerName) : base( 0x2D1E )
		{
			Weight = 5.0;
			Name = "Dragonbane";
			Layer = Layer.TwoHanded;
			Resource = CraftResource.RegularWood;
			ItemID = 11550;
            Hue = 0x373;
			Slayer = SlayerName.DragonSlaying;
			Attributes.WeaponDamage = 100;
			m_Owner = ownerName;
		}
		[Constructable]
		public LevelElvenCompositeLongbowDragonBane() : this("unknown")
		{
		}
		public override bool OnEquip(Mobile from)
		{
			if (m_Owner != null && m_Owner.Length > 0 && from.Name != m_Owner && from.Skills[SkillName.Tracking].Base < 111.0)
			{
				from.SendMessage("You are not worthy of the Dragonbane.");
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
		public LevelElvenCompositeLongbowDragonBane( Serial serial ) : base( serial )
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