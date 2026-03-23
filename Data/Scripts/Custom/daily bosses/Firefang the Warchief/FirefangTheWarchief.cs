using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Commands;
using Server.Commands.Generic;
using Server.Spells;
using Server.EffectsUtil;
using Server.Custom;
using Server.Custom.DailyBosses.System;
using Server.Custom.BossSystems;
using Server.Custom.Ascensions;
namespace Server.Mobiles
{
	[CorpseName( "Firefang's Corpse" )]
	public class FirefangTheWarchief : BaseCreature
	{
    	private static readonly Type[] SummonTypes = new Type[] 
		{ 
			typeof(Orc), 
	 		typeof(OrcBomber), 
			typeof(OrcBomber), 
			typeof(OrcishMage), 
			typeof(OrcishLord)
		};

		private static readonly string[] SummonWarcries = new string[]
		{
			"ME MATES WILL CUT YOU!",
			"WE WILL EAT YOU RAW!",
			"KILL IT WITH FIRE!",
			"KILL IT KILL IT FASTA!"
		};

		private static readonly List<Type> BossDrops = new List<Type>
		{
			typeof(Artifact_TunicOfImmolation),
			typeof(Artifact_GauntletsOfImmolation),
			typeof(Artifact_ArmsOfImmolation),
			typeof(Artifact_CoifOfImmolation),
			typeof(Artifact_LeggingsOfImmolation),
		};

		private int m_Rage = 0;
		private Mobile m_LastTarget;
		private DateTime m_NextSummonTime = DateTime.MinValue;
		private DateTime m_NextSpecialAttack = DateTime.MinValue;
		private DateTime m_NextBomb = DateTime.MinValue;
		private int m_Thrown;
		private List<BaseCreature> m_Summons = new List<BaseCreature>();

		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }

