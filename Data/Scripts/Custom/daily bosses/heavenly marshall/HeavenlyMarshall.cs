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
using Server.Spells.Necromancy;
using Server.Spells;
using Server.EffectsUtil;
using Server.Custom;
using Server.Custom.DailyBosses.System;
using Server.Custom.BossSystems;
using Server.Custom.Ascensions;
namespace Server.Mobiles
{
	[CorpseName( "Heavenly Marshall's Corpse" )]
	public class HeavenlyMarshall : BaseCreature
	{
		private const int SUMMON_RANGE = 12;
		
		private static readonly Type[] SummonTypes = new Type[] 
		{ 
			typeof(WarGriffon), 
			typeof(WarGriffon), 
			typeof(Angel), 
			typeof(Archangel), 
			typeof(EtherealWarriorGeneral) 
		};
		
		private static readonly string[] SummonWarcries = new string[]
		{
			"Come forth, comrades!", 
			"Lets end this menace right now!",
			"We shall stand against the tyranny of chaos!",
			"Hosts of heaven, answer my call!"
		};

		private static readonly List<Type> BossDrops = new List<Type>
    	{
    	    typeof(Artifact_GauntletsOfDevotion),
    	    typeof(Artifact_LeggingsOfDevotion),
    	    typeof(Artifact_TunicOfDevotion),
    	    typeof(Artifact_ArmsOfDevotion),
			typeof(Artifact_CoifOfDevotion),
			typeof(Artifact_HolySword)
    	};

		private int m_Rage = 0;
		private Mobile m_LastTarget;
		private DateTime m_NextSummonTime = DateTime.MinValue;
		private DateTime m_NextSpecialAttack = DateTime.MinValue;
        private List<BaseCreature> m_Summons = new List<BaseCreature>();

		[Constructable]
		public HeavenlyMarshall () : base( AIType.AI_Mage, FightMode.Evil, 10, 1, 0.2, 0.4 )
		{
			Name = "Heavenly Marshall";
			Title = "The Envoy from Above";
			Body = 346;
			BaseSoundID = 466;
			NameHue = 0x92E;
			Hue = 0x0672;

			SetStr( 796, 885 );
			SetDex( 125, 175 );
			SetInt( 586, 675 );

			SetHits( 19000 );
			SetDamage( 11, 15 );

			SetDamageType( ResistanceType.Energy, 100 );
			SetResistance( ResistanceType.Physical, 60 );
			SetResistance( ResistanceType.Fire, 70 );
			SetResistance( ResistanceType.Cold, 70 );
			SetResistance( ResistanceType.Poison, 70 );
			SetResistance( ResistanceType.Energy, 75 );

			SetSkill( SkillName.Anatomy, 55.1, 75.0 );
			SetSkill( SkillName.Psychology, 90.1, 125.0 );
			SetSkill( SkillName.Meditation, 112.5, 125.0 );
			SetSkill( SkillName.MagicResist, 125.5, 150.0 );
			SetSkill( SkillName.Tactics, 101.0, 125.0 );
			SetSkill( SkillName.FistFighting, 101.0, 125.0 );
			
			Fame = 35000;
			Karma = -35000;

			VirtualArmor = 60;

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 8 );
		}

		public override int TreasureMapLevel{ get{ return 5; } }
		public override int Skeletal{ get{ return 50; } }
		public override SkeletalType SkeletalType{ get{ return SkeletalType.Mystical; } }
		public override int Cloths{ get{ return Utility.Random(50); } }
		public override ClothType ClothType{ get{ return ClothType.Divine; } }
		public override bool CanRummageCorpses{ get{ return false; } }
		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool BleedImmune{ get{ return true; } }
		public override bool BardImmune { get { return true; } }
		public override bool Unprovokable { get { return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }
		public override bool AlwaysAttackable{ get{ return true; } }
		public override bool AlwaysMurderer { get { return false; } }

        
		private bool IsFriendlyCreature(Mobile m)
		{
			return 	m is HeavenlyMarshall || 
					m is SkyKnight || 
					m is Angel || 
					m is Archangel ||
					m is WarGriffon || 
					m is EtherealWarriorGeneral;
		}

		public override bool IsEnemy( Mobile m )
	    {
			if (m == null || m.Deleted)
	        	return false;
			
			if (IsFriendlyCreature(m))
		    	return false;
			
			if (m.Player && m.Karma >= 0 && m.Combatant != this)
				return false;
			
			if ( !IntelligentAction.GetMyEnemies( m, this, true ) )
				return false;
			
			if ( m.Region != this.Region )
				return false;
			
			if (m is BaseCreature && ((BaseCreature)m).ControlMaster == null )
			{
				this.Location = m.Location;
				this.Combatant = m;
				this.Warmode = true;
			}
			
			return true;
	    }

		public override void AggressiveAction(Mobile m, bool criminal)
		{
		    if (IsFriendlyCreature(m))
				return;

		    base.AggressiveAction(m, criminal);
		}

		public override bool CanBeHarmful(Mobile m, bool message, bool ignoreOurBlessedness)
		{
		    if (IsFriendlyCreature(m))
		        return false;

		    return base.CanBeHarmful(m, message, ignoreOurBlessedness);
		}

		public override bool CanBeBeneficial(Mobile m, bool message, bool allowDead)
		{
		    if (IsFriendlyCreature(m))
		        return true;

		    return base.CanBeBeneficial(m, message, allowDead);
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
		        30
		    );

		    if (DateTime.UtcNow >= m_NextSpecialAttack)
		    {
		        PerformRageAttack(combatant);
		        m_NextSpecialAttack = DateTime.UtcNow + TimeSpan.FromSeconds(30 - (m_Rage * 2));
		    }

		    m_LastTarget = combatant;
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			m_LastTarget = from;
			Server.Misc.IntelligentAction.LeapToAttacker( this, from );
			
			if (Utility.RandomDouble() < 0.75 )
				Server.Misc.IntelligentAction.LeapToAttacker( this, from );
			
			if (from.Player && from.Kills < 5 && !from.Criminal) 
				from.Criminal = true;		
		
			base.OnDamage( amount, from, willKill );
		}


