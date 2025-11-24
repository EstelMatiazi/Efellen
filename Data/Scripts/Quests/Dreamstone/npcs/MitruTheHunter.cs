using System;
using Server;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.Misc;

namespace Server.Mobiles
{
	public class MitruTheHunter : BaseCreature
	{
       
		[Constructable]
		public MitruTheHunter()  : base(AIType.AI_Thief, FightMode.None, 10, 1, 0.4, 1.6)
		{
            InitStats( 125, 55, 65 ); 
			Name = "Mitru the Hunter";
			Body = 0x190;
			Hue = 1040;
			Blessed = true;
            SpeechHue = 1040;
            CantWalk = false;
            Utility.AssignRandomHair( this );
            AddItem( new DoubleBladedStaff());
            AddItem( new RangerArms() );
            AddItem( new RangerLegs() );
            AddItem( new Cloak( Utility.RandomBirdHue() ) );
            AddItem( new RangerChest() );
            AddItem( new RangerGloves() );
            AddItem( new Boots( Utility.RandomBirdHue() ) );
            AddItem( new FancyHood( Utility.RandomBirdHue( ) ) );
		}
    

		public override void OnDoubleClick(Mobile from)
		{
			if (!from.InRange(this.Location, 4))
			{
				from.SendMessage("You are too far away to speak to Mitru.");
				return;
			}

            if (Server.Misc.PlayerSettings.GetKeys(from, "Mitru"))
	        {
	        	this.PrivateOverheadMessage(MessageType.Regular, 1150, false, "I've done all I can for you, for now. Come back once you and blade sing as one and the same.", from.NetState);
	        	return;
	        }

			from.CloseGump(typeof(MitruDialogueGump));
			from.SendGump(new MitruDialogueGump(from, this));
		}

		public MitruTheHunter(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}

	public class MitruDialogueGump : Gump
	{
		private Mobile m_Player;
		private MitruTheHunter m_Mitru;

		public MitruDialogueGump(Mobile from, MitruTheHunter mitru) : base(100, 100)
		{
			m_Player = from;
			m_Mitru = mitru;

			double tactics = from.Skills[SkillName.Tactics].Base;
            int rawDex = from.RawDex;
			int fame = from.Fame;
			bool hasDreamstone = Server.Misc.PlayerSettings.GetKeys(from, "Dreamstone");

			string name = from.Name;
			string text = GetDialogueText(name, tactics, fame, rawDex, hasDreamstone);

			AddBackground(0, 0, 420, 280, 9270);
			AddHtml(20, 20, 380, 200, text, true, true);
            

			if (ShouldOfferQuest(fame, tactics, rawDex))
			{
				AddButton(160, 235, 4005, 4007, 1, GumpButtonType.Reply, 0);
				AddHtml(195, 237, 100, 20, "Continue", false, false);
			}
			else
			{
				AddButton(160, 235, 4005, 4007, 0, GumpButtonType.Reply, 0);
				AddHtml(195, 237, 100, 20, "Close", false, false);
			}
		}

