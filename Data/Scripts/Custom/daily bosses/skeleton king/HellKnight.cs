using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a skeletal corpse" )]
	public class HellKnight : BaseCreature
	{
		[Constructable]
		public HellKnight() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a Hell knight";
			Body = Utility.RandomList( 57, 168, 170, 327 );
			BaseSoundID = 451;
            Hue = 0x85;

			SetStr( 396, 450 );
			SetDex( 276, 325 );
			SetInt( 36, 60 );

			SetHits( 418, 550 );

			SetDamage( 18, 24 );

			SetDamageType( ResistanceType.Physical, 40 );
			SetDamageType( ResistanceType.Fire, 60 );

			SetResistance( ResistanceType.Physical, 45 );
			SetResistance( ResistanceType.Fire, 75 );
			SetResistance( ResistanceType.Cold, 50 );
			SetResistance( ResistanceType.Poison, 60 );
			SetResistance( ResistanceType.Energy, 50 );

			SetSkill( SkillName.MagicResist, 101.0 );
			SetSkill( SkillName.Tactics, 110.0 );
			SetSkill( SkillName.FistFighting, 115.0 );

			Fame = 7000;
			Karma = -7000;

			VirtualArmor = 40;
		}

		public override bool BleedImmune{ get{ return true; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Rich );
		}
		
		public HellKnight( Serial serial ) : base( serial )
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