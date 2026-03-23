using System;
using Server;
using Server.Items;
using System.Collections;
using Server.CustomSpells;
using Server.Custom.DailyBosses.System;

namespace Server.Mobiles
{
	[CorpseName( "a Frost Wyrm's corpse" )]
	public class BlackguardsFrostWyrm : BaseCreature
	{
		public override bool DeleteCorpseOnDeath { get { return true; } }
		public override bool IsDispellable { get { return false; } }
		private DateTime m_NextSpecialAttack = DateTime.MinValue;

		public override int BreathPhysicalDamage{ get{ return 20; } }
		public override int BreathFireDamage{ get{ return 20; } }
		public override int BreathColdDamage{ get{ return 20; } }
		public override int BreathPoisonDamage{ get{ return 20; } }
		public override int BreathEnergyDamage{ get{ return 20; } }
		public override int BreathEffectHue{ get{ return 0x9C1; } }
		public override int BreathEffectSound{ get{ return 0x653; } }
		public override int BreathEffectItemID{ get{ return 0x37BC; } }
		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool HasBreath{ get{ return true; } }
		public override double BreathEffectDelay{ get{ return 0.1; } }
		public override int GetBreathForm()
		{
		    return 25;
		}


		[Constructable]
		public BlackguardsFrostWyrm() : base( AIType.AI_Mage, FightMode.Evil, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "dragon" );
			Title = "the dracolich";
			Body = 104;
			BaseSoundID = 0x488;

			SetStr( 1285 );
			SetDex( 245 );
			SetInt( 180 );

			SetHits( 811 );

			SetDamage( 22, 29 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Fire, 25 );
			SetDamageType( ResistanceType.Energy, 25 );

			SetResistance( ResistanceType.Physical, 70 );
			SetResistance( ResistanceType.Fire, 50 );
			SetResistance( ResistanceType.Cold, 70 );
			SetResistance( ResistanceType.Poison, 50 );
			SetResistance( ResistanceType.Energy, 50 );

			SetSkill( SkillName.Anatomy, 50.0 );
			SetSkill( SkillName.Psychology, 95.0 );
			SetSkill( SkillName.Magery, 99.0 );
			SetSkill( SkillName.Meditation, 50.0 );
			SetSkill( SkillName.MagicResist, 130.0 );
			SetSkill( SkillName.Tactics, 95.0 );
			SetSkill( SkillName.FistFighting, 110.0 );

			Fame = 24000;
			Karma = 24000;


			VirtualArmor = 60;
			ControlSlots = 2;
		}

		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override bool BleedImmune{ get{ return true; } }
		public override bool CanRummageCorpses{ get{ return false; } }

		public BlackguardsFrostWyrm( Serial serial ): base( serial )
		{
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			if ( DateTime.UtcNow >= m_NextSpecialAttack )
			{
				PerformRageAttack( from );
				m_NextSpecialAttack = DateTime.UtcNow + TimeSpan.FromSeconds( 45 );
			}
			
			base.OnDamage( amount, from, willKill );
		}

		private void PerformRageAttack( Mobile target )
		{
			if ( target == null || target.Deleted || !target.Alive )
				return;

			int attackChoice = Utility.RandomMinMax( 1, 3 );
            Map map = this.Map;

			switch ( attackChoice  )
			{
				case 1: // energy burst
				{
					BossSpecialAttack.PerformTargettedAoE(
						this,
						target,
						1,
						"*bursts with cold*",
						0x47E,  // hue
						0,     // physical
						0,   // fire
						100,     // cold
						0,     // poison
						0      // energy
					);
					break;
				}
				case 2: // cone breath
				{
					BossSpecialAttack.PerformConeBreath(
					    boss: this,
					    target: target,
					    warcry: "*exhales devastating cold!*",
					    hue: 0x47E,
					    rage: 2,
						range:5,
					    physicalDmg: 0,
						coldDmg: 100
					);
                	break;
			    }
				case 3: // cold nova
				{
					BossSpecialAttack.PerformSlam(
                	    boss: this,
                	    warcry: "*screeches*",
                	    hue: 0x47E,
                	    rage: 2,
                	    range: 6,
                	    physicalDmg: 0,
						coldDmg: 100
                	);
                	break;
			    }
			}
		}

		public override void OnAfterSpawn()
		{
			this.MobileMagics(7, SpellType.Wizard, 0x498);
			base.OnAfterSpawn();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
			writer.Write( m_NextSpecialAttack );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			m_NextSpecialAttack = reader.ReadDateTime();
			this.MobileMagics(7, SpellType.Wizard, 0x498);
		}
	}
}