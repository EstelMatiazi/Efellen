using Server;

namespace Server.Custom.Ascensions
{
    public class BattlemageAscension : AscensionDefinition
    {
        public override AscensionType Type { get { return AscensionType.Battlemage; } }

        public override SkillName[] RequiredSkills
        {
            get { return new SkillName[] { SkillName.Focus, SkillName.Magery }; }
        }

        public override SkillName[] ForbiddenSkills
        {
            get { return new SkillName[] { SkillName.Bushido, SkillName.Knightship }; }
        }
    }
}
