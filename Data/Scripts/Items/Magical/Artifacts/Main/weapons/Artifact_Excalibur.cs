using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Spells;
using Server.Network;
using Server.Regions;
using System.Collections;
using System.Collections.Generic;
using Server.Engines.PartySystem;

namespace Server.Items
{
    public class Artifact_Excalibur : GiftClaymore
    {
        private DateTime m_LastHeal;

        public override int InitMinHits { get { return 80; } }
        public override int InitMaxHits { get { return 160; } }

        [Constructable]
        public Artifact_Excalibur()
        {
            Hue = 0x835;
            Name = "Excalibur";
            ItemID = 0x568F;
            Attributes.BonusStr = 10;
            SkillBonuses.SetValues(0, SkillName.Knightship, 20);
            Attributes.AttackChance = 10;
            ArtifactLevel = 2;
            Server.Misc.Arty.ArtySetup(this, "The holy sword of an ancient king.");
        }

        public Artifact_Excalibur(Serial serial) : base(serial)
        {
        }
        public override bool OnEquip(Mobile from)
        {
            if (from.Karma < 0)
            {
                from.SendMessage("This holy blade burns your hands and refuses to be wielded by you!");
                return false;
            }

            return base.OnEquip(from);
        }

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

            if (attacker == null || attacker.Deleted || !attacker.Player) // you never know
                return;

            if (attacker.Karma < 0)
                return;

			if (DateTime.Now < m_LastHeal + TimeSpan.FromMinutes(3.0))
                return;
				
			double knightship = attacker.Skills[SkillName.Knightship].Value;
			double karma = attacker.Karma;
            
			int chance = 10 + (int)(karma / 1000);
			if (chance > 25)
    			chance = 25;
			
			int radius = 3 + (int)(knightship / 25);
				if (radius > 8)
					radius = 8;
			
			int minHeal = 5 + (int)(knightship / 10) + (int)(karma / 1000);
			if (minHeal > 25)
			    minHeal = 25;

			int maxHeal = 10 + (int)(knightship / 8) + (int)(karma / 500);
			if (maxHeal > 55)
    			maxHeal = 55;

            if (Utility.Random(100) >= chance)
                return;

            m_LastHeal = DateTime.Now;

            ArrayList list = new ArrayList();
            Map map = attacker.Map;

            if (map == null)
                return;

            IPooledEnumerable eable = map.GetMobilesInRange(attacker.Location, radius);

            foreach (Mobile m in eable)
            {
                if (m == null || m.Deleted)
                    continue;

                bool valid = (m == attacker);

                Party party = Party.Get(attacker);
				ArrayList partyMobiles = new ArrayList();

                if (!valid && party != null)
                {
                    if (!valid && partyMobiles.Contains(m))
					    valid = true;
                }

                if (!valid && m is BaseCreature)
                {
                    BaseCreature bc = (BaseCreature)m;

                    if (!valid && bc.Controlled)
					{
					    if (partyMobiles.Contains(bc.ControlMaster))
					        valid = true;
					}
                }
				// keep rogues stealthed
				// don't try to heal the dead
				if ((m.Hidden && m != attacker) || !m.Alive)
				    continue;

                if (valid)
                    list.Add(m);
            }

            eable.Free();

            int healedCount = 0;

            foreach (Mobile m in list)
            {
                int amount = Utility.RandomMinMax(minHeal, maxHeal);
                m.Heal(amount);
                m.FixedParticles(0x375A, 1, 20, 9910, 0x47E, 3, EffectLayer.Head);
                healedCount++;
            }

            attacker.PlaySound(0x212);
            attacker.PlaySound(0x206);

            if (healedCount >= 5)
                attacker.Say("We are the legion of light!");
            else if (healedCount >= 3)
                attacker.Say("Fear no darkness, my companions!");
            else
                attacker.Say("Light protects me!");
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)2); 
            writer.Write(m_LastHeal);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            if (version >= 2)
                m_LastHeal = reader.ReadDateTime();

            ArtifactLevel = 2;
        }
    }
}
