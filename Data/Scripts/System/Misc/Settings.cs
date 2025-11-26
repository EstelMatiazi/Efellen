using System;
using Server;
using System.Collections;
using Server.Misc;
using Server.Network;
using Server.Commands;
using Server.Commands.Generic;
using Server.Mobiles;
using Server.Accounting;
using Server.Regions;
using Server.Targeting;
using System.Collections.Generic;
using Server.Items;
using Server.Spells.Fifth;
using System.IO;
using System.Xml;

namespace Server
{
    class MyServerSettings
    {
		public static void UpdateWarning()
		{
			if ( Utility.DateUpdated() != 20240922 )
				Console.WriteLine( "Warning: Your World.exe requires an update!" );
		}

		public static double ServerSaveMinutes() // HOW MANY MINUTES BETWEEN AUTOMATIC SERVER SAVES
		{
			if ( MySettings.S_ServerSaveMinutes > 240 ){ MySettings.S_ServerSaveMinutes = 240.0; }
			else if ( MySettings.S_ServerSaveMinutes < 10 ){ MySettings.S_ServerSaveMinutes = 10.0; }

			return MySettings.S_ServerSaveMinutes;
		}

		public static int FloorTrapTrigger()
		{
			// THERE ARE MANY HIDDEN TRAPS ON THE FLOOR, BUT THE PERCENT CHANCE
			// IS SET BELOW THAT THEY WILL TRIGGER WHEN WALKED OVER BY PLAYERS
			// 20% IS THE DEFAULT...WHERE 0 IS NEVER AND 100 IS ALWAYS

			if ( MySettings.S_FloorTrapTrigger < 5 ){ MySettings.S_FloorTrapTrigger = 5; }

			return MySettings.S_FloorTrapTrigger;
		}

		public static int GetUnidentifiedChance()
		{
			// CHANCE THAT ITEMS ARE UNIDENTIFIED
			// IF YOU SET THIS VERY LOW, THEN MERCANTILE STARTS TO BECOME A USELESS SKILL
			return 25;
		}

		public static double StatGain()
		{
			// THIS IS NOT ADVISED, BUT YOU CAN INCREASE THE CHANCE OF A STAT GAIN TO OCCUR
			// STATS ONLY GAIN WHEN SKILLS ARE USED, SO A SKILL GAIN POTENTIAL MUST PRECEDE A STAT GAIN

			if ( MySettings.S_StatGain > 50 ){ MySettings.S_StatGain = 50.0; } else if ( MySettings.S_StatGain < 10 ){ MySettings.S_StatGain = 10.0; }

			return MySettings.S_StatGain; // LOWER THIS VALUE FOR MORE STAT GAIN - 33.3 IS DEFAULT - 0.01 IS VERY OFTEN
		}

		public static TimeSpan StatGainDelay()
		{
			// THIS IS NOT ADVISED, BUT YOU CAN CHANGE THE TIME BETWEEN STAT GAINS
			// HOW MANY MINUTES BETWEEN STAT GAINS

			if ( MySettings.S_StatGainDelay > 60 ){ MySettings.S_StatGainDelay = 60.0; } else if ( MySettings.S_StatGainDelay < 5 ){ MySettings.S_StatGainDelay = 5.0; }

			return TimeSpan.FromMinutes( MySettings.S_StatGainDelay ); // 15.0 IS DEFAULT
		}

		public static int StatGainDelayNum()
		{
			if ( MySettings.S_StatGainDelay > 60 ){ MySettings.S_StatGainDelay = 60; } else if ( MySettings.S_StatGainDelay < 5 ){ MySettings.S_StatGainDelay = 5; }

			return Convert.ToInt32( MySettings.S_StatGainDelay );
		}

		public static TimeSpan PetStatGainDelay()
		{
			// THIS IS NOT ADVISED, BUT YOU CAN CHANGE THE TIME BETWEEN STAT GAINS FOR PETS
			// HOW MANY MINUTES BETWEEN STAT GAINS
			return TimeSpan.FromMinutes( 5.0); // 5.0 IS DEFAULT
		}

		public static int GetTimeBetweenQuests()
		{
			return 60; // MINUTES
		}

		public static int GetTimeBetweenArtifactQuests()
		{
			return 10080; // MINUTES -- one week
		}

		public static int GetGoldCutRate() // DEFAULT IS 25% OF WHAT GOLD NORMALLY DROPS
		{
			// THIS AFFECTS MONEY ELEMENTS SUCH AS...
			// MONSTER DROPS
			// CHEST DROPS
			// CARGO
			// MUSEUM SEARCHES
			// SHOPPE PROFITS
			// SOME QUESTS
			return 25;
		}

		public static double DamageToPets()
		{
			// IF YOU THINK TAMER PETS SOMEHOW RUIN YOUR GAME, YOU CAN INCREASE THIS VALUE
			// AS IT WILL INCREASE A CREATURES DAMAGE TOWARD SUCH PETS AND IT ONLY ALTERS MELEE DAMAGE 
			return 1.0; // DEFAULT 1.0
		}

