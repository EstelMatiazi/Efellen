using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Custom.Ascensions;

namespace Server.Custom.DefenderOfTheRealm
{
    public class RewardTables
    {
        /* 
        public Type ItemType;
        public int Cost; <-- cost in marks
        public int ItemID; <-- itemID 
        public string Name; <-- item name
        public bool Hueable; <-- sets default hue for the npc faction
        public int Hue <-- 0 for faction items, hue for items with hardcoded hues;
        public object[] Args; <-- sets amount of items
         */
        public static RewardInfo[] CommonRewards = new RewardInfo[]
        {
            new RewardInfo(typeof(MagicalDyes), 50, 0xF7D, "Magical Dyes",true,0),
            new RewardInfo(typeof(SlayerDeed), 1000, 0x400B, "Slayer Deed",false,0),
            new RewardInfo(typeof(LuckyHorseShoes), 1250, 0xFB6, "Lucky Horse Shoes",false,0),
            new RewardInfo(typeof(EtherealHorse), 250, 0x20DD, "Ethereal Horse",true,0),
            new RewardInfo(typeof(EtherealLlama), 250, 0x20F6, "Ethereal Llama",true,0),
            new RewardInfo(typeof(ArcaneDust), 50, 12265, "100 Arcane Dust",false,33,100),
            new RewardInfo(typeof(ArcaneDust), 500, 12265, "1000 Arcane Dust",false,33,1000),
            new RewardInfo(typeof(TomeOfPower), 2000, 0x2259, "Tome of Power", false,23)
        };

        public static RewardInfo[] DefenderRewards = new RewardInfo[]
        {
            new RewardInfo(typeof(PotionOfMight),                           20,   0x2827, "Potion of Might",                  false, 0xB9E),
            new RewardInfo(typeof(ChargerOfTheFallen),                      500,  0x0499, "Charger of the Fallen",             true,  0),
            new RewardInfo(typeof(Artifact_DefenderOfTheRealmArms),         1000, 0x1410, "Defender's Arms",                   true,  0),
            new RewardInfo(typeof(Artifact_DefenderOfTheRealmChestpiece),   1000, 0x1415, "Defender's Chest",                  true,  0),
            new RewardInfo(typeof(Artifact_DefenderOfTheRealmGloves),       1000, 0x1414, "Defender's Gloves",                 true,  0),
            new RewardInfo(typeof(Artifact_DefenderOfTheRealmGorget),       1000, 0x1413, "Defender's Gorget",                 true,  0),
            new RewardInfo(typeof(Artifact_DefenderOfTheRealmHelmet),       1000, 0x1412, "Defender's Helmet",                 true,  0),
            new RewardInfo(typeof(Artifact_DefenderOfTheRealmLeggings),     1000, 0x46AA, "Defender's Leggings",               true,  0),
            new RewardInfo(typeof(AscensionScroll),                         300,  0x2D9E, "Berserker Ascension Scroll",        false, 0x0F1, AscensionType.Berserker),
            new RewardInfo(typeof(AscensionScroll),                         300,  0x2D9E, "Archmage Ascension Scroll",         false, 0x213, AscensionType.Archmage),
            new RewardInfo(typeof(AscensionScroll),                         300,  0x2D9E, "Crusader Ascension Scroll",         false, 0x0F8, AscensionType.Crusader),
        };

        public static RewardInfo[] ScourgeRewards = new RewardInfo[]
        {
            new RewardInfo(typeof(PotionOfMight),                           20,   0x2827, "Potion of Might",                  false, 0xB9E),
            new RewardInfo(typeof(ChargerOfTheFallen),                      500,  0x0499, "Charger of the Fallen",             true,  0),
            new RewardInfo(typeof(Artifact_ScourgeOfTheRealmArms),         1000,  0x1410, "Scourge's Arms",                   true,  0),
            new RewardInfo(typeof(Artifact_ScourgeOfTheRealmChestpiece),   1000,  0x1415, "Scourge's Chest",                  true,  0),
            new RewardInfo(typeof(Artifact_ScourgeOfTheRealmGloves),       1000,  0x1414, "Scourge's Gloves",                 true,  0),
            new RewardInfo(typeof(Artifact_ScourgeOfTheRealmGorget),       1000,  0x1413, "Scourge's Gorget",                 true,  0),
            new RewardInfo(typeof(Artifact_ScourgeOfTheRealmHelmet),       1000,  0x1412, "Scourge's Helmet",                 true,  0),
            new RewardInfo(typeof(Artifact_ScourgeOfTheRealmLeggings),     1000,  0x46AA, "Scourge's Leggings",               true,  0),
            new RewardInfo(typeof(AscensionScroll),                         300,  0x2D9E, "Berserker Ascension Scroll",        false, 0x0F1, AscensionType.Berserker),
            new RewardInfo(typeof(AscensionScroll),                         300,  0x2D9E, "Palemaster Ascension Scroll",       false, 0xB97, AscensionType.Palemaster),
            new RewardInfo(typeof(AscensionScroll),                         300,  0x2D9E, "Assassin Ascension Scroll",         false, 0x233, AscensionType.Assassin),
            new RewardInfo(typeof(AscensionScroll),                         300,  0x2D9E, "Blackguard Ascension Scroll",         false, 0x47E, AscensionType.Blackguard),
        };

