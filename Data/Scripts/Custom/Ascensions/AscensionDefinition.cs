using Server;

public abstract class AscensionDefinition
{
    public abstract AscensionType Type { get; }

    public abstract SkillName[] RequiredSkills { get; }
    public abstract SkillName[] ForbiddenSkills { get; }

    public virtual bool RequiresGood { get { return false; } }
    public virtual bool RequiresEvil { get { return false; } }

    public virtual int GetRequiredSkillValue(int level)
    {
        return 95 + (level - 1);
    }
}
