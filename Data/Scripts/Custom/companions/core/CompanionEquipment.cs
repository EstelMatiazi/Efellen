using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Companions.Data;

namespace Server.Companions.Core
{
    public static partial class CompanionEquipment
    {
        public static void ApplyEquipment(CompanionMobile mob, CompanionContract c)
        {
            if (mob == null || mob.Deleted || c == null)
                return;

            RemoveExistingEquipment(mob);

            ApplyArmor(mob, c);
            ApplyWeapon(mob, c);
            ApplyShield(mob, c, c.CompanionClass,mob.Level / 4 );
            ApplyCloak(mob, c);
            ApplyBoots(mob);

            if (c.CompanionGearTier >= 4)
                ApplyAlignmentEffects(mob, c);
        }

        #region armor definitions    
        private static readonly Type[] PlateArmor =
        {
            typeof(PlateArms), typeof(PlateChest), typeof(PlateGloves),
            typeof(PlateGorget), typeof(PlateLegs),typeof(PlateSkirt),typeof(FemalePlateChest)
        };

        private static readonly Type[] OrientalPlateArmor =
        {
            typeof(PlateDo),typeof(PlateHaidate), typeof(PlateHiroSode), typeof(PlateMempo),typeof(PlateSuneate)            
        };

        private static readonly Type[] ChainArmor =
        {
            typeof(ChainChest), typeof(ChainLegs)
        };

        private static readonly Type[] RingmailArmor =
        {
            typeof(RingmailArms), typeof(RingmailChest),
            typeof(RingmailGloves), typeof(RingmailLegs),
            typeof(RingmailSkirt)
        };

        private static readonly Type[] BoneArmor =
        {
            typeof(BoneArms), typeof(BoneChest),
            typeof(BoneGloves), typeof(BoneHelm),
            typeof(BoneLegs)
        };

        private static readonly Type[] SavageArmor =
        {
            typeof(SavageArms), typeof(SavageChest),
            typeof(SavageGloves),
            typeof(SavageLegs)
        };

        private static readonly Type[] LeatherArmor =
        {
            typeof(LeatherArms), typeof(LeatherChest),
            typeof(LeatherGloves), typeof(LeatherGorget),
            typeof(LeatherLegs), typeof(LeatherSkirt),
        };

        private static readonly Type[] LeatherOrientalArmor =
        {
            typeof(LeatherDo),
            typeof(LeatherHaidate), typeof(LeatherHiroSode),
            typeof(LeatherMempo), typeof(LeatherSuneate),
        };

        private static readonly Type[] RoyalArmor =
        {
            typeof(RoyalArms),typeof(RoyalChest),typeof(RoyalGloves),typeof(RoyalsLegs),typeof(RoyalGorget)
        };

        private static readonly Type[] LeatherNinjaArmor =
        {
            typeof(LeatherNinjaHood), typeof(LeatherNinjaMitts),
            typeof(OniwabanGloves), typeof(OniwabanLeggings),
            typeof(OniwabanTunic)
        };

        private static readonly Type[] RangerArmor =
        {
            typeof(RangerArms), typeof(RangerChest),
            typeof(RangerGloves), typeof(RangerGorget),
            typeof(RangerLegs)
        };

        private static readonly Type[] StuddedArmor =
        {
            typeof(StuddedArms), typeof(StuddedChest),
            typeof(StuddedGloves), typeof(StuddedGorget),
            typeof(StuddedLegs),
        };

        private static readonly Type[] StuddedOrientalArmor =
        {
            typeof(StuddedArms), typeof(StuddedChest),
            typeof(StuddedGloves), typeof(StuddedGorget),
            typeof(StuddedLegs)
        };

        private static readonly Type[] ScaledArmor =
        {
            typeof(ScaledArms), typeof(ScaledChest),
            typeof(ScaledGloves), typeof(ScaledGorget),
            typeof(ScaledLegs)
            };

        private static readonly Type[] DragonArmor =
        {
            typeof(DragonArms), typeof(DragonChest),
            typeof(DragonGloves), typeof(DragonLegs),
        };

        private static readonly Type[] DrakboneArmor = 
        {
            typeof(DrakboneBracers), typeof(DrakboneGreaves),
            typeof(DrakboneGuantlets), typeof(DrakboneTunic)

        };

