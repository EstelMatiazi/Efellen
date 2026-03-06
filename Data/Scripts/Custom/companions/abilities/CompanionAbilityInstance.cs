using System;
using Server;
using Server.Companions.Core;

namespace Server.Companions.Abilities
{
    public class CompanionAbilityInstance
    {
        private ICompanionAbility m_Definition;
        private DateTime m_LastUsed;

        public CompanionAbilityInstance(ICompanionAbility definition)
        {
            m_Definition = definition;
            m_LastUsed = DateTime.MinValue;
        }

        public ICompanionAbility Definition
        {
            get { return m_Definition; }
        }

        public bool CanUse(CompanionMobile companion)
        {
            if (!m_Definition.CanUse(companion))
                return false;

            if (DateTime.UtcNow < m_LastUsed + m_Definition.GetCooldown())
                return false;

            return true;
        }

        public void Use(CompanionMobile companion, Mobile target)
        {
            m_Definition.Use(companion, target);
            m_LastUsed = DateTime.UtcNow;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(m_Definition.GetType().FullName);
            writer.Write(m_LastUsed);
        }

        public static CompanionAbilityInstance Deserialize(GenericReader reader)
        {
            string typeName = reader.ReadString();
            DateTime lastUsed = reader.ReadDateTime();

            Type t = ScriptCompiler.FindTypeByName(typeName);
            if (t == null)
                return null;

            ICompanionAbility def = Activator.CreateInstance(t) as ICompanionAbility;
            if (def == null)
                return null;

            CompanionAbilityInstance inst = new CompanionAbilityInstance(def);
            inst.m_LastUsed = lastUsed;
            return inst;
        }
    }
}
