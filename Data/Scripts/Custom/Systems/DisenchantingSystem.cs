using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Misc
{
    public class DisenchantingSystem
    {
        private static readonly string[] InvalidNames = new string[]
        { //non-identified items are implemented in a really weird way and the cleanest way to check for them if direct name check
            "an odd item",
            "an unusual item",
            "a bizarre item",
            "a curious item",
            "a peculiar item",
            "a strange item",
            "a weird item"
        };

        public static void HandleTarget(Mobile from, Item target, Item wand)
        {
            if (from == null || wand == null || wand.Deleted)
                return;

            int charges = GetCharges(wand);
            if (charges <= 0)
            {
                from.SendMessage("The wand has no remaining charges.");
                wand.Delete();
                return;
            }
            if (target is Container)
            {
                if (target.Name != null)
                {
                    for (int i = 0; i < InvalidNames.Length; i++)
                    {
                        if (target.Name.Equals(InvalidNames[i], StringComparison.OrdinalIgnoreCase))
                        {
                            from.SendMessage("You cannot disenchant unidentified items.");
                            return;
                        }
                    }
                }

                Container cont = (Container)target;

                if (!cont.IsChildOf(from.Backpack))
                {
                    from.SendMessage("You can only disenchant containers in your backpack.");
                    return;
                }

                from.SendGump(new ConfirmDisenchantGump(from, cont, wand));
                return;
            }

            if (!IsDisenchantable(target))
            {
                from.SendMessage("That cannot be disenchanted.");
                return;
            }

            int dust = GetArcaneDustValue(target);
            if (dust <= 0)
            {
                from.SendMessage("The power of this item is too faint to produce arcane dust.");
                return;
            }

            ConsumeCharge(wand, from);
            target.Delete();

            GiveArcaneDust(from, dust);
            from.SendMessage("{0} crumbled into {1} arcane dust.", target.Name != null ? target.Name : "An item", dust);
        }

        public static void DisenchantContainer(Mobile from, Container cont, Item wand)
        {
            if (from == null || cont == null || wand == null || wand.Deleted)
                return;

            int charges = GetCharges(wand);
            if (charges <= 0)
            {
                from.SendMessage("The wand has no remaining charges.");
                wand.Delete();
                return;
            }

            ArrayList validItems = new ArrayList();
            foreach (Item item in cont.Items)
            {
                if (IsDisenchantable(item))
                    validItems.Add(item);
            }

            if (validItems.Count == 0)
            {
                from.SendMessage("No valid items found to disenchant.");
                return;
            }

            int totalDust = 0;
            int disenchanted = 0;

            for (int i = 0; i < validItems.Count; i++)
            {
                if (charges <= 0)
                {
                    from.SendMessage("You could not finish your task because your wand has run out of charges.");
                    break;
                }

                Item item = (Item)validItems[i];
                int dust = GetArcaneDustValue(item);

                if (dust > 0)
                {
                    totalDust += dust;
                    disenchanted++;
                    item.Delete();
                    charges--;
                }
            }

            if (disenchanted > 0)
            {
                GiveArcaneDust(from, totalDust);
                from.SendMessage("{0} item{1} crumbled into {2} arcane dust.", disenchanted, disenchanted != 1 ? "s" : "", totalDust);
            }

            SetCharges(wand, charges);

            if (charges <= 0)
            {
                from.SendMessage("The wand has exhausted its unraveling powers and has ceased to be.");
                wand.Delete();
            }
        }

        private static bool IsDisenchantable(object o)
        {
            if (!(o is Item))
                return false;

            Item item = (Item)o;
            // can't disenchant unidentified items
            if (item.Name != null)
            {
                for (int i = 0; i < InvalidNames.Length; i++)
                {
                    if (item.Name.Equals(InvalidNames[i], StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
            }

            if (item.LootType == LootType.Blessed)
                return false;

            // can't disenchant arties
            try
            {
                System.Reflection.PropertyInfo p = item.GetType().GetProperty("ArtifactLevel");
                if (p != null)
                {
                    object val = p.GetValue(item, null);
                    if (val is int && (int)val > 0)
                        return false;
                }
            }
            catch { }

            return (item is BaseWeapon || item is BaseArmor || item is BaseTrinket || item is BaseClothing);
        }

        private static void GiveArcaneDust(Mobile from, int amount)
        {
            if (amount <= 0)
                return;

            ArcaneDust dust = new ArcaneDust(amount);
            if (!from.AddToBackpack(dust))
                dust.MoveToWorld(from.Location, from.Map);
        }

        private static void ConsumeCharge(Item wand, Mobile from)
        {
            int charges = GetCharges(wand);
            charges--;

            if (charges <= 0)
            {
                from.SendMessage("The wand has exhausted its unraveling powers and has ceased to be.");
                wand.Delete();
                return;
            }

            SetCharges(wand, charges);
        }

        private static int GetCharges(Item wand)
        {
            System.Reflection.PropertyInfo p = wand.GetType().GetProperty("Charges");
            if (p != null)
            {
                object val = p.GetValue(wand, null);
                if (val is int)
                    return (int)val;
            }
            return 0;
        }

        private static void SetCharges(Item wand, int value)
        {
            System.Reflection.PropertyInfo p = wand.GetType().GetProperty("Charges");
            if (p != null)
                p.SetValue(wand, value, null);
        }

        public static int GetArcaneDustValue(Item item)
        {
            int total = 0;

            if (item is BaseArmor || item is BaseShield)
            {
                BaseArmor armor = (BaseArmor)item;
                if (armor.ArmorAttributes != null && armor.ArmorAttributes.MageArmor == 1)
                    total += 10;
            }

            if (item is BaseWeapon)
            {
                BaseWeapon weapon = (BaseWeapon)item;
                total += 3 * (weapon.WeaponAttributes.HitColdArea + weapon.WeaponAttributes.HitDispel + weapon.WeaponAttributes.HitEnergyArea + weapon.WeaponAttributes.HitFireArea +
                              weapon.WeaponAttributes.HitFireball + weapon.WeaponAttributes.HitHarm + weapon.WeaponAttributes.HitLeechHits + weapon.WeaponAttributes.HitLightning +
                              weapon.WeaponAttributes.HitLowerAttack + weapon.WeaponAttributes.HitLowerDefend + weapon.WeaponAttributes.HitMagicArrow + weapon.WeaponAttributes.HitLeechMana +
                              weapon.WeaponAttributes.HitPhysicalArea + weapon.WeaponAttributes.HitPoisonArea + weapon.WeaponAttributes.HitLeechStam);

                if (weapon.WeaponAttributes.MageWeapon == 1)
                    total += 5;
                if (weapon.WeaponAttributes.UseBestSkill == 1)
                    total += 10;

                total += 2 * weapon.WeaponAttributes.SelfRepair;
                if (weapon.Slayer != SlayerName.None)
                    total += 20;

                if (weapon.Slayer2 != SlayerName.None)
                    total += 20;

            }

            if (item is BaseWeapon || item is BaseArmor || item is BaseTrinket || item is BaseClothing || item is Spellbook || item is BaseShield || item is BaseInstrument)
            {
                AosAttributes a = null;
                try
                {
                    System.Reflection.PropertyInfo p = item.GetType().GetProperty("Attributes");
                    if (p != null)
                        a = (AosAttributes)p.GetValue(item, null);
                }
                catch { }

                if (a != null)
                {
                    total += 8 * a.DefendChance;
                    total += 2 * a.EnhancePotions;
                    total += 20 * a.CastRecovery;
                    total += 20 * a.CastSpeed;
                    total += 10 * a.AttackChance;
                    total += 10 * a.BonusDex;
                    total += 5 * a.BonusHits;
                    total += 10 * a.BonusInt;
                    total += 5 * a.LowerManaCost;
                    total += 5 * a.LowerRegCost;
                    total += 2 * a.Luck;
                    total += 5 * a.BonusMana;
                    total += 5 * a.RegenMana;
                    total += 2 * a.ReflectPhysical;
                    total += 5 * a.RegenStam;
                    total += 5 * a.RegenHits;
                    total += 5 * a.SpellDamage;
                    total += 5 * a.BonusStam;
                    total += 10 * a.BonusStr;
                    total += 6 * a.WeaponSpeed;
                    if (a.NightSight == 1) total += 6;
                    if (a.SpellChanneling == 1) total += 15;
                }

                try
                {//need to cast everything to check for skill bonuses because runUO is very picky about this
                    SkillName skill;
                    double bonus;

                    if (item is BaseArmor)
                    {
                        BaseArmor armor = (BaseArmor)item;
                        for (int i = 0; i < 5; i++)
                        {
                            armor.SkillBonuses.GetValues(i, out skill, out bonus);
                            if (bonus > 0)
                                total += (int)(bonus * 3);
                        }
                    }
                    else if (item is BaseWeapon)
                    {
                        BaseWeapon weap = (BaseWeapon)item;
                        for (int i = 0; i < 5; i++)
                        {
                            weap.SkillBonuses.GetValues(i, out skill, out bonus);
                            if (bonus > 0)
                                total += (int)(bonus * 3);
                        }
                    }
                    else if (item is BaseClothing)
                    {
                        BaseClothing cloth = (BaseClothing)item;
                        for (int i = 0; i < 5; i++)
                        {
                            cloth.SkillBonuses.GetValues(i, out skill, out bonus);
                            if (bonus > 0)
                                total += (int)(bonus * 3);
                        }
                    }
                    else if (item is BaseTrinket)
                    {
                        BaseTrinket trinket = (BaseTrinket)item;
                        for (int i = 0; i < 5; i++)
                        {
                            trinket.SkillBonuses.GetValues(i, out skill, out bonus);
                            if (bonus > 0)
                                total += (int)(bonus * 3);
                        }
                    }
                }
                catch
                {
                }
            }

            int min = 0, max = 0;
            if (total < 25) { return 0; }
            else if (total < 50) { min = 1; max = 3; }
            else if (total < 100) { min = 3; max = 7; }
            else if (total < 150) { min = 5; max = 11; }
            else if (total < 200) { min = 8; max = 14; }
            else if (total < 250) { min = 11; max = 19; }
            else if (total < 300) { min = 15; max = 25; }
            else if (total < 350) { min = 20; max = 34; }
            else if (total < 400) { min = 26; max = 45; }
            else if (total < 450) { min = 33; max = 57; }
            else if (total < 500) { min = 41; max = 71; }
            else { min = 60; max = 90; }

            return Utility.RandomMinMax(min, max);
        }
    }

    public class ConfirmDisenchantGump : Gump
    {
        private Mobile m_From;
        private Container m_Container;
        private Item m_Wand;

        public ConfirmDisenchantGump(Mobile from, Container cont, Item wand) : base(100, 100)
        {
            m_From = from;
            m_Container = cont;
            m_Wand = wand;

            Closable = true;
            Dragable = true;
            AddPage(0);
            AddBackground(0, 0, 300, 140, 9270);
            AddHtml(20, 20, 260, 60, "Are you sure? Confirming will disenchant all valid items inside of this container.", true, true);
            AddButton(40, 100, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddLabel(75, 100, 0, "Confirm");
            AddButton(170, 100, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddLabel(205, 100, 0, "Cancel");
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1 && m_From != null && m_Container != null && m_Wand != null)
            {
                DisenchantingSystem.DisenchantContainer(m_From, m_Container, m_Wand);
            }
        }
    }
}
