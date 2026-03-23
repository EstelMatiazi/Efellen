using System;
using Server;
using Server.Items;
using Server.Misc;
using Server.Custom.DailyBosses.System;

namespace Server.Mobiles
{
	[CorpseName( "a Herald of discordance's corpse" )]
	public class HeraldOfDiscordance : BaseCreature
	{
		private DateTime m_NextSpecialAttack = DateTime.MinValue;
		public override bool IsDispellable { get { return false; } }

		[Constructable]
		public HeraldOfDiscordance () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a Herald of Discordance";
			Body = 427;
			Hue = 0xB01;
			BaseSoundID = 357;

			SetStr( 1186, 1385 );
			SetDex( 277, 355 );
			SetInt( 251, 350 );

			SetHits( 992, 1711 );

			SetDamage( 26, 33 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Fire, 25 );
			SetDamageType( ResistanceType.Energy, 25 );

			SetResistance( ResistanceType.Physical, 65, 80 );
			SetResistance( ResistanceType.Fire, 60, 80 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.Anatomy, 69.0 );
			SetSkill( SkillName.Psychology, 110.0 );
			SetSkill( SkillName.Magery, 110.0 );
			SetSkill( SkillName.Meditation, 5.0 );
			SetSkill( SkillName.MagicResist, 125.0 );
			SetSkill( SkillName.Tactics, 110.0 );
			SetSkill( SkillName.FistFighting, 110.0 );
			SetSkill( SkillName.Musicianship, 115.0, 135.5 );

			Fame = 26000;
			Karma = -26000;

			VirtualArmor = 90;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 3 );
			AddLoot( LootPack.Rich,3 );
			AddLoot( LootPack.MedScrolls, 2 );
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

			int attackChoice = Utility.RandomMinMax( 1, 2 );
            Map map = this.Map;

			switch ( attackChoice  )
			{
				case 1: // energy burst
				{
					BossSpecialAttack.PerformTargettedAoE(
						this,
						target,
						1,
						"Just sign here...",
						Hue,  // hue
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
					BossSpecialAttack.PerformSlam(
                	    boss: this,
                	    warcry: "Hell calls thy name!",
                	    hue: Hue,
                	    rage: 1,
                	    range: 6,
                	    physicalDmg: 0,
						energyDmg: 100
                	);
                	break;
			    }
			}
		}

		public override void OnThink()
		{
			if ( DateTime.Now >= NextPickup )
			{
				switch( Utility.RandomMinMax( 0, 3 ) )
				{
					case 0:	Peace( Combatant ); break;
					case 1:	Undress( Combatant ); break;
					case 2:	Suppress( Combatant ); break;
					case 3:	Provoke( Combatant ); break;
				}
			}
			base.OnThink();
		}

		
		public override bool CanRummageCorpses{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override int TreasureMapLevel{ get{ return 5; } }
		public override int Meat{ get{ return 3; } }
		public override int Hides{ get{ return 22; } }
		public override HideType HideType{ get{ return HideType.Hellish; } }
		public override int Skin{ get{ return Utility.Random(9); } }
		public override SkinType SkinType{ get{ return SkinType.Demon; } }
		public override int Skeletal{ get{ return Utility.Random(7); } }
		public override SkeletalType SkeletalType{ get{ return SkeletalType.Devil; } }

		public override int Cloths{ get{ return 11; } }
		public override ClothType ClothType{ get{ return ClothType.Fiendish; } }

		public HeraldOfDiscordance( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );
			writer.Write( m_NextSpecialAttack );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			if ( version >= 1 )
			{
				m_NextSpecialAttack = reader.ReadDateTime();
			}
		}
	}
}