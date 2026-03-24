using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	[CorpseName( "a witch corpse" )]
	public class WitchOfTheDreadHost : BaseCreature
	{
		[Constructable]
		public WitchOfTheDreadHost() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "evil witch" );
            Title = "Bruxa da Hoste do Pavor";
			Hue = 743;
			Body = 0x191;
			Utility.AssignRandomHair( this );
			HairHue = Utility.RandomHairHue();
			SetStr( 146, 195 );
			SetDex( 71, 150 );
			SetInt( 181, 255 );

			SetDamage( 14, 21 );
            SetHits( 225 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 40 );
			SetResistance( ResistanceType.Fire, 40 );
			SetResistance( ResistanceType.Cold, 40 );
			SetResistance( ResistanceType.Poison, 50 );
			SetResistance( ResistanceType.Energy, 30 );

			SetSkill( SkillName.Psychology, 90.0 );
			SetSkill( SkillName.Bludgeoning, 85.0 );
			SetSkill( SkillName.Magery, 106.0 );
			SetSkill( SkillName.Meditation, 90.0 );
			SetSkill( SkillName.MagicResist, 110.0 );
            SetSkill( SkillName.Necromancy, 95.1, 100.0 );
			SetSkill( SkillName.Spiritualism, 95.1, 100.0 );
			SetSkill( SkillName.Tactics, 95.0 );
			SetSkill( SkillName.FistFighting, 75.0 );

			Fame = 1200;
			Karma = -1200;
			VirtualArmor = 10;
			
			PackReg( 10, 15 );
		
			AddItem( new WildStaff() );
			Server.Misc.IntelligentAction.DressUpWizards( this, false );
			MorphingTime.ColorMyClothes(this, 1109, 0);

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.MedPotions );
		}

		public override bool OnBeforeDeath()
		{
			if ( Server.Misc.IntelligentAction.HealThySelf( this ) ){ return false; }
			return base.OnBeforeDeath();
		}

		public override bool ClickTitle{ get{ return false; } }
		public override bool ShowFameTitle{ get{ return false; } }
		public override bool AlwaysAttackable{ get{ return true; } }
		public override bool CanRummageCorpses{ get{ return false; } }
		public override int Skeletal{ get{ return Utility.Random(3); } }
		public override SkeletalType SkeletalType{ get{ return SkeletalType.Brittle; } }

		public WitchOfTheDreadHost( Serial serial ) : base( serial )
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