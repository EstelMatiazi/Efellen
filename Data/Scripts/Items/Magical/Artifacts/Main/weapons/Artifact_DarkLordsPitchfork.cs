using System;
using Server;
using Server.Spells.Eighth;
using Server.Spells;

namespace Server.Items
{
    public class Artifact_DarkLordsPitchfork : GiftPitchfork
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

        [Constructable]
        public Artifact_DarkLordsPitchfork()
        {
            Name = "Dark Lord's PitchFork";
            Hue = 1157;
            WeaponAttributes.HitFireArea = 50;
            WeaponAttributes.HitFireball = 50;
            WeaponAttributes.ResistFireBonus = 15;
            Attributes.SpellChanneling = 1;
			ArtifactLevel = 2;
			Server.Misc.Arty.ArtySetup( this, "Calls forth demons" );
		}

        public override void OnDoubleClick( Mobile from )
		{
			if ( Parent != from )
			{
				from.SendMessage( "You must be holding the pitchfork to summon a Demon." );
			}
			else
			{
				new SummonDaemonSpell( from, this ).Cast();
			}
			return;
		}

        public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
        {
            phys = 0;
            cold = 0;
            fire = 100;
            nrgy = 0;
            pois = 0;
            chaos = 0;
            direct = 0;
        }
        public Artifact_DarkLordsPitchfork( Serial serial )
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