		private void PerformRageAttack( Mobile target )
		{
			if ( target == null || target.Deleted || !target.Alive )
				return;

			
			int attackChoice = Utility.RandomMinMax( 1, 3 );
            Map map = this.Map;

			switch ( attackChoice  )
			{
				case 1: // holy nova
				{
					BossSpecialAttack.PerformSlam(
                       boss: this,
                       warcry: "Heavens smite thee!",
                       hue: 0x4D5,
                       rage: m_Rage+1,
                       range: 6,
					   physicalDmg:0,
                       energyDmg: 100
                   );
                   break;
				}

				case 2: //holy cross
				{
					BossSpecialAttack.PerformCrossExplosion(
				       boss: this,
				       target: target,
				       warcry: "*Burn in the light!*",
				       hue: 0xb73,
				       rage: m_Rage+1,
					   physicalDmg:0,
				       fireDmg: 100
				   );
				   break;
				}
				
				case 3: // Rage 3: holy blast (Mana drain + damage)
				{
					PublicOverheadMessage( MessageType.Regular, 0x21, false, "Light everlasting shall consume you!" );
					PlaySound( 0x228 );
					FixedParticles( 0x3789, 10, 25, 5032, EffectLayer.Head );
					IPooledEnumerable eable = GetMobilesInRange( 8 );
					foreach ( Mobile m in eable )
					{
						if ( m != this && m.Player && m.Alive && CanBeHarmful( m ) )
						{
							DoHarmful( m );
							int manaDrain = Utility.RandomMinMax( 60, 90 );
							m.Mana -= manaDrain;
							int damage = Utility.RandomMinMax( manaDrain/2, manaDrain*2 );
							AOS.Damage( m, this, damage, 0, 0, 0, 0, 100 );
							m.FixedParticles( 0x374A, 10, 15, 5013, 0x497, 0, EffectLayer.Waist );
							m.PlaySound( 0x1FB );
							this.Mana = Math.Min( this.ManaMax, this.Mana + manaDrain / 3 );
						}
					}
					SlamVisuals.SlamVisual(this, 6, 0x36B0, 0x497);
					eable.Free();
					break;
				}
			}
		}

		private void GetDirectionVector(Direction d, out int dx, out int dy)
        {
            dx = 0;
            dy = 0;

            switch (d)
            {
                case Direction.North:
                    dy = -1;
                    break;
                case Direction.Right:
                    dx = 1;
                    dy = -1;
                    break;
                case Direction.East:
                    dx = 1;
                    break;
                case Direction.Down:
                    dx = 1;
                    dy = 1;
                    break;
                case Direction.South:
                    dy = 1;
                    break;
                case Direction.Left:
                    dx = -1;
                    dy = 1;
                    break;
                case Direction.West:
                    dx = -1;
                    break;
                case Direction.Up:
                    dx = -1;
                    dy = -1;
                    break;
            }
        }

		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			reflect = ( Utility.Random( 100 ) < m_Rage * 20 );
		}

		private int GetMaxSummons()
		{
			switch( m_Rage )
			{
				case 0: return 12;
				case 1: return 10;
				case 2: return 8;
				case 3: return 6;
				default: return 12;
			}
		}

		public override bool OnBeforeDeath()
		{
        	if ( m_Rage == 0 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Justice shall not falther today!" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				SetDamage( 16, 20 );
				VirtualArmor += 10;	
				m_Rage = 1;
				return false;
			}
			else if ( m_Rage == 1 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "By the heavens above I command thee to stand down!" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				SetDamage( 21, 25 );
				VirtualArmor += 10;
				m_Rage = 2;
				return false;
			}
			else if ( m_Rage == 2 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "For the Skywatch!" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				SetDamage( 26, 30 );
				VirtualArmor += 10;	
				m_Rage = 3;
				return false;
			}
			else 
			{
				Effects.SendLocationParticles( EffectItem.Create( this.Location, this.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
				this.PlaySound( 0x1FE );
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "I return...to the skies..." );
				Mobile killer = this.LastKiller;

            	if (killer != null && killer.Player && killer.Karma < 0)
            	{
            	    int marks = Utility.RandomMinMax(231, 347);
            	    Server.Custom.DefenderOfTheRealm.MarkLootHelper.AwardMarks(killer, 0, marks);
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

			BossLootSystem.AwardBossSpecial(this,BossDrops, 15);
			for ( int i = 0; i < 5; i++ )
			{
				c.DropItem( Loot.RandomArty() );
				c.DropItem( new EtherealPowerScroll() );
				c.DropItem( AscensionScrollFactory.CreateRandom());
			}
			if ( Utility.RandomDouble() < 0.15 )
			{
				c.DropItem( new EternalPowerScroll() );
			}
			BossLootSystem.BossEnchant(this, c, 500, 75, 3, "skyknight");

			// gold explosion
		    RichesSystem.SpawnRiches( m_LastTarget, 5 );
		}

		public override void OnAfterSpawn()
		{
			base.OnAfterSpawn();
			LeechImmune = true;
		}

		public HeavenlyMarshall( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version

			writer.Write( m_Rage );
			writer.Write( m_NextSummonTime );
			writer.Write( m_NextSpecialAttack );
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
			}

			LeechImmune = true;
			// Initialize summons list if null
			if (m_Summons == null)
				m_Summons = new List<BaseCreature>();
		}
	}
}