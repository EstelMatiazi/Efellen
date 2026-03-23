using Server;

namespace Server.Custom.Ascensions
{
    public class SkaldAscension : AscensionDefinition
    {
        public override AscensionType Type { get { return AscensionType.Skald; } }

        public override SkillName[] RequiredSkills
        {
            get { return new SkillName[] { SkillName.Musicianship, SkillName.Tactics, SkillName.Discordance }; }
        }

        public override SkillName[] ForbiddenSkills
        {
            get { return new SkillName[] { SkillName.Bushido, SkillName.Necromancy, SkillName.Marksmanship, SkillName.Hiding,SkillName.Begging }; }
        }
    }
}
