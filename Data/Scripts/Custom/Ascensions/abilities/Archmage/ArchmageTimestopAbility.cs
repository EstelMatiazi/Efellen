using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public class ArchmageTimestopAbility : AscensionAbility
    {
        public override string Name          { get { return "TimeStop"; } }
        public override string        DisplayName { get { return "Time Stop"; } }
        public override AscensionType Ascension { get { return AscensionType.Archmage; } }
        public override int RequiredLevel    { get { return 18; } }
        public override bool IsPassive       { get { return false; } }

        public override TimeSpan Cooldown
        {
            get { return TimeSpan.FromSeconds(180); }
        }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
            {
                pm.SendMessage("You cannot use Time Stop.");
                return;
            }

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            if (pm.Mana < 60)
            {
                pm.SendMessage("You do not have enough mana.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            if (prog == null)
                return;

            int level          = prog.Level;
            int range          = 3 + (level / 5);
            double duration    = 12 + (level / 4);
            bool drainOnExpire = (level >= 20);
            int drain          = drainOnExpire ? 40 + level : 0;

            pm.Mana -= 60;
            pm.SetAbilityCooldown(Name, Cooldown);

            IPooledEnumerable eable = pm.Map.GetMobilesInRange(pm.Location, range);

            try
            {
                foreach (Mobile m in eable)
                {
                    if (m == pm || !m.Alive)
                        continue;

                    BaseCreature bc = m as BaseCreature;
                    if (bc == null || bc.IsDeadBondedPet || bc.Controlled)
                        continue;

                    if (!pm.CanBeHarmful(bc))
                        continue;

                    pm.DoHarmful(bc);
                    bc.Paralyze(TimeSpan.FromSeconds(duration));
                    Effects.SendLocationEffect(bc.Location, bc.Map, 0x376A, 20, 10, 0x0213, 0);

                    if (drainOnExpire)
                    {
                        bc.Mana = Math.Max(bc.Mana - drain, 0);
                        bc.Stam = Math.Max(bc.Stam - drain, 0);
                    }
                }
            }
            finally
            {
                eable.Free();
            }
        }
    }
}