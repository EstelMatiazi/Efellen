using System;
using Server.Network;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using Server.Engines.PartySystem;
using Server.EffectsUtil;
namespace Server.Items
{
	public class Artifact_RuneCarvingKnife : GiftAssassinSpike
	{
		private DateTime m_NextAoE;
		[Constructable]
		public Artifact_RuneCarvingKnife()
		{
			Hue = 0x48D;
			Name = "Rune Carving Knife";
			ItemID = 0x2677;
			Attributes.SpellChanneling = 1;
			WeaponAttributes.HitLeechMana = 40;
			Attributes.LowerManaCost = 10;
			Attributes.WeaponSpeed = 10;
			Attributes.WeaponDamage = 10;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Herald of thunder" );
		}

		public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
		{
			base.OnHit(attacker, defender, damageBonus);
			if (attacker == null || defender == null)
				return;
			//only works for people serious about the fencer business
			if (attacker.Skills[SkillName.Fencing].Value > 105.0  && attacker.Dex > 111)
			{
				if (DateTime.UtcNow < m_NextAoE)
    	        return;
    	    	double skill = attacker.Skills[SkillName.Fencing].Value; 
    	    	double chance = 0.05 + (skill / 125.0) * 0.20; // 5% chant at 0 skill, 25% chance at 125 skill
    	    	if (Utility.RandomDouble() > chance)
    	    	    return;
    	    	double seconds = 120.0 - (skill * (90.0 / 125.0)); // 120secs cooldown at 0 skill, 30 secs cooldown at 125 skill
    	    	m_NextAoE = DateTime.UtcNow + TimeSpan.FromSeconds(seconds);
				int minDmg = attacker.Dex / 13; // 13 base min damage at 150 dex
		    	int maxDmg = attacker.Dex / 6; // 25 base max damage at 150 dex
		    	if (minDmg < 0) minDmg = 0;
		    	if (maxDmg < 0) maxDmg = 0;
		    	if (maxDmg < minDmg) maxDmg = minDmg;
    	    	foreach (Mobile mob in defender.GetMobilesInRange(8))
    	    	{
    	    	    if (mob == null || mob == attacker || mob == defender)
    	    	        continue;
					// dont shock pets/owned summoned creatures
    	    	    if (mob is BaseCreature)
    	    	    {
    	    	        BaseCreature bc = (BaseCreature)mob;
    	    	        if ((bc.Controlled && bc.ControlMaster == attacker) || (bc.Summoned && bc.SummonMaster == attacker))
    	    	            continue;
    	    	    }
    	    	    // don't shock party members
    	    	    Party attackerParty = Party.Get(attacker);
    	    	    Party mobParty = Party.Get(mob);
					if (attackerParty != null && mobParty != null && attackerParty == mobParty)
						continue;
    	    	    // dont shock guild mates
    	    	    if (attacker.Guild != null && mob.Guild != null && attacker.Guild == mob.Guild)
    	    	        continue;
    	    	   // shock targets that are closer harder
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
    	    	   
					// shock!
					int dmg = Utility.RandomMinMax(minDmg + bonus, maxDmg + bonus);
					if (dmg > 0)
					{
						AOS.Damage(mob, attacker, dmg, 0, 0, 0, 0, 100);
						mob.PlaySound(0x208);
       				}
    	    	}
    			attacker.SendMessage("Your Rune Carving Knife unleashes a shockwave!");
				SlamVisuals.SlamVisual(attacker, 8, 0x36B0, 92);
			}
    	}

		public Artifact_RuneCarvingKnife( Serial serial ) : base( serial )
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