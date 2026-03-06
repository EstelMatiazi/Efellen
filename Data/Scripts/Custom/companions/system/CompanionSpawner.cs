using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Engines.PartySystem;
using Server.Companions.Core;
using Server.Companions.Data;

namespace Server.Companions.Systems
{
    public static class CompanionSpawner
    {
        private const int MaxCompanions = 3;
        private const int MaxPartySize = 4;

        public static bool ValidateSummon(Mobile owner, out string message)
        {
            message = null;

            if (owner == null)
            {
                message = "Invalid owner.";
                return false;
            }

            int activeCompanions = GetActiveCompanionCount(owner);
            if (activeCompanions >= MaxCompanions)
            {
                message = "You already have " + MaxCompanions.ToString() + " companions active.";
                return false;
            }

            if (!CheckPartyLimits(owner, out message))
                return false;

            return true;
        }

        public static bool CheckPartyLimits(Mobile owner, out string message)
        {
            message = null;

            Party party = Party.Get(owner);
            if (party == null)
                return true;

            int totalPlayers = 0;
            int totalCompanions = 0;

            if (party.Leader != null)
            {
                totalPlayers++;
                totalCompanions += GetActiveCompanionCount(party.Leader);
            }

            for (int i = 0; i < party.Members.Count; i++)
            {
                PartyMemberInfo info = (PartyMemberInfo)party.Members[i];
                if (info != null && info.Mobile != null && info.Mobile != party.Leader)
                {
                    totalPlayers++;
                    totalCompanions += GetActiveCompanionCount(info.Mobile);
                }
            }

            if (totalPlayers + totalCompanions >= MaxPartySize)
            {
                message = "Your party already has the maximum number of players and companions.";
                return false;
            }

            return true;
        }

        public static bool HasActiveCompanionOfClass(Mobile owner, CompanionClass classType)
        {
            List<CompanionMobile> activeCompanions = GetActiveCompanions(owner);

            for (int i = 0; i < activeCompanions.Count; i++)
            {
                if (activeCompanions[i].Class == classType)
                    return true;
            }

            return false;
        }

        public static int GetActiveCompanionCount(Mobile owner)
        {
            if (owner == null)
                return 0;

            int count = 0;

            IPooledEnumerable eable = owner.GetMobilesInRange(50);
            foreach (Mobile m in eable)
            {
                CompanionMobile companion = m as CompanionMobile;
                if (companion != null && companion.Owner == owner && !companion.Deleted && companion.Alive)
                    count++;
            }
            eable.Free();

            return count;
        }

        public static List<CompanionMobile> GetActiveCompanions(Mobile owner)
        {
            List<CompanionMobile> companions = new List<CompanionMobile>();

            if (owner == null)
                return companions;

            IPooledEnumerable eable = owner.GetMobilesInRange(50);
            foreach (Mobile m in eable)
            {
                CompanionMobile companion = m as CompanionMobile;
                if (companion != null && companion.Owner == owner && !companion.Deleted)
                    companions.Add(companion);
            }
            eable.Free();

            return companions;
        }

        
        public static void DismissAllCompanions(Mobile owner)
        {
            List<CompanionMobile> companions = GetActiveCompanions(owner);

            for (int i = 0; i < companions.Count; i++)
            {
                CompanionMobile companion = companions[i];
                Serial contractSerial = companion.ContractSerial;

                if (contractSerial != Serial.MinusOne)
                {
                    Item item = World.FindItem(contractSerial);
                    CompanionContract contract = item as CompanionContract;

                    if (contract != null)
                        contract.DismissCompanion(false);
                    else
                        companion.Delete();
                }
                else
                {
                    companion.Delete();
                }
            }

            if (companions.Count > 0 && owner != null)
                owner.SendMessage("You dismiss " + companions.Count.ToString() + " companion(s).");
        }

        public static bool IsInSafeZone(Mobile mobile)
        {
            return CompanionTimerHelper.IsInSafeZone(mobile);
        }

