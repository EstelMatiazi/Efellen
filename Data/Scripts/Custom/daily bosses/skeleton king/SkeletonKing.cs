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
	[CorpseName( "Skeleton King's Corpse" )]
	public class SkeletonKing : BaseCreature
	{
		private static readonly Type[] SummonTypes = new Type[] 
		{ 
			typeof(SkeletonArcher),
			typeof(BoneMagi), 
			typeof(BoneKnight), 
			typeof(Mummy), 
			typeof(MummyLord) 
		};

		private static readonly string[] SummonWarcries = new string[]
		{
			"Seus ossos se juntarão aos meus exércitos!",
			"Logo você estará entre meus súditos!",
			"Nos banquetearmos com sua carne!",
			"O exército real cuidará do seu fim!"
		};
		
		private static readonly List<Type> BossDrops = new List<Type>
    	{
    	    typeof(Artifact_LeoricsBreastplate),
    	    typeof(Artifact_LeoricsCrown),
    	    typeof(Artifact_LeoricsGloves),
    	    typeof(Artifact_LeoricsArms),
			typeof(Artifact_LeoricsLegging),
			typeof(Artifact_LeoricsSword)
    	};

		private int m_Rage = 0;
		private Mobile m_LastTarget;
		private DateTime m_NextSummonTime = DateTime.MinValue;
		private DateTime m_NextSpecialAttack = DateTime.MinValue;
		private List<BaseCreature> m_Summons = new List<BaseCreature>();

		[Constructable]
		public SkeletonKing () : base( AIType.AI_Melee, FightMode.Closest, 20, 1, 0.4, 0.8 )
		{
			Name = "Rei Esqueleto";
            Title = "O Escravo do Inferno";
			Body = 0x147;
			BaseSoundID = 451;
			NameHue = 0x22;
			Hue = 0x09d3;

			SetStr( 696, 685 );
			SetDex( 185, 205 );
			SetInt( 286, 325 );

			SetHits( 7000 );
			SetDamage( 11, 15 );

			SetDamageType( ResistanceType.Physical, 100 );
			SetResistance( ResistanceType.Physical, 75 );
			SetResistance( ResistanceType.Fire, 50 );
			SetResistance( ResistanceType.Cold, 60 );
			SetResistance( ResistanceType.Poison, 75 );
			SetResistance( ResistanceType.Energy, 60 );

			SetSkill( SkillName.MagicResist, 115.0 );
			SetSkill( SkillName.Tactics, 115.0 );
			SetSkill( SkillName.FistFighting, 115.0 );
			SetSkill( SkillName.Anatomy, 115.0);

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

		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			reflect = ( Utility.Random( 100 ) < m_Rage * 12 );
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
			if (Utility.RandomDouble() < 0.55 )
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
                case 1: // Bone blast
                {
                    BossSpecialAttack.PerformSlam(
                       boss: this,
                       warcry: "Compartilhe a recompensa da minha tumba!",
                       hue: 0x09d3,
                       rage: m_Rage+1,
                       range: 6,
                       physicalDmg: 100
                   );
                   break;
                }
                case 2: // Rampage
                {
                   BossSpecialAttack.PerformRampage(
                       boss: this,
                       warcry: "*O Rei Esqueleto se prepara para avançar!*",
                       hue: 0x09d3,
                       rage: m_Rage+1,
                       stunDuration: 4.0
                   );
                   break;
                }
                case 3: // Honor guard
                {
                    BossSpecialAttack.SummonHonorGuard(
                        boss: this,
                        target: target,
                        warcry: "Guarda de honra! Eu vos chamo!",
                        amount: 4,
                        creatureType: typeof(HellKnight),
                        hue: 0x09d3
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
				default: return 8;
			}
		}

		public override bool OnBeforeDeath()
		{
			if ( m_Rage == 0 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "O calor da vida entrou em minha tumba. Prepare-se, mortal, para servir a meu mestre pela eternidade!" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				SetDamage( 16, 21 );
				VirtualArmor += 5;
				m_Rage = 1;
				return false;
			}
			else if ( m_Rage == 1 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Tema a ira de Leoric!" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				SetDamage( 25, 30 );
				VirtualArmor += 5;
				
				m_Rage = 2;
				return false;
			}
			else if ( m_Rage == 2 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "O senhor do terror fará um banquete com sua alma!" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				SetDamage( 26, 35 );
				VirtualArmor += 10;				
				m_Rage = 3;
				return false;
			}
			else 
			{
				Effects.SendLocationParticles( EffectItem.Create( this.Location, this.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
				this.PlaySound( 0x1FE );
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Finalmente... Estou livre..." );
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
            BossSummonSystem.CleanupSummons(m_Summons);
            base.OnDelete();
        }

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			BossLootSystem.AwardBossSpecial(this,BossDrops, 15);
			for ( int i = 0; i < 3; i++ )
			{
				c.DropItem( AscensionScrollFactory.CreateRandom());
				c.DropItem( Loot.RandomArty() );
				c.DropItem( new EtherealPowerScroll() );
			}
			// gold explosion
		    RichesSystem.SpawnRiches( m_LastTarget, 3 );
		}

		public override void OnAfterSpawn()
		{
			base.OnAfterSpawn();
			LeechImmune = true;
		}

		public SkeletonKing( Serial serial ) : base( serial )
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