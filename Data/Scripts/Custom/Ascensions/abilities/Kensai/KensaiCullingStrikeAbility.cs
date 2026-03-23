using System;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public class KensaiCullingStrikeAbility : AscensionAbility
    {
        public override AscensionType Ascension     { get { return AscensionType.Kensai; } }
        public override int           RequiredLevel { get { return 11; } }
        public override string        Name          { get { return "CullingStrike"; } }
        public override string        DisplayName { get { return "Culling Strike"; } }
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

            if (pm.Stam < 50)
            {
                pm.SendMessage("You do not have enough stamina.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            if (prog == null)
                return;

            pm.Stam -= 50;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x448, false, "*Culling Strike*");
            pm.FixedParticles(0x376A, 9, 32, 5030, 0x448, 0, EffectLayer.Waist);
            pm.PlaySound(0x1F5);

            pm.AddAscensionEffect("CullingStrike", TimeSpan.FromSeconds(30), prog.Level);

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

                m_Player.SendMessage(0x448, "Culling Strike can be used again.");
            }
        }
    }
}