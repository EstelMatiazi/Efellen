using System;
using System.Collections.Generic;
using Server;
using Server.Targeting;
using Server.Items;
using Server.Network;
using Server.ContextMenus;
using Server.Gumps;
using Server.Misc;
using Server.Mobiles;
using Server.Custom.DefenderOfTheRealm.Vow;

namespace Server.Mobiles
{
	public class ThiefGuildmaster : BaseGuildmaster
	{
		public override NpcGuild NpcGuild{ get{ return NpcGuild.ThievesGuild; } }

		private static Dictionary<Mobile, DateTime> m_LastContrabandTurnIn = new Dictionary<Mobile, DateTime>();

		[Constructable]
		public ThiefGuildmaster() : base( "thief" )
		{
			SetSkill( SkillName.Searching, 75.0, 98.0 );
			SetSkill( SkillName.Hiding, 65.0, 88.0 );
			SetSkill( SkillName.Lockpicking, 85.0, 100.0 );
			SetSkill( SkillName.Snooping, 90.0, 100.0 );
			SetSkill( SkillName.Stealing, 90.0, 100.0 );
			SetSkill( SkillName.Fencing, 75.0, 98.0 );
			SetSkill( SkillName.Stealth, 85.0, 100.0 );
			SetSkill( SkillName.RemoveTrap, 85.0, 100.0 );
			AddItem( new Server.Items.Cloak() );
			AddItem(new Server.Items.Artifact_ShadowBrokerArms());
			AddItem(new Server.Items.Artifact_ShadowBrokerCap());
			AddItem(new Server.Items.Artifact_ShadowBrokerGloves());
			AddItem(new Server.Items.Artifact_ShadowBrokerGorget());
			AddItem(new Server.Items.Artifact_ShadowBrokerLeggings());
			AddItem(new Server.Items.Artifact_ShadowBrokerTunic());
		}

		public override void InitSBInfo( Mobile m )
		{
			m_Merchant = m;
			SBInfos.Add( new MyStock() );
		}

		public override bool HandlesOnSpeech( Mobile from ) 
		{ 
			return true; 
		} 

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;

            if (from == null || !(from is PlayerMobile))
                return;
          
            if( e.Mobile.InRange( this, 4 ))
			{
			    if (e.Speech.IndexOf("reward") >= 0)
                {
					if (from is PlayerMobile && ((PlayerMobile)from).NpcGuild == NpcGuild.ThievesGuild)
                    {
                        from.SendGump(new Server.Custom.DefenderOfTheRealm.RewardGump(from, 3, 0));
                        Say("These are the rewards I can offer you, friend.");
                    }
                    else
                    {
                        Say("I don't do business outside of the guild, friend.");
                    }
                }
			    else 
			    { 
			        base.OnSpeech( e ); 
			    }
			}
        }

		public class MyStock: SBInfo
		{
			private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
			private IShopSellInfo m_SellInfo = new InternalSellInfo();

			public MyStock()
			{
			}

			public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
			public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

			public class InternalBuyInfo : List<GenericBuyInfo>
			{
				public InternalBuyInfo()
				{
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.All,			ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Thief,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.All,			ItemSalesInfo.Material.All,		ItemSalesInfo.Market.All,		ItemSalesInfo.World.None,	typeof( DisguiseKit )	 );
				}
			}

