using System;
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
            {
                return false;
            }

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

            // Count party leader
            if (party.Leader != null)
            {
                totalPlayers++;
                totalCompanions += GetActiveCompanionCount(party.Leader);
            }

            // Count party members
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
            // hacky as all hell, TODO: unfuck this
            IPooledEnumerable eable = owner.GetMobilesInRange(50);
            foreach (Mobile m in eable)
            {
                if (m is CompanionMobile)
                {
                    CompanionMobile companion = (CompanionMobile)m;
                    if (companion.Owner == owner && !companion.Deleted && companion.Alive)
                    {
                        count++;
                    }
                }
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
                if (m is CompanionMobile)
                {
                    CompanionMobile companion = (CompanionMobile)m;
                    if (companion.Owner == owner && !companion.Deleted)
                    {
                        companions.Add(companion);
                    }
                }
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
                    if (item is CompanionContract)
                    {
                        CompanionContract contract = (CompanionContract)item;
                        contract.DismissCompanion(false);
                    }
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

        public static CompanionContract CreateContract(Mobile owner, CompanionClass classType, CompanionAlignment alignment, string customName, bool isUnique)
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

            // Create contract and initialize companion data
            CompanionContract contract = new CompanionContract();
            contract.InitializeCompanion(owner, classType, alignment, isUnique, customName);
            
            return contract;
        }

        public static CompanionContract CreateContract(Mobile owner, CompanionClass classType, CompanionAlignment alignment)
        {
            return CreateContract(owner, classType, alignment, null, false);
        }
    }
}