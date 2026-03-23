using System;
using Server;
using Server.Items;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName( "a giant's corpse" )]
	public class PaleMasterUndeadGiant : BaseCreature
	{
		public override bool DeleteCorpseOnDeath { get { return true; } }
		public override bool IsDispellable { get { return false; } }

		public override double GetFightModeRanking( Mobile m, FightMode acqType, bool bPlayerOnly )
		{
			return ( m.Int + m.Skills[SkillName.Necromancy].Value ) / Math.Max( GetDistanceToSqrt( m ), 1.0 );
		}

		[Constructable]
		public PaleMasterUndeadGiant() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an Undead Giant";
			Body = 325;
			BaseSoundID = 471;
			Hue = 0xB97;

			SetStr( 520 );
			SetDex( 190 );
			SetInt( 90 );

			SetHits( 448, 582 );

			SetDamage( 19, 26 );

			SetDamageType( ResistanceType.Physical, 40 );
			SetDamageType( ResistanceType.Cold, 60 );

			SetResistance( ResistanceType.Physical, 65 );
			SetResistance( ResistanceType.Fire, 20 );
			SetResistance( ResistanceType.Cold, 40 );
			SetResistance( ResistanceType.Poison, 80 );
			SetResistance( ResistanceType.Energy, 50 );

			SetSkill( SkillName.MagicResist, 105.0 );
			SetSkill( SkillName.Tactics, 112.0 );
			SetSkill( SkillName.FistFighting, 112.0 );

			Fame = 0;
			Karma = 0;

			VirtualArmor = 40;
			ControlSlots = 0;
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune { get { return Poison.Greater; } }

		public override Poison HitPoison{ get{ return Poison.Greater; } }


		public PaleMasterUndeadGiant( Serial serial ): base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}