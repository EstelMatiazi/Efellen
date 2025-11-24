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
	public class Soveliss : BaseCreature
	{
       
		[Constructable]
		public Soveliss()  : base(AIType.AI_Thief, FightMode.None, 10, 1, 0.4, 1.6)
		{
            InitStats( 125, 55, 65 ); 
			Name = "Soveliss the ranger";
			Body = 0x190;
			Hue = 1420;
			Blessed = true;
            SpeechHue = 1420;
            CantWalk = false;
            Utility.AssignRandomHair( this );
            AddItem( new RangerArms() );
            AddItem( new RangerLegs() );
            AddItem( new Cloak( Utility.RandomBirdHue() ) );
            AddItem( new RangerChest() );
            AddItem( new RangerGloves());
            AddItem( new Boots( Utility.RandomBirdHue() ) );
            AddItem( new FancyHood( Utility.RandomBirdHue( ) ) );
            AddItem( new Daisho());
		}
	

		public override void OnDoubleClick(Mobile from)
		{
			if (!from.InRange(this.Location, 4))
			{
				from.SendMessage("You are too far away to speak to Soveliss.");
				return;
			}

            if (Server.Misc.PlayerSettings.GetKeys(from, "Soveliss"))
	        {
	        	this.PrivateOverheadMessage(MessageType.Regular, 1150, false, "I've done all I can for you, for now. Come back once you have finished your hunt.", from.NetState);
	        	return;
	        }

			from.CloseGump(typeof(SovelissDialogueGump));
			from.SendGump(new SovelissDialogueGump(from, this));
		}

		public Soveliss(Serial serial) : base(serial)
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

	public class SovelissDialogueGump : Gump
	{
		private Mobile m_Player;
		private Soveliss m_Soveliss;

		public SovelissDialogueGump(Mobile from, Soveliss soveliss) : base(100, 100)
		{
			m_Player = from;
			m_Soveliss = soveliss;

			double tracking = from.Skills[SkillName.Tracking].Base;
            int rawAttr = (from.RawStr + from.RawDex)/2;
			int fame = from.Fame;
			bool hasDreamstone = Server.Misc.PlayerSettings.GetKeys(from, "Dreamstone");

			string name = from.Name;
			string text = GetDialogueText(name, tracking, fame, rawAttr, hasDreamstone);

			AddBackground(0, 0, 420, 280, 9270);
			AddHtml(20, 20, 380, 200, text, true, true);
            

			if (ShouldOfferQuest(fame, tracking, rawAttr))
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

		private string GetDialogueText(string name, double tracking, int fame, double rawAttr, bool hasDreamstone)
		{
			string intro = "Mae govannen! " + name + ". So you join me in my hunt.<br><br>" +
						   "For long have I stalked this land, protecting it from the many threats that surround it.<br><br>"+
                           "Are you a ranger, aswell? I have a task that I need to complete before I can safely return to my home, mellon.<br><br>";

			if (!hasDreamstone)
            {
                intro += "If you were to help me to find a Dreamstone, I could bestow upon you the weapon I was entrusted with in order to aqquire it, if you are worthy of it.<br><br>";
            }
            else if (hasDreamstone)
				intro += "You have it... In which land have you found the beast? For long have I wandered...<br><br>";

			if (fame <= 0)
			{
				return intro + "Nay! You are not ready for this! It is a symbol of my people, and it requires one to honor it properly!<br><br>";
			}

			if (fame >= 0 && (tracking < 111.0 || rawAttr < 110))
			{
				return intro + "You seem trustworthy, yet I do not think you are ready for a weapon such as this, " + name + ".<br>" +
					   "Learn the ways of the hunt, be one with the wild, and I'll consider you fit for the weapons of my people.<br><br>" +
					   "I shall not offer you my services today, I'm afraid.<br><br>";
			}

			if (tracking >= 111.0 && rawAttr >= 125 && fame >= 15000)
			{
				return intro + "You will make the weapon proud, " + name + ". Very well. I shall grant you this gift.<br><br>";
			}

            if (fame > 0 && (tracking >= 111.0 || rawAttr >= 110))
			{
				return intro + "I know of your deeds, " + name + ".<br><br>" +
					   "I know of your deeds, but not as much as I would like.<br><br>" +
					   "Hunt the beasts that haunt this land, do so and I shall give you a gift worthy of my woodland kin!<br><br>";
			}
			return intro;
		}

		private bool ShouldOfferQuest(int fame, double tracking, double rawAttr)
		{
			return (fame >= 15000 && tracking >= 111.0 && rawAttr >= 110);
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			if (info.ButtonID == 1)
			{
				m_Player.CloseGump(typeof(SovelissConfirmationGump));
				m_Player.SendGump(new SovelissConfirmationGump(m_Player, m_Soveliss));
			}
		}
	}

	public class SovelissConfirmationGump : Gump
	{
		private Mobile m_Player;
		private Mobile m_Soveliss;

		public SovelissConfirmationGump(Mobile from, Mobile soveliss) : base(100, 100)
		{
			m_Player = from;
			m_Soveliss = soveliss;

			AddBackground(0, 0, 350, 160, 9270);
			AddHtml(20, 20, 310, 60, "Do you want to hand your Dreamstone to Soveliss?<br>You will not find another.", true, true);
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

				Item reward = new LevelElvenCompositeLongbowDragonBane(m_Player.Name);
				m_Player.AddToBackpack(reward);

				m_Player.SendMessage("Soveliss takes the Dreamstone and nods eagerly.");
				m_Player.SendMessage("You are awarded the legendary Dragonbane!");
                if ( PlayerSettings.GetKeys( m_Player, "Soveliss" ) )
					{
						m_Player.PrivateOverheadMessage(MessageType.Regular, 1150, false, "Soveliss already gifted you a weapon!.", m_Player.NetState);
					}
					else
					{
						PlayerSettings.SetKeys( m_Player, "Soveliss", true );
                        PlayerSettings.SetKeys( m_Player,"DreamstoneUsed", true);
						m_Player.SendSound( 0x3D );
						m_Player.PrivateOverheadMessage(MessageType.Regular, 1150, false, "You aqquired the Dragonbane.", m_Player.NetState);
					}
			    }
			else
			{
				m_Player.SendMessage("You decide to keep your Dreamstone for now.");
			}
		}
	}
}
