using System;

namespace Server.Companions.Data
{
    public enum OrderAxis
    {
        Lawful,
        Neutral,
        Chaotic
    }

    public enum MoralAxis
    {
        Good,
        Neutral,
        Evil
    }

    public struct CompanionAlignment
    {
        private OrderAxis m_Order;
        private MoralAxis m_Moral;

        public OrderAxis Order 
        { 
            get { return m_Order; }
            set { m_Order = value; }
        }
        
        public MoralAxis Moral 
        { 
            get { return m_Moral; }
            set { m_Moral = value; }
        }

        public CompanionAlignment(OrderAxis order, MoralAxis moral)
        {
            m_Order = order;
            m_Moral = moral;
        }

        public bool GetIsLawful()
        {
            return m_Order == OrderAxis.Lawful;
        }

        public bool GetIsChaotic()
        {
            return m_Order == OrderAxis.Chaotic;
        }

        public bool GetIsGood()
        {
            return m_Moral == MoralAxis.Good;
        }

        public bool GetIsEvil()
        {
            return m_Moral == MoralAxis.Evil;
        }

        public bool GetIsLawfulGood()
        {
            return GetIsLawful() && GetIsGood();
        }

        public bool GetIsNeutral()
        {
            return m_Order == OrderAxis.Neutral || m_Moral == MoralAxis.Neutral;
        }

        public override string ToString()
        {
            if (m_Order == OrderAxis.Neutral && m_Moral == MoralAxis.Neutral)
                return "True Neutral";
            if (m_Order == OrderAxis.Neutral)
                return "Neutral " + m_Moral.ToString();
            if (m_Moral == MoralAxis.Neutral)
                return m_Order.ToString() + " Neutral";
            return m_Order.ToString() + " " + m_Moral.ToString();
        }

        public static CompanionAlignment GetLawfulGood()
        {
            return new CompanionAlignment(OrderAxis.Lawful, MoralAxis.Good);
        }

        public static CompanionAlignment GetLawfulNeutral()
        {
            return new CompanionAlignment(OrderAxis.Lawful, MoralAxis.Neutral);
        }

        public static CompanionAlignment GetLawfulEvil()
        {
            return new CompanionAlignment(OrderAxis.Lawful, MoralAxis.Evil);
        }

        public static CompanionAlignment GetNeutralGood()
        {
            return new CompanionAlignment(OrderAxis.Neutral, MoralAxis.Good);
        }

        public static CompanionAlignment GetTrueNeutral()
        {
            return new CompanionAlignment(OrderAxis.Neutral, MoralAxis.Neutral);
        }

        public static CompanionAlignment GetNeutralEvil()
        {
            return new CompanionAlignment(OrderAxis.Neutral, MoralAxis.Evil);
        }

        public static CompanionAlignment GetChaoticGood()
        {
            return new CompanionAlignment(OrderAxis.Chaotic, MoralAxis.Good);
        }

        public static CompanionAlignment GetChaoticNeutral()
        {
            return new CompanionAlignment(OrderAxis.Chaotic, MoralAxis.Neutral);
        }

        public static CompanionAlignment GetChaoticEvil()
        {
            return new CompanionAlignment(OrderAxis.Chaotic, MoralAxis.Evil);
        }
    }
}