using System;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;
using Server.Items;
using Server.Misc;

namespace Server.Spells.Necromancy
{
    public class AnimateDeadSpell : NecromancerSpell
    {
        private static SpellInfo m_Info = new SpellInfo(
            "Animate Dead", "Uus Corp",
            203,
            9031,
            Reagent.GraveDust,
            Reagent.DaemonBlood
        );

        public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds(1.5); } }
        public override double  RequiredSkill  { get { return 40.0; } }
        public override int     RequiredMana   { get { return 23; } }

        public AnimateDeadSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
            Caster.SendLocalizedMessage(1061083); // Animate what corpse?
        }

        public void Target(object obj)
        {
            Corpse c = obj as Corpse;

            if (c == null)
            {
                Caster.SendLocalizedMessage(1061084); // You cannot animate that.
                FinishSequence();
                return;
            }

            BaseCreature bc = c.m_Owner as BaseCreature;

            if (bc == null)
            {
                Caster.SendLocalizedMessage(1061084); // You cannot animate that.
                FinishSequence();
                return;
            }

            // ── valid target check ────────────────────────────────────────────
            SlayerEntry undead = SlayerGroup.GetEntryByName(SlayerName.Silver);
            SlayerEntry golems = SlayerGroup.GetEntryByName(SlayerName.GolemDestruction);

            if (undead.Slays(bc))
            {
                Caster.SendMessage("You cannot animate supernatural creatures!");
                FinishSequence();
                return;
            }

            if (golems.Slays(bc))
            {
                Caster.SendMessage("You cannot animate constructs!");
                FinishSequence();
                return;
            }

            int levelCorpse = IntelligentAction.GetCreatureLevel(bc);
            int levelCaster = (int)((Spell.ItemSkillValue(Caster, SkillName.Necromancy, false)
                                   + Spell.ItemSkillValue(Caster, SkillName.Spiritualism, false)) / 2);

            if (levelCaster > 100) levelCaster = 100;

            if (levelCorpse > levelCaster)
            {
                Caster.SendMessage("You are not powerful enough to animate that!");
                FinishSequence();
                return;
            }

            // ── build the mobile ───────────────────────────────────
            double modify = levelCaster - levelCorpse;
            double mod    = modify < 0
                ? Math.Max(0.0, (100.0 - (levelCorpse + 50 - levelCaster)) * 0.01)
                : 1.0;

            int hitPoison = PoisonToInt(bc.HitPoison);
            int immune    = PoisonToInt(bc.PoisonImmune);

            SummonedCorpse creature = new SummonedCorpse(
                (int)(mod * bc.HitsMax), (int)(mod * bc.StamMax), (int)(mod * bc.ManaMax),
                (int)(mod * bc.RawStr),  (int)(mod * bc.RawDex),  (int)(mod * bc.RawInt),
                hitPoison, immune
            );

            creature.DamageMin             = (int)(mod * bc.DamageMin);
            creature.DamageMax             = (int)(mod * bc.DamageMax);
            creature.ColdDamage            = (int)(mod * bc.ColdDamage);
            creature.EnergyDamage          = (int)(mod * bc.EnergyDamage);
            creature.FireDamage            = (int)(mod * bc.FireDamage);
            creature.PhysicalDamage        = (int)(mod * bc.PhysicalDamage);
            creature.PoisonDamage          = (int)(mod * bc.PoisonDamage);
            creature.ColdResistSeed        = (int)(mod * bc.ColdResistSeed);
            creature.EnergyResistSeed      = (int)(mod * bc.EnergyResistSeed);
            creature.FireResistSeed        = (int)(mod * bc.FireResistSeed);
            creature.PhysicalResistanceSeed = (int)(mod * bc.PhysicalResistanceSeed);
            creature.PoisonResistSeed      = (int)(mod * bc.PoisonResistSeed);
            creature.VirtualArmor          = (int)(mod * bc.VirtualArmor);
            creature.CanSwim               = bc.CanSwim;
            creature.ControlSlots          =  1;

            if (creature.CanSwim)
                creature.CantWalk = bc.CantWalk;

            bool isMage = bc.AI == AIType.AI_Mage;

            if (isMage)
            {
                creature.AI = AIType.AI_Mage;
                creature.SetSkill(SkillName.Meditation,   mod * bc.Skills[SkillName.Meditation].Value);
                creature.SetSkill(SkillName.Psychology,   mod * bc.Skills[SkillName.Psychology].Value);
                creature.SetSkill(SkillName.Magery,       mod * bc.Skills[SkillName.Magery].Value);
                creature.SetSkill(SkillName.MagicResist,  mod * bc.Skills[SkillName.MagicResist].Value);
                creature.SetSkill(SkillName.Tactics,      mod * bc.Skills[SkillName.Tactics].Value);
                creature.SetSkill(SkillName.FistFighting, mod * bc.Skills[SkillName.FistFighting].Value);
            }
            else
            {
                creature.AI = AIType.AI_Melee;
                creature.SetSkill(SkillName.MagicResist,  mod * bc.Skills[SkillName.MagicResist].Value);
                creature.SetSkill(SkillName.Tactics,      mod * bc.Skills[SkillName.Tactics].Value);
                creature.SetSkill(SkillName.FistFighting, mod * bc.Skills[SkillName.FistFighting].Value);
            }

            // ── Appearance  ──────────────────────────────────
            int originalBody = (bc.TithingPoints > 0 && bc.Body != 400 && bc.Body != 401)
    			? bc.TithingPoints
    			: (int)bc.Body;

            ApplyAppearance(creature, bc, originalBody, isMage);

            // ── Summon the creature ──────────
            TimeSpan duration = TimeSpan.FromMinutes(2);

            SpellHelper.Summon(creature, Caster, 0x216, duration, false, false);

            creature.Summoned    = true;
            creature.SummonMaster = Caster;
            creature.Controlled  = false;
            creature.FightMode   = FightMode.Closest;
            creature.RangeHome   = 15;
            creature.Home        = Caster.Location;

            // Restore body set by ApplyAppearance after SpellHelper.Summon may reset it
            creature.Body = originalBody;

            Effects.SendLocationEffect(c.Location, c.Map, 0x3400, 60, 0, 0);
            Effects.PlaySound(c.Location, c.Map, 0x108);

            c.Delete();

            FinishSequence();
        }

        // ── Converts a Poison reference to the integer stored on SummonedCorpse ──
        private static int PoisonToInt(Poison p)
        {
            if (p == null)               return 0;
            if (p == Poison.Lesser)      return 1;
            if (p == Poison.Regular)     return 2;
            if (p == Poison.Greater)     return 3;
            if (p == Poison.Deadly)      return 4;
            if (p == Poison.Lethal)      return 5;
            return 0;
        }

        // ── Applies body, hue, sound, and name based on creature type ────────
        private static void ApplyAppearance(SummonedCorpse creature, BaseCreature bc, int baseBody, bool isMage)
        {
            SlayerEntry exorcism = SlayerGroup.GetEntryByName(SlayerName.Exorcism);
            SlayerEntry animal   = SlayerGroup.GetEntryByName(SlayerName.AnimalHunter);
            SlayerEntry plants   = SlayerGroup.GetEntryByName(SlayerName.WeedRuin);
            SlayerEntry repond   = SlayerGroup.GetEntryByName(SlayerName.Repond);
            SlayerEntry dragon   = SlayerGroup.GetEntryByName(SlayerName.DragonSlaying);
            SlayerEntry reptile  = SlayerGroup.GetEntryByName(SlayerName.ReptilianDeath);
            SlayerEntry spider   = SlayerGroup.GetEntryByName(SlayerName.ArachnidDoom);
            SlayerEntry elemental= SlayerGroup.GetEntryByName(SlayerName.ElementalBan);
            SlayerEntry wizard   = SlayerGroup.GetEntryByName(SlayerName.WizardSlayer);
            SlayerEntry birds    = SlayerGroup.GetEntryByName(SlayerName.AvianHunter);
            SlayerEntry slime    = SlayerGroup.GetEntryByName(SlayerName.SlimyScourge);
            SlayerEntry giant    = SlayerGroup.GetEntryByName(SlayerName.GiantKiller);

            // Determine creature type suffix
            string ghost = " creature";
            if      (exorcism.Slays(bc))  ghost = " demon";
            else if (animal.Slays(bc))    ghost = " animal";
            else if (plants.Slays(bc))    ghost = " weed";
            else if (dragon.Slays(bc))    ghost = " dragon";
            else if (reptile.Slays(bc))   ghost = " reptile";
            else if (spider.Slays(bc))    ghost = " insect";
            else if (elemental.Slays(bc)) ghost = " necromental";
            else if (birds.Slays(bc))     ghost = " bird";
            else if (slime.Slays(bc))     ghost = " slime";
            else if (giant.Slays(bc))     ghost = " giant";
            else if (repond.Slays(bc))    ghost = "";

            bool isHumanBody = (bc.Body == 400 || bc.Body == 401 || bc.Body == 605 || bc.Body == 606);

            if (isMage)
            {
                // ── Ghost / wraith ───────────────────────────────────────────
                creature.Hue        = Utility.RandomList(0x4001, 0x4001, 1150, 0x9C2);
                creature.BaseSoundID = 0x482;

                if (isHumanBody)
                    creature.Body = Utility.RandomList(0x3CA, 310, 26, 84);

                string[] mageNames = new string[] { "a wraith", "a ghostly", "a spectral", "a haunting", "a phantasmal", "a phantom", "a banshee" };
                creature.Name = mageNames[Utility.Random(mageNames.Length)] + ghost;

                // Skeletal mage variant for repond+wizard
                if (repond.Slays(bc) && wizard.Slays(bc) && ghost == "" && Utility.RandomBool())
                {
                    creature.Body        = Utility.RandomList(148, 110, 24);
                    creature.Hue         = 0;
                    creature.BaseSoundID = Utility.RandomList(0x19C, 0x3E9);

                    string[] skeleTypes = new string[] { " wizard", " mage", " sorcerer", " conjurer", " magician", " warlock", " enchanter" };
                    string   skeleType  = skeleTypes[Utility.Random(skeleTypes.Length)];

                    string[] skeleNames = new string[] { "a skeletal", "a bone", "a skeleton" };
                    creature.Name = skeleNames[Utility.Random(skeleNames.Length)] + skeleType;
                }
            }
            else
            {
                // ── Zombie ───────────────────────────────────────────────────
                creature.Hue        = Utility.RandomList(0xB97, 0xB98, 0xB99, 0xB9A, 0xB85, 0xB79, 0xB5F, 0xB60, 0xB19, 0xACC, 0xACD, 0xACE, 0xACF, 0xAB0, 0x938, 0x92D);
                creature.BaseSoundID = 471;

                if (isHumanBody)
                    creature.Body = Utility.RandomList(3, 728, 305, 181, 304, 307);

                string[] zombieNames = new string[] { "a zombie", "a dead", "a rotten", "an undead", "a rotting", "a decaying" };
                creature.Name = zombieNames[Utility.Random(zombieNames.Length)] + ghost;

                // Skeletal repond 
                if (repond.Slays(bc) && ghost == "" && Utility.RandomBool())
                {
                    creature.Body        = Utility.RandomList(57, 168, 170, 247, 327, 50, 56, 167);
                    creature.Hue         = 0;
                    creature.BaseSoundID = 451;

                    if (creature.Body == 327) creature.Hue = 0x9C4;

                    string[] warriorTypes = new string[] { " warrior", " knight", " fighter", " champion", " crusader", " soldier", " guard" };
                    string   warriorType  = warriorTypes[Utility.Random(warriorTypes.Length)];

                    string[] skeleNames = new string[] { "a skeletal", "a bone", "a skeleton" };
                    creature.Name = skeleNames[Utility.Random(skeleNames.Length)] + warriorType;
                }

                // Undead dragon 
                if (dragon.Slays(bc) && ghost != "" && bc.Fame >= 15000 && Utility.RandomBool())
                {
                    creature.Hue        = Utility.RandomList(0x83B, 0x89F, 0x8A0, 0x8A1, 0x8A2, 0x8A3, 0x8A4);
                    creature.BaseSoundID = 471;

                    string[] dragonNames = new string[] { "a zombie", "a dead", "a rotten", "an undead", "a rotting", "a decaying" };
                    creature.Name = dragonNames[Utility.Random(dragonNames.Length)] + ghost;

                    if (Utility.RandomBool())
                    {
                        creature.Body        = Utility.RandomList(104, 323, 323);
                        creature.BaseSoundID = 0x488;
                        creature.Hue         = 0;

                        string[] skeleNames = new string[] { "a skeletal", "a bone", "a skeleton" };
                        creature.Name = skeleNames[Utility.Random(skeleNames.Length)] + ghost;
                    }
                }

                // Skeletal giant 
                if (giant.Slays(bc) && ghost != "" && Utility.RandomBool())
                {
                    creature.Body = 999;

                    if (Utility.RandomBool())
                    {
                        creature.Body        = 308;
                        creature.BaseSoundID = 0x4FB;
                        creature.Hue         = 0;

                        string[] skeleNames = new string[] { "a skeletal", "a bone", "a skeleton" };
                        creature.Name = skeleNames[Utility.Random(skeleNames.Length)] + ghost;
                    }
                }

                // Skeletal demon 
                if (exorcism.Slays(bc) && ghost != "" && Utility.RandomMinMax(0, 5) == 1)
                {
                    creature.Body        = 339;
                    creature.BaseSoundID = 0x48D;
                    creature.Hue         = 0x80F;

                    string[] skeleNames = new string[] { "a skeletal", "a bone", "a skeleton" };
                    creature.Name = skeleNames[Utility.Random(skeleNames.Length)] + ghost;
                }
            }
        }

        private class InternalTarget : Target
        {
            private AnimateDeadSpell m_Owner;

            public InternalTarget(AnimateDeadSpell owner) : base(Core.ML ? 10 : 12, false, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                m_Owner.Target(o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}