		public static int CriticalToPets()
		{
			// IF YOU THINK TAMER PETS SOMEHOW RUIN YOUR GAME, YOU CAN INCREASE THIS VALUE
			// AS IT WILL INCREASE A CREATURES CHANCE OF DOING DOUBLE MELEE DAMAGE TO PETS
			return 20; // DEFAULT 0
		}

		public static int SpellDamageIncreaseVsMonsters()
		{
			return 200;
		}

		public static int SpellDamageIncreaseVsPlayers()
		{
			return 100;
		}

		public static int QuestRewardModifier()
		{
			// FOR ASSSASSIN, THIEF, FISHING, & STANDARD QUESTS
			return 0; // PERCENT
		}

		public static int PlayerLevelMod( int value, Mobile m )
		{
			double mod = 1.0;
				if ( m is PlayerMobile ){ mod = 1.0; } // ONLY CHANGE THIS VALUE

			value = (int)( value * mod );
				if ( value < 0 ){ value = 1; }

			return value;
		}

		public static int WyrmBody()
		{
			return 723; // THIS IS WHAT WYRMS LOOK LIKE IN THE GAME...IF YOU WANT A DIFFERENT APPEARANCE THEN CHANGE THIS VALUE
		}

		public static bool FastFriends( Mobile m )					// IF TRUE, FOLLOWERS WILL ATTEMPT TO STAY WITH YOU IF YOU ARE RUNNING FAST
		{															// OTHERWISE THEY HAVE THEIR OWN DEFAULT SPEEDS
			if ( m is BaseCreature && ((BaseCreature)m).ControlMaster != null ){ return true; } // THIS VALUE YOU WOULD CHANGE
			return true;
		}

		public static double BoatDecay() // HOW MANY DAYS A BOAT WILL LAST BEFORE IT DECAYS, WHERE using IT REFRESHES THE TIME
		{
			if ( MySettings.S_BoatDecay < 5 ){ MySettings.S_BoatDecay = 5.0; }
			return MySettings.S_BoatDecay;
		}

		public static double HomeDecay() // HOW MANY DAYS A HOUSE WILL LAST BEFORE IT DECAYS, WHERE using IT REFRESHES THE TIME
		{
			if ( MySettings.S_HomeDecay < 30 ){ MySettings.S_HomeDecay = 30.0; }
			return MySettings.S_HomeDecay;
		}

		public static double ResourcePrice()
		{
			return 1;
		}

		public static double SellGoldCutRate()
		{
			return 0.5;
		}

		public static double HigherPrice()
		{
			return 0;
		}

		public static int LowerReg()
		{
			return 50;
		}

		public static int LowerMana()
		{
			return 40;
		}

		public static int LowMana()
		{
			if ( MyServerSettings.LowerMana() > 50 )
				return MyServerSettings.LowerMana();

			return MyServerSettings.LowerMana();
		}

		public static int LowReg()
		{
			if ( MyServerSettings.LowerReg() > 50 )
				return MyServerSettings.LowerReg();

			return MyServerSettings.LowerReg();
		}

		public static int HousesPerAccount() // HOW MANY HOUSES CAN ONE ACCOUNT HAVE, WHERE -1 IS NO LIMIT
		{
			return 5;
		}

		public static bool LineOfSight( Mobile m, bool hidden )
		{
			if ( m is BaseCreature && m.CanHearGhosts && hidden && m.Hidden )
				return true;
			else if ( m is BaseCreature && m.CanHearGhosts )
				return true;

			return false;
		}

		public static int Resources()
		{
			return 1;
		}

		public static bool Humanoids()
		{
			if ( Utility.RandomMinMax(1,20) == 1 )
				return true;

			return false;
		}

		public static bool Humanoid()
		{
			if ( Utility.RandomBool() )
				return true;

			return false;
		}

		public static bool RandomCityVisitor()
		{
			return 50 > Utility.Random(100);
		}

		public static bool BlackMarket()
		{
			return false;
		}

		public static double CorpseDecay()
		{
			if ( MySettings.S_CorpseDecay < 1 ){ MySettings.S_CorpseDecay = 0; }

			return (double)MySettings.S_CorpseDecay;
		}

		public static double BoneDecay()
		{
			if ( MySettings.S_BoneDecay < 1 ){ MySettings.S_BoneDecay = 0; }

			return (double)MySettings.S_BoneDecay;
		}

		public static bool AlterArtifact( Item item )
		{
			if ( item.ArtifactLevel > 0 && !MySettings.S_ChangeArtyLook )
				return false;

			return true;
		}

		public static bool MonstersAllowed()
		{
			return false;
		}

		public static double DeleteDelay()
		{
			if ( MySettings.S_DeleteDays < 1 ){ MySettings.S_DeleteDays = 0; }

			return (double)MySettings.S_DeleteDays;
		}

