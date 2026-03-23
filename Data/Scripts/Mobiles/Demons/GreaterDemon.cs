using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a greater demon corpse" )]
	public class GreaterDemon : BaseCreature
	{
		public override bool IsDispellable { get { return false; } }

		[Constructable]
		public GreaterDemon () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a greater demon";
			Body = Utility.RandomList( 195, 137, 353, 444, 445, 930, 935, 969 );
			BaseSoundID = 357;

			if ( Utility.RandomMinMax( 1, 10 ) == 1 ) // FEMALE
			{
				Body = 131;
				BaseSoundID = 0x4B0;
			}

			SetStr( 376, 405 );
			SetDex( 146, 165 );
			SetInt( 301, 325 );

			SetHits( 286, 353 );

			SetDamage( 6, 12 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Fire, 50 );

			SetResistance( ResistanceType.Physical, 60 );
			SetResistance( ResistanceType.Fire, 60 );
			SetResistance( ResistanceType.Cold, 40 );
			SetResistance( ResistanceType.Poison, 60 );
			SetResistance( ResistanceType.Energy, 50 );

			SetSkill( SkillName.Psychology, 90.0 );
			SetSkill( SkillName.Magery, 90.0 );
			SetSkill( SkillName.MagicResist, 95.0 );
			SetSkill( SkillName.Tactics, 110.0 );
			SetSkill( SkillName.FistFighting, 110.0 );

			Fame = 13000;
			Karma = -13000;

			VirtualArmor = 40;
			ControlSlots = Core.SE ? 4 : 5;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich,2 );
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override bool CanRummageCorpses{ get{ return false; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override int TreasureMapLevel{ get{ return 3; } }
		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 8; } }
		public override HideType HideType{ get{ return HideType.Hellish; } }
		public override int Skin{ get{ return Utility.Random(6); } }
		public override SkinType SkinType{ get{ return SkinType.Demon; } }
		public override int Skeletal{ get{ return Utility.Random(6); } }
		public override SkeletalType SkeletalType{ get{ return SkeletalType.Devil; } }

		public GreaterDemon( Serial serial ) : base( serial )
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
