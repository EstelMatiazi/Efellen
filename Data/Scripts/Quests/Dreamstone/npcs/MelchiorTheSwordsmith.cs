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
	public class MelchiorTheSwordsmith : BaseCreature
	{
       
		[Constructable]
		public MelchiorTheSwordsmith()  : base(AIType.AI_Thief, FightMode.None, 10, 1, 0.4, 1.6)
		{
            InitStats( 125, 55, 65 ); 
			Name = "Melchior the Swordsmith";
			Body = 0x190;
			Hue = 0;
			Blessed = true;
            SpeechHue = Utility.RandomTalkHue();
            Hue = Utility.RandomSkinHue(); 
			CantWalk = false;
            Utility.AssignRandomHair( this );
            FacialHairItemID = Utility.RandomList( 0, 8254, 8255, 8256, 8257, 8267, 8268, 8269 );
            AddItem( new Boots( Utility.RandomBirdHue() ) );
            AddItem( new GildedRobe( Utility.RandomBirdHue() ) );
            AddItem( new Cloak( Utility.RandomBirdHue() ) );
            AddItem( new Bonnet( Utility.RandomBirdHue() ) );
		}
	

		public override void OnDoubleClick(Mobile from)
		{
			if (!from.InRange(this.Location, 4))
			{
				from.SendMessage("You are too far away to speak to Melchior.");
				return;
			}

            if (Server.Misc.PlayerSettings.GetKeys(from, "Masamune"))
	        {
	        	this.PrivateOverheadMessage(MessageType.Regular, 1150, false, "I've done all I can for you, for now. Come back once you have mastered the Masamune.", from.NetState);
	        	return;
	        }

			from.CloseGump(typeof(MelchiorDialogueGump));
			from.SendGump(new MelchiorDialogueGump(from, this));
		}

		public MelchiorTheSwordsmith(Serial serial) : base(serial)
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

	public class MelchiorDialogueGump : Gump
	{
		private Mobile m_Player;
		private MelchiorTheSwordsmith m_Melchior;

		public MelchiorDialogueGump(Mobile from, MelchiorTheSwordsmith melchior) : base(100, 100)
		{
			m_Player = from;
			m_Melchior = melchior;

			double bushido = from.Skills[SkillName.Bushido].Base;
			int karma = from.Karma;
			int tithing = from.TithingPoints;
			bool hasDreamstone = Server.Misc.PlayerSettings.GetKeys(from, "Dreamstone");

			string name = from.Name;
			string text = GetDialogueText(name, bushido, karma, tithing, hasDreamstone);

			AddBackground(0, 0, 420, 280, 9270);
			AddHtml(20, 20, 380, 200, text, true, true);
            

			if (ShouldOfferQuest(karma, bushido, tithing))
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

		private string GetDialogueText(string name, double bushido, int karma, int tithing, bool hasDreamstone)
		{
			string intro = "Greetings " + name + ", I've wandered far and learned much, and now have settled in this little piece of my old home.<br><br>" +
						   "Much have I learned about the craft of war, a craft from which I grew weary. What is the need that we have of more swords?<br><br>";

			if (!hasDreamstone)
            {
                intro += "Besides, what am I to do with the crude metal of this land? No, my skills would serve no purpose unless I had the material to match. <br><br>"+
                "I would require a dreamstone, which was lost to time.<br><br>";
            }
            else if (hasDreamstone)
				intro += "You have it... You somehow found the Dream Stone...The Perfect metal. I thought all of it was lost in a begone age. <br><br>"+
                "No. It will not do anyone any good.<br><br>";

			if (karma < 0)
			{
				return intro + "I see the rot at the edges of your soul, " + name + ". You carry within you a darkness that would bend my craft into an instrument of debased vileness.<br><br>" +
					   "Begone! I shall not offer thee any of my services!";
			}

			if (karma >= 0 && bushido < 111.0)
			{
				return intro + "I see your soul at the edges of your ambition, " + name + ". You would wield a blade with a desire to do good, yet what good is there to be done that has not been thwarted a thousand times?<br><br>" +
					   "No — you have the heart, but not the skill. My blades are a thing of terror meant to cut the very souls in men in half.<br><br>" +
					   "I shall not offer you my services, I'm afraid.";
			}

			if (karma > 0 && bushido >= 111.0 && tithing >= 50000 && karma >= 15000)
			{
				return intro + "Your soul weighs like a feather, " + name + ". Very well. I shall light up the forge, one last time.";
			}

			if (karma > 0 && bushido >= 111.0)
			{
				return intro + "I see your soul at the edges of your ambition, " + name + ". You would wield a blade with a desire to do good, yet what good is there to be done that has not been thwarted a thousand times?<br><br>" +
					   "Prove it to me — prove your relentless desire to make of this world a better place.<br><br>" +
					   "Give Durama his due and practice good for its own sake, and do so of your own volition.<br><br>" +
					   "Do so and I shall light up the forge, one last time.";
			}

			return intro;
		}

		private bool ShouldOfferQuest(int karma, double bushido, int tithing)
		{
			return (karma >= 15000 && bushido >= 111.0 && tithing >= 50000);
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			if (info.ButtonID == 1)
			{
				m_Player.CloseGump(typeof(MelchiorConfirmationGump));
				m_Player.SendGump(new MelchiorConfirmationGump(m_Player, m_Melchior));
			}
		}
	}

	public class MelchiorConfirmationGump : Gump
	{
		private Mobile m_Player;
		private Mobile m_Melchior;

		public MelchiorConfirmationGump(Mobile from, Mobile melchior) : base(100, 100)
		{
			m_Player = from;
			m_Melchior = melchior;

			AddBackground(0, 0, 350, 160, 9270);
			AddHtml(20, 20, 310, 60, "Do you want to hand your Dreamstone to Melchior?<br>You will not find another. Confirming will also consume 50,000 tithing points.", true, true);
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

				if (m_Player.TithingPoints < 50000)
				{
					m_Player.SendMessage("You lack the 50,000 tithing points required.");
					return;
				}

				m_Player.TithingPoints -= 50000;

				Item reward = new LevelNoDachiMasamune(m_Player.Name);
				m_Player.AddToBackpack(reward);

				m_Player.SendMessage("Melchior takes the Dreamstone and nods solemnly.");
				m_Player.SendMessage("You are awarded the legendary Masamune!");
				m_Melchior.Say(true, "Then it is done. The forge is silent once more...");
                if ( PlayerSettings.GetKeys( m_Player, "Masamune" ) )
					{
						m_Player.PrivateOverheadMessage(MessageType.Regular, 1150, false, "Belchior already forged you the Masamune!.", m_Player.NetState);
					}
					else
					{
						PlayerSettings.SetKeys( m_Player, "Masamune", true );
                        PlayerSettings.SetKeys( m_Player,"DreamstoneUsed", true);
						m_Player.SendSound( 0x3D );
						m_Player.PrivateOverheadMessage(MessageType.Regular, 1150, false, "You aqquired the Masamune.", m_Player.NetState);
					}
			    }
			else
			{
				m_Player.SendMessage("You decide to keep your Dreamstone for now.");
			}
		}
	}
}
