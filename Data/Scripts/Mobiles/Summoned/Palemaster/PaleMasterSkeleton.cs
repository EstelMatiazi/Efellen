using System;
using Server;
using Server.Items;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName( "a Skeleton's corpse" )]
	public class PaleMasterSkeleton : BaseCreature
	{
		public override bool DeleteCorpseOnDeath { get { return true; } }

		public override bool IsDispellable { get { return false; } }

		public override double GetFightModeRanking( Mobile m, FightMode acqType, bool bPlayerOnly )
		{
			return ( m.Int + m.Skills[SkillName.Necromancy].Value ) / Math.Max( GetDistanceToSqrt( m ), 1.0 );
		}

		[Constructable]
		public PaleMasterSkeleton() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = Utility.RandomList( 50, 56, 167, 168, 170, 878 );
			BaseSoundID = 0x48D;
			Hue = 0xB97;
			SetStr( 100 );
			SetDex( 100 );
			SetInt( 50 );

			SetHits( 80 );
			SetStam( 120 );
			SetMana( 0 );

			SetDamage( 7, 12 );

			SetDamageType( ResistanceType.Physical, 100 );
			SetResistance( ResistanceType.Physical, 20, 30 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.MagicResist, 66.9 );
			SetSkill( SkillName.Tactics, 77.0 );
			SetSkill( SkillName.FistFighting, 88.0 );

			Fame = 0;
			Karma = 0;

			VirtualArmor = 40;
			ControlSlots = 0;
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune { get { return Poison.Regular; } }

		public override int GetAngerSound()
		{
			return 0x15;
		}

		public override int GetAttackSound()
		{
			return 0x28;
		}

		public PaleMasterSkeleton( Serial serial ): base( serial )
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