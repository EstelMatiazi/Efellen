using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a mongbat corpse" )]
	public class MongbatBrute : BaseCreature
	{
		[Constructable]
		public MongbatBrute() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a mongbat brute";
			Body = 39;
			Hue = 0x25;
			BaseSoundID = 422;

			SetStr( 26, 30 );
			SetDex( 46, 58 );
			SetInt( 36, 34 );

			SetHits( 20, 36 );
			SetMana( 0 );

			SetDamage( 3, 9 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 15, 30 );

			SetSkill( SkillName.MagicResist, 35.1, 44.0 );
			SetSkill( SkillName.Tactics, 35.1, 40.0 );
			SetSkill( SkillName.FistFighting, 35.1, 50.0 );

			Fame = 150;
			Karma = -150;

			VirtualArmor = 20;

			Tamable = false;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Poor );
		}

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }

		public MongbatBrute( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}