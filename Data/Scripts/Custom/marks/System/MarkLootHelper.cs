using System;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom.DefenderOfTheRealm
{
    public static class MarkLootHelper
    {
        public static void CheckForMarks(BaseCreature bc, Container c, Mobile killer)
        {
            if (bc == null || c == null || killer == null)
                return;

            if (bc.Controlled || bc.Summoned || bc.Player)
                return;

            if (bc.Fame < 3000)
                return;

            if (Utility.RandomDouble() > 0.05)
                return;

            int fameMod = bc.Fame / 750;
            int baseMin = 1;
            int baseMax = bc.Fame/750 > 25 ? 25 : bc.Fame/750;

            int amount = Utility.RandomMinMax(baseMin, baseMax);

            if (killer.Karma < 0)
            {
                c.DropItem(new MarksOfTheScourge(amount));
            }
            else if (killer.Karma >= 0)
            {
                c.DropItem(new MarksOfHonor(amount));
            }
        }

        public static void AwardMarks(Mobile recipient, int type, int amount)
        {
            if (recipient == null || recipient.Deleted)
                return;

            if (amount <= 0)
                return;

            Item marks = null;
            string str = "";

            try
            {
                switch (type)
                {
                    case 0: marks = new MarksOfTheScourge(amount); str = "Scourge"; break;
                    case 1: marks = new MarksOfHonor(amount); str = "Honor"; break;
                    case 2: marks = new MarksOfTheShadowbroker(amount); str = "Shadowbroker"; break;
                    case 3: marks = new MarksOfTheWilds(amount); str = "Wilds"; break;
                    case 4: marks = new MarksOfDevotion(amount); str = "Devotion"; break;
                    case 5: marks = new MarksOfTheWeave(amount); str = "Weave"; break;
                    case 6: marks = new MarksOfBhaal(amount); str = "Bhaal"; break;
                    default:
                        return;
                }

                Container pack = recipient.Backpack;

                if (pack != null && !pack.Deleted)
                {
                    Item existing = null;

                    foreach (Item i in pack.Items)
                    {
                        if (i.GetType() == marks.GetType())
                        {
                            existing = i;
                            break;
                        }
                    }

                    if (existing != null)
                    {
                        existing.Amount += amount;

                        string msg = "You have gained " + amount + " mark" + (amount > 1 ? "s" : "") +
                                     " of the " + str + ".";
                        recipient.SendMessage(msg);

                        marks.Delete();
                    }
                    else
                    {
                        pack.DropItem(marks);

                        string msg = "You have received " + amount + " mark" +
                                     (amount > 1 ? "s" : "") + " of the " + str + ".";
                        recipient.SendMessage(msg);
                    }
                }
                else
                {
                    marks.MoveToWorld(recipient.Location, recipient.Map);
                    recipient.SendMessage("Your marks of the " + str + " have been placed at your feet.");
                }
            }
            catch (Exception) { }
        }
    }

}
