using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Items;

namespace Server.Custom.DefenderOfTheRealm
{
    public class RewardGump : Gump
    {
        private Mobile m_From;
        private RewardInfo[] m_Rewards;
        private int m_Page;        
        /*
            1 - defender of the realm
            2 - scourge of the realm
            3 - thief guildmaster
        */
        private int type;
        private int m_Hue;
        private int m_Type;
        private string m_CurrencyType;


        public RewardGump(Mobile from, int type, int page) : base(50, 50)
        {
            this.type = type;
            m_From = from;
            m_Page = page;
            m_Type = type;
            int hue = 0;

            List<RewardInfo> list = new List<RewardInfo>();
            list.AddRange(RewardTables.CommonRewards);
            
            AddBackground(0, 0, 420, 420, 9270);
           
            switch (m_Type)
            {
                case 1:
                    AddLabel(160, 20, 1152, "Defender of the Realm Rewards");
                    m_CurrencyType = "Marks of Honor";
                    m_Hue = 53;
                    list.AddRange(RewardTables.DefenderRewards);
                    break;

                case 2:
                    AddLabel(160, 20, 1152, "Scourge of the Realm Rewards");
                    m_CurrencyType = "Marks of the Scourge";
                    m_Hue = 37;
                    list.AddRange(RewardTables.ScourgeRewards);
                    break;

                case 3:
                    AddLabel(160, 20, 1152, "Shadowbroker Rewards");
                    m_CurrencyType = "Marks of the Shadowbroker";
                    m_Hue = 1109;
                    list.AddRange(RewardTables.ShadowbrokerRewards);
                    break;

                default:
                    AddLabel(160, 20, 1152, "Rewards");
                    m_CurrencyType = "Marks";
                    m_Hue = 0;
                    break;
            }
            m_Rewards = list.ToArray();
            int perPage = 6;
            int start = page * perPage;
            int end = Math.Min(start + perPage, m_Rewards.Length);

            int x = 30, y = 60;
            for (int i = start; i < end; i++)
            {
                RewardInfo info = m_Rewards[i];
                int buttonID = 1000 + i;
                int itemHue = info.Hue > 0 ? info.Hue : (info.Hueable ? m_Hue : 0);
                AddItem(x, y, info.ItemID, itemHue);
                AddLabel(x + 50, y, 1152, info.Name);
                AddLabel(x + 50, y + 20, 1153, "Cost: " + info.Cost + " " + m_CurrencyType);
                AddButton(x + 300, y, 4005, 4007, buttonID, GumpButtonType.Reply, 0);

                y += 50;
            }


            if (page > 0)
                AddButton(100, 380, 4014, 4016, 1, GumpButtonType.Reply, 0); // prev
            if (end < m_Rewards.Length)
                AddButton(250, 380, 4005, 4007, 2, GumpButtonType.Reply, 0); // next
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 1) // prev
                m_From.SendGump(new RewardGump(m_From, m_Type, m_Page - 1));
            else if (info.ButtonID == 2) // next
                m_From.SendGump(new RewardGump(m_From, m_Type, m_Page + 1));
            else if (info.ButtonID >= 1000)
            {
                int index = info.ButtonID - 1000;
                if (index >= 0 && index < m_Rewards.Length)
                    m_From.SendGump(new RewardConfirmGump(m_From, m_Rewards[index], m_Type));
            }
        }
    }
}
