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
using Server.Custom.Ascensions;

namespace Server.Mobiles
{
	[CorpseName( "Butcher's Corpse" )]
	public class UndeadButcher : BaseCreature
	{
		private int m_Rage = 0;
		private Mobile m_LastTarget;
		private DateTime m_NextSpecialAttack = DateTime.MinValue;

		private static readonly List<Type> BossDrops = new List<Type>
    	{
    	    typeof(Artifact_ButchersSaw),
    	    typeof(Artifact_ButchersGrinder),
    	    typeof(Artifact_ButchersCleaver),
    	    typeof(Artifact_ButchersCaress),
			typeof(ButchersViolence)
    	};
        
		[Constructable]
		public UndeadButcher () : base( AIType.AI_Melee, FightMode.Closest, 20, 1, 0.4, 0.8 )
		{
			Title = " the Hungry";
			NameHue = 0x92E;
            Body = 999;
            Hue = 0x845;
			BaseSoundID = 684;
			Name = "Butcher";
			SetStr( 366, 455 );
			SetDex( 125 );
			SetInt( 106, 135 );

			SetHits( 1755 );
			SetDamage( 11, 15 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 60 );
			SetResistance( ResistanceType.Fire, 45 );
			SetResistance( ResistanceType.Cold, 45 );
			SetResistance( ResistanceType.Poison, 70 );
			SetResistance( ResistanceType.Energy, 35 );

			SetSkill( SkillName.Anatomy, 95.5, 125.0 );
			SetSkill( SkillName.MagicResist, 95.5, 125.0 );
			SetSkill( SkillName.Tactics, 101.0, 125.0 );
			SetSkill( SkillName.FistFighting, 111.0, 121.0 );

			Fame = 13000;
			Karma = -15000;

			VirtualArmor = 20;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 2 );
		}


		public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
        }


		public override int TreasureMapLevel{ get{ return 2; } }
		public override bool CanRummageCorpses{ get{ return false; } }
		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool BleedImmune{ get{ return true; } }
		public override bool BardImmune { get { return true; } }
		public override bool Unprovokable { get { return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }
		public override Poison HitPoison{ get{ return Poison.Greater; } }

		public override void OnThink()
		{
		    base.OnThink();

		    Mobile combatant = this.Combatant;

		    if (combatant == null || combatant.Deleted || !combatant.Alive)
		        return;

		    if (DateTime.UtcNow >= m_NextSpecialAttack)
		    {
		        PerformRageAttack(combatant);
		        m_NextSpecialAttack = DateTime.UtcNow + TimeSpan.FromSeconds(25 - (m_Rage * 2));
		    }

		    m_LastTarget = combatant;
		}

		private void PerformRageAttack(Mobile target)
        {
            if (target == null || target.Deleted || !target.Alive)
                return;

			int attackChoice = Utility.RandomMinMax( 1, 3 );
            switch (attackChoice)
            {
                case 0:
                     BossSpecialAttack.PerformRampage(
                       boss: this,
                       warcry: "FRESH MEAT!",
                       hue: 0x845,
                       rage: m_Rage+1,
                       stunDuration: 4.0
                   );
                   break;

                case 1:
                   BossSpecialAttack.PerformFear(
				      boss: this,
				      warcry: "FRESH MEAT!",
				      range: 6,
				      rage: m_Rage+1,
				      terror: 90  // Knightship 70+ saves from fear
				  );
				  break;

                case 2:
                    BossSpecialAttack.PerformDegenAura(
		                this,
		                "*spreads its innards around!*",
		                8,          // radius
		                m_Rage+1,     // rage level
		                12,         // duration - 12 + rage*2 seconds, damage happens every 2 seconds 
		                26,         // intensity - 20 + rage damage per tick
		                "health",   // target attribute
		                0x845       // hue
		            );
                    break;
            }
        }

		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			reflect = ( Utility.Random( 100 ) < m_Rage * 8 );
		}

		public override bool OnBeforeDeath()
		{
			if ( m_Rage == 0 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "*RWAAAARRRGHH*" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				SetDamage( 16, 21 );
				m_Rage = 1;
				return false;
			}
			else if ( m_Rage == 1 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "*RWAAAAAAAAARRRGHH*!" );
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
				Effects.SendLocationParticles( EffectItem.Create( this.Location, this.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
				this.PlaySound( 0x1FE );
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "I go...Home...." );
                Mobile killer = this.LastKiller;
				if (killer != null && killer.Player && killer.Karma < 0)
            	{
            	    int marks = Utility.RandomMinMax(40, 75);
            	    Server.Custom.DefenderOfTheRealm.MarkLootHelper.AwardMarks(killer, 1, marks);
            	}
			}
			
			return base.OnBeforeDeath();
		}  

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			BossLootSystem.AwardBossSpecial(this,BossDrops, 15);
			c.DropItem( new EtherealPowerScroll() );
			c.DropItem( AscensionScrollFactory.CreateRandom());
			// gold explosion
		    RichesSystem.SpawnRiches( m_LastTarget, 1 );
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			m_LastTarget = from;
			if (Utility.RandomDouble() < 0.35 )
				Server.Misc.IntelligentAction.LeapToAttacker( this, from );

			base.OnDamage( amount, from, willKill );
		}

		public override void OnAfterSpawn()
		{
			base.OnAfterSpawn();
		}

		public UndeadButcher( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version

			writer.Write( m_Rage );
			writer.Write( m_NextSpecialAttack );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( version >= 1 )
			{
				m_Rage = reader.ReadInt();
				m_NextSpecialAttack = reader.ReadDateTime();
			}
		}
	}
}