using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class Artifact_VampiricDaisho : GiftDaisho
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

      [Constructable]
		public Artifact_VampiricDaisho()
		{
			Name = "Vampiric Daisho";
			Hue = 1153;
			WeaponAttributes.HitHarm = 50;
			WeaponAttributes.HitLeechHits = 50;
			Attributes.SpellChanneling = 1;
			Slayer = SlayerName.BloodDrinking ;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "The thirsty daisho of a godless Ronin." );
		}

		public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
		{
		    base.OnHit(attacker, defender, damageBonus);

		    if (attacker == null || defender == null || attacker.Map == null || defender.Map == null)
		        return;

		    double Bushido = attacker.Skills[SkillName.Bushido].Value;

		    int tiles = 0;
		    int chance = 0;
		    if (Bushido >= 125.0) { chance = 15; tiles = 7; }
		    else if (Bushido >= 120.0) { chance = 10; tiles = 6; }
		    else if (Bushido >= 110.0) { chance = 8; tiles = 5; }
		    else if (Bushido >= 100.0) { chance = 6; tiles = 4; }
		    else if (Bushido >= 90.0) { chance = 4; tiles = 3; }
		    else if (Bushido >= 80.0) { chance = 2; tiles = 2; }

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

		public Artifact_VampiricDaisho( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			ArtifactLevel = 2;
			int version = reader.ReadInt();
		}
	}
}
