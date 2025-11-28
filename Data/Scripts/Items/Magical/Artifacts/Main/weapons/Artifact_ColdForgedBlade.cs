using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class Artifact_ColdForgedBlade : GiftElvenSpellblade
	{
		[Constructable]
		public Artifact_ColdForgedBlade()
		{
			Name = "Cold Forged Blade";
			WeaponAttributes.HitHarm = 40;
			Attributes.SpellChanneling = 1;
			Slayer = SlayerName.Fey;
			Attributes.WeaponSpeed = 20;

			Hue = this.GetElementalDamageHue();
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "" );
		}

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			phys = fire = pois = nrgy = chaos = direct = 0;
			cold = 100;
		}

		public Artifact_ColdForgedBlade( Serial serial ) : base( serial )
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