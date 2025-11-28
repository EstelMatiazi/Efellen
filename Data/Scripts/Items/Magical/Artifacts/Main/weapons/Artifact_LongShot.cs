using System;
using Server;

namespace Server.Items
{
    public class Artifact_LongShot : GiftCompositeBow
    {
        public override int InitMinHits { get { return 80; } }
        public override int InitMaxHits { get { return 160; } }

        private DateTime m_NextHeadshot;

        [Constructable]
        public Artifact_LongShot()
        {
            Name = "Long Shot";
            Hue = 1195;
            ItemID = 0x13B2;
            Attributes.AttackChance = 30;
            Attributes.SpellChanneling = 1;
            Velocity = 30;

            ArtifactLevel = 2;

            m_NextHeadshot = DateTime.UtcNow;

            Server.Misc.Arty.ArtySetup(this, "Headhunter");
        }

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

            if (attacker == null || defender == null || !defender.Alive)
                return;

            double ms = attacker.Skills[SkillName.Marksmanship].Value;
            double tr = attacker.Skills[SkillName.Tracking].Value;
            double tactics = attacker.Skills[SkillName.Tactics].Value;
            double anatomy = attacker.Skills[SkillName.Anatomy].Value;

            double chance = 15.0 + ((ms / 25.0) + (tr / 25.0));
            double cooldownSeconds = (10 * 60) - (tr * 5.0);
            if (cooldownSeconds < 10) cooldownSeconds = 10;

            if (DateTime.UtcNow < m_NextHeadshot)
                return;

            if (Utility.RandomDouble() > (chance / 100.0))
                return;


            m_NextHeadshot = DateTime.UtcNow + TimeSpan.FromSeconds(cooldownSeconds);

            
            int dmg = Utility.RandomMinMax(this.MinDamage, this.MaxDamage);
            dmg += (int)(attacker.Str / 10.0);
            dmg = (int)(dmg * (0.5 + (tactics * 0.005)));
            dmg = (int)(dmg * (1.0 + (anatomy * 0.005)));
            // Apply damageScalar
            dmg = (int)(dmg * (1.0 + damageBonus));
            // Velocity
            int dist = (int)attacker.GetDistanceToSqrt(defender);
            if (this.Velocity > 0 && dist > 2)
            {
                double velBonus = (this.Velocity / 100.0) * dist;
                if (velBonus > 0.9) velBonus = 0.9;
                dmg = (int)(dmg * (1.0 + velBonus));
            }
            if (dmg < 1) dmg = 1;
            // Headshot does extra damage based on 35% of the simulated value
            int extra = (int)(dmg * 0.35);
            if (extra < 1) extra = 1;
            // Apply as physical damage
            AOS.Damage(defender, attacker, extra, 100, 0, 0, 0, 0);

            attacker.SendMessage(1161, "Headshot for " + extra+ " extra damage");
            defender.FixedParticles(0x36BD, 10, 10, 5044, 0, 0, EffectLayer.Head);
            defender.PlaySound(0x22F);
        }


        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = 100;
            cold = 0;
            fire = 0;
            nrgy = 0;
            pois = 0;
            chaos = 0;
            direct = 0;
        }

        public Artifact_LongShot(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write(m_NextHeadshot);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            ArtifactLevel = 2;

            int version = reader.ReadInt();
            m_NextHeadshot = reader.ReadDateTime();
        }
    }
}
