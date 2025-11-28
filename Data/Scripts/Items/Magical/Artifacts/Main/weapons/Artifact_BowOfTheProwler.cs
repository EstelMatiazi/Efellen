using System;
using Server;

namespace Server.Items
{
	public class Artifact_BowOfTheProwler : GiftBow
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }
		private DateTime m_NextHeadshot;

		[Constructable]
		public Artifact_BowOfTheProwler()
		{
			Name = "Bow of the Prowler";
			Hue = 0x30A;
			Attributes.AttackChance = 10;
			Attributes.WeaponDamage = 20;
			SkillBonuses.SetValues(0, SkillName.Marksmanship, 15);
			SkillBonuses.SetValues(1, SkillName.Hiding, 15);
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Headhunter" );
			 m_NextHeadshot = DateTime.UtcNow;
		}

		private bool m_WasHiddenBeforeShot = false;

		public override TimeSpan OnSwing(Mobile attacker, Mobile defender)
		{
		    m_WasHiddenBeforeShot = attacker.Hidden;
		    return base.OnSwing(attacker, defender);
		}

		public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

			if (!m_WasHiddenBeforeShot)
			    return;

            if (attacker == null || defender == null || !defender.Alive)
                return;

            double ms = attacker.Skills[SkillName.Marksmanship].Value;
            double hd = attacker.Skills[SkillName.Hiding].Value;
            double tactics = attacker.Skills[SkillName.Tactics].Value;
            double anatomy = attacker.Skills[SkillName.Anatomy].Value;

            double chance = 15.0 + ((ms / 25.0) + (hd / 25.0));
            double cooldownSeconds = (10 * 60) - (hd * 5.0);
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
            
            if (dmg < 1) dmg = 1;
            // Headshot does extra damage based on 35% of the simulated value
            int extra = (int)(dmg * 0.50);
            if (extra < 1) extra = 1;
            // Apply as physical damage
            AOS.Damage(defender, attacker, extra, 100, 0, 0, 0, 0);

            attacker.SendMessage(1161, "Headshot for " + extra+ " extra damage");
            defender.FixedParticles(0x36BD, 10, 10, 5044, 0, 0, EffectLayer.Head);
            defender.PlaySound(0x22F);
        }

		public Artifact_BowOfTheProwler( Serial serial ) : base( serial )
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