using System;
using Server;
using Server.Items;
using System.Collections;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Magical
{
	public class SummonPhoenixSpell : MagicalSpell
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
		
		public SummonPhoenixSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
				BaseCreature.Summon( new SummonPhoenix(), false, Caster, new Point3D( p ), 0x212, duration );

				Caster.SendMessage( "You can double click the summon to dispel them." );
			}
			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private SummonPhoenixSpell m_Owner;

			public InternalTarget( SummonPhoenixSpell owner ) : base( 12, true, TargetFlags.None )
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
	[CorpseName( "smoldering ashes" )]
	public class SummonPhoenix : BaseCreature
	{
		public override bool DeleteCorpseOnDeath { get { return Summoned; } }
		public override double DispelDifficulty { get { return 900.0; } }
		public override double DispelFocus { get { return 900.0; } }

		public override double GetFightModeRanking( Mobile m, FightMode acqType, bool bPlayerOnly )
		{
			return 200 / Math.Max( GetDistanceToSqrt( m ), 1.0 );
		}

		[Constructable]
		public SummonPhoenix() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a Phoenix";

			Body = 0xf3;
			Hue = 0xB73;
			BaseSoundID = 0x8F;

			SetStr( 301, 330 );
			SetDex( 133, 152 );
			SetInt( 301, 340 );

			SetHits( 201, 208 );

			SetDamage( 15, 21 );

			SetDamageType( ResistanceType.Fire, 100 );

			SetResistance( ResistanceType.Physical, 45, 50 );
			SetResistance( ResistanceType.Fire, 60, 70 );
			SetResistance( ResistanceType.Cold, 30, 40 );
			SetResistance( ResistanceType.Poison, 45, 50 );
			SetResistance( ResistanceType.Energy, 45, 50 );

			SetSkill( SkillName.Psychology, 90.2, 100.0 );
			SetSkill( SkillName.Magery, 90.2, 100.0 );
			SetSkill( SkillName.Meditation, 75.1, 100.0 );
			SetSkill( SkillName.MagicResist, 86.0, 135.0 );
			SetSkill( SkillName.Tactics, 80.1, 90.0 );
			SetSkill( SkillName.FistFighting, 90.1, 100.0 );

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

		public SummonPhoenix( Serial serial ) : base( serial )
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