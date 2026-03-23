using System;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public class ArchmageConfluxAbility : AscensionAbility
    {
        public override string Name
        {
            get { return "Conflux"; }
        }
        public override string        DisplayName { get { return "Conflux"; } }
        public override AscensionType Ascension
        {
            get { return AscensionType.Archmage; }
        }

        public override int RequiredLevel
        {
            get { return 6; }
        }

        public override bool IsPassive
        {
            get { return false; }
        }

        public override TimeSpan Cooldown
        {
            get { return TimeSpan.FromMinutes(2); }
        }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
            {
                pm.SendMessage("You cannot use Conflux.");
                return;
            }

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            int level = prog.Level;

            ActivateConflux(pm, level);

            pm.SetAbilityCooldown(Name, Cooldown);
            pm.PublicOverheadMessage(
                MessageType.Regular,
                0x22,
                false,
                "*Conflux*"
            );
            Timer.DelayCall(
                Cooldown,
                new TimerCallback(
                    delegate
                    {
                        if (pm != null && !pm.Deleted)
                            pm.SendMessage("You may use Conflux again.");
                    }
                )
            );
        }

        private void ActivateConflux(PlayerMobile pm, int level)
        {
            double scalar;

            if (level >= 12)
                scalar = 0.15;
            else
                scalar = 0.10;

            int duration = 10 + level;

            pm.SendMessage("Arcane power surges through you!");

            pm.ArchmageConfluxScalar = scalar;
            pm.ArchmageConfluxEnd = DateTime.UtcNow + TimeSpan.FromSeconds(duration);

            Timer.DelayCall(
                TimeSpan.FromSeconds(duration),
                new TimerStateCallback(RemoveConflux),
                pm
            );
        }

        private void RemoveConflux(object state)
        {
            PlayerMobile pm = state as PlayerMobile;

            if (pm == null || pm.Deleted)
                return;

            pm.ArchmageConfluxScalar = 0.0;
            pm.SendMessage("The Conflux fades.");
        }
    }
}
