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
	[CorpseName( "Waervaerendor's Corpse" )]
	public class Waervaerendor : BaseSpellCaster
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
		
		[Constructable]
		public Waervaerendor () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Waervaerendor";
            NameHue = 0x22;
			Title = "The Rapacious Raider";
			EmoteHue = 11;
			Body = 123;
			BaseSoundID = 362;
			SetStr( 596, 785 );
			SetDex( 165, 225 );
			SetInt( 556, 655 );
			SetHits( 10000 );
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
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 6 );
		}

		public override int GetAngerSound()
        {
            return 0x63E;
        }

        public override int GetDeathSound()
        {
            return 0x63F;
        }

        public override int GetHurtSound()
        {
            return 0x640;
        }

        public override int GetIdleSound()
        {
            return 0x641;
        }

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
			if (Utility.RandomDouble() < 0.35 )
				Server.Misc.IntelligentAction.LeapToAttacker( this, from );
			base.OnDamage( amount, from, willKill );
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
						physicalDmg:0,
					    poisonDmg: 100
					);
					break;
				case 2:
					BossSpecialAttack.PerformResistBreach(
						boss: this,
						target: target,
						warcry: "*exhales noxious fumes*",
						hue: 1285,
						rage: m_Rage,
						radius: 6,
						resist:"poison"
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
			if ( m_Rage == 0 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "We will consume you!" );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Brother, let us end this!" );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "We are vengeance!" );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Brother...!" );
				Mobile killer = this.LastKiller;
				if ( killer != null && killer.Player && killer.Karma > 0 )
            	{
            	    int marks = Utility.RandomMinMax( 156, 223 );
            	    Server.Custom.DefenderOfTheRealm.MarkLootHelper.AwardMarks( killer, 1, marks );
            	}
			}
			return base.OnBeforeDeath();
		}

		public override void OnDeath( Container c )
		{

			BossLootSystem.AwardBossSpecial( this, BossDrops, 15 );
			for ( int i = 0; i < 4; i++ )
			{
 	           	c.DropItem( Loot.RandomArty() );				
				c.DropItem( new EtherealPowerScroll() );
				c.DropItem( AscensionScrollFactory.CreateRandom());
			}
			int amount = Utility.Random(3,6);
			RichesSystem.SpawnRiches( m_LastTarget, 4 );
			base.OnDeath( c );
		}

		public override void OnAfterSpawn()
		{
			base.OnAfterSpawn();
			LeechImmune = true;
		}

		public Waervaerendor( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 2 );
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
			LeechImmune = true;
		}
	}
}