		[Constructable]
		public FirefangTheWarchief () : base( AIType.AI_Mage, FightMode.Closest, 20, 1, 0.4, 0.8 )
		{
			Name = "Firefang";
			Title = "The Warchief";
			Body = 0x1d9;
			NameHue = 0x22;
			Hue = 348;
			BaseSoundID = 0x45A;

			SetStr( 496, 585 );
			SetDex( 155, 235 );
			SetInt( 206, 275 );

			SetHits( 3000 );
			SetDamage( 11, 15 );

			SetDamageType( ResistanceType.Fire, 50 );
			SetDamageType( ResistanceType.Physical, 50 );

			SetResistance( ResistanceType.Physical, 45 );
			SetResistance( ResistanceType.Fire, 65 );
			SetResistance( ResistanceType.Cold, 40 );
			SetResistance( ResistanceType.Poison, 50 );
			SetResistance( ResistanceType.Energy, 50 );

			SetSkill( SkillName.Magery, 82.5, 115.0 );
			SetSkill( SkillName.Psychology, 62.5, 85.0 );
			SetSkill( SkillName.Meditation, 72.5, 85.0 );
			SetSkill( SkillName.MagicResist, 75.5, 125.0 );
			SetSkill( SkillName.Tactics, 81.0, 95.0 );
			SetSkill( SkillName.FistFighting, 111.0, 125.0 );

			Fame = 15000;
			Karma = -15000;

			VirtualArmor = 30;

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 2 );
		}

		public override int TreasureMapLevel{ get{ return 3; } }
		public override bool CanRummageCorpses{ get{ return false; } }
		public override int BreathPhysicalDamage{ get{ return 0; } }
		public override int BreathFireDamage{ get{ return 100; } }
		public override int BreathColdDamage{ get{ return 0; } }
		public override int BreathPoisonDamage{ get{ return 0; } }
		public override int BreathEnergyDamage{ get{ return 0; } }
		public override int BreathEffectHue{ get{ return 348; } }
		public override int BreathEffectSound{ get{ return 0x64F; } }
		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool HasBreath{ get{ return true; } }
		public override double BreathEffectDelay{ get{ return 0.1; } }
		public override int GetBreathForm()
		{
		    return 2; // potion
		}
		public override bool BleedImmune{ get{ return true; } }
		public override bool BardImmune { get { return true; } }
		public override bool Unprovokable { get { return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }


		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			m_LastTarget = from;
			if (Utility.RandomDouble() < 0.30 )
				Server.Misc.IntelligentAction.LeapToAttacker( this, from );

			base.OnDamage( amount, from, willKill );
		}

		public override void OnThink()
		{
		    base.OnThink();

		    Mobile combatant = this.Combatant;

		    if (combatant == null || combatant.Deleted || !combatant.Alive)
		        return;

		    BossSummonSystem.TrySummonCreature(
		        this,
		        combatant,
		        SummonTypes,
		        m_Rage,
		        ref m_NextSummonTime,
		        SummonWarcries,
		        m_Summons,
		        1316,
		        GetMaxSummons(),
		        45
		    );

		    if (DateTime.UtcNow >= m_NextSpecialAttack)
		    {
		        PerformRageAttack(combatant);
		        m_NextSpecialAttack = DateTime.UtcNow + TimeSpan.FromSeconds(30 - (m_Rage * 2));
		    }

		    m_LastTarget = combatant;
		}

		private void PerformRageAttack( Mobile target )
		{
			if ( target == null || target.Deleted || !target.Alive )
				return;

			
			int attackChoice = Utility.RandomMinMax( 1, 3 );

			switch ( attackChoice )
			{
				case 1:
				{
					BossSpecialAttack.PerformTargettedAoE(
						this,
						target,
						m_Rage+1,
						"*BOOM TIME!*",
						348,  // hue
						0,     // physical
						100,   // fire
						0,     // cold
						0,     // poison
						0      // energy
					);
					break;
				}
				case 2:
				{
					BossSpecialAttack.PerformDelayedExplosion(
					    this,
					    "*LIGHT DA FUSES BOYS!*",
					    348,   // hue
					    8,     // radius
					    m_Rage+1,
					    0,     // physical
					    100,   // fire
					    0,     // cold
					    0,     // poison
					    0      // energy
					);
					break;
				}
				case 3:
				{
					 BossSpecialAttack.PerformDelayedExplosion(
		                this,
		                "*LIGHT DA FUSES BOYS!*",
		                348,   // hue
		                16,    // radius
		                m_Rage+2,
		                0,     // physical
		                100,   // fire
		                0,     // cold
		                0,     // poison
		                0      // energy
		            );
					break;
				}
			}
		}

		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			reflect = ( Utility.Random( 100 ) < m_Rage * 10 );
		}

		private int GetMaxSummons()
		{
			switch( m_Rage )
			{
				//firefang has lots of buddies
				case 0: return 16;
				case 1: return 14;
				case 2: return 12;
				case 3: return 10;
				default: return 10;
			}
		}
		
		public override bool OnBeforeDeath()
		{
			if ( m_Rage == 0 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "ME NO HURT!" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				SetDamage( 16, 20 );
				
				m_Rage = 1;
				return false;
			}
			else if ( m_Rage == 1 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "ME WILL CHEW UR BONES*" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				SetDamage( 21, 25 );
				VirtualArmor += 5;
				m_Rage = 2;
				return false;
			}
			else if ( m_Rage == 2 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "ME WILL BLOW U UP*" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				SetDamage( 26, 30 );
				VirtualArmor += 5;
				m_Rage = 3;
				return false;
			}
			else if ( m_Rage == 2 )
			{
				Effects.SendLocationParticles( EffectItem.Create( this.Location, this.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
				this.PlaySound( 0x1FE );
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "AM...DONE*" );
				Mobile killer = this.LastKiller;
				if (killer != null && killer.Player && killer.Karma > 0)
				{
					int marks = Utility.RandomMinMax(70, 90);
					Server.Custom.DefenderOfTheRealm.MarkLootHelper.AwardMarks(killer, 1, marks);
				}
			}
			
			return base.OnBeforeDeath();
		}

		public override void OnDelete()
		{
		    if (m_Summons != null)
		    {
		        BossSummonSystem.CleanupSummons(m_Summons);
		        m_Summons.Clear();
		        m_Summons = null;
		    }

		    base.OnDelete();
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			BossLootSystem.AwardBossSpecial(this, BossDrops, 15);
			for ( int i = 0; i < 2; i++ )
			{
				c.DropItem( AscensionScrollFactory.CreateRandom());
				c.DropItem( Loot.RandomArty() );
				c.DropItem( new EtherealPowerScroll() );
			}
			RichesSystem.SpawnRiches( m_LastTarget, 2 );
		}

		public override void OnAfterSpawn()
		{
			base.OnAfterSpawn();
			LeechImmune = true;
		}

		public FirefangTheWarchief( Serial serial ) : base( serial )
		{
		}

		public override void OnActionCombat()
		{
			Mobile combatant = Combatant;

			if ( combatant == null || combatant.Deleted || combatant.Map != Map || !InRange( combatant, 12 ) || !CanBeHarmful( combatant ) || !InLOS( combatant ) )
				return;

			if ( DateTime.UtcNow >= m_NextBomb )
			{
				ThrowBomb( combatant );

				m_Thrown++;

				if ( 0.85 >= Utility.RandomDouble() && (m_Thrown % 2) == 1 )
					m_NextBomb = DateTime.UtcNow + TimeSpan.FromSeconds( 3.0 );
				else
					m_NextBomb = DateTime.UtcNow + TimeSpan.FromSeconds( 5.0 + (10.0 * Utility.RandomDouble()) );
			}
		}

		public void ThrowBomb( Mobile m )
		{
			DoHarmful( m );

			this.MovingParticles( m, 0x1C19, 1, 0, false, true, 0, 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );

			new BombTimer( m, this, m_Rage ).Start();
		}

		private class BombTimer : Timer
		{
			private Mobile m_Mobile;
			private Mobile m_From;
			private int m_Rage;

			public BombTimer( Mobile m, Mobile from, int rage ) : base( TimeSpan.FromSeconds( 1.0 ) )
			{
				m_Mobile = m;
				m_From = from;
				m_Rage = rage;
				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
				if ( m_Mobile == null || m_Mobile.Deleted )
					return;

				m_Mobile.PlaySound( 0x11D );
				
				int minDmg = 10 + (m_Rage * 2);
				int maxDmg = 20 + (m_Rage * 2);
				
				AOS.Damage( m_Mobile, m_From, Utility.RandomMinMax( minDmg, maxDmg ), 0, 100, 0, 0, 0 );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version

			writer.Write( m_Rage );
			writer.Write( m_NextSummonTime );
			writer.Write( m_NextSpecialAttack );
			writer.Write( m_NextBomb );
			writer.Write( m_Thrown );
			
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( version >= 1 )
			{
				m_Rage = reader.ReadInt();
				m_NextSummonTime = reader.ReadDateTime();
				m_NextSpecialAttack = reader.ReadDateTime();
				m_NextBomb = reader.ReadDateTime();
				m_Thrown = reader.ReadInt();
			}

			LeechImmune = true;
			if (m_Summons == null)
					m_Summons = new List<BaseCreature>();
		}
	}
}