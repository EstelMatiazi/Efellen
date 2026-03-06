using System;
using System.Collections.Generic;
using Server;
using Server.Companions.Core;
using Server.Companions.Data;

namespace Server.Companions.Systems
{
    public class CompanionSystem
    {
        private static CompanionSystem m_Instance;
        private Timer m_GlobalTimer;
        private List<CompanionContract> m_ActiveContracts;

        public static CompanionSystem Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new CompanionSystem();
                return m_Instance;
            }
        }

        private CompanionSystem()
        {
            m_ActiveContracts = new List<CompanionContract>();
        }

        public static void Initialize()
        {
            CompanionDefinition.Initialize();

            Instance.StartGlobalTimer();

            Console.WriteLine("Companion System initialized.");
        }

        private void StartGlobalTimer()
        {
            if (m_GlobalTimer != null)
            {
                m_GlobalTimer.Stop();
            }

            m_GlobalTimer = new InternalTimer(this);
            m_GlobalTimer.Start();
        }

        private class InternalTimer : Timer
        {
            private CompanionSystem m_System;

            public InternalTimer(CompanionSystem system) : base(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10))
            {
                m_System = system;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                m_System.OnGlobalTick();
            }
        }

        private void OnGlobalTick()
        {
            List<CompanionContract> toRemove = new List<CompanionContract>();
            
            for (int i = 0; i < m_ActiveContracts.Count; i++)
            {
                CompanionContract contract = m_ActiveContracts[i];
                if (contract == null || contract.Deleted)
                {
                    toRemove.Add(contract);
                }
            }
            
            for (int i = 0; i < toRemove.Count; i++)
            {
                m_ActiveContracts.Remove(toRemove[i]);
            }

            for (int i = 0; i < m_ActiveContracts.Count; i++)
            {
                try
                {
                    m_ActiveContracts[i].Tick();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error ticking companion contract: " + ex.ToString());
                }
            }
        }

        public void RegisterContract(CompanionContract contract)
        {
            if (contract != null && !m_ActiveContracts.Contains(contract))
            {
                m_ActiveContracts.Add(contract);
            }
        }

        public void UnregisterContract(CompanionContract contract)
        {
            m_ActiveContracts.Remove(contract);
        }

        public List<CompanionMobile> GetAllCompanions()
        {
            List<CompanionMobile> companions = new List<CompanionMobile>();

            foreach (Mobile m in World.Mobiles.Values)
            {
                if (m is CompanionMobile)
                {
                    companions.Add((CompanionMobile)m);
                }
            }

            return companions;
        }

        public List<CompanionContract> GetAllContracts()
        {
            List<CompanionContract> contracts = new List<CompanionContract>();

            foreach (Item item in World.Items.Values)
            {
                if (item is CompanionContract)
                {
                    contracts.Add((CompanionContract)item);
                }
            }

            return contracts;
        }

        public void DisplayStats(Mobile from)
        {
            from.SendMessage("=== Companion System Statistics ===");
            from.SendMessage("Total Companions: " + GetAllCompanions().Count.ToString());
            from.SendMessage("Total Contracts: " + GetAllContracts().Count.ToString());
            from.SendMessage("Active Contracts: " + m_ActiveContracts.Count.ToString());

            Dictionary<CompanionClass, int> classCounts = new Dictionary<CompanionClass, int>();
            
            List<CompanionMobile> allCompanions = GetAllCompanions();
            for (int i = 0; i < allCompanions.Count; i++)
            {
                CompanionMobile companion = allCompanions[i];
                if (!classCounts.ContainsKey(companion.Class))
                    classCounts[companion.Class] = 0;
                
                classCounts[companion.Class]++;
            }

            from.SendMessage("--- By Class ---");
            foreach (KeyValuePair<CompanionClass, int> kvp in classCounts)
            {
                from.SendMessage(kvp.Key.ToString() + ": " + kvp.Value.ToString());
            }
        }
    }
}