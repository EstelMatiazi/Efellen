using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Menus;
using Server.Menus.Questions;
using Server.Accounting;
using Server.Multis;
using Server.Mobiles;
using Server.Regions;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;
using Server.Misc;
using Server.Items;
using System.Globalization;

namespace Server.Engines.Help
{
	public class ContainedMenu : QuestionMenu
	{
		private Mobile m_From;

		public ContainedMenu( Mobile from ) : base( "Você já tem uma solicitação de ajuda aberta. Alguém irá ajudá-lo assim que possível. O que você gostaria de fazer?", new string[]{ "Manter minha solicitação de ajuda como está.", "Remover minha solicitação de ajuda da fila." } )
		{
			m_From = from;
		}

		public override void OnCancel( NetState state )
		{
			m_From.SendLocalizedMessage( 1005306, "", 0x35 ); // Help request unchanged.
		}

		public override void OnResponse( NetState state, int index )
		{
			m_From.SendSound( 0x4A );
			if ( index == 0 )
			{
				m_From.SendLocalizedMessage( 1005306, "", 0x35 ); // Help request unchanged.
			}
			else if ( index == 1 )
			{
				PageEntry entry = PageQueue.GetEntry( m_From );

				if ( entry != null && entry.Handler == null )
				{
					m_From.SendLocalizedMessage( 1005307, "", 0x35 ); // Removed help request.
					entry.AddResponse( entry.Sender, "[Canceled]" );
					PageQueue.Remove( entry );
				}
				else
				{
					m_From.SendLocalizedMessage( 1005306, "", 0x35 ); // Help request unchanged.
				}
			}
		}
	}

	public class HelpGump : Gump
	{
		public static void Initialize()
		{
			EventSink.HelpRequest += new HelpRequestEventHandler( EventSink_HelpRequest );
		}

		private static void EventSink_HelpRequest( HelpRequestEventArgs e )
		{
			foreach ( Gump g in e.Mobile.NetState.Gumps )
			{
				if ( g is HelpGump )
					return;
			}

			if ( !PageQueue.CheckAllowedToPage( e.Mobile ) )
				return;

			if ( PageQueue.Contains( e.Mobile ) )
				e.Mobile.SendMenu( new ContainedMenu( e.Mobile ) );
			else
				e.Mobile.SendGump( new HelpGump( e.Mobile, 1 ) );
		}

		private static bool IsYoung( Mobile m )
		{
			if ( m is PlayerMobile )
				return ((PlayerMobile)m).Young;

			return false;
		}

		public static bool CheckCombat( Mobile m )
		{
			for ( int i = 0; i < m.Aggressed.Count; ++i )
			{
				AggressorInfo info = m.Aggressed[i];

				if ( DateTime.Now - info.LastCombatTime < TimeSpan.FromSeconds( 30.0 ) )
					return true;
			}

			return false;
		}

		public HelpGump( Mobile from, int page ) : base( 50, 50 )
		{
			string HelpText = MyHelp();
			string color = "#ddbc4b";
			int button = 4005;

			from.SendSound( 0x4A ); 

            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);

			int r = 40;
			int e = 30;

