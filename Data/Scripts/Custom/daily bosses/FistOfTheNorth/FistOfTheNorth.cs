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
using Server.EffectsUtil;
using Server.Custom;
using Server.Custom.DailyBosses.System;
using Server.Custom.BossSystems;
using Server.Custom.Ascensions;
namespace Server.Mobiles
{
	[CorpseName( "Hrimah's Corpse" )]
	public class FistOfTheNorth : BaseCreature
	{
		private static readonly Type[] SummonTypes = new Type[] 
		{ 
			typeof(FrostTroll),
			typeof(Giant),
			typeof(HillGiant),  
			typeof(FrostGiant), 
			typeof(IceGiant) 
		};

		private static readonly string[] SummonWarcries = new string[]
		{
			"Kin of frost, I call thee!",
			"This land belongs to us! Thou shall regret invading it!",
			"We shall make trophies of thy bones!",
			"Brothers, prey approaches!"
		};
		
		private static readonly List<Type> BossDrops = new List<Type>
    	{
    	    typeof(Artifact_BreastplateOfTheNorth),
    	    typeof(Artifact_CrownOfTheNorth),
    	    typeof(Artifact_GlovesOfTheNorth),
    	    typeof(Artifact_ArmsOfTheNorth),
			typeof(Artifact_LeggingsOfTheNorth),
			typeof(Artifact_FistOfTheNorth)
    	};

		private int m_Rage = 0;
		private Mobile m_LastTarget;
		private DateTime m_NextSummonTime = DateTime.MinValue;
		private DateTime m_NextSpecialAttack = DateTime.MinValue;
		private List<BaseCreature> m_Summons = new List<BaseCreature>();

		[Constructable]
		public FistOfTheNorth () : base( AIType.AI_Melee, FightMode.Closest, 20, 1, 0.4, 0.8 )
		{
			Name = "Hrimah";
            Title = "The fist of the north";
			Body = 0x20;
			BaseSoundID = 609;
			NameHue = 0x22;
			
			SetStr( 696, 685 );
			SetDex( 185, 205 );
			SetInt( 286, 325 );

			SetHits( 7000 );
			SetDamage( 11, 15 );

			SetDamageType( ResistanceType.Physical, 100 );
			SetResistance( ResistanceType.Physical, 75 );
			SetResistance( ResistanceType.Fire, 40 );
			SetResistance( ResistanceType.Cold, 80 );
			SetResistance( ResistanceType.Poison, 55 );
			SetResistance( ResistanceType.Energy, 60 );

			SetSkill( SkillName.MagicResist, 105.0 );
			SetSkill( SkillName.Tactics, 125.0 );
			SetSkill( SkillName.FistFighting, 125.0 );
			SetSkill( SkillName.Anatomy, 105.0);

			Fame = 25000;
			Karma = -25000;

			VirtualArmor = 40;

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 4 );
		}

		public override int TreasureMapLevel{ get{ return 4; } }
		public override bool CanRummageCorpses{ get{ return false; } }
		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool BleedImmune{ get{ return true; } }
		public override bool BardImmune { get { return true; } }
		public override bool Unprovokable { get { return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }

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
		        40
		    );

		    if (DateTime.UtcNow >= m_NextSpecialAttack)
		    {
		        PerformRageAttack(combatant);
		        m_NextSpecialAttack = DateTime.UtcNow + TimeSpan.FromSeconds(20 - (m_Rage * 2));
		    }

		    m_LastTarget = combatant;
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			m_LastTarget = from;
			if (Utility.RandomDouble() < 0.65 )
				Server.Misc.IntelligentAction.LeapToAttacker( this, from );

			base.OnDamage( amount, from, willKill );
		}

        private void PerformRageAttack( Mobile target )
        {
            if ( target == null || target.Deleted || !target.Alive )
                return;

            
            int attackChoice = Utility.RandomMinMax( 1, 3 );
            Map map = this.Map;

            switch ( attackChoice )
            {
                case 1: // Rampage
                {
                   BossSpecialAttack.PerformRampage(
                       boss: this,
                       warcry: "*Hrimah chaarges!*",
                       hue: 1153,
                       rage: m_Rage+1,
                       stunDuration: 4.0
                   );
                   break;
                }
				case 2: // Honor guard
                {
                    BossSpecialAttack.SummonHonorGuard(
                        boss: this,
                        target: target,
                        warcry: "Come hither, my kin!",
                        amount: 5,
                        creatureType: typeof(IceGiant),
                        hue: 1153
                    );
                    break;
                }
				case 3: // blast
                {
                    BossSpecialAttack.PerformSlam(
                       boss: this,
                       warcry: "Frost shall end thee!",
                       hue: 1153,
                       rage: m_Rage+1,
                       range: 6,
                       physicalDmg: 0,
					   coldDmg: 100
                   );
                   break;
                }
                
            }
        }

	
		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			reflect = ( Utility.Random( 100 ) < m_Rage * 12 );
		}

		private int GetMaxSummons()
		{
			switch( m_Rage )
			{
				case 0: return 10;
				case 1: return 8;
				case 2: return 6;
				case 3: return 4;
				default: return 4;
			}
		}

		public override bool OnBeforeDeath()
		{
			if ( m_Rage == 0 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "For winter's biting blessing!" );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Cold shall be thy tomb!" );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Hell shall freeze over before you can defeat me!" );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Ancestors...Here I come..." );
				Mobile killer = this.LastKiller;
				if (killer != null && killer.Player && killer.Karma > 0)
            	{
            	    int marks = Utility.RandomMinMax(103, 110);
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

			BossLootSystem.AwardBossSpecial(this,BossDrops, 15);
			for ( int i = 0; i < 3; i++ )
			{
				c.DropItem( Loot.RandomArty() );
				c.DropItem( new EtherealPowerScroll() );
				c.DropItem( AscensionScrollFactory.CreateRandom());
			}
			// gold explosion
		    RichesSystem.SpawnRiches( m_LastTarget, 3 );
		}

		public override void OnAfterSpawn()
		{
			base.OnAfterSpawn();
			LeechImmune = true;
		}

		public FistOfTheNorth( Serial serial ) : base( serial )
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