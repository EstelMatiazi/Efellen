using System;
using Server;
using Server.Items;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName( "a Skeleton's corpse" )]
	public class PaleMasterSkeletonKnight : BaseCreature
	{
		public override bool DeleteCorpseOnDeath { get { return true; } }

		public override bool IsDispellable { get { return false; } }

		public override double GetFightModeRanking( Mobile m, FightMode acqType, bool bPlayerOnly )
		{
			return ( m.Int + m.Skills[SkillName.Necromancy].Value ) / Math.Max( GetDistanceToSqrt( m ), 1.0 );
		}

		[Constructable]
		public PaleMasterSkeletonKnight() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a skeletal knight";
			Body = Utility.RandomList( 57, 168, 170 );
			Hue = 0xB97;
			SetStr( 226, 250 );
			SetDex( 96, 115 );
			SetInt( 36, 60 );

			SetHits( 168, 210 );

			SetDamage( 7, 17 );

			SetDamageType( ResistanceType.Physical, 40 );
			SetDamageType( ResistanceType.Cold, 60 );

			SetResistance( ResistanceType.Physical, 45 );
			SetResistance( ResistanceType.Fire, 30 );
			SetResistance( ResistanceType.Cold, 60 );
			SetResistance( ResistanceType.Poison, 30 );
			SetResistance( ResistanceType.Energy, 40 );

			SetSkill( SkillName.MagicResist, 90.0 );
			SetSkill( SkillName.Tactics, 105.0 );
			SetSkill( SkillName.FistFighting, 105.0 );

			Fame = 0;
			Karma = 0;

			VirtualArmor = 40;
			ControlSlots = 0;
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune { get { return Poison.Regular; } }

		public PaleMasterSkeletonKnight( Serial serial ): base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

		}
	}
}