using Server;

namespace Server.Custom.Ascensions
{
    public class CrusaderAscension : AscensionDefinition
    {
        public override AscensionType Type { get { return AscensionType.Crusader; } }

        public override SkillName[] RequiredSkills
        {
            get { return new SkillName[] { SkillName.Knightship, SkillName.Tactics,SkillName.Spiritualism }; }
        }

        public override SkillName[] ForbiddenSkills
        {
            get { return new SkillName[] { SkillName.Bushido, SkillName.Necromancy,SkillName.Hiding, SkillName.Stealing }; }
        }

        public override bool RequiresGood { get { return true; } }
    }
}
