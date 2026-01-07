using System;
using System.Collections.Generic;
using Server;
using Server.Companions.Core;
using Server.Companions.Data;

namespace Server.Companions.Abilities
{
    public class AbilityManager
    {
        private CompanionMobile m_Companion;
        private List<CompanionAbilityInstance> m_Abilities;

        public AbilityManager(CompanionMobile companion)
        {
            m_Companion = companion;
            m_Abilities = new List<CompanionAbilityInstance>();
        }

        public List<CompanionAbilityInstance> Abilities
        {
            get { return m_Abilities; }
        }

        public void GenerateInitialAbilities()
        {
            ClassProgression prog = ProgressionRegistry.Get(m_Companion.Class);
            if (prog == null)
                return;

            for (int level = 1; level <= m_Companion.Level; level++)
            {
                int picks = prog.GetFeatsAtLevel(level);

                for (int i = 0; i < picks; i++)
                    GrantRandomMartialFeat();
            }
        }
        public bool HasAbilities()
        {
            return m_Abilities.Count > 0;
        }
        public void EnsureInitialized()
        {
            if (m_Abilities.Count == 0)
                GenerateInitialAbilities();
        }

        public List<ICompanionAbility> GetAllAbilities()
        {
            List<ICompanionAbility> list = new List<ICompanionAbility>();

            for (int i = 0; i < m_Abilities.Count; i++)
                list.Add(m_Abilities[i].Definition);

            return list;
        }

        public List<ICompanionAbility> GetAvailableAbilities()
        {
            List<ICompanionAbility> list = new List<ICompanionAbility>();

            for (int i = 0; i < m_Abilities.Count; i++)
            {
                CompanionAbilityInstance inst = m_Abilities[i];

                if (inst.Definition.GetRequiredLevel() <= m_Companion.Level)
                    list.Add(inst.Definition);
            }

            return list;
        }

        public List<ICompanionAbility> GetUsableAbilities()
        {
            List<ICompanionAbility> list = new List<ICompanionAbility>();

            for (int i = 0; i < m_Abilities.Count; i++)
            {
                CompanionAbilityInstance inst = m_Abilities[i];

                if (inst.CanUse(m_Companion))
                    list.Add(inst.Definition);
            }

            return list;
        }

        
        public void OnLevelUp(int newLevel)
        {
            ClassProgression prog = ProgressionRegistry.Get(m_Companion.Class);
            if (prog == null)
                return;

            int picks = prog.GetFeatsAtLevel(newLevel);
            if (picks <= 0)
                return;

            for (int i = 0; i < picks; i++)
                GrantRandomMartialFeat();

            if (m_Companion.Owner != null)
                m_Companion.Owner.SendMessage(
                    m_Companion.Name + " has learned new combat techniques."
                );
        }


        private void GrantRandomMartialFeat()
        {
            List<ICompanionAbility> pool =
                MartialFeatRegistry.GetAvailable(m_Companion);

            for (int i = pool.Count - 1; i >= 0; i--)
            {
                if (HasAbility(pool[i].GetType()))
                    pool.RemoveAt(i);
            }

            if (pool.Count == 0)
                return;

            ICompanionAbility chosen = pool[Utility.Random(pool.Count)];
            m_Abilities.Add(new CompanionAbilityInstance(chosen));
        }

        private bool HasAbility(Type type)
        {
            for (int i = 0; i < m_Abilities.Count; i++)
            {
                if (m_Abilities[i].Definition.GetType() == type)
                    return true;
            }
            return false;
        }

        public bool UseAbility(string abilityName, Mobile target)
        {
            for (int i = 0; i < m_Abilities.Count; i++)
            {
                CompanionAbilityInstance inst = m_Abilities[i];

                if (!inst.Definition.GetName().Equals(
                        abilityName,
                        StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!inst.CanUse(m_Companion))
                {
                    if (m_Companion.Owner != null)
                        m_Companion.Owner.SendMessage(
                            inst.Definition.GetName() + " is not ready yet."
                        );
                    return false;
                }

                inst.Use(m_Companion, target);
                return true;
            }

            return false;
        }

        public bool TryUseRandomMartialSpecial(Mobile target)
        {
            List<CompanionAbilityInstance> candidates = new List<CompanionAbilityInstance>();
        
            for (int i = 0; i < m_Abilities.Count; i++)
            {
                CompanionAbilityInstance inst = m_Abilities[i];
        
                if (!inst.Definition.IsMartialSpecial)
                    continue;
        
                if (!inst.CanUse(m_Companion))
                    continue;
        
                candidates.Add(inst);
            }
        
            if (candidates.Count == 0)
                return false;
        
            CompanionAbilityInstance chosen =
                candidates[Utility.Random(candidates.Count)];
        
            chosen.Use(m_Companion, target);
            return true;
        }



        public void Serialize(GenericWriter writer)
        {
            writer.Write(m_Abilities.Count);

            for (int i = 0; i < m_Abilities.Count; i++)
                m_Abilities[i].Serialize(writer);
        }

        public void Deserialize(GenericReader reader)
        {
            m_Abilities.Clear();

            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                CompanionAbilityInstance inst =
                    CompanionAbilityInstance.Deserialize(reader);

                if (inst != null)
                    m_Abilities.Add(inst);
            }
        }
    }
}