		public static double SpecialWeaponAbilSkill() // MIN SKILLS NEEDED TO START WEAPON SPECIAL ABILITIES
		{
			return 70.0;
		}

		public static int JoiningFee( Mobile m )
		{
			int fee = 2000;
		
			if ( m != null && m is PlayerMobile )
				fee = fee + ( ((PlayerMobile)m).CharacterGuilds * fee );

			if ( fee < 2000 )
				fee = 2000;

			if ( GetPlayerInfo.isFromSpace( m ) )
				fee = fee * 4;

			return fee;
		}

		public static int ExtraStableSlots()
		{
			int stable = MySettings.S_Stables;

			if ( stable < 0 )
				stable = 0;

			if ( stable > 20 )
				stable = 20;

			return stable;
		}

		public static int TrainMulti()
		{
			return 1;
		}

		public static bool Safari( Land land )
		{
			bool safari = false;

			if ( land == Land.Sosaria && Utility.RandomMinMax(1,100) <= MySettings.S_Safari_Sosaria )
				safari = true;

			if ( land == Land.Lodoria && Utility.RandomMinMax(1,100) <= MySettings.S_Safari_Lodoria )
				safari = true;

			if ( land == Land.Serpent && Utility.RandomMinMax(1,100) <= MySettings.S_Safari_Serpent )
				safari = true;

			if ( land == Land.Kuldar && Utility.RandomMinMax(1,100) <= MySettings.S_Safari_Kuldar )
				safari = true;

			if ( land == Land.Savaged && Utility.RandomMinMax(1,100) <= MySettings.S_Safari_Savaged )
				safari = true;

			return safari;
		}

		public static bool SafariStore()
		{
			bool safari = false;

			if ( Utility.RandomMinMax(1,100) <= MySettings.S_Safari_Sosaria )
				safari = true;

			if ( Utility.RandomMinMax(1,100) <= MySettings.S_Safari_Lodoria )
				safari = true;

			if ( Utility.RandomMinMax(1,100) <= MySettings.S_Safari_Serpent )
				safari = true;

			if ( Utility.RandomMinMax(1,100) <= MySettings.S_Safari_Kuldar )
				safari = true;

			if ( Utility.RandomMinMax(1,100) <= MySettings.S_Safari_Savaged )
				safari = true;

			return safari;
		}

		public static int SkillBoost()
		{
			return 0;
		}

		public static string SkillGypsy( string area )
		{
			int skills = 10;

			if ( area == "savage" )
				skills = 11;
			else if ( area == "fugitive" )
				skills = 13;
			else if ( area == "alien" )
				skills = 40;

			return skills.ToString();
		}

		public static void SkillBegin( string area, PlayerMobile pm )
		{
			pm.SkillBoost = SkillBoost();

			if ( area == "savage" )
				pm.SkillStart = 11000;
			else if ( area == "alien" )
				pm.SkillStart = 40000;
			else if ( area == "fugitive" )
				pm.SkillStart = 13000;
			else
				pm.SkillStart = 10000;

			pm.Skills.Cap = pm.SkillStart + pm.SkillBoost + pm.SkillEther;
		}

		public static int SkillBase()
		{
			return ( 10000 + SkillBoost() );
		}

		public static double SkillGain()
		{
			int skill = MySettings.S_SkillGain;

			if ( skill > 10 )
				skill = 10;

			if ( skill < 1 )
				skill = 0;

			return skill * 0.1;
		}

		public static int FoodCheck()
		{
			return 5;
		}

		public static double BondDays()
		{
			int days = MySettings.S_BondDays;

			if ( days > 30 )
				days = 30;

			if ( days < 0 )
				days = 0;

			return (double)days;
		}

		public static string BondingDays()
		{
			string text = "To bond a creature, simply give them some food that they prefer. Once you do that, give them another. They should then be bonded to you from that moment onward.";

			if ( MySettings.S_BondDays > 0 )
				text = "To bond a creature, simply give them some food that they prefer. Once you do that, give them another " + MySettings.S_BondDays.ToString() + " days later. They should then be bonded to you from that moment onward.";

			return text;
		}

		public static int AdditionalFollowerSlots()
		{
			return 0;
		}

		#region KoperPets
		public static bool KoperPets()
		{
			return true;
		}

		public static bool KoperPetsImmersive()
		{
			return true;
		}

		public static double KoperTamingChance()
		{
			return 1.0;
		}

		public static double KoperHerdingChance()
		{
			return 1.0;
		}

		public static int KoperCooldown()
		{
			return 60;
		}
		#endregion


		public static int StartingGold()
		{
			int min = 150;
			int max = 250;
			int gold = Utility.RandomMinMax(min,max);
			return gold;
		}

		public static double DeathStatAndSkillLoss()
		{
			return 5.0;
		}
	}
}