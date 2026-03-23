using Server;

namespace Server.Custom.Ascensions
{
    public class BerserkerAscension : AscensionDefinition
    {
        public override AscensionType Type
        {
            get { return AscensionType.Berserker; }
        }

        public override SkillName[] RequiredSkills
        {
            get { return new SkillName[] { SkillName.MagicResist, SkillName.Tactics, SkillName.Camping }; }
        }

        public override SkillName[] ForbiddenSkills
        {
            get { return new SkillName[] { SkillName.Magery, SkillName.Knightship, SkillName.Necromancy, SkillName.Bushido }; }
        }
    }
}
