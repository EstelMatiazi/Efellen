using System;
using Server;
using Server.Items;
using System.Collections;
using Server.Custom.DailyBosses.System;

namespace Server.Mobiles
{
	[CorpseName( "a Mummy's corpse" )]
	public class PaleMasterSkeletalDragon : BaseCreature
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

		public override double GetFightModeRanking( Mobile m, FightMode acqType, bool bPlayerOnly )
		{
			return ( m.Int + m.Skills[SkillName.Necromancy].Value ) / Math.Max( GetDistanceToSqrt( m ), 1.0 );
		}

		[Constructable]
		public PaleMasterSkeletalDragon() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an Skeletal Dragon";
			Body = 323;
			BaseSoundID = 0x488;
			Hue = 0xB97;

			SetStr( 898, 1030 );
			SetDex( 68, 200 );
			SetInt( 488, 620 );

			SetHits( 558, 599 );

			SetDamage( 29, 35 );

			SetDamageType( ResistanceType.Physical, 75 );
			SetDamageType( ResistanceType.Fire, 25 );

			SetResistance( ResistanceType.Physical, 70 );
			SetResistance( ResistanceType.Fire, 60 );
			SetResistance( ResistanceType.Cold, 60 );
			SetResistance( ResistanceType.Poison, 70 );
			SetResistance( ResistanceType.Energy, 60 );

			SetSkill( SkillName.Psychology, 100.0 );
			SetSkill( SkillName.Magery, 80.1, 100.0 );
			SetSkill( SkillName.MagicResist, 101.3, 130.0 );
			SetSkill( SkillName.Tactics, 97.6, 101.0 );
			SetSkill( SkillName.FistFighting, 97.6, 101.0 );

			Fame = 0;
			Karma = 0;

			VirtualArmor = 40;
			ControlSlots = 0;
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			if ( DateTime.UtcNow >= m_NextSpecialAttack )
			{
				PerformRageAttack( from );
				m_NextSpecialAttack = DateTime.UtcNow + TimeSpan.FromSeconds( 30 );
			}
			
			base.OnDamage( amount, from, willKill );
		}
		
		private void PerformRageAttack( Mobile target )
		{
			if ( target == null || target.Deleted || !target.Alive )
				return;

			int attackChoice = Utility.RandomMinMax( 1, 2 );
            Map map = this.Map;

			switch ( attackChoice  )
			{
				case 1:
                {
                    BossSpecialAttack.PerformConeBreath(
					    boss: this,
					    target: target,
					    warcry: "*exhales devastating fumes!*",
					    hue: 267,
					    rage: 1,
					    range: 6, 
						physicalDmg:0,
						coldDmg:0,
						poisonDmg:100,
						energyDmg:0,
					    fireDmg: 0
					);
					break;
                }
				case 2:
				{
					BossSpecialAttack.PerformDegenAura(
		                this,
		                "*Channels the powers of undeath!*",
		                6,          // radius
		                2,     		// rage level
		                12,         // duration - 12 + rage*2 seconds, damage happens every 2 seconds 
		                12,         // intensity - 20 + rage damage per tick
		                "health",   // target attribute
		                267         // hue
		            );
                    break;
				}
			}
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune { get { return Poison.Greater; } }

		public override Poison HitPoison{ get{ return Poison.Greater; } }


		public PaleMasterSkeletalDragon( Serial serial ): base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}