			AddImage(0, 0, 9548, Server.Misc.PlayerSettings.GetGumpHue( from ));
			AddHtml( 12, 12, 300, 20, @"<BODY><BASEFONT Color=" + color + ">HELP OPTIONS</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(967, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			button = 4005; if ( page == 1 ){ button = 4006; AddHtml( 252, 71, 739, 630, @"<BODY><BASEFONT Color=" + color + ">" + HelpText + "</BASEFONT></BODY>", (bool)false, (bool)true); }
			AddButton(15, r, button, button, 1, GumpButtonType.Reply, 0);
			AddHtml( 50, r, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Main</BASEFONT></BODY>", (bool)false, (bool)false);
			r=r+e;

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			button = 3609;
			HelpText = "Your 'Away From Keyboard' Settings Are Disabled.";
			if ( Server.Commands.AFK.m_AFK.Contains( from.Serial.Value ) )
			{
				button = 4018;
				HelpText = "Your 'Away From Keyboard' Settings Are Enabled.";
			}
			if ( page == 2 ){ AddHtml( 252, 71, 739, 630, @"<BODY><BASEFONT Color=" + color + ">" + HelpText + "</BASEFONT></BODY>", (bool)false, (bool)true); }
			AddButton(15, r, button, button, 2, GumpButtonType.Reply, 0);
			AddHtml( 50, r, 148, 20, @"<BODY><BASEFONT Color=" + color + ">AFK</BASEFONT></BODY>", (bool)false, (bool)false);
			r=r+e;

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			button = 4005; if ( page == 3 ){ button = 4006; }
			AddButton(15, r, button, button, 3, GumpButtonType.Reply, 0);
			AddHtml( 50, r, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Chat</BASEFONT></BODY>", (bool)false, (bool)false);
			r=r+e;

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			button = 4005; if ( page == 18 ){ button = 4006; }
			AddButton(15, r, button, button, 18, GumpButtonType.Reply, 0);
			AddHtml( 50, r, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Conversations</BASEFONT></BODY>", (bool)false, (bool)false);
			r=r+e;

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			button = 4005; if ( page == 4 ){ button = 4006; AddHtml( 252, 71, 739, 630, @"<BODY><BASEFONT Color=" + color + ">Your empty corpses have been removed.</BASEFONT></BODY>", (bool)false, (bool)true); }
			AddButton(15, r, button, button, 4, GumpButtonType.Reply, 0);
			AddHtml( 50, r, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Corpse Clear</BASEFONT></BODY>", (bool)false, (bool)false);
			r=r+e;

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			button = 4005; if ( page == 5 ){ button = 4006; }
			AddButton(15, r, button, button, 5, GumpButtonType.Reply, 0);
			AddHtml( 50, r, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Corpse Search</BASEFONT></BODY>", (bool)false, (bool)false);
			r=r+e;

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			button = 4005; if ( page == 6 ){ button = 4006; }
			AddButton(15, r, button, button, 6, GumpButtonType.Reply, 0);
			AddHtml( 50, r, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Emote</BASEFONT></BODY>", (bool)false, (bool)false);
			r=r+e;

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			button = 4005; if ( page == 13 ){ button = 4006; }
			AddButton(15, r, button, button, 13, GumpButtonType.Reply, 0);
			AddHtml( 50, r, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Library</BASEFONT></BODY>", (bool)false, (bool)false);
			r=r+e;

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			button = 4005; if ( page == 7 ){ button = 4006; }
			AddButton(15, r, button, button, 7, GumpButtonType.Reply, 0);
			AddHtml( 50, r, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Magic Toolbars</BASEFONT></BODY>", (bool)false, (bool)false);
			r=r+e;
			if ( page == 7 )
			{
				int barS = 40;
				int barM = 30;

				AddButton(904, 10, 3610, 3610, 95, GumpButtonType.Reply, 0);

				AddButton(245, barS, 4005, 4005, 1081, GumpButtonType.Reply, 0);
				AddHtml( 280, barS, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Ancient Spell Bar I</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(605, barS, 4005, 4005, 384, GumpButtonType.Reply, 0);
				AddHtml( 640, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Open Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(800, barS, 4020, 4020, 385, GumpButtonType.Reply, 0);
				AddHtml( 835, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Close Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				barS = barS + barM;

				AddButton(245, barS, 4005, 4005, 1082, GumpButtonType.Reply, 0);
				AddHtml( 280, barS, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Ancient Spell Bar II</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(605, barS, 4005, 4005, 386, GumpButtonType.Reply, 0);
				AddHtml( 640, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Open Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(800, barS, 4020, 4020, 387, GumpButtonType.Reply, 0);
				AddHtml( 835, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Close Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				barS = barS + barM;

				AddButton(245, barS, 4005, 4005, 1083, GumpButtonType.Reply, 0);
				AddHtml( 280, barS, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Ancient Spell Bar III</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(605, barS, 4005, 4005, 388, GumpButtonType.Reply, 0);
				AddHtml( 640, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Open Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(800, barS, 4020, 4020, 389, GumpButtonType.Reply, 0);
				AddHtml( 835, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Close Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				barS = barS + barM;

				AddButton(245, barS, 4005, 4005, 1084, GumpButtonType.Reply, 0);
				AddHtml( 280, barS, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Ancient Spell Bar IV</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(605, barS, 4005, 4005, 390, GumpButtonType.Reply, 0);
				AddHtml( 640, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Open Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(800, barS, 4020, 4020, 391, GumpButtonType.Reply, 0);
				AddHtml( 835, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Close Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				barS = barS + barM;

				AddButton(245, barS, 4005, 4005, 66, GumpButtonType.Reply, 0);
				AddHtml( 280, barS, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Bard Songs Bar I</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(605, barS, 4005, 4005, 266, GumpButtonType.Reply, 0);
				AddHtml( 640, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Open Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(800, barS, 4020, 4020, 366, GumpButtonType.Reply, 0);
				AddHtml( 835, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Close Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				barS = barS + barM;

				AddButton(245, barS, 4005, 4005, 67, GumpButtonType.Reply, 0);
				AddHtml( 280, barS, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Bard Songs Bar II</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(605, barS, 4005, 4005, 267, GumpButtonType.Reply, 0);
				AddHtml( 640, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Open Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(800, barS, 4020, 4020, 367, GumpButtonType.Reply, 0);
				AddHtml( 835, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Close Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				barS = barS + barM;

				AddButton(245, barS, 4005, 4005, 68, GumpButtonType.Reply, 0);
				AddHtml( 280, barS, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Knight Spell Bar I</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(605, barS, 4005, 4005, 268, GumpButtonType.Reply, 0);
				AddHtml( 640, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Open Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(800, barS, 4020, 4020, 368, GumpButtonType.Reply, 0);
				AddHtml( 835, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Close Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				barS = barS + barM;

				AddButton(245, barS, 4005, 4005, 69, GumpButtonType.Reply, 0);
				AddHtml( 280, barS, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Knight Spell Bar II</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(605, barS, 4005, 4005, 269, GumpButtonType.Reply, 0);
				AddHtml( 640, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Open Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(800, barS, 4020, 4020, 369, GumpButtonType.Reply, 0);
				AddHtml( 835, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Close Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				barS = barS + barM;

				AddButton(245, barS, 4005, 4005, 70, GumpButtonType.Reply, 0);
				AddHtml( 280, barS, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Death Knight Spell Bar I</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(605, barS, 4005, 4005, 270, GumpButtonType.Reply, 0);
				AddHtml( 640, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Open Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(800, barS, 4020, 4020, 370, GumpButtonType.Reply, 0);
				AddHtml( 835, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Close Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				barS = barS + barM;

				AddButton(245, barS, 4005, 4005, 71, GumpButtonType.Reply, 0);
				AddHtml( 280, barS, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Death Knight Spell Bar II</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(605, barS, 4005, 4005, 271, GumpButtonType.Reply, 0);
				AddHtml( 640, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Open Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(800, barS, 4020, 4020, 371, GumpButtonType.Reply, 0);
				AddHtml( 835, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Close Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				barS = barS + barM;

				AddButton(245, barS, 4005, 4005, 978, GumpButtonType.Reply, 0);
				AddHtml( 280, barS, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Elemental Spell Bar I</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(605, barS, 4005, 4005, 282, GumpButtonType.Reply, 0);
				AddHtml( 640, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Open Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(800, barS, 4020, 4020, 382, GumpButtonType.Reply, 0);
				AddHtml( 835, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Close Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				barS = barS + barM;

				AddButton(245, barS, 4005, 4005, 979, GumpButtonType.Reply, 0);
				AddHtml( 280, barS, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Elemental Spell Bar II</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(605, barS, 4005, 4005, 283, GumpButtonType.Reply, 0);
				AddHtml( 640, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Open Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(800, barS, 4020, 4020, 383, GumpButtonType.Reply, 0);
				AddHtml( 835, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Close Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				barS = barS + barM;

				AddButton(245, barS, 4005, 4005, 72, GumpButtonType.Reply, 0);
				AddHtml( 280, barS, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Magery Spell Bar I</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(605, barS, 4005, 4005, 272, GumpButtonType.Reply, 0);
				AddHtml( 640, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Open Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(800, barS, 4020, 4020, 372, GumpButtonType.Reply, 0);
				AddHtml( 835, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Close Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				barS = barS + barM;

				AddButton(245, barS, 4005, 4005, 73, GumpButtonType.Reply, 0);
				AddHtml( 280, barS, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Magery Spell Bar II</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(605, barS, 4005, 4005, 273, GumpButtonType.Reply, 0);
				AddHtml( 640, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Open Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(800, barS, 4020, 4020, 373, GumpButtonType.Reply, 0);
				AddHtml( 835, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Close Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				barS = barS + barM;

				AddButton(245, barS, 4005, 4005, 74, GumpButtonType.Reply, 0);
				AddHtml( 280, barS, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Magery Spell Bar III</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(605, barS, 4005, 4005, 274, GumpButtonType.Reply, 0);
				AddHtml( 640, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Open Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(800, barS, 4020, 4020, 374, GumpButtonType.Reply, 0);
				AddHtml( 835, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Close Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				barS = barS + barM;

				AddButton(245, barS, 4005, 4005, 75, GumpButtonType.Reply, 0);
				AddHtml( 280, barS, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Magery Spell Bar IV</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(605, barS, 4005, 4005, 275, GumpButtonType.Reply, 0);
				AddHtml( 640, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Open Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(800, barS, 4020, 4020, 375, GumpButtonType.Reply, 0);
				AddHtml( 835, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Close Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				barS = barS + barM;

				AddButton(245, barS, 4005, 4005, 980, GumpButtonType.Reply, 0);
				AddHtml( 280, barS, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Monk Ability Bar I</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(605, barS, 4005, 4005, 280, GumpButtonType.Reply, 0);
				AddHtml( 640, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Open Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(800, barS, 4020, 4020, 380, GumpButtonType.Reply, 0);
				AddHtml( 835, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Close Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				barS = barS + barM;

				AddButton(245, barS, 4005, 4005, 981, GumpButtonType.Reply, 0);
				AddHtml( 280, barS, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Monk Ability Bar II</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(605, barS, 4005, 4005, 281, GumpButtonType.Reply, 0);
				AddHtml( 640, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Open Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(800, barS, 4020, 4020, 381, GumpButtonType.Reply, 0);
				AddHtml( 835, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Close Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				barS = barS + barM;

				AddButton(245, barS, 4005, 4005, 76, GumpButtonType.Reply, 0);
				AddHtml( 280, barS, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Necromancer Spell Bar I</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(605, barS, 4005, 4005, 276, GumpButtonType.Reply, 0);
				AddHtml( 640, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Open Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(800, barS, 4020, 4020, 376, GumpButtonType.Reply, 0);
				AddHtml( 835, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Close Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				barS = barS + barM;

				AddButton(245, barS, 4005, 4005, 77, GumpButtonType.Reply, 0);
				AddHtml( 280, barS, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Necromancer Spell Bar II</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(605, barS, 4005, 4005, 277, GumpButtonType.Reply, 0);
				AddHtml( 640, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Open Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(800, barS, 4020, 4020, 377, GumpButtonType.Reply, 0);
				AddHtml( 835, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Close Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				barS = barS + barM;

				AddButton(245, barS, 4005, 4005, 78, GumpButtonType.Reply, 0);
				AddHtml( 280, barS, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Priest Prayer Bar I</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(605, barS, 4005, 4005, 278, GumpButtonType.Reply, 0);
				AddHtml( 640, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Open Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(800, barS, 4020, 4020, 378, GumpButtonType.Reply, 0);
				AddHtml( 835, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Close Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				barS = barS + barM;

				AddButton(245, barS, 4005, 4005, 79, GumpButtonType.Reply, 0);
				AddHtml( 280, barS, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Priest Prayer Bar II</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(605, barS, 4005, 4005, 279, GumpButtonType.Reply, 0);
				AddHtml( 640, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Open Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(800, barS, 4020, 4020, 379, GumpButtonType.Reply, 0);
				AddHtml( 835, barS, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Close Toolbar</BASEFONT></BODY>", (bool)false, (bool)false);
				barS = barS + barM;
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			button = 4005; if ( page == 8 ){ button = 4006; }
			AddButton(15, r, button, button, 8, GumpButtonType.Reply, 0);
			AddHtml( 50, r, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Moongate Search</BASEFONT></BODY>", (bool)false, (bool)false);
			r=r+e;

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


			button = 4005; if ( page == 9 ){ button = 4006; }
			AddButton(15, r, button, button, 9, GumpButtonType.Reply, 0);
			AddHtml( 50, r, 148, 20, @"<BODY><BASEFONT Color=" + color + ">MOTD</BASEFONT></BODY>", (bool)false, (bool)false);
			r=r+e;

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			button = 4005; if ( page == 10 )
			{
				button = 4006;
				AddHtml( 252, 71, 739, 630, @"<BODY><BASEFONT Color=" + color + ">Ao longo de sua jornada, você pode encontrar eventos particulares que aparecem em seu registro de missões. Eles podem ser uma simples conquista de encontrar uma terra estranha, ou podem fazer referência a um item que você deve encontrar. As missões são tratadas de forma 'virtual'. O que isso significa é que quaisquer conquistas são reais, mas quaisquer referências a itens encontrados não são. Se seu registro de missões afirma que você encontrou uma chave de ébano, você não terá uma chave de ébano em sua mochila... mas você terá o item 'virtualmente'. A missão manterá o registro desse fato para você. Por causa disso, você nunca perderá essa chave de ébano e ela permanecerá única para a missão do seu personagem. A missão sabe que você a encontrou e a possui. Você pode ser encarregado de encontrar um item em uma masmorra. Quando houver uma indicação de que você o encontrou, ele estará 'virtualmente' em sua posse. Você ouvirá frequentemente um som de vitória quando um evento de missão for alcançado, junto com uma mensagem sobre isso. Você ainda pode perdê-lo, no entanto. Então, verifique seu registro de missões de tempos em tempos. Uma maneira de obter missões é visitar tavernas ou estalagens. Se você vir um mural chamado 'Procurando Aventureiros Corajosos', clique uma vez nele para começar sua vida de missões em busca de fama e fortuna.<BR><BR>" + MyQuests( from ) + "<BR><BR></BASEFONT></BODY>", (bool)false, (bool)true);
			}
			AddButton(15, r, button, button, 10, GumpButtonType.Reply, 0);
			AddHtml( 50, r, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Quests</BASEFONT></BODY>", (bool)false, (bool)false);
			r=r+e;

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			button = 4005; if ( page == 11 ){ button = 4006; }
			AddButton(15, r, button, button, 11, GumpButtonType.Reply, 0);
			AddHtml( 50, r, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Quick Bar</BASEFONT></BODY>", (bool)false, (bool)false);
			r=r+e;

			button = 4005; if ( page == 62 ){ button = 4006; }
			AddButton(15, r, button, button, 62, GumpButtonType.Reply, 0);
			AddHtml( 50, r, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Reagent Bar</BASEFONT></BODY>", (bool)false, (bool)false);
			r=r+e;

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


			button = 4005; if ( page == 12 ){ button = 4006; }
			AddButton(15, r, button, button, 12, GumpButtonType.Reply, 0);
			AddHtml( 50, r, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Settings</BASEFONT></BODY>", (bool)false, (bool)false);
			r=r+e;
			if ( page == 12 )
			{
				int g = 70;
				int j = 30;
				int setB = 3609;
				int xm = 245;
				int xo = 700;
				int xr = 0;
				int xs = 245;

				if ( !from.NoAutoAttack ){ setB = 4018; } else { setB = 3609; }
				AddButton(xs, g, setB, setB, 61, GumpButtonType.Reply, 0);
				AddButton(xs+40, g, 4011, 4011, 89, GumpButtonType.Reply, 0);
				AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Auto Attack</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( xr == 1 ){ g=g+j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				if ( ((PlayerMobile)from).CharacterSheath == 1 ){ setB = 4018; } else { setB = 3609; }
				AddButton(xs, g, setB, setB, 52, GumpButtonType.Reply, 0);
				AddButton(xs+40, g, 4011, 4011, 100, GumpButtonType.Reply, 0);
				AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Auto Sheath</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( xr == 1 ){ g=g+j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				if ( ((PlayerMobile)from).ClassicPoisoning == 1 ){ setB = 4018; } else { setB = 3609; }
				AddButton(xs, g, setB, setB, 64, GumpButtonType.Reply, 0);
				AddButton(xs+40, g, 4011, 4011, 86, GumpButtonType.Reply, 0);
				AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Classic Poisoning</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( xr == 1 ){ g=g+j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				if ( from.RaceID > 0 && (from.Region).Name == "the Tavern" && Server.Items.BaseRace.GetMonsterMage( from.RaceID ) )
				{
					string magic = "Default";
					if ( from.RaceMagicSchool == 1 ){ magic = "Magery"; }
					else if ( from.RaceMagicSchool == 2 ){ magic = "Necromancy"; }
					else if ( from.RaceMagicSchool == 3 ){ magic = "Elementalism"; }
					AddButton(xs, g, 4005, 4005, 989, GumpButtonType.Reply, 0);
					AddButton(xs+40, g, 4011, 4011, 103, GumpButtonType.Reply, 0);
					AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Creature Magic (" + magic + ")</BASEFONT></BODY>", (bool)false, (bool)false);
					if ( xr == 1 ){ g=g+j; xr=0; xs=xm; } else { xr=1; xs=xo; }
				}

				if ( from.RaceID > 0 )
				{
					if ( from.RaceMakeSounds ){ setB = 4018; } else { setB = 3609; }
					AddButton(xs, g, setB, setB, 991, GumpButtonType.Reply, 0);
					AddButton(xs+40, g, 4011, 4011, 105, GumpButtonType.Reply, 0);
					AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Creature Sounds</BASEFONT></BODY>", (bool)false, (bool)false);

					if ( xr == 1 ){ g=g+j; xr=0; xs=xm; } else { xr=1; xs=xo; }
				}

				if ( from.RaceID > 0 
					&& (
					(from.Region).Name == "the Tavern" ||
					( from.Map == Map.Sosaria && from.X >= 6982 && from.Y >= 694 && from.X <= 6999 && from.Y <= 713 )
				))
				{
					AddButton(xs, g, 4005, 4005, 990, GumpButtonType.Reply, 0);
					AddButton(xs+40, g, 4011, 4011, 104, GumpButtonType.Reply, 0);
					AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Creature Type</BASEFONT></BODY>", (bool)false, (bool)false);

					if ( xr == 1 ){ g=g+j; xr=0; xs=xm; } else { xr=1; xs=xo; }
				}

				if ( MySettings.S_AllowCustomTitles )
				{
					AddButton(xs, g, 4005, 4005, 80, GumpButtonType.Reply, 0);
					AddButton(xs+40, g, 4011, 4011, 97, GumpButtonType.Reply, 0);
					AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Custom Title</BASEFONT></BODY>", (bool)false, (bool)false);

					if ( xr == 1 ){ g=g+j; xr=0; xs=xm; } else { xr=1; xs=xo; }
				}

				if ( ((PlayerMobile)from).GumpHue > 0 ){ setB = 4018; } else { setB = 3609; }
				AddButton(xs, g, setB, setB, 985, GumpButtonType.Reply, 0);
				AddButton(xs+40, g, 4011, 4011, 101, GumpButtonType.Reply, 0);
				AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Gump Images</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( xr == 1 ){ g=g+j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddButton(xs, g, 4005, 4005, 55, GumpButtonType.Reply, 0);
				AddButton(xs+40, g, 4011, 4011, 85, GumpButtonType.Reply, 0);
				AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Loot Options</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( xr == 1 ){ g=g+j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				if ( from.RainbowMsg ){ setB = 4018; } else { setB = 3609; }
				AddButton(xs, g, setB, setB, 60, GumpButtonType.Reply, 0);
				AddButton(xs+40, g, 4011, 4011, 88, GumpButtonType.Reply, 0);
				AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Message Colors</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( xr == 1 ){ g=g+j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddButton(xs, g, 4005, 4005, 65, GumpButtonType.Reply, 0);
				AddButton(xs+40, g, 4011, 4011, 83, GumpButtonType.Reply, 0);
				AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Music Playlist</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( xr == 1 ){ g=g+j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				if ( ((PlayerMobile)from).CharMusical == "Forest" ){ setB = 4018; } else { setB = 3609; }
				AddButton(xs, g, setB, setB, 53, GumpButtonType.Reply, 0);
				AddButton(xs+40, g, 4011, 4011, 82, GumpButtonType.Reply, 0);
				AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Music Tone</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( xr == 1 ){ g=g+j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				if ( ((PlayerMobile)from).PublicInfo == false ){ setB = 4018; } else { setB = 3609; }
				AddButton(xs, g, setB, setB, 54, GumpButtonType.Reply, 0);
				AddButton(xs+40, g, 4011, 4011, 84, GumpButtonType.Reply, 0);
				AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Private Play</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( xr == 1 ){ g=g+j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddButton(xs, g, 4005, 4005, 56, GumpButtonType.Reply, 0);
				AddButton(xs+40, g, 4011, 4011, 87, GumpButtonType.Reply, 0);
				AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Skill Title</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( xr == 1 ){ g=g+j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				string skillLocks = "Skill List (Show Up)"; 
				if ( ((PlayerMobile)from).SkillDisplay == 1 ){ setB = 4018; skillLocks = "Skill List (Show Up and Locked)"; } else { setB = 4017; }
				AddButton(xs, g, setB, setB, 982, GumpButtonType.Reply, 0);
				AddButton(xs+40, g, 4011, 4011, 199, GumpButtonType.Reply, 0);
				AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">" + skillLocks + "</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( xr == 1 ){ g=g+j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				if ( ((PlayerMobile)from).WeaponBarOpen > 0 ){ setB = 4018; } else { setB = 3609; }
				AddButton(xs, g, setB, setB, 986, GumpButtonType.Reply, 0);
				AddButton(xs+40, g, 4011, 4011, 102, GumpButtonType.Reply, 0);
				AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Weapon Ability Bar</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( xr == 1 ){ g=g+j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddButton(xs, g, 4005, 4005, 5001, GumpButtonType.Reply, 0);
				AddButton(xs+40, g, 4011, 4011, 107, GumpButtonType.Reply, 0);
				AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Set Crafting Container</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( xr == 1 ){ g=g+j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				if ( ((PlayerMobile)from).CharacterWepAbNames == 1 ){ setB = 4018; } else { setB = 3609; }
				AddButton(xs, g, setB, setB, 51, GumpButtonType.Reply, 0);
				AddButton(xs+40, g, 4011, 4011, 99, GumpButtonType.Reply, 0);
				AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Weapon Ability Names</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( xr == 1 ){ g=g+j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddButton(xs, g, 4005, 4005, 5002, GumpButtonType.Reply, 0);
				AddButton(xs+40, g, 4011, 4011, 108, GumpButtonType.Reply, 0);
				AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Set Harvesting Container</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( xr == 1 ){ g=g+j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				if ( ResearchSettings.BookCaster( from ) ){ setB = 4018; } else { setB = 3609; }
				AddButton(xs, g, setB, setB, 63, GumpButtonType.Reply, 0);
				AddButton(xs+40, g, 4011, 4011, 106, GumpButtonType.Reply, 0);
				AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Use Ancient Spellbook</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( xr == 1 ){ g=g+j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddButton(xs, g, 4005, 4005, 5000, GumpButtonType.Reply, 0);
				AddButton(xs+40, g, 4011, 4011, 109, GumpButtonType.Reply, 0);
				AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Set Loot Container</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( xr == 1 ){ g=g+j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				if ( ((PlayerMobile)from).DoubleClickID ){ setB = 4018; } else { setB = 3609; }
				AddButton(xs, g, setB, setB, 5003, GumpButtonType.Reply, 0);
				AddButton(xs+40, g, 4011, 4011, 111, GumpButtonType.Reply, 0);
				AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Double Click to ID Items</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( xr == 1 ){ g=g+j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				if ( from.HarvestOrdinary ){ setB = 4018; } else { setB = 3609; }
				AddButton(xs, g, setB, setB, 49, GumpButtonType.Reply, 0);
				AddButton(xs+40, g, 4011, 4011, 110, GumpButtonType.Reply, 0);
				AddHtml( xs+80, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Ordinary Resources</BASEFONT></BODY>", (bool)false, (bool)false);

				g=g+j;
				g=g+j;

				AddHtml( 325, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Play Styles</BASEFONT></BODY>", (bool)false, (bool)false);
				g=g+j;

				if ( ((PlayerMobile)from).CharacterEvil == 0 && ((PlayerMobile)from).CharacterOriental == 0 && ((PlayerMobile)from).CharacterBarbaric == 0 ){ setB = 4018; } else { setB = 3609; }
				AddButton(325, g, setB, setB, 57, GumpButtonType.Reply, 0);
				AddButton(370, g, 4011, 4011, 92, GumpButtonType.Reply, 0);
				AddHtml( 410, g, 65, 20, @"<BODY><BASEFONT Color=" + color + ">Normal</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( ((PlayerMobile)from).CharacterEvil == 1 ){ setB = 4018; } else { setB = 3609; }
				AddButton(535, g, setB, setB, 58, GumpButtonType.Reply, 0);
				AddButton(575, g, 4011, 4011, 93, GumpButtonType.Reply, 0);
				AddHtml( 620, g, 65, 20, @"<BODY><BASEFONT Color=" + color + ">Evil</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( ((PlayerMobile)from).CharacterOriental == 1 ){ setB = 4018; } else { setB = 3609; }
				AddButton(745, g, setB, setB, 59, GumpButtonType.Reply, 0);
				AddButton(785, g, 4011, 4011, 94, GumpButtonType.Reply, 0);
				AddHtml( 830, g, 65, 20, @"<BODY><BASEFONT Color=" + color + ">Oriental</BASEFONT></BODY>", (bool)false, (bool)false);

				g=g+j;

				string amazon = "";
				if ( ((PlayerMobile)from).CharacterBarbaric == 1 ){ setB = 4018; } 
				else if ( ((PlayerMobile)from).CharacterBarbaric == 2 ){ setB = 4003; amazon = " with Amazon Fighting Titles"; } 
				else { setB = 3609; }
				AddButton(325, g, setB, setB, 984, GumpButtonType.Reply, 0);
				AddButton(370, g, 4011, 4011, 198, GumpButtonType.Reply, 0);
				AddHtml( 410, g, 300, 20, @"<BODY><BASEFONT Color=" + color + ">Barbaric" + amazon + "</BASEFONT></BODY>", (bool)false, (bool)false);

				g=g+j;
				g=g+j;

				AddButton(285, g, 4011, 4011, 96, GumpButtonType.Reply, 0);
				AddHtml( 325, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Magery Spell Color</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( ((PlayerMobile)from).MagerySpellHue == 0x47E ){ setB = 4018; } else { setB = 3609; }
				AddHtml( 565, g, 61, 20, @"<BODY><BASEFONT Color=" + color + ">White</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(523, g, setB, setB, 500, GumpButtonType.Reply, 0);

				if ( ((PlayerMobile)from).MagerySpellHue == 0x94E ){ setB = 4018; } else { setB = 3609; }
				AddHtml( 685, g, 61, 20, @"<BODY><BASEFONT Color=" + color + ">Black</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(643, g, setB, setB, 501, GumpButtonType.Reply, 0);

				if ( ((PlayerMobile)from).MagerySpellHue == 0x48D ){ setB = 4018; } else { setB = 3609; }
				AddHtml( 805, g, 61, 20, @"<BODY><BASEFONT Color=" + color + ">Blue</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(764, g, setB, setB, 502, GumpButtonType.Reply, 0);

				if ( ((PlayerMobile)from).MagerySpellHue == 0x48E ){ setB = 4018; } else { setB = 3609; }
				AddHtml( 925, g, 61, 20, @"<BODY><BASEFONT Color=" + color + ">Red</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(883, g, setB, setB, 503, GumpButtonType.Reply, 0);

				g=g+j;

				if ( ((PlayerMobile)from).MagerySpellHue == 0x48F ){ setB = 4018; } else { setB = 3609; }
				AddHtml( 565, g, 61, 20, @"<BODY><BASEFONT Color=" + color + ">Green</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(523, g, setB, setB, 504, GumpButtonType.Reply, 0);

				if ( ((PlayerMobile)from).MagerySpellHue == 0x490 ){ setB = 4018; } else { setB = 3609; }
				AddHtml( 685, g, 61, 20, @"<BODY><BASEFONT Color=" + color + ">Purple</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(643, g, setB, setB, 505, GumpButtonType.Reply, 0);

				if ( ((PlayerMobile)from).MagerySpellHue == 0x491 ){ setB = 4018; } else { setB = 3609; }
				AddHtml( 805, g, 61, 20, @"<BODY><BASEFONT Color=" + color + ">Yellow</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(764, g, setB, setB, 506, GumpButtonType.Reply, 0);

				if ( ((PlayerMobile)from).MagerySpellHue == 0 ){ setB = 4018; } else { setB = 3609; }
				AddHtml( 925, g, 61, 20, @"<BODY><BASEFONT Color=" + color + ">Default</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(883, g, setB, setB, 507, GumpButtonType.Reply, 0);

				g=g+j;
				g=g+j;
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			button = 4005; if ( page == 983 ){ button = 4006; }
			AddButton(15, r, button, button, 983, GumpButtonType.Reply, 0);
			AddHtml( 50, r, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Skill List</BASEFONT></BODY>", (bool)false, (bool)false);
			r=r+e;

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			button = 4005; if ( page == 14 ){ button = 4006; }
			AddButton(15, r, button, button, 14, GumpButtonType.Reply, 0);
			AddHtml( 50, r, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Statistics</BASEFONT></BODY>", (bool)false, (bool)false);
			r=r+e;

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			bool house = false;
			if ( from.Region is HouseRegion )
			    if (((HouseRegion)from.Region).House.IsOwner(from))
					house = true;
			if ( from.Region.GetLogoutDelay( from ) != TimeSpan.Zero && house == false && !( from.Region is SkyHomeDwelling ) && !( from.Region is PrisonArea ) && !( from.Region is DungeonHomeRegion ) && !( from.Region is GargoyleRegion ) && !( from.Region is SafeRegion ) )
			{
				button = 4005; if ( page == 15 ){ button = 4006; }
				AddButton(15, r, button, button, 15, GumpButtonType.Reply, 0);
				AddHtml( 50, r, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Stuck in World</BASEFONT></BODY>", (bool)false, (bool)false);
				r=r+e;
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			HelpText = Server.Misc.ChangeLog.Versions();
			button = 4005; if ( page == 19 ){ button = 4006; AddHtml( 252, 71, 739, 630, @"<BODY><BASEFONT Color=" + color + ">" + HelpText + "</BASEFONT></BODY>", (bool)false, (bool)true); }
			AddButton(15, r, button, button, 19, GumpButtonType.Reply, 0);
			AddHtml( 50, r, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Version</BASEFONT></BODY>", (bool)false, (bool)false);
			r=r+e;

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			button = 4005; if ( page == 17 ){ button = 4006; }
			AddButton(15, r, button, button, 17, GumpButtonType.Reply, 0);
			AddHtml( 50, r, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Wealth Bar</BASEFONT></BODY>", (bool)false, (bool)false);
			r=r+e;

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			button = 4005; if ( page == 16 ){ button = 4006; }
			AddButton(15, r, button, button, 16, GumpButtonType.Reply, 0);
			AddHtml( 50, r, 148, 20, @"<BODY><BASEFONT Color=" + color + ">Weapon Abilities</BASEFONT></BODY>", (bool)false, (bool)false);
			r=r+e;
		}

        public void InvokeCommand( string c, Mobile from )
        {
            CommandSystem.Handle(from, String.Format("{0}{1}", CommandSystem.Prefix, c));
        }

		public override void OnResponse( NetState state, RelayInfo info )
		{
			int pressed = info.ButtonID;

			Mobile from = state.Mobile;

			from.SendSound( 0x4A ); 

			from.CloseGump( typeof(Server.Engines.Help.HelpGump) );

			int box = 0;
			if ( pressed == 5000 || pressed == 5001 || pressed == 5002 )
			{
				if ( pressed == 5000 )
					box = 1;
				else if ( pressed == 5001 )
					box = 2;
				else if ( pressed == 5002 )
					box = 3;

				pressed = 50;
			}
			else if ( pressed == 5003 )
			{
				if ( ((PlayerMobile)from).DoubleClickID )
					((PlayerMobile)from).DoubleClickID = false;
				else
					((PlayerMobile)from).DoubleClickID = true;

				pressed = 12;
			}

			if ( pressed > 81 && pressed < 200 ) // SMALL INFO HELP WINDOWS
			{
				from.CloseGump( typeof( InfoHelpGump ) );
				from.SendGump( new InfoHelpGump( from, pressed, 12 ) );
			}
			else if ( pressed >= 200 && pressed <= 400 ) // MAGIC BARS OPEN AND CLOSE
			{
				from.SendGump( new Server.Engines.Help.HelpGump( from, 7 ) );

				if ( pressed == 266 ){ InvokeCommand( "bardtool1", from ); }
				else if ( pressed == 366 ){ InvokeCommand( "bardclose1", from ); }
				else if ( pressed == 267 ){ InvokeCommand( "bardtool2", from ); }
				else if ( pressed == 367 ){ InvokeCommand( "bardclose2", from ); }
				else if ( pressed == 268 ){ InvokeCommand( "knighttool1", from ); }
				else if ( pressed == 368 ){ InvokeCommand( "knightclose1", from ); }
				else if ( pressed == 269 ){ InvokeCommand( "knighttool2", from ); }
				else if ( pressed == 369 ){ InvokeCommand( "knightclose2", from ); }
				else if ( pressed == 270 ){ InvokeCommand( "deathtool1", from ); }
				else if ( pressed == 370 ){ InvokeCommand( "deathclose1", from ); }
				else if ( pressed == 271 ){ InvokeCommand( "deathtool2", from ); }
				else if ( pressed == 371 ){ InvokeCommand( "deathclose2", from ); }
				else if ( pressed == 272 ){ InvokeCommand( "magetool1", from ); }
				else if ( pressed == 372 ){ InvokeCommand( "mageclose1", from ); }
				else if ( pressed == 273 ){ InvokeCommand( "magetool2", from ); }
				else if ( pressed == 373 ){ InvokeCommand( "mageclose2", from ); }
				else if ( pressed == 274 ){ InvokeCommand( "magetool3", from ); }
				else if ( pressed == 374 ){ InvokeCommand( "mageclose3", from ); }
				else if ( pressed == 275 ){ InvokeCommand( "magetool4", from ); }
				else if ( pressed == 375 ){ InvokeCommand( "mageclose4", from ); }
				else if ( pressed == 276 ){ InvokeCommand( "necrotool1", from ); }
				else if ( pressed == 376 ){ InvokeCommand( "necroclose1", from ); }
				else if ( pressed == 277 ){ InvokeCommand( "necrotool2", from ); }
				else if ( pressed == 377 ){ InvokeCommand( "necroclose2", from ); }
				else if ( pressed == 278 ){ InvokeCommand( "holytool1", from ); }
				else if ( pressed == 378 ){ InvokeCommand( "holyclose1", from ); }
				else if ( pressed == 279 ){ InvokeCommand( "holytool2", from ); }
				else if ( pressed == 379 ){ InvokeCommand( "holyclose2", from ); }
				else if ( pressed == 280 ){ InvokeCommand( "monktool1", from ); }
				else if ( pressed == 380 ){ InvokeCommand( "monkclose1", from ); }
				else if ( pressed == 281 ){ InvokeCommand( "monktool2", from ); }
				else if ( pressed == 381 ){ InvokeCommand( "monkclose2", from ); }
				else if ( pressed == 282 ){ InvokeCommand( "elementtool1", from ); }
				else if ( pressed == 382 ){ InvokeCommand( "elementclose1", from ); }
				else if ( pressed == 283 ){ InvokeCommand( "elementtool2", from ); }
				else if ( pressed == 383 ){ InvokeCommand( "elementclose2", from ); }
				else if ( pressed == 384 ){ InvokeCommand( "archtool1", from ); }
				else if ( pressed == 385 ){ InvokeCommand( "archclose1", from ); }
				else if ( pressed == 386 ){ InvokeCommand( "archtool2", from ); }
				else if ( pressed == 387 ){ InvokeCommand( "archclose2", from ); }
				else if ( pressed == 388 ){ InvokeCommand( "archtool3", from ); }
				else if ( pressed == 389 ){ InvokeCommand( "archclose3", from ); }
				else if ( pressed == 390 ){ InvokeCommand( "archtool4", from ); }
				else if ( pressed == 391 ){ InvokeCommand( "archclose4", from ); }
			}
			else
			{
				switch ( pressed )
				{
					case 0: // Close/Cancel
					{
						//from.SendLocalizedMessage( 501235, "", 0x35 ); // Help request aborted.
						break;
					}
					case 1: // MAIN
					{
						from.SendGump( new Server.Engines.Help.HelpGump( from, pressed ) );
						break;
					}
					case 2: // AFK
					{
						InvokeCommand( "afk", from );
						from.SendGump( new Server.Engines.Help.HelpGump( from, pressed ) );
						break;
					}
					case 3: // Chat
					{
						InvokeCommand( "c", from );
						break;
					}
					case 4: // Corpse Clear
					{
						InvokeCommand( "corpseclear", from );
						from.SendGump( new Server.Engines.Help.HelpGump( from, pressed ) );
						break;
					}
					case 5: // Corpse Search
					{
						InvokeCommand( "corpse", from );
						break;
					}
					case 6: // Emote
					{
						InvokeCommand( "emote", from );
						break;
					}
					case 7: // Magic
					{
						from.SendGump( new Server.Engines.Help.HelpGump( from, 7 ) );
						break;
					}
					case 8: // Moongate
					{
						InvokeCommand( "magicgate", from );
						break;
					}
					case 9: // MOTD
					{
						from.CloseGump( typeof( Joeku.MOTD.MOTD_Gump ) );
						Joeku.MOTD.MOTD_Utility.SendGump( from, false, 0, 1 );
						break;
					}
					case 10: // Quests
					{
						from.SendGump( new Server.Engines.Help.HelpGump( from, pressed ) );
						break;
					}
					case 11: // Quick Bar
					{
						from.CloseGump( typeof( QuickBar ) );
						from.SendGump( new QuickBar( from ) );
						break;
					}
					case 62: // Reagent Bar
					{
						from.CloseGump( typeof( RegBar ) );
						from.SendGump( new RegBar( from ) );
						break;
					}
					case 12: // Settings
					{
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 13: // Library
					{
						from.CloseGump( typeof( MyLibrary ) );
						from.SendSound( 0x4A ); 
						from.SendGump( new MyLibrary( from, 1 ) );
						break;
					}
					case 14: // Statistics
					{
						from.CloseGump( typeof( Server.Statistics.StatisticsGump ) );
						from.SendGump( new Server.Statistics.StatisticsGump( from, 1 ) );
						break;
					}
					case 15: // Stuck
					{
						BaseHouse house = BaseHouse.FindHouseAt( from );

						if ( house != null && house.IsAosRules )
						{
							from.Location = house.BanLocation;
						}
						else if ( from.Region.IsPartOf( typeof( Server.Regions.Jail ) ) )
						{
							from.SendLocalizedMessage( 1041530, "", 0x35 ); // You'll need a better jailbreak plan then that!
						}
						else if ( from.CanUseStuckMenu() && from.Region.CanUseStuckMenu( from ) && !CheckCombat( from ) && !from.Frozen && !from.Criminal && (Core.AOS || from.Kills < 5) )
						{
							StuckMenu menu = new StuckMenu( from, from, true );

							menu.BeginClose();

							from.SendGump( menu );
						}

						break;
					}
					case 16: // Weapon Abilities
					{
						InvokeCommand( "sad", from );
						break;
					}
					case 17: // Wealth Bar
					{
						from.CloseGump( typeof( WealthBar ) );
						from.SendGump( new WealthBar( from ) );
						break;
					}
					case 18: // Conversations
					{
						from.CloseGump( typeof( MyChat ) );
						from.SendSound( 0x4A ); 
						from.SendGump( new MyChat( from, 1 ) );
						break;
					}
					case 19: // Versions
					{
						from.SendGump( new Server.Engines.Help.HelpGump( from, pressed ) );
						break;
					}
					case 49: // Set Ordinary Resources
					{
						if ( from.HarvestOrdinary )
							from.HarvestOrdinary = false;
						else
							from.HarvestOrdinary = true;

						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 50: // Set Containers
					{
						BaseContainer.ContainerSetTarget( from, box );
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 51: // Weapon Ability Names
					{
						if ( ((PlayerMobile)from).CharacterWepAbNames != 1 )
						{
							((PlayerMobile)from).CharacterWepAbNames = 1;
						}
						else
						{
							((PlayerMobile)from).CharacterWepAbNames = 0;
						}
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 52: // Auto Sheathe
					{
						if ( ((PlayerMobile)from).CharacterSheath == 1 )
						{
							((PlayerMobile)from).CharacterSheath = 0;
						}
						else
						{
							((PlayerMobile)from).CharacterSheath = 1;
						}
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 53: // Musical
					{
						string tunes = ((PlayerMobile)from).CharMusical;

						if ( tunes == "Forest" )
						{
							((PlayerMobile)from).CharMusical = "Dungeon";
						}
						else
						{
							((PlayerMobile)from).CharMusical = "Forest";
						}
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 54: // Private
					{
						PlayerMobile pm = (PlayerMobile)from;

						if ( pm.PublicInfo == false )
						{
							pm.PublicInfo = true;
						}
						else
						{
							pm.PublicInfo = false;
						}
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 55: // Loot
					{
						from.CloseGump( typeof( LootChoices ) );
						from.SendGump( new LootChoices( from, 1 ) );
						break;
					}
					case 56: // Skill Titles
					{
						from.CloseGump( typeof( SkillTitleGump ) );
						from.SendGump( new SkillTitleGump( from ) );
						break;
					}
					case 63: // Ancient Spellbook
					{
						if ( !ResearchSettings.BookCaster( from ) )
						{
							((PlayerMobile)from).UsingAncientBook = true;
						}
						else
						{
							((PlayerMobile)from).UsingAncientBook = false;
						}
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 982: // Skill List
					{
						if ( ((PlayerMobile)from).SkillDisplay > 0 ){ ((PlayerMobile)from).SkillDisplay = 0; } else { ((PlayerMobile)from).SkillDisplay = 1; }
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						Server.Gumps.SkillListingGump.RefreshSkillList( from );
						break;
					}
					case 983: // Open Skill List
					{
						Server.Gumps.SkillListingGump.OpenSkillList( from );
						break;
					}
					case 985: // Gump Images
					{
						int gump = ((PlayerMobile)from).GumpHue;

						if ( gump > 0 )
						{
							((PlayerMobile)from).GumpHue = 0;
						}
						else
						{
							((PlayerMobile)from).GumpHue = 1;
						}
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 986: // Weapon Ability Auto-Open
					{
						int wep = ((PlayerMobile)from).WeaponBarOpen;

						if ( wep > 0 )
						{
							((PlayerMobile)from).WeaponBarOpen = 0;
						}
						else
						{
							((PlayerMobile)from).WeaponBarOpen = 1;
						}
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 989: // Creature Magic Focus
					{
						if ( from.RaceMagicSchool == 0 )
								from.RaceMagicSchool = 1;
						else if ( from.RaceMagicSchool == 1 )
								from.RaceMagicSchool = 2;
						else if ( from.RaceMagicSchool == 2 )
								from.RaceMagicSchool = 3;
						else
								from.RaceMagicSchool = 0;

						if ( from.FindItemOnLayer( Layer.Special ) != null && from.RaceID > 0 )
						{
							if ( from.FindItemOnLayer( Layer.Special ) is BaseRace )
								Server.Items.BaseRace.SetMonsterMagic( from, (BaseRace)(from.FindItemOnLayer( Layer.Special )) );
						}

						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 990: // Creature Type Choice
					{
						from.RaceSection = 1;
						from.SendGump( new Server.Items.RacePotions.RacePotionsGump( from, 1 ) );
						break;
					}
					case 991: // Creature Sounds
					{
						if ( !from.RaceMakeSounds )
								from.RaceMakeSounds = true;
						else
								from.RaceMakeSounds = false;

						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 57: // Normal Play
					{
						((PlayerMobile)from).CharacterEvil = 0;
						((PlayerMobile)from).CharacterOriental = 0;
						((PlayerMobile)from).CharacterBarbaric = 0;
						Server.Items.BarbaricSatchel.GetRidOf( from );
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 58: // Evil Play
					{
						((PlayerMobile)from).CharacterEvil = 1;
						((PlayerMobile)from).CharacterOriental = 0;
						((PlayerMobile)from).CharacterBarbaric = 0;
						Server.Items.BarbaricSatchel.GetRidOf( from );
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 59: // Oriental Play
					{
						((PlayerMobile)from).CharacterEvil = 0;
						((PlayerMobile)from).CharacterOriental = 1;
						((PlayerMobile)from).CharacterBarbaric = 0;
						Server.Items.BarbaricSatchel.GetRidOf( from );
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 60: // Message Color
					{
						if ( from.RainbowMsg )
						{
							from.RainbowMsg = false;
						}
						else
						{
							from.RainbowMsg = true;
						}
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 61: // Auto Attack
					{
						if ( from.NoAutoAttack )
						{
							from.NoAutoAttack = false;
						}
						else
						{
							from.NoAutoAttack = true;
						}
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 984: // Barbaric Play
					{
						if ( ((PlayerMobile)from).CharacterBarbaric == 1 && from.Female )
						{
							((PlayerMobile)from).CharacterBarbaric = 2;
						}
						else if ( ((PlayerMobile)from).CharacterBarbaric > 0 )
						{
							((PlayerMobile)from).CharacterBarbaric = 0;
							Server.Items.BarbaricSatchel.GetRidOf( from );
						}
						else
						{
							((PlayerMobile)from).CharacterEvil = 0;
							((PlayerMobile)from).CharacterOriental = 0;
							((PlayerMobile)from).CharacterBarbaric = 1;
							Server.Items.BarbaricSatchel.GivePack( from );
						}
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 64: // Poisoning
					{
						if ( ((PlayerMobile)from).ClassicPoisoning == 1 )
						{
							((PlayerMobile)from).ClassicPoisoning = 0;
						}
						else
						{
							((PlayerMobile)from).ClassicPoisoning = 1;
						}
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 65: // Music Playlist
					{
						from.CloseGump( typeof( MusicPlaylist ) );
						from.SendGump( new MusicPlaylist( from, 1 ) );
						break;
					}
					case 66: // SPELL BARS BELOW ---------------------------------------
					{
						from.CloseGump( typeof( SetupBarsBard1 ) );
						from.SendGump( new SetupBarsBard1( from, 1 ) );
						break;
					}
					case 67:
					{
						from.CloseGump( typeof( SetupBarsBard2 ) );
						from.SendGump( new SetupBarsBard2( from, 1 ) );
						break;
					}
					case 68:
					{
						from.CloseGump( typeof( SetupBarsKnight1 ) );
						from.SendGump( new SetupBarsKnight1( from, 1 ) );
						break;
					}
					case 69:
					{
						from.CloseGump( typeof( SetupBarsKnight2 ) );
						from.SendGump( new SetupBarsKnight2( from, 1 ) );
						break;
					}
					case 70:
					{
						from.CloseGump( typeof( SetupBarsDeath1 ) );
						from.SendGump( new SetupBarsDeath1( from, 1 ) );
						break;
					}
					case 71:
					{
						from.CloseGump( typeof( SetupBarsDeath2 ) );
						from.SendGump( new SetupBarsDeath2( from, 1 ) );
						break;
					}
					case 72:
					{
						from.CloseGump( typeof( SetupBarsMage1 ) );
						from.SendGump( new SetupBarsMage1( from, 1 ) );
						break;
					}
					case 73:
					{
						from.CloseGump( typeof( SetupBarsMage2 ) );
						from.SendGump( new SetupBarsMage2( from, 1 ) );
						break;
					}
					case 74:
					{
						from.CloseGump( typeof( SetupBarsMage3 ) );
						from.SendGump( new SetupBarsMage3( from, 1 ) );
						break;
					}
					case 75:
					{
						from.CloseGump( typeof( SetupBarsMage4 ) );
						from.SendGump( new SetupBarsMage4( from, 1 ) );
						break;
					}
					case 76:
					{
						from.CloseGump( typeof( SetupBarsNecro1 ) );
						from.SendGump( new SetupBarsNecro1( from, 1 ) );
						break;
					}
					case 77:
					{
						from.CloseGump( typeof( SetupBarsNecro2 ) );
						from.SendGump( new SetupBarsNecro2( from, 1 ) );
						break;
					}
					case 78:
					{
						from.CloseGump( typeof( SetupBarsPriest1 ) );
						from.SendGump( new SetupBarsPriest1( from, 1 ) );
						break;
					}
					case 79:
					{
						from.CloseGump( typeof( SetupBarsPriest2 ) );
						from.SendGump( new SetupBarsPriest2( from, 1 ) );
						break;
					}
					case 80:
					{
						from.SendGump( new CustomTitleGump( from ) );
						break;
					}
					case 1081:
					{
						from.CloseGump( typeof( SetupBarsArch1 ) );
						from.SendGump( new SetupBarsArch1( from, 1 ) );
						break;
					}
					case 1082:
					{
						from.CloseGump( typeof( SetupBarsArch2 ) );
						from.SendGump( new SetupBarsArch2( from, 1 ) );
						break;
					}
					case 1083:
					{
						from.CloseGump( typeof( SetupBarsArch3 ) );
						from.SendGump( new SetupBarsArch3( from, 1 ) );
						break;
					}
					case 1084:
					{
						from.CloseGump( typeof( SetupBarsArch4 ) );
						from.SendGump( new SetupBarsArch4( from, 1 ) );
						break;
					}
					case 980:
					{
						from.CloseGump( typeof( SetupBarsMonk1 ) );
						from.SendGump( new SetupBarsMonk1( from, 1 ) );
						break;
					}
					case 981:
					{
						from.CloseGump( typeof( SetupBarsMonk2 ) );
						from.SendGump( new SetupBarsMonk2( from, 1 ) );
						break;
					}
					case 978:
					{
						from.CloseGump( typeof( SetupBarsElement1 ) );
						from.SendGump( new SetupBarsElement1( from, 1 ) );
						break;
					}
					case 979:
					{
						from.CloseGump( typeof( SetupBarsElement2 ) );
						from.SendGump( new SetupBarsElement2( from, 1 ) );
						break;
					}
					case 500:
					{
						((PlayerMobile)from).MagerySpellHue = 0x47E;
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 501:
					{
						((PlayerMobile)from).MagerySpellHue = 0x94E;
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 502:
					{
						((PlayerMobile)from).MagerySpellHue = 0x48D;
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 503:
					{
						((PlayerMobile)from).MagerySpellHue = 0x48E;
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 504:
					{
						((PlayerMobile)from).MagerySpellHue = 0x48F;
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 505:
					{
						((PlayerMobile)from).MagerySpellHue = 0x490;
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 506:
					{
						((PlayerMobile)from).MagerySpellHue = 0x491;
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
					case 507:
					{
						((PlayerMobile)from).MagerySpellHue = 0;
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
						break;
					}
				}
			}
		}

		public static string MyQuests( Mobile from )
        {
			PlayerMobile pm = (PlayerMobile)from;

			string sQuests = "Abaixo está uma breve lista de missões atuais, juntamente com conquistas em descobertas específicas. Estas são missões pertencentes a você, que são específicas para o seu personagem. Outras missões (como mensagens em uma garrafa, mapas do tesouro ou notas rabiscadas) não estão listadas aqui.<br><br>";

			string ContractQuest = PlayerSettings.GetQuestInfo( from, "StandardQuest" );
			if ( PlayerSettings.GetQuestState( from, "StandardQuest" ) ){ string sAdventurer = StandardQuestFunctions.QuestStatus( from ); sQuests = sQuests + "-" + sAdventurer + ".<br><br>"; }

			string ContractKiller = PlayerSettings.GetQuestInfo( from, "AssassinQuest" );
			if ( PlayerSettings.GetQuestState( from, "AssassinQuest" ) ){ string sAssassin = AssassinFunctions.QuestStatus( from ); sQuests = sQuests + "-" + sAssassin + ".<br><br>"; }

			string ContractSailor = PlayerSettings.GetQuestInfo( from, "FishingQuest" );
			if ( PlayerSettings.GetQuestState( from, "FishingQuest" ) ){ string sSailor = FishingQuestFunctions.QuestStatus( from ); sQuests = sQuests + "-" + sSailor + ".<br><br>"; }

			sQuests = sQuests + OtherQuests( from );

			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleMadGodName" ) ){ sQuests = sQuests + "-Aprendeu sobre o Deus Louco Tarjan.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleCatacombKey" ) ){ sQuests = sQuests + "-O sacerdote do Templo do Deus Louco me deu a chave para as Catacumbas.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleSpectreEye" ) ){ sQuests = sQuests + "-Encontrou um olho misterioso das Catacumbas.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleHarkynKey" ) ){ sQuests = sQuests + "-Encontrou uma chave com um símbolo de dragão.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleDragonKey" ) ){ sQuests = sQuests + "-Encontrou uma chave enferrujada no pescoço de um dragão cinzento.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleCrystalSword" ) ){ sQuests = sQuests + "-Encontrou uma espada de cristal.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleSilverSquare" ) ){ sQuests = sQuests + "-Encontrou um quadrado de prata.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleKylearanKey" ) ){ sQuests = sQuests + "-Encontrou uma chave com um símbolo de unicórnio.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleBedroomKey" ) ){ sQuests = sQuests + "-Encontrou uma chave com um símbolo de árvore.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleSilverTriangle" ) ){ sQuests = sQuests + "-Encontrou um triângulo de prata.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleCrystalGolem" ) ){ sQuests = sQuests + "-Destruiu o golem de cristal e encontrou uma chave dourada.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleEbonyKey" ) ){ sQuests = sQuests + "-Kylearan me deu uma chave de ébano com um símbolo de demônio.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleSilverCircle" ) ){ sQuests = sQuests + "-Encontrou um círculo de prata.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleWin" ) && ((PlayerMobile)from).Fugitive != 1 ){ sQuests = sQuests + "-Derrotou o mago maligno Mangar e escapou de Skara Brae.<br><br>"; }

			if ( PlayerSettings.GetKeys( from, "UndermountainKey" ) ){ sQuests = sQuests + "-Encontrou uma chave feita de aço anão.<br><br>"; }
			if ( PlayerSettings.GetKeys( from, "BlackKnightKey" ) ){ sQuests = sQuests + "-Encontrou a chave do Cavaleiro Negro.<br><br>"; }
			if ( PlayerSettings.GetKeys( from, "SkullGate" ) ){ sQuests = sQuests + "-Descobriu o segredo do Portão da Caveira.<br>   Um está na Cidade Subterrânea de Umbra em Sosaria.<br>   O outro está na Floresta Ravendark.<br><br>"; }
			if ( PlayerSettings.GetKeys( from, "SerpentPillars" ) ){ sQuests = sQuests + "-Descobriu o segredo dos Pilares da Serpente.<br>   Sosaria: 86° 41'S, 124° 39'E<br>   Lodoria: 35° 36'S, 65° 2'E<br><br>"; }
			if ( PlayerSettings.GetKeys( from, "RangerOutpost" ) ){ sQuests = sQuests + "-Descobriu o Posto Avançado dos Rangers.<br><br>"; }
			if ( PlayerSettings.GetKeys( from, "DragonRiding" ) ){ sQuests = sQuests + "-Aprendeu os segredos de montar criaturas dracônicas.<br><br>"; }

			if ( PlayerSettings.GetDiscovered( from, "the Land of Sosaria" ) ){ sQuests = sQuests + "-Descobriu o Mundo de Sosaria.<br><br>"; }
			if ( PlayerSettings.GetDiscovered( from, "the Island of Umber Veil" ) ){ sQuests = sQuests + "-Descobriu Umber Veil.<br><br>"; }
			if ( PlayerSettings.GetDiscovered( from, "the Land of Ambrosia" ) ){ sQuests = sQuests + "-Descobriu Ambrosia.<br><br>"; }
			if ( PlayerSettings.GetDiscovered( from, "the Land of Lodoria" ) ){ sQuests = sQuests + "-Descobriu o Mundo Élfico de Lodoria.<br><br>"; }
			if ( PlayerSettings.GetDiscovered( from, "the Serpent Island" ) ){ sQuests = sQuests + "-Descobriu a Ilha da Serpente.<br><br>"; }
			if ( PlayerSettings.GetDiscovered( from, "the Isles of Dread" ) ){ sQuests = sQuests + "-Descobriu as Ilhas do Pavor.<br><br>"; }
			if ( PlayerSettings.GetDiscovered( from, "the Savaged Empire" ) ){ sQuests = sQuests + "-Descobriu o Vale do Império Selvagem.<br><br>"; }
			if ( PlayerSettings.GetDiscovered( from, "the Bottle World of Kuldar" ) ){ sQuests = sQuests + "-Descobriu o Mundo da Garrafa de Kuldar.<br><br>"; }
			if ( PlayerSettings.GetDiscovered( from, "the Underworld" ) ){ sQuests = sQuests + "-Descobriu o Submundo.<br><br>"; }

			return "Missões Para " + from.Name + "<br><br>" + sQuests;
        }

		public static string OtherQuests( Mobile from )
        {
			string quests = "";

			ArrayList targets = new ArrayList();
			foreach ( Item item in World.Items.Values )
			{
				if ( item is ThiefNote )
				{
					if ( ((ThiefNote)item).NoteOwner == from )
					{
						if ( Server.Items.ThiefNote.ThiefAllowed( from ) == null )
						{
							quests = quests + "-" + ((ThiefNote)item).NoteStory + "<br><br>";
						}
						else
						{
							quests = quests + "-Você tem uma nota secreta instruindo você a roubar algo, mas vai fazer uma pausa no roubo e lê-la em cerca de " + Server.Items.ThiefNote.ThiefAllowed( from ) + " minutos.<br><br>";
						}
					}
				}
				else if ( item is CourierMail )
				{
					if ( ((CourierMail)item).Owner == from )
					{
						quests = quests + "-Você precisa encontrar " + ((CourierMail)item).SearchItem + " para " + ((CourierMail)item).ForWho + ". Eles disseram em sua carta que você deve procurar em " + ((CourierMail)item).SearchDungeon + " em " + ((CourierMail)item).SearchWorld + ".<br><br>";
					}
				}
				else if ( item is SearchPage )
				{
					if ( ((SearchPage)item).Owner == from )
					{
						quests = quests + "-Você quer encontrar " + ((SearchPage)item).SearchItem + " em " + ((SearchPage)item).SearchDungeon + " em " + ((SearchPage)item).SearchWorld + ".<br><br>";
					}
				}
				else if ( item is SummonPrison )
				{
					if ( ((SummonPrison)item).owner == from )
					{
						quests = quests + "-Você atualmente tem " + System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(((SummonPrison)item).Prisoner.ToLower()) + " em uma Prisão Mágica.<br><br>";
					}
				}
				else if ( item is FrankenJournal )
				{
					if ( ((FrankenJournal)item).JournalOwner == from )
					{
						int parts = 0;
						if ( ((FrankenJournal)item).HasArmRight > 0 ){ parts++; }
						if ( ((FrankenJournal)item).HasArmLeft > 0 ){ parts++; }
						if ( ((FrankenJournal)item).HasLegRight > 0 ){ parts++; }
						if ( ((FrankenJournal)item).HasLegLeft > 0 ){ parts++; }
						if ( ((FrankenJournal)item).HasTorso > 0 ){ parts++; }
						if ( ((FrankenJournal)item).HasHead > 0 ){ parts++; }

						quests = quests + "-Você atualmente tem " + parts + " de 6 partes do corpo necessárias para criar um golem de carne.<br><br>";
					}
				}
				else if ( item is RuneBox )
				{
					if ( ((RuneBox)item).RuneBoxOwner == from )
					{
						int runes = 0;
						if ( ((RuneBox)item).HasCompassion > 0 ){ runes++; }
						if ( ((RuneBox)item).HasHonesty > 0 ){ runes++; }
						if ( ((RuneBox)item).HasHonor > 0 ){ runes++; }
						if ( ((RuneBox)item).HasHumility > 0 ){ runes++; }
						if ( ((RuneBox)item).HasJustice > 0 ){ runes++; }
						if ( ((RuneBox)item).HasSacrifice > 0 ){ runes++; }
						if ( ((RuneBox)item).HasSpirituality > 0 ){ runes++; }
						if ( ((RuneBox)item).HasValor > 0 ){ runes++; }

						quests = quests + "-Você atualmente tem " + runes + " de 8 runas de virtude.<br><br>";
					}
				}
				else if ( item is SearchPage )
				{
					if ( ((SearchPage)item).owner == from )
					{
						quests = quests + "-Você está em uma missão para obter o " + ((SearchPage)item).SearchItem + ".<br><br>";
					}
				}
				else if ( item is VortexCube )
				{
					if ( ((VortexCube)item).CubeOwner == from )
					{
						VortexCube cube = (VortexCube)item;
						quests = quests + "-Você está procurando pelo Codex of Ultimate Wisdom.<br>";

						if ( cube.HasConvexLense > 0 ){ quests = quests + "   -Você tem o Convex Lense.<br>"; }
						if ( cube.HasConcaveLense > 0 ){ quests = quests + "   -Você tem o Concave Lense.<br>"; }

						if ( cube.HasKeyLaw > 0 ){ quests = quests + "   -Você tem o Key of Law.<br>"; }
						if ( cube.HasKeyBalance > 0 ){ quests = quests + "   -Você tem o Key of Balance.<br>"; }
						if ( cube.HasKeyChaos > 0 ){ quests = quests + "   -Você tem o Key of Chaos.<br>"; }

						if ( cube.HasCrystalRed > 0 ){ quests = quests + "   -Você tem o Red Void Crystal.<br>"; }
						if ( cube.HasCrystalBlue > 0 ){ quests = quests + "   -Você tem o Blue Void Crystal.<br>"; }
						if ( cube.HasCrystalGreen > 0 ){ quests = quests + "   -Você tem o Green Void Crystal.<br>"; }
						if ( cube.HasCrystalYellow > 0 ){ quests = quests + "   -Você tem o Yellow Void Crystal.<br>"; }
						if ( cube.HasCrystalWhite > 0 ){ quests = quests + "   -Você tem o White Void Crystal.<br>"; }
						if ( cube.HasCrystalPurple > 0 ){ quests = quests + "   -Você tem o Purple Void Crystal.<br>"; }

						quests = quests + "<br>";
					}
				}
				else if ( item is ObeliskTip )
				{
					if ( ((ObeliskTip)item).ObeliskOwner == from )
					{
						ObeliskTip obelisk = (ObeliskTip)item;
						quests = quests + "-Você está tentando se tornar um Titã do Éter.<br>";
						quests = quests + "   -Você tem o Obelisk Tip.<br>"; 

						if ( obelisk.WonAir > 0 ){ quests = quests + "   -Você derrotou Stratos, o Titã do Ar.<br>"; }
						else if ( obelisk.HasAir > 0 ){ quests = quests + "   -Você tem o Breath of Air.<br>"; }
						if ( obelisk.WonFire > 0 ){ quests = quests + "   -Você derrotou Pyros, o Titã do Fogo.<br>"; }
						else if ( obelisk.HasFire > 0 ){ quests = quests + "   -Você tem o Tongue of Flame.<br>"; }
						if ( obelisk.WonEarth > 0 ){ quests = quests + "   -Você derrotou Lithos, o Titã da Terra.<br>"; }
						else if ( obelisk.HasEarth > 0 ){ quests = quests + "   -Você tem o Heart of Earth.<br>"; }
						if ( obelisk.WonWater > 0 ){ quests = quests + "   -Você derrotou Hydros, o Titã da Água.<br>"; }
						else if ( obelisk.HasWater > 0 ){ quests = quests + "   -Você tem o Tear of the Seas.<br>"; }

						quests = quests + "<br>";
					}
				}
				else if ( item is MuseumBook )
				{
					if ( ((MuseumBook)item).ArtOwner == from )
					{
						quests = quests + "-Você encontrou " + MuseumBook.GetTotal( (MuseumBook)item ) + " de 60 antiguidades para o museu.<br><br>";
					}
				}
				else if ( item is RuneBox )
				{
					if ( ((RuneBox)item).RuneBoxOwner == from )
					{
						int runes = ((RuneBox)item).HasCompassion + ((RuneBox)item).HasHonesty + ((RuneBox)item).HasHonor + ((RuneBox)item).HasHumility + ((RuneBox)item).HasJustice + ((RuneBox)item).HasSacrifice + ((RuneBox)item).HasSpirituality + ((RuneBox)item).HasValor;
						quests = quests + "-Você encontrou " + runes + " de 8 runas da virtude.<br><br>";
					}
				}
				else if ( item is QuestTome )
				{
					if ( ((QuestTome)item).QuestTomeOwner == from )
					{
						quests = quests + "-Você está em uma missão para encontrar " + ((QuestTome)item).GoalItem4 + ".<br><br>";
					}
				}
			}

			if ( 	from.Backpack.FindItemByType( typeof ( ScalesOfEthicality ) ) != null || 
					from.Backpack.FindItemByType( typeof ( OrbOfLogic ) ) != null || 
					from.Backpack.FindItemByType( typeof ( LanternOfDiscipline ) ) != null || 
					from.Backpack.FindItemByType( typeof ( BlackrockSerpentOrder ) ) != null || 
					from.Backpack.FindItemByType( typeof ( BlackrockSerpentChaos ) ) != null || 
					from.Backpack.FindItemByType( typeof ( BlackrockSerpentBalance ) ) != null )
			{
				quests = quests + "-Você está em uma missão para trazer os Serpentes de volta para o equilíbrio.<br><br>";
			}

			if ( 	from.Backpack.FindItemByType( typeof ( ShardOfFalsehood ) ) != null || 
					from.Backpack.FindItemByType( typeof ( ShardOfCowardice ) ) != null || 
					from.Backpack.FindItemByType( typeof ( ShardOfHatred ) ) != null || 
					from.Backpack.FindItemByType( typeof ( CandleOfLove ) ) != null || 
					from.Backpack.FindItemByType( typeof ( BookOfTruth ) ) != null || 
					from.Backpack.FindItemByType( typeof ( BellOfCourage ) ) != null )
			{
				quests = quests + "-Você está em uma missão para destruir os Shadowlords e construir uma Gem of Immortality.<br><br>";
			}
			else if ( from.Backpack.FindItemByType( typeof ( GemImmortality ) ) != null )
			{
				quests = quests + "-Você construiu uma Gem of Immortality.<br><br>";
			}
			if(PlayerSettings.GetKeys ( from, "Soveliss"))
			{
				quests = quests + "-Você deu o Dreamstone para Soveliss, e ele lhe deu o Dragonbane.<br><br>";
			}
			if(PlayerSettings.GetKeys ( from, "Mitru"))
			{
				quests = quests + "-Você deu o Dreamstone para Mitru, e ele lhe deu o Moon Dancer.<br><br>";
			}
			if(PlayerSettings.GetKeys ( from, "Kongor"))
			{
				quests = quests + "-Você deu o Dreamstone para Kongor, e ele lhe deu o Kongor's Undying Rage.<br><br>";
			}
			if(PlayerSettings.GetKeys ( from, "Masamune"))
			{
				quests = quests + "-Você deu o Dreamstone para Melchior, e ele lhe deu o Masamune.<br><br>";
			}
			if(PlayerSettings.GetKeys ( from, "DreamstoneUsed"))
			{
				quests = quests + "-Você usou seu Dreamstone.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "Dreamstone" ) )
			{
				quests = quests + "-Você encontrou o Dreamstone.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "Museums" ) )
			{
				quests = quests + "-Você encontrou todos as antiguidades para o Museu.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "Gygax" ) )
			{
				quests = quests + "-Você obteve a Estátua de Gygax.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "Virtues" ) )
			{
				quests = quests + "-Você limpou todas as Runas da Virtude.<br><br>";
			}
			else if ( PlayerSettings.GetKeys( from, "Corrupt" ) )
			{
				quests = quests + "-Você corrompeu todas as Runas da Virtude.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "Exodus" ) )
			{
				quests = quests + "-Você destruiu o Core of Exodus.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "BlackGateDemon" ) )
			{
				quests = quests + "-Você derrotou o Demônio do Portão Negro e encontrou um portal para o Plano Etéreo.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "Jormungandr" ) )
			{
				quests = quests + "-Você derrotou a serpente lendária conhecida como Jormungandr.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "Dracula" ) )
			{
				quests = quests + "-Você destruiu Drácula, o governante de todos os vampiros.<br><br>";
			}
			if ( 	from.Backpack.FindItemByType( typeof ( StaffPartVenom ) ) != null || 
					from.Backpack.FindItemByType( typeof ( StaffPartCaddellite ) ) != null || 
					from.Backpack.FindItemByType( typeof ( StaffPartFire ) ) != null || 
					from.Backpack.FindItemByType( typeof ( StaffPartLight ) ) != null || 
					from.Backpack.FindItemByType( typeof ( StaffPartEnergy ) ) != null )
			{
				quests = quests + "-Você está buscando montar o Staff of Ultimate Power.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "Arachnar" ) )
			{
				quests = quests + "-Você derrotou Arachnar, o guardião do cajado.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "Surtaz" ) )
			{
				quests = quests + "-Você derrotou Surtaz, o guardião do cajado.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "Vordinax" ) )
			{
				quests = quests + "-Você derrotou Vordinax, o guardião do cajado.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "Vulcrum" ) )
			{
				quests = quests + "-Você derrotou Vulcrum, o guardião do cajado.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "Xurtzar" ) )
			{
				quests = quests + "-Você derrotou Xurtzar, o guardião do cajado.<br><br>";
			}

			return quests;
		}

		public static string MyHelp()
        {
			string HelpText = "Se você está procurando ajuda para explorar este mundo, pode aprender sobre quase qualquer coisa dentro do mundo do jogo onde viaja. Alguns comerciantes vendem pergaminhos ou livros que explicam como algumas habilidades podem ser realizadas, recursos coletados, e até mesmo como elementos do mundo podem ser manipulados. Um sábio frequentemente vende muitos tomos com informações úteis sobre habilidades, habilidades de armas ou vários tipos de magias disponíveis. Se você é totalmente novo neste jogo, compre um livro Guia de Aventura de um sábio se perdeu o que recebeu no início. Este livro explica como navegar e jogar o jogo. Você também aprenderá algumas coisas sobre como o mundo se comporta, como interações com comerciantes, como usar itens e o que fazer quando seu personagem morrer. Converse com os habitantes da cidade para aprender o que puder. Nesta tela há muitas opções, informações e configurações que podem auxiliar em sua jornada. Muitas das opções aqui têm comandos de teclado listados abaixo. Certifique-se de verificar a seção 'Info' no paperdoll do seu personagem, pois contém algumas informações vitais sobre seu personagem.<br><br>"
				+ "Comandos Comuns: Abaixo estão os comandos que você pode usar para várias coisas no jogo.<br><br>"
				+ "[abilitynames - Ativa/desativa os nomes das habilidades especiais de armas ao lado dos ícones apropriados.<br><br>"
				+ "[afk - Ativa/desativa a notificação para outros de que você está ausente do teclado.<br><br>"
				+ "[ancient - Ativa/desativa se você está usando magia da bolsa de pesquisa ou do livro de feitiços antigos.<br><br>"
				+ "[autoattack - Ativa/desativa se você ataca automaticamente quando atacado.<br><br>"
				+ "[bandother - Comando de atadura em outro.<br><br>"
				+ "[bandself - Comando de atadura em si mesmo.<br><br>"
				+ "[barbaric - Ativa/desativa o sabor bárbaro que o jogo oferece (veja o final).<br><br>"
				+ "[c - Inicia o sistema de bate-papo.<br><br>"
				+ "[corpse - Ajuda a encontrar seus restos mortais.<br><br>"
				+ "[corpseclear - Remove seu cadáver do convés de um navio.<br><br>"
				+ "[e - Abre a mini janela de emoção.<br><br>"
				+ "[emote - Abre a janela de emoção.<br><br>"
				+ "[evil - Ativa/desativa o sabor maligno que o jogo oferece (veja o final).<br><br>"
				+ "[loot - Coleta automaticamente certos itens de baús de masmorras comuns ou cadáveres e os coloca em sua mochila. Os itens desconhecidos são aqueles que precisarão de identificação, mas você pode decidir pegá-los mesmo assim. As opções de reagentes têm algumas categorias. Reagentes de magia e necromancia são aqueles usados especificamente por esses personagens, enquanto reagentes de poções de bruxaria se enquadram principalmente na categoria de necromancia. Reagentes alquímicos são aqueles que estão fora da categoria de reagentes de magia e necromancia, e apenas alquimistas os usam. Reagentes de herbalismo são úteis no herbalismo druídico.<br><br>"
				+ "[magicgate - Ajuda a encontrar o portal mágico mais próximo.<br><br>"
				+ "[motd - Abre a mensagem do dia.<br><br>"
				+ "[oriental - Ativa/desativa o sabor oriental que o jogo oferece (veja o final).<br><br>"
				+ "[password - Altera a senha da sua conta. As senhas têm um limite de 16 caracteres.<br><br>"
				+ "[poisons - Isso altera como as armas envenenadas funcionam, que pode ser para controle preciso com golpes infecciosos de armas especiais (padrão) ou com golpes de uma arma de corte ou perfuração de uma mão.<br><br>"
				+ "[private - Ativa/desativa mensagens detalhadas de sua jornada para o pregoeiro e conversas de cidadãos locais.<br><br>"
				+ "[quests - Abre um pergaminho para mostrar certos eventos de missão.<br><br>"
				+ "[quickbar - Abre uma pequena barra vertical com funções comuns do jogo para facilitar o uso.<br><br>"
				+ "[checksecure - Verifica a casa do jogador em busca de itens não seguros/destrancados no chão para evitar perda de itens. Os itens não seguros serão marcados com cor vermelha e um som será reproduzido pelo cliente se algum for encontrado.<br><br>"
				+ "[sad - Abre as habilidades especiais da arma.<br><br>"
				+ "[set1 - Define a primeira habilidade da sua arma como ativa.<br>"
				+ "[set2 - Define a segunda habilidade da sua arma como ativa.<br>"
				+ "[set3 - Define a terceira habilidade da sua arma como ativa.<br>"
				+ "[set4 - Define a quarta habilidade da sua arma como ativa.<br>"
				+ "[set5 - Define a quinta habilidade da sua arma como ativa.<br><br>"
				+ "[sheathe - Ativa/desativa a função de embainhar sua arma quando não está em batalha.<br><br>"
				+ "[skill - Mostra para que cada habilidade é usada.<br><br>"
				+ "[skilllist - Exibe uma lista mais condensada das habilidades que você definiu como 'up' e talvez 'locked'.<br><br>"
				+ "[spellhue ## - Este comando, seguido por um número de referência de cor, mudará todos os seus efeitos de feitiços de magia para aquela cor. Um valor de '1' normalmente será renderizado como '0', então evite essa configuração, pois não produzirá o resultado desejado.<br><br>"
				+ "[statistics - Mostra algumas estatísticas do servidor.<br><br>"
				+ "[wealth - Abre uma pequena barra horizontal mostrando o valor do seu ouro para as várias formas de moeda e ouro em seu banco e mochila. Moeda são itens que você faria um banqueiro converter em ouro para você (prata, cobre, xormita, joias e cristais). Se você colocar esses itens em seu banco, pode atualizar os valores na barra de riqueza clicando com o botão direito nela.<br><br>"
				+ "[ShowSkillGainChance - alterna a exibição da chance de ganho de habilidade no seu diário.<br><br>"
				+ "[VetSupplies - encontra e usa automaticamente suprimentos veterinários do inventário do personagem, se estiverem lá.<br><br>"
				+ "[Ascension - mostra a janela principal de ascensão.<br><br>"
				+ "[AscensionQuickbar - abre a barra de habilidades rápidas para a ascensão atualmente selecionada.<br><br>"
				+ "[SkillDrop - abre o gump de redução de habilidade.<br><br>"

				+ "<br><br>"

				+ "Níveis de Dificuldade da Área: Quando você entra em muitas áreas perigosas, haverá uma mensagem informando que você entrou em uma determinada área. Pode haver um nível de dificuldade mostrado entre parênteses, que lhe dará uma indicação sobre a dificuldade da área. Abaixo estão as descrições para cada nível.<br><br>"
				+ " - Fácil (Não é muito desafiador)<br><br>"
				+ " - Normal (Um nível médio de<br>"
				+ "        desafio)<br><br>"
				+ " - Difícil (Um pouco mais difícil)<br><br>"
				+ " - Desafiador (Você provavelmente<br>"
				+ "        vai fugir muito)<br><br>"
				+ " - Duro (Você provavelmente vai morrer muito)<br><br>"
				+ " - Mortal (Eu te desafio)<br><br>"
				+ " - Épico (Para Titãs do Éter)<br><br>"

				+ "<br><br>"

				+ "Títulos de Habilidade: Você pode definir seu título padrão para seu personagem. Embora você possa ser um Mestre Driven, pode querer que seu título reflita seu título de Aprendiz de Mago. É assim que você o define...<br><br>"
				+ "Digite o comando '[SkillName' seguido pelo nome da habilidade que você deseja definir como padrão. Certifique-se de colocar o nome da habilidade entre aspas e em letras minúsculas. Exemplo...<br>"
				+ "  [SkillName \"taming\"<br><br>"
				+ "Se você quer que o jogo gerencie o título do seu personagem, use o mesmo comando com um nome de habilidade \"clear\".<br><br>"

				+ "<br><br>"

				+ "Barras de Reagentes: Abaixo estão os comandos que você pode usar para monitorar suas quantidades de reagentes enquanto lança feitiços ou cria poções. Estas são barras personalizáveis que mostrarão as quantidades dos reagentes que você está carregando. Elas mostrarão quantidades atualizadas de reagentes sempre que você lançar um feitiço ou fizer uma poção que os utilize. Caso contrário, você pode criar uma macro para esses comandos e usá-los para atualizar os valores manualmente.<br><br>"
				+ "[regbar - Abre a barra de reagentes.<br><br>"
				+ "[regclose - Fecha a barra de reagentes.<br><br>"

				+ "<br><br>"

				+ "Barras de Ferramentas Mágicas: Abaixo estão os comandos que você pode usar para gerenciar barras de ferramentas mágicas que podem ajudá-lo a jogar melhor.<br><br>"
				+ "[archspell1 - Abre o editor da 1ª barra de feitiços antigos.<br><br>"
				+ "[archspell2 - Abre o editor da 2ª barra de feitiços antigos.<br><br>"
				+ "[archspell3 - Abre o editor da 3ª barra de feitiços antigos.<br><br>"
				+ "[archspell4 - Abre o editor da 4ª barra de feitiços antigos.<br><br>"
				+ "[bardsong1 - Abre o editor da 1ª barra de canções de bardo.<br><br>"
				+ "[bardsong2 - Abre o editor da 2ª barra de canções de bardo.<br><br>"
				+ "[knightspell1 - Abre o editor da 1ª barra de feitiços de cavaleiro.<br><br>"
				+ "[knightspell2 - Abre o editor da 2ª barra de feitiços de cavaleiro.<br><br>"
				+ "[deathspell1 - Abre o editor da 1ª barra de feitiços de cavaleiro da morte.<br><br>"
				+ "[deathspell2 - Abre o editor da 2ª barra de feitiços de cavaleiro da morte.<br><br>"
				+ "[elementspell1 - Abre o editor da 1ª barra de feitiços elementais.<br><br>"
				+ "[elementspell2 - Abre o editor da 2ª barra de feitiços elementais.<br><br>"
				+ "[holyspell1 - Abre o editor da 1ª barra de preces de sacerdote.<br><br>"
				+ "[holyspell2 - Abre o editor da 2ª barra de preces de sacerdote.<br><br>"
				+ "[magespell1 - Abre o editor da 1ª barra de feitiços de mago.<br><br>"
				+ "[magespell2 - Abre o editor da 2ª barra de feitiços de mago.<br><br>"
				+ "[magespell3 - Abre o editor da 3ª barra de feitiços de mago.<br><br>"
				+ "[magespell4 - Abre o editor da 4ª barra de feitiços de mago.<br><br>"
				+ "[monkspell1 - Abre o editor da 1ª barra de habilidades de monge.<br><br>"
				+ "[monkspell2 - Abre o editor da 2ª barra de habilidades de monge.<br><br>"
				+ "[necrospell1 - Abre o editor da 1ª barra de feitiços de necromante.<br><br>"
				+ "[necrospell2 - Abre o editor da 2ª barra de feitiços de necromante.<br><br>"

				+ "<br><br>"

				+ "[archtool1 - Abre a 1ª barra de feitiços antigos.<br><br>"
				+ "[archtool2 - Abre a 2ª barra de feitiços antigos.<br><br>"
				+ "[archtool3 - Abre a 3ª barra de feitiços antigos.<br><br>"
				+ "[archtool4 - Abre a 4ª barra de feitiços antigos.<br><br>"
				+ "[monktool1 - Abre a 1ª barra de habilidades de monge.<br><br>"
				+ "[monktool2 - Abre a 2ª barra de habilidades de monge.<br><br>"
				+ "[bardtool1 - Abre a 1ª barra de canções de bardo.<br><br>"
				+ "[bardtool2 - Abre a 2ª barra de canções de bardo.<br><br>"
				+ "[knighttool1 - Abre a 1ª barra de feitiços de cavaleiro.<br><br>"
				+ "[knighttool2 - Abre a 2ª barra de feitiços de cavaleiro.<br><br>"
				+ "[deathtool1 - Abre a 1ª barra de feitiços de cavaleiro da morte.<br><br>"
				+ "[deathtool2 - Abre a 2ª barra de feitiços de cavaleiro da morte.<br><br>"
				+ "[elementtool1 - Abre a 1ª barra de feitiços elementais.<br><br>"
				+ "[elementtool2 - Abre a 2ª barra de feitiços elementais.<br><br>"
				+ "[holytool1 - Abre a 1ª barra de preces de sacerdote.<br><br>"
				+ "[holytool2 - Abre a 2ª barra de preces de sacerdote.<br><br>"
				+ "[magetool1 - Abre a 1ª barra de feitiços de mago.<br><br>"
				+ "[magetool2 - Abre a 2ª barra de feitiços de mago.<br><br>"
				+ "[magetool3 - Abre a 3ª barra de feitiços de mago.<br><br>"
				+ "[magetool4 - Abre a 4ª barra de feitiços de mago.<br><br>"
				+ "[monktool1 - Abre a 1ª barra de habilidades de monge.<br><br>"
				+ "[monktool2 - Abre a 2ª barra de habilidades de monge.<br><br>"
				+ "[necrotool1 - Abre a 1ª barra de feitiços de necromante.<br><br>"
				+ "[necrotool2 - Abre a 2ª barra de feitiços de necromante.<br><br>"

				+ "<br><br>"

				+ "[archclose1 - Fecha a 1ª barra de feitiços antigos.<br><br>"
				+ "[archclose2 - Fecha a 2ª barra de feitiços antigos.<br><br>"
				+ "[archclose3 - Fecha a 3ª barra de feitiços antigos.<br><br>"
				+ "[archclose4 - Fecha a 4ª barra de feitiços antigos.<br><br>"
				+ "[bardclose1 - Fecha a 1ª barra de canções de bardo.<br><br>"
				+ "[bardclose2 - Fecha a 2ª barra de canções de bardo.<br><br>"
				+ "[knightclose1 - Fecha a 1ª barra de feitiços de cavaleiro.<br><br>"
				+ "[knightclose2 - Fecha a 2ª barra de feitiços de cavaleiro.<br><br>"
				+ "[deathclose1 - Fecha a 1ª barra de feitiços de cavaleiro da morte.<br><br>"
				+ "[deathclose2 - Fecha a 2ª barra de feitiços de cavaleiro da morte.<br><br>"
				+ "[elementclose1 - Fecha a 1ª barra de feitiços elementais.<br><br>"
				+ "[elementclose2 - Fecha a 2ª barra de feitiços elementais.<br><br>"
				+ "[holyclose1 - Fecha a 1ª barra de preces de sacerdote.<br><br>"
				+ "[holyclose2 - Fecha a 2ª barra de preces de sacerdote.<br><br>"
				+ "[mageclose1 - Fecha a 1ª barra de feitiços de mago.<br><br>"
				+ "[mageclose2 - Fecha a 2ª barra de feitiços de mago.<br><br>"
				+ "[mageclose3 - Fecha a 3ª barra de feitiços de mago.<br><br>"
				+ "[mageclose4 - Fecha a 4ª barra de feitiços de mago.<br><br>"
				+ "[monkclose1 - Fecha a 1ª barra de habilidades de monge.<br><br>"
				+ "[monkclose2 - Fecha a 2ª barra de habilidades de monge.<br><br>"
				+ "[necroclose1 - Fecha a 1ª barra de feitiços de necromante.<br><br>"
				+ "[necroclose2 - Fecha a 2ª barra de feitiços de necromante.<br><br>"

				+ "<br><br>"

				+ "Música: Há muitas peças de música clássica diferentes no jogo, e elas tocam dependendo das áreas que você visita. Algumas das músicas são do jogo original, mas há algumas peças de jogos mais antigos. Há também algumas peças de jogos de computador dos anos 1990, mas elas realmente se encaixam no tema ao viajar pela terra. Você pode optar por ouvi-las ou mudar a música que está ouvindo ao explorar o mundo. Lembre-se de que, quando você muda a música e entra em uma nova área, a música padrão para aquela área será reproduzida e você pode ter que mudar sua música novamente. Lembre-se também de que seu cliente do jogo vai querer tocar a música por alguns segundos antes de permitir a troca para uma nova música. Você pode usar o comando abaixo para abrir uma janela que permite escolher uma música para tocar. Quase todas tocam em loop, exceto três que não e são marcadas com um asterisco. Há duas páginas de músicas para escolher, então use a seta superior para ir e voltar entre cada tela. Quando sua música começar a tocar, pressione o botão OK para sair da tela. Embora seja uma função desnecessária, ela lhe dá algum controle sobre a música no jogo.<br><br>"
				+ "[music - Abre a lista de reprodução e o player de música.<br><br>"
				+ "O comando abaixo simplesmente alternará sua preferência de música para tocar um conjunto diferente de músicas nas masmorras. Quando ativado, tocará a música que você normalmente ouve ao viajar pela terra, em vez da música comumente tocada em masmorras.<br><br>"
				+ "[musical - Define a música padrão da masmorra.<br><br>"

				+ "<br><br>"

				+ "Estilo Maligno: Há um elemento maligno no jogo no qual alguns desejam participar. Com classes como Necromantes, alguns jogadores podem querer viajar por um mundo com este sabor adicionado. Esta configuração específica permite que você alterne entre os sabores normal e maligno. No modo maligno, parte do tesouro que você encontrará frequentemente terá um nome que se encaixa no estilo maligno. Quando você permanece com karma negativo, os títulos de habilidade também mudarão para você, mas não todos. Consulte o livro de títulos de habilidade (encontrado no mundo do jogo) para ver quais títulos mudarão com base no karma. Algumas das relíquias que você encontrará também podem ter este estilo, para talvez decorar uma casa dessa forma. Esta opção pode ser ativada e desativada a qualquer momento. Você só pode ter um tipo de estilo de jogo ativo por vez.<br><br>"
				+ "[evil - Ativa/desativa o sabor maligno que o jogo oferece.<br><br>"

				+ "<br><br>"

				+ "Estilo Oriental: Há um elemento oriental no jogo no qual a maioria não deseja participar. Com classes como Ninja e Samurai, alguns jogadores podem querer viajar por um mundo com este sabor adicionado. Esta configuração específica permite que você alterne entre fantasia e oriental. No modo oriental, metade do tesouro que você encontrará será de origens históricas chinesas ou japonesas. Esses tipos de itens geralmente serão nomeados para combinar com o estilo. Itens que já pertenceram a alguém frequentemente terão um nome que se encaixa no estilo oriental. Alguns dos títulos de habilidade também mudarão para você, mas não todos. Consulte o livro de títulos de habilidade (encontrado no mundo do jogo) para ver quais títulos mudarão com base neste estilo de jogo. Algumas das relíquias e obras de arte que você encontrará também terão este estilo, para talvez decorar uma casa dessa forma. Esta opção pode ser ativada e desativada a qualquer momento. Você só pode ter um tipo de estilo de jogo ativo por vez.<br><br>"
				+ "[oriental - Ativa/desativa o sabor oriental que o jogo oferece.<br><br>"

				+ "<br><br>"

				+ "Estilo Bárbaro: O jogo padrão não se presta a uma experiência de espada e feitiçaria. Isso significa que não é a experiência de jogo mais ideal ser um bárbaro de tanga que vagueia pela terra com um enorme machado. Os personagens geralmente obtêm o máximo de equipamento possível para maximizar sua taxa de sobrevivência. Este estilo de jogo específico pode ajudar nesse aspecto. Escolher jogar neste estilo fará com que uma alforje apareça em sua mochila principal. Você não pode armazenar nada nesta alforje, pois seu propósito é mudar certas peças de equipamento que você coloca nela. Ele mudará escudos, chapéus, elmos, túnicas, mangas, calças, botas, gorjeiras, luvas, colares, capas e vestimentas. Quando esses itens são alterados, eles se tornarão algo que aparece de forma diferente, mas se comporta da mesma forma que o item anterior. Esses itens diferentes podem ser equipados, mas podem não aparecer em seu personagem. Observe também que, quando você usa vestimentas, elas cobrem as túnicas e mangas do seu personagem. Usar uma vestimenta de espada e feitiçaria fará a mesma coisa, então você terá que remover a vestimenta para acessar as mangas e/ou túnica. Este estilo de jogo também tem seu próprio conjunto de títulos de habilidade para muitas habilidades. Se você está jogando com um personagem feminino, pressionar o botão adicional converterá quaisquer títulos 'Bárbaro' para 'Amazona'. Você pode abrir sua alforje para aprender mais sobre este estilo de jogo. Esta opção pode ser ativada e desativada a qualquer momento. Você só pode ter um tipo de estilo de jogo ativo por vez.<br><br>"
				+ "[barbaric - Ativa/desativa o sabor bárbaro que o jogo oferece.<br><br>"

			+ "";

			return HelpText;
		}
	}
}

namespace Server.Gumps
{
    public class InfoHelpGump : Gump
    {
		public int m_Origin;

        public InfoHelpGump( Mobile from, int page, int origin ) : base( 50, 50 )
        {
			m_Origin = origin;

            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			string title = "";
			string info = "";
			bool scrollbar = true;

			if ( page == 82 )
			{
				scrollbar = false;
				title = "Tom Musical";
				info = "Esta opção simplesmente alternará sua preferência de música para tocar um conjunto diferente de músicas nas masmorras. Quando ativado, tocará a música que você normalmente ouve ao viajar pela terra, em vez da música comumente tocada em masmorras.";
			}
			else if ( page == 83 )
			{
				title = "Lista de Reprodução";
				info = "Isso lhe dá uma lista completa das músicas do jogo. Você pode selecionar as músicas que gosta e essas escolhas tocarão aleatoriamente conforme você vai de região para região. Para ouvir uma música para avaliação, selecione o ícone de gema azul. Observe que o cliente tem um tempo de atraso quando você pode começar outra música, então selecionar a gema azul pode não responder se você começou uma música muito cedo antes disso. Aguarde alguns segundos e tente clicar na gema azul novamente para ver se essa música começa a tocar. As listas de reprodução estão desativadas por padrão, então se você quer que sua lista de reprodução funcione, certifique-se de ativá-la.";
			}
			else if ( page == 84 )
			{
				scrollbar = false;
				title = "Jogo Privado";
				info = "Esta opção ativa ou desativa as mensagens detalhadas de sua jornada para o pregoeiro e conversas de cidadãos locais. Isso mantém suas atividades privadas para que outros não vejam onde você está viajando pelo mundo.";
			}
			else if ( page == 85 )
			{
				title = "Opções de Saque";
				info = "Isso permite que você selecione em uma lista de categorias, onde elas coletarão automaticamente esses tipos de itens de baús de masmorras comuns ou cadáveres e os colocarão em sua mochila. Se você selecionar moedas, você pegará riquezas na forma de moeda ou pepitas de ouro. Se você pegar gemas e joias, isso consistirá em gemas, pedras preciosas, joias, jóias e cristais. Os itens desconhecidos são aqueles que precisarão de identificação, mas você pode decidir pegá-los mesmo assim. As opções de reagentes têm algumas categorias. Reagentes de magia e necromancia são aqueles usados especificamente por esses personagens, onde reagentes de poções de bruxaria se enquadram na categoria de necromancia. Reagentes alquímicos são aqueles que estão fora da categoria de reagentes de magia e necromancia, e apenas alquimistas os usam. Reagentes de herbalismo são úteis no herbalismo druídico.";
			}
			else if ( page == 86 )
			{
				title = "Envenenamento Clássico";
				info = "Existem dois métodos que os assassinos usam para lidar com armas envenenadas. Um é o método simples de mergulhar a lâmina e fazê-la envenenar sempre que atinge seu oponente. Com este método, conhecido como envenenamento clássico, há pouco controle sobre a dosagem dada, mas é mais fácil de manobrar. Quando esta opção está desativada, tem o método mais novo e tático, onde apenas certas armas podem ser envenenadas e o assassino pode controlar quando o veneno é administrado com o golpe. Embora o método tático exija mais pensamento, ele tem o potencial de permitir que um assassino envenene certas flechas, por exemplo. A escolha dos métodos pode ser alternada a qualquer momento, mas apenas um método pode estar em uso por vez.";
			}
			else if ( page == 87 )
			{
				title = "Título de Habilidade";
				info = "Quando você não define seu título de habilidade aqui, o jogo pegará sua habilidade mais alta e fará disso o título do seu personagem. Escolher uma habilidade aqui forçará seu título para aquela profissão. Então, se você sempre quer ser conhecido como um mago, selecione a opção 'Magia' (por exemplo). Você pode deixar o jogo gerenciar isso a qualquer momento, voltando para 'Título Automático'. Esteja avisado ao escolher uma habilidade: se você tiver zero pontos de habilidade nela, você será intitulado 'o Idiota da Vila'. Se você obtiver pelo menos 0.1, você será pelo menos 'Aspirante'.";
			}
			else if ( page == 88 )
			{
				scrollbar = false;
				title = "Cor da Mensagem";
				info = "Por padrão, a maioria das mensagens que aparecem no canto inferior esquerdo da tela são de cor cinza. Ativar esta opção mudará essas mensagens para ter uma cor aleatória sempre que uma nova mensagem aparecer. Este recurso pode ajudar alguns a ver essas mensagens com mais facilidade, e as cores variadas também podem ajudar a distinguir mensagens individuais que podem estar passando rapidamente.";
			}
			else if ( page == 89 )
			{
				scrollbar = false;
				title = "Ataque Automático";
				info = "Por padrão, quando você é atacado, você automaticamente revida. Se você quiser decidir quando ou se quer revidar, pode desativar esta opção. Isso pode ser útil se você não quiser matar inocentes acidentalmente, ou se estiver tentando domar uma criatura irritada.";
			}
			else if ( page == 92 )
			{
				title = "Estilo de Jogo - Normal";
				info = "Este é o estilo de jogo padrão para " + MySettings.S_ServerName + ". É projetado para uma experiência de mundo de fantasia clássica para os jogadores. Existem outros dois estilos de jogo disponíveis, maligno e oriental. Os estilos de jogo não mudam a mecânica da experiência de jogo, mas mudam o sabor do tesouro que você encontra e dos capangas que você contrata. Por exemplo, você pode definir seu estilo de jogo para um estilo de jogo 'maligno'. O que acontece é que você encontrará tesouros voltados para esse estilo de jogo. Onde você normalmente encontraria um 'maça do poder' azul, o estilo maligno faria você encontrar uma 'maça da morte fantasmagórica' preta. Eles são simplesmente uma maneira de ajustar a experiência do seu personagem no jogo.";
			}
			else if ( page == 93 )
			{
				title = "Estilo de Jogo - Maligno";
				info = "Há um elemento maligno no jogo no qual alguns desejam participar. Com classes como Necromantes, alguns jogadores podem querer viajar por um mundo com este sabor adicionado. Esta configuração específica permite que você alterne entre os sabores normal e maligno. No modo maligno, parte do tesouro que você encontrará frequentemente terá um nome que se encaixa no estilo maligno. Quando você permanece com karma negativo, os títulos de habilidade também mudarão para você, mas não todos. Consulte o livro de títulos de habilidade (encontrado no mundo do jogo) para ver quais títulos mudarão com base no karma. Algumas das relíquias que você encontrará também podem ter este estilo, para talvez decorar uma casa dessa forma. Esta opção pode ser ativada e desativada a qualquer momento. Você só pode ter um tipo de estilo de jogo ativo por vez.<br><br>"
				+ "[evil - Ativa/desativa o sabor maligno que o jogo oferece.";
			}
			else if ( page == 94 )
			{
				title = "Estilo de Jogo - Oriental";
				info = "Há um elemento oriental no jogo no qual a maioria não deseja participar. Com classes como Ninja e Samurai, alguns jogadores podem querer viajar por um mundo com este sabor adicionado. Esta configuração específica permite que você alterne entre fantasia e oriental. No modo oriental, grande parte do tesouro que você encontrará será de origens históricas chinesas ou japonesas. Esses tipos de itens geralmente serão nomeados para combinar com o estilo. Itens que já pertenceram a alguém frequentemente terão um nome que se encaixa no estilo oriental. Alguns dos títulos de habilidade também mudarão para você, mas não todos. Consulte o livro de títulos de habilidade (encontrado no mundo do jogo) para ver quais títulos mudarão com base neste estilo de jogo. Algumas das relíquias e obras de arte que você encontrará também terão este estilo, para talvez decorar uma casa dessa forma. Esta opção pode ser ativada e desativada a qualquer momento. Você só pode ter um tipo de estilo de jogo ativo por vez.";
			}
			else if ( page == 95 )
			{
				m_Origin = 7;
				title = "Barras de Ferramentas Mágicas";
				info = "Estas barras de ferramentas podem ser configuradas para todas as áreas de feitiços de estilo mágico no jogo. Cada escola de magia tem duas barras de ferramentas separadas que você pode personalizar, exceto para magia, que tem quatro disponíveis. O grande número de feitiços para magia se beneficia das duas barras de ferramentas extras. Essas barras de ferramentas permitem que você selecione feitiços que gosta de lançar com frequência e defina se a barra aparecerá vertical ou horizontalmente. Se você escolher que a barra de ferramentas apareça vertical, você tem a opção adicional de mostrar os nomes dos feitiços ao lado dos ícones. Essas barras de ferramentas podem ser movidas e você só precisa clicar uma vez no ícone apropriado para lançar o feitiço. Se você tiver feitiços selecionados para uma barra de ferramentas, mas não tiver o feitiço em seu livro de feitiços, o ícone não aparecerá quando você abrir a barra de ferramentas. Essas barras de ferramentas não podem ser fechadas por meios normais, para evitar que você as feche acidentalmente durante o combate. Você pode usar o botão de comando disponível na seção 'Ajuda' ou o comando de teclado apropriado.";
			}
			else if ( page == 96 )
			{
				scrollbar = false;
				title = "Cor do Feitiço de Magia";
				info = "Você pode mudar a cor de todos os seus efeitos de feitiços de magia aqui. Há um número limitado de opções fornecidas aqui. Uma vez definido, seus feitiços serão dessa cor para cada efeito. Se você quiser voltar ao normal, selecione a opção 'Padrão'. Você também pode usar o comando '[spellhue' seguido por um número de qualquer cor que desejar definir.";
			}
			else if ( page == 97 )
			{
				scrollbar = false;
				title = "Título Personalizado";
				info = "Isso permite que você insira um título personalizado para seu personagem, em vez de depender do jogo para atribuir um com base em sua melhor habilidade ou na habilidade que você escolhe para representá-lo. Para limpar um título personalizado que você possa ter definido com esta opção, digite a palavra 'clear' para removê-lo.";
			}
			else if ( page == 99 )
			{
				scrollbar = false;
				title = "Nomes de Habilidades de Armas";
				info = "Quando você se torna bom o suficiente em táticas e um tipo de arma, você obterá habilidades especiais que pode realizar. Elas geralmente aparecem como ícones simples que você pode selecionar para realizar a ação, mas esta opção ativará ou desativará os nomes das habilidades especiais de armas ao lado dos ícones apropriados.";
			}
			else if ( page == 100 )
			{
				scrollbar = false;
				title = "Embainhamento Automático";
				info = "Esta opção ativa ou desativa a função de embainhar sua arma quando não está em batalha. Quando você coloca seu personagem de volta no modo de guerra, ele sacará a arma.";
			}
			else if ( page == 101 )
			{
				scrollbar = false;
				title = "Imagens de Fundo";
				info = "Muitas janelas de gump têm uma imagem desbotada no fundo. Desativar isso fará com que essas janelas sejam apenas de cor preta, sem imagem de fundo.";
			}
			else if ( page == 102 )
			{
				scrollbar = false;
				title = "Barra de Habilidades de Armas";
				info = "Esta opção ativa ou desativa a abertura automática da barra de ícones de habilidades de armas, o que significa que você terá que fazê-lo manualmente se a desativar.";
			}
			else if ( page == 103 )
			{
				scrollbar = false;
				title = "Magia de Criaturas";
				info = "Algumas criaturas têm uma habilidade natural para magia. Esta configuração permite que você mude para qual escola de magia você quer focar: magia, necromancia ou elementalismo. Isso permite que criaturas de magia ou necromancia movam seu foco para o elementalismo, ou alternem entre magia e necromancia.";
			}
			else if ( page == 104 )
			{
				scrollbar = false;
				title = "Tipo de Criatura";
				info = "Algumas espécies de criaturas têm mais de uma opção de aparência. Esta configuração permite que você mude para outra dessa espécie se outra aparência estiver disponível. Você também pode se transformar em humano, se escolher. Se você se tornar humano, permanecerá assim para sempre.";
			}
			else if ( page == 105 )
			{
				scrollbar = false;
				title = "Sons de Criaturas";
				info = "Como você é uma criatura, às vezes você faz sons ao atacar ou ser ferido por ataques. Você pode ativar ou desativar esses sons aqui.";
			}
			else if ( page == 106 )
			{
				scrollbar = false;
				title = "Livro de Feitiços Antigos";
				info = "Se você começar a pesquisar os 64 feitiços antigos que foram há muito esquecidos, ativar esta configuração significa que você lançará essa magia de um livro em vez de usar sua bolsa de pesquisa. Se você tiver isso ativado, precisará de reagentes para lançar feitiços e os feitiços lançados devem estar em seu livro. Desativar isso verifica sua bolsa de pesquisa para ver se você tem o feitiço preparado antecipadamente.";
			}
			else if ( page == 107 )
			{
				scrollbar = false;
				title = "Definir Recipiente de Criação";
				info = "Isso permite que você defina um recipiente, onde os itens irão quando você os estiver criando através de crafting. O recipiente deve estar em sua mochila principal para coletar os itens, e não dentro de outro recipiente.";
			}
			else if ( page == 108 )
			{
				scrollbar = false;
				title = "Definir Recipiente de Coleta";
				info = "Isso permite que você defina um recipiente, onde os itens irão quando você estiver coletando itens. Estes são itens que você obtém de atividades como mineração, corte de madeira e pesca. O recipiente deve estar em sua mochila principal para coletar os itens, e não dentro de outro recipiente.";
			}
			else if ( page == 109 )
			{
				scrollbar = false;
				title = "Definir Recipiente de Saque";
				info = "Isso permite que você defina um recipiente, onde os itens irão que você configurou na configuração de Opções de Saque. O recipiente deve estar em sua mochila principal para coletar os itens, e não dentro de outro recipiente.";
			}
			else if ( page == 110 )
			{
				scrollbar = false;
				title = "Recursos Ordinários";
				info = "Ativar esta configuração fará com que seu personagem apenas colete ou reúna recursos ordinários como madeira, couro, granito, ferro e ossos. Isso significa que você não coletará itens de recursos superiores ao esfolar, minerar ou cortar madeira.";
			}
			else if ( page == 111 )
			{
				scrollbar = false;
				title = "Clique Duas Vezes para ID de Itens";
				info = "Ativar isso permitirá que seu personagem tente identificar itens clicando duas vezes neles.<BR><BR>NOTA: se você estiver usando qualquer software de terceiros que tente abrir todos os seus recipientes, esse software de terceiros tentará identificar esses itens sem o seu consentimento.";
			}
			else if ( page == 198 )
			{
				title = "Estilo de Jogo - Bárbaro";
				info = "O jogo padrão não se presta a uma experiência de espada e feitiçaria. Isso significa que não é a experiência de jogo mais ideal ser um bárbaro de tanga que vagueia pela terra com um enorme machado. Os personagens geralmente obtêm o máximo de equipamento possível para maximizar sua taxa de sobrevivência. Este estilo de jogo específico pode ajudar nesse aspecto. Escolher jogar neste estilo fará com que uma alforje apareça em sua mochila principal. Você não pode armazenar nada nesta alforje, pois seu propósito é mudar certas peças de equipamento que você coloca nela. Ele mudará escudos, chapéus, elmos, túnicas, mangas, calças, botas, gorjeiras, luvas, colares, capas e vestimentas. Quando esses itens são alterados, eles se tornarão algo que aparece de forma diferente, mas se comporta da mesma forma que o item anterior. Esses itens diferentes podem ser equipados, mas podem não aparecer em seu personagem. Observe também que, quando você usa vestimentas, elas cobrem as túnicas e mangas do seu personagem. Usar uma vestimenta de espada e feitiçaria fará a mesma coisa, então você terá que remover a vestimenta para acessar as mangas e/ou túnica. Este estilo de jogo também tem seu próprio conjunto de títulos de habilidade para muitas habilidades. Se você está jogando com um personagem feminino, pressionar o botão adicional converterá quaisquer títulos 'Bárbaro' para 'Amazona'. Você pode abrir sua alforje para aprender mais sobre este estilo de jogo. Esta opção pode ser ativada e desativada a qualquer momento. Você só pode ter um tipo de estilo de jogo ativo por vez.";
			}
			else if ( page == 199 )
			{
				title = "Listas de Habilidades";
				info = "Listas de habilidades são uma alternativa às listas de habilidades normais que você pode obter clicando no botão apropriado no paperdoll. Embora você ainda precise usar isso para gerenciamento de habilidades (up, down, lock), as listas de habilidades têm uma aparência mais condensada para quando você joga o jogo. Para que as habilidades apareçam nesta lista alternativa, elas devem estar definidas como 'up', ou podem estar definidas como 'locked'. As habilidades 'locked' só serão exibidas nesta lista se você alterar suas configurações aqui para refletir isso. A lista não é atualizada em tempo real, mas frequentemente se atualizará para mostrar o status de suas habilidades em valores reais e aprimorados. Qualquer habilidade que apareça em laranja indica uma habilidade que você bloqueou. Você pode abrir esta lista com o comando '[skilllist', ou o botão apropriado na tela principal.";
			}
			else if ( page == 1000 )
			{
				title = "Virar Escritura";
				info = "Esta opção permite que você vire algumas escrituras que podem vir em uma de duas direções de face. Então, se uma escritura afirma que um móvel fica de frente para o leste, você pode colocar a escritura no chão de sua casa e virá-la para ficar de frente para o sul. Isso pode virar quase qualquer item semelhante a escritura dessa maneira, mas nem todos os itens são chamados de 'escrituras' ou parecem escrituras. Alguns itens se comportam como escrituras e esses podem ser virados da mesma maneira. Tendas ou tapetes de urso, por exemplo, têm uma face e você pode virá-los com este comando.";
			}

			AddPage(0);

			string color = "#ddbc4b";

			AddImage(0, 0, 9577, Server.Misc.PlayerSettings.GetGumpHue( from ));
			AddHtml( 12, 12, 239, 20, @"<BODY><BASEFONT Color=" + color + ">" + title + "</BASEFONT></BODY>", (bool)false, (bool)false);
			AddHtml( 12, 43, 278, 212, @"<BODY><BASEFONT Color=" + color + ">" + info + "</BASEFONT></BODY>", (bool)false, (bool)scrollbar);
			AddButton(268, 9, 4017, 4017, 0, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
			from.SendSound( 0x4A );
			from.CloseGump( typeof( Server.Engines.Help.HelpGump ) );
			if ( m_Origin != 999 ){ from.SendGump( new Server.Engines.Help.HelpGump( from, m_Origin ) ); }
        }
    }
}
