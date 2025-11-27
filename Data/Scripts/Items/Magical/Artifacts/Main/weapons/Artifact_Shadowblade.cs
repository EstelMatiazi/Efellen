using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Engines.PartySystem;

namespace Server.Items
{
    public class Artifact_ShadowBlade : GiftLongsword
    {
        private DateTime m_NextAoE;

        [Constructable]
        public Artifact_ShadowBlade()
        {
            Name = "Blade of the Shadows";
            ItemID = 0xF61;
            Hue = 1899;

            Attributes.AttackChance = 5;
            Attributes.BonusDex = 2;
            Attributes.CastSpeed = 1;
            Attributes.SpellChanneling = 1;
            Attributes.SpellDamage = 21;

            WeaponAttributes.HitFireball = 10;
            WeaponAttributes.HitLeechMana = 10;
            WeaponAttributes.HitLeechStam = 10;

            ArtifactLevel = 2;
            Server.Misc.Arty.ArtySetup(this, "Reaps the Light");
        }

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

            if (attacker == null || defender == null)
                return;

            //only works for people serious about the dark knight business
            if (attacker.Skills[SkillName.Knightship].Value > 75.0 && attacker.Karma < -7777)
            {
                if (DateTime.UtcNow < m_NextAoE)
                    return;

                double skill = attacker.Skills[SkillName.Knightship].Value;

                double chance = 0.05 + (skill / 125.0) * 0.20; // 5% chant at 0 skill, 25% chance at 125 skill

                if (Utility.RandomDouble() > chance)
                    return;

                double seconds = 120.0 - (skill * (90.0 / 125.0)); // 120secs cooldown at 0 skill, 30 secs cooldown at 125 skill

                m_NextAoE = DateTime.UtcNow + TimeSpan.FromSeconds(seconds);

                // Base damage scales with NEGATIVE karma (inverted)
                int minDmg = (-attacker.Karma) / 777; // 19 at -15k
                int maxDmg = (-attacker.Karma) / 555; // 27 at -15k

                if (minDmg < 0) minDmg = 0;
                if (maxDmg < 0) maxDmg = 0;
                if (maxDmg < minDmg) maxDmg = minDmg;

                foreach (Mobile mob in defender.GetMobilesInRange(8))
                {
                    if (mob == null || mob == attacker || mob == defender)
                        continue;

                    // don't smite your pets
                    if (mob is BaseCreature)
                    {
                        BaseCreature bc = (BaseCreature)mob;
                        if ((bc.Controlled && bc.ControlMaster == attacker) ||
                            (bc.Summoned && bc.SummonMaster == attacker))
                            continue;
                    }

                    // don't smite your buddies
                    Party aParty = Party.Get(attacker);
                    Party mParty = Party.Get(mob);

                    if (aParty != null && mParty != null && aParty == mParty)
                        continue;

                    // don't smite guildmates
                    if (attacker.Guild != null && mob.Guild != null && attacker.Guild == mob.Guild)
                        continue;

                    // only smites *good* targets
                    if (mob.Karma < 0)
                        continue;

                    // smites gooder people harder
                    int bonus = 0;
                    if (mob.Karma > 0)
                    {
                        int scaled = 1 + ((mob.Karma) * 24 / 15000);
                        if (scaled < 1) scaled = 0;
                        if (scaled > 25) scaled = 25;
                        bonus = scaled;
                    }
					// smite!
                    int dmg = Utility.RandomMinMax(minDmg, maxDmg) + bonus;

                    if (dmg > 0)
                    {
                        AOS.Damage(mob, attacker, dmg, 0, 0, 0, 0, 100);//nrgy damage
                        mob.FixedParticles(0x373A, 20, 10, 5044, 0, 0, EffectLayer.Head);
                        mob.PlaySound(0x1FE);

                        attacker.SendMessage("Your Shadow Blade erupts with soul-devouring darkness!");
                    }
                }
            }
        }

        public Artifact_ShadowBlade(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            ArtifactLevel = 2;
            reader.ReadInt();
        }
    }
}
