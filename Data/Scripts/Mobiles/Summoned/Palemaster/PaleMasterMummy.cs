using System;
using Server;
using Server.Items;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName( "a Mummy's corpse" )]
	public class PaleMasterMummy : BaseCreature
	{
		public override bool DeleteCorpseOnDeath { get { return true; } }

		public override bool IsDispellable { get { return false; } }
		public override int BreathPhysicalDamage{ get{ return 100; } }
		public override int BreathFireDamage{ get{ return 0; } }
		public override int BreathColdDamage{ get{ return 0; } }
		public override int BreathPoisonDamage{ get{ return 0; } }
		public override int BreathEnergyDamage{ get{ return 0; } }
		public override int BreathEffectHue{ get{ return 0; } }
		public override int BreathEffectSound{ get{ return 0x5D2; } }
		public override int BreathEffectItemID{ get{ return 0x239F; } }
		public override bool HasBreath{ get{ return true; } }
		public override double BreathEffectDelay{ get{ return 0.1; } }
		public override int GetBreathForm()
		{
		    return 37;
		}

		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.MortalStrike;
		}

		public override double GetFightModeRanking( Mobile m, FightMode acqType, bool bPlayerOnly )
		{
			return ( m.Int + m.Skills[SkillName.Necromancy].Value ) / Math.Max( GetDistanceToSqrt( m ), 1.0 );
		}

		[Constructable]
		public PaleMasterMummy() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a Mummy";
			Body = Utility.RandomList( 154, 471 );
			BaseSoundID = 471;
			Hue = 0xB97;

			SetStr( 346, 370 );
			SetDex( 71, 90 );
			SetInt( 26, 40 );

			SetHits( 208, 222 );

			SetDamage( 13, 23 );

			SetDamageType( ResistanceType.Physical, 40 );
			SetDamageType( ResistanceType.Cold, 60 );

			SetResistance( ResistanceType.Physical, 45, 55 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.MagicResist, 15.1, 40.0 );
			SetSkill( SkillName.Tactics, 35.1, 50.0 );
			SetSkill( SkillName.FistFighting, 35.1, 50.0 );

			Fame = 0;
			Karma = 0;

			VirtualArmor = 40;
			ControlSlots = 0;
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune { get { return Poison.Regular; } }

		public PaleMasterMummy( Serial serial ): base( serial )
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