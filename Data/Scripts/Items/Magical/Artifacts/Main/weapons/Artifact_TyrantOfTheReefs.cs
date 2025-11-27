using System;
using Server;

namespace Server.Items
{
	public class Artifact_TyrantOfTheReefs : GiftExecutionersAxe
	{
		private DateTime m_NextParalyze;
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_TyrantOfTheReefs()
		{
			Name = "Tyrant of the Reefs";
			Hue = 1365;
			ItemID = 0xF45;
			Slayer = SlayerName.NeptunesBane;
			WeaponAttributes.HitLeechHits = 35;
			Attributes.WeaponSpeed = 15;
			Attributes.AttackChance = 10;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Paralyzes Water creatures." );
		}

		public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

            if (DateTime.Now < m_NextParalyze)
                return;

            bool validTarget = false;
            SlayerEntry NeptunesBane = SlayerGroup.GetEntryByName(SlayerName.NeptunesBane);
			if (NeptunesBane != null && NeptunesBane.Slays(defender))
                validTarget = true;
			if (!validTarget)
                return;

            if (Utility.RandomDouble() < 0.45)
            {
                if (defender != null && defender.Alive && !defender.Paralyzed)
                {
                    defender.Paralyze(TimeSpan.FromSeconds(6));
                    attacker.SendMessage("Your arrow immobilizes your foe!");
                    m_NextParalyze = DateTime.Now + TimeSpan.FromSeconds(30);
                }
            }
        }

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			chaos = direct = fire = pois = nrgy= 0;
			phys = cold = 50;
		}

		public Artifact_TyrantOfTheReefs( Serial serial ) : base( serial )
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