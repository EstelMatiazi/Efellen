using Server;

namespace Server.Custom.Ascensions
{
    public class PalemasterAscension : AscensionDefinition
    {
        public override AscensionType Type { get { return AscensionType.Palemaster; } }

        public override SkillName[] RequiredSkills
        {
            get { return new SkillName[] { SkillName.Necromancy, SkillName.Spiritualism, SkillName.Forensics }; }
        }

        public override SkillName[] ForbiddenSkills
        {
            get { return new SkillName[] { SkillName.Knightship, SkillName.Elementalism, SkillName.Bushido }; }
        }

        public override bool RequiresEvil { get { return true; } }
    }
}
