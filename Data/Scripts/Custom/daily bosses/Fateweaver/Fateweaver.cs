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
	[CorpseName( "Fateweaver's Corpse" )]
	public class Fateweaver : BaseCreature
	{		
		private static readonly Type[] SummonTypes = new Type[] 
		{ 
			typeof(LargeSpider), 
			typeof(GiantSpider), 
			typeof(GiantBlackWidow),
			typeof(DreadSpider),
			typeof(PhaseSpider)
		};

		private static readonly string[] SummonWarcries = new string[]
		{
			"*Focuses its alien insectoid eyes*",
			"*Screeches violently*",
			"*Diabolically focuses its alien eyes*",
			"*Stares maniacally into the cave ceiling above*"
		};

		private static readonly List<Type> BossDrops = new List<Type>
    	{
    	   	typeof(Artifact_RobeOfTheFateweaver),
			typeof(Artifact_CircletOfTheDreamweaver),
			typeof(Artifact_MantleOfTheFateweaver),
			typeof(Artifact_CloakOfTheFateweaver),
			typeof(Artifact_TalonOfLolth),
			typeof(Artifact_NecklaceOfTheFateweaver)
    	};

		private int m_Rage = 0;
		private Mobile m_LastTarget;
		private DateTime m_NextSummonTime = DateTime.MinValue;
		private DateTime m_NextSpecialAttack = DateTime.MinValue;
		private List<BaseCreature> m_Summons = new List<BaseCreature>();

		[Constructable]
		public Fateweaver () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Fateweaver";

			Body = 0x1cc;
			BaseSoundID = 0x388;
			NameHue = 0x22;
			Hue = 2498;
            Title = "The Mirrorbreaker";
			
			SetStr( 796, 1085 );
			SetDex( 275 );
			SetInt( 505 );

			SetHits( 11000 );
			SetDamage( 11, 15 );

			SetDamageType( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Physical, 50 );
			SetResistance( ResistanceType.Fire, 55 );
			SetResistance( ResistanceType.Cold, 40 );
			SetResistance( ResistanceType.Poison, 90 );
			SetResistance( ResistanceType.Energy, 55 );

			SetSkill( SkillName.Meditation, 102.5, 125.0 );
			SetSkill( SkillName.MagicResist, 125.5, 145.0 );
			SetSkill( SkillName.Tactics, 101.0, 120.0 );
			SetSkill( SkillName.FistFighting, 101.0, 111.0 );
			SetSkill( SkillName.Magery, 101.0, 120.0 );

			Fame = 30000;
			Karma = 30000;

			VirtualArmor = 50;

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
        // pack instinct makes this boss very deadly if summons arent dealt with, which is by design
        public override PackInstinct PackInstinct{ get{ return PackInstinct.Arachnid; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override Poison HitPoison{ get{ return Poison.Lethal; } }

		public override int GetAttackSound(){ return 0x601; }	// A
		public override int GetDeathSound(){ return 0x602; }	// D
		public override int GetHurtSound(){ return 0x603; }		// H

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
		        m_NextSpecialAttack = DateTime.UtcNow + TimeSpan.FromSeconds(30 - (m_Rage * 2));
		    }

		    m_LastTarget = combatant;
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			m_LastTarget = from;
			//higher chance than average from her tier because the weaver is jumpy
			if (Utility.RandomDouble() < 0.85 )
				Server.Misc.IntelligentAction.LeapToAttacker( this, from );

			base.OnDamage( amount, from, willKill );
		}

      	private void PerformRageAttack( Mobile target )
		{
			if ( target == null || target.Deleted || !target.Alive )
				return;

			
			int attackChoice = Utility.RandomMinMax( 1, 3 );

			switch ( attackChoice  )
			{
				case 1: // cross poison
				{
                    BossSpecialAttack.PerformCrossExplosion(
				       this,
				       target,
				       "*Cocoons burst and explode!*",
				       2498,
				       m_Rage+1,
					   0,      // physical
					   0,      // fire
					   0,      // cold
				       100,    // poison
					   0       // energy
				   );
					break;
				}
				case 2: // paralyzing webs + bleed
				{
                    BossSpecialAttack.PerformEntangle(
    				    this,
    				    "*calls forth thick webs*",
    				    2498,
    				    m_Rage+1,
    				    7,
    				    11  // 22-33 damage per tick
    				);
					break;
				}
                case 3: // delayed poison explosion
				{
                    BossSpecialAttack.PerformDelayedExplosion(
				        this,
				        "*spreads poisonous eggs*",
				        2498,   // hue
				        12,     // radius
				        m_Rage+1,
				        0,      // physical
				        0,      // fire
				        0,      // cold
				        100,    // poison
				        0       // energy
				    );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "*Stares defiantly*" );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "*Jumps forward in anticipation*" );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "*Thrashes around maniacly*" );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "*Twitches and falls one last time*" );
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
			int amount = Utility.Random(3,6);
			c.DropItem(new EssenceOfLolthsHatred(amount));
			// gold explosion
			RichesSystem.SpawnRiches( m_LastTarget, 4 );
            // wildshape totem
			Mobile killer = this.LastKiller;
            TotemDropHelper.TryDropTotem(
		        killer,
		        this,
		        "Monstrous Spider",
		        120.0,
		        0.75
		    );
		}

        public Fateweaver( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 2 ); // version

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