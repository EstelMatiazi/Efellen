using System;
using Server;
using Server.Items;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName( "a Skeleton's corpse" )]
	public class PaleMasterSkeletonWarrior : BaseCreature
	{
		public override bool DeleteCorpseOnDeath { get { return true; } }

		public override bool IsDispellable { get { return false; } }

		public override double GetFightModeRanking( Mobile m, FightMode acqType, bool bPlayerOnly )
		{
			return ( m.Int + m.Skills[SkillName.Necromancy].Value ) / Math.Max( GetDistanceToSqrt( m ), 1.0 );
		}

		[Constructable]
		public PaleMasterSkeletonWarrior() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = Utility.RandomList( 57, 168, 170 );
			BaseSoundID = 451;
			Hue = 0xB97;
			SetStr( 196, 250 );
			SetDex( 76, 95 );
			SetInt( 36, 60 );

			SetHits( 118, 150 );

			SetDamage( 6, 16 );

			SetDamageType( ResistanceType.Physical, 40 );
			SetDamageType( ResistanceType.Cold, 60 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.MagicResist, 65.1, 80.0 );
			SetSkill( SkillName.Tactics, 85.1, 100.0 );
			SetSkill( SkillName.FistFighting, 85.1, 95.0 );

			Fame = 0;
			Karma = 0;

			VirtualArmor = 40;
			ControlSlots = 0;
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune { get { return Poison.Regular; } }

		public PaleMasterSkeletonWarrior( Serial serial ): base( serial )
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