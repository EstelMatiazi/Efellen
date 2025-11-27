using System;
using Server;

namespace Server.Items
{
    public class Artifact_Fury : GiftKatana
	{
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
			Server.Misc.Arty.ArtySetup( this, "A blade eager of neverending war" );
		}

        public override void OnHit(Mobile attacker, Mobile defender, double damage)
        {
            base.OnHit(attacker, defender, damage);

            if (attacker == null || defender == null)
                return;

            if (!defender.Alive || defender.Hits <= 0)
            {
                if (Utility.Random(100) < 15)
                {
                    int stam = Utility.RandomMinMax(5, 25);
                    int mana = Utility.RandomMinMax(5, 15);

                    attacker.Stam += stam;
                    attacker.Mana += mana;

                    attacker.SendMessage(33, "Fury empowers you!");
                }
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
