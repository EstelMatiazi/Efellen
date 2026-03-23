using System;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public class KensaiBattleMeditationAbility : AscensionAbility
    {
        public override AscensionType Ascension     { get { return AscensionType.Kensai; } }
        public override int           RequiredLevel { get { return 1; } }
        public override string        Name          { get { return "BattleMeditation"; } }
        public override string        DisplayName { get { return "Battle Meditation"; } }
        public override bool          IsPassive     { get { return false; } }
        public override TimeSpan      Cooldown      { get { return TimeSpan.FromMinutes(2); } }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
                return;

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            if (pm.Stam < 20)
            {
                pm.SendMessage("You do not have enough stamina.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            if (prog == null)
                return;

            int level    = prog.Level;
            int duration = 20 + level;

            pm.Stam -= 20;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x448, false, "*Battle Meditation*");
            pm.FixedParticles(0x376A, 9, 32, 5030, 0x448, 0, EffectLayer.Waist);
            pm.PlaySound(0x1F5);

            pm.AddAscensionEffect("BattleMeditation", TimeSpan.FromSeconds(duration), level);

            new CooldownNotifyTimer(pm, Cooldown, "Battle Meditation").Start();
        }

        private sealed class CooldownNotifyTimer : Timer
        {
            private readonly PlayerMobile m_Player;
            private readonly string       m_Name;

            public CooldownNotifyTimer(PlayerMobile pm, TimeSpan delay, string name)
                : base(delay)
            {
                m_Player  = pm;
                m_Name    = name;
                Priority  = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (m_Player == null || m_Player.Deleted)
                    return;

                m_Player.SendMessage(0x448, "Battle Meditation can be used again.");
            }
        }
    }
}