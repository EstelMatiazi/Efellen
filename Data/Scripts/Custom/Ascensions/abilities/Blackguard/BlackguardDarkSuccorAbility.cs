using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;

namespace Server.Custom.Ascensions
{
    public class BlackguardDarkSuccorAbility : AscensionAbility
    {
        public override AscensionType Ascension { get { return AscensionType.Blackguard; } }
        public override int RequiredLevel       { get { return 1; } }
        public override string Name             { get { return "DarkSuccor"; } }
        public override string        DisplayName { get { return "Dark Succor"; } }
        public override bool IsPassive          { get { return false; } }
        public override TimeSpan Cooldown       { get { return TimeSpan.FromSeconds(90); } }

        private const string BuffKey = "DarkSuccor";

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
                return;

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            if (pm.Mana < 20)
            {
                pm.SendMessage("You do not have enough mana.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            DoDarkSuccor(pm, prog.Level);
        }

        private void DoDarkSuccor(PlayerMobile pm, int level)
        {
            pm.Mana -= 20;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x47E, false, "*Dark Succor*");

            pm.FixedParticles(0x374A, 10, 15, 5021, 0x47E, 0, EffectLayer.Waist);
            pm.FixedParticles(0x376A, 9,  32, 5005, 0x47E, 0, EffectLayer.Waist);
            pm.PlaySound(0x1FB);

            int strBonus  = 15 + (level / 2);
            int duration  = 30 + level;

            StatMod strMod = new StatMod(StatType.Str, BuffKey + "_Str", strBonus, TimeSpan.FromSeconds(duration));
            pm.AddStatMod(strMod);

            pm.AddAscensionEffect(BuffKey, TimeSpan.FromSeconds(duration), level);

            new DarkSuccorExpiryTimer(pm, level, TimeSpan.FromSeconds(duration)).Start();
            new CooldownNotifyTimer(pm, Cooldown).Start();
        }

        private sealed class DarkSuccorExpiryTimer : Timer
        {
            private readonly PlayerMobile m_Player;
            private readonly int          m_Level;

            public DarkSuccorExpiryTimer(PlayerMobile pm, int level, TimeSpan duration)
                : base(duration)
            {
                m_Player = pm;
                m_Level  = level;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (m_Player == null || m_Player.Deleted)
                    return;

                int manaDrain = m_Player.Mana / 2;
                m_Player.Mana -= manaDrain;

                m_Player.SendMessage(0x47E, "The dark trance fades, draining your magical reserves.");
                m_Player.FixedParticles(0x374A, 10, 15, 5021, 0x47E, 0, EffectLayer.Waist);
                m_Player.PlaySound(0x1FB);
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
                if (m_Player == null || m_Player.Deleted)
                    return;

                m_Player.SendMessage(0x47E, "You can use Dark Succor again.");
            }
        }
    }
}