        public static bool ValidateAlignment(Mobile from,CompanionClass classType,CompanionAlignment alignment)
        {
            CompanionDefinition def = CompanionDefinition.Get(classType);

            if (def == null)
            {
                if (from != null)
                    from.SendMessage("That companion class does not exist.");
                return false;
            }

            if (!def.IsAlignmentAllowed(alignment))
            {
                if (from != null)
                {
                    from.SendMessage(
                        "A {0} cannot have a {1} alignment.",
                        classType.ToString(),
                        alignment.ToString()
                    );
                }
                return false;
            }
            return true;
        }

        private static CompanionAlignment[] GetAllAlignments()
        {
            return new CompanionAlignment[]
            {
                CompanionAlignment.GetLawfulGood(),
                CompanionAlignment.GetLawfulNeutral(),
                CompanionAlignment.GetLawfulEvil(),
                CompanionAlignment.GetNeutralGood(),
                CompanionAlignment.GetTrueNeutral(),
                CompanionAlignment.GetNeutralEvil(),
                CompanionAlignment.GetChaoticGood(),
                CompanionAlignment.GetChaoticNeutral(),
                CompanionAlignment.GetChaoticEvil()
            };
        }

        public static bool TryResolveAlignment(
            CompanionClass classType,
            AlignmentRestrictions restrictions,
            out CompanionAlignment alignment
        )
        {
            CompanionDefinition def = CompanionDefinition.Get(classType);
            CompanionAlignment[] all = GetAllAlignments();
            ArrayList valid = new ArrayList();

            for (int i = 0; i < all.Length; i++)
            {
                CompanionAlignment a = all[i];

                if (!def.IsAlignmentAllowed(a))
                    continue;

                if ((restrictions & AlignmentRestrictions.NonGood) != 0 && a.Moral == MoralAxis.Good)
                    continue;

                if ((restrictions & AlignmentRestrictions.NonEvil) != 0 && a.Moral == MoralAxis.Evil)
                    continue;

                if ((restrictions & AlignmentRestrictions.NonNeutral) != 0 && a.Moral == MoralAxis.Neutral || (restrictions & AlignmentRestrictions.NonNeutral) != 0 && a.Order == OrderAxis.Neutral)
                    continue;

                if ((restrictions & AlignmentRestrictions.NonLawful) != 0 && a.Order == OrderAxis.Lawful)
                    continue;

                if ((restrictions & AlignmentRestrictions.NonChaotic) != 0 && a.Order == OrderAxis.Chaotic)
                    continue;

                valid.Add(a);
            }

            if (valid.Count == 0)
            {
                alignment = all[0]; // dummy
                return false;
            }

            alignment = (CompanionAlignment)valid[Utility.Random(valid.Count)];
            return true;
        }

        public static CompanionContract CreateContract(
            Mobile owner,
            CompanionClass classType,
            CompanionAlignment alignment,
            string customName,
            bool isUnique
        )
        {
            if (owner == null)
                return null;

            CompanionDefinition def = CompanionDefinition.Get(classType);
            if (def == null)
                return null;

            if (!def.IsAlignmentAllowed(alignment))
            {
                owner.SendMessage("A " + classType.ToString() + " cannot have " + alignment.ToString() + " alignment.");
                return null;
            }

            CompanionContract contract = new CompanionContract();
            contract.InitializeCompanion(owner, classType, alignment, isUnique, customName);

            return contract;
        }

        public static CompanionContract CreateContract(
            Mobile owner,
            CompanionClass classType,
            CompanionAlignment alignment
        )
        {
            return CreateContract(owner, classType, alignment, null, false);
        }

        public static CompanionContract CreateContract(
            Mobile owner,
            CompanionClass classType,
            CompanionAlignment alignment,
            int level,
            string customName,
            bool isUnique
        )
        {
            CompanionContract contract = new CompanionContract();
            contract.InitializeCompanion(owner, classType, alignment, isUnique, customName, level);
            return contract;
        }

        public static CompanionContract CreateContract(
            Mobile owner,
            CompanionClass classType,
            AlignmentRestrictions restrictions,
            int level,
            string customName,
            bool isUnique
        )
        {
            CompanionAlignment alignment;

            if (!TryResolveAlignment(classType, restrictions, out alignment))
            {
                owner.SendMessage("No valid alignment matches those restrictions.");
                return null;
            }

            CompanionContract contract = new CompanionContract();
            contract.InitializeCompanion(owner, classType, alignment, isUnique, customName, level);

            return contract;
        }
    }
}
