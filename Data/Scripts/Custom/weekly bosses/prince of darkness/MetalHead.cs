using System;
using Server;
using System.Collections; 
using System.Collections.Generic;
using Server.Items; 
using Server.ContextMenus; 
using Server.Misc; 
using Server.Network;
using Server.Mobiles;

namespace Server.Mobiles 
{
	public class MetalHead : BaseCreature 
	{
		[Constructable] 
		public MetalHead() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 ) 
		{
           	SpeechHue = Utility.RandomTalkHue();
			Hue = Utility.RandomSkinColor();

			if ( this.Female = Utility.RandomBool() ) 
			{
				this.Body = 0x191; 
				this.Name = NameList.RandomName( "female" ); 
				Utility.AssignRandomHair( this );
				HairHue = Utility.RandomHairHue();
			} 
			else 
			{ 
				this.Body = 0x190; 
				this.Name = NameList.RandomName( "male" );
				Utility.AssignRandomHair( this );
				FacialHairItemID = Utility.RandomList( 0, 8254, 8255, 8256, 8257, 8267, 8268, 8269 );
				HairHue = Utility.RandomHairHue();
				FacialHairHue = HairHue;
			}

			SetStr( Utility.RandomMinMax( 225, 355 ) );
			SetDex( Utility.RandomMinMax( 95, 130 ) );
			SetInt( Utility.RandomMinMax( 50, 75 ) );

			SetHits( RawStr );

			SetDamage( 11, 24 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 45 );
			SetResistance( ResistanceType.Fire, 35 );
			SetResistance( ResistanceType.Cold, 35 );
			SetResistance( ResistanceType.Poison, 35 );
			SetResistance( ResistanceType.Energy, 35 );

			SetSkill( SkillName.Searching, 40.0 );
			SetSkill( SkillName.Anatomy, 55.0 );
			SetSkill( SkillName.MagicResist, 80.0 );
			SetSkill( SkillName.Bludgeoning, 60.0 );
			SetSkill( SkillName.Fencing, 60.0 );
			SetSkill( SkillName.FistFighting, 60.0 );
			SetSkill( SkillName.Swords, 60.0 );
			SetSkill( SkillName.Tactics, 60.0 );

			Fame = 1100;
			Karma = -1100;

			VirtualArmor = 0;
            EquipMetalGear();
            
            this.Title = "the Metalhead";
        
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
        }

		public override bool ClickTitle{ get{ return false; } }
		public override bool ShowFameTitle { get { return false; } }
		public override bool CanRummageCorpses{ get{ return false; } }
		public override bool AlwaysAttackable{ get{ return true; } }
		
		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );
			Server.Misc.IntelligentAction.CryOut( this );
		}


		public MetalHead( Serial serial ) : base( serial ) 
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

        public void EquipMetalGear()
        {
            ChaosShield shield = new ChaosShield();

            switch (Utility.Random(10))
            {
                case 0: this.AddItem(new BattleAxe { Hue = 0x497 }); break;
                case 1: this.AddItem(new Halberd { Hue = 0x497 }); break;
                case 2: this.AddItem(new DoubleAxe { Hue = 0x497 }); break;
                case 3: this.AddItem(new ExecutionersAxe { Hue = 0x497 }); break;
                case 4: this.AddItem(new WarAxe { Hue = 0x497 }); break;
                case 5: this.AddItem(new TwoHandedAxe { Hue = 0x497 }); break;
                case 6:
                    this.AddItem(new VikingSword { Hue = 0x497 });
                    if (Utility.Random(3) == 1) { shield.Hue = 0x497; this.AddItem(shield); }
                    break;
                case 7:
                    this.AddItem(new ThinLongsword { Hue = 0x497 });
                    if (Utility.Random(3) == 1) { shield.Hue = 0x497; this.AddItem(shield); }
                    break;
                case 8:
                    this.AddItem(new Longsword { Hue = 0x497 });
                    if (Utility.Random(3) == 1) { shield.Hue = 0x497; this.AddItem(shield); }
                    break;
                case 9:
                    this.AddItem(new Broadsword { Hue = 0x497 });
                    if (Utility.Random(3) == 1) { shield.Hue = 0x497; this.AddItem(shield); }
                    break;
            }

            Robe robe = new Robe
            {
                Name = "Metalhead robe",
                Hue = 0x497
            };
            this.AddItem(robe);

            LeatherThighBoots boots = new LeatherThighBoots
            {
                Name = "Metalhead Boots",
                Hue = 0x497
            };
            this.AddItem(boots);

            NorseHelm helm = new NorseHelm
            {
                Name = "Metalhead Helmet",
                Hue = 0x497
            };
            this.AddItem(helm);

            RingmailArms arms = new RingmailArms {
                Name = "Metalhead Ringmail Arms",
                Hue = 0x497
            };
            this.AddItem(arms);

            RingmailLegs legs = new RingmailLegs {
                Name = "Metalhead Ringmail legs",
                Hue = 0x497
            };
            this.AddItem(legs);

            RingmailChest chest = new RingmailChest {
                Name = "Metalhead Ringmail Chest",
                Hue = 0x497
            };
            this.AddItem(chest);
        }
	}
}