using System;
using Server.Network;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using Server.Engines.PartySystem;
using Server.EffectsUtil;

namespace Server.Items
{
	public class Artifact_MaulOfTheTitans : GiftMaul
	{
		private DateTime m_NextAoE;
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

      [Constructable]
		public Artifact_MaulOfTheTitans()
		{
			Name = "Maul of the Titans";
			Hue = 0xB89;
			SkillBonuses.SetValues(1, SkillName.Bludgeoning, 25);
			Attributes.BonusStr = 25;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Shatters the earth" );
		}

		public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
		{
			base.OnHit(attacker, defender, damageBonus);
			if (attacker == null || defender == null)
				return;
			//requires you to be mighty to attempt to groundslam
			if (attacker.Skills[SkillName.Bludgeoning].Value > 105.0 && attacker.Str > 111)
			{
				if (DateTime.UtcNow < m_NextAoE)
    	        return;
    	    	double skill = attacker.Skills[SkillName.Bludgeoning].Value; 
    	    	double chance = 0.05 + (skill / 125.0) * 0.20; // 5% chant at 0 skill, 25% chance at 125 skill
    	    	if (Utility.RandomDouble() > chance)
    	    	    return;
    	    	double seconds = 120.0 - (skill * (90.0 / 125.0)); // 120secs cooldown at 0 skill, 30 secs cooldown at 125 skill
    	    	m_NextAoE = DateTime.UtcNow + TimeSpan.FromSeconds(seconds);
				int minDmg = attacker.Str / 13; // 13 base min damage at 150 str
		    	int maxDmg = attacker.Str / 6; // 25 base max damage at 150 str
		    	if (minDmg < 0) minDmg = 0;
		    	if (maxDmg < 0) maxDmg = 0;
		    	if (maxDmg < minDmg) maxDmg = minDmg;
    	    	foreach (Mobile mob in defender.GetMobilesInRange(8))
    	    	{
    	    	    if (mob == null || mob == attacker || mob == defender)
    	    	        continue;
					// dont slam pets/owned summoned creatures
    	    	    if (mob is BaseCreature)
    	    	    {
    	    	        BaseCreature bc = (BaseCreature)mob;
    	    	        if ((bc.Controlled && bc.ControlMaster == attacker) || (bc.Summoned && bc.SummonMaster == attacker))
    	    	            continue;
    	    	    }
    	    	    // don't slam party members
    	    	    Party attackerParty = Party.Get(attacker);
    	    	    Party mobParty = Party.Get(mob);
					if (attackerParty != null && mobParty != null && attackerParty == mobParty)
						continue;
    	    	    // dont slam guild mates
    	    	    if (attacker.Guild != null && mob.Guild != null && attacker.Guild == mob.Guild)
    	    	        continue;

					// slams targets that are closer harder
					int distance = (int)(attacker.GetDistanceToSqrt(mob));
					int bonus = 0;
					if (distance <= 1)
					    bonus = 5;
					else if (distance <= 3)
					    bonus = 4;
					else if (distance <= 5)
					    bonus = 3;
					else if (distance <= 7)
					    bonus = 1;
					else
					    bonus = 0;
    	    	   
					// groundslam!
					int dmg = Utility.RandomMinMax(minDmg + bonus, maxDmg + bonus);
					if (dmg > 0)
					{
						AOS.Damage(mob, attacker, dmg, 100, 0, 0, 0, 0);
						mob.PlaySound(0x208);
       				}
    	    	}
				attacker.SendMessage("Your Maul of the titans shatters the ground!");
				SlamVisuals.SlamVisual(attacker, 8, 0x36B0, 0x455);
			}
    	}


		public Artifact_MaulOfTheTitans( Serial serial ) : base( serial )
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
