using Server;
using Server.Mobiles;
using Server.Custom.Ascensions;

namespace Server.Custom.Ascensions
{
    public static class AscensionExperienceSystem
    {
        public static void AwardExperience(PlayerMobile pm, BaseCreature creature)
        {
            if (pm == null || creature == null)
                return;

            if (!pm.HasActiveAscension)
                return;

            AscensionType type = pm.ActiveAscension;

            AscensionProgress prog = pm.AscensionProfile.Get(type);

            if (prog.Level >= AscensionConstants.MaxAscensionLevel)
                return;

            if (prog.IsExperienceCapped())
                return;

            int fame = creature.Fame;

            if (fame <= 0)
                return;

            int min = fame / 125;
            int max = fame / 95;

            if (max <= 0)
                return;

            int amount = Utility.RandomMinMax(min, max);

            prog.AddExperience(amount);

            pm.SendMessage(1153, "You gain " + amount + " ascension experience.");
        }
        private static bool CanGainExperience(PlayerMobile pm, AscensionDefinition def, AscensionProgress prog)
        {
            if (pm == null || def == null || prog == null)
                return false;

            SkillName[] required = def.RequiredSkills;

            if (required != null && required.Length > 0)
            {
                int requiredValue = 95 + (prog.Level - 1);

                for (int i = 0; i < required.Length; i++)
                {
                    Skill skill = pm.Skills[required[i]];

                    if (skill == null || skill.Base < requiredValue)
                        return false;
                }
            }

            SkillName[] forbidden = def.ForbiddenSkills;

            if (forbidden != null && forbidden.Length > 0)
            {
                for (int i = 0; i < forbidden.Length; i++)
                {
                    Skill skill = pm.Skills[forbidden[i]];

                    if (skill != null && skill.Base > 0)
                        return false;
                }
            }

            if (def.RequiresGood && pm.Karma < 0)
                return false;

            if (def.RequiresEvil && pm.Karma > 0)
                return false;

            return true;
        }


        public static void AwardExperienceDirect(PlayerMobile pm, BaseCreature creature, int amount)
        {
            if (pm == null || creature == null || amount <= 0)
                return;

            if (!pm.HasActiveAscension)
                return;

            AscensionType type = pm.ActiveAscension;
            AscensionDefinition def = AscensionRegistry.Get(type);
            AscensionProgress prog = pm.AscensionProfile.Get(type);

            if (prog.Level >= AscensionConstants.MaxAscensionLevel)
                return;

            if (prog.IsExperienceCapped())
                return;

            if (!CanGainExperience(pm, def, prog))
            {
                pm.SendMessage("You either have restricted skills that prevent you from gaining experience on this Ascension or you have the incorrect required karma for it");
                return;
            }

            prog.AddExperience(amount);

            pm.SendMessage(1153, "You gain " + amount + " ascension experience.");

            if (pm.HasGump(typeof(AscensionQuickbarGump)))
            {
                pm.CloseGump(typeof(AscensionQuickbarGump));
                pm.SendGump(new AscensionQuickbarGump(pm));
            }
        }
    }
}
