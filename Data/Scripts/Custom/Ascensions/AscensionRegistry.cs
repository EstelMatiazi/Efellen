using System.Collections.Generic;
using Server.Custom.Ascensions;

public static class AscensionRegistry
{
    private static Dictionary<AscensionType, AscensionDefinition> m_Definitions;

    static AscensionRegistry()
    {
        m_Definitions = new Dictionary<AscensionType, AscensionDefinition>();

        Register(new BerserkerAscension());
        Register(new ArchmageAscension());
        Register(new BattlemageAscension());
        Register(new ArchdruidAscension());
        Register(new PalemasterAscension());
        Register(new CrusaderAscension());
        Register(new KensaiAscension());
        Register(new SkaldAscension());
        Register(new AssassinAscension());
        Register(new ReaverAscension());
        Register(new ArcaneArcherAscension());
        Register(new HierophantAscension());
        Register(new BlackguardAscension());
    }

    private static void Register(AscensionDefinition def)
    {
        m_Definitions[def.Type] = def;
    }

    public static AscensionDefinition Get(AscensionType type)
    {
        return m_Definitions.ContainsKey(type) ? m_Definitions[type] : null;
    }

    public static Dictionary<AscensionType, AscensionDefinition> All
    {
        get { return m_Definitions; }
    }
}
