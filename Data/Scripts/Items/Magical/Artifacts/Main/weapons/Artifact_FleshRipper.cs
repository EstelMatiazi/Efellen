using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class Artifact_FleshRipper : GiftAssassinSpike
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_FleshRipper()
		{
			Name = "Flesh Ripper";
			Hue = 0x341;
			ItemID = 0x2D21;
			Attributes.BonusDex = 10;
			Attributes.AttackChance = 25;
			Slayer = SlayerName.Repond;
			Attributes.SpellChanneling = 1;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "A vicious blade made for evisceration." );
		}

		public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
		{
		    base.OnHit(attacker, defender, damageBonus);

		    if (attacker == null || defender == null || attacker.Map == null || defender.Map == null)
		        return;

		    double anatomy = attacker.Skills[SkillName.Anatomy].Value;

		    int tiles = 0;
		    int chance = 0;
		    if (anatomy >= 125.0) { chance = 15; tiles = 7; }
		    else if (anatomy >= 120.0) { chance = 10; tiles = 6; }
		    else if (anatomy >= 110.0) { chance = 8; tiles = 5; }
		    else if (anatomy >= 100.0) { chance = 6; tiles = 4; }
		    else if (anatomy >= 90.0) { chance = 4; tiles = 3; }
		    else if (anatomy >= 80.0) { chance = 2; tiles = 2; }

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



		public Artifact_FleshRipper( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			ArtifactLevel = 2;

			int version = reader.ReadEncodedInt();
		}
	}
}