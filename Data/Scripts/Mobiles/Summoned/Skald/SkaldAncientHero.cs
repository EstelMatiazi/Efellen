using System;
using Server;
using Server.Items;
using System.Collections;
using System.Net;

namespace Server.Mobiles
{
	[CorpseName( "an Ancient Heroe's corpse" )]
	public class SkaldAncientHero : BaseCreature
	{
		public override bool DeleteCorpseOnDeath { get { return true; } }

		public override bool IsDispellable { get { return false; } }
		
		public override double GetFightModeRanking( Mobile m, FightMode acqType, bool bPlayerOnly )
		{
			return ( m.Int + m.Skills[SkillName.Musicianship].Value ) / Math.Max( GetDistanceToSqrt( m ), 1.0 );
		}

		[Constructable]
		public SkaldAncientHero() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an Ancient Hero";
			Body = 400; 
			FacialHairItemID = Utility.RandomList( 0, 0, 8254, 8255, 8256, 8257, 8267, 8268, 8269 );
			Utility.AssignRandomHair( this );
			HairHue = Utility.RandomHairHue();
			FacialHairHue = HairHue;
			Hue = Utility.RandomSkinColor();
			SetStr( 250 );
			SetDex( 250 );
			SetInt( 100 );

			SetHits( 350 );
			SetStam( 250 );
			SetMana( 0 );

			SetDamage( 15, 19 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 50 );
			SetResistance( ResistanceType.Fire, 50 );
			SetResistance( ResistanceType.Cold, 50 );
			SetResistance( ResistanceType.Poison, 50 );
			SetResistance( ResistanceType.Energy, 50 );	

			SetSkill( SkillName.MagicResist, 109.9 );
			SetSkill( SkillName.Tactics, 109.9 );
			SetSkill( SkillName.FistFighting, 120.9 );

			Fame = 0;
			Karma = 0;
			VirtualArmor = 50;
			ControlSlots = 0;
		}

		public void AddEquipment()
        {
            if (Deleted)
                return;

            AddItem(new BarbarianBoots());
			AddItem(new RoyalLoinCloth());

            if (Utility.RandomBool())
                AddItem(new NorseHelm());

            switch (Utility.Random(2))
            {
                case 0: AddItem(new VikingSword()); break;
                case 1: AddItem(new OrnateAxe());   break;
            }
        }	

		public override bool CanRummageCorpses{ get{ return false; } }
		
		public override bool BleedImmune{ get{ return true; } }

		public SkaldAncientHero( Serial serial ): base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

		}
	}
}