		private string GetDialogueText(string name, double tactics, int fame, double rawDex, bool hasDreamstone)
		{
			string intro = "Hail and well met " + name + " !.<br><br>" +
						   "For long have a heard the song - Do you hear it? The clase of blade and shield, the splatter of blood, the conquer of glory!<br><br>"+
                           "I'm but a passionate devotee of the sweet music that the strife of this land makes";

			if (!hasDreamstone)
            {
                intro += "Are you interested in joining the great song?<br><br>"+
                "Prove it to be, then. Aqquire the fabled Dreamstone from the fearsome creature that guards it, and that battle shall surely be worthy of song!<br><br>";
            }
            else if (hasDreamstone)
				intro += "You have it... You have slain the beast? It matters not how you did, what matters is that one stood against many and conquered the most daring of foes!<br><br>";

			if (fame <= 0)
			{
				return intro + "What a shameful thing it would be to bring this song to a halt, would it not be? Grow your legend, face danger ever"+ 
                " growing, and come back! I shall not entertain those not worthy of the great song!<br><br>";
			}

			if (fame >= 0 && (tactics < 111.0 || rawDex < 125))
			{
				return intro + "You seem valiant, yet I do not think you deserve a weapon such as this, " + name + "." +
					   "Steel yourself - Learn the minutiae of warfare, the trade of blood, and grow nimble by slaying many a foul beast, and I shall consider you fit for a weapon of legend!<br><br>" +
					   "I shall not offer you my services today, I'm afraid.<br><br>";
			}

			if (tactics >= 111.0 && rawDex >= 125 && fame >= 15000)
			{
				return intro + "You will make the weapon proud, " + name + ". Very well. I shall grant you this gift.<br><br>";
			}

            if (fame > 0 && (tactics >= 111.0 || rawDex >= 125))
			{
				return intro + "I know of you, " + name + ".<br><br>" +
					   "But not as much as I would like to.<br><br>" +
					   "Carve a legend for yourself in the bones of this land! Have your legend grow, and I shall give you a weapon of legend!<br><br>";
			}
			return intro;
		}

		private bool ShouldOfferQuest(int fame, double tactics, double rawDex)
		{
			return (fame >= 15000 && tactics >= 111.0 && rawDex >= 125);
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			if (info.ButtonID == 1)
			{
				m_Player.CloseGump(typeof(MitruConfirmationGump));
				m_Player.SendGump(new MitruConfirmationGump(m_Player, m_Mitru));
			}
		}
	}

	public class MitruConfirmationGump : Gump
	{
		private Mobile m_Player;
		private Mobile m_Mitru;

		public MitruConfirmationGump(Mobile from, Mobile mitru) : base(100, 100)
		{
			m_Player = from;
			m_Mitru = mitru;

			AddBackground(0, 0, 350, 160, 9270);
			AddHtml(20, 20, 310, 60, "Do you want to hand your Dreamstone to Mitru?<br>You will not find another.", true, true);
			AddButton(60, 110, 4005, 4007, 1, GumpButtonType.Reply, 0);
			AddHtml(95, 112, 60, 20, "Yes", false, false);
			AddButton(190, 110, 4005, 4007, 0, GumpButtonType.Reply, 0);
			AddHtml(225, 112, 60, 20, "No", false, false);
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			if (m_Player == null || m_Player.Deleted)
				return;

			if (info.ButtonID == 1)
			{
				bool hasUsableDreamstone = Server.Misc.PlayerSettings.GetKeys(m_Player, "Dreamstone") 
	                    && !Server.Misc.PlayerSettings.GetKeys(m_Player, "DreamstoneUsed");

                if(!hasUsableDreamstone)
                {
                	m_Player.SendMessage("You do not have a Dreamstone to give.");
                	return;
                }

				Item reward = new LevelDoubleBladedStaffMoonDancer(m_Player.Name);
				m_Player.AddToBackpack(reward);

				m_Player.SendMessage("Mitru takes the Dreamstone and nods eagerly.");
				m_Player.SendMessage("You are awarded the legendary Moon Dancer!");
                if ( PlayerSettings.GetKeys( m_Player, "Mitru" ) )
					{
						m_Player.PrivateOverheadMessage(MessageType.Regular, 1150, false, "Mitru already gifted you a weapon!.", m_Player.NetState);
					}
					else
					{
						PlayerSettings.SetKeys( m_Player, "Mitru", true );
                        PlayerSettings.SetKeys( m_Player,"DreamstoneUsed", true);
						m_Player.SendSound( 0x3D );
						m_Player.PrivateOverheadMessage(MessageType.Regular, 1150, false, "You aqquired the Moon Dancer.", m_Player.NetState);
					}
			    }
			else
			{
				m_Player.SendMessage("You decide to keep your Dreamstone for now.");
			}
		}
	}
}
