using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public class SkaldSagaOfValorAbility : AscensionAbility
    {
        public override AscensionType Ascension     { get { return AscensionType.Skald; } }
        public override int           RequiredLevel { get { return 6; } }
        public override string        Name          { get { return "SagaOfValor"; } }
        public override bool          IsPassive     { get { return false; } }
        public override TimeSpan      Cooldown      { get { return TimeSpan.FromSeconds(90); } }

        private const int    BuffRadius = 4;
        private const string BuffKey    = "SagaOfValor";

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
                return;

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            if (pm.Mana < 30)
            {
                pm.SendMessage("You do not have enough mana.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            DoSagaOfValor(pm, prog.Level);
        }

        private void DoSagaOfValor(PlayerMobile pm, int level)
        {
            pm.Mana -= 30;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x445, false, "*Chants a Saga of Valor*");

            int duration  = 15 + level;
            int hitBonus  = level / 2;

            List<Mobile> targets = GetFriendlyTargets(pm);

            foreach (Mobile m in targets)
                PlaySagaEffect(m);

            for (int i = 0; i < targets.Count; i++)
            {
                PlayerMobile target = targets[i] as PlayerMobile;

                if (target != null)
                    target.AddAscensionEffect(BuffKey, TimeSpan.FromSeconds(duration), level);
            }

            new SagaExpiryTimer(pm, targets, level, TimeSpan.FromSeconds(duration)).Start();
            new CooldownNotifyTimer(pm, Cooldown).Start();
        }

        private static void PlaySagaEffect(Mobile m)
        {
            m.FixedParticles(0x376A, 9, 32, 5030, 0x445, 0, EffectLayer.Waist);
            m.PlaySound(0x1F5);
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

        private sealed class SagaExpiryTimer : Timer
        {
            private readonly PlayerMobile m_Caster;
            private readonly List<Mobile> m_Targets;
            private readonly int          m_Level;

            public SagaExpiryTimer(PlayerMobile caster, List<Mobile> targets,
                int level, TimeSpan duration)
                : base(duration)
            {
                m_Caster  = caster;
                m_Targets = targets;
                m_Level   = level;
                Priority  = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (m_Caster == null || m_Caster.Deleted)
                    return;

                for (int i = 0; i < m_Targets.Count; i++)
                {
                    Mobile m = m_Targets[i];

                    if (m == null || m.Deleted || !m.Alive)
                        continue;

                    m.FixedParticles(0x373A, 10, 15, 5018, 0x445, 0, EffectLayer.Waist);
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
                if (m_Player == null || m_Player.Deleted)
                    return;

                m_Player.SendMessage(0x445, "You can use Saga of Valor again.");
            }
        }
    }
}