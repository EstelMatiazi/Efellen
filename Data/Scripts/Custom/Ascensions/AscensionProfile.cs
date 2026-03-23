using System;
using System.Collections.Generic;
using Server;

public class AscensionProfile
{
    private Dictionary<AscensionType, AscensionProgress> m_Ascensions;
    private AscensionType m_ActiveAscension;

    public AscensionType ActiveAscension
    {
        get { return m_ActiveAscension; }
        set { m_ActiveAscension = value; }
    }

    public AscensionProfile()
    {
        m_Ascensions = new Dictionary<AscensionType, AscensionProgress>();
        m_ActiveAscension = AscensionType.None;
    }

    public AscensionProgress Get(AscensionType type)
    {
        AscensionProgress prog;

        if (!m_Ascensions.TryGetValue(type, out prog))
        {
            prog = new AscensionProgress();
            m_Ascensions[type] = prog;
        }

        return prog;
    }

    public int GetTotalLevels()
    {
        int total = 0;

        foreach (KeyValuePair<AscensionType, AscensionProgress> kvp in m_Ascensions)
        {
            if (kvp.Value.Unlocked)
                total += kvp.Value.Level;
        }

        return total;
    }

    public bool SetActive(AscensionType type)
    {
        if (!IsUnlocked(type))
            return false;
    
        m_ActiveAscension = type;
        return true;
    }


    public bool IsUnlocked(AscensionType type)
    {
        AscensionProgress prog = Get(type);
        return prog.Unlocked;
    }



    public Dictionary<AscensionType, AscensionProgress> GetAll()
    {
        return m_Ascensions;
    }

    public void Serialize(GenericWriter writer)
    {
        writer.Write((int)0); // version

        writer.Write((int)m_ActiveAscension);

        writer.Write(m_Ascensions.Count);

        foreach (KeyValuePair<AscensionType, AscensionProgress> kvp in m_Ascensions)
        {
            writer.Write((int)kvp.Key);
            kvp.Value.Serialize(writer);
        }
    }

    public void Deserialize(GenericReader reader)
    {
        int version = reader.ReadInt();

        m_ActiveAscension = (AscensionType)reader.ReadInt();

        int count = reader.ReadInt();

        m_Ascensions = new Dictionary<AscensionType, AscensionProgress>();

        for (int i = 0; i < count; i++)
        {
            AscensionType type = (AscensionType)reader.ReadInt();

            AscensionProgress prog = new AscensionProgress();
            prog.Deserialize(reader);

            m_Ascensions[type] = prog;
        }
    }
}
