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
    	    typeof(Artifact_ExaltedGauntletsOfDevotion),
    	    typeof(Artifact_ExaltedLeggingsOfDevotion),
    	    typeof(Artifact_ExaltedTunicOfDevotion),
    	    typeof(Artifact_ExaltedArmsOfDevotion),
			typeof(Artifact_ExaltedCoifOfDevotion),
			typeof(Artifact_HolySword),
			typeof(Artifact_BeadsOfPrayer)
    	};

		private int m_Rage = 0;
		private Mobile m_LastTarget;
		private DateTime m_NextSummonTime = DateTime.MinValue;
		private DateTime m_NextSpecialAttack = DateTime.MinValue;
		private DateTime m_NextLeap = DateTime.MinValue;
        private List<BaseCreature> m_Summons = new List<BaseCreature>();

		private bool m_Rage1Applied = false;
		private bool m_Rage2Applied = false;
		private bool m_Rage3Applied = false;

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

			SetHits( 57000 );
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
			IsBoss = true;
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

			Server.Misc.IntelligentAction.TryToLeapToAttacker( this, from, 0.15, ref m_NextLeap );
			
			if (from != null && from.Player && from.Kills < 5 && !from.Criminal) 
				from.Criminal = true;		
		
			base.OnDamage( amount, from, willKill );

			CheckRageThresholds();
		}

		private void CheckRageThresholds()
		{
			if (this.HitsMax <= 0)
				return;

			double hpPercent = (double)this.Hits / (double)this.HitsMax;

			if (!m_Rage1Applied && hpPercent <= 0.75)
			{
				m_Rage1Applied = true;
				m_Rage = 1;
				ApplyRage1();
			}
			else if (!m_Rage2Applied && hpPercent <= 0.50)
			{
				m_Rage2Applied = true;
				m_Rage = 2;
				ApplyRage2();
			}
			else if (!m_Rage3Applied && hpPercent <= 0.25)
			{
				m_Rage3Applied = true;
				m_Rage = 3;
				ApplyRage3();
			}
		}

		private void ApplyRage1()
		{
			PublicOverheadMessage( MessageType.Regular, 0x21, false, "Justice shall not falther today!" );
			this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
			this.PlaySound( 0x202 );
			SetDamage( 16, 20 );
			VirtualArmor += 10;
		}

		private void ApplyRage2()
		{
			PublicOverheadMessage( MessageType.Regular, 0x21, false, "By the heavens above I command thee to stand down!" );
			this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
			this.PlaySound( 0x202 );
			SetDamage( 21, 25 );
			VirtualArmor += 10;
		}

		private void ApplyRage3()
		{
			PublicOverheadMessage( MessageType.Regular, 0x21, false, "For the Skywatch!" );
			this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
			this.PlaySound( 0x202 );
			SetDamage( 26, 30 );
			VirtualArmor += 10;
		}

		private void PerformRageAttack( Mobile target )
		{
			if ( target == null || target.Deleted || !target.Alive )
				return;

			int attackChoice = Utility.RandomMinMax( 1, 3 );
            Map map = this.Map;

			switch ( attackChoice )
			{
				case 1:
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
				case 2:
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
				case 3:
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
                case Direction.North:     dy = -1; break;
                case Direction.Right: dx = 1; dy = -1; break;
                case Direction.East:      dx = 1; break;
                case Direction.Down:  dx = 1;  dy = 1; break;
                case Direction.South:     dy = 1; break;
                case Direction.Left:  dx = -1; dy = 1; break;
                case Direction.West:      dx = -1; break;
                case Direction.Up:    dx = -1; dy = -1; break;
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
			BossLootSystem.AwardBossMarks(this, this.LastKiller, 231, 347, "The skyes...I can feel...");
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

			BossLootSystem.AwardBossSpecial(this, BossDrops, 30);
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
			writer.Write( (int) 2 );

			writer.Write( m_Rage );
			writer.Write( m_NextSummonTime );
			writer.Write( m_NextSpecialAttack );
			writer.Write( m_Rage1Applied );
			writer.Write( m_Rage2Applied );
			writer.Write( m_Rage3Applied );
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
				m_Rage1Applied = reader.ReadBool();
				m_Rage2Applied = reader.ReadBool();
				m_Rage3Applied = reader.ReadBool();
			}
			else
			{
				m_Rage1Applied = m_Rage >= 1;
				m_Rage2Applied = m_Rage >= 2;
				m_Rage3Applied = m_Rage >= 3;
			}

			LeechImmune = true;
			if (m_Summons == null)
				m_Summons = new List<BaseCreature>();
		}
	}
}