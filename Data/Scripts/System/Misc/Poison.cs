using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;

namespace Server
{
	public class PoisonImpl : Poison
	{
		[CallPriority( 10 )]
		public static void Configure()
		{
			if ( Core.AOS )
			{
				Register( new PoisonImpl( "Lesser",		0,  4, 16,  7.5, 3.0, 2.25, 10, 4 ) );
				Register( new PoisonImpl( "Regular",	1,  8, 18, 10.0, 3.0, 3.25, 10, 3 ) );
				Register( new PoisonImpl( "Greater",	2, 12, 20, 15.0, 3.0, 4.25, 10, 2 ) );
				Register( new PoisonImpl( "Deadly",		3, 16, 30, 30.0, 3.0, 5.25, 15, 2 ) );
				Register( new PoisonImpl( "Lethal",		4, 20, 50, 35.0, 3.0, 5.25, 20, 2 ) );
			}
			else
			{
				Register( new PoisonImpl( "Lesser",		0, 4, 26,  2.500, 3.5, 3.0, 10, 2 ) );
				Register( new PoisonImpl( "Regular",	1, 5, 26,  3.125, 3.5, 3.0, 10, 2 ) );
				Register( new PoisonImpl( "Greater",	2, 6, 26,  6.250, 3.5, 3.0, 10, 2 ) );
				Register( new PoisonImpl( "Deadly",		3, 7, 26, 12.500, 3.5, 4.0, 10, 2 ) );
				Register( new PoisonImpl( "Lethal",		4, 9, 26, 25.000, 3.5, 5.0, 10, 2 ) );
			}
		}

		public static Poison IncreaseLevel( Poison oldPoison )
		{
			Poison newPoison = ( oldPoison == null ? null : GetPoison( oldPoison.Level + 1 ) );

			return ( newPoison == null ? oldPoison : newPoison );
		}

		// Immediately resolves one poison tick's worth of damage against the target,
		// attributed to 'from'. Used by Assassin Virulent Strikes passive.
		public static void ResolveSingleTick( Mobile target, Mobile from )
		{
		    if ( target == null || target.Deleted || !target.Alive )
		        return;

		    PoisonImpl poison = target.Poison as PoisonImpl;

		    if ( poison == null )
		        return;

		    int damage = 1 + (int)(target.Hits * poison.m_Scalar);

		    if ( damage < poison.m_Minimum ) damage = poison.m_Minimum;
		    if ( damage > poison.m_Maximum ) damage = poison.m_Maximum;

		    if ( from != null )
		        from.DoHarmful( target, true );

		    AOS.Damage( target, from, damage, 0, 0, 0, 100, 0 );

		    target.FixedParticles( 0x374A, 10, 15, 5021, 0x233, 0, EffectLayer.Waist );
		}

		// Info
		private string m_Name;
		private int m_Level;

		// Damage
		private int m_Minimum, m_Maximum;
		private double m_Scalar;

		// Timers
		private TimeSpan m_Delay;
		private TimeSpan m_Interval;
		private int m_Count, m_MessageInterval;

		public PoisonImpl( string name, int level, int min, int max, double percent, double delay, double interval, int count, int messageInterval )
		{
			m_Name = name;
			m_Level = level;
			m_Minimum = min;
			m_Maximum = max;
			m_Scalar = percent * 0.01;
			m_Delay = TimeSpan.FromSeconds( delay );
			m_Interval = TimeSpan.FromSeconds( interval );
			m_Count = count;
			m_MessageInterval = messageInterval;
		}

		public override string Name{ get{ return m_Name; } }
		public override int Level{ get{ return m_Level; } }

		public class PoisonTimer : Timer
		{
			private PoisonImpl m_Poison;
			private Mobile m_Mobile;
			private Mobile m_From;
			private int m_LastDamage;
			private int m_Index;

			public Mobile From{ get{ return m_From; } set{ m_From = value; } }

			public PoisonTimer( Mobile m, PoisonImpl p ) : base( p.m_Delay, p.m_Interval )
			{
				m_From = m;
				m_Mobile = m;
				m_Poison = p;
				BuffInfo.RemoveBuff( m, BuffIcon.Poisoned );
				BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.Poisoned, 1063494 ) );
			}

