using System;
using Server;

public class AscensionProgress
{
    public bool Unlocked;
    public int Level;
    public int Experience;

    public AscensionProgress()
    {
        Unlocked = false;
        Level = 0;
        Experience = 0;
    }

    public int GetNextLevel()
    {
        return Level + 1;
    }
    
    public bool IsMaxLevel()
    {
        return Level >= 20;
    }
    
    public int GetRequiredExperience()
    {
        if (IsMaxLevel())
            return 0;
    
        return AscensionCosts.GetRequiredExperience(GetNextLevel());
    }
    
    public bool IsExperienceCapped()
    {
        int required = GetRequiredExperience();
        return required > 0 && Experience >= required;
    }

    public void AddExperience(int amount)
    {
        if (IsExperienceCapped())
            return;
    
        Experience += amount;
    
        int required = GetRequiredExperience();
        if (required > 0 && Experience > required)
            Experience = required;
    }

    public bool CanLevelUp()
    {
        if (!Unlocked || IsMaxLevel())
            return false;
    
        return Experience >= GetRequiredExperience();
    }


    public void Serialize(GenericWriter writer)
    {
        writer.Write((int)0); // version

        writer.Write(Unlocked);
        writer.Write(Level);
        writer.Write(Experience);
    }

    public void Deserialize(GenericReader reader)
    {
        int version = reader.ReadInt();

        Unlocked = reader.ReadBool();
        Level = reader.ReadInt();
        Experience = reader.ReadInt();
    }
}
