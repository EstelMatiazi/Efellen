using Server;

namespace Server.Custom.Ascensions
{
    public class ArchmageAscension : AscensionDefinition
    {
        public override AscensionType Type { get { return AscensionType.Archmage; } }

        public override SkillName[] RequiredSkills
        {
            get { return new SkillName[] { SkillName.Magery, SkillName.Psychology, SkillName.Inscribe }; }
        }

        public override SkillName[] ForbiddenSkills
        {
            get { return new SkillName[] { SkillName.Bushido, SkillName.Knightship, SkillName.Elementalism, SkillName.Necromancy }; }
        }
    }
}
