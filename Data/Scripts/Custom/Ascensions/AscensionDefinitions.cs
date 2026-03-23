using System;

namespace Server.Custom.Ascensions
{
    public static class AscensionDefinitions
    {
        public static string GetDescription(AscensionType type)
        {
            switch (type)
            {
                case AscensionType.Berserker:
                    return
                    "The Berserker is a savage warrior that wields two-handed weapons to great effect. In combat, they fly into a murderous rage that empowers them.<br>" +
                    "In order to activate this Ascension, you need to have 95 base skill in Tactics, camping and Magic Resist. Every time you level up the class, the requirement also increases by 1.<br>" +
                    "So a level 20 Berserker cannot activate this ascension unless they have 115 base skill in Tactics, camping and Magic Resist.<br>"+
                    "Berserkers also require their spirit to be free of the taint of civilization - a Berserker will not gain experience in this Ascension if they have learned Bushido, Knightship, Magery or Necromancy";
                case AscensionType.Archmage:
                    return
                    "The Archmage is a master of the Arcane. In combat, they control their opponents and execute powerful spells to bend the Weave itself to their will itself.<br>" +
                    "In order to activate this Ascension, you need to have 95 base skill in Magery, inscription and Psychology. Every time you level up the class, the requirement also increases by 1.<br>" +
                    "So a level 20 Archmage cannot activate this ascension unless they have 115 base skill in Magery , inscription and Psychology<br>"+
                    "Archmages also require a singular focus on arcane magic, and they will not gain experience in this class if they have learned Knightship, Elementalism, Necromancy or Bushido";
                case AscensionType.Palemaster:
                    return
                    "The Palemaster is death made flesh. In combat, they control an army of undead and curse and enfeeble their opponents until they have met their end.<br>" +
                    "In order to activate this Ascension, you need to have 95 base skill in Necromancy, forensics and Spiritualism. Every time you level up the class, the requirement also increases by 1.<br>" +
                    "So a level 20 Palemaster cannot activate this ascension unless they have 115 base skill in Necromancy, forensics and Spiritualism<br>"+
                    "Palemasters also are required to revel in death and vileness, and they will not gain experience in this class if they have learned Knightship, Elementalism or Bushido, or if they stray from the path of evil";
                case AscensionType.Crusader:
                    return
                    "The Crusader is the embodiment of virtue. In combat, they stand tall in defiance against evil.<br><br>"+
                    "In order to activate this Ascension, you need to have 95 base skill in Knightship, spiritualism and Tactics. Every time you level up the class, the requirement also increases by 1.<br>"+
                    "So a level 20 Crusader cannot activate this ascension unless they have 115 base skill in Knightship, spiritualism and Tactics.<br>"+
                    "Crusaders also are required to follow the path of justice and uphold good, and they will not gain experience in this class if they have learned Bushido, Necromancy or Forensics, or if they stray from the path of good.<br>";
                case AscensionType.Assassin:
                    return
                    "The Assassin is an expert in ending lifes prematurely. In combat, they employ poisons to great effect to hinder and annihilate their foes.<br>"+
                    "In order to activate this Ascension, you need to have 95 base skill in both Poisoning, hiding and Fencing. Every time you level up the class, the requirement also increases by 1.<br>"+
                    "So a level 20 Crusader cannot activate this ascension unless they have 115 base skill in both Poisoning, hiding and Fencing.<br>"+
                    "Assassins also are required to abandon their morals, and they will not gain experience in this class if they have learned Bushido or knightship, or if they abandon the path of evil.";   
                case AscensionType.Blackguard:
                    return
                    "The Blackguard is a champion of evil and corruption. In combat, they are juggernauts that bring frost e blood into the battlefield.<br>"+
                    "In order to activate this Ascension, you need to have 95 base skill in Knightship, Arms lore and Necromancy. Every time you level up the class, the requirement also increases by 1.br>"+
                    "So a level 20 Crusader cannot activate this ascension unless they have 115 base skill in Knightship, Arms lore and Necromancy.<br>"+
                    "Blackguardss also are required to abandon their morals, and they will not gain experience in this class if they have learned Bushido, spiritualism or elementalism, or if they abandon the path of evil.";
                case AscensionType.Skald:
                    return 
                    "The Skald is the hero of song and the keeper of valor. In combat, they use powerful music to strengthen themselves and their allies.<br>"+
                    "In order to activate this Ascension, you need to have 95 base skill in Musicianship, Tactics and Discordance. Every time you level up the class, the requirement also increases by 1.<br>"+
                    "So a level 20 Skald cannot activate this ascension unless they have 115 base skill in Musicianship, Tactics and Discordance.<br>"+
                    "Skalds are proud and pragmatic. They will not gain experience in this class if they have learned bushido, necromancy, marksmanship, hiding or begging.";
                case AscensionType.Reaver:
                    return 
                    " The Reaver is a vicious and savage combatant. In combat, they specialize in making their opponents bleed and sap their will to live with powerful and crippling strikes.<br>"+
                    "In order to activate this Ascension, you need to have 95 base skill in Tactics, Anatomy and Forensics. Every time you level up the class, the requirement also increases by 1.<br>"+
                    "So a level 20 Reaver cannot activate this ascension unless they have 115 base skill in Tactics, Anatomy and Forensics.<br>"+
                    "Reavers will not gain experience in this class if they have learned Bushido, Healing, Veterinary or Spiritualism.";
                case AscensionType.Kensai:
                    return
                    "The Kensai is master of the blade. In combat, they specialize in powerful strikes and pure mastery of the bushido.<br>"+
                    "In order to activate this Ascension, you need to have 95 base skill in Bushido, Arms Lore and Swordsmanship. Every time you level up the class, the requirement also increases by 1.<br>"+
                    "So a level 20 Kensai cannot activate this ascension unless they have 115 base skill in Bushido, Arms Lore and Swordsmanship.<br>"+
                    "Kensai will not gain experience in this class if they have learned Knightship, Magery, Necromancy or Ninjitsu.";
                case AscensionType.Hierophant:
                    return 
                    "The Hierophant is master of the divine. In combat, they specialize in conjuring aid from their deity and channeling divine power to aid their allies.<br>"+
                    "In order to activate this Ascension, you need to have 95 base skill in Healing, Spiritualism and Meditation. Every time you level up the class, the requirement also increases by 1.<br>"+
                    "So a level 20 Hierophant cannot activate this ascension unless they have 115 base skill in Healing, Spiritualism and Meditation.<br>"+
                    "Hierophants will not gain experience in this class if they have learned Knightship, Forensics, Necromancy, Bushido or Ninjitsu.";
                case AscensionType.ArcaneArcher:
                    return 
                    "The Arcane Archer is master marksman and conjurer. In combat, they specialize in raining magical projectiles at their opponents.<br>"+
                    "In order to activate this Ascension, you need to have 95 base skill in Magery, Focus, inscription and Marksmanship. Every time you level up the class, the requirement also increases by 1.<br>"+
                    "So a level 20 Arcane Archer cannot activate this ascension unless they have 115 base skill in Magery, Focus, inscription and Marksmanship.<br>"+
                    "Arcane archers will not gain experience in this class if they have learned Knightship, Necromancy, elementalism or Bushido.";
                default:
                    return "IN DEVELOPMENT.";
            }
        }


