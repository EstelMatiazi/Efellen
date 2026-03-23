using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;

namespace Server.Custom.Ascensions
{
    public class ReaverAbsoluteTyrannyAbility : AscensionAbility
    {
        public override AscensionType Ascension     { get { return AscensionType.Reaver; } }
        public override int           RequiredLevel { get { return 18; } }
        public override string        Name          { get { return "AbsoluteTyranny"; } }
        public override string        DisplayName { get { return "Absolute Tyranny"; } }
        public override bool          IsPassive     { get { return false; } }
        public override TimeSpan      Cooldown      { get { return TimeSpan.FromMinutes(3); } }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
                return;

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            if (pm.Mana < 30 || pm.Stam < 30)
            {
                pm.SendMessage("You do not have enough mana or stamina.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            if (prog == null)
                return;

            int level = prog.Level;

            pm.Mana    -= 30;
            pm.Stam -= 30;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x675, false, "*Absolute Tyranny*");

            pm.FixedParticles(0x23B2, 10, 20, 0, 0x675, 0, EffectLayer.Waist);
            pm.PlaySound(0x51D);

            pm.AddAscensionEffect("AbsoluteTyranny", TimeSpan.FromSeconds(30), level);

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

                m_Player.SendMessage(0x675, "You can now use Absolute Tyranny again.");
            }
        }
    }
}