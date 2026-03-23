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
using Server.CustomSpells;
using Server.Custom.Ascensions;

namespace Server.Mobiles
{
	[CorpseName( "Kamina's Corpse" )]
	public class BalTsareth : BaseSpellCaster
	{		
		private static readonly Type[] SummonTypes = new Type[] 
		{ 
			typeof(Imp), 
			typeof(AnyElemental), 
			typeof(AnyGemElemental) 
		};

		private static readonly string[] SummonWarcries = new string[]
		{
			"The weave answers to me!",
			"My magic has outlasted civilizations, taste it!",
			"This is MY HOME! MINE!",
			"I've forgotten magic that your civilization is yet to learn!"
		};

		private static readonly List<Type> BossDrops = new List<Type>
    	{
    	    typeof(Artifact_PrismaticRobeOfBalTsareth),
    	    typeof(Artifact_PrismaticGlassesOfBalTsareth),
    	    typeof(Artifact_PrismaticCapeOfBalTsareth),
    	    typeof(Artifact_PrismaticBootsOfBalTsareth),
            typeof(Artifact_PrismaticRingOfBalTsareth)
    	};

		private int m_Rage;
		private Mobile m_LastTarget;
		private DateTime m_NextSummonTime;
		private DateTime m_NextSpecialAttack;
		private List<BaseCreature> m_Summons;

		[Constructable]
		public BalTsareth () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Bal Tsareth";
            Body = 401; 
			Utility.AssignRandomHair( this );
			HairHue = Utility.RandomHairHue();
            Hue = Utility.RandomSkinHue(); 
			NameHue = 0x22;
			Title = "The Ancient Lorekeeper";
			
			SetStr( 596, 785 );
			SetDex( 165, 225 );
			SetInt( 556, 655 );
			SetHits( 10000 );
			SetDamage( 11, 15 );
			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Poison, 20 );
			SetDamageType( ResistanceType.Cold, 20 );
			SetDamageType( ResistanceType.Fire, 20 );
			SetDamageType( ResistanceType.Energy, 20 );
			SetResistance( ResistanceType.Physical, 45 );
			SetResistance( ResistanceType.Fire, 70 );
			SetResistance( ResistanceType.Cold, 70 );
			SetResistance( ResistanceType.Poison, 70 );
			SetResistance( ResistanceType.Energy, 70 );
			SetSkill( SkillName.Meditation, 102.5, 125.0 );
			SetSkill( SkillName.MagicResist, 125.5, 145.0 );
			SetSkill( SkillName.Tactics, 101.0, 110.0 );
			SetSkill( SkillName.FistFighting, 91.0 );
			SetSkill( SkillName.Bludgeoning, 101.0, 111.0 );
			SetSkill( SkillName.Magery, 101.0, 110.0 );
			SetSkill( SkillName.Psychology, 101.0, 110.0 );

			Fame = 30000;
			Karma = -30000;
			VirtualArmor = 50;

			PackItem( new EerieIdol(Utility.Random(12,26)) );
            AddItem( new ScholarRobe{ Hue = 0x0213 } );
            AddItem( new Sandals{ Hue = 0x0213 } );
            AddItem( new BlackStaff{ Hue = 0x0213 } );

            m_NextSummonTime = DateTime.MinValue;
			m_NextSpecialAttack = DateTime.MinValue;
			m_Summons = new List<BaseCreature>();
          }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 6 );
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

        public override bool AlwaysAttackable{ get{ return true; } }
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
			if (Utility.RandomDouble() < 0.25 )
				TryWeaveStep();

			base.OnDamage( amount, from, willKill );
		}

        private void TryWeaveStep()
        {
            Map map = Map;

            if ( map == null )
                return;

            for ( int i = 0; i < 10; i++ )
            {
                int x = X + Utility.RandomMinMax( -6, 6 );
                int y = Y + Utility.RandomMinMax( -6, 6 );
                int z = map.GetAverageZ( x, y );
                Point3D p = new Point3D( x, y, z );

                if ( map.CanSpawnMobile( p ) )
                {
                    Location = p;
                    PublicOverheadMessage( MessageType.Emote, 0x3B2, false, "*Steps into the weave*" );
                    Effects.SendLocationEffect( p, map, 0x3728, 13, 10, 0, 0 );
                    Effects.PlaySound( p, map, 0x1FE );
                    break;
                }
            }
        }

		private void PerformRageAttack( Mobile target )
		{
			if ( target == null || target.Deleted || !target.Alive )
				return;

			int attackChoice = Utility.RandomMinMax( 1, 3 );

			switch ( attackChoice )
			{
				case 1:
					BossSpecialAttack.PerformSlam( this, "MINE! ALL OF THESE SECRETS ARE MINE!", 0x0213, m_Rage+1, 6, 20, 20, 20, 20, 20 );
					break;
				case 2:
					BossSpecialAttack.PerformDegenAura( this, "You dare to attack me? ME? IN MY HOME?", 8, m_Rage+1, 16, 29, "mana", 0x0213 );
					break;
				case 3:
					Type summonType;
					switch ( Utility.Random( 3 ) )
					{
						case 0:
							summonType = typeof( IceGiant );
							break;
						case 1:
							summonType = typeof( StormGiant );
							break;
						default:
							summonType = typeof( LavaGiant );
							break;
					}

					BossSpecialAttack.SummonHonorGuard( this, target, "The elements are mine! I unravelled their secrets before your kingdom was born!", m_Rage+1, summonType, 0x0213 );
					break;
			}
		}

		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			reflect = ( Utility.Random( 100 ) < m_Rage * 16 );
		}

		private int GetMaxSummons()
		{
			switch ( m_Rage )
			{
				case 1: return 8;
				case 2: return 7;
				case 3: return 6;
				default: return 6;
			}
		}

		public override bool OnBeforeDeath()
		{
			if ( m_Rage == 0 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "You will forever guard my tomb!" );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Surrender your mind to me!" );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "I WILL DESTROY YOU!" );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "I shall rise again...in another thousand years..." );
				Mobile killer = this.LastKiller;
				if ( killer != null && killer.Player && killer.Karma > 0 )
            	{
            	    int marks = Utility.RandomMinMax( 156, 223 );
            	    Server.Custom.DefenderOfTheRealm.MarkLootHelper.AwardMarks( killer, 1, marks );
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

			BossLootSystem.AwardBossSpecial( this, BossDrops, 15 );
			for ( int i = 0; i < 4; i++ )
			{
				c.DropItem( Loot.RandomArty() );
				c.DropItem( new EtherealPowerScroll() );
				c.DropItem( AscensionScrollFactory.CreateRandom());
			}

			RichesSystem.SpawnRiches( m_LastTarget, 4 );
		}

		public override void OnAfterSpawn()
		{
			base.OnAfterSpawn();
			this.MobileMagics(7, SpellType.Wizard, 0x0213);
			LeechImmune = true;
		}

		public BalTsareth( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 2 );
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
			if(version>=2)
			{
				this.MobileMagics(7, SpellType.Wizard, 0x0213);
			}
			LeechImmune = true;

			if ( m_Summons == null )
				m_Summons = new List<BaseCreature>();
		}
	}
}