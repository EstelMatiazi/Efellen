using Server;

namespace Server.Custom.Ascensions
{
    public class ArcaneArcherAscension : AscensionDefinition
    {
        public override AscensionType Type { get { return AscensionType.ArcaneArcher; } }

        public override SkillName[] RequiredSkills
        {
            get { return new SkillName[] { SkillName.Magery, SkillName.Focus, SkillName.Inscribe,SkillName.Marksmanship }; }
        }

        public override SkillName[] ForbiddenSkills
        {
            get { return new SkillName[] { SkillName.Knightship, SkillName.Necromancy, SkillName.Elementalism, SkillName.Bushido }; }
        }
    }
}