        public static string GetAbilities(AscensionType type)
        {
            if(type == AscensionType.Berserker)
            {
                return
                "<BASEFONT COLOR=#FFFFFF>" +
                "These are the abilities that the berserker learns as they level up:<br><br>" + 

                "Rage, Level 1<br>" +
                "Command: [BerserkerRage<br>" +
                "Gain +(15+level) STR, immunity to paralyze, +10% damage, -10% defend chance.<br>" +
                "Lasts 10 + level seconds. 1 min cooldown. Ends with 50% stamina loss.<br>" +
                "Lvl 10: +level/2 resists, +5% damage, -5% defend chance.<br>" +
                "Lvl 15: Hits above 40% max HP are reduced to 40%.<br>" +
                "Lvl 20: Additional +5% damage, -5% defend chance.<br><br>" +   

                "Leap Slam, Level 6<br>" +
                "Command: [BerserkerLeapSlam<br>" +
                "Leap 3 + level/4 tiles. On landing deal 20–35 + ((STR/15)+(level/3)) damage to adjacent enemies.<br>" +
                "Cooldown: 9 sec - (1 sec per 3 levels).<br>" +
                "Lvl 12: Stuns adjacent enemies for 3 sec.<br><br>" +   

                "Warcry, Level 11<br>" +
                "Command: [BerserkerWarCry<br>" +
                "Enemies lose (25+level) STR, (10+level) DEX, (10+level) INT for (20+level) sec.<br>" +
                "Allies gain 2 stamina every 2 sec for same duration.<br>" +
                "Lvl 16: Enemies lose 20+2*level stamina instantly and 2+level every 2 sec.<br><br>" +  

                "Tenacity,Level 18<br>" +
                "Command: [BerserkerTenacity<br>" +
                "Heal to full HP immediately and again every 10 sec for 30 sec. 2 min cooldown.<br>" +
                "Lvl 20: Also heals full stamina each pulse.<br><br>" + 

                "Passives:<br>" +
                "Cleave, level 2<br>" +
                "When making a melee strike with a two handed weapon, the berserker has a 4% + 1% level chance of making another strike against an adjacent enemy<br>" +
                "At level 5, they have a 1%+1%/level chance of a second aditional strike. At level 13, they have a 1%+1%/level chance of a third aditional strike<br><br>" +    

                "Uncanny dodge, level 8<br>" +
                "The berserker has a 25+1%/level chance of ignoring the effects of traps<br>"+
                "Level 17: the chance increases to 45+1%/level<br><br>"+

                "Pummeling Strikes, level 14<br>" +
                "When making a strike with a two handed melee weapon, the berserker has a (level/4)% chance of ignoring the target's armor<br>"+
                "Level 19: the chance increases to (levle/2)%<br><br>"+

                "Undying Wrath, level 20<br>" +
                "If the berserker takes a hit that would reduce their hit points to 0 or less, they reduce it to 1 instead and the cooldown on their Warcry is set to zero.<br>"+
                "This effect can trigger once per minute<br>"+
                "</BASEFONT>";                
            }
            else if ( type == AscensionType.Archmage)
            {
                return 
                "<BASEFONT COLOR=#FFFFFF>" +
                "These are the abilities that the Archmage learns as they level up:<br><br>" + 

                "Arcane storm, level 1<br>" +
                "command: [ArchmageArcaneStorm <br>" +
                "The archmage unleashes (6 + 1 per 3 archmage levels) energy missiles at the target, each missile causes 10-18 + (int/25 + level/3) damage.<br>" + 
                "Casting this spell costs 45 mana, and once used, it cannot be used again for (45-1/level) seconds.<br>" +
                "level 10; a target hit by an arcane storm has a 3% chance per level of the archmage to receive a -(8 + level/4) energy resistance debuff.<br>" + 
                "that lasts for 12 + archmage level /2 seconds.<br>" + 
                "level 15: a target hit by an arcane storm loses 34 + level/2 stamina after being hit by a missile storm.<br>" + 
                "level 20: if a target is killed by an arcane storm, the cooldown on Conflux is set to zero.<br><br>" +  

                "Conflux, level 6<br>" + 
                "command: [ArchmageConflux <br>" + 
                "when activated, the archmage's spell damage is increased by 10% for (10 +1/level) seconds, 2 minutes cooldown.<br>" + 
                "level 12: the spell damage is increased by 15%.<br><br>" +

                "Mana Singularity, level 11.<br>" +
                "command: [ArchmageMassSingularity <br>" +
                "The Archmage creates a collapsing arcane vortex at a target location within 2 + (level/3) tiles. After 2.5 seconds, the singularity<br>" + 
                "explodes, causing 25-40 + (int/10 + level/2) energy damage to all hostile targets up to 2 tiles away from the source point.<br>" + 
                "Targets caught in the explosion also lose 20+level mana. The archmage recovers 4 mana per enemy hit. Activating this ability costs 50 mana.<br>" +
                "This ability has a cooldown of 60 seconds.<br>" + 
                "Level 16: all enemies hit by this ability are paralyzed for 4 seconds, and lose 20+level stamina.<br><br>" +

                "Timestop, level 18<br>" +
                "command:[ArchmageTimestop <br>" +
                "all hostile creatures up to (3+(level/5)) tiles away from the caster are paralyzed for (12+level/4) seconds.<br>" +
                "This ability costs 60 mana and has a 3 minutes cooldown.<br>" +
                "Level 20: once the paralyze ends, all enemies hit by this ability lose 40+level mana and stamina.<br><br>" +

                "mana vault, level 2<br>" +
                "when an enemy would leech or drain mana from the archmage, there's a 2% chance per level that the mana in immediately refunded to the  archmage<br>" +
                "Level 5: when an enemy would leech or drain mana from the archmage, they receive 15-25 + (level/2) energy damage<br>"+
                "Level 13: when an enemy would leech or drain mana from the archmage, they are paralyzed for 2 seconds<br><br>"+

                "weave reflection, level 8<br>"+
                "- the archmage receives a 2.5%/level chance of reflecting spells<br>"+
                "Level 17: when the archmage reflects a spell, they have a 1% chance / level of setting the cooldown on Conflux to zero<br>"+

                "arcane tempest, level 14<br>"+
                "when casting a spell at a hostile target, there's a (0.25 * level)% chance of triggering a Mana singularity at the target that costs<br>"+
                " no mana and doesn't have a cooldown.<br>"+
                "-Level 19: when arcane tempest triggers, there's a 1% chance/level that the cooldown on arcane storm is set to zero.<br><br>"+

                "Weave Unraveling, level 20<br>"+
                "when casting a harmful magery spell at a target, there's a 0.25% chance per level of creating weave disruptions around it.<br>"+ 
                "The amount of disruptions varies between 6-12, and they last from 12 to 22 seconds.<br>"+ 
                "Anyone standing on top of these disruptions receives 14-22 + (int/15) energy damage per second.<br>"+ 
                "</BASEFONT>";  
            }
            else if ( type == AscensionType.Palemaster)
            {
                return
                "<BASEFONT COLOR=#FFFFFF>" +
                "These are the abilities that the Palemaster learns as they level up:<br><br>" + 
                "Undying hordes, level 1<br>" + 
                "command: [PalemasterUndyingHordes<br>" + 
                "The Palemaster calls forth a mindless horde of undead to attack their enemies.<br>" + 
                "These will attack anything hostile to the caster relentlessly until they are destroyed or until they crumble into dust. <br>" + 
                "The Undying hordes has a 3 minutes cooldown between uses.<br>" +    
                "The horde lasts for 60 (+3 per level) seconds and its composition is based on the Palemaster Level.<br>" +  
                "Level 1-4: 3-5 skeletons, 2-3 skeleton warriors<br>" + 
                "Level 5-8: 3-5 skeletons, 2-3 skeleton warriors, 1-2 skeleton knights<br>" + 
                "Level 9-12: 3-4 skeleton warriors, 2-3 skeleton knights, 1-2 mummies<br>" + 
                "Level 13-16: 2-3 skeleton knights, 2-3 mumies, 1-2 ancient mummies, 1 undead giant<br>" + 
                "Level 17-19: 3-4 undead giants, 2-3 ancient mummies, 1 skeletal dragon<br>" + 
                "Level 20: 4-5 undead giants, 3-4 ancient mummies, 1-2 skeletal dragons<br><br>" +          

                "Enervate, level 6<br>" + 
                "command: [PalemasterEnervate<br>" + 
                "The palemaster syphons life from their enemies, feeding himself and enfeebling them.<br>" +  
                "When cast, this spell causes all hostile living creatures in up to 3 + (1 per 5 levels) tiles receive 40 + (level /3 ) cold damage and lose the same amount in mana.<br>" +  
                "For every creature hit, the palemaster recovers 5 hit points and 5 mana, to a maximum of 50 of each.<br>" + 
                "Creatures hit also receive -(5 + palemaster level / 4)% on their strength and dexterity for 30 seconds.<br>" +
                "This ability costs 30 mana to activate and has a one minute cooldown.<br>" + 
                "Level 12: the maximum amount recovered is capped at 100 for hit points and mana.<br><br>" +         

                "Circle Of Death, level 11.<br>" +
                "command: [PalemasterCircleOfDeath.<br>" +
                "The palemaster corrupts the weave to erode life from their enemies.<br>" +
                "This ability creates a circle arount the target creature, and all living creatures up to 2 (+1 per 5 levels) of it receive 12-18 + (level /2 ) + (int / 20) cold damage per second for an amount of seconds equal to 3 + level / 4.<br>" +
                "This ability has a 2 minutes cooldown and costs 60 mana to activate.<br>" +
                "Level 16: creatures caught in the circle lose stamina every second equal to half the damage dealt.<br><br>" +            

                "Danse Macabre, level 18.<br>" +
                "command: [PalemasterDanseMacabre<br>"+
                "The Palemaster shatters the gate between words, bringing the ever hungry hordes to the land of the living.<br>" + 
                "This ability has a 4 minutes cooldown and costs 80 mana to activate.<br>" +
                "Every second for level/3 seconds, one of those happens:.<br>" +
                "    - 1-2 undead giants are spawned and attack the enemies of the pale master, they last for 30 seconds.<br>" + 
                "    - 2-3 mummy lords are spawned and attack the enemies of the pale master, they last for 30 seconds.<br>" +
                "    - every undead under the palemaster's control that is up to 5 tiles away from the palemaster heals an amount of hit points equal to 25% of their maximum hit points.<br>" +
                "    - the palemaster heals to full health.<br>" +
                
                "at level 20, these effects are added to the possible effects that happen every second:.<br>" +
                "    - every hostile living creature that is up to 3 tiles away from the palemaster and that is at less than 50% of their maximum hit points loses 10% of their hit points.<br>" +
                "    - all living creatures up to 4 tiles away from the pale master receive a -20% debuff to their strenght that lasts for 30 seconds.<br>" +
                "    - 1 skeletal dragon spawns and attack the enemies of the pale master, it lasts for 30 seconds.<br><br>" +

                "deathless vigor, level 2.<br>" +
                "When casting an offensive necromancy spell, 1.5% per level chance of recovering 2 + (level / 2) hit points.<br>" + 
                "Level 5: 0.25% per level chance of gaining +10% spell damage for 15 seconds.<br>" +
                "Level 13: the spell damage bonus increases to +15%, the duration increases to 25 and the chance increases to 0.45% per level.<br><br>" +

                "undead graft, level 8.<br>" +
                "When casting an offensive necromancy spell while wearing a full set of bone armor, there's a 0.25% per level chance all friendly summoned undead creatures will heal 5% of their hit points.<br>" +
                "Level 17: when undead graft triggers, friendly summoned undead cretures recover 2% of their maximum health per second for level/2 seconds.<br><br>" + 
            
                "creeping cold, level 14.<br>" +
                "When casting wither, there's a 0.25% chance per level that it will trigger an additional time.<br>" +
                "Level 19: when the creeping cold effect triggers, there's a 0.25% chance per level that enervate will be cast automatically ignoring mana costs and cooldown.<br>" +
            
                "herald of hereafter, level 20.<br>" +
                "When casting a harmful necromancy spell at a target, there's a 0.25% chance per level of creating weave disruptions around it. The amount of disruptions varies between 6-12, and they last from 12 to 22 seconds.<br>" +
                "Anyone standing on top of these disruptions receives 14-22 + (int/15) poison damage per second.<br>"+ 

                "</BASEFONT>"; 
            }
            else if (type == AscensionType.Crusader)
            {
                return
                "<BASEFONT COLOR=#ffffff>"+
                "Smite, level 1.<br>" +
                "command: [CrusaderSmite<br>" +
                "The crusader channels its valor and calls forth an explosion of purifying flame.<br>" + 
                "This effect causes 20-32 +( level / 2 + str/15) damage to all evil creatures that are up to 2 (+1 per 5 levels) tiles away from the crusader.<br>" +
                "Smite costs 20 mana and 20 stamina to activate, and has a 1 minute cooldown.<br>" +
                "- Level 10: hostile creatures that have karma of -10k or lower are paralyzed for 4(+1 per 6 levels) seconds after being hit by the smite.<br>" +
                "- Level 15: friendly creatures with positive karma caught in the area of the smite are healed for 20 + level/2 hit points.<br>" +
                "- Level 20: hostile evil creatures that have karma of -10k or lower damaged by the smite are caught in searing flames. They receive 10-20 + str/15 fire damage every 2 seconds for 12 seconds.<br><br>" +

                "Charge, level 6.<br>" +
                "command: [CrusaderCharge<br>" +
                "When used, the character chooses a free tile that is at least 2 tiles away from them and its moved to that tile.<br>" +
                "The maximum range is equal to  3 + level/5. When arriving, every hostile creature adjacent to that tile has a 1% per level chance of being paralyzed for 3 seconds.<br>" +  
                "Has a cooldown of 9 seconds, minus one second per 3 crusader levels.<br>" +
                "- Level 12: when arriving, all hostile creatures adjacent to the destination square receive 30-45 + ((str / 15) + level / 3) physical damage.<br><br>" +

                "Aura of Hope, level 11.<br>" +
                "command: [CrusaderAuraOfHope<br>" +
                "When used, the crusader and all friendly creatures up to 4 tiles away from the crusader receive the following benefits:<br>" +
                "- they restore 12 + (crusader level / 2) hitpoints every 4 seconds per 30 seconds.<br>" +
                "- they restore 6 + (crusader level / 2) mana every 4 seconds per 30 seconds.<br>" +
                "- they restore 6 + (crusader level / 2) stamina every 4 seconds per 30 seconds.<br>" +
                "This skill has a 3 minutes cooldown, and costs 40 mana and 40 stamina to activate.<br>" +
                "- Level 16: creatures affected by the aura of hope are healed to full health once the buff expires.<br><br>" +

                "Heavenly Gate, level 18.<br>" +
                "command: [CrusaderHeavensGate<br>" +
                "The crusader opens the pearly gates and calls forth a celestial companion to aid them in their war against all evil.<br>" +
                "The creature is a powerful Archon that requires 2 control slots and remains at the crusader's site until defeated.<br>" +
                "Activating this ability costs 60 mana and 60 stamina. It has a 5 minutes cooldown.<br>" +
                "- Level 20: There's a 1% chance per level that two archons will be summoned instead.<br><br>" + 
            
                "Divine Grace, level 2.<br>" +
                "the crusader has a 4% chance per level chance of automatically healing from lesser, regular and greater poisons.<br>" + 
                "- Level 5:  the chance also applies to deadly poisons.<br>" +
                "- Level 13:  the chance also applies to lethal poisons.<br><br>" +

                "Holy Fervor, level 8.<br>" +
                "When the crusader kills a demon, they have a 1.5% per level chance of triggering an AoE explosion of holy fire, causing 20-32 + (level/2 + str/15) damage to all evil hostile creatures that are up to 2 (+1 per 5 levels) tiles away from the crusader.<br>" +
                "- Level 17: when Holy Fervor triggers, the Cooldown on Smite is set to zero.<br>" +
            
                "Inquisitorial Strikes, level 14.<br>" +
                "the crusader's melee weapon strikes cause + (level/2)% damage against demons.<br>" +
                "- Level 19: the crusader's melee weapon strikes cause + (level/4)% damage against evil creatures.<br>" +
            
                "Divine Judgement, level 20.<br>" +
                "When the crusader kills a powerful evil creature, there's a 2.5% chance that a strong creature called Luminar will be summoned to aid the Crusader on their war against evil.<br>" +
                " creature will attack any nearby opposing creature with great ferocity, and will remain in the material plane for one minute.<br>" +
                "</BASEFONT>";
            }
            else if (type == AscensionType.Assassin)
            {
                return
                "<BASEFONT COLOR=#ffffff>"+
                "Noxious Cloud, level 1.<br>" +
                "command: [AssassinNoxiousClou<br>" +
                "The assassin creates a cloud of foul vapors at target location that poisons all enemies caught in its area.<br>" +
                "The cloud inflicts greater poison on all affected enemies. This ability costs 30 mana and 30 stamina to activate.<br>" + 
                "This ability has a 1 minute cooldown. The are of the cloud is equal to 1 + 1 per 6 levels.<br>" +
                "- Level 10: The cloud inflics Deadly poison on all affected enemies.<br>" +
                "- Level 15: The cloud inflicts Lethal poison on all affected enemies.<br>" +
                "- Level 20: all affected enemies lose 20 poison resist for 30 seconds.<br><br>" +

                "Crippling poison, level 6.<br>" +
                "command: [AssassinCripplingPoison<br>" +
                "The assassin throws a vial of crippling poison at a target, inflicting deadly poison on it and preventing them from walking for level / 2 seconds.<br>" +
                "This ability costs 40 mana and 40 stamina to activate and has a 1 minute cooldown.<br>" +
                "- Level 12: The crippling poison is Lethal and affects all hostile creatures up to 2 tiles away from the original target.<br><br>" + 

                "Toxic surge, level 11.<br>" +
                "command: [AssassinToxicSurge<br>" +
                "For the next 30 seconds, the assassin causes + 10% damage bonus with weapon strikes against poisoned creatures.<br>" + 
                "This ability costs 40 mana and 40 stamina to activate and has a 2 minutes cooldown.<br>" +
                "- Level 16: When activated, there's a 1% chance per level of setting the cooldown on Noxious Cloud to zero.<br>" + 

                "Cleansing Annihilation, level 18.<br>" +
                "command: [AssassinCleansingAnnihilation<br>" +
                "The assassin inflicts lethal poison on the target, then immediately consumes all of its ticks.<br>" + 
                "If the target survives, all creatures around it to up to 3 tiles away are the inflicted with a lethal poison.<br>" + 
                "Activating this ability costs 50 mana and 50 stamina and it has a 4 minutes cooldown.<br>" +
                "- Level 20: when activating this ability, there's a 1% chance per level that the cooldown on toxic surge will be set to zero.<br><br>" +

                "Virulent Strikes, level 2.<br>" +
                "When applying a poison on a target with a strike, the assassin has a 1% per level chance of immediately resolving 1 poison tick.<br>" +
                "- Level 5: when making a weapon attack against a poisoned creature, the assassin has a 0.25% chance per level of immediately resolving a poison tick.<br>" +
                "- Level 13: when making a weapon attack against a poisoned creature, the assassin has a 0.12% chance per level of immediately resolving an additional poison tick.<br><br>" +

                "dangerous habits, level 8.<br>" +
                "increases tick damage of poisons inflicted by the assassin by 9%.<br>" +
                "- Level 17: increases tick damage of poisons inflicted by the assassin by 18%.<br><br>" +

                "Deadly Strikes, level 14.<br>" +
                "The assassin causes 9% more damage with attacks to poisoned creatures.<br>" +
                "- Level 19: attacks against poisoned creatures ignore 25% of the target's poison resistance.<br><br>" +

                "Terminal, level 20.<br>" +
                "When killing a poisoned creature, the assassin has a 0.25% chance per level of inflicting lethal poison on all hostile creatures that are up to 2 tiles away from it.<br>" +
                "</BASEFONT>";
            }
            else if (type == AscensionType.Blackguard)
            {
                return
                "<BASEFONT COLOR=#ffffff>"+
                "Dark Succor, level 1.<br>" +
                "command: [BlackguardDarkSuccor<br>" +
                "The blackguard enters an unholy trance for (30 +1 per level) seconds.<br>" +
                "Dark succor costs 20 mana to activate. While its active, the Blackguard has 15 + level/2 strength.<br>" + 
                "When Dark Succor ends, the blackguard loses half ot their remaining mana. This ability has a 90 seconds cooldown.<br>" +
                "- Level 10: While on the trance and under 50% of their maximum hitpoints, the blackguard causes + 0.75%  damage per level against positive karma creatures.<br>" +
                "- Level 15: While on the trance and under 50% of their maximum mana, whenever the blackguard kills a creature with a weapon strike, the blackguard recovers 2 + level/2 mana.<br>" + 
                "- Level 20: While on the trance amd imder 50% of their maximum hitpoints, whenever the blackguard kills a creature with a weapon strike, the blackguard heals for 6 + level/2 hit points.<br><br>" +

                "Death's Advance, level 6.<br>" +
                "command: [BlackguardDeathsAdvance<br>" +
                "When used, the blackguard chooses a free tile that is at least 2 tiles away from them and its moved to that tile. The maximum range is equal to  3 + level/5. When arriving, every hostile creature adjacent to that tile has a 1% per level chance of knockedback for 2 + 1 tile for every 5 levels of the blackguard.<br>" +  
                "Has a cooldown of 9 seconds, minus one second per 3 Blackguard levels.<br>" + 
                "- Level 12: when arriving, all hostile creatures adjacent to the destination square receive 30-45 + ((str / 15) + level / 3) physical damage<br><br>" +

                "Chains of Ice, level 11.<br>" +
                "command: [BlackguardChainsOfIce<br>" +
                "The blackguard throws icy chains around themselves, freezing all hostile creatures for 3 + 1  per 4 levels seconds. Creatures caught by the chains receive 30-43 + (str/15) cold damage.<br>" +
	            "costs 30 mana to activate and has a 90 seconds cooldown.<br>" +
                "- Level 16: When the paralyze ends, the chains explode causing an additional 30-43 + (str/15) cold damage at each creature adjacent to an affected creature.<br><br>" +

                "Frostwyrm's Fury, level 18.<br>" +
                "command: [BlackguardsFrostwyrmsFury<br>" +
                "The blackguard opens the netherworld gates and calls forth an undead dragon bound to his will.<br>" +
                "The creature is a powerful dracolich that requires 4 control slots and remains at the blackguard's side until defeated.<br>" + 
                "Activating this ability costs 60 mana and 60 stamina. It has a 5 minutes cooldown.<br>" +
                "- Level 20: there's a 1% chance per level that two frostwyrms will be summoned instead.<br><br>" + 

                "Frozen Heart, level 2.<br>" +
                "Increases the cold damage done by the blackguard's weapon strikes by 6%.<br>" +
                "- Level 5: Increases the cold damage done by the blackguard's weapon strikes by 12%.<br>" +
                "- Level 13:Increases the cold damage done by the blackguard's weapon strikes by 18%.<br><br>" +

                "Morbidity, level 8.<br>" +
                "When the blackguard kills a positive karma creature, they have a 0.25% chance per level of terrorizing most nearby foes that are up to 2 tiles away and making them flee for 3 seconds.<br>" + 
                "- Level 17: Terrorized enemies flee for 6 seconds instead.<br><br>" +

                "Merciless Strikes, level 14.<br>" +
                "The blackguard's weapon attacks have a 1% chance per level of affecting the target's weakest resistance.<br>" +
                "- Level 19: The blackguard's merciless strikes have a 0.25% chance per level of paralyzing the target for 3 seconds.<br><br>" +

                "Soul Reaper, level 20.<br>" +
                "When killing a hostile creature, the blackguard has a 0.25% chance per level of syphoning level + str /25 health from  all hostile creatures that are up to 2 tiles away from it.<br>" +
                "</BASEFONT>";
            }
            else if (type == AscensionType.Skald)
            {
                return
                "<BASEFONT COLOR=#ffffff>"+
                "War Chant, level 1<br>" +
                "command: [SkaldWarChant<br>" +
                "The Skald channels a powerful song that empowers their allies.<br>" +
                "For 10 (+1 per level) seconds, the skald and all friendly creatures up to 4 tiles away receive (5 + 1 per level) strength and dexterity,  and a (5 +1 per level) bonus to tactics and magic resistance.<br>" +
                "This skill costs 20 mana to activate and has a 90 seconds cooldown.<br>" +
                "- Level 10: The skald and their allies received a 0.5% per level damage bonus with weapons while the warchant is active.<br>" +
                "- Level 15: When the skald or their allies defeat an enemy while the warchant is active, they recover 5 health and 3 stamina.<br>" +
                "- Level 20: When the War Chant ends on the Skald, there's a 2% chace per level of it being cast again ignoring its cooldown and mana cost.<br><br>" +

                "Saga of Valor, level 6<br>" +
                "command: [SkaldSagaOfValor<br>" +
                "The skald and all of their allies up to 4 tiles away receive +(level/2)% hit chance bonus for 15 +1 per level seconds.<br>" + 
                "This skill costs 30 mana to activate and has a 90 seconds cooldown.<br>" +
                "- Level 12: The skald and all of their allies up to 4 tiles aawy receive a +(level/2)% defend chance increase for the duration of the sage.<br><br>" +

                "Song of Thunder, level 11<br>" +
                "command: [SkaldSongOfThunder<br>" +
                "The skald calls forth lightning to strike at their foes. For 15 + 1 per level seconds, lightningbolt hits a random nearby hostile target every 3 seconds causing 30-45 (+ skalds fame / 1000) energy damage, to a maximum range of 4 tiles.<br>" + 
                "This skill costs 40 mana to activate and has a 180 seconds cooldown.<br>" +
                "- Level 16: Every 3 seconds after the first lightning hit, an additional lightning strike happens at a different random nearby hostile target.<br><br>" + 

                "Dirge Of The Fallen, level 18<br>" +
                "command: [SkaldDirgeOfTheFallen<br>" +
                "The skald sings about the valor of ancient heroes and brings them to battle. When this song is played, the skald summons 3 + (level/4) ancient warriors of legend that will attack the skald's foes until they are defeated.<br>" +
                "The acient warriors last for a maximum of 90 seconds, this skill costs 50 mana to activate and has a 5 minutes cooldown.<br>" +
                "- Level 20: When using Dirge of The Fallen, there's a 1% chance per level of Song of Thunder automatically triggering.<br><br>" +

                "Battlefield Rhythm, level 2<br>" +
                "The skald gains a 3% damage bonus with weapons against creatures that are discorded, provoked or peaced.<br>" +
                "- Level 5: The skald gains a 9% damage bonus with weapons against creatures that are discorded, provoked or peaced  instead.<br>" +
                "- Level 13: The skald gains a 18% damage bonus with weapons against creatures that are discorded, provoked or peaced  instead.<br><br>" +

                "Cutting Words, level 8<br>" +
                "The skalds discordance attempts suppress an additional 4% of the target's attributes and skills.<br>" +
                "- Level 17: The skalds discordance attempts suppress an additional 8% of the target's attributes and skills.<br><br>" +

                "Ressonance, level 14.<br>" +
                "Your musical prowess is considered 15% higher for song spells.<br>" +
                "- Level 19: Your musical prowess is considered 25% higher for song spells instead.<br><br>" +

                "Saga of Steel, level 20<br>" +
                "When killing a hostile creature with a weapon, the Skald has a 0.25% chance per level of immediately triggering an instant Foe Requiem spell song on the nearest hostile creature.<br>"+
                "</BASEFONT>";
            }
            else if (type == AscensionType.Reaver)
            {
                return
                "<BASEFONT COLOR=#ffffff>"+
                "Gorge, level 1.<br>" +
                "command: [ReaverGorge<br>" +
                "The Reaver lays a curse around target area, spread thick vicious blood on it (radius 2 + 1 / 4 levels).<br>" +
                "Hostile creatures standing on the area receive -1.25 physical resistance per level of the reaver. The area lasts for 10 + 1/level seconds.<br>" + 
                "This ability costs 15 stamina and 15 mana to activate and it has a 3 minutes cooldown.<br>" +
                "- Level 10: Creatures caught inside the gorge's area are slowed and can't run.<br>" + 
                "- Level 15: Creatures caught inside the gorge's area receive -0.66 penalty per level of the reaver on all elemental resistances.<br>" + 
                "- Level 20: When a creature dies inside the gorge's area, there's a 1.5% chance per level of the reaver that it's corpse explodes, causing 40-66 + (reaver's str / 15 + reaver's tactics/12) physical damage on all hostile creatures that are up to 2 tiles away from it.<br><br>" + 

                "Exsanguinate, level 6.<br>" +
                "command: [ReaverExsanguinate <br>" +
                "The Reaver performs a whirlwind attack that makes all adjacent targets bleed. The attack causes 18-26 + str/15 physical damage, and the bleed causes 26-32 physical damage every 3 seconds for 13 + level seconds. <br>" +
                "This ability costs 20 mana and 20 stamina to activate, and has a 1 minute cooldown. <br>" +
                "- Level 12: the reaver recovers 9 health for each enemy affected by the Exsanguinate. <br><br>" +

                "Bloodstorm, level 11. <br>" +
                "command: [ReaverBloodstorm <br>" +
                "The reaver channels its scorn and calls forth an explosion of blood. This effect causes 20-32 + (level/2 + str/15) damage to all creatures that can bleed and are up to 3 (+1 per 5 levels) tiles away from the reaver. <br>" +
                "Exsanguinate costs 25 mana and 25 stamina to activate, and has a 2 minutes cooldown. <br>" +
                "- Level 16: When used, there's a 0.5% chance per level that the cooldown on Exsanguinate will be set to zero.<br><br>" +

                "Absolute Tyranny, level 18. <br>" +
                "command: [ReaverAbsoluteTyranny <br>" +
                "For the next 30 seconds, the Reaver causes 18% extra physical damage with axes, and when they kill an enemy with an axe strike, they recover 10% of their hit points and stamina. <br>" + 
                "Absolute tyranny costs 30 mana and 30 stamina to activate, and has a 3 minutes cooldown. <br>" +   
                "- Level 20: When a Reaver kills an enemy while under the effect of Absolute Tyranny, there's a 1% chance per level that its corpse explodes, causing 30-56 + (str/15 + tactics/12) physical damage to all hostile creatures that are up to 2 tiles away from it. <br>" +

                "Leech, level 2. <br>" +
                "When the reaver makes an axe strike against a foe, they have a 0.25% chance per level of draining 0.10%/level hit points from the target. <br>" +
                "- Level 5: When the reaver makes an axe strike against a foe, they have a 0.25% chance per level of draining 0.12%/level mana from the target. <br>" +
                "- Level 13: when the reaver makes an axe strike against a foe that has 25% or less hit points remaining, they have a 0.5%/level chance of draining an additional 1%/level hit points from the target. <br><br>" +

                "Ruthless, level 8. <br>" +
                "The reaver causes 9% extra  damage with axes.<br>" +
                "- Level 17: The reaver causes 18% extra  damage with axes instead.<br><br>" +
           
                "Flaying Strikes, level 14. <br>" +
                "The Reaver's weapon attacks with axes have a 1% chance per level of affecting the target's weakest resistance. <br>" +
                "- Level 19: The Reaver's merciless strikes have a 0.25% chance per level of setting the cooldown on Gorge to zero.<br><br>" +

                "Deep Cuts, level 20.<br>" +
                "When killing a hostile creature, the reaver has a 0.25% chance per level of applying a heavy bleed on all hostile creatures adjacent to it. <br>" + 
                "These creatures are crippled (can't run) and take 52-72 + str/10 damage every 3 seconds for 12 to 21 seconds. This effect does not stack. <br>" +
                "</BASEFONT>";
            }
            else if (type == AscensionType.Kensai )
            {
                return 
                "<BASEFONT COLOR=#ffffff>"+
                "Battle Meditation, level 1<br>" +
                "command: [KensaiBattleMeditation<br>" +
                "The Kensai enters a powerful battle trance. For the next 20 + 1/level seconds, the Kensai causes +(10 + level/2) % damage with swords and has +(5 + level/2)% increased swing speed.<br>" + 
                "This costs 20 stamina to activate and has a 2 min]utes cooldown.<br>" +
                "- Level 10: While under the effects of battle meditation, the Kensai has +(10+level/2) defense chance increase;<br>" +
                "- Level 15: When the Kensai hits an enemy while under the effects of battle meditation, there's an 1%/level chance that the cooldown on Culling Strike is set to zero.<br>" +
                "- Level 20: When the Kensai kills an enemy while under the effects of battle meditation, the cooldown on tempest is reduced by 5 seconds.<br><br>" +

                "Kai, level 6<br>" +
                "command: [KensaiKai<br>" +
                "When used, the character chooses a free tile that is at least 2 tiles away from them and its moved to that tile. The maximum range is equal to  3 + level/4. When landing, the character causes 20-35 + ( (Dex / 15) + level / 3 ) damage on all targets adjacent to that tile. Has a cooldown of 9 seconds, minus one second per 3 Kensai levels.<br>" + 
                "This ability costs 30 stamina to activate.<br>" +
                "- Level 12, when landing, the kensai has a 1%/level chance of performing a culling strike on all adjacent targets if the kensai is wearing a sword. <br><br>" +

                "Culling Strike, level 11<br>" +
                "command: [KensaiCullingStrike<br>" +
                "For the next 30 seconds, whenver the Kensai makes an attack with a sword against a target that has less than 10% of their total hit points, the Kensai has a 2%/level chance of causing + 80% damage with that strike.<br>" +
                "This ability costs 50 stamina to activate and has a 3 minutes cooldown.<br>" +
                "- Level 16: Culling strike's bonus damage can trigger on opponents that have less than 15% of their total hit points.<br><br>" +

                "Tempest, level 18<br>" +
                "command: [KensaiTempest<br>" +
                "The Kensai becames a storm of swords. When this ability is activated, the Kensai will attack against every enemy up to 6 tiles away, causing 70-85 + dex/15 physical damage. <br>" +
                "This ability costs 75 Stamina to activate and has a 1 minute cooldown.<br>" +
                "- Level 20: When  Tempest kills an enemy, there's an 0.5%/level chance that it will trigger again immediately without requiring the stamina cost.<br><br>" + 

                "Practiced Perfection, level 2<br>" +
                "The Kensai causes 6% increased damage with swords.<br>" + 
                "- Level 5: The Kensai causes 12% increased damage with swords.<br>" + 
                "- Level 13: The Kensai causes 18% increased damage with swords.<br><br>" + 

                "Singular Focus, level 8<br>" +
                "When the Kensai kills an enemy that was at full health with a sword attack, they gain 9% extra damage with swords for 30 seconds. This effect does not stack.<br>" +
                "- Level 17: When the Kensai kills an enemy that was at full health with a sword attack, they gain 18% extra damage with swords for 30 seconds. This effect does not stack.<br><br>" +

                "Iaijutsu, level 14<br>" +
                "The Kensai weapon attacks with swords have a 1% chance per level of affecting the target's weakest resistance.<br>" +
                "- Level 19: The Kensai's Iaijutsu attacks have a 0.25% chance per level of triggering an additional time.<br><br>" +

                "Final Cut, level 20<br>" +
                "When the Kensai kills a full health enemy with a sword attack, they have a 0.25% chance per level of triggering a Tempest that does not cost stamina.<br>" +
                "</BASEFONT>";
            }
            else if (type == AscensionType.Hierophant)
            {
                return
                "<BASEFONT COLOR='#FFFFFF'>"+
                "Divine Wrath, level 1<br>" +
                "command: [HierophantDivineWrath<br>" +
                "The Hierophant calls forth divine vengeance. The Hierophant selects a target location, and divine fire falls upon it, causing 30-48 + (karma/1000) fire damage on all targets up to 2 tiles away from that location.<br>" +
                "Creatures with negative karma receive + 15% damage from this ability.<br>" +
                "This ability costs 35 mana, and it has a 1 minute cooldown.<br>" +
                "- Level 10: Creatures with negative Karma receive + 25% damage from this ability.<br>" +
                "- Level 15: Creatures with negative Karma hit by this ability are paralyzed for 4 seconds and receive 15-24 fire damage every 2 seconds for 12 + level/3 seconds.<br>" +
                "- Level 20: Whis this ability is cast, there's a 20% chance that it will be cast again immediately.<br><br>" + 
            
                "Exalted Presence, level 6<br>" +
                "command: [HierophantExaltedPresence<br>" +
                "The Hierophant calls forth the power of their god to humble and awe all enemies. They are forced to move level/3 tiles away from the Hierophant and stop attacking the hierophant immediately.<br>" + 
                "This ability costs 45 mana to activate, and it has a 2 minutes cooldown.<br>" +
                "- Level 12: Evil creatures affected by Exalted Presence are paralyzed for 8 seconds.<br><br>" + 
                
                "Consecrated Ground, level 11<br>" +
                "command: [HierophantConsecratedGround<br>" +
                "The Hierophant calls forth holy light to purify up to 4 tiles around it. Positive Karma creatures standing on it recover 12 + level/3 hit points and 5 + level/3 mana every 2 seconds for level/3 seconds.<br>" +
                "Negative karma creatures standing on the consecrated ground receive 22-32 + level/3 fire damage every 2 seconds for level/3 seconds.<br>" +
                "This ability costs 55 mana to activate, and it has a 3 minutes cooldown.<br>" +
                "- Level 16: When the Hierophant casts Heal, Greater Heal or touch of life, the target recovers 25% extra hit points.<br><br>" +
            
                "Divine Power, level 18<br>" +
                "command: [HierophantDivinePower<br>" +
                "The Hierophant calls the power of their god to strengthen then. They receive +20 str and dex, +15 tactics and spiritualism, regenerate 8 hit points per second and cause + 20% damage with bashing weapons.<br>" + 
                "This spell costs 55 mana to activate and it has a 3 minutes cooldown. It lasts for 30+level seconds.<br>" +
                "- Level 20: When the Hierophant defeats an enemy with negative karma in combat, there's a 20% chance that the cooldown of Divine Wrath will be set to zero.<br><br>" + 
                
                "Blessed Might, level 2<br>" +
                "The Hierophant causes 5% more damage with bashing weapons.<br>" +
                "- Level 5: The Hierophant causes 10% more damage with bashing weapons.<br>" +
                "- Level 13: The Hierophant causes 15% more damage with bashing weapons.<br><br>" +
            
                "Divine Resilience, level 8<br>" +
                "The Hierophant receives 4% less damage from all sources.<br>" + 
                "- Level 17: The Hierophant receives 8% less damage from all sources.<br><br>" + 
            
                "Death Ward, level 14<br>" +
                "When the Hierophant receives damage that would kill them, they have a 1% chance per level of ignoring that damage.<br>" + 
                "- Level 19: When death Ward triggers, the Hierophant recovers 33% of their hit points.<br><br>" + 
            
                "Divine Absolution, level 20<br>" +
                "When the Hierophant receives damage from an evil creature, they have a 0.25%/level chance of triggering consecrated ground without paying its mana cost." + 
                "</BASEFONT>";
            }
            else if (type == AscensionType.ArcaneArcher)
            {
                return
                "<BASEFONT COLOR='#FFFFFF'>"+
                "Imbue Arrow, level 1<br>"+
                "command: [ArcaneArcherImbueArrows<br>"+
                "For the next 30 (+1/level) seconds, the arcane archer ranged attacks have a 2%/level chance of affecting the target's worst resistance.<br>"+ 
                "This ability costs 40 mana to activate and has a 2 minutes cooldown.<br>"+
                "- Level 10: When a triggered ability from a ranged attack would happen, there's a 1%/level chance that it will trigger twice.<br>"+
                "- Level 15: Triggered abilities from ranged attacks do 25% more damage while Imbue Arrows is active.<br>"+
                "- Level 20: When imbue arrows ends, there's a 1%/level chance of the cooldown on barrage being set to zero.<br><br>"+

                "Charged Arrows, level 6<br>"+
                "command: [ArcaneArcherChargedArrows<br>"+
                "The arcane archer releases a powerful arrow that damages everything in its path for 45-75 + (int/15 + dex/15) damage of a random element.<br>"+ 
                "This ability costs 50 mana to activate and has a 1 minute cooldown. It has a range of 6 tiles.<br>"+
                "- Level 12: when charged arrows is cast, there's a 0.5%/level chance of setting the cooldown of Arcane Volley to zero.<br><br>"+

                "Arcane Volley, level 11<br>"+
                "command: [ArcaneArcherArcaneVolley<br>"+
                "The arcane archer targets a location and rapidly fires multiple arrows at it, hitting every enemy that is up to 4 tiles for 55-75 + inscription/10 damage of a random element.<br>"+ 
                "This ability costs 60 mana to activate and has a 30 seconds cooldown. <br>"+
                "- Level 16: there's a 1%/level chance that Arcane volley will trigger twice when cast.<br><br>"+

                "Barrage, level 18<br>"+
                "command: [ArcaneArcherBarrage<br>"+
                "The arcane archers fires a torrent of arrows towards the target, attacking level/4 times in an instant. This ability costs 70 mana to activate and has a 3 minutes cooldown.<br>"+
                "- Level 20: there's a 0.25%/level chance of barrage firing twice when cast.<br><br>"+

                "Arcane Precision, level 2<br>"+
                "The arcane archer causes 6% more damage with ranged attacks.<br>"+
                "- Level 5: The arcane archer causes 12% more damage with ranged attacks.<br>"+
                "- Level 13: The arcane archer causes 18% more damage with ranged attacks.<br><br>"+

                "Mystical Ricochet, level 8<br>"+
                "When firing a ranged attack, there's a 0.25%/level chance that the nearest enemy will also be damaged by it.<br>"+
                "- Level 17: Ricochet can trigger to up the two nearby enemies.<br><br>"+

                "Arcane Feedback, level 14<br>"+
                "When killing an opponent with a ranged attack, the arcane archer recovers inscription/25 mana.<br>"+
                "- Level 19:  When killing an opponent with a ranged attack, the arcane archer recovers either focus/25 stamina or focus/25 health.<br><br>"+

                "Arcane Momentum, level 20<br>"+
                "When killing an opponent with a ranged attack, the arcane archer gains inscription/8 spell absorbtion 30 seconds.<br>"+ 
                "</BASEFONT>";
            }
            else
            {
                return "IN DEVELOPMENT.";                
            }
        }

    }
}
