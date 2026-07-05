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
	[CorpseName( "Voaraghamanthar's Corpse" )]
	public class Voaraghamanthar : BaseSpellCaster
	{		
		private static readonly List<Type> BossDrops = new List<Type>
    	{
            typeof(Artifact_BeadsOfRuin),
            typeof(NoxiousScalesOfRuin),
			typeof(NoxiousHelmOfRuin),
			typeof(NoxiousarmsOfRuin),
			typeof(NoxiousChestOfRuin),
			typeof(NoxiousLegsOfRuin),
			typeof(NoxiousGlovesOfRuin)
    	};

		private int m_Rage;
		private Mobile m_LastTarget;
		private DateTime m_NextSpecialAttack;
		private DateTime m_NextLeap = DateTime.MinValue;

		private bool m_Rage1Applied = false;
		private bool m_Rage2Applied = false;
		private bool m_Rage3Applied = false;

		[Constructable]
		public Voaraghamanthar () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Voaraghamanthar";
            NameHue = 0x22;
			Title = "The Black Death";
			EmoteHue = 11;
			Body = 123;
			BaseSoundID = 362;
			SetStr( 596, 785 );
			SetDex( 165, 225 );
			SetInt( 556, 655 );
			SetHits( 30000 );
			SetDamage( 11, 15 );
			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Poison, 50 );
			SetResistance( ResistanceType.Physical, 50 );
			SetResistance( ResistanceType.Fire, 60 );
			SetResistance( ResistanceType.Cold, 50 );
			SetResistance( ResistanceType.Poison, 80 );
			SetResistance( ResistanceType.Energy, 70 );
			SetSkill( SkillName.Meditation, 102.5, 125.0 );
			SetSkill( SkillName.MagicResist, 125.5, 145.0 );
			SetSkill( SkillName.Tactics, 101.0, 110.0 );
			SetSkill( SkillName.FistFighting, 111.0 );
			SetSkill( SkillName.Magery, 101.0, 110.0 );
			SetSkill( SkillName.Psychology, 101.0, 110.0 );
			Fame = 30000;
			Karma = -30000;
			VirtualArmor = 50;
            m_NextSpecialAttack = DateTime.MinValue;
			IsBoss = true;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 6 );
		}

		public override int GetAngerSound(){ return 0x63E; }
		public override int GetDeathSound(){ return 0x63F; }
		public override int GetHurtSound(){ return 0x640; }
		public override int GetIdleSound(){ return 0x641; }

		public override bool AlwaysAttackable{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 4; } }
		public override bool CanRummageCorpses{ get{ return false; } }
		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool BleedImmune{ get{ return true; } }
		public override bool BardImmune { get { return true; } }
		public override bool Unprovokable { get { return true; } }

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			m_LastTarget = from;

			Server.Misc.IntelligentAction.TryToLeapToAttacker( this, from, 0.07, ref m_NextLeap );

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
			PublicOverheadMessage( MessageType.Regular, 0x21, false, "You are nothing to me, nothing!" );
			this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
			this.PlaySound( 0x202 );
			SetDamage( 16, 20 );
			VirtualArmor += 5;
		}

		private void ApplyRage2()
		{
			PublicOverheadMessage( MessageType.Regular, 0x21, false, "My brother and I shall enjoy your bloodletting!" );
			this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
			this.PlaySound( 0x202 );
			SetDamage( 21, 25 );
			VirtualArmor += 10;
		}

		private void ApplyRage3()
		{
			PublicOverheadMessage( MessageType.Regular, 0x21, false, "You...We will end you!" );
			this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
			this.PlaySound( 0x202 );
			SetDamage( 26, 30 );
			VirtualArmor += 15;
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
		        m_NextSpecialAttack = DateTime.UtcNow + TimeSpan.FromSeconds(25 - (m_Rage * 2));
		    }
		    m_LastTarget = combatant;
		}

		private void PerformRageAttack( Mobile target )
		{
			if ( target == null || target.Deleted || !target.Alive )
				return;

			int attackChoice = Utility.RandomMinMax( 1, 2 );

			switch ( attackChoice )
			{
				case 1:
					BossSpecialAttack.PerformConeBreath(
				    boss: this,
				    target: target,
				    warcry: "*exhales noxious acid!*",
				    hue: 1285,
				    rage: m_Rage*3,
				    range: 8,
					physicalDmg: 0,
				    poisonDmg: 100
				);
					break;
				case 2:
					BossSpecialAttack.PerformDegenAura( 
						this, 
						"You shall rot in our hoard!", 
						8, 
						m_Rage+1, 
						16, 
						29, 
						"health", 
						1285
					);
					break;
			}
		}
		
		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			reflect = ( Utility.Random( 100 ) < m_Rage * 16 );
		}

		public override bool OnBeforeDeath()
		{
			BossLootSystem.AwardBossMarks(this, this.LastKiller, 156, 223, "Brother...!");
			return base.OnBeforeDeath();
		}

		public override void OnDeath( Container c )
		{
			BossLootSystem.AwardBossSpecial(this, BossDrops, 30);
			for ( int i = 0; i < 4; i++ )
			{
 	           	c.DropItem( Loot.RandomArty() );				
				c.DropItem( new EtherealPowerScroll() );
				c.DropItem( AscensionScrollFactory.CreateRandom());
			}
			RichesSystem.SpawnRiches( m_LastTarget, 4 );
			base.OnDeath( c );
		}

		public override void OnAfterSpawn()
		{
			base.OnAfterSpawn();
			LeechImmune = true;
		}

		public Voaraghamanthar( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 3 );
			writer.Write( m_Rage );
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
		}
	}
}