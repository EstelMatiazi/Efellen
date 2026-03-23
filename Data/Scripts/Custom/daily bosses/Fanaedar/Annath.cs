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
	[CorpseName( "Annath's Corpse" )]
	public class Annath : BaseSpellCaster
	{		
		private static readonly Type[] SummonTypes = new Type[] 
		{ 
			typeof(LolthsBrood), 
			typeof(LolthsChampion), 
			typeof(LolthsChosen)
		};

		private static readonly string[] SummonWarcries = new string[]
		{
			"Lolth, I call thee!",
			"Webspinner, bring forth thy ruin!",
			"Fanaedar shall be thy grave eternal!",
			"To the Demonweb with thee!"
		};

		private static readonly List<Type> BossDrops = new List<Type>
    	{
            typeof(Artifact_LolthsCaressingChoker),
            typeof(Artifact_LolthsEnduringApathy),
			typeof(Artifact_LolthsObstination),
			typeof(Artifact_LolthsDomination),
			typeof(Artifact_LolthsHunger)
    	};

		private int m_Rage;
		private Mobile m_LastTarget;
		private DateTime m_NextSummonTime;
		private DateTime m_NextSpecialAttack;
		private List<BaseCreature> m_Summons;

		[Constructable]
		public Annath () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Annath";
            Body = 606; 
			Utility.AssignRandomHair( this );
			NameHue = 0x22;
			Title = "The Shroud of the Lightless";
            HairHue = 1150;
			Hue = 1316;
			EmoteHue = 11;
			
			SetStr( 596, 785 );
			SetDex( 165, 225 );
			SetInt( 556, 655 );
			SetHits( 10000 );
			SetDamage( 11, 15 );
			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Poison, 50 );
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
            int drowhue = Utility.RandomDrowHue();

            AddItem( new SpiderRobe{ Hue = drowhue } );
            AddItem( new Sandals{ Hue = drowhue } );

			Item weapon = null;
		    Item shield = null;
			weapon = new Whips();
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
			AddLoot( LootPack.UltraRich, 6 );
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
			if (Utility.RandomDouble() < 0.50 )
				Server.Misc.IntelligentAction.LeapToAttacker( this, from );

			base.OnDamage( amount, from, willKill );
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

		private void PerformRageAttack( Mobile target )
		{
			if ( target == null || target.Deleted || !target.Alive )
				return;

			int attackChoice = Utility.RandomMinMax( 1, 3 );

			switch ( attackChoice )
			{
				case 1:
					RallyMinions();
					break;
				case 2:
					BossSpecialAttack.PerformDegenAura( this, "By my goddess and by church, thou shall perish!", 8, m_Rage+1, 16, 29, "health", 1316 );
					break;
				case 3:
					BossSpecialAttack.PerformSlam(
                       boss: this,
                       warcry: "Lolth smites thee!",
                       hue: 1316,
                       rage: m_Rage+1,
                       range: 6,
                       physicalDmg: 100
                   );
					break;
			}
		}
		public void RallyMinions()
		{
		    if (Combatant == null)
		        return;

			PublicOverheadMessage( MessageType.Regular, 0x21, false, "Lolth, nurture thy children!" );

		    PlaySound(0x1FB);
		    PlaySound(0x5C3);

		    FixedEffect(0x37C4, 10, 30, 0x496, 0);
		    Effects.SendLocationEffect(Location, Map, 0x3709, 20, 10, 0x496, 0);

		    for(int i = 0; i < 8; i++)
		    {
		        int xOffset = 0;
		        int yOffset = 0;

		        switch(i)
		        {
		            case 0: xOffset = 6; yOffset = 0; break;
		            case 1: xOffset = -6; yOffset = 0; break;
		            case 2: xOffset = 0; yOffset = 6; break;
		            case 3: xOffset = 0; yOffset = -6; break;
		            case 4: xOffset = 4; yOffset = 4; break;
		            case 5: xOffset = -4; yOffset = 4; break;
		            case 6: xOffset = 4; yOffset = -4; break;
		            case 7: xOffset = -4; yOffset = -4; break;
		        }

		        Point3D waveLoc = new Point3D(X + xOffset, Y + yOffset, Z);
		        Effects.SendLocationEffect(waveLoc, Map, 0x3728, 15, 10, 0x496, 0);
		    }

		    IPooledEnumerable eable = Map.GetMobilesInRange(Location, 8);

		    foreach (Mobile m in eable)
		    {
		        if (m == null || m == this || m.Deleted)
		            continue;

		        bool isMinion = false;
		        Type mType = m.GetType();

		        for(int i = 0; i < SummonTypes.Length; i++)
		        {
		            if (mType == SummonTypes[i])
		            {
		                isMinion = true;
		                break;
		            }
		        }

		        if (!isMinion)
		            continue;

		        Effects.SendMovingEffect(this, m, 0x374A, 10, 0, false, false, 0x496, 0);

		        Timer.DelayCall(TimeSpan.FromSeconds(0.3), new TimerStateCallback(HealMinion), m);
		    }

		    eable.Free();

		    int healAmount = m_Rage * 150;
		    Hits = Math.Min(HitsMax, Hits + healAmount);

		    FixedEffect(0x376A, 10, 16, 0x496, 0);
		    PlaySound(0x202);
		}

		private void HealMinion(object state)
		{
		    if (Deleted)
		        return;

		    Mobile m = state as Mobile;

		    if (m == null || m.Deleted)
		        return;

		    m.Hits = m.HitsMax;

		    m.FixedEffect(0x376A, 10, 16, 0x496, 0);
		    m.FixedEffect(0x3779, 10, 20, 0x496, 0);
		    m.PlaySound(0x202);

		    if (Combatant != null && !Combatant.Deleted)
		        m.Combatant = Combatant;

		    for(int i = 0; i < 4; i++)
		    {
		        int xOffset = Utility.RandomMinMax(-1, 1);
		        int yOffset = Utility.RandomMinMax(-1, 1);
		        Point3D energyLoc = new Point3D(m.X + xOffset, m.Y + yOffset, m.Z);
		        Effects.SendLocationEffect(energyLoc, m.Map, 0x3709, 10, 10, 0x496, 0);
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
				case 1: return 12;
				case 2: return 10;
				case 3: return 8;
				default: return 8;
			}
		}

		public override bool OnBeforeDeath()
		{
			if ( m_Rage == 0 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Lolth, bring me hatred!" );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Lolth, bring me ruin!" );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Lolth, bring me vengeance!" );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Lolth will be the end of you!" );
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
		    BossLootSystem.BossEnchant(this, c, 550, 100, 3, "DrowPriestess");

			BossLootSystem.AwardBossSpecial( this, BossDrops, 15 );
			for ( int i = 0; i < 4; i++ )
			{
 	           	c.DropItem( Loot.RandomArty() );				
				c.DropItem( new EtherealPowerScroll() );
				c.DropItem( AscensionScrollFactory.CreateRandom());
			}
			int amount = Utility.Random(3,6);
			c.DropItem(new EssenceOfLolthsHatred(amount));
			RichesSystem.SpawnRiches( m_LastTarget, 4 );

			base.OnDeath( c );
		}

		public override void OnAfterSpawn()
		{
			base.OnAfterSpawn();
			this.MobileMagics(7, SpellType.Cleric, 1316);
			LeechImmune = true;
		}

		public Annath( Serial serial ) : base( serial )
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
				this.MobileMagics(7, SpellType.Cleric, 1316);
			}
			LeechImmune = true;

			if ( m_Summons == null )
				m_Summons = new List<BaseCreature>();
		}
	}
}