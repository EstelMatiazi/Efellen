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
	[CorpseName( "Caelan's Corpse" )]
	public class Caelan : BaseCreature
	{
		private static readonly List<Type> BossDrops = new List<Type>
    	{
    	    typeof(Artifact_CaelansBloodyBlade),
    	    typeof(Artifact_CaelansShroud),
    	    typeof(Artifact_CaelansVisage)
    	};

		private int m_Rage = 0;
		private Mobile m_LastTarget;
		private DateTime m_NextSpecialAttack = DateTime.MinValue;

		[Constructable]
		public Caelan () : base( AIType.AI_Melee, FightMode.Aggressor, 20, 1, 0.4, 0.8 )
		{
			Name = "Caelan";
            Title = "The Dread Knight";
			Body = 0x147;
			NameHue = 0x22;
			Body = 400; 			
			FacialHairItemID = Utility.RandomList( 0, 0, 8254, 8255, 8256, 8257, 8267, 8268, 8269 );
			Utility.AssignRandomHair( this );
			HairHue = Utility.RandomHairHue();
			FacialHairHue = HairHue;

			SetStr( 796, 885 );
			SetDex( 205, 235 );
			SetInt( 286, 325 );

			SetHits( 7250 );
			SetDamage( 11, 15 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Cold, 50 );
			SetResistance( ResistanceType.Physical, 65 );
			SetResistance( ResistanceType.Fire, 55 );
			SetResistance( ResistanceType.Cold, 65 );
			SetResistance( ResistanceType.Poison, 65 );
			SetResistance( ResistanceType.Energy, 65 );

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
		public override bool ClickTitle{ get{ return false; } }
		public override bool ShowFameTitle{ get{ return false; } }
		public override bool AlwaysAttackable{ get{ return true; } }
		public override bool BleedImmune{ get{ return true; } }
		public override bool BardImmune { get { return true; } }
		public override bool Unprovokable { get { return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }

		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			reflect = ( Utility.Random( 100 ) < m_Rage * 15 );
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
                case 1: // Blood blast
                {
                    BossSpecialAttack.PerformSlam(
                       boss: this,
                       warcry: "Your guts shall decorate these halls!",
                       hue: 0x0AA5,
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
                       warcry: "*The Dreadful knight charges!*",
                       hue: 0x0AA5,
                       rage: m_Rage+1,
                       stunDuration: 3.0
                   );
                   break;
                }
                case 3: // Honor guard
                {
                    BossSpecialAttack.SummonHonorGuard(
                        boss: this,
                        target: target,
                        warcry: "Come, my brothers!",
                        amount: 3,
                        creatureType: typeof(BloodstoneKeepKnight),
                        hue: 0x09d3
                    );
                    break;
                }
            }
        }

		private void AddEquipment()
		{
		    AddItem(new RoyalSword { Hue = 0x0AA5 });
		    AddItem(new RoyalArms { Hue = 0x0AA5 });
		    AddItem(new RoyalChest { Hue = 0x0AA5 });
		    AddItem(new RoyalsLegs { Hue = 0x0AA5 });
		    AddItem(new RoyalGorget { Hue = 0x0AA5 });
		    AddItem(new RoyalGloves { Hue = 0x0AA5 });
		    AddItem(new Boots { Hue = 0x0AA5 });
		    AddItem(new DreadHelm { Hue = 0x0AA5 });
		    AddItem(new RoyalShield { Hue = 0x0AA5 });		
		    AddItem(new Cloak { Hue = 0x0AA5 });
		}		

		private void ColorKnight()
		{
		    MorphingTime.ColorMyClothes(this, 0x0AA5, 0);
		    MorphingTime.ColorMyArms(this, 0, 0);
		    Server.Misc.MorphingTime.CheckMorph(this);
		}	
		public override bool OnBeforeDeath()
		{
			if ( m_Rage == 0 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Tis' folly to challenge me!" );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "I shall make an example out of you!" );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "You shall regret crossing me!" );
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
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "I am...Finally...bested" );
				Mobile killer = this.LastKiller;
				if (killer != null && killer.Player && killer.Karma > 0)
            	{
            	    int marks = Utility.RandomMinMax(103, 110);
            	    Server.Custom.DefenderOfTheRealm.MarkLootHelper.AwardMarks(killer, 1, marks);
            	}
			}
			return base.OnBeforeDeath();
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
			BossLootSystem.BossEnchant(this, c, 400, 100, 2, "deathknight");
			// gold explosion
		    RichesSystem.SpawnRiches( m_LastTarget, 3 );
		}

		public override void OnAfterSpawn()
		{
			base.OnAfterSpawn();
			LeechImmune = true;
			AddEquipment();
		    ColorKnight();
		}

		public Caelan( Serial serial ) : base( serial )
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
			LeechImmune = true;
		}
	}
}