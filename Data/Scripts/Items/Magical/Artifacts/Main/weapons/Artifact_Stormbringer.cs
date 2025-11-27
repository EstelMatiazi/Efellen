using System;
using Server;
using Server.Network;

namespace Server.Items
{
    public class Artifact_Stormbringer : GiftVikingSword
    {
        private DateTime m_LastSoulHeal;

        public override int InitMinHits { get { return 80; } }
        public override int InitMaxHits { get { return 160; } }

        [Constructable]
        public Artifact_Stormbringer()
        {
            Hue = 0x76B;
            Name = "Stormbringer";
            ItemID = 0x2D00;

            WeaponAttributes.HitLeechHits = 50;
            Attributes.BonusStr = 10;
            Slayer = SlayerName.Repond;

            ArtifactLevel = 2;

            Server.Misc.Arty.ArtySetup(this, "Stormbringer is a vicious sword that fells the weak.");
        }

        public Artifact_Stormbringer(Serial serial) : base(serial)
        {
        }

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            if (attacker == null || defender == null)
                return;
            
            if (Utility.RandomDouble() < 0.05)
            {
                int baseDmg = Utility.RandomMinMax(1, 5);

                int karma = attacker.Karma;
                if (karma < 0)
                    karma = 0;

                int karmaDmg = 5 + (int)(20.0 * ((double)karma / 15000.0)); //5-25
                if (karmaDmg > 25)
                    karmaDmg = 25;

                int total = baseDmg + karmaDmg;

                attacker.Damage(total, attacker);
                attacker.PublicOverheadMessage(MessageType.Regular, 0x486, false, "*Stormbringer betrays you!*");
                attacker.PlaySound(0x1F1);
            }

            if (defender.Hits > 0 && defender.Hits < (defender.HitsMax / 5))
            {
                int extra = (int)(defender.HitsMax * 0.25);
                if (extra < 1)
                    extra = 1;
                else if (extra > 150)
                    extra = 150;

                defender.Damage(extra, attacker);

                attacker.FixedParticles(0x3728, 10, 10, 5052, 0, 0, EffectLayer.Head);
                attacker.PlaySound(0x1F1);
            }

            base.OnHit(attacker, defender, damageBonus);
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            ArtifactLevel = 2;
        }
    }
}