        private static readonly Type[] ScalyArmor = 
        {
            typeof(ScalyArms),typeof(ScalyChest),typeof(ScalyGloves),typeof(ScalyGorget),typeof(ScalyLegs)
        };
        #endregion
        #region helmets and shields
        private static readonly Type[] Helmets =
        {
            typeof(Bascinet), typeof(CloseHelm), typeof(DreadHelm),
            typeof(Helmet), typeof(NorseHelm),
            typeof(PlateHelm), typeof(RoyalHelm),
            typeof(ScalyHelm), typeof(ScaledHelm),
            typeof(BearCap), typeof(DeerCap),
            typeof(StagCap), typeof(WolfCap),
            typeof(OniwabanHood), typeof(OrcHelm),
            typeof(SavageHelm),typeof(DragonHelm),
            typeof(DrakboneHelm)
        };

        private static readonly Type[] Shields =
        {
            typeof(Buckler), typeof(BronzeShield),
            typeof(HeaterShield), typeof(MetalShield),
            typeof(MetalKiteShield), typeof(WoodenShield),
            typeof(WoodenKiteShield),
            typeof(OrderShield), typeof(ChaosShield),
            typeof(RoyalShield), typeof(VirtueShield),
            typeof(ChampionShield), typeof(ElvenShield),
            typeof(SunShield), typeof(DarkShield)
        };
        #endregion

        #region hues
        private static readonly int[] LawfulGoodHues =
        {
            1170, // gold
            2406, // pale white-gold
            2410, // soft holy glow
            1111, // iron grey
            2413, // muted plate
        };

        private static readonly int[] LawfulNeutralHues =
        {
            1109, // steel
            1111, // iron grey
            2401, // polished silver
            2305, // neutral metal
            2413, // muted plate
        };

        private static readonly int[] LawfulEvilHues =
        {
            1107, // dark iron
            1115, // blackened steel
            1175, // dull bronze
            1908, // blood-metal
            2419, // ominous metal
        };

        private static readonly int[] NeutralGoodHues =
        {
            2125, // soft green
            2213, // pale blue
            2407, // light cloth
            2414, // gentle silver
            2422, // calm neutral
        };

        private static readonly int[] TrueNeutralHues =
        {
            2101, // brown
            2115, // tan
            2309, // faded iron
            2403, // weathered steel
            2420, // dusted grey
        };

        private static readonly int[] NeutralEvilHues =
        {
            1905, // dark brown-red
            1910, // dried blood
            2109, // dirty leather
            2307, // tarnished metal
            2425, // grim neutral
        };

        private static readonly int[] ChaoticGoodHues =
        {
            1266, // bright blue
            1272, // teal
            1359, // emerald
            1365, // vivid green
            1281, // sky blue
            1153, // gold accent
        };

        private static readonly int[] ChaoticNeutralHues =
        {
            1184, // copper
            1903, // rust
            2107, // muddy brown
            2210, // faded blue
            2427, // weird grey
        };

        private static readonly int[] ChaoticEvilHues =
        {
            1645, // deep red
            1909, // blood
            1912, // corrupted flesh
            1108, // pitch metal
            1176, // sickly bronze
            2429, // void-dark
        };
        #endregion



        /* =========================================================
         * ARMOR APPLICATION
         * ========================================================= */
        #region core equipment application
        private static void ApplyArmor(CompanionMobile mob, CompanionContract c)
        {
            if (mob == null || c == null)
                return;

            // Armorless classes
            switch (c.CompanionClass)
            {
                case CompanionClass.Monk:
                case CompanionClass.Mage:
                case CompanionClass.Sorcerer:
                    return;
            }

            int tier = Math.Max(1, Math.Min(5, mob.Level / 4));
            Type[] armorSet = ResolveArmorSet(mob, c, tier);

            if (armorSet == null)
                return;
            
            CompanionAlignment alignment = mob.Alignment;
            bool chaotic = alignment.GetIsChaotic();
            int uniformHue = chaotic ? 0 : GetUniformHue(alignment);
            for (int i = 0; i < armorSet.Length; i++)
            {
                int hue = chaotic
                    ? GetRandomPieceHue(alignment)
                    : uniformHue;

                AddArmor(armorSet[i], mob, hue);
            }
        }