        public static RewardInfo[] ShadowbrokerRewards = new RewardInfo[]
        {
            new RewardInfo(typeof(PotionOfDexterity),                       20,   0x2827, "Potion of Dexterity",              false, 0xB51),
            new RewardInfo(typeof(EtherealOstard),                          500,  0x2135, "Ethereal Ostard",                  true,  0),
            new RewardInfo(typeof(Artifact_ShadowBrokerArms),              1000,  0x13CD, "Shadow Broker Arms",               true,  0),
            new RewardInfo(typeof(Artifact_ShadowBrokerTunic),             1000,  0x13CC, "Shadow Broker Tunic",              true,  0),
            new RewardInfo(typeof(Artifact_ShadowBrokerGloves),            1000,  0x13C6, "Shadow Broker Gloves",             true,  0),
            new RewardInfo(typeof(Artifact_ShadowBrokerGorget),            1000,  0x13C7, "Shadow Broker Gorget",             true,  0),
            new RewardInfo(typeof(Artifact_ShadowBrokerCap),               1000,  0x1DB9, "Shadow Broker Cap",                true,  0),
            new RewardInfo(typeof(Artifact_ShadowBrokerLeggings),          1000,  0x13D2, "Shadow Broker Leggings",           true,  0),
            new RewardInfo(typeof(AscensionScroll),                         250,  0x2D9E, "Assassin Ascension Scroll",        false, 0x233, AscensionType.Assassin),
        };

        public static RewardInfo[] NatureMasterRewards = new RewardInfo[]
        {
            new RewardInfo(typeof(PotionOfWisdom),                          20,   0x2827, "Potion of Wisdom",                 false, 0xB9E),
            new RewardInfo(typeof(EtherealReptalon),                        1000, 0x2D95, "Ethereal Reptalon",                true,  0),
            new RewardInfo(typeof(Artifact_NatureMasterArms),              1000,  0x13CD, "Arms of the Nature's Master",      true,  0),
            new RewardInfo(typeof(Artifact_NatureMasterCoat),              1000,  0x13CC, "Coat of the Nature's Master",      true,  0),
            new RewardInfo(typeof(Artifact_NatureMasterGloves),            1000,  0x13C6, "Gloves of the Nature's Master",    true,  0),
            new RewardInfo(typeof(Artifact_NatureMasterHeaddress),         1000,  0x1DB9, "Nature's Master Headdress",        true,  0),
            new RewardInfo(typeof(Artifact_NatureMasterLeggings),          1000,  0x13D2, "Nature's Master Leggings",         true,  0),
            new RewardInfo(typeof(Artifact_NatureMasterGorget),            1000,  0x13C7, "Nature's Master Gorget",           true,  0),
            new RewardInfo(typeof(AscensionScroll),                         250,  0x2D9E, "Berserker Ascension Scroll",       false, 0x0F1, AscensionType.Berserker),
        };

        public static RewardInfo[] HealerRewards = new RewardInfo[]
        {
            new RewardInfo(typeof(PotionOfWisdom),                          20,   0x2827, "Potion of Wisdom",                 false, 0xB9E),
            new RewardInfo(typeof(EtherealUnicorn),                         500,  0x25CE, "Ethereal Unicorn",                 true,  0),
            new RewardInfo(typeof(Artifact_ArmsOfDevotion),                1000,  0x13CD, "Arms of Devotion",                 true,  0),
            new RewardInfo(typeof(Artifact_TunicOfDevotion),               1000,  0x13CC, "Tunic of Devotion",                true,  0),
            new RewardInfo(typeof(Artifact_GauntletsOfDevotion),           1000,  0x13C6, "Gauntlets of Devotion",            true,  0),
            new RewardInfo(typeof(Artifact_CoifOfDevotion),                1000,  0x1DB9, "Coif of Devotion",                 true,  0),
            new RewardInfo(typeof(Artifact_LeggingsOfDevotion),            1000,  0x13D2, "Leggings of Devotion",             true,  0),
            new RewardInfo(typeof(AscensionScroll),                         250,  0x2D9E, "Crusader Ascension Scroll",        false, 0x0F8, AscensionType.Crusader),
        };

        public static RewardInfo[] WeaveRewards = new RewardInfo[]
        {
            new RewardInfo(typeof(PotionOfWisdom),                          20,   0x2827, "Potion of Wisdom",                 false, 0xB9E),
            new RewardInfo(typeof(EtherealKirin),                           500,  0x25A0, "Ethereal Kirin",                   true,  0),
            new RewardInfo(typeof(Artifact_ArcaneCap),                     1000,  0x1DB9, "Arcane Cap",                       true,  0),
            new RewardInfo(typeof(Artifact_ArcaneArms),                    1000,  0x13CD, "Arcane Arm",                       true,  0),
            new RewardInfo(typeof(Artifact_ArcaneTunic),                   1000,  0x13CC, "Arcane Tunic",                     true,  0),
            new RewardInfo(typeof(Artifact_ArcaneGloves),                  1000,  0x13C6, "Arcane Gloves",                    true,  0),
            new RewardInfo(typeof(Artifact_ArcaneGorget),                  1000,  0x13C7, "Arcane Gorget",                    true,  0),
            new RewardInfo(typeof(Artifact_ArcaneLeggings),                1000,  0x13CB, "Arcane Leggings",                  true,  0),
            new RewardInfo(typeof(AscensionScroll),                         250,  0x2D9E, "Archmage Ascension Scroll",        false, 0x213, AscensionType.Archmage),
        };
    }
}
