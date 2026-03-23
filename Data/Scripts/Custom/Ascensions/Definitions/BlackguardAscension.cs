using Server;

namespace Server.Custom.Ascensions
{
    public class BlackguardAscension : AscensionDefinition
    {
        public override AscensionType Type { get { return AscensionType.Blackguard; } }

        public override SkillName[] RequiredSkills
        {
            get { return new SkillName[] { SkillName.Knightship, SkillName.ArmsLore, SkillName.Necromancy }; }
        }
        
        public override SkillName[] ForbiddenSkills
        {
            get { return new SkillName[] { SkillName.Bushido, SkillName.Spiritualism, SkillName.Elementalism }; }
        }
        public override bool RequiresEvil { get { return true; } }
    }
}
