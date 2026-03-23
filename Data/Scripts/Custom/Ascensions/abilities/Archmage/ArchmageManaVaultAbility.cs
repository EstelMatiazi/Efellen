using System;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public class ArchmageManaVaultAbility : AscensionAbility
    {
        public override string Name { get { return "ManaVault"; } }

        public override string        DisplayName { get { return "Mana Vault"; } }

        public override AscensionType Ascension
        {
            get { return AscensionType.Archmage; }
        }

        public override int RequiredLevel { get { return 2; } }

        public override bool IsPassive { get { return true; } }

        public override TimeSpan Cooldown
        {
            get { return TimeSpan.Zero; }
        }
        public override void Execute(PlayerMobile pm)
        {
            // Passive ability
        }

        public int OnManaLoss(PlayerMobile pm, int amount, Mobile attacker)
        {
            if (pm == null || attacker == null)
                return 0;

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            if (prog == null || prog.Level < RequiredLevel)
                return 0;

            int level = prog.Level;

            int chance = level * 2;

            if (Utility.Random(100) >= chance)
                return 0;

            pm.Mana += amount;

            if (level >= 5)
            {
                int dmg = Utility.RandomMinMax(15, 25) + (level / 2);
                AOS.Damage(attacker, pm, dmg, 0, 0, 100, 0, 0);
            }

            if (level >= 13)
            {
                attacker.Paralyzed = true;
                Timer.DelayCall(TimeSpan.FromSeconds(2), delegate()
                {
                    if (attacker != null && !attacker.Deleted)
                        attacker.Paralyzed = false;
                });
            }

            pm.SendMessage("Your Mana Vault protects your energy!");
            pm.PrivateOverheadMessage(MessageType.Regular, 0x22, false, "*Mana Vault*", pm.NetState);
            return amount;
        }
    }
}
