using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Items;

namespace Server.Custom.DefenderOfTheRealm
{
    public class RewardConfirmGump : Gump
    {
        private readonly Mobile m_From;
        private readonly RewardInfo m_Info;
        private readonly int m_Type;
        private readonly string m_CurrencyType;
        private readonly int m_Hue;

        public RewardConfirmGump(Mobile from, RewardInfo info, int type) : base(100, 100)
        {
            m_From = from;
            m_Info = info;
            m_Type = type;

            switch (m_Type)
            {
                case 1:
                    m_CurrencyType = "Marks of Honor";
                    m_Hue = 53;
                    break;
                case 2:
                    m_CurrencyType = "Marks of the Scourge";
                    m_Hue = 37;
                    break;
                case 3:
                    m_CurrencyType = "Marks of the Shadowbroker";
                    m_Hue = 1109;
                    break;
                default:
                    m_CurrencyType = "Marks";
                    m_Hue = 0;
                    break;
            }

            int itemHue = info.Hue > 0 ? info.Hue : (info.Hueable ? m_Hue : 0);

           
            AddBackground(0, 0, 400, 200, 9270);
            AddLabel(120, 20, 1152, "Confirm your selection");

            AddItem(60, 60, info.ItemID,itemHue);
            AddLabel(130, 60, 1152, info.Name);
            AddLabel(130, 80, 1153, "Cost: " + info.Cost+ " " + m_CurrencyType);

            AddButton(50, 150, 4005, 4007, 1, GumpButtonType.Reply, 0); // Yes
            AddLabel(90, 150, 1152, "Yes");

            AddButton(150, 150, 4017, 4019, 2, GumpButtonType.Reply, 0); // No
            AddLabel(190, 150, 1152, "No");
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID != 1)
                return;

            Type markType = null;

            switch (m_Type)
            {
                case 1:
                    markType = typeof(MarksOfHonor);
                    break;
                case 2:
                    markType = typeof(MarksOfTheScourge);
                    break;
                case 3:
                    markType = typeof(MarksOfTheShadowbroker);
                    break;
            }

            if (markType == null)
                return;

            int total = 0;
            Container pack = m_From.Backpack;

            if (pack == null)
                return;

            Item[] marks = pack.FindItemsByType(markType, true);
            for (int i = 0; i < marks.Length; i++)
                total += marks[i].Amount;

            if (total < m_Info.Cost)
            {
                m_From.SendMessage("You do not have enough {0} for this reward.", m_CurrencyType);
                return;
            }

            pack.ConsumeTotal(markType, m_Info.Cost);

            Item reward = m_Info.CreateItem(m_Type);
            if (reward != null)
            {
                m_From.AddToBackpack(reward);
                m_From.SendMessage("You receive: {0}", m_Info.Name);
            }
            else
            {
                m_From.SendMessage("An error occurred while generating your reward.");
            }
        }

    }
}
