using System;
using Server;
using Server.Commands;
using Server.Targeting;
using Server.Companions.Core;
using Server.Companions.Data;
using Server.Companions.Systems;
using Server.Companions.Abilities;
using System.Collections.Generic;

namespace Server.Companions.Commands
{
    public class CompanionCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("CreateCompanion", AccessLevel.GameMaster, new CommandEventHandler(CreateCompanion_OnCommand));
            CommandSystem.Register("CreateCompanionRestricted", AccessLevel.GameMaster, new CommandEventHandler(CreateCompanionRestricted_OnCommand));
            CommandSystem.Register("CompanionStats", AccessLevel.GameMaster, new CommandEventHandler(CompanionStats_OnCommand));
            CommandSystem.Register("GiveCompanionXP", AccessLevel.GameMaster, new CommandEventHandler(GiveCompanionXP_OnCommand));
            CommandSystem.Register("Rest", AccessLevel.Player, new CommandEventHandler(Rest_OnCommand));
            CommandSystem.Register("CompanionAbilities", AccessLevel.Player, new CommandEventHandler(CompanionAbilities_OnCommand));
        }

        [Usage("CompanionAbilities")]
        [Description("Returns a list of abilities in the targetted companion")]
        private static void CompanionAbilities_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage("Target a companion to list its abilities.");
            from.Target = new AbilityTarget();
        }

        private class AbilityTarget : Target
        {
            public AbilityTarget() : base(12, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                CompanionMobile companion = targeted as CompanionMobile;

                if (companion == null)
                {
                    from.SendMessage("That is not a companion.");
                    return;
                }

                AbilityManager mgr = companion.AbilityManager;

                if (mgr == null)
                {
                    from.SendMessage("This companion has no ability manager.");
                    return;
                }

                SendAbilityList(from, companion, mgr);
            }
        }

        private static void SendAbilityList(
            Mobile viewer,
            CompanionMobile companion,
            AbilityManager mgr
        )
        {
            List<ICompanionAbility> abilities = mgr.GetAllAbilities();

            viewer.SendMessage(0x3B2, "=== Abilities for {0} (Level {1}) ===",
                companion.Name,
                companion.Level
            );

            if (abilities.Count == 0)
            {
                viewer.SendMessage("No abilities.");
                return;
            }

            for (int i = 0; i < abilities.Count; i++)
            {
                ICompanionAbility ability = abilities[i];

                bool canUse = ability.CanUse(companion);
                int reqLevel = ability.GetRequiredLevel();

                string status = canUse ? "Ready" : "Unavailable";

                viewer.SendMessage(
                    canUse ? 0x59 : 0x21,
                    "- {0} (Req Lvl {1}) [{2}]",
                    ability.GetName(),
                    reqLevel,
                    status
                );
            }
        }


        [Usage("CreateCompanion <class> <alignment> [level]")]
        [Description("Creates a companion contract. Example: CreateCompanion Fighter LawfulGood 10")]
        private static void CreateCompanion_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Length < 2)
            {
                from.SendMessage("Usage: CreateCompanion <class> <alignment> [level]");
                from.SendMessage("Classes: Wizard, Fighter, Druid, Rogue, Monk, Barbarian, Ranger, Paladin, Cleric, Sorcerer, Bard");
                from.SendMessage("Alignments: LawfulGood, LawfulNeutral, LawfulEvil, NeutralGood, TrueNeutral, NeutralEvil, ChaoticGood, ChaoticNeutral, ChaoticEvil");
                return;
            }

            CompanionClass classType;
            try
            {
                classType = (CompanionClass)Enum.Parse(typeof(CompanionClass), e.GetString(0), true);
            }
            catch
            {
                from.SendMessage("Invalid class. Valid classes are: Wizard, Fighter, Druid, Rogue, Monk, Barbarian, Ranger, Paladin, Cleric, Sorcerer, Bard");
                return;
            }

            CompanionAlignment alignment;
            string alignmentStr = e.GetString(1).ToLower();

            switch (alignmentStr)
            {
                case "lawfulgood": alignment = CompanionAlignment.GetLawfulGood(); break;
                case "lawfulneutral": alignment = CompanionAlignment.GetLawfulNeutral(); break;
                case "lawfulevil": alignment = CompanionAlignment.GetLawfulEvil(); break;
                case "neutralgood": alignment = CompanionAlignment.GetNeutralGood(); break;
                case "trueneutral": alignment = CompanionAlignment.GetTrueNeutral(); break;
                case "neutralevil": alignment = CompanionAlignment.GetNeutralEvil(); break;
                case "chaoticgood": alignment = CompanionAlignment.GetChaoticGood(); break;
                case "chaoticneutral": alignment = CompanionAlignment.GetChaoticNeutral(); break;
                case "chaoticevil": alignment = CompanionAlignment.GetChaoticEvil(); break;
                default:
                    from.SendMessage("Invalid alignment.");
                    return;
            }

            int level = 1;

            if (e.Length >= 3)
            {
                try
                {
                    level = e.GetInt32(2);
                }
                catch
                {
                    from.SendMessage("Invalid level.");
                    return;
                }
            }
            if (!CompanionSpawner.ValidateAlignment(from, classType, alignment))
                return;

            CompanionContract contract =
                CompanionSpawner.CreateContract(from, classType, alignment, level, null, false);

            if (contract != null)
            {
                from.AddToBackpack(contract);
                from.SendMessage("Created a " + alignment.ToString() + " " + classType.ToString() + " companion contract!");
                CompanionSystem.Instance.RegisterContract(contract);
            }
            else
            {
                from.SendMessage("Failed to create companion contract.");
            }
        }

        [Usage("CreateCompanionRestricted <class> <level> [restrictions...]")]
        [Description("Creates a companion using alignment restrictions. Example: CreateCompanionRestricted Fighter 10 non-evil non-chaotic")]
        private static void CreateCompanionRestricted_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Length < 2)
            {
                from.SendMessage("Usage: CreateCompanionRestricted <class> <level> [restrictions]");
                from.SendMessage("Restrictions: non-good, non-evil, non-lawful, non-chaotic, non-neutral");
                return;
            }

            CompanionClass classType;
            try
            {
                classType = (CompanionClass)Enum.Parse(typeof(CompanionClass), e.GetString(0), true);
            }
            catch
            {
                from.SendMessage("Invalid class.");
                return;
            }

            int level;
            try
            {
                level = e.GetInt32(1);
            }
            catch
            {
                from.SendMessage("Invalid level.");
                return;
            }

            AlignmentRestrictions restrictions = AlignmentRestrictions.None;

            for (int i = 2; i < e.Length; i++)
            {
                string r = e.GetString(i).ToLower();

                switch (r)
                {
                    case "non-good": restrictions |= AlignmentRestrictions.NonGood; break;
                    case "non-evil": restrictions |= AlignmentRestrictions.NonEvil; break;
                    case "non-lawful": restrictions |= AlignmentRestrictions.NonLawful; break;
                    case "non-chaotic": restrictions |= AlignmentRestrictions.NonChaotic; break;
                    case "non-neutral": restrictions |= AlignmentRestrictions.NonNeutral; break;
                }
            }
            CompanionAlignment alignment;
            if (!CompanionSpawner.TryResolveAlignment(
                    classType,
                    restrictions,
                    out alignment))
            {
                from.SendMessage("No valid alignment matches those restrictions.");
                return;
            }
            if (!CompanionSpawner.ValidateAlignment(from, classType, alignment))
                return;

            CompanionContract contract =
                CompanionSpawner.CreateContract(from, classType, restrictions, level, null, false);

            if (contract != null)
            {
                from.AddToBackpack(contract);
                from.SendMessage(
                    "Created a level " + level.ToString() + " " +
                    classType.ToString() + " companion with the restricted alignment: " + alignment.ToString()
                );

                CompanionSystem.Instance.RegisterContract(contract);
            }
            else
            {
                from.SendMessage("Failed to create companion with those restrictions.");
            }
        }

        [Usage("CompanionStats")]
        [Description("Displays companion system statistics")]
        private static void CompanionStats_OnCommand(CommandEventArgs e)
        {
            CompanionSystem.Instance.DisplayStats(e.Mobile);
        }

        [Usage("GiveCompanionXP <amount>")]
        [Description("Gives experience to a targeted companion")]
        private static void GiveCompanionXP_OnCommand(CommandEventArgs e)
        {
            if (e.Length < 1)
            {
                e.Mobile.SendMessage("Usage: GiveCompanionXP <amount>");
                return;
            }

            int amount = e.GetInt32(0);
            e.Mobile.SendMessage("Target a companion to give experience to.");
            e.Mobile.Target = new GiveXPTarget(amount);
        }

        private class GiveXPTarget : Target
        {
            private int m_Amount;

            public GiveXPTarget(int amount) : base(15, false, TargetFlags.None)
            {
                m_Amount = amount;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is CompanionMobile)
                {
                    CompanionMobile companion = (CompanionMobile)targeted;
                    companion.GainExperience(m_Amount);
                    from.SendMessage("Gave " + m_Amount.ToString() + " experience to " + companion.Name + ".");
                }
                else
                {
                    from.SendMessage("That is not a companion.");
                }
            }
        }

        [Usage("Rest")]
        [Description("Dismisses all your companions when in a safe zone")]
        private static void Rest_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (!CompanionSpawner.IsInSafeZone(from))
            {
                from.SendMessage("You must be in a safe zone (house, inn, or bank) to rest.");
                return;
            }

            CompanionSpawner.DismissAllCompanions(from);
        }
    }
}
