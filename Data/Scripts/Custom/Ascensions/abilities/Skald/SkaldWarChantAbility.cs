using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public class SkaldWarChantAbility : AscensionAbility
    {
        public override AscensionType  Ascension     { get { return AscensionType.Skald; } }
        public override int            RequiredLevel { get { return 1; } }
        public override string         Name          { get { return "WarChant"; } }
        public override bool           IsPassive     { get { return false; } }
        public override TimeSpan       Cooldown      { get { return TimeSpan.FromSeconds(90); } }

        private const int   BuffRadius    = 4;
        private const string BuffKey      = "WarChant";

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
            DoWarChant(pm, prog.Level);
        }

        private void DoWarChant(PlayerMobile pm, int level)
        {
            pm.Mana -= 20;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x445, false, "*plays a war chant*");

            int duration   = 10 + level;
            int statBonus  = 5 + level;
            int skillBonus = 5 + level;

            // Snapshot targets
            List<Mobile> targets = GetFriendlyTargets(pm);

            // Registry
            foreach (Mobile m in targets)
                ApplyBuff(m, pm, level, statBonus, skillBonus, duration);

            // OnHit and OnBeforeDeath registry
            pm.AddAscensionEffect(BuffKey, TimeSpan.FromSeconds(duration), level);

            new WarChantExpiryTimer(pm, targets, level, statBonus, skillBonus,
                TimeSpan.FromSeconds(duration)).Start();

            new CooldownNotifyTimer(pm, Cooldown).Start();
        }

        private static void ApplyBuff(Mobile target, PlayerMobile caster,
            int level, int statBonus, int skillBonus, int duration)
        {
            string key = BuffKey + "_" + caster.Serial;

            target.AddStatMod(new StatMod(StatType.Str, key + "_Str", statBonus, TimeSpan.Zero));
            target.AddStatMod(new StatMod(StatType.Dex, key + "_Dex", statBonus, TimeSpan.Zero));

            target.AddSkillMod(new DefaultSkillMod(SkillName.Tactics,     true, skillBonus));
            target.AddSkillMod(new DefaultSkillMod(SkillName.MagicResist, true, skillBonus));

            target.FixedParticles(0x376A, 9, 32, 5030, 0x445, 0, EffectLayer.Waist);
            target.PlaySound(0x1F5);
        }

        private static void RemoveBuff(Mobile target, PlayerMobile caster,
            int statBonus, int skillBonus)
        {
            string key = BuffKey + "_" + caster.Serial;

            target.RemoveStatMod(key + "_Str");
            target.RemoveStatMod(key + "_Dex");

            RemoveSkillModByValue(target, SkillName.Tactics,     skillBonus);
            RemoveSkillModByValue(target, SkillName.MagicResist, skillBonus);
        }

        private static void RemoveSkillModByValue(Mobile m, SkillName skill, double value)
        {
            if (m.SkillMods == null)
                return;

            for (int i = m.SkillMods.Count - 1; i >= 0; i--)
            {
                SkillMod mod = (SkillMod)m.SkillMods[i];

                if (mod.Skill == skill && mod.Value == value)
                {
                    mod.Remove();
                    break; 
                }
            }
        }

        private static List<Mobile> GetFriendlyTargets(PlayerMobile pm)
        {
            List<Mobile> targets = new List<Mobile>();
            targets.Add(pm); 
            Map map = pm.Map;
            if (map == null || map == Map.Internal)
                return targets;

            IPooledEnumerable eable = map.GetMobilesInRange(pm.Location, BuffRadius);

            try
            {
                foreach (Mobile m in eable)
                {
                    if (m == null || m.Deleted || !m.Alive || m == pm)
                        continue;

                    if (!pm.CanBeHarmful(m, false))
                        targets.Add(m);
                }
            }
            finally
            {
                eable.Free();
            }

            return targets;
        }

        private sealed class WarChantExpiryTimer : Timer
        {
            private readonly PlayerMobile  m_Caster;
            private readonly List<Mobile>  m_Targets;
            private readonly int           m_Level;
            private readonly int           m_StatBonus;
            private readonly int           m_SkillBonus;

            public WarChantExpiryTimer(PlayerMobile caster, List<Mobile> targets,
                int level, int statBonus, int skillBonus, TimeSpan duration)
                : base(duration)
            {
                m_Caster     = caster;
                m_Targets    = targets;
                m_Level      = level;
                m_StatBonus  = statBonus;
                m_SkillBonus = skillBonus;
                Priority     = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (m_Caster == null || m_Caster.Deleted)
                    return;

                for (int i = 0; i < m_Targets.Count; i++)
                {
                    Mobile target = m_Targets[i];

                    if (target == null || target.Deleted)
                        continue;

                    RemoveBuff(target, m_Caster, m_StatBonus, m_SkillBonus);
                }

                if (m_Level >= 20 && Utility.Random(100) < (m_Level * 2))
                {
                    m_Caster.SendMessage(0x445, "The war chant echoes onward!");

                    AscensionProgress prog = m_Caster.AscensionProfile.Get(AscensionType.Skald);

                    new SkaldWarChantAbility().DoWarChantDirect(m_Caster, prog.Level);
                }
            }
        }

        internal void DoWarChantDirect(PlayerMobile pm, int level)
        {
            pm.PublicOverheadMessage(MessageType.Regular, 0x445, false, "*the war chant continues!*");

            int duration   = 10 + level;
            int statBonus  = 5 + level;
            int skillBonus = 5 + level;

            List<Mobile> targets = GetFriendlyTargets(pm);

            foreach (Mobile m in targets)
                ApplyBuff(m, pm, level, statBonus, skillBonus, duration);

            pm.AddAscensionEffect(BuffKey, TimeSpan.FromSeconds(duration), level);

            new WarChantExpiryTimer(pm, targets, level, statBonus, skillBonus,
                TimeSpan.FromSeconds(duration)).Start();
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

                m_Player.SendMessage(0x445, "You can use War Chant again.");
            }
        }
    }
}