        private static Type[] ResolveArmorSet(CompanionMobile mob,CompanionContract c,int tier)
        {
            CompanionAlignment align = mob.Alignment;
            bool evil = align.GetIsEvil();
            int armorChoice;
            switch (c.CompanionClass)
            {
                case CompanionClass.Fighter:
                    if (tier == 1) return ChainArmor;
                    if (tier == 2) return RingmailArmor;
                    if (tier == 3) 
                    {
                        armorChoice = Utility.Random(8);
                        if (armorChoice <= 1)
                        {
                            return OrientalPlateArmor;
                        }
                        else if (armorChoice <= 3)
                        {
                            return PlateArmor;
                        }
                        else
                        {
                            return RingmailArmor;
                        }
                    } 
                    if (tier <= 4)
                    {
                        armorChoice = Utility.Random(8);
                        if (armorChoice <= 2)
                        {
                            return OrientalPlateArmor;
                        }
                        else
                        {
                            return PlateArmor;
                        }
                          
                    } 
                    return evil ? BoneArmor : DragonArmor; 

                case CompanionClass.Barbarian:
                    if (tier == 1) return null;
                    if (tier == 2) return LeatherArmor;
                    if (tier <= 4) return StuddedArmor;
                    return evil ? BoneArmor : ScalyArmor;

                case CompanionClass.Paladin:
                    if (tier == 1) return ChainArmor;
                    if (tier == 2) return RingmailArmor;
                    if (tier == 3) return PlateArmor;
                    if (tier == 4) return Utility.RandomBool() ? PlateArmor : RoyalArmor;
                    return RoyalArmor;

                case CompanionClass.Druid:
                    if (tier <= 2) return LeatherArmor;
                    if (tier == 3) return Utility.RandomBool() ? StuddedArmor : SavageArmor;
                    return evil ? DrakboneArmor : ScalyArmor;

                case CompanionClass.Ranger:
                    if (tier == 1) return LeatherArmor;
                    if (tier <= 3) return Utility.RandomBool() ? LeatherNinjaArmor : SavageArmor;
                    return RangerArmor;

                case CompanionClass.Cleric:
                    if (tier <= 2) return ChainArmor;
                    if (tier <= 4) return evil ? ScaledArmor : RingmailArmor;
                    return evil ? ScaledArmor : PlateArmor;

                case CompanionClass.Bard:
                    if (tier <= 2) return Utility.RandomBool() ? LeatherOrientalArmor : LeatherArmor;
                    if (tier <= 4) return Utility.RandomBool() ? StuddedOrientalArmor : StuddedArmor;
                    return ChainArmor;

                case CompanionClass.Rogue:
                    if (tier <= 2) return Utility.RandomBool() ? LeatherOrientalArmor : LeatherArmor;
                    return Utility.RandomBool() ? StuddedOrientalArmor : StuddedArmor;
            }

            return null;
        }

        /* =========================================================
         * WEAPONS / SHIELDS
         * ========================================================= */

        private static void ApplyWeapon(CompanionMobile mob, CompanionContract c)
        {
            if (c.CompanionWeaponID <= 0)
                return;

            Item weapon;

            switch (c.CompanionWeaponType)
            {
                case 2: weapon = new GnarledStaff(); break;
                case 3: weapon = new Bow(); break;
                default: weapon = new Longsword(); break;
            }

            weapon.ItemID = c.CompanionWeaponID;
            weapon.Movable = false;
            weapon.LootType = LootType.Blessed;
            mob.AddItem(weapon);
        }

        private static void AddShield(Type shieldType, Mobile mob, int hue)
        {
            if (shieldType == null || mob == null)
                return;

            BaseShield shield = Activator.CreateInstance(shieldType) as BaseShield;
            if (shield == null)
                return;

            shield.Hue = hue;
            shield.Movable = false;
            shield.LootType = LootType.Blessed;

            mob.AddItem(shield);
        }

        private static void ApplyShield(CompanionMobile mob,CompanionContract c,CompanionClass cClass,int tier)
        {
            if (mob == null || mob.Deleted)
                return;

            CompanionAlignment alignment = mob.Alignment;

            int hue = alignment.GetIsChaotic()
                ? GetRandomPieceHue(alignment)
                : GetUniformHue(alignment);

            switch (cClass)
            {
                case CompanionClass.Druid:
                    ApplyDruidShield(mob, tier, hue);
                    break;

                case CompanionClass.Paladin:
                    ApplyPaladinShield(mob, tier, alignment, hue);
                    break;

                case CompanionClass.Cleric:
                    ApplyClericShield(mob, tier, alignment, hue);
                    break;

                case CompanionClass.Fighter:
                    ApplyFighterShield(mob, tier, hue);
                    break;
            }
        }

        private static void ApplyDruidShield(Mobile mob, int tier, int hue)
        {
            Type shield;

            if (tier <= 2)
                shield = typeof(WoodenShield);
            else if (tier == 3)
                shield = typeof(WoodenKiteShield);
            else
                shield = typeof(ElvenShield);

            AddShield(shield, mob, hue);
        }

