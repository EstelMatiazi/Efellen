using System;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public class HierophantDivinePowerAbility : AscensionAbility
    {
        public override AscensionType Ascension     { get { return AscensionType.Hierophant; } }
        public override int           RequiredLevel { get { return 18; } }
        public override string        Name          { get { return "DivinePower"; } }
        public override string        DisplayName { get { return "Divine Power"; } }
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

            if (pm.Mana < 55)
            {
                pm.SendMessage("You do not have enough mana.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            if (prog == null)
                return;

            int level    = prog.Level;
            int duration = 30 + level;

            pm.Mana -= 55;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x439, false, "*Divine Power*");
            pm.FixedParticles(0x376A, 9, 32, 5030, 0x439, 0, EffectLayer.Waist);
            pm.PlaySound(0x1F5);

            string modKey = "DivinePower_" + pm.Serial.Value.ToString();

            pm.AddStatMod(new StatMod(StatType.Str, modKey + "_Str", 20, TimeSpan.Zero));
            pm.AddStatMod(new StatMod(StatType.Dex, modKey + "_Dex", 15, TimeSpan.Zero));

            pm.AddSkillMod(new DefaultSkillMod(SkillName.Tactics,      true, 15));
            pm.AddSkillMod(new DefaultSkillMod(SkillName.Spiritualism,  true, 15));

            pm.AddAscensionEffect("DivinePower", TimeSpan.FromSeconds(duration), level);

            new DivinePowerExpiryTimer(pm, TimeSpan.FromSeconds(duration), modKey).Start();
            new CooldownNotifyTimer(pm, Cooldown).Start();
        }

        private sealed class DivinePowerExpiryTimer : Timer
        {
            private readonly PlayerMobile m_Player;
            private readonly string       m_ModKey;

            public DivinePowerExpiryTimer(PlayerMobile pm, TimeSpan delay, string modKey)
                : base(delay)
            {
                m_Player = pm;
                m_ModKey = modKey;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (m_Player == null || m_Player.Deleted)
                    return;

                m_Player.RemoveStatMod(m_ModKey + "_Str");
                m_Player.RemoveStatMod(m_ModKey + "_Dex");

                for (int i = m_Player.SkillMods.Count - 1; i >= 0; i--)
                {
                    SkillMod mod = (SkillMod)m_Player.SkillMods[i];

                    if (mod is DefaultSkillMod)
                    {
                        DefaultSkillMod dsm = (DefaultSkillMod)mod;

                        if ((dsm.Skill == SkillName.Tactics     && dsm.Value == 15) ||
                            (dsm.Skill == SkillName.Spiritualism && dsm.Value == 15))
                        {
                            m_Player.RemoveSkillMod(dsm);
                        }
                    }
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
                m_Player.SendMessage(0x439, "Divine Power can be used again.");
            }
        }
    }
}