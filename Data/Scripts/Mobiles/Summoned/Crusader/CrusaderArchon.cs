using System;
using Server;
using Server.Items;
using System.Collections;
using Server.CustomSpells;
using Server.Custom.DailyBosses.System;

namespace Server.Mobiles
{
	[CorpseName( "a Mummy's corpse" )]
	public class CrusaderArchon : BaseCreature
	{
		public override bool DeleteCorpseOnDeath { get { return true; } }
		public override bool IsDispellable { get { return false; } }
		private DateTime m_NextSpecialAttack = DateTime.MinValue;

		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.MortalStrike;
		}

		[Constructable]
		public CrusaderArchon() : base( AIType.AI_Mage, FightMode.Evil, 10, 1, 0.2, 0.4 )
		{
			Name = "an Archon";
			Body = 346;
			Hue = 0xf8;
			BaseSoundID = 466;

			SetStr( 1285 );
			SetDex( 245 );
			SetInt( 180 );

			SetHits( 811 );

			SetDamage( 22, 29 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Fire, 25 );
			SetDamageType( ResistanceType.Energy, 25 );

			SetResistance( ResistanceType.Physical, 70 );
			SetResistance( ResistanceType.Fire, 60 );
			SetResistance( ResistanceType.Cold, 50 );
			SetResistance( ResistanceType.Poison, 70 );
			SetResistance( ResistanceType.Energy, 40 );

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

		public override Poison PoisonImmune { get { return Poison.Regular; } }

		public override bool CanRummageCorpses{ get{ return false; } }

		public CrusaderArchon( Serial serial ): base( serial )
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
						"Heavens burn thee!",
						0x498,  // hue
						20,     // physical
						20,   // fire
						20,     // cold
						20,     // poison
						20      // energy
					);
					break;
				}
				case 2: // energy nova
				{
					BossSpecialAttack.PerformCrossExplosion(
					    boss: this,
					    target: target,
					    warcry: "Light everlasting!",
					    hue: 0x498,
					    rage: 2,
					    coldDmg: 20,
					    fireDmg: 20,
					    energyDmg: 20,
					    poisonDmg: 20,
					    physicalDmg: 20
					);
                	break;
			    }
				case 3: // energy nova
				{
					BossSpecialAttack.PerformSlam(
                	    boss: this,
                	    warcry: "Face judgement!",
                	    hue: 0x498,
                	    rage: 2,
                	    range: 6,
                	    physicalDmg: 0,
						energyDmg: 100
                	);
                	break;
			    }
			}
		}

		public override void OnAfterSpawn()
		{
			this.MobileMagics(7, SpellType.Cleric, 0x498);
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
			this.MobileMagics(7, SpellType.Cleric, 0x498);
		}
	}
}