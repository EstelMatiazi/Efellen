using System;
using Server;
using Server.Companions.Core;
using Server.Companions.Data;
using Server.Items;
using Server.Mobiles;

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
            ApplyShield(mob, c);
            ApplyCloak(mob, c);
            ApplyBoots(mob);
            
            if (c.CompanionGearTier >= 4)
            {
                ApplyAlignmentEffects(mob, c);

            }
        }

        private static void ApplyArmor(CompanionMobile mob, CompanionContract c)
        {
            int hue = c.CompanionGearColor;
            bool female = mob.Female;

            switch (c.CompanionArmorType)
            {
                case 1: // Chain
                    AddArmor(new ChainChest(), mob, hue, female ? 0x13BF : 0);
                    AddArmor(new ChainLegs(), mob, hue, 0);
                    AddArmor(new ChainCoif(), mob, hue, 0);
                    break;

                case 2: // Ring
                    AddArmor(new RingmailChest(), mob, hue, 0);
                    AddArmor(new RingmailLegs(), mob, hue, 0);
                    AddArmor(new RingmailGloves(), mob, hue, 0);
                    break;

                case 3: // Plate
                    if (female)
                        AddArmor(new FemalePlateChest(), mob, hue, 0);
                    else
                        AddArmor(new PlateChest(), mob, hue, 0);

                    AddArmor(new PlateLegs(), mob, hue, 0);
                    AddArmor(new PlateArms(), mob, hue, 0);
                    AddArmor(new PlateGorget(), mob, hue, 0);
                    AddArmor(new PlateGloves(), mob, hue, 0);

                    if (c.CompanionHelmID > 0)
                        AddArmor(new PlateHelm(), mob, hue, c.CompanionHelmID);
                    break;
            }
            ApplyClassHelm(mob,c);
        }
        private static void ApplyClassHelm(CompanionMobile mob, CompanionContract c)
        {
            Item helm = null;

            switch (c.CompanionClass)
            {
                case CompanionClass.Mage:
                    helm = new WizardsHat();
                    break;

                case CompanionClass.Ranger:
                    helm = new FeatheredHat();
                    break;

                case CompanionClass.Fighter:
                    if (c.CompanionHelmID > 0)
                    {
                        helm = new PlateHelm();
                        helm.ItemID = c.CompanionHelmID;
                    }
                    break;
            }

            if (helm != null)
            {
                helm.Hue = c.CompanionGearColor;
                helm.Movable = false;
                helm.LootType = LootType.Blessed;
                mob.AddItem(helm);
            }
        }


        private static void ApplyWeapon(CompanionMobile mob, CompanionContract c)
        {
            if (c.CompanionWeaponID <= 0)
                return;

            Item weapon;

            switch (c.CompanionWeaponType)
            {
                case 0: // 1h slashing
                    weapon = new Longsword();
                    break;

                case 2: // Staff
                    weapon = new GnarledStaff();
                    break;

                case 3: // Ranged
                    weapon = new Bow();
                    break;

                default:
                    return;
            }

            weapon.ItemID = c.CompanionWeaponID;
            weapon.Movable = false;
            weapon.LootType = LootType.Blessed;
            mob.AddItem(weapon);
        }

        private static bool IsTwoHandedWeapon(int weaponType)
        {
            // 2 = staff, 3 = ranged
            return weaponType == 2 || weaponType == 3;
        }

        private static int ChooseWeaponBySkills(CompanionMobile mob, CompanionContract c)
        {
            double sword = mob.Skills[SkillName.Swords].Base;
            double mace = mob.Skills[SkillName.Bludgeoning].Base;
            double arch = mob.Skills[SkillName.Marksmanship].Base;
            double fencing = mob.Skills[SkillName.Fencing].Base;

            if (arch > sword && arch > mace && arch > fencing)
            {
                c.CompanionWeaponType = 3;
                return Utility.RandomList(0x13B2, 0x26C2); // bows
            }

            if (sword > arch && sword > mace && sword > fencing)
            {
                c.CompanionWeaponType = 0;
                return Utility.RandomList(0x13B2, 0x26C2); // swords
            }

            if (mace > arch && mace > fencing && mace > sword)
            {
                c.CompanionWeaponType = 2;
                return Utility.RandomList(0x13B2, 0x26C2); // bludgeoning
            }

            if (fencing > arch && fencing > mace && mace > sword)
            {
                c.CompanionWeaponType = 2;
                return Utility.RandomList(0x13B2, 0x26C2); // fencing
            }

            c.CompanionWeaponType = 0;
            // sanity
            return Utility.RandomList(0x13B9, 0x13F6); // swords
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
        private static void ApplyShield(CompanionMobile mob, CompanionContract c)
        {
            if (c.CompanionShieldID <= 0)
                return;

            if (IsTwoHandedWeapon(c.CompanionWeaponType))
                return; // suppress shield


            Item shield = new BronzeShield();
            shield.ItemID = c.CompanionShieldID;
            shield.Movable = false;
            shield.LootType = LootType.Blessed;

            mob.AddItem(shield);
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
        private static void RemoveExistingEquipment(Mobile m)
        {
            Item item;
            for (int i = m.Items.Count - 1; i >= 0; i--)
            {
                item = m.Items[i];
                if (item != null && !item.Deleted)
                    item.Delete();
            }
        }
        private static void AddArmor(BaseArmor armor, Mobile mob, int hue, int itemID)
        {
            if (itemID > 0)
                armor.ItemID = itemID;
            armor.Hue = hue;
            armor.Movable = false;
            armor.LootType = LootType.Blessed;
            armor.StrRequirement = 1;
            mob.AddItem(armor);
        }
    }
    public static partial class CompanionEquipment
    {
        public static void EquipCompanion(
            CompanionContract contract,
            CompanionClass cClass,
            int level,
            CompanionAlignment alignment,
            bool forceRegenerate
        )
        {
            if (contract == null)
                return;

            int tier = GetLevelTier(level);

            if (!forceRegenerate && contract.CompanionGearTier == tier)
                return;

            ResetStoredGear(contract);
            contract.CompanionGearTier = tier;

            bool isGood = alignment.GetIsGood();
            bool isEvil = alignment.GetIsEvil();
            bool isNeutral = alignment.GetIsNeutral();
            bool isChaotic = alignment.GetIsChaotic();

            switch (cClass)
            {
                case CompanionClass.Fighter:
                    GenerateFighterGear(contract, tier, isGood, isEvil, isNeutral);
                    break;

                case CompanionClass.Mage:
                    GenerateMageGear(contract, tier, isEvil);
                    break;

                case CompanionClass.Ranger:
                    GenerateRangerGear(contract, tier, isNeutral);
                    break;
            }
        }
        /* =========================
         * TIERS
         * ========================= */
        private static int GetLevelTier(int level)
        {
            if (level < 5) return 0;
            if (level < 10) return 1;
            if (level < 15) return 2;
            if (level < 20) return 3;
            return 4;
        }

        private static void ResetStoredGear(CompanionContract c)
        {
            c.CompanionWeaponID = 0;
            c.CompanionShieldID = 0;
            c.CompanionHelmID = 0;
            c.CompanionArmorType = 0;
            c.CompanionWeaponType = 0;
            c.CompanionCloak = 0;
            c.CompanionCloakColor = 0;
            c.CompanionGearColor = 0;
        }
        /* =========================
         * HUE HELPERS
         * ========================= */
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
        /* =========================
         * FIGHTER
         * ========================= */
        private static void GenerateFighterGear(
            CompanionContract c,
            int tier,
            bool good,
            bool evil,
            bool neutral
        )
        {
            c.CompanionGearColor = GetAlignmentHue(good, evil, neutral);
            c.CompanionWeaponType = 0; // 1h slashing
            c.CompanionShieldID = Utility.RandomList(0x1B72, 0x1BC3);

            switch (tier)
            {
                case 0: // Chain
                    c.CompanionArmorType = 1;
                    c.CompanionWeaponID = Utility.RandomList(0x13B9, 0x13B6);
                    // c.CompanionWeaponID = ChooseWeaponBySkills(mob, c);
                    break;

                case 1: // Ring
                    c.CompanionArmorType = 2;
                    c.CompanionWeaponID = Utility.RandomList(0x13B9, 0x13F6);
                    break;

                case 2: // Mixed
                    c.CompanionArmorType = Utility.RandomBool() ? 2 : 3;
                    c.CompanionWeaponID = Utility.RandomList(0x13B9, 0x13F6, 0x1401);
                    break;

                case 3: // Plate
                    c.CompanionArmorType = 3;
                    c.CompanionHelmID = Utility.RandomList(0x140C, 0x1412);
                    c.CompanionWeaponID = Utility.RandomList(0x13F6, 0x1401);
                    break;

                case 4: // Champion
                    c.CompanionArmorType = 3;
                    c.CompanionHelmID = Utility.RandomList(0x140C, 0x1412);
                    c.CompanionGearColor = Utility.RandomMetalHue();
                    c.CompanionWeaponID = Utility.RandomList(0x13F6, 0x1401);
                    break;
            }
        }
        /* =========================
         * MAGE
         * ========================= */
        private static void GenerateMageGear(
            CompanionContract c,
            int tier,
            bool evil
        )
        {
            c.CompanionGearColor = evil
                ? Utility.RandomList(0x485, 0x497, 0x4E9)
                : Utility.RandomList(0x482, 0x59B);

            c.CompanionWeaponType = 2; // staff
            c.CompanionWeaponID = Utility.RandomList(0x13F8, 0x26BC);
            c.CompanionCloak = Utility.RandomBool() ? 1 : 0;
            c.CompanionCloakColor = c.CompanionGearColor;
        }
        /* =========================
         * RANGER
         * ========================= */
        private static void GenerateRangerGear(
            CompanionContract c,
            int tier,
            bool neutral
        )
        {
            c.CompanionGearColor = Utility.RandomList(0x59B, 0x5A3, 0x455);
            c.CompanionWeaponType = 3; // ranged
            c.CompanionWeaponID = Utility.RandomList(0x13B2, 0x26C2);
            c.CompanionArmorType = Utility.RandomBool() ? 0 : 1;
            c.CompanionCloak = Utility.RandomBool() ? 1 : 0;
            c.CompanionCloakColor = c.CompanionGearColor;
        }
    }
}