			protected override void OnTick()
			{
			    if ( (Core.AOS && m_Poison.Level < 4 && TransformationSpellHelper.UnderTransformation( m_Mobile, typeof( VampiricEmbraceSpell ) )) ||
			        (m_Poison.Level < 3 && OrangePetals.UnderEffect( m_Mobile )) ||
			        AnimalForm.UnderTransformation( m_Mobile, typeof( Unicorn ) ) || m_Mobile.Skills[SkillName.Druidism].Value > 90 )
			    {
			        if ( m_Mobile.CurePoison( m_Mobile ) )
			        {
			            m_Mobile.LocalOverheadMessage( MessageType.Emote, 0x3F, true,
			                "* You feel yourself resisting the effects of the poison *" );

			            m_Mobile.NonlocalOverheadMessage( MessageType.Emote, 0x3F, true,
			                String.Format( "* {0} seems resistant to the poison *", m_Mobile.Name ) );

			            Stop();
			            return;
			        }
			    }

			    PlayerMobile pm = m_Mobile as PlayerMobile;

			    if ( pm != null && !pm.Deleted && pm.ActiveAscension == AscensionType.Crusader )
			    {
			        AscensionProgress prog = pm.AscensionProfile.Get( AscensionType.Crusader );
			        int level = prog.Level;

			        if ( level >= 2 )
			        {
			            bool eligible = false;

			            switch ( m_Poison.Level )
			            {
			                case 0:
			                case 1:
			                case 2: eligible = true; break;
			                case 3: eligible = level >= 5;  break;
			                case 4: eligible = level >= 13; break;
			            }

			            if ( eligible && Utility.Random( 100 ) < (level * 4) )
			            {
			                if ( pm.CurePoison( pm ) )
			                {
			                    pm.LocalOverheadMessage( MessageType.Emote, 0x3F, true,
			                        "* Divine Grace purges the poison from your body *" );

			                    pm.NonlocalOverheadMessage( MessageType.Emote, 0x3F, true,
			                        String.Format( "* Divine Grace purges the poison from {0}'s body *", pm.Name ) );

			                    Stop();
			                    return;
			                }
			            }
			        }
			    }

			    if ( m_Index++ == m_Poison.m_Count )
			    {
			        m_Mobile.SendLocalizedMessage( 502136 ); // The poison seems to have worn off.
			        m_Mobile.Poison = null;
			        BuffInfo.RemoveBuff( m_Mobile, BuffIcon.Poisoned );

			        Stop();
			        return;
			    }

			    int damage;

			    if ( !Core.AOS && m_LastDamage != 0 && Utility.RandomBool() )
			    {
			        damage = m_LastDamage;
			    }
			    else
			    {
			        damage = 1 + (int)(m_Mobile.Hits * m_Poison.m_Scalar);

			        if ( damage < m_Poison.m_Minimum )
			            damage = m_Poison.m_Minimum;
			        else if ( damage > m_Poison.m_Maximum )
			            damage = m_Poison.m_Maximum;

			        m_LastDamage = damage;
			    }

			    // ── Dangerous Habits (Assassin passive, level 8+) ────────────────────
			    PlayerMobile dangerousPm = m_From as PlayerMobile;

			    if ( dangerousPm != null && !dangerousPm.Deleted
			        && dangerousPm.ActiveAscension == AscensionType.Assassin )
			    {
			        AscensionProgress dangerousProg = dangerousPm.AscensionProfile.Get( AscensionType.Assassin );
			        int dangerousLevel = dangerousProg.Level;

			        if ( dangerousLevel >= 17 )
			            damage = (int)(damage * 1.18);
			        else if ( dangerousLevel >= 8 )
			            damage = (int)(damage * 1.09);
			    }
			    // ── End Dangerous Habits ─────────────────────────────────────────────

			    if ( m_From != null )
			        m_From.DoHarmful( m_Mobile, true );

			    AOS.Damage( m_Mobile, m_From, damage, 0, 0, 0, 100, 0 );

			    if ( 0.60 <= Utility.RandomDouble() )
			        m_Mobile.RevealingAction();

			    if ( (m_Index % m_Poison.m_MessageInterval) == 0 )
			        m_Mobile.OnPoisoned( m_From, m_Poison, m_Poison );
			}
		}

		public override Timer ConstructTimer( Mobile m )
		{
			return new PoisonTimer( m, this );
		}
	}
}