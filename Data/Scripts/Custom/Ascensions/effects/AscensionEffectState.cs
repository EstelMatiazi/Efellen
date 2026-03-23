using System;

namespace Server.Custom.Ascensions
{
    public class AscensionEffectState
    {
        private string m_Name;
        private DateTime m_Expires;
        private int m_Level;

        public string Name
        {
            get { return m_Name; }
        }

        public DateTime Expires
        {
            get { return m_Expires; }
        }

        public int Level
        {
            get { return m_Level; }
        }

        public bool IsExpired
        {
            get { return DateTime.UtcNow >= m_Expires; }
        }

        public AscensionEffectState(string name, TimeSpan duration, int level)
        {
            m_Name = name;
            m_Level = level;
            m_Expires = DateTime.UtcNow + duration;
        }
    }
}
