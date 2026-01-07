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

        private CompanionAbilityInstance FindFeatByKey(string featKey)
        {
            for (int i = 0; i < m_Abilities.Count; i++)
            {
                BaseFeat feat = m_Abilities[i].Definition as BaseFeat;

                if (feat != null && feat.FeatKey == featKey)
                    return m_Abilities[i];
            }

            return null;
        }

        private List<BaseFeat> GetUpgradeableFeats(List<BaseFeat> available)
        {
            List<BaseFeat> list = new List<BaseFeat>();

            for (int i = 0; i < m_Abilities.Count; i++)
            {
                CompanionAbilityInstance inst = m_Abilities[i];
                BaseFeat owned = inst.Definition as BaseFeat;

                if (owned == null)
                    continue;

                for (int j = 0; j < available.Count; j++)
                {
                    BaseFeat candidate = available[j];

                    if (candidate.FeatKey != owned.FeatKey)
                        continue;

                    if (owned.CanUpgradeTo(candidate.Tier))
                    {
                        list.Add(candidate);
                    }
                }
            }

            return list;
        }



        private void GrantRandomMartialFeat()
        {
            Dictionary<int, List<BaseFeat>> byTier = MartialFeatRegistry.GetAvailableByTier(m_Companion);

            if (byTier.Count == 0)
                return;

            List<BaseFeat> all = new List<BaseFeat>();

            foreach (List<BaseFeat> list in byTier.Values)
                all.AddRange(list);

            List<BaseFeat> upgradeable = GetUpgradeableFeats(all);

            if (upgradeable.Count > 0 && Utility.RandomDouble() < 0.70)
            {
                BaseFeat chosen =
                    upgradeable[Utility.Random(upgradeable.Count)];

                ReplaceOrAddFeat(chosen);
                return;
            }

            int highestTier = 0;

            foreach (int tier in byTier.Keys)
                if (tier > highestTier)
                    highestTier = tier;

            List<BaseFeat> candidates;

            if (Utility.RandomDouble() < 0.55 && byTier.ContainsKey(highestTier))
                candidates = byTier[highestTier];
            else
                candidates = all;

            if (candidates.Count == 0)
                return;

            BaseFeat finalPick =
                candidates[Utility.Random(candidates.Count)];

            ReplaceOrAddFeat(finalPick);
        }

        private void ReplaceOrAddFeat(BaseFeat feat)
        {
            CompanionAbilityInstance existing =
                FindFeatByKey(feat.FeatKey);

            if (existing != null)
            {
                BaseFeat oldFeat = existing.Definition as BaseFeat;

                if (oldFeat != null && feat.Tier > oldFeat.Tier)
                {
                    m_Abilities.Remove(existing);
                    m_Abilities.Add(new CompanionAbilityInstance(feat));
                }

                return;
            }
            m_Abilities.Add(new CompanionAbilityInstance(feat));
        }



        public bool HasAbility(Type type)
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
