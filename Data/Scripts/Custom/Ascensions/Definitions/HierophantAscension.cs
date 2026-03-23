using Server;

namespace Server.Custom.Ascensions
{
    public class HierophantAscension : AscensionDefinition
    {
        public override AscensionType Type { get { return AscensionType.Hierophant; } }

        public override SkillName[] RequiredSkills
        {
            get { return new SkillName[] { SkillName.Healing, SkillName.Spiritualism, SkillName.Meditation }; }
        }

        public override SkillName[] ForbiddenSkills
        {
            get { return new SkillName[] { SkillName.Knightship, SkillName.Necromancy, SkillName.Forensics, SkillName.Bushido, SkillName.Ninjitsu }; }
        }
    }
}
