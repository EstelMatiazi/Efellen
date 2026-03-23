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
using Server.Regions;
using Server.CustomSpells;
using Server.Custom.Ascensions;
namespace Server.Mobiles
{
	[CorpseName( "Mother Superior's Corpse" )]
	public class MotherSuperior : BaseSpellCaster
	{
		private int m_Rage = 0;
		private Mobile m_LastTarget;
		private DateTime m_NextSpecialAttack = DateTime.MinValue;

		private static readonly List<Type> BossDrops = new List<Type>
    	{
    	    typeof(Artifact_GauntletsOfDevotion),
    	    typeof(Artifact_LeggingsOfDevotion),
    	    typeof(Artifact_TunicOfDevotion),
    	    typeof(Artifact_ArmsOfDevotion),
			typeof(Artifact_CoifOfDevotion),
    	};
        
		[Constructable]
		public MotherSuperior () : base( AIType.AI_Mage, FightMode.Evil, 20, 1, 0.4, 0.8 )
		{
			Title = " the Mother Superior";
			NameHue = 0x92E;
            Body = 401; 
			Name = NameList.RandomName( "female" );
			Utility.AssignRandomHair( this );
			HairHue = Utility.RandomHairHue();
            Hue = Utility.RandomSkinHue(); 

			SetStr( 296, 385 );
			SetDex( 95, 125 );
			SetInt( 186, 225 );

			SetHits( 1455 );
			SetDamage( 14, 24 );

			SetDamageType( ResistanceType.Physical, 100 );
			SetResistance( ResistanceType.Physical, 40 );
			SetResistance( ResistanceType.Fire, 45 );
			SetResistance( ResistanceType.Cold, 45 );
			SetResistance( ResistanceType.Poison, 70 );
			SetResistance( ResistanceType.Energy, 55 );

            SetSkill( SkillName.Magery, 82.5, 125.0 );
            SetSkill( SkillName.Psychology, 52.5, 85.0 );
			SetSkill( SkillName.Meditation, 82.5, 95.0 );
			SetSkill( SkillName.MagicResist, 75.5, 125.0 );
			SetSkill( SkillName.Tactics, 81.0, 95.0 );
			SetSkill( SkillName.FistFighting, 101.0, 115.0 );
			SetSkill( SkillName.Bludgeoning, 101.0, 115.0 );

			Fame = 13000;
			Karma = 15000;

			VirtualArmor = 20;
            AddItem( new NunRobe( ) );
			AddItem( new LightCitizen( true ) );
			AddItem(new WarMace { Hue = 0x9C2 });
		    AddItem(new ChainChest { Hue = 0x9C2 });
		    AddItem(new ChainSkirt { Hue = 0x9C2 });
		    AddItem(new ChainCoif { Hue = 0x9C2 });
		    AddItem(new Cloak { Hue = 0x9C2 });
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 2 );
		}

		private static readonly string[] AttackLines = new string[]
		{
		    "We have no wealth for you to take!",
		    "Thou shall not harm my sisters, {0}!",
		    "{0} is here, escape if you can!",
            "Why do you bring violence to our sanctuary?",
            "Sisters, pray for strength!",
            "{0}, the heavens weep for you!",
            "Protect the patients!",
            "Your heart is clouded, {0}!",
            "Repent before it is too late!",
            "I shall pray for your soul!",
            "Turn back, {0}! Turn back from this darkness!",
            "You defile sacred ground!",
            "This is a house of healing! Cease at once!",
            "We shall outlast you!"
        };

		public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
			if(Utility.RandomDouble() < 0.25)
            {
				int i = Utility.Random(AttackLines.Length);
			    Say(string.Format(AttackLines[i], defender.Name));                
            }
        }


		public override int TreasureMapLevel{ get{ return 2; } }
		public override bool CanRummageCorpses{ get{ return false; } }
		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool BleedImmune{ get{ return true; } }
		public override bool BardImmune { get { return true; } }
		public override bool Unprovokable { get { return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override bool AlwaysAttackable{ get{ return true; } }
		public override bool AlwaysMurderer { get { return false; } }

			private bool IsFriendlyCreature(Mobile m)
		{
			Region reg = Region.Find( this.Location, this.Map );
			return (reg.IsPartOf( "House of Holy Mercy" ) && (
					m is nun || 
					m is Cook || 
					m is Herbalist || 
					m is Healer || 
					m is Painter ||
					m is MotherSuperior
					));
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
			if (Utility.RandomDouble() < 0.20 )
				Server.Misc.IntelligentAction.LeapToAttacker( this, from );

			if (from.Player && from.Kills < 5 && !from.Criminal) 
				from.Criminal = true;	
			
			base.OnDamage( amount, from, willKill );
		}

		private void PerformRageAttack( Mobile target )
		{
			if ( target == null || target.Deleted || !target.Alive )
				return;

			
			int attackChoice = Utility.RandomMinMax( 1, 2 );

			switch ( attackChoice  )
			{
				case 1: // holy smite
				{
					BossSpecialAttack.PerformSmite(
						this,
						target,
						m_Rage+1,
						"*I shall smite you down!*",
						0x9C2,  // hue
						0,     // physical
						50,   // fire
						0,     // cold
						0,     // poison
						50      // energy
					);
					break;
				}
				case 2:  // cleansing burst = a nova of fire damage
				{
					BossSpecialAttack.PerformTargettedAoE(
						this,
						target,
						m_Rage+1,
						"*Heavens protect us!*",
						0x9C2,  // hue
						0,     // physical
						50,   // fire
						0,     // cold
						0,     // poison
						50      // energy
					);
					break;
				}
			}
		}

		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			int chance = m_Rage * 8;
			reflect = ( Utility.Random(100) < chance );
		}

		public override bool OnBeforeDeath()
		{
			if ( m_Rage == 0 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Please stop this madness!" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				
				SetStr( Str + 50 );
				SetDex( Dex + 25 );
				SetDamage( 13, 19 );
				m_Rage = 1;
				return false;
			}
			else if ( m_Rage == 1 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "You forced my hand!" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				
				SetStr( Str + 80 );
				SetDex( Dex + 35 );
				SetDamage( 21, 29 );
				VirtualArmor += 5;
				m_Rage = 2;
				return false;
			}
			else if ( m_Rage == 2 )
			{
				Effects.SendLocationParticles( EffectItem.Create( this.Location, this.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
				this.PlaySound( 0x1FE );
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "The gods...will forgive you." );
                Mobile killer = this.LastKiller;
				if (killer != null && killer.Player && killer.Karma < 0)
            	{
            	    int marks = Utility.RandomMinMax(40, 75);
            	    Server.Custom.DefenderOfTheRealm.MarkLootHelper.AwardMarks(killer, 0, marks);
            	}
			}
			
			return base.OnBeforeDeath();
		}

        public override void OnDelete()
        {
            base.OnDelete();
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

		public override void OnAfterSpawn()
		{
			this.MobileMagics(5, SpellType.Cleric, 0);
			base.OnAfterSpawn();
		}

		public MotherSuperior( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 2 ); // version

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
			if(version >=2)
			{
				this.MobileMagics(5, SpellType.Cleric, 0);
			}
		}
	}
}