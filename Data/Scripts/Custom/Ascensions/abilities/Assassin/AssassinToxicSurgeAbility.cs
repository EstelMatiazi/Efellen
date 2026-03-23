using System;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public class AssassinToxicSurgeAbility : AscensionAbility
    {
        public override AscensionType Ascension { get { return AscensionType.Assassin; } }
        public override int RequiredLevel       { get { return 11; } }
        public override string Name             { get { return "ToxicSurge"; } }
        public override string        DisplayName { get { return "Toxic Surge"; } }
        public override bool IsPassive          { get { return false; } }
        public override TimeSpan Cooldown       { get { return TimeSpan.FromMinutes(2); } }

        private static readonly TimeSpan BuffDuration = TimeSpan.FromSeconds(30);
        private const string BuffKey = "ToxicSurge";

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
                return;

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            if (pm.Mana < 40)
            {
                pm.SendMessage("You do not have enough mana.");
                return;
            }

            if (pm.Stam < 40)
            {
                pm.SendMessage("You do not have enough stamina.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            if (prog == null)
                return;

            DoToxicSurge(pm, prog.Level);
        }

        private void DoToxicSurge(PlayerMobile pm, int level)
        {
            pm.Mana -= 40;
            pm.Stam -= 40;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x233, false, "*Toxic Surge*");

            pm.AddAscensionEffect(BuffKey, BuffDuration, level);

            pm.FixedParticles(0x374A, 10, 15, 5021, 0x233, 0, EffectLayer.Waist);
            pm.PlaySound(0x474);

            if (level >= 16 && Utility.Random(100) < level)
                pm.SetAbilityCooldown("NoxiousCloud", TimeSpan.Zero);

            new CooldownNotifyTimer(pm, Cooldown).Start();
        }

        private sealed class CooldownNotifyTimer : Timer
        {
            private readonly PlayerMobile m_Player;

            public CooldownNotifyTimer(PlayerMobile pm, TimeSpan delay)
                : base(delay)
            {
                m_Player = pm;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (m_Player == null || m_Player.Deleted)
                    return;

                m_Player.SendMessage(0x233, "You can now Toxic Surge again.");
            }
        }
    }
}