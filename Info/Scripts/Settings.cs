namespace Server
{
	public static class MySettings
	{

	///////////////////////////////////////////////////////////////////////////////////////////////
	// INDEX - USE CTRL-F to get to the desired section ///////////////////////////////////////////
	// 001 - SYSTEM OPTIONS ///////////////////////////////////////////////////////////////////////
	// 002 - GAME OPTIONS /////////////////////////////////////////////////////////////////////////
	// 003 - PLAYER OPTIONS ///////////////////////////////////////////////////////////////////////
	// 004 - QUESTS & TREASURE ////////////////////////////////////////////////////////////////////
	// 005 - SKILLS ///////////////////////////////////////////////////////////////////////////////
	// 006 - CRAFTING /////////////////////////////////////////////////////////////////////////////
	// 007 - MONSTERS & CREATURES /////////////////////////////////////////////////////////////////
	// 008 - MERCHANTS ////////////////////////////////////////////////////////////////////////////
	// 009 - HOMES & SHIPS ////////////////////////////////////////////////////////////////////////
	// 010 - PETS, MOUNTS, & FOLLOWERS ////////////////////////////////////////////////////////////
	// 011 - ACKNOWLEDGEMENT //////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////

	///////////////////////////////////////////////////////////////////////////////////////////////
	// 001 - SYSTEM OPTIONS ///////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////

	// Enables commands to be entered into the server console. WARNING: May cause crashes so enable it at your own risk.

		public static bool S_EnableConsole = false;

	// Enables output messages to the console. Messages are from tasks that run and the steps of running the world building command.

		public static bool ConsoleLog = false;

	// These settings will create a button on the Message of the Day. If you do not fill in a website name, the text next to the 
	// button will simply say Website. When players select the button, it should open their browser to that site.
	// EXAMPLE: https://google.com

		public static string S_WebsiteLink = "";
		public static string S_WebsiteName = "";

	// The game saves itself after this many minutes in decimal format between 10 and 240 minutes.

		public static double S_ServerSaveMinutes = 15.0;

	// If true, saves the game when your character logs out. Helpful for single player games.

		public static bool S_SaveOnCharacterLogout = true;

	// The server has some self-cleaning and self-sustaining scripts it runs every hour, 3 hours, & 24 hours. If you run
	// a 24x7 server, you can set the below to false since your server will run these at those times, but if you play
	// single player, and you turn the server on/off as required, then set this to true so these routines at least run
	// when you start the game for you.

		public static bool S_RunRoutinesAtStartup = true;

	// This setting is the number of days a character must exist before a player can delete them.

		public static double S_DeleteDays = 0.0;

	// If true, players can just type in a name and password and it will create an account for them.

		public static bool S_AutoAccounts = true;

	// The port you want your server to listen on.

		public static int S_Port = 2593;

	// If you want to enter your IP for external connections, you can enter it here. Otherwise, the autodetect function
	// below can likely do it for you automatically.
	// EXAMPLES
	// public static string S_Address = "192.16.1.4";
	// public static string S_Address = "211.12.35.213";

		public static string S_Address = null;

	// Here you can enter the name of your server/world

		public static string S_ServerName = "Efellen";

	// If true, your public IP address will be auto detected to help with external connections.

		public static bool S_AutoDetect = true;



	///////////////////////////////////////////////////////////////////////////////////////////////
	// 002 - GAME OPTIONS /////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////

	// The percent chance a floor trap will trigger in whole number format and no less than 5 percent.

		public static int S_FloorTrapTrigger = 20;

	// If set to 1,000 gold or higher, then the bribery system will be enabled that allows characters to give this amount of gold
	// to the Assassin Guildmaster so they can bribe the right people and remove a murder count one at a time (never applies to
	// fugitives, and assassin guild members only pay half this amount).

		public static int S_Bribery = 50000;

	// There are almost 300 classic artifacts in the game, as well as artifacts created for this game that are specifically named
	// and designed. These are items like 'Stormbringer' or 'Conan's Lost Sword'. By default, these items will retain their
	// appearance and color no matter what is done to them. Setting this to true will allow a player to use items to change the appearance of the items, 
	// but they will always retain their name. This is false by default.

		public static bool S_ChangeArtyLook = false;

	// The below setting is the number of minutes that a player character corpse will turn into bones, which can be used in
	// conjunction with the setting below. These two settings, when added together, are the total number of minutes that a
	// player has to find their corpse and potentially collect their belongings. The default for this setting is 10 minutes
	// and the below is 110 minutes for a combined 2 hours or 120 minutes.

		public static int S_CorpseDecay = 7;

	// The below setting is the number of minutes that a player character bones will decay. This option, as well as the
	// option above, could potentially be used to have player character corpses remain longer or for a more difficult style
	// of play where the corpse and belongings vanish immediatley. If running a multiplayer game, where PVP is promoted and
	// you want to use a more difficult style of play, then setting these two combined minutes to something long enough for
	// an enemy player to take the dead character's belongings may be desired. The default is 110 minutes.

		public static int S_BoneDecay = 113;

	///////////////////////////////////////////////////////////////////////////////////////////////
		// 003 - PLAYER OPTIONS ///////////////////////////////////////////////////////////////////////
		///////////////////////////////////////////////////////////////////////////////////////////////

	// You can increase the rate that stats gain from 50.0 (slow) to 10.0 (fast).

		public static double S_StatGain = 25.0;

	// How many minutes between stat gains which helps with the above setting. This can be between 5.0 to 60.0 minutes.

		public static double S_StatGainDelay = 10.0;

	// If true, then characters will be able to set a custom title for their character in the HELP section.

		public static bool S_AllowCustomTitles = false;

	// If true, player footstep sounds will change based on terrain (grass, stone, wood, etc.). 
	// Must also uncheck the 'Play footstep sounds' option in ClassicUO settings
		public static bool S_PlayerTerrainFootstepSounds = true;


	///////////////////////////////////////////////////////////////////////////////////////////////
	// 004 - QUESTS & TREASURE ////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////

	// If set to true (default), then a character will get a warning before they are entering Skara Brae. This area is an extensive
	// quest driven area, that has some quest requirements to be met before they can leave that area.

		public static bool S_WarnSkaraBrae = true;

	// If set to true (default), then a character will get a warning before they are entering the Bottle City. This area
	// is an extensive quest driven area, that has some quest requirements to be met before they can leave that area.

		public static bool S_WarnBottleCity = true;

	///////////////////////////////////////////////////////////////////////////////////////////////
	// 005 - SKILLS ///////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////

	// This number can be set from zero to 10, where 10 will give characters faster skill gain and zero
	// leaves it normal (default).

		public static int S_SkillGain = 0;

	///////////////////////////////////////////////////////////////////////////////////////////////
	// 006 - CRAFTING /////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////

	// When viewing crafting line items, this will show the 1, 10, and 100 buttons next to each line item.
	// If you want to allow players to craft many items at once, and don't want the trade window screen
	// having many buttons on it, then leave this set to false.

		public static bool S_CraftButtons = true;

	// If false, characters will get a CAPTCHA windows occasionally to avoid unattended resource gathering with macros.

		public static bool S_AllowMacroResources = false;

	// If false, then characters will need to have the appropriate tool equipped to gather resources.
	// Affects harvest tools that serve additional purposes (such as grave digging or hunting treasure).

		public static bool S_AllowBackpackHarvestTool = false;

	///////////////////////////////////////////////////////////////////////////////////////////////
		// 007 - MONSTERS & CREATURES /////////////////////////////////////////////////////////////////
		///////////////////////////////////////////////////////////////////////////////////////////////

	// These values represent the percentage of difficulty increase for dungeons with the below categories. The higher
	// the number, the more difficult the monsters will become. So setting the difficult dungeons to 50, will increase
	// monsters in "difficult" dungeons by 50%. This increase affects their attributes, skills, fame, karma, and statistics.
	// Any creatures, that can be tamed, will have their taming skill requirements raised as well. Gold is also increased.
	// The default values here are 0, 30, 60, 90, and 120. Some higher level creatures will scale these values down, in
	// order for them to remain defeatable.

		public static int S_Normal = 0;
		public static int S_Difficult = 30;
		public static int S_Challenging = 60;
		public static int S_Hard = 90;
		public static int S_Deadly = 120;


	// These 5 settings control whether that particular land has safari animals spawn like elephants, giraffes, cheetahs, or zebras.
	// These are values between 0 and 100, where 0 never occurs and 100 always does. Setting it to '50' would be 50% of the time.

		public static int S_Safari_Sosaria = 0;		// Sosaria
		public static int S_Safari_Lodoria = 0;		// Lodoria
		public static int S_Safari_Serpent = 50;	// Serpent Island
		public static int S_Safari_Kuldar = 0;		// Kuldar
		public static int S_Safari_Savaged = 50;	// Savaged Empire

	// These two settings set the overall minimum and maximum amount of minutes that a creature will respawn.
	// Creatures will respawn between the range below, and it is most effective for dungeon areas. Some of the
	// spawners (on the land) may have spawners that spawn multiple amounts of creatures. In those few cases,
	// the spawner will spawn one at a time in the time range provided below. Some creatures have a longer spawn
	// rate than most of the creatures, and those particular creatures will use this spawn rate. They will then
	// add additional minutes to reflect the longer spawn.

		public static int S_SpawnMin = 45;
		public static int S_SpawnMax = 60;

	///////////////////////////////////////////////////////////////////////////////////////////////
	// 008 - MERCHANTS ////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////


	// If false, then vendors will not buy things from player characters. Merchant crates will also be disabled and act as normal containers.

		public static bool S_VendorsBuyStuff = true;

	// If true, then players can buy merchant crates to lock down in their house to sell the items they craft.

		public static bool S_MerchantCrates = false;

	// If true, then the custom merchant is enabled. After a [buildworld command, these merchants will appear in
	// the various settlements with their wagon. They will sell any custom items you set in the Info/Scripts folder.
	// This is in the Merchant.cs file. WARNING: Vendors can only sell 250 different items. NOTE: Many settings
	// here, that affect vendors, will not affect the custom merchant.

		public static bool S_CustomMerchant = false;




	///////////////////////////////////////////////////////////////////////////////////////////////
	// 009 - HOMES & SHIPS ////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////

	// If true, then co-owners of houses will have the same permissions as owners. The security choice gump will
	// then specify this dual ownership when choosing an item security level. The default setting is false, where
	// co-owners have much more limited permissions as the standard game allows.

		public static bool S_HouseOwners = false;

	// When true (default setting), characters can use lawn tools (from architects) to add items to the outside
	// of their home like trees, shrubs, fences, lave, water, and other items. Lawn tools require an amount of
	// gold to place items. If this was previously true and characters placed lawn items, and then you set it to
	// false, the lawn items will refund the gold back to the character's bank box and the lawn tools will be
	// removed from the game.

		public static bool S_LawnsAllowed = true;

	// When true (default setting), characters can use remodeling tools (from architects) to add items to their
	// home like walls, doors, tiles, and other items. Remodeling tools require an amount of gold to place items.
	// If this was previously true and characters placed remodeling items, and then you set it to false, the
	// remodeling items will refund the gold back to the character's bank box and the remodeling tools will be
	// removed from the game.

		public static bool S_ShantysAllowed = true;

	// The number of days, no less than 5.0 (decimal format), that a boat or magic carpet will decay if on
	// the sea not used.

		public static double S_BoatDecay = 30.0;

	// The number of days, no less than 30.0 (decimal format), that a home will decay if an owner never shows up.

		public static double S_HomeDecay = 30.0;

	// If true, this means that the players can dye construction contracts so their pre-designed home is
	// entirely in that same color.

		public static bool S_AllowHouseDyes = false;

	// If true, then players can make use of the custom house system. Otherwise they can only purchase the
	// pre-built classic houses.

		public static bool S_AllowCustomHomes = true;

	// If true, the public basement system is active. This lets players buy basement doors for their homes
	// and basement doors will appear in some trade shops. These lead to the same basement public area and
	// is usually used for multiplayer game environments.

		public static bool S_Basements = true;

	///////////////////////////////////////////////////////////////////////////////////////////////
	// 010 - PETS, MOUNTS, & FOLLOWERS ////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////
	// If true, some areas will not allow you to mount a creature for riding. This makes dungeons (for example)
	// more challenging. Player mounts get stabled when they go in certain areas like dungeons or caves and
	// they will remount them when they leave these areas. Set to false if you do not want to limit where they
	// take mounts. Keep in mind that having no mounts in dungeons does increase the difficulty.

		public static bool S_NoMountsInCertainRegions = true;

	// If true, then characters on mounts will dismount when they enter a building. They should mount their
	// steed again when they leave.

		public static bool S_NoMountBuilding = true;

	// If true, then characters on mounts will dismount when they enter a player character's home. They
	// should mount their steed again when they leave.

		public static bool S_NoMountsInHouses = true;

	// If true, then followers will not only guard you when commanded, but guard the other
	// followers in your group.

		public static bool S_FriendsGuardFriends = true;

	// The below setting default is '5', where this value can be between 0 and 20. This is the number of
	// extra stabled pets players get (beyond the normal amount of '2'), where anything more will rely
	// on their skills in druidism, taming, veterinary, and herding.

		public static int S_Stables = 5;

	// This number can be set from 0 to 30, which determines the number of days before you can bond
	// a pet one tamed (default is 1).

		public static int S_BondDays = 1;
		
	///////////////////////////////////////////////////////////////////////////////////////////////
	// 011 - ACKNOWLEDGEMENT //////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////

	// If true, then it notifies the game that you have reviewed the various game settings here and
	// are confirming that you set each one to your personal play style and what you expect from the
	// game. Any settings here, that interfere with your enjoyment of the game, are under your
	// control and you can change these settings at any time if you wish to.

		public static bool S_Reviewed = false;


	}
}
