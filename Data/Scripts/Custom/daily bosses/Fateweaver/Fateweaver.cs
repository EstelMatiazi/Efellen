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
			typeof(Artifact_MantleOfTheFateweaver),
			typeof(Artifact_CloakOfTheFateweaver),
			typeof(Artifact_TalonOfLolth),
			typeof(Artifact_NecklaceOfTheFateweaver)
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

			SetHits( 33000 );
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
			IsBoss = true;
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
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Arachnid; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override Poison HitPoison{ get{ return Poison.Lethal; } }

		public override int GetAttackSound(){ return 0x601; }
		public override int GetDeathSound(){ return 0x602; }
		public override int GetHurtSound(){ return 0x603; }

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
			
			Server.Misc.IntelligentAction.TryToLeapToAttacker( this, from, 0.2, ref m_NextLeap );

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
			PublicOverheadMessage( MessageType.Regular, 0x21, false, "*Stares defiantly*" );
			this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
			this.PlaySound( 0x202 );
			SetDamage( 16, 20 );
			VirtualArmor += 5;
		}

		private void ApplyRage2()
		{
			PublicOverheadMessage( MessageType.Regular, 0x21, false, "*Jumps forward in anticipation*" );
			this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
			this.PlaySound( 0x202 );
			SetDamage( 21, 25 );
			VirtualArmor += 10;
		}

		private void ApplyRage3()
		{
			PublicOverheadMessage( MessageType.Regular, 0x21, false, "*Thrashes around maniacly*" );
			this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
			this.PlaySound( 0x202 );
			SetDamage( 26, 30 );
			VirtualArmor += 15;
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
                    BossSpecialAttack.PerformCrossExplosion(
				       this,
				       target,
				       "*Cocoons burst and explode!*",
				       2498,
				       m_Rage+1,
					   0,
					   0,
					   0,
				       100,
					   0
				   );
					break;
				}
				case 2:
				{
                    BossSpecialAttack.PerformEntangle(
    				    this,
    				    "*calls forth thick webs*",
    				    2498,
    				    m_Rage+1,
    				    7,
    				    11
    				);
					break;
				}
                case 3:
				{
                    BossSpecialAttack.PerformDelayedExplosion(
				        this,
				        "*spreads poisonous eggs*",
				        2498,
				        12,
				        m_Rage+1,
				        0,
				        0,
				        0,
				        100,
				        0
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
			BossLootSystem.AwardBossMarks(this, this.LastKiller, 156, 223, "*Twitches and falls one last time*");
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
			for ( int i = 0; i < 4; i++ )
			{
				c.DropItem( Loot.RandomArty() );
				c.DropItem( new EtherealPowerScroll() );
				c.DropItem( AscensionScrollFactory.CreateRandom());
			}
			int amount = Utility.Random(3, 6);
			c.DropItem(new EssenceOfLolthsHatred(amount));
			RichesSystem.SpawnRiches( m_LastTarget, 4 );
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
			writer.Write( (int) 3 );

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

			if ( version >= 3 )
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