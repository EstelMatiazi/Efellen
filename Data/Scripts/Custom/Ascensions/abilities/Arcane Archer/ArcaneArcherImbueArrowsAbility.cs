using System;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public class ArcaneArcherImbueArrowsAbility : AscensionAbility
    {
        public override AscensionType Ascension     { get { return AscensionType.ArcaneArcher; } }
        public override int           RequiredLevel { get { return 1; } }
        public override string        Name          { get { return "ImbueArrows"; } }

        public override string        DisplayName { get { return "Imbue Arrows"; } }
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

            if (pm.Mana < 40)
            {
                pm.SendMessage("You do not have enough mana.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            if (prog == null)
                return;

            int level    = prog.Level;
            int duration = 30 + level;

            pm.Mana -= 40;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x48F, false, "*Imbue Arrows*");
            pm.FixedParticles(0x376A, 9, 32, 5030, 0x48F, 0, EffectLayer.Waist);
            pm.PlaySound(0x1F5);

            pm.AddAscensionEffect("ImbueArrows", TimeSpan.FromSeconds(duration), level);

            new ImbueArrowsExpiryTimer(pm, TimeSpan.FromSeconds(duration)).Start();
            new CooldownNotifyTimer(pm, Cooldown).Start();
        }

        private sealed class ImbueArrowsExpiryTimer : Timer
        {
            private readonly PlayerMobile m_Player;

            public ImbueArrowsExpiryTimer(PlayerMobile pm, TimeSpan delay)
                : base(delay)
            {
                m_Player = pm;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (m_Player == null || m_Player.Deleted)
                    return;

                AscensionProgress prog = m_Player.AscensionProfile.Get(AscensionType.ArcaneArcher);
                if (prog == null)
                    return;

                int level = prog.Level;

                if (level >= 20 && Utility.Random(100) < level)
                {
                    m_Player.SetAbilityCooldown("Barrage", TimeSpan.Zero);
                    m_Player.SendMessage(0x48F, "Barrage can be used again.");
                }
            }
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
                if (m_Player == null || m_Player.Deleted) return;
                m_Player.SendMessage(0x48F, "Imbue Arrows can be used again.");
            }
        }
    }
}