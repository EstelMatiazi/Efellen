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
	[CorpseName( "TeelFanae's Corpse" )]
	public class TeelFanae : BaseSpellCaster
	{		
		private static readonly Type[] SummonTypes = new Type[] 
		{  
			typeof(Succubus), 
			typeof(LolthsChosen),
			typeof(DemonwebSpinner)
		};

		private static readonly string[] SummonWarcries = new string[]
		{
			"Lolth guides my hand!",
			"The hosts of the demomweb come for thee!",
			"Long shall be thy suffering!",
			"Lolth, answer thy daughter!"
		};

		private static readonly List<Type> BossDrops = new List<Type>
    	{
            typeof(Artifact_BookOfVileDarkness),
            typeof(Artifact_LolthsAbsoluteWill),
			typeof(Artifact_WebOfDominion)
    	};

		private int m_Rage;
		private Mobile m_LastTarget;
		private DateTime m_NextSummonTime;
		private DateTime m_NextSpecialAttack;
		private List<BaseCreature> m_Summons;

		[Constructable]
		public TeelFanae () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Te'el Fanae";
            Body = 606; 
			Utility.AssignRandomHair( this );
			NameHue = 0x22;
			Title = "The Voice of Lolth";
            HairHue = 1150;
			Hue = 1316;
			EmoteHue = 11;
			
			SetStr( 896, 985 );
			SetDex( 125, 175 );
			SetInt( 586, 675 );

			SetHits( 19000 );
			SetDamage( 11, 16 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Poison, 50 );
			SetResistance( ResistanceType.Physical, 55 );
			SetResistance( ResistanceType.Fire, 70 );
			SetResistance( ResistanceType.Cold, 70 );
			SetResistance( ResistanceType.Poison, 70 );
			SetResistance( ResistanceType.Energy, 70 );

			SetSkill( SkillName.Meditation, 112.5, 125.0 );
			SetSkill( SkillName.MagicResist, 155.0 );
			SetSkill( SkillName.Tactics, 110.0 );
			SetSkill( SkillName.FistFighting, 115.0 );
			SetSkill( SkillName.Bludgeoning, 115.0 );
			SetSkill( SkillName.Magery, 120.0 );
			SetSkill( SkillName.Psychology, 120.0 );

			Fame = 35000;
			Karma = -35000;
			VirtualArmor = 60;
            int drowhue = Utility.RandomDrowHue();

			if ( Backpack == null )
				AddItem( new Backpack() );

            AddItem( new SpiderRobe{ Hue = drowhue } );
            AddItem( new Sandals{ Hue = drowhue } );

			Item weapon = null;
		    Item shield = null;
			weapon = new Scepter();
			shield = new DarkShield();
		    if (weapon != null)
		    {
		        weapon.Hue = drowhue;
				MakeSpellChanneling(weapon);
			    AddItem(weapon);
		    }
		
		    if (shield != null)
		    {
		        shield.Hue = drowhue;
				MakeSpellChanneling(shield);
			    AddItem(shield);
		    }

            m_NextSummonTime = DateTime.MinValue;
			m_NextSpecialAttack = DateTime.MinValue;
			m_Summons = new List<BaseCreature>();
          }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 8 );
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			m_LastTarget = from;
			if (Utility.RandomDouble() < 0.75 )
				Server.Misc.IntelligentAction.LeapToAttacker( this, from );

			base.OnDamage( amount, from, willKill );
		}

		private void MakeSpellChanneling(Item item)
		{
		    if (item == null)
		        return;

		    if (item is BaseWeapon)
		    {
		        ((BaseWeapon)item).Attributes.SpellChanneling = 1;
		    }
		    else if (item is BaseShield)
		    {
		        ((BaseShield)item).Attributes.SpellChanneling = 1;
		    }
		}

        public override bool AlwaysAttackable{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 5; } }
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
		        30
		    );

		    if (DateTime.UtcNow >= m_NextSpecialAttack)
		    {
		        PerformRageAttack(combatant);
		        m_NextSpecialAttack = DateTime.UtcNow + TimeSpan.FromSeconds(25 - (m_Rage * 2));
		    }

		    m_LastTarget = combatant;
		}

		private void PerformRageAttack( Mobile target )
		{
			if ( target == null || target.Deleted || !target.Alive )
				return;

			int attackChoice = Utility.RandomMinMax( 1, 3 );

			switch ( attackChoice )
			{
				case 1:
					BossSpecialAttack.PerformPull(
                        this,
                        "Spider Queen, I offer thee a sacrifice!",
                        1316,
                        m_Rage+1,
                        true
                    );
            	break;
				case 2:
					BossSpecialAttack.SummonHonorGuard(
                        boss: this,
                        target: target,
                        warcry: "Come hither, Lolth's favored!",
                        amount: 4+m_Rage,
                        creatureType: typeof(DemonwebSpinner),
                        hue: 0x845
                    );
					break;
				case 3:
					BossSpecialAttack.PerformDemonWebRitual(
                       boss: this,
                       hue: 1316
                    );
					break;
			}
		}
		
		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			reflect = ( Utility.Random( 100 ) < m_Rage * 20 );
		}

		private int GetMaxSummons()
		{
			switch ( m_Rage )
			{
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Delightful guest, have a taste of Lolth's hospitality!" );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "This is my city, my house! In here my will is supreme!" );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Lolth, prepare for the feast!" );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Lolth will have you answer for this insolence!" );
				Mobile killer = this.LastKiller;
				if ( killer != null && killer.Player && killer.Karma > 0 )
            	{
            	    int marks = Utility.RandomMinMax( 231, 347 );
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

		    BossLootSystem.BossEnchant(this, c, 650, 100, 4, "DrowPriestess");
			BossLootSystem.AwardBossSpecial( this, BossDrops, 15 );
			for ( int i = 0; i < 5; i++ )
			{
 	           	c.DropItem( Loot.RandomArty() );				
				c.DropItem( new EtherealPowerScroll() );
				c.DropItem( AscensionScrollFactory.CreateRandom());
			}
			if ( Utility.RandomDouble() < 0.25 )
			{
				c.DropItem( new EternalPowerScroll() );
			}
			int amount = Utility.Random(6,12);
			c.DropItem(new EssenceOfLolthsHatred(amount));
			RichesSystem.SpawnRiches( m_LastTarget, 5 );

			base.OnDeath( c );
		}

		public override void OnAfterSpawn()
		{
			base.OnAfterSpawn();
			this.MobileMagics(8, SpellType.Cleric, 1316);
			LeechImmune = true;
		}

		public TeelFanae( Serial serial ) : base( serial )
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
				this.MobileMagics(8, SpellType.Cleric, 1316);
			}
			LeechImmune = true;

			if ( m_Summons == null )
				m_Summons = new List<BaseCreature>();
		}
	}
}