        private static void ApplyPaladinShield(Mobile mob,int tier,CompanionAlignment alignment,int hue)
        {
            Type shield;

            if (tier == 2)
                shield = typeof(OrderShield);
            else if (tier == 3 || tier == 4)
                shield = typeof(VirtueShield);
            else
                shield = typeof(ChampionShield);

            AddShield(shield, mob, hue);
        }

        private static void ApplyClericShield(Mobile mob,int tier,CompanionAlignment alignment,int hue)
        {
            Type shield;

            if (tier == 1)
                shield = typeof(BronzeShield);
            else if (tier == 2)
                shield = typeof(HeaterShield);
            else if (tier == 3)
            {
                if (alignment.GetIsGood())
                    shield = typeof(VirtueShield);
                else if (alignment.GetIsEvil())
                    shield = typeof(ChaosShield);
                else
                    shield = typeof(ChampionShield);
            }
            else
            {
                if (alignment.GetIsGood())
                    shield = typeof(SunShield);
                else if (alignment.GetIsEvil())
                    shield = typeof(DarkShield);
                else
                    shield = typeof(ChampionShield);
            }

            AddShield(shield, mob, hue);
        }

        private static void ApplyFighterShield(Mobile mob, int tier, int hue)
        {
            Type shield;

            if (tier == 1)
                shield = typeof(BronzeShield);
            else if (tier == 2)
                shield = typeof(HeaterShield);
            else if (tier == 3)
                shield = typeof(MetalShield);
            else
                shield = typeof(MetalKiteShield);

            AddShield(shield, mob, hue);
        }
        private static void ApplyCloak(CompanionMobile mob, CompanionContract c)
        {
            if (c.CompanionCloak != 1)
                return;

            Cloak cloak = new Cloak();
            cloak.Hue = c.CompanionCloakColor;
            cloak.Movable = false;
            cloak.LootType = LootType.Blessed;
            mob.AddItem(cloak);
        }

        private static void ApplyBoots(Mobile mob)
        {
            Boots boots = new Boots();
            boots.Hue = 0x967;
            boots.Movable = false;
            boots.LootType = LootType.Blessed;
            mob.AddItem(boots);
        }
        #endregion

        #region helpers


        private static void AddArmor(Type type, Mobile mob, int hue)
        {
            BaseArmor armor = Activator.CreateInstance(type) as BaseArmor;
            if (armor == null)
                return;

            armor.Hue = hue;
            armor.Movable = false;
            armor.LootType = LootType.Blessed;
            armor.StrRequirement = 1;
            mob.AddItem(armor);
        }

        private static void RemoveExistingEquipment(Mobile m)
        {
            for (int i = m.Items.Count - 1; i >= 0; i--)
            {
                Item item = m.Items[i];
                if (item != null && !item.Deleted)
                    item.Delete();
            }
        }

        private static int GetAlignmentHue(bool good, bool evil, bool neutral)
        {
            if (evil)
                return Utility.RandomList(0x485, 0x497, 0x4E9);

            if (good)
                return Utility.RandomList(0x47E, 0x966, 0x973);

            return Utility.RandomList(0x59B, 0x5A3, 0x455);
        }
        public static int GetHue(int nValue)
        {
            int Hue = 0;
            switch (nValue)
            {
                case 0: Hue = Utility.RandomNeutralHue(); break;
                case 1: Hue = Utility.RandomRedHue(); break;
                case 2: Hue = Utility.RandomBlueHue(); break;
                case 3: Hue = Utility.RandomGreenHue(); break;
                case 4: Hue = Utility.RandomYellowHue(); break;
                case 5: Hue = Utility.RandomSnakeHue(); break;
                case 6: Hue = Utility.RandomMetalHue(); break;
                case 7: Hue = Utility.RandomAnimalHue(); break;
                case 8: Hue = Utility.RandomSlimeHue(); break;
                case 9: Hue = Utility.RandomOrangeHue(); break;
                case 10: Hue = Utility.RandomPinkHue(); break;
                case 11: Hue = Utility.RandomDyedHue(); break;
                case 12: Hue = Utility.RandomList(0x467E, 0x481, 0x482, 0x47F); break;
                case 13: Hue = Utility.RandomList(0x54B, 0x54C, 0x54D, 0x54E, 0x54F, 0x550, 0x4E7, 0x4E8, 0x4E9, 0x4EA, 0x4EB, 0x4EC); break;
                case 14: Hue = Utility.RandomList(0x551, 0x552, 0x553, 0x554, 0x555, 0x556, 0x4ED, 0x4EE, 0x4EF, 0x4F0, 0x4F1, 0x4F2); break;
                case 15: Hue = Utility.RandomList(0x557, 0x558, 0x559, 0x55A, 0x55B, 0x55C, 0x4F3, 0x4F4, 0x4F5, 0x4F6, 0x4F7, 0x4F8); break;
                case 16: Hue = Utility.RandomList(0x55D, 0x55E, 0x55F, 0x560, 0x561, 0x562, 0x4F9, 0x4FA, 0x4FB, 0x4FC, 0x4FD, 0x4FE); break;
                case 17: Hue = Utility.RandomList(0xB93, 0xB94, 0xB95, 0xB96, 0xB83); break;
                case 18: Hue = Utility.RandomList(0x1, 0x497, 0x965, 0x966, 0x96B, 0x96C); break;
            }
            return Hue;
        }
         private static int GetLevelTier(int level)
        {
            if (level < 5) return 0;
            if (level < 10) return 1;
            if (level < 15) return 2;
            if (level < 20) return 3;
            return 4;
        }
        private static void ApplyAlignmentEffects(CompanionMobile mob,CompanionContract c)
        {
            if (mob == null || mob.Deleted || c == null)
                return;     

            CompanionAlignment alignment =
                new CompanionAlignment(c.OrderAxis, c.MoralAxis);       

            if (alignment.GetIsGood())
                ApplyGoodEffects(mob);
            else if (alignment.GetIsEvil())
                ApplyEvilEffects(mob);
            else if (alignment.GetIsChaotic())
                ApplyChaoticEffects(mob);
            else
                ApplyNeutralEffects(mob);
        }


