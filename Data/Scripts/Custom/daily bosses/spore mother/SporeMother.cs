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
	[CorpseName( "Spore Mother's Corpse" )]
	public class SporeMother : BaseCreature
	{
		private static readonly Type[] SummonTypes = new Type[] 
		{ 
			typeof(WhippingVine), 
			typeof(Fungal), 
			typeof(FungalMage), 
			typeof(UmberHulk),
			typeof(WeedElemental)
		};

		private static readonly string[] SummonWarcries = new string[]
		{
			"*Releases spores that animate vines!*",
			"*Causes mushrooms to grow with a psychic surge!*",
			"*A psychic surge brings forth creatures from the underdark!*",
			"*Weeds rise and form into new monstrosities!*"
		};

		private static readonly List<Type> BossDrops = new List<Type>
    	{
    	    typeof(Artifact_MyconidChestplate),
    	    typeof(Artifact_MyconidHelmet),
    	    typeof(Artifact_MyconidGloves),
    	    typeof(Artifact_MyconidLeggings),
			typeof(Artifact_MyconidArms),
    	};

		private int m_Rage = 0;
		private Mobile m_LastTarget;
		private DateTime m_NextSummonTime = DateTime.MinValue;
		private DateTime m_NextSpecialAttack = DateTime.MinValue;
        private List<BaseCreature> m_Summons = new List<BaseCreature>();

		[Constructable]
		public SporeMother () : base( AIType.AI_Mage, FightMode.Closest, 20, 1, 0.4, 0.8 )
		{
			Name = "Spore Mother";
			Title = "The Living Infestation";
			Body = 341;
			NameHue = 0x22;
			Hue = 0x497;

			SetStr( 496, 585 );
			SetDex( 155, 185 );
			SetInt( 286, 375 );

			SetHits( 3000 );
			SetDamage( 11, 15 );

			SetDamageType( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Physical, 40 );
			SetResistance( ResistanceType.Fire, 45 );
			SetResistance( ResistanceType.Cold, 40 );
			SetResistance( ResistanceType.Poison, 70 );
			SetResistance( ResistanceType.Energy, 50 );

			SetSkill( SkillName.Magery, 82.5, 125.0 );
			SetSkill( SkillName.Psychology, 52.5, 85.0 );
			SetSkill( SkillName.Meditation, 82.5, 95.0 );
			SetSkill( SkillName.MagicResist, 75.5, 125.0 );
			SetSkill( SkillName.Tactics, 81.0, 95.0 );
			SetSkill( SkillName.FistFighting, 101.0, 115.0 );

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
		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool BleedImmune{ get{ return true; } }
		public override bool BardImmune { get { return true; } }
		public override bool Unprovokable { get { return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }

		public override int GetAngerSound()
		{
			return 0x451;
		}

		public override int GetIdleSound()
		{
			return 0x452;
		}

		public override int GetAttackSound()
		{
			return 0x453;
		}

		public override int GetHurtSound()
		{
			return 0x454;
		}

		public override int GetDeathSound()
		{
			return 0x455;
		}

		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			reflect = ( Utility.Random( 100 ) < m_Rage * 10 );
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
		        m_Rage+1,
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

			switch ( attackChoice  )
			{
				case 1: // Poison blast
				{
					BossSpecialAttack.PerformSlam(
                       boss: this,
                       warcry: "*Releases a burst of crippling poison!*",
                       hue: 267,
                       rage: m_Rage+1,
                       range: 6,
					   physicalDmg:0,
                       poisonDmg: 100
                   );
                   break;
				}

				case 2: // entangling vines + bleed
				{
					BossSpecialAttack.PerformEntangle(
    				    boss: this,
    				    warcry: "*calls forth piercing vines*",
    				    hue: 0x4F6,
    				    rage: m_Rage+1,
    				    range: 6,
    				    bleedLevel: 5  // 10-15 damage per tick
    				);
    				break;
				}
				case 3: // cross poison
				{
					BossSpecialAttack.PerformCrossExplosion(
				       boss: this,
				       target: target,
				       warcry: "*Spores burst and explode!*",
				       hue: 0x4F6,
				       rage: m_Rage+1,
					   physicalDmg:0,
				       poisonDmg: 100
				   );
				   break;
				}
			}
		}

		

		private int GetMaxSummons()
		{
			switch( m_Rage )
			{
				case 0: return 12;
				case 1: return 10;
				case 2: return 8;
				case 3: return 6;
				default: return 6;
			}
		}

		public override bool OnBeforeDeath()
		{
			if ( m_Rage == 0 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "*releases a psychic shriek!*" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				SetDamage( 16, 21 );
				
				m_Rage = 1;
				return false;
			}
			else if ( m_Rage == 1 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "*releases a crushing psychic scream!*" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				SetDamage( 21, 26 );
				VirtualArmor += 5;
				m_Rage = 2;
				return false;
			}
			else if ( m_Rage == 2 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "*releases an agonizing psychic scream!*" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				SetDamage( 26, 31 );
				VirtualArmor += 5;
				m_Rage = 3;
				return false;
			}
			else
			{
				Effects.SendLocationParticles( EffectItem.Create( this.Location, this.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
				this.PlaySound( 0x1FE );
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "*withers into nothingness...*" );
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

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			m_LastTarget = from;
			if (Utility.RandomDouble() < 0.30 )
				Server.Misc.IntelligentAction.LeapToAttacker( this, from );
			
			base.OnDamage( amount, from, willKill );
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );
			BossLootSystem.AwardBossSpecial(this,BossDrops, 15);
			for ( int i = 0; i < 2; i++ )
			{
				c.DropItem( AscensionScrollFactory.CreateRandom());
				c.DropItem( Loot.RandomArty() );
				c.DropItem( new EtherealPowerScroll() );
			}
			// gold explosion
		    RichesSystem.SpawnRiches( m_LastTarget, 2 );
		}

		public override void OnAfterSpawn()
		{
			base.OnAfterSpawn();
			LeechImmune = true;
		}

		public SporeMother( Serial serial ) : base( serial )
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
			if (m_Summons == null)
					m_Summons = new List<BaseCreature>();
		}
	}
}