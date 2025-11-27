using System;
using Server;

namespace Server.Items
{
	public class Artifact_NoxRangersHeavyCrossbow : GiftHeavyCrossbow
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_NoxRangersHeavyCrossbow()
		{
			Name = "Nox Ranger's Heavy Crossbow";
			Hue = 0x58C;
			ItemID = 0x13FD;
			WeaponAttributes.HitLeechStam = 40;
			Attributes.SpellChanneling = 1;
			Attributes.WeaponSpeed = 20;
			WeaponAttributes.ResistPoisonBonus = 20;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Holds bolts dripping with venom" );
		}

		public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
		{
			base.OnHit( attacker, defender, damageBonus );

			if (Utility.RandomDouble() > 0.25)
				return;
			double poisoning = attacker.Skills[SkillName.Poisoning].Value;
		    double roll = Utility.RandomDouble();
			Poison chosen = null;

    		if (poisoning < 25.0)
    		{
    		    if (roll < 0.20) chosen = Poison.Regular;
    		    else chosen = Poison.Lesser;
    		}
    		else if (poisoning < 50.0)
    		{
    		    if (roll < 0.20) chosen = Poison.Greater;
    		    else if (roll < 0.60) chosen = Poison.Regular;
    		    else chosen = Poison.Lesser;
    		}
    		else if (poisoning < 75.0)
    		{
    		    if (roll < 0.20) chosen = Poison.Deadly;
    		    else if (roll < 0.40) chosen = Poison.Greater;
    		    else chosen = Poison.Lesser;
    		}
    		else if (poisoning < 100.0)
    		{
    		    if (roll < 0.33) chosen = Poison.Deadly;
    		    else if (roll < 0.77) chosen = Poison.Greater;
    		    else chosen = Poison.Lesser;
    		}
    		else
    		{
    		    if (roll < 0.44) chosen = Poison.Deadly;
    		    else chosen = Poison.Greater;
    		}

    		if (chosen != null)
    		{
    		    defender.ApplyPoison(attacker, chosen);

    		    if (chosen == Poison.Lesser)
    		        Misc.Titles.AwardKarma(attacker, -50, true);
    		    else if (chosen == Poison.Regular)
    		        Misc.Titles.AwardKarma(attacker, -60, true);
    		    else if (chosen == Poison.Greater)
    		        Misc.Titles.AwardKarma(attacker, -70, true);
    		    else if (chosen == Poison.Deadly)
    		        Misc.Titles.AwardKarma(attacker, -80, true);
    		}
		}

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			pois = 50;
			phys = 50;

			fire = cold = nrgy = chaos = direct = 0;
		}

		public Artifact_NoxRangersHeavyCrossbow( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			ArtifactLevel = 2;

			int version = reader.ReadInt();
		}
	}
}