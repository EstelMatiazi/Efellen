using System;
using Server;
using Server.Items;
using System.Collections;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Magical
{
	public class SummonDireBearSpell : MagicalSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"", "", 
				266,
				9040,
				false
			);

		public override SpellCircle Circle { get { return SpellCircle.Eighth; } }
		public override double RequiredSkill{ get{ return 0.0; } }
		public override int RequiredMana{ get{ return 30; } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.0 ); } }
		
		public SummonDireBearSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if ( (Caster.Followers + 3) > Caster.FollowersMax )
			{
				Caster.SendLocalizedMessage( 1049645 ); // You have too many followers to summon that creature.
				return false;
			}

			return true;
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( IPoint3D p )
		{
			Map map = Caster.Map;
			SpellHelper.GetSurfaceTop( ref p );

			if ( map == null || !map.CanSpawnMobile( p.X, p.Y, p.Z ) )
			{
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
			}
			else if ( SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				TimeSpan duration;
				duration = TimeSpan.FromSeconds( 120 );
				BaseCreature.Summon( new SummonDireBear(), false, Caster, new Point3D( p ), 0x212, duration );

				Caster.SendMessage( "You can double click the summon to dispel them." );
			}
			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private SummonDireBearSpell m_Owner;

			public InternalTarget( SummonDireBearSpell owner ) : base( 12, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is IPoint3D )
					m_Owner.Target( (IPoint3D)o );
			}

			protected override void OnTargetOutOfLOS( Mobile from, object o )
			{
				from.SendLocalizedMessage( 501943 ); // Target cannot be seen. Try again.
				from.Target = new InternalTarget( m_Owner );
				from.Target.BeginTimeout( from, TimeoutTime - DateTime.Now );
				m_Owner = null;
			}

			protected override void OnTargetFinish( Mobile from )
			{
				if ( m_Owner != null )
					m_Owner.FinishSequence();
			}
		}
	}
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Server.Mobiles
{
	[CorpseName( "Dire Bear Corpse" )]
	public class SummonDireBear : BaseCreature
	{
		public override bool DeleteCorpseOnDeath { get { return Summoned; } }
		public override double DispelDifficulty { get { return 900.0; } }
		public override double DispelFocus { get { return 900.0; } }
        public override PackInstinct PackInstinct{ get{ return PackInstinct.Bear; } }

		public override double GetFightModeRanking( Mobile m, FightMode acqType, bool bPlayerOnly )
		{
			return 200 / Math.Max( GetDistanceToSqrt( m ), 1.0 );
		}

		[Constructable]
		public SummonDireBear() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a Spiritual Dire Bear";

			Body = 0xd5;
			Hue = 0x323;
			BaseSoundID = 0xA3;

			SetStr( 331, 360 );
			SetDex( 173, 192 );
			SetInt( 101, 140 );

			SetHits( 251, 298 );

			SetDamage( 19, 24 );

			SetDamageType( ResistanceType.Fire, 100 );

			SetResistance( ResistanceType.Physical, 80, 90 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Cold, 30, 50 );
			SetResistance( ResistanceType.Poison, 30, 50 );
			SetResistance( ResistanceType.Energy, 50, 60 );

			SetSkill( SkillName.MagicResist, 106.0, 135.0 );
			SetSkill( SkillName.Tactics, 100.1, 110.0 );
			SetSkill( SkillName.FistFighting, 110.1, 115.0 );

			Fame = 0;
			Karma = 0;

			VirtualArmor = 50;
			Tamable = false;
            ControlSlots = 3;
		}

		public override bool ReacquireOnMovement{ get{ return true; } }
		public override bool HasBreath{ get{ return true; } }

		public override void OnThink()
		{
			if ( Core.SE && Summoned )
			{
				ArrayList spirtsOrVortexes = new ArrayList();

				foreach ( Mobile m in GetMobilesInRange( 5 ) )
				{
					if ( BaseCreature.isVortex(m) )
					{
						if ( ( (BaseCreature) m ).Summoned )
							spirtsOrVortexes.Add( m );
					}
				}

				while ( spirtsOrVortexes.Count > 6 )
				{
					int index = Utility.Random( spirtsOrVortexes.Count );
					Dispel( ( (Mobile) spirtsOrVortexes[index] ) );
					spirtsOrVortexes.RemoveAt( index );
				}
			}

			base.OnThink();
		}

		public SummonDireBear( Serial serial ) : base( serial )
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