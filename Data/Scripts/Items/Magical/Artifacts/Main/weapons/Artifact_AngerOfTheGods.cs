using System;
using Server;

namespace Server.Items
{
    public class Artifact_AngeroftheGods : GiftBroadsword
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

        [Constructable]
        public Artifact_AngeroftheGods()
        {
            Name = "Anger of the Gods";
			ItemID = 0xF5E;
            Attributes.AttackChance = 15;
            WeaponAttributes.HitLowerAttack = 50;
            Hue = 1265;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Culls the prideful" );
		}

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            if (attacker == null || defender == null)
                return;

            if (defender.Hits > 0 && defender.Hits < (defender.HitsMax / 5) && defender.Fame > 5000)
            {
                int extra = (int)(defender.HitsMax * 0.25);
                if (extra < 1)
                    extra = 1;
				if (extra > 100)
					extra = 100;

                defender.Damage(extra, attacker);

                attacker.FixedParticles(0x3728, 10, 10, 5052, 0, 0, EffectLayer.Head);
                attacker.PlaySound(0x1F1);
            }

            base.OnHit(attacker, defender, damageBonus);
        }

        public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
        {
            phys = 0;
            cold = 0;
            fire = 0;
            nrgy = 100;
            pois = 0;
            chaos = 0;
            direct = 0;
        }

        public Artifact_AngeroftheGods( Serial serial ) : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
			ArtifactLevel = 2;
            int version = reader.ReadInt();
        }
    }
}
