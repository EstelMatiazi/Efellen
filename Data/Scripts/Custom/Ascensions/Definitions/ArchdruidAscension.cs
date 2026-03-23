using Server;

namespace Server.Custom.Ascensions
{
    public class ArchdruidAscension : AscensionDefinition
    {
        public override AscensionType Type { get { return AscensionType.Archdruid; } }

        public override SkillName[] RequiredSkills
        {
            get { return new SkillName[] { SkillName.Taming, SkillName.Veterinary }; }
        }

        public override SkillName[] ForbiddenSkills
        {
            get { return new SkillName[] { SkillName.Bushido, SkillName.Knightship, SkillName.Necromancy, SkillName.Magery }; }
        }
    }
}
