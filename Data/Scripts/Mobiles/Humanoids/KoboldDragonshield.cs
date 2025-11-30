using System;
using Server;
using Server.Items;
using System.Collections;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a Kobold champion corpse" )]
	public class KoboldDragonshield : BaseCreature
	{
		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Ratman; } }

		[Constructable]
		public KoboldDragonshield() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a Kobold Champion";
			Body = 245;
			Hue = 0x25;
			BaseSoundID = 0x543;

			SetStr( 147, 215 );
			SetDex( 91, 115 );
			SetInt( 61, 85 );

			SetHits( 95, 123 );

			SetDamage( 4, 14 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 25, 35 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.MagicResist, 70.1, 85.0 );
			SetSkill( SkillName.Swords, 60.1, 85.0 );
			SetSkill( SkillName.Tactics, 75.1, 90.0 );
			SetSkill( SkillName.FistFighting, 60.1, 85.0 );

			Fame = 20;
			Karma = -20;

			VirtualArmor = 5;

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
			AddLoot( LootPack.Average );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 1; } }
		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 1; } }
		public override HideType HideType{ get{ return HideType.Horned; } }

		public override int GetAttackSound(){ return 0x5FD; }	// A
		public override int GetDeathSound(){ return 0x5FE; }	// D
		public override int GetHurtSound(){ return 0x5FF; }		// H

		public KoboldDragonshield( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}