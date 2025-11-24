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
	public class ChieftainKongor : BaseCreature
	{
       
		[Constructable]
		public ChieftainKongor()  : base(AIType.AI_Thief, FightMode.None, 10, 1, 0.4, 1.6)
		{
            InitStats( 125, 55, 65 ); 
			Name = "Kongor the Chieftain";
			Body = 0x190;
			Hue = 1420;
			Blessed = true;
            SpeechHue = 1420;
            CantWalk = false;
            Utility.AssignRandomHair( this );
            FacialHairItemID = Utility.RandomList( 0, 8254, 8255, 8256, 8257, 8267, 8268, 8269 );
            AddItem( new PlateArms() );
            AddItem( new PlateLegs() );
            AddItem( new Cloak( Utility.RandomBirdHue() ) );
            AddItem( new PlateChest() );
            AddItem( new PlateGloves());
            AddItem( new Boots( Utility.RandomBirdHue() ) );
            AddItem( new PlateHelm());
		}
	

		public override void OnDoubleClick(Mobile from)
		{
			if (!from.InRange(this.Location, 4))
			{
				from.SendMessage("You are too far away to speak to Kongor.");
				return;
			}

            if (Server.Misc.PlayerSettings.GetKeys(from, "Kongor"))
	        {
	        	this.PrivateOverheadMessage(MessageType.Regular, 1150, false, "I've done all I can for you, for now. Come back once you have quenched the hammer's thirst.", from.NetState);
	        	return;
	        }

			from.CloseGump(typeof(KongorDialogueGump));
			from.SendGump(new KongorDialogueGump(from, this));
		}

		public ChieftainKongor(Serial serial) : base(serial)
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

	public class KongorDialogueGump : Gump
	{
		private Mobile m_Player;
		private ChieftainKongor m_Kongor;

		public KongorDialogueGump(Mobile from, ChieftainKongor kongor) : base(100, 100)
		{
			m_Player = from;
			m_Kongor = kongor;

			double magicResist = from.Skills[SkillName.MagicResist].Base;
            int rawStr = from.RawStr;
			int karma = from.Karma;
			bool hasDreamstone = Server.Misc.PlayerSettings.GetKeys(from, "Dreamstone");

			string name = from.Name;
			string text = GetDialogueText(name, magicResist, karma, rawStr, hasDreamstone);

			AddBackground(0, 0, 420, 280, 9270);
			AddHtml(20, 20, 380, 200, text, true, true);
            

			if (ShouldOfferQuest(karma, magicResist, rawStr))
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

		private string GetDialogueText(string name, double magicResist, int karma, double rawStr, bool hasDreamstone)
		{
			string intro = "Hai! " + name + ". So you found our home.<br><br>" +
						   "Much have we endured far from the conforts of what you would call civilized. Eh, We have no use for that here.<br><br>"+
                           "I'm the chief of these people, and responsible for keeping them safe. I also inherited the secrets of our tenacity. You would care to learn them, eh?";

			if (!hasDreamstone)
            {
                intro += "Nah, it would do you no good. You civilized people are weak.<br><br>Soft.<br><br>"+
                "Do you disagree? Prove it, then. Bring me a dreamstone and I shall make you a weapon fit for our warriors.<br><br>";
            }
            else if (hasDreamstone)
				intro += "You have it... How did you conquer that beast? It matters not. I do not think you can tame a weapon such as ours.<br><br>";

			if (karma < 0)
			{
				return intro + "What guarantees do I have that you will not use this weapon against my people, " + name + ". I smell the wail of many a widow in you wake.<br><br>" +
					   "Begone! I shall not offer you any of my services!<br><br>";
			}

			if (karma >= 0 && (magicResist < 111.0 || rawStr < 125))
			{
				return intro + "You seem trustworthy, yet I do not think you deserve a weapon such as ours, " + name + "." +
					   "Steel yourself - Learn to resist the lure of foul magic, and grow strong by slaying many a foul beast, and I shall consider you fit for the weapons of my people.<br><br>" +
					   "I shall not offer you my services today, I'm afraid.<br><br>";
			}

			if (magicResist >= 111.0 && rawStr >= 125 && karma >= 15000)
			{
				return intro + "You will make the weapon proud, " + name + ". Very well. I shall grant you this gift.<br><br>";
			}

            if (karma > 0 && (magicResist >= 111.0 || rawStr >= 125))
			{
				return intro + "I know of your people, " + name + ".<br><br>" +
					   "I know of your kin and I do not trust them. Prove it to me — prove yourself worthy.<br><br>" +
					   "Do so and I shall give you a gift worthy of our ancestors!<br><br>";
			}
			return intro;
		}

		private bool ShouldOfferQuest(int karma, double magicResist, double rawStr)
		{
			return (karma >= 15000 && magicResist >= 111.0 && rawStr >= 125);
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			if (info.ButtonID == 1)
			{
				m_Player.CloseGump(typeof(KongorConfirmationGump));
				m_Player.SendGump(new KongorConfirmationGump(m_Player, m_Kongor));
			}
		}
	}

	public class KongorConfirmationGump : Gump
	{
		private Mobile m_Player;
		private Mobile m_Kongor;

		public KongorConfirmationGump(Mobile from, Mobile kongor) : base(100, 100)
		{
			m_Player = from;
			m_Kongor = kongor;

			AddBackground(0, 0, 350, 160, 9270);
			AddHtml(20, 20, 310, 60, "Do you want to hand your Dreamstone to Kongor?<br>You will not find another.", true, true);
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

				Item reward = new LevelWarHammerKongor(m_Player.Name);
				m_Player.AddToBackpack(reward);

				m_Player.SendMessage("Kongor takes the Dreamstone and nods eagerly.");
				m_Player.SendMessage("You are awarded the legendary Kongor's Undying Rage!");
                if ( PlayerSettings.GetKeys( m_Player, "Kongor" ) )
					{
						m_Player.PrivateOverheadMessage(MessageType.Regular, 1150, false, "Kongor already gifted you a weapon!.", m_Player.NetState);
					}
					else
					{
						PlayerSettings.SetKeys( m_Player, "Kongor", true );
                        PlayerSettings.SetKeys( m_Player,"DreamstoneUsed", true);
						m_Player.SendSound( 0x3D );
						m_Player.PrivateOverheadMessage(MessageType.Regular, 1150, false, "You aqquired Kongor's Undying Rage.", m_Player.NetState);
					}
			    }
			else
			{
				m_Player.SendMessage("You decide to keep your Dreamstone for now.");
			}
		}
	}
}
