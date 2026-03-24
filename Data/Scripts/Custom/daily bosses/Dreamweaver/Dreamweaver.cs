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
using Server.Custom.BeholderSpecials;
using Server.Custom.DailyBosses.System;
using Server.Custom.BossSystems;
using Server.Custom.Ascensions;

namespace Server.Mobiles
{
	[CorpseName( "Dreamweaver's Corpse" )]
	public class Dreamweaver : BaseCreature
	{		
		private static readonly Type[] SummonTypes = new Type[] 
		{ 
			typeof(Gazer), 
			typeof(ElderGazer), 
			typeof(Beholder) 
		};

		private static readonly string[] SummonWarcries = new string[]
		{
			"*Foca seus olhos alienígenas*",
			"*Foca intensamente seus olhos alienígenas*",
			"*Foca diabolicamente seus olhos alienígenas*",
			"*Encara o vazio maniacamente*"
		};

		private static readonly List<Type> BossDrops = new List<Type>
    	{
    	    typeof(Artifact_RobeOfTheDreamweaver),
    	    typeof(Artifact_CircletOfTheDreamweaver),
    	    typeof(Artifact_CapeOfTheDreamweaver),
    	    typeof(Artifact_BootsofTheDreamweaver),
			typeof(Artifact_TalonOfNightmares),
            typeof(Artifact_RingOfTheDreamweaver)
    	};

		private int m_Rage = 0;
		private Mobile m_LastTarget;
		private DateTime m_NextSummonTime = DateTime.MinValue;
		private DateTime m_NextSpecialAttack = DateTime.MinValue;
		private DateTime m_NextSpecialBeholderAttack = DateTime.MinValue;
		private List<BaseCreature> m_Summons = new List<BaseCreature>();

		[Constructable]
		public Dreamweaver () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Dreamweaver";

			Body = 674;
			BaseSoundID = 377;
			NameHue = 0x22;
			Hue = 0x96;
            Title = "O Pastor da Imundície";
			
			SetStr( 796, 885 );
			SetDex( 165, 225 );
			SetInt( 506, 605 );

			SetHits( 11000 );
			SetDamage( 11, 15 );

			SetDamageType( ResistanceType.Energy, 100 );
			SetResistance( ResistanceType.Physical, 50 );
			SetResistance( ResistanceType.Fire, 75 );
			SetResistance( ResistanceType.Cold, 70 );
			SetResistance( ResistanceType.Poison, 70 );
			SetResistance( ResistanceType.Energy, 75 );

			SetSkill( SkillName.Meditation, 102.5, 125.0 );
			SetSkill( SkillName.MagicResist, 125.5, 145.0 );
			SetSkill( SkillName.Tactics, 101.0, 120.0 );
			SetSkill( SkillName.FistFighting, 101.0, 111.0 );
			SetSkill( SkillName.Magery, 101.0, 120.0 );

			Fame = 30000;
			Karma = 30000;

			VirtualArmor = 50;

