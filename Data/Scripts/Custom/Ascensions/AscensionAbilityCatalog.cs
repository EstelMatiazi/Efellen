using System;
using System.Collections.Generic;

namespace Server.Custom.Ascensions
{
    public class AscensionAbilityEntry
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public bool IsPassive { get; set; }
        public string Command { get; set; }
        public int IconArt { get; set; }
        public int IconHue { get; set; }
        public string Description { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }

        public AscensionAbilityEntry(string name, int level, bool isPassive, string command, int iconArt, int iconHue, string description, int offsetX = 0, int offsetY = 0)
        {
            Name = name;
            Level = level;
            IsPassive = isPassive;
            Command = command;
            IconArt = iconArt;
            IconHue = iconHue;
            Description = description;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }
    }

    public static class AscensionAbilityCatalog
    {
        public static List<AscensionAbilityEntry> GetActiveAbilities(AscensionType type)
        {
            List<AscensionAbilityEntry> list = new List<AscensionAbilityEntry>();

            switch (type)
            {
                case AscensionType.Berserker:
                    list.Add(new AscensionAbilityEntry("Rage", 1, false, "[BerserkerRage", 0xF47, 0x21,
                        "Enters a bloodthirsty rage. Gains +(15+level) STR, immunity to paralysis, +10% damage and -10% defense chance.<br>" +
                        "Lasts 10 + level sec. Cooldown: 1 min. On expiry, consumes 50% stamina.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 10:</BASEFONT> +level/2 resistances, +5% damage.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 15:</BASEFONT> Hits above 40% max HP are reduced to 40%.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 20:</BASEFONT> Additional +5% damage."));

                    list.Add(new AscensionAbilityEntry("Leap Slam", 6, false, "[BerserkerLeapSlam", 0x143B, 0,
                        "Leaps 3 + level/4 tiles toward the target. On landing, deals 20-35 + ((STR/15)+(level/3)) physical damage to adjacent enemies.<br>" +
                        "Cooldown: 9 sec (-1 sec per 3 levels).<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 12:</BASEFONT> Stuns and paralyzes adjacent enemies for 3 seconds."));

                    list.Add(new AscensionAbilityEntry("Warcry", 11, false, "[BerserkerWarCry", 0x0FC7, 0x35,
                        "Terrifying roar. Enemies lose (25+level) STR, (10+level) DEX and INT for (20+level) sec.<br>" +
                        "Allies gain 2 stamina every 2 sec for the same duration.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 16:</BASEFONT> Enemies lose 20+2*level stamina immediately and 2+level every 2 sec."));

                    list.Add(new AscensionAbilityEntry("Tenacity", 18, false, "[BerserkerTenacity", 0x1B74, 0x21,
                        "Fully heals hit points immediately and again every 10 sec for 30 sec.<br>" +
                        "Cooldown: 2 minutes.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 20:</BASEFONT> Also regenerates and fully heals all stamina with each pulse."));
                    break;

                case AscensionType.Archmage:
                    list.Add(new AscensionAbilityEntry("Arcane Storm", 1, false, "[ArchmageArcaneStorm", 0xEFA, 0x48D,
                        "Fires (6 + 1 per 3 levels) arcane projectiles. Each missile deals 10-18 + (INT/25 + level/3) energy damage. Cost: 45 mana.<br>" +
                        "Cooldown: 45 - 1/level sec.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 10:</BASEFONT> 3%/level chance to cause -(8 + level/4) energy resist.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 15:</BASEFONT> Target loses 34 + level/2 stamina.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 20:</BASEFONT> Killing with the storm resets Conflux cooldown."));

                    list.Add(new AscensionAbilityEntry("Conflux", 6, false, "[ArchmageConflux", 0x1F19, 0x48D,
                        "Amplifies arcane power. The archmage's spell damage is increased by +10% for 10 + 1/level sec.<br>" +
                        "Cooldown: 2 minutes.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 12:</BASEFONT> The spell damage bonus is increased to +15%."));

                    list.Add(new AscensionAbilityEntry("Mana Singularity", 11, false, "[ArchmageMassSingularity", 0x0E2D, 0x48D,
                        "Creates a collapsing arcane vortex within 2+(level/3) tiles. Explodes after 2.5 sec, dealing 25-40 + (INT/10 + level/2) area damage.<br>" +
                        "Enemies lose 20+level mana. Recovers 4 mana per target. Cooldown: 60 sec.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 16:</BASEFONT> Paralyzes enemies for 4 sec and drains 20+level stamina."));

                    list.Add(new AscensionAbilityEntry("Timestop", 18, false, "[ArchmageTimestop", 0x1810, 0x48D,
                        "Bends the very fabric of time. Hostile creatures within 3+(level/5) tiles are completely paralyzed for (12 + level/4) sec.<br>" +
                        "Cost: 60 mana. Cooldown: 3 minutes.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 20:</BASEFONT> When the paralysis expires, enemies lose 40+level mana and stamina."));
                    break;

                case AscensionType.Palemaster:
                    list.Add(new AscensionAbilityEntry("Undying Hordes", 1, false, "[PalemasterUndyingHordes", 0x1AE2, 0x455,
                        "Summons a relentless horde of undead (skeletons, mummies, giants and dragons depending on your level).<br>" +
                        "Duration: 60 + 3/level sec. Cooldown: 3 minutes.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 17-20:</BASEFONT> Summons undead giants, ancient mummies and skeletal dragons."));

                    list.Add(new AscensionAbilityEntry("Enervate", 6, false, "[PalemasterEnervate", 0x182D, 0x455,
                        "Drains the vitality of the living. Deals 40 + (level/3) cold damage and mana burn in an area.<br>" +
                        "Restores up to 50 health and mana to the Palemaster and reduces targets' STR and DEX by -(5+level/4)%.<br>" +
                        "Cooldown: 1 minute.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 12:</BASEFONT> Healing and mana cap increased to 100."));

                    list.Add(new AscensionAbilityEntry("Circle of Death", 11, false, "[PalemasterCircleOfDeath", 0x0E2E, 0x455,
                        "Corrupts the area around the target. Creatures take 12-18 + (level/2) + (INT/20) cold damage per second for 3 + level/4 sec.<br>" +
                        "Cooldown: 2 minutes.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 16:</BASEFONT> Enemies in the circle lose stamina each second equal to 50% of the damage."));

                    list.Add(new AscensionAbilityEntry("Danse Macabre", 18, false, "[PalemasterDanseMacabre", 0x1CF0, 0x455,
                        "Tears the veil to the world of the dead. Every second for level/3 sec, summons undead giants, mummies or fully heals the Palemaster's health.<br>" +
                        "Cooldown: 4 minutes.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 20:</BASEFONT> Can summon Skeletal Dragons and debuff enemies by -20% STR."));
                    break;

                case AscensionType.Crusader:
                    list.Add(new AscensionAbilityEntry("Smite", 1, false, "[CrusaderSmite", 0x0F5E, 0x481,
                        "Explosion of holy flames that deals 20-32 + (level/2 + STR/15) damage to evil creatures in an area.<br>" +
                        "Cooldown: 1 minute.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 10:</BASEFONT> Paralyzes targets with very negative karma for 4 sec.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 15:</BASEFONT> Heals allies in the area for 20 + level/2 health.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 20:</BASEFONT> Enemies suffer continuous holy burn for 12 sec."));

                    list.Add(new AscensionAbilityEntry("Charge", 6, false, "[CrusaderCharge", 0x1BC3, 0x481,
                        "Swift charge toward the target location (range 3+level/5). Chance to paralyze enemies around the impact.<br>" +
                        "Cooldown: 9 sec (-1 sec per 3 levels).<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 12:</BASEFONT> Deals 30-45 + (STR/15 + level/3) physical damage to all adjacent."));

                    list.Add(new AscensionAbilityEntry("Aura of Hope", 11, false, "[CrusaderAuraOfHope", 0x099A, 0x481,
                        "Protective celestial aura. The crusader and allies restore 12+level/2 health and 6+level/2 mana and stamina every 4 sec for 30 sec.<br>" +
                        "Cooldown: 3 minutes.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 16:</BASEFONT> When the buff expires, fully heals all affected targets' health."));

                    list.Add(new AscensionAbilityEntry("Heavenly Gate", 18, false, "[CrusaderHeavensGate", 0x2C7F, 0x481,
                        "Opens the heavenly gates and summons a powerful celestial Archon to wage war at your side until defeated.<br>" +
                        "Cooldown: 5 minutes.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 20:</BASEFONT> 1%/level chance to summon two celestial Archons at once."));
                    break;

                case AscensionType.Assassin:
                    list.Add(new AscensionAbilityEntry("Noxious Cloud", 1, false, "[AssassinNoxiousCloudAbility", 0x0F0A, 0x44,
                        "Cloud of poisonous vapors that inflicts greater poison in an area.<br>" +
                        "Cooldown: 1 minute.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 10:</BASEFONT> The cloud inflicts Deadly Poison.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 15:</BASEFONT> The cloud inflicts Lethal Poison.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 20:</BASEFONT> Enemies lose 20 poison resistance for 30 sec."));

                    list.Add(new AscensionAbilityEntry("Crippling Poison", 6, false, "[AssassinCripplingPoison", 0x0F51, 0x44,
                        "Throws a crippling flask, applying deadly poison and preventing the target from walking for level/2 sec.<br>" +
                        "Cooldown: 1 minute.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 12:</BASEFONT> The poison becomes Lethal and affects enemies within a 2-tile radius."));

                    list.Add(new AscensionAbilityEntry("Toxic Surge", 11, false, "[AssassinToxicSurge", 0x0F7E, 0x44,
                        "Assassin's surge. For 30 sec, deals +10% additional weapon damage against poisoned targets.<br>" +
                        "Cooldown: 2 minutes.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 16:</BASEFONT> 1%/level chance to reset Noxious Cloud's cooldown."));

                    list.Add(new AscensionAbilityEntry("Cleansing Annihilation", 18, false, "[AssassinCleansingAnnihilation", 0x1AE2, 0x44,
                        "Applies lethal poison and instantly consumes all of its damage ticks at once.<br>" +
                        "If the target survives, infects everyone within a 3-tile radius with lethal poison. Cooldown: 4 min.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 20:</BASEFONT> 1%/level chance to reset Toxic Surge's cooldown."));
                    break;

                case AscensionType.Blackguard:
                    list.Add(new AscensionAbilityEntry("Dark Succor", 1, false, "[BlackguardDarkSuccor", 0x140E, 0x497,
                        "Enters an unholy trance for 30+1/level sec, gaining +15+level/2 STR. Cooldown: 90 sec.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 10:</BASEFONT> Deals +0.75%/level extra damage against positive karma targets.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 15:</BASEFONT> Killing enemies restores 2 + level/2 mana.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 20:</BASEFONT> Killing enemies heals 6 + level/2 health."));

                    list.Add(new AscensionAbilityEntry("Death's Advance", 6, false, "[BlackguardDeathsAdvance", 0x1BC4, 0x497,
                        "Spectral advance toward a target tile. Upon arrival, knocks back adjacent enemies.<br>" +
                        "Cooldown: 9 sec (-1 sec per 3 levels).<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 12:</BASEFONT> Deals 30-45 + (STR/15 + level/3) physical damage on impact."));

                    list.Add(new AscensionAbilityEntry("Chains of Ice", 11, false, "[BlackguardChainsOfIce", 0x1A03, 0x48D,
                        "Icy chains that freeze nearby enemies for 3 + 1/4 levels sec and deal 30-43 + STR/15 cold damage.<br>" +
                        "Cooldown: 90 seconds.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 16:</BASEFONT> The chains explode for additional cold damage at the end of the paralysis."));

                    list.Add(new AscensionAbilityEntry("Frostwyrm's Fury", 18, false, "[BlackguardsFrostwyrmsFury", 0x2235, 0x48D,
                        "Summons a Dracolich from the depths of the underworld bound to your will, fighting until it falls in combat.<br>" +
                        "Cooldown: 5 minutes.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 20:</BASEFONT> 1%/level chance to summon two Frostwyrms simultaneously."));
                    break;

                case AscensionType.Skald:
                    list.Add(new AscensionAbilityEntry("War Chant", 1, false, "[SkaldWarChant", 0x0EB2, 0x53,
                        "Song of bravery. Allies within 4 tiles gain +(5+level) STR and DEX, and +(5+level) Tactics and Magic Resistance for 10+level sec.<br>" +
                        "Cooldown: 90 sec.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 10:</BASEFONT> +0.5%/level weapon damage.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 15:</BASEFONT> Slaying enemies restores 5 health and 3 stamina.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 20:</BASEFONT> 2%/level chance to reactivate with no cost or cooldown."));

                    list.Add(new AscensionAbilityEntry("Saga of Valor", 6, false, "[SkaldSagaOfValor", 0x0EB3, 0x53,
                        "The skald and allies within 4 tiles gain +(level/2)% hit chance for 15+level sec.<br>" +
                        "Cooldown: 90 sec.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 12:</BASEFONT> Also grants +(level/2)% defense chance."));

                    list.Add(new AscensionAbilityEntry("Song of Thunder", 11, false, "[SkaldSongOfThunder", 0x0EB4, 0x48D,
                        "Summons rhythmic lightning. Every 3 sec, bolts strike enemies dealing 30-45 (+fame/1000) energy damage.<br>" +
                        "Cooldown: 180 sec.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 16:</BASEFONT> Fires an additional bolt with each pulse."));

                    list.Add(new AscensionAbilityEntry("Dirge of the Fallen", 18, false, "[SkaldDirgeOfTheFallen", 0x1428, 0x53,
                        "Intones an epic requiem summoning 3+(level/4) legendary warriors to massacre your opponents for up to 90 sec.<br>" +
                        "Cooldown: 5 minutes.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 20:</BASEFONT> 1%/level chance to automatically trigger Song of Thunder."));
                    break;

                case AscensionType.Reaver:
                    list.Add(new AscensionAbilityEntry("Gorge", 1, false, "[ReaverGorge", 0x0EC2, 0x20,
                        "Curses the ground with vicious blood. Enemies in the area suffer -1.25 physical resist per level for 10+level sec.<br>" +
                        "Cooldown: 3 minutes.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 10:</BASEFONT> Enemies in the pool are crippled and cannot run.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 15:</BASEFONT> -0.66/level penalty to all elemental resistances.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 20:</BASEFONT> Enemies killed in the pool explode for area physical damage."));

                    list.Add(new AscensionAbilityEntry("Exsanguinate", 6, false, "[ReaverExsanguinate", 0x0F47, 0x20,
                        "Whirlwind attack dealing 18-26 + STR/15 damage and causing all to bleed for 26-32 damage every 3 sec for 13+level sec.<br>" +
                        "Cooldown: 1 minute.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 12:</BASEFONT> Restores 9 health for each enemy hit."));

                    list.Add(new AscensionAbilityEntry("Bloodstorm", 11, false, "[ReaverBloodstorm", 0x1182, 0x20,
                        "Violent blood explosion hitting everyone within 3+(level/5) tiles, dealing 20-32 + (level/2 + STR/15) damage.<br>" +
                        "Cooldown: 2 minutes.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 16:</BASEFONT> 0.5%/level chance to reset Exsanguinate's cooldown."));

                    list.Add(new AscensionAbilityEntry("Absolute Tyranny", 18, false, "[ReaverAbsoluteTyranny", 0x153B, 0x20,
                        "Absolute domination on the battlefield, decimating the resistance and morale of adjacent adversaries.<br>" +
                        "Cooldown: 4 minutes."));
                    break;

                case AscensionType.Kensai:
                    list.Add(new AscensionAbilityEntry("Battle Meditation", 1, false, "[KensaiBattleMeditation", 0x227B, 0,
                        "Supreme blade trance. Deals +(10+level/2)% sword damage and +(5+level/2)% attack speed for 20+level sec.<br>" +
                        "Cooldown: 2 minutes.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 10:</BASEFONT> +(10+level/2) defense chance.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 15:</BASEFONT> 1%/level chance to reset Culling Strike's cooldown.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 20:</BASEFONT> Slaying enemies reduces Tempest's cooldown."));

                    list.Add(new AscensionAbilityEntry("Kai", 6, false, "[KensaiKai", 0x13FF, 0,
                        "Instant leap toward the target (range 3+level/4). Deals 20-35 + ((DEX/15) + level/3) damage to all adjacent.<br>" +
                        "Cooldown: 9 sec (-1 sec per 3 levels).<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 12:</BASEFONT> 1%/level chance to deliver a Culling Strike to everyone around."));

                    list.Add(new AscensionAbilityEntry("Culling Strike", 11, false, "[KensaiCullingStrike", 0x13FE, 0,
                        "Execution strike. For 30 sec, attacks against enemies below 10% health have a 2%/level chance to deal +80% damage.<br>" +
                        "Cooldown: 3 minutes.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 16:</BASEFONT> The execution threshold increases to targets below 15% health."));

                    list.Add(new AscensionAbilityEntry("Tempest", 18, false, "[KensaiTempest", 0x27A4, 0,
                        "Becomes a hurricane of cutting blades, hitting all enemies within 6 tiles for 70-85 + DEX/15 physical damage.<br>" +
                        "Cooldown: 1 minute.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 20:</BASEFONT> Slaying an enemy with Tempest has a chance to reactivate it for free."));
                    break;

                case AscensionType.Hierophant:
                    list.Add(new AscensionAbilityEntry("Divine Wrath", 1, false, "[HierophantDivineWrath", 0x26BC, 0x482,
                        "Divine fire falls upon the target area, dealing 30-48 + (karma/1000) fire damage (+15% against evil). Cooldown: 1 min.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 10:</BASEFONT> +25% damage against negative karma creatures.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 15:</BASEFONT> Paralyzes for 4 sec and inflicts continuous holy burn.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 20:</BASEFONT> 20% chance to cast a second time immediately."));

                    list.Add(new AscensionAbilityEntry("Exalted Presence", 6, false, "[HierophantExaltedPresence", 0x0E28, 0x482,
                        "Overwhelming sacred presence. Forces enemies to move away and interrupts their attacks immediately.<br>" +
                        "Cooldown: 2 minutes.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 12:</BASEFONT> Affected evil creatures are paralyzed for 8 seconds."));

                    list.Add(new AscensionAbilityEntry("Consecrated Ground", 11, false, "[HierophantConsecratedGround", 0x0E24, 0x482,
                        "Purifies up to 4 tiles around. Allies regenerate 12+level/3 health and 5+level/3 mana; evil creatures take constant fire damage.<br>" +
                        "Cooldown: 3 minutes.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 16:</BASEFONT> Your healing spells restore an additional +25% health."));

                    list.Add(new AscensionAbilityEntry("Divine Power", 18, false, "[HierophantDivinePower", 0x1F03, 0x482,
                        "Channels avatar power. Gains +20 STR and DEX, +15 Tactics and Spiritualism, regenerates 8 HP/sec and +20% bludgeoning damage.<br>" +
                        "Lasts 30+level sec. Cooldown: 3 minutes.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 20:</BASEFONT> Slaying evil enemies has a 20% chance to reset Divine Wrath."));
                    break;

                case AscensionType.ArcaneArcher:
                    list.Add(new AscensionAbilityEntry("Imbue Arrow", 1, false, "[ArcaneArcherImbueArrows", 0x2B02, 0x489,
                        "Enchants arrows with magical power for 30+1/level sec. Arrows have a 2%/level chance to strike the target's worst resistance.<br>" +
                        "Cooldown: 2 minutes.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 10:</BASEFONT> Triggered effects have a chance to fire twice.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 15:</BASEFONT> Triggered effects deal +25% damage.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 20:</BASEFONT> Chance to reset Barrage's cooldown."));

                    list.Add(new AscensionAbilityEntry("Charged Arrows", 6, false, "[ArcaneArcherChargedArrows", 0x0F3F, 0x489,
                        "Fires a massive piercing arrow (range 6 tiles) that deals 45-75 + (INT/15 + DEX/15) elemental damage.<br>" +
                        "Cooldown: 1 minute.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 12:</BASEFONT> 0.5%/level chance to reset Arcane Volley's cooldown."));

                    list.Add(new AscensionAbilityEntry("Arcane Volley", 11, false, "[ArcaneArcherArcaneVolley", 0x2FB7, 0x489,
                        "Fires a rapid barrage of arcane arrows in an area (4 tiles), dealing 55-75 + inscription/10 elemental damage.<br>" +
                        "Cooldown: 30 seconds.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 16:</BASEFONT> 1%/level chance to fire the volley twice in a row."));

                    list.Add(new AscensionAbilityEntry("Barrage", 18, false, "[ArcaneArcherBarrage", 0x2B02, 0x489,
                        "Relentless torrent of magical projectiles, attacking the target level/4 times in a single instant.<br>" +
                        "Cooldown: 3 minutes.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 20:</BASEFONT> 0.25%/level chance to be fired twice."));
                    break;

                default:
                    list.Add(new AscensionAbilityEntry("Ancestral Power", 1, false, "[Ativar", 0x2259, 0,
                        "Primary ability granted by the spiritual ascension upon reaching the corresponding level."));
                    break;
            }

            return list;
        }

        public static List<AscensionAbilityEntry> GetPassiveAbilities(AscensionType type)
        {
            List<AscensionAbilityEntry> list = new List<AscensionAbilityEntry>();

            switch (type)
            {
                case AscensionType.Berserker:
                    list.Add(new AscensionAbilityEntry("Cleave", 2, true, null, 0x0F4B, 0x21,
                        "Two-handed strikes have a 4% + 1%/level chance to deliver a second attack to an adjacent enemy.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 5 & 13:</BASEFONT> Chance to deliver a second and third additional strike."));

                    list.Add(new AscensionAbilityEntry("Uncanny Dodge", 8, true, null, 0x1515, 0,
                        "Superhuman reflexes. 25 + 1%/level chance to completely ignore the effects of traps.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 17:</BASEFONT> The chance increases to 45 + 1%/level."));

                    list.Add(new AscensionAbilityEntry("Pummeling Strikes", 14, true, null, 0x1414, 0,
                        "Two-handed weapon strikes have a (level/4)% chance to completely ignore the target's physical armor.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 19:</BASEFONT> The chance increases to (level/2)%."));

                    list.Add(new AscensionAbilityEntry("Undying Wrath", 20, true, null, 0x1AE2, 0x21,
                        "If you take a blow that would reduce your health to zero or less, it is reduced to 1 hit point and Warcry's cooldown is reset.<br>" +
                        "Can occur once per minute."));
                    break;

                case AscensionType.Archmage:
                    list.Add(new AscensionAbilityEntry("Mana Vault", 2, true, null, 0x09A8, 0x48D,
                        "When someone tries to drain your mana, there is a 2%/level chance to instantly recover it.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 5:</BASEFONT> Enemies take 15-25 + level/2 energy damage when trying to drain.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 13:</BASEFONT> Enemies are paralyzed for 2 seconds."));

                    list.Add(new AscensionAbilityEntry("Weave Reflection", 8, true, null, 0x1008, 0x48D,
                        "Gain a 2.5%/level passive chance to reflect spells back at the caster.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 17:</BASEFONT> Reflecting spells has a 1%/level chance to reset Conflux's cooldown."));

                    list.Add(new AscensionAbilityEntry("Arcane Tempest", 14, true, null, 0x1F56, 0x48D,
                        "Casting spells at hostile targets has a (0.25 * level)% chance to trigger a free Mana Singularity with no cooldown.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 19:</BASEFONT> Chance to reset Arcane Storm's cooldown."));

                    list.Add(new AscensionAbilityEntry("Weave Unraveling", 20, true, null, 0x1ECD, 0x48D,
                        "Harmful spells have a 0.25%/level chance to create 6-12 rifts in the Weave around the target for 12-22 sec.<br>" +
                        "Enemies standing on the distortions suffer 14-22 + (INT/15) energy damage per second."));
                    break;

                case AscensionType.Palemaster:
                    list.Add(new AscensionAbilityEntry("Deathless Vigor", 2, true, null, 0x1CF0, 0x455,
                        "Casting offensive necromancy spells has a 1.5%/level chance to heal 2 + (level/2) health.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 5:</BASEFONT> Chance to gain +10% spell damage for 15 seconds.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 13:</BASEFONT> Bonus increases to +15% damage for 25 seconds."));

                    list.Add(new AscensionAbilityEntry("Undead Graft", 8, true, null, 0x1B10, 0x455,
                        "While wearing a full suit of bone armor, necromancy spells have a chance to heal 5% of all allied undead's health.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 17:</BASEFONT> Grants continuous regeneration of 2% max health per second."));

                    list.Add(new AscensionAbilityEntry("Creeping Cold", 14, true, null, 0x221D, 0x455,
                        "Casting Wither has a 0.25%/level chance to activate it a second additional time.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 19:</BASEFONT> Chance to automatically fire Enervate without spending mana or cooldown."));

                    list.Add(new AscensionAbilityEntry("Herald of Hereafter", 20, true, null, 0x3735, 0x455,
                        "Necromancy spells have a 0.25%/level chance to generate a fetid distortion mist around the target.<br>" +
                        "Deals 14-22 + (INT/15) poison damage per second to those in the area."));
                    break;

                case AscensionType.Crusader:
                    list.Add(new AscensionAbilityEntry("Divine Grace", 2, true, null, 0x0F80, 0x481,
                        "Purity of spirit. 4%/level chance to automatically cure yourself of lesser, regular, and greater poisons.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 5:</BASEFONT> Cures deadly poisons.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 13:</BASEFONT> Cures lethal poisons."));

                    list.Add(new AscensionAbilityEntry("Holy Fervor", 8, true, null, 0x1F2B, 0x481,
                        "Killing a demon has a 1.5%/level chance to cause a holy detonation in an area.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 17:</BASEFONT> Activating the fervor immediately resets Smite's cooldown."));

                    list.Add(new AscensionAbilityEntry("Inquisitorial Strikes", 14, true, null, 0x143B, 0x481,
                        "Melee strikes deal +(level/2)% additional damage against demons.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 19:</BASEFONT> Deals +(level/4)% extra damage against all evil creatures."));

                    list.Add(new AscensionAbilityEntry("Divine Judgement", 20, true, null, 0x1579, 0x481,
                        "When killing a powerful evil creature, there is a 2.5% chance to summon a celestial Luminar to fight at your side for 1 minute."));
                    break;

                case AscensionType.Assassin:
                    list.Add(new AscensionAbilityEntry("Virulent Strikes", 2, true, null, 0x27AC, 0x44,
                        "When poisoning with a strike, 1%/level chance to immediately resolve 1 poison damage tick.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 5 & 13:</BASEFONT> Chance to resolve instant poison ticks with each subsequent attack."));

                    list.Add(new AscensionAbilityEntry("Dangerous Habits", 8, true, null, 0x1544, 0x44,
                        "Increases the damage of all poison ticks you inflict by +9%.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 17:</BASEFONT> The tick damage bonus increases to +18%."));

                    list.Add(new AscensionAbilityEntry("Deadly Strikes", 14, true, null, 0x1401, 0x44,
                        "Deals +9% damage with attacks against poisoned targets.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 19:</BASEFONT> Your attacks ignore 25% of the enemy's poison resistance."));

                    list.Add(new AscensionAbilityEntry("Terminal", 20, true, null, 0x1848, 0x44,
                        "When killing a poisoned creature, 0.25%/level chance to spread lethal poison to all enemies within a 2-tile radius."));
                    break;

                case AscensionType.Blackguard:
                    list.Add(new AscensionAbilityEntry("Frozen Heart", 2, true, null, 0x221D, 0x48D,
                        "Increases the cold damage of all your weapon strikes by +6%.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 5:</BASEFONT> Increases to +12%.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 13:</BASEFONT> Increases to +18%."));

                    list.Add(new AscensionAbilityEntry("Morbidity", 8, true, null, 0x141B, 0x497,
                        "When killing a positive karma creature, 0.25%/level chance to terrify nearby enemies, causing them to flee in panic.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 17:</BASEFONT> The fear flee duration increases to 6 seconds."));

                    list.Add(new AscensionAbilityEntry("Merciless Strikes", 14, true, null, 0x1441, 0x497,
                        "Weapon strikes have a 1%/level chance to target the target's lowest defensive resistance.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 19:</BASEFONT> Chance to paralyze the target for 3 seconds."));

                    list.Add(new AscensionAbilityEntry("Soul Reaper", 20, true, null, 0x1EA7, 0x497,
                        "When killing a hostile enemy, 0.25%/level chance to drain level + STR/25 health from all creatures within 2 tiles."));
                    break;

                case AscensionType.Skald:
                    list.Add(new AscensionAbilityEntry("Battlefield Rhythm", 2, true, null, 0x0E9D, 0x53,
                        "Gain +3% weapon damage against creatures that are discorded, provoked, or pacified.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 5:</BASEFONT> Bonus increases to +9%.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 13:</BASEFONT> Bonus increases to +18%.", 1, 0));

                    list.Add(new AscensionAbilityEntry("Cutting Words", 8, true, null, 0x0EBE, 0x53,
                        "Your discord attempts suppress an additional +4% of the target's attributes and skills.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 17:</BASEFONT> Suppresses an additional +8%."));

                    list.Add(new AscensionAbilityEntry("Resonance", 14, true, null, 0x1C12, 0x53,
                        "Your musical prowess is considered 15% more potent for all song spells.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 19:</BASEFONT> Increases to 25% more potent."));

                    list.Add(new AscensionAbilityEntry("Saga of Steel", 20, true, null, 0x13B9, 0x53,
                        "When killing an enemy with a weapon, 0.25%/level chance to instantly activate the Foe Requiem spell on the nearest enemy."));
                    break;

                case AscensionType.Reaver:
                    list.Add(new AscensionAbilityEntry("Blood Scent", 2, true, null, 0x0F7E, 0x20,
                        "Voracious blood scent. Your attacks against bleeding creatures deal increased damage."));

                    list.Add(new AscensionAbilityEntry("Hemorrhage", 8, true, null, 0x0E21, 0x20,
                        "Grievous wounds. The bleed damage over time applied to your targets becomes more persistent and cruel."));

                    list.Add(new AscensionAbilityEntry("Mutilate", 14, true, null, 0x0EC4, 0x20,
                        "Mutilating strikes against wounded creatures have a chance to ignore part of their armor and reduce their stamina."));

                    list.Add(new AscensionAbilityEntry("Deep Cuts", 20, true, null, 0x0EC2, 0x20,
                        "When killing a hostile enemy, 0.25%/level chance to apply heavy bleeding and cripple all adjacent creatures for up to 21 sec."));
                    break;

                case AscensionType.Kensai:
                    list.Add(new AscensionAbilityEntry("Practiced Perfection", 2, true, null, 0x13FE, 0,
                        "Mastery of the cut. Deals +6% increased damage with all swords.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 5:</BASEFONT> +12% damage.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 13:</BASEFONT> +18% damage."));

                    list.Add(new AscensionAbilityEntry("Singular Focus", 8, true, null, 0x27A8, 0,
                        "When killing a full-health opponent using a sword, gain +9% extra sword damage for 30 sec.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 17:</BASEFONT> The bonus increases to +18%."));

                    list.Add(new AscensionAbilityEntry("Iaijutsu", 14, true, null, 0x27AB, 0,
                        "Sword strikes have a 1%/level chance to hit the target's lowest resistance.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 19:</BASEFONT> 0.25%/level chance to deliver an immediate second cut."));

                    list.Add(new AscensionAbilityEntry("Final Cut", 20, true, null, 0x13FF, 0,
                        "When killing a full-health opponent, 0.25%/level chance to unleash a Tempest without spending stamina."));
                    break;

                case AscensionType.Hierophant:
                    list.Add(new AscensionAbilityEntry("Blessed Might", 2, true, null, 0x143B, 0x482,
                        "Sacred blessing. Deals +5% more damage with bludgeoning weapons.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 5:</BASEFONT> +10% damage.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 13:</BASEFONT> +15% damage."));

                    list.Add(new AscensionAbilityEntry("Divine Resilience", 8, true, null, 0x1B76, 0x482,
                        "Unshakable divine protection. Takes 4% less damage from all attack sources.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 17:</BASEFONT> Reduction increased to 8% damage from all sources."));

                    list.Add(new AscensionAbilityEntry("Death Ward", 14, true, null, 0x2C7F, 0x482,
                        "Protection from death. When suffering lethal damage, 1%/level chance to ignore the fatal damage.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 19:</BASEFONT> When saved by the protection, heals 33% of your hit points."));

                    list.Add(new AscensionAbilityEntry("Divine Absolution", 20, true, null, 0x0E28, 0x482,
                        "When taking damage from an evil creature, 0.25%/level chance to activate Consecrated Ground for free without spending mana."));
                    break;

                case AscensionType.ArcaneArcher:
                    list.Add(new AscensionAbilityEntry("Arcane Precision", 2, true, null, 0x0F3F, 0x489,
                        "Enchanted aim. Deals +6% increased damage with any ranged attack.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 5:</BASEFONT> +12% damage.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 13:</BASEFONT> +18% damage."));

                    list.Add(new AscensionAbilityEntry("Mystical Ricochet", 8, true, null, 0x1BFB, 0x489,
                        "Bouncing projectiles. 0.25%/level chance for the projectile to ricochet and hit the nearest enemy.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 17:</BASEFONT> Can ricochet to up to two additional enemies."));

                    list.Add(new AscensionAbilityEntry("Arcane Feedback", 14, true, null, 0x1F52, 0x489,
                        "When slaying an opponent at range, recover Inscription/25 mana.<br>" +
                        "<BASEFONT COLOR=#82E0AA>Lvl 19:</BASEFONT> Also recovers Focus/25 stamina or health."));

                    list.Add(new AscensionAbilityEntry("Arcane Momentum", 20, true, null, 0x2FB7, 0x489,
                        "When slaying an opponent with a ranged attack, gain Inscription/8 spell absorption for 30 seconds."));
                    break;

                default:
                    list.Add(new AscensionAbilityEntry("Spiritual Blessing", 2, true, null, 0x0EFA, 0,
                        "Grants spiritual focus and gradual passive bonuses throughout the evolution of the ascension."));
                    break;
            }

            return list;
        }
    }
}