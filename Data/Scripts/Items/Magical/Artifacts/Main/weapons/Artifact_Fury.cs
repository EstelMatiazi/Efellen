using System;
using Server;
using System.Collections;
using Server.Spells.Chivalry;

namespace Server.Items
{
    public class Artifact_Fury : GiftKatana
	{
         private Hashtable m_Cooldowns;
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

        [Constructable]
        public Artifact_Fury()
        {
            Name = "Fury";
			ItemID = 0x13FF;
            WeaponAttributes.HitFireball = 25;
            WeaponAttributes.HitLightning = 25;
            WeaponAttributes.HitHarm = 25;
            Attributes.ReflectPhysical = 25;
            Hue = 1357;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Brings forth Divine Fury" );
            m_Cooldowns = new Hashtable();
		}

        private bool CanCast(Mobile m)
        {
            if (m == null)
                return false;

            DateTime next;

            if (m_Cooldowns[m] == null)
                return true;

            try
            {
                next = (DateTime)m_Cooldowns[m];
            }
            catch
            {
                return true;
            }

            return DateTime.UtcNow >= next;
        }

        private void StartCooldown(Mobile m)
        {
            m_Cooldowns[m] = DateTime.UtcNow + TimeSpan.FromMinutes(1.0);
        }

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

            if (attacker == null || defender == null)
                return;

            if (Utility.RandomDouble() < 0.15)
            {
                if (CanCast(attacker))
                {
                    new DivineFurySpell(attacker, this).Cast();
                    StartCooldown(attacker);
                    attacker.SendMessage("Fury empowers you!");
                }
            }
        }

        public override void OnDoubleClick( Mobile from )
		{
			if (Parent != from)
            {
                from.SendMessage("You must be holding Fury to invoke its power.");
            }
            else
            {
                if (CanCast(from))
                {
                    new DivineFurySpell(from, this).Cast();
                    StartCooldown(from);
                }
                else
                    from.SendMessage("Fury is still recharging.");
            }
		}


        public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
        {
            phys = 40;
            cold = 15;
            fire = 15;
            nrgy = 15;
            pois = 15;
            chaos = 0;
            direct = 0;
        }
        public Artifact_Fury( Serial serial )
            : base( serial )
        {
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            // Save cooldowns
            writer.Write(m_Cooldowns.Count);
            foreach (DictionaryEntry entry in m_Cooldowns)
            {
                writer.Write((Mobile)entry.Key);
                writer.Write((DateTime)entry.Value);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Cooldowns = new Hashtable();

            if (version >= 1)
            {
                int count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    Mobile m = reader.ReadMobile();
                    DateTime next = reader.ReadDateTime();

                    if (m != null)
                        m_Cooldowns[m] = next;
                }
            }
            ArtifactLevel = 2;
        }
    }
}