			m_NextSpecialBeholderAttack = DateTime.UtcNow;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 6 );
		}

		public override int TreasureMapLevel{ get{ return 4; } }
		public override bool CanRummageCorpses{ get{ return false; } }
		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool BleedImmune{ get{ return true; } }
		public override bool BardImmune { get { return true; } }
		public override bool Unprovokable { get { return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			m_LastTarget = from;
			Server.Misc.IntelligentAction.LeapToAttacker( this, from );
			
			if ( DateTime.UtcNow >= m_NextSpecialBeholderAttack && from != null && from.Alive && !willKill )
			{
				if ( Utility.RandomDouble() < 0.50 )
				{
					TriggerEyestalkAttack( from );
					m_NextSpecialBeholderAttack = DateTime.UtcNow + TimeSpan.FromSeconds( 33 );
				}
			}
			
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
		        35
		    );

		    if (DateTime.UtcNow >= m_NextSpecialAttack)
		    {
		        PerformRageAttack(combatant);
		        m_NextSpecialAttack = DateTime.UtcNow + TimeSpan.FromSeconds(25 - (m_Rage * 2));
		    }

		    m_LastTarget = combatant;
		}

        private int getParalyzeDuration(Mobile m)
		{
			int resist = (int)(m.Skills.MagicResist.Value);
			// 2s at 125, 8s at 0 magic resist
			int duration = 8 - (int)(resist * (6.0 / 125.0));
			return Math.Max(2, Math.Min(8, duration));
		}

		private void PerformRageAttack( Mobile target )
		{
			if ( target == null || target.Deleted || !target.Alive )
				return;

			
			int attackChoice = Utility.RandomMinMax( 1, 3 );

			switch ( attackChoice  )
			{
				case 1: // energy blast
				{
					BossSpecialAttack.PerformSlam(
                       boss: this,
                       warcry: "*Encara ferozmente em todas as direções*",
                       hue: 0x36B0,
                       rage: m_Rage+1,
                       range: 6,
					   physicalDmg:0,
                       energyDmg: 100
                   );
                   break;
				}
				case 2: // Rage 2: psychic blast (stamina drain + damage)
				{
					PublicOverheadMessage( MessageType.Regular, 0x21, false, "*Estremece com poder*" );
					PlaySound( 0x228 );
					FixedParticles( 0x3789, 10, 25, 5032, EffectLayer.Head );
					
					List<Mobile> targets = new List<Mobile>();
					IPooledEnumerable eable = GetMobilesInRange( 8 );
					
					foreach ( Mobile m in eable )
					{
						if ( m != this && m.Player && m.Alive && CanBeHarmful( m ) )
							targets.Add(m);
					}
					eable.Free();
					
					foreach ( Mobile m in targets )
					{
						DoHarmful( m );
						int staminaDrain = Utility.RandomMinMax( 75, 95 );
						m.Stam -= staminaDrain;
						int damage = Utility.RandomMinMax( staminaDrain/2, staminaDrain*2 ) + m_Rage*3;
						AOS.Damage( m, this, damage, 0, 0, 0, 0, 100 );
						m.FixedParticles( 0x374A, 10, 15, 5013, 0x81b, 0, EffectLayer.Waist );
						m.PlaySound( 0x1FB );
						this.Stam = Math.Min( this.StamMax, this.Stam + staminaDrain / 3 );
						m.Paralyze( TimeSpan.FromSeconds( getParalyzeDuration( m ) + Utility.RandomMinMax(1,3 ) ) );
					}
					
					SlamVisuals.SlamVisual(this, 6, 0x36B0, 0x25);
					break;
				}
                case 3: // telekinetic crush - move away + damage + paralyze
				{
                    if (this == null || this.Deleted)
        		        return;
        	        PublicOverheadMessage( MessageType.Regular, 0x21, false, "*Estremece de raiva*" );

					List<Mobile> victims = new List<Mobile>();
					foreach (Mobile victim in this.GetMobilesInRange(9))
        	        {
        	        	if (victim == null || victim.Deleted || !victim.Alive || victim == this)
        	        		continue;
        	        	if (victim.Combatant != this && this.Combatant != victim)
        	        		continue;
        	        	if (!this.InLOS(victim))
        	        		continue;
						victims.Add(victim);
					}

					foreach (Mobile victim in victims)
					{
        	        	int resist = (int)victim.Skills.MagicResist.Value;
        	        	int distance = 10 - (int)(resist * (2.0 / 125.0));
        	        	distance = Math.Max(5, Math.Min(10, distance));
        	        	
						Direction d = this.GetDirectionTo(victim);
        	        	Point3D newLoc = victim.Location;
        	        	bool moved = false;
        	        	
						for (int i = 0; i < distance; i++)
        	        	{
        	        		int x = newLoc.X;
        	        		int y = newLoc.Y;
        	        		Movement.Movement.Offset(d, ref x, ref y);
        	        		Point3D testLoc = new Point3D(x, y, newLoc.Z);
        	        		if (victim.Map.CanSpawnMobile(testLoc))
        	        		{
        	        			newLoc = testLoc;
        	        			moved = true;
        	        		}
        	        		else
        	        		{
        	        			break;
        	        		}
        	        	}
        	        	
						if (moved && newLoc != victim.Location)
        	        	{
                            victim.MoveToWorld(newLoc, victim.Map);
        	        		victim.PlaySound(0x204);
                            DoHarmful( victim );
							int damage = Utility.RandomMinMax( 41, 52 ) + m_Rage * 3;
							AOS.Damage( victim, this, damage, 0, 0, 0, 0, 100 );
							victim.PlaySound( 0x1FB );
							victim.Paralyze( TimeSpan.FromSeconds( getParalyzeDuration( victim ) * 1.5 ) );
        	        		victim.FixedParticles(0x3728, 10, 10, 0x1F4, 0, 5029, 0);
                            BeholderSpecials.DoRayEffect(
                            	this,
                            	victim,
                            	0x36D4,
                            	1153,
                            	10
                            );
        	        		victim.SendMessage("Uma força telecinética o esmaga!");
        	        	}
        	        }
        	       break;
				}
			}
		}

		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			reflect = ( Utility.Random( 100 ) < m_Rage * 16 );
		}

		private int GetMaxSummons()
		{
			switch( m_Rage )
			{
				case 0: return 8;
				case 1: return 7;
				case 2: return 6;
				case 3: return 5;
				default: return 5;
			}
		}

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			if ( DateTime.UtcNow >= m_NextSpecialBeholderAttack && defender != null && defender.Alive )
			{
				if ( Utility.RandomDouble() < 0.30 )
				{
					TriggerEyestalkAttack( defender );
					m_NextSpecialBeholderAttack = DateTime.UtcNow + TimeSpan.FromSeconds( 30 );
				}
			}
		}

		public override bool OnBeforeDeath()
		{
			if ( m_Rage == 0 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "*Encara com tédio*" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				SetDamage( 16, 20 );
				VirtualArmor += 5;
				m_Rage = 1;
				return false;
			}
			else if ( m_Rage == 1 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "*Faz beicinho irritado*" );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "*Morde a língua e cospe sangue negro*" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				SetDamage( 26, 30 );
				VirtualArmor += 15;
				m_Rage = 3;
				return false;
			}
			else 
			{
				Effects.SendLocationParticles( EffectItem.Create( this.Location, this.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
				this.PlaySound( 0x1FE );
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "*Encara em descrença*" );
				Mobile killer = this.LastKiller;
				if (killer != null && killer.Player && killer.Karma > 0)
            	{
            	    int marks = Utility.RandomMinMax(156, 223);
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
			for ( int i = 0; i < 4; i++ )
			{
				c.DropItem( Loot.RandomArty() );
				c.DropItem( new EtherealPowerScroll() );
				c.DropItem( AscensionScrollFactory.CreateRandom());
			}
			if ( Utility.RandomDouble() < 0.15 )
			{
				c.DropItem( new EternalPowerScroll() );
			}
			// gold explosion
			RichesSystem.SpawnRiches( m_LastTarget, 4 );
		}

        private void TriggerEyestalkAttack( Mobile target )
		{
			int choice = Utility.Random( 6 );

			switch ( choice )
			{
				case 0:
				{
					if ( BeholderSpecials.AntiMagicEye( this, 80, 45, target ) )
					{
						this.Say( "*Foca seu olho antimagia em {0}*", target.Name );
					}
					break;
				}
				case 1:
				{
					if ( BeholderSpecials.Disintegration( this, 100, 90, target ) )
					{
						this.Say( "*Dispara um raio de desintegração em {0}*", target.Name );
					}
					break;
				}
				case 2:
				{
					if ( BeholderSpecials.Petrification( this, 30, target ) )
					{
						this.Say( "*Petrifica {0} com seu olhar*", target.Name );
					}
					break;
				}
				case 3:
				{
					if ( BeholderSpecials.Fear( this, 60, target ) )
					{
						this.Say( "*Infunde medo em {0}*", target.Name );
					}
					break;
				}
				case 4:
				{
					if ( BeholderSpecials.TelekineticRay( this, 9, 40 ) )
					{
						this.Say( "*Uma onda de energia telecinética emana de um olho!*" );
					}
					break;
				}
				case 5:
				{
					if ( BeholderSpecials.DeathRay( this, target, 31, 10, 90 ) )
					{
						this.Say( "*Dispara um raio necrótico em {0}*", target.Name );
					}
					break;
				}
			}
		}

		public override void OnAfterSpawn()
		{
			base.OnAfterSpawn();
			LeechImmune = true;
		}

         public override int GetDeathSound()
        {
            return 0x56F;
        }
 
        public override int GetAttackSound()
        {
            return 0x570;
        }
 
        public override int GetIdleSound()
        {
            return 0x571;
        }
 
        public override int GetAngerSound()
        {
            return 0x572;
        }
 
        public override int GetHurtSound()
        {
            return 0x573;
        }

		public Dreamweaver( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 2 ); // version

			writer.Write( m_Rage );
			writer.Write( m_NextSummonTime );
			writer.Write( m_NextSpecialAttack );
			writer.Write( m_NextSpecialBeholderAttack );
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

			if ( version >= 2 )
			{
				m_NextSpecialBeholderAttack = reader.ReadDateTime();
			}

			LeechImmune = true;
			
			// Initialize summons list if null
			if (m_Summons == null)
				m_Summons = new List<BaseCreature>();
		}
	}
}