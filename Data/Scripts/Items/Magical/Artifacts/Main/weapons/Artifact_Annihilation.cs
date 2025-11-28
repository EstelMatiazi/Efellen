using System;
using Server;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
    public class Artifact_Annihilation : GiftBardiche
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

        [Constructable]
        public Artifact_Annihilation()
        {
            Name = "Annihilation";
			Hue = 1154;
			ItemID = 0xF4D;
            Attributes.WeaponDamage = 20;
            Attributes.AttackChance = 10;
            WeaponAttributes.HitLeechHits = 30;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Eviscerates enemies" );
		}

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
		{
		    base.OnHit(attacker, defender, damageBonus);

		    if (attacker == null || defender == null || attacker.Map == null || defender.Map == null)
		        return;

		    double lumberjacking = attacker.Skills[SkillName.Lumberjacking].Value;

		    int tiles = 0;
		    int chance = 0;
		    if (lumberjacking >= 125.0) { chance = 15; tiles = 7; }
		    else if (lumberjacking >= 120.0) { chance = 10; tiles = 6; }
		    else if (lumberjacking >= 110.0) { chance = 8; tiles = 5; }
		    else if (lumberjacking >= 100.0) { chance = 6; tiles = 4; }
		    else if (lumberjacking >= 90.0) { chance = 4; tiles = 3; }
		    else if (lumberjacking >= 80.0) { chance = 2; tiles = 2; }

		    if (tiles == 0 || Utility.Random(100) >= chance)
		        return;

		    SpillBlood(attacker, defender, tiles);
		}

		private void SpillBlood(Mobile attacker, Mobile target, int amount)
		{
			if (target == null || target.Map == null)
		        return;

		    Map map = target.Map;
		    Point3D baseLoc = target.Location;

		    for (int i = 0; i < amount; i++)
		    {
		        Point3D loc = baseLoc;

		        bool valid = false;
		        for (int j = 0; !valid && j < 10; j++)
		        {
		            loc = new Point3D(
		                baseLoc.X + (Utility.Random(0, 3) - 1),
		                baseLoc.Y + (Utility.Random(0, 3) - 1),
		                baseLoc.Z
		            );

		            loc.Z = map.GetAverageZ(loc.X, loc.Y);

		            if (!map.CanFit(loc, 16, false, false))
		                continue;

		            bool occupied = false;
		            IPooledEnumerable items = map.GetItemsInRange(loc, 0);

		            foreach (Item it in items)
		            {
		                if (it is PoolOfBlood)
		                {
		                    occupied = true;
		                    break;
		                }
		            }

		            items.Free();

		            if (!occupied)
		                valid = true;
		        }

		        if (!valid)
		            continue;

		        PoolOfBlood pool = new PoolOfBlood(
		            TimeSpan.FromSeconds(10.0),
		            5, 10
		        );
		        pool.MoveToWorld(loc, map);
		    }
		}

        public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
        {
            phys = 100;
            cold = 0;
            fire = 0;
            nrgy = 0;
            pois = 0;
            chaos = 0;
            direct = 0;
        }
        public Artifact_Annihilation( Serial serial )
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
