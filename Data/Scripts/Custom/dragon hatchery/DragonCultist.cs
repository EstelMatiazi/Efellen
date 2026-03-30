using System;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections;


namespace Server.Mobiles
{
    [CorpseName("a cultist corpse")]
    public class DraconicCultist : BaseCreature
    {
        [Constructable]
        public DraconicCultist() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            if (Female = Utility.RandomBool())
            {
                Body = 401;
                Name = NameList.RandomName( "female" );
                AddItem(new Server.Items.LeatherBoots());
            }
            else
            {
                Body = 400;
                FacialHairItemID = Utility.RandomList(0, 0, 8254, 8255, 8256, 8257, 8267, 8268, 8269);
                AddItem(new Server.Items.Shoes());
                Name = NameList.RandomName( "male" );
            }
            
            Utility.AssignRandomHair(this);
            HairHue = Utility.RandomHairHue();
            FacialHairHue = HairHue;
            Title = "the draconic cultist";
            Hue = Utility.RandomSkinHue();

            SetStr(150, 200);
            SetDex(80, 100);
            SetInt(200, 250);

            SetHits(190, 230);

            SetDamage(10, 15);

            SetDamageType(ResistanceType.Physical, 25);
            SetDamageType(ResistanceType.Fire, 75);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.Psychology, 80.0, 100.0);
            SetSkill(SkillName.Magery, 80.0, 110.0);
            SetSkill(SkillName.MagicResist, 75.0, 95.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.FistFighting, 60.0, 80.0);

            Fame = 9000;
            Karma = -9000;

            VirtualArmor = 40;

            DragonRobe robe = new DragonRobe();
            robe.Hue = GetRandomHue();
            
            int power = Utility.RandomMinMax(150, 300);
            robe = (DragonRobe)Server.LootPackEntry.Enchant(this, power, robe);
            
            AddItem(robe);
            PackReg( Utility.RandomMinMax( 2, 10 ) );
			PackReg( Utility.RandomMinMax( 2, 10 ) );
			PackReg( Utility.RandomMinMax( 2, 10 ) );
            if ( 0.7 > Utility.RandomDouble() )
				PackItem( new ArcaneGem() );
        }

        public virtual int GetRandomHue()
		{
			switch ( Utility.Random( 7 ) )
			{
				default:
				case 0: return Utility.RandomBlueHue();
				case 1: return Utility.RandomGreenHue();
				case 2: return Utility.RandomRedHue();
				case 3: return Utility.RandomYellowHue();
				case 4: return Utility.RandomNeutralHue();
                case 5: return Utility.RandomPinkHue();
                case 6: return Utility.RandomRedHue();
			}
		}

        public override bool ClickTitle{ get{ return false; } }
		public override bool ShowFameTitle{ get{ return false; } }
		public override bool CanRummageCorpses{ get{ return true; } }
		public override bool AlwaysAttackable{ get{ return true; } }
		
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.MedScrolls, 2);
        }

        private static readonly string[] AttackLines = new string[]
		{
		    "Ashardalon shall rise again!",
		    "Our lords are everlasting!",
		    "The dragonlords shall feast upon thy flesh, {0}!",
		    "Your suffering will fuel Ashardalon's rebirth!",
            "Witness the power of the draconic covenant!",
            "The ancient one stirs, and you have sealed your doom!",
            "Ashardalon's flame shall reduce you to ash, {0}!",
            "Fire shall set us free!",
            "My magic will devour you!",
            "The dragon god favors me!",
            "Each drop of your blood brings our lord closer to awakening!",
            "Mortal arrogance! You face a true disciple of the eternal wyrm!",
            "Ashardalon's wrath flows through my very veins!",
            "Thy soul shall burn in dragonfire!",
            "The prophecy cannot be stopped, {0}! The dragon rises!",
            "Centuries of preparation will not be undone by the likes of you!",
            "Feel the fury of one who has gazed upon Ashardalon's glory!",
            "You tresspass into a sacred place! Your fate is sealed!",
            "The dragon-god hungers, and I shall offer him your essence!",
            "Pathetic whelp! I have forgotten more magic than you will ever know!",
            "Ashardalon whispers to me... he promises me your screaming soul!"
        };

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);
			if(Utility.RandomDouble() < 0.25)
            {
				int i = Utility.Random(AttackLines.Length);
			    Say(string.Format(AttackLines[i], attacker.Name));                
            }
        }

        public DraconicCultist(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}