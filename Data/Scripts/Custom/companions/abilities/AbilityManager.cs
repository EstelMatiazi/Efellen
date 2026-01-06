using System;
using System.Collections.Generic;
using Server.Companions.Core;
using Server.Companions.Data;

namespace Server.Companions.Abilities
{
    public class AbilityManager
    {
        private CompanionMobile m_Companion;
        private List<ICompanionAbility> m_AllAbilities;
        private Dictionary<int, List<ICompanionAbility>> m_AbilitiesByLevel;

        public AbilityManager(CompanionMobile companion)
        {
            m_Companion = companion;
            m_AllAbilities = new List<ICompanionAbility>();
            m_AbilitiesByLevel = new Dictionary<int, List<ICompanionAbility>>();
        }

        public void Initialize(CompanionClass classType)
        {
            m_AllAbilities.Clear();
            m_AbilitiesByLevel.Clear();

            switch (classType)
            {
                case CompanionClass.Mage:
                    // TODO: Add mage abilities
                    break;
                case CompanionClass.Fighter:
                    // TODO: Add fighter abilities
                    break;
                case CompanionClass.Druid:
                    // TODO: Add druid abilities
                    break;
                case CompanionClass.Rogue:
                    // TODO: Add rogue abilities
                    break;
                case CompanionClass.Monk:
                    // TODO: Add monk abilities
                    break;
                case CompanionClass.Barbarian:
                    // TODO: Add barbarian abilities
                    break;
                case CompanionClass.Ranger:
                    // TODO: Add ranger abilities
                    break;
                case CompanionClass.Paladin:
                    // TODO: Add paladin abilities
                    break;
                case CompanionClass.Cleric:
                    // TODO: Add cleric abilities
                    break;
                case CompanionClass.Sorcerer:
                    // TODO: Add sorcerer abilities
                    break;
                case CompanionClass.Bard:
                    // TODO: Add bard abilities
                    break;
            }

            for (int i = 0; i < m_AllAbilities.Count; i++)
            {
                ICompanionAbility ability = m_AllAbilities[i];
                int reqLevel = ability.GetRequiredLevel();
                
                if (!m_AbilitiesByLevel.ContainsKey(reqLevel))
                    m_AbilitiesByLevel[reqLevel] = new List<ICompanionAbility>();

                m_AbilitiesByLevel[reqLevel].Add(ability);
            }
        }

        public void OnLevelUp(int newLevel)
        {
            if (m_AbilitiesByLevel.ContainsKey(newLevel))
            {
                List<ICompanionAbility> unlockedAbilities = m_AbilitiesByLevel[newLevel];
                
                if (m_Companion.Owner != null && unlockedAbilities.Count > 0)
                {
                    m_Companion.Owner.SendMessage(m_Companion.Name + " has unlocked new abilities!");
                    
                    for (int i = 0; i < unlockedAbilities.Count; i++)
                    {
                        ICompanionAbility ability = unlockedAbilities[i];
                        m_Companion.Owner.SendMessage("- " + ability.GetName());
                    }
                }
            }
        }

        public List<ICompanionAbility> GetAvailableAbilities()
        {
            List<ICompanionAbility> available = new List<ICompanionAbility>();
            
            for (int i = 0; i < m_AllAbilities.Count; i++)
            {
                ICompanionAbility ability = m_AllAbilities[i];
                if (ability.GetRequiredLevel() <= m_Companion.Level)
                    available.Add(ability);
            }
            
            return available;
        }

        public List<ICompanionAbility> GetUsableAbilities()
        {
            List<ICompanionAbility> usable = new List<ICompanionAbility>();
            
            for (int i = 0; i < m_AllAbilities.Count; i++)
            {
                ICompanionAbility ability = m_AllAbilities[i];
                if (ability.CanUse(m_Companion))
                    usable.Add(ability);
            }
            
            return usable;
        }

        public bool UseAbility(string abilityName, Mobile target)
        {
            ICompanionAbility foundAbility = null;
            
            for (int i = 0; i < m_AllAbilities.Count; i++)
            {
                ICompanionAbility ability = m_AllAbilities[i];
                if (ability.GetName().Equals(abilityName, StringComparison.OrdinalIgnoreCase))
                {
                    foundAbility = ability;
                    break;
                }
            }

            if (foundAbility == null)
                return false;

            if (!foundAbility.CanUse(m_Companion))
            {
                if (m_Companion.Owner != null)
                    m_Companion.Owner.SendMessage(foundAbility.GetName() + " is not available right now.");
                return false;
            }

            foundAbility.Use(m_Companion, target);
            return true;
        }

        private void AddAbility(ICompanionAbility ability)
        {
            if (ability != null)
                m_AllAbilities.Add(ability);
        }
    }
}