			public class InternalSellInfo : GenericSellInfo
			{
				public InternalSellInfo()
				{
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.All,			ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Thief,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.All,			ItemSalesInfo.Material.All,		ItemSalesInfo.Market.All,		ItemSalesInfo.World.None,	typeof( DisguiseKit )	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.All,			ItemSalesInfo.Material.All,		ItemSalesInfo.Market.All,		ItemSalesInfo.World.None,	typeof( CommonContrabandBox )	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.All,			ItemSalesInfo.Material.All,		ItemSalesInfo.Market.All,		ItemSalesInfo.World.None,	typeof( UncommonContrabandBox )	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.All,			ItemSalesInfo.Material.All,		ItemSalesInfo.Market.All,		ItemSalesInfo.World.None,	typeof( RareContrabandBox )	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.All,			ItemSalesInfo.Material.All,		ItemSalesInfo.Market.All,		ItemSalesInfo.World.None,	typeof( VeryRareContrabandBox )	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.All,			ItemSalesInfo.Material.All,		ItemSalesInfo.Market.All,		ItemSalesInfo.World.None,	typeof( ExtremelyRareContrabandBox )	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.All,			ItemSalesInfo.Material.All,		ItemSalesInfo.Market.All,		ItemSalesInfo.World.None,	typeof( LegendaryContrabandBox )	 );
				}
			}
		}


		public override void SayWelcomeTo( Mobile m )
		{
			SayTo( m, 1008053 ); // Welcome to the guild! Stay to the shadows, friend.
		}

		private class JobEntry : ContextMenuEntry
		{
			private ThiefGuildmaster m_ThiefGuildmaster;
			private Mobile m_From;

			public JobEntry( ThiefGuildmaster ThiefGuildmaster, Mobile from ) : base( 2078, 3 )
			{
				m_ThiefGuildmaster = ThiefGuildmaster;
				m_From = from;
				Enabled = m_ThiefGuildmaster.CheckVendorAccess( from );
			}

			public override void OnClick()
			{
				m_ThiefGuildmaster.FindMessage( m_From );
			}
		}

        public void FindMessage( Mobile m )
        {
            if ( Deleted || !m.Alive )
                return;

			Item note = Server.Items.ThiefNote.GetMyCurrentJob( m );

			if ( note != null )
			{
				ThiefNote job = (ThiefNote)note;
				m.AddToBackpack( note );
				m.PlaySound( 0x249 );
				SayTo(m, "Hmmm...you already have a job from " + job.NoteItemPerson + ". Here is a copy if you lost it.");
			}
			else
			{
				ThiefNote task = new ThiefNote();
				Server.Items.ThiefNote.SetupNote( task, m );
				m.AddToBackpack( task );
				m.PlaySound( 0x249 );
				SayTo(m, "Here is something I think you can handle.");
            }
        }

		public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( from.Alive && !from.Blessed )
			{
				list.Add( new JobEntry( this, from ) );
			}

			base.AddCustomContextEntries( from, list );
		}

		public ThiefGuildmaster( Serial serial ) : base( serial )
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
			int version = reader.ReadInt();
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if(dropped is Gold || dropped is BankCheck)
			{
				ProcessGuild( from, dropped );
			}
			else if (dropped is ContrabandBox)
    		{
				DateTime lastTime;
				PlayerMobile pm = (PlayerMobile)from;
        		if (m_LastContrabandTurnIn.TryGetValue(from, out lastTime))
        		{
        		    TimeSpan remaining = (lastTime + TimeSpan.FromHours(1)) - DateTime.UtcNow;

        		    if (remaining > TimeSpan.Zero)
        		    {
        		        SayTo(from, "I'm still waiting on the buyer for the last one. Give me about {0} minute{1}.",
        		            (int)Math.Ceiling(remaining.TotalMinutes),
        		            remaining.TotalMinutes > 1 ? "s" : "");
        		        return false;
        		    }
        		}

	    		if (pm == null || pm.NpcGuild != NpcGuild.ThievesGuild)
	    		{
	    		    SayTo(from, "Sorry but I do not do business with those I don't trust. Guild members only, {0}.", from.Name);
	    		    return false;
	    		}

        		ContrabandBox box = (ContrabandBox)dropped;
        		string[] messages = GetMessageForBox(box);

    			if (messages.Length > 0)
    		    	{
    		    	    SayTo(from, messages[Utility.Random(messages.Length)]);
    		    	    dropped.Delete(); 
						m_LastContrabandTurnIn[from] = DateTime.UtcNow;

						RewardPlayer(from,box);
    		    	    return true;
    		    	}
    		}
			return base.OnDragDrop( from, dropped );
		}

		private string[] GetMessageForBox(ContrabandBox box)
		{
			if (box is LegendaryContrabandBox)
			{
				return new string[]
				{
					"We will be rich my friend, rich beyond our wildest dreams!",
					"I can't believe you managed to snatch one of those! You did the guild a great favor, my friend!",
					"They'll be telling stories about this one in every tavern for years to come!",
					"This is the score of a lifetime, friend!"
				};
			}
			else if (box is ExtremelyRareContrabandBox)
			{
				return new string[]
				{
					"This will take all my kids through college!",
					"I might be looking to retire after I pass this one along.",
					"Careful with that one—it might be a lot of trouble. I hope no one can trace this back to us",
					"Top-tier stuff, I'll need to make some calls. You focus on celebrating. You earned it.",
					"This is worth its weight in platinum.",
					"You might just be the best thief in the realm!"
				};
			}
			else if (box is VeryRareContrabandBox || box is RareContrabandBox)
			{
				return new string[]
				{
					"You make the guild proud, my friend.",
					"Ah...This will fetch a nice price.",
					"It's not often you see these around... I know someone that will be very happy to receive this.",
					"A find like this doesn't come around often.",
					"I'm sure that this one will a some noble sweat...",
					"It's got that special shine to it...Doesn't it?"
				};
			}
			else if (box is UncommonContrabandBox || box is CommonContrabandBox)
			{
				return new string[]
				{
					"A day's work for a day's pay, eh?",
					"I think I know someone that might be interested in this.",
					"Not bad for an honest day's crime.",
					"Keeps the network running, these little ones.",
					"I don't think that this one will be missed too much."
				};
			}

			return new string[0];
		}

		private static void RewardPlayer(Mobile mobile, Item box)
		{
		    if (mobile == null || box == null)
		        return;

		    Container rewardBag = new Bag();
		    rewardBag.Name = "Ill gotten gains";
		    rewardBag.Hue = Utility.RandomDyedHue();

		    int luck = mobile.Luck;

		    if (box is CommonContrabandBox)
		    {
				VowRewardHelper.GenerateRewards( mobile, 5, rewardBag, VowType.Shadowbroker );
				rewardBag.DropItem(new MarksOfTheShadowbroker(Utility.RandomMinMax(10, 25)));
		    }
		    else if (box is UncommonContrabandBox)
		    {
		    	VowRewardHelper.GenerateRewards( mobile, 10, rewardBag, VowType.Shadowbroker );
				rewardBag.DropItem(new MarksOfTheShadowbroker(Utility.RandomMinMax(35, 75)));
		    }
		    else if (box is RareContrabandBox)
		    {
		        VowRewardHelper.GenerateRewards( mobile, 20, rewardBag, VowType.Shadowbroker );
				rewardBag.DropItem(new MarksOfTheShadowbroker(Utility.RandomMinMax(105, 145)));
		    }
		    else if (box is VeryRareContrabandBox)
		    {
				VowRewardHelper.GenerateRewards( mobile, 30, rewardBag, VowType.Shadowbroker );
				rewardBag.DropItem(new MarksOfTheShadowbroker(Utility.RandomMinMax(185, 225)));
		    }
		    else if (box is ExtremelyRareContrabandBox)
		    {
		        VowRewardHelper.GenerateRewards( mobile, 40, rewardBag, VowType.Shadowbroker );
				rewardBag.DropItem(new MarksOfTheShadowbroker(Utility.RandomMinMax(285, 345)));
		    }
		    else if (box is LegendaryContrabandBox)
		    {
		        VowRewardHelper.GenerateRewards( mobile, 50, rewardBag, VowType.Shadowbroker );
				rewardBag.DropItem(new MarksOfTheShadowbroker(Utility.RandomMinMax(425, 500)));
		    }
		    mobile.AddToBackpack(rewardBag);
			mobile.SendMessage("The Guildmaster rewards you for your skill and discretion.");
			Effects.PlaySound(mobile.Location, mobile.Map, 0x32);

			int fame = 0;

			if (box is CommonContrabandBox)
			    fame = Utility.RandomMinMax(10, 50);
			else if (box is UncommonContrabandBox)
			    fame = Utility.RandomMinMax(60, 120);
			else if (box is RareContrabandBox)
			    fame = Utility.RandomMinMax(130, 190);
			else if (box is VeryRareContrabandBox)
			    fame = Utility.RandomMinMax(250, 350);
			else if (box is ExtremelyRareContrabandBox)
			    fame = Utility.RandomMinMax(600, 800);
			else if (box is LegendaryContrabandBox)
			    fame = Utility.RandomMinMax(1200, 1800);

			Titles.AwardFame(mobile, fame, false);
			LoggingFunctions.LogStandard( mobile, "has smuggled a " + box.Name + "!" );

		}
	}
}