        private static void ApplyGoodEffects(CompanionMobile mob)
        {
            mob.FixedParticles(
                0x376A, // holy sparkles
                9,
                32,
                5008,
                EffectLayer.Waist
            );

            if (GetLevelTier(mob.Level) >= 4)
            {
                mob.FixedParticles(
                    0x37B9,
                    10,
                    25,
                    9502,
                    EffectLayer.Head
                );
            }

        }
        private static void ApplyEvilEffects(CompanionMobile mob)
        {
            mob.FixedParticles(
                0x3709, // dark flames
                10,
                30,
                5052,
                EffectLayer.Waist
            );

            if (GetLevelTier(mob.Level) >= 4)
            {
                mob.FixedParticles(
                    0x37B9,
                    10,
                    25,
                    9502,
                    EffectLayer.Head
                );
            }

        }

        private static void ApplyChaoticEffects(CompanionMobile mob)
        {
            mob.FixedParticles(
                0x375A, // chaotic swirl
                8,
                20,
                5036,
                EffectLayer.Waist
            );

            if (GetLevelTier(mob.Level) >= 4)
            {
                mob.FixedParticles(
                    0x37B9,
                    10,
                    25,
                    9502,
                    EffectLayer.Head
                );
            }
        }
        private static int[] GetHuePool(CompanionAlignment alignment)
        {
            if (alignment.GetIsLawful())
            {
                if (alignment.GetIsGood())
                    return LawfulGoodHues;

                if (alignment.GetIsEvil())
                    return LawfulEvilHues;

                return LawfulNeutralHues;
            }

            if (alignment.GetIsChaotic())
            {
                if (alignment.GetIsGood())
                    return ChaoticGoodHues;

                if (alignment.GetIsEvil())
                    return ChaoticEvilHues;

                return ChaoticNeutralHues;
            }

            if (alignment.GetIsGood())
                return NeutralGoodHues;

            if (alignment.GetIsEvil())
                return NeutralEvilHues;

            return TrueNeutralHues;
        }


        private static int GetUniformHue(CompanionAlignment a)
        {
            int[] hues = GetHuePool(a);
            return hues[Utility.Random(hues.Length)];
        }

        private static int GetRandomPieceHue(CompanionAlignment a)
        {
            int[] hues = GetHuePool(a);
            return hues[Utility.Random(hues.Length)];
        }

        private static Type GetRandomType(Type[] types)
        {
            if (types == null || types.Length == 0)
                return null;

            return types[Utility.Random(types.Length)];
        }


        private static void ApplyNeutralEffects(CompanionMobile mob)
        {
            mob.FixedParticles(
                0x373A, // subtle glow
                6,
                15,
                5018,
                EffectLayer.Waist
            );

            if (GetLevelTier(mob.Level) >= 4)
            {
                mob.FixedParticles(
                    0x37B9,
                    10,
                    25,
                    9502,
                    EffectLayer.Head
                );
            }
        }
        #endregion
    }
}
