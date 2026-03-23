using Server;

namespace Server.Custom.Ascensions
{
    public class AssassinAscension : AscensionDefinition
    {
        public override AscensionType Type { get { return AscensionType.Assassin; } }

        public override SkillName[] RequiredSkills
        {
            get { return new SkillName[] { SkillName.Poisoning, SkillName.Fencing, SkillName.Hiding }; }
        }

        public override SkillName[] ForbiddenSkills
        {
            get { return new SkillName[] { SkillName.Knightship, SkillName.Bushido }; }
        }

        public override bool RequiresEvil { get { return true; } }
    }
}
