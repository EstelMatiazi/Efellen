using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public class SkaldSongOfThunderAbility : AscensionAbility
    {
        public override AscensionType Ascension     { get { return AscensionType.Skald; } }
        public override int           RequiredLevel { get { return 11; } }
        public override string        Name          { get { return "SongOfThunder"; } }
        public override bool          IsPassive     { get { return false; } }
        public override TimeSpan      Cooldown      { get { return TimeSpan.FromSeconds(180); } }

        private const int StrikeRadius   = 4;
        private const int StrikeInterval = 3;

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
            DoSongOfThunder(pm, prog.Level);
        }

        private void DoSongOfThunder(PlayerMobile pm, int level)
        {
            pm.Mana -= 40;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x445, false, "*Chants a Song of Thunder*");

            pm.FixedParticles(0x2A4E, 10, 25, 5029, 0x445, 0, EffectLayer.Head);
            pm.PlaySound(0x29);

            int totalDuration = 15 + level;
            int totalTicks    = totalDuration / StrikeInterval;

            new SongOfThunderTimer(pm, level, totalTicks).Start();
            new CooldownNotifyTimer(pm, Cooldown).Start();
        }

        internal void DoSongOfThunderDirect(PlayerMobile pm, int level)
        {
            pm.PublicOverheadMessage(MessageType.Regular, 0x445, false, "*Chants a Song of Thunder*");

            pm.FixedParticles(0x2A4E, 10, 25, 5029, 0x445, 0, EffectLayer.Head);
            pm.PlaySound(0x29);

            int totalDuration = 15 + level;
            int totalTicks    = totalDuration / StrikeInterval;

            new SongOfThunderTimer(pm, level, totalTicks).Start();
        }

        private static void DoLightningStrike(PlayerMobile pm, int level)
        {
            Mobile target = GetRandomHostile(pm);

            if (target == null)
                return;

            int fameDamage = (int)(pm.Fame / 1000);
            int damage     = Utility.RandomMinMax(30, 45) + fameDamage;

            pm.DoHarmful(target);
            AOS.Damage(target, pm, damage, 0, 0, 0, 0, 100); 

            target.FixedParticles(0x2A4E, 10, 25, 5029, 0x445, 0, EffectLayer.Head);
            target.PlaySound(0x29);
        }

        private static Mobile GetRandomHostile(PlayerMobile pm)
        {
            Map map = pm.Map;
            if (map == null || map == Map.Internal)
                return null;

            List<Mobile> candidates = new List<Mobile>();

            IPooledEnumerable eable = map.GetMobilesInRange(pm.Location, StrikeRadius);

            try
            {
                foreach (Mobile m in eable)
                {
                    if (m == null || m.Deleted || !m.Alive || m == pm)
                        continue;

                    if (!pm.CanBeHarmful(m, false))
                        continue;

                    candidates.Add(m);
                }
            }
            finally
            {
                eable.Free();
            }

            if (candidates.Count == 0)
                return null;

            return candidates[Utility.Random(candidates.Count)];
        }

        private sealed class SongOfThunderTimer : Timer
        {
            private readonly PlayerMobile m_Caster;
            private readonly int          m_Level;
            private          int          m_TicksRemaining;

            public SongOfThunderTimer(PlayerMobile caster, int level, int totalTicks)
                : base(TimeSpan.FromSeconds(StrikeInterval), TimeSpan.FromSeconds(StrikeInterval))
            {
                m_Caster         = caster;
                m_Level          = level;
                m_TicksRemaining = totalTicks;
                Priority         = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (m_Caster == null || m_Caster.Deleted || !m_Caster.Alive)
                {
                    Stop();
                    return;
                }

                if (m_TicksRemaining <= 0)
                {
                    Stop();
                    return;
                }

                m_TicksRemaining--;

                DoLightningStrike(m_Caster, m_Level);

                if (m_Level >= 16)
                    DoLightningStrike(m_Caster, m_Level);

                if (m_TicksRemaining <= 0)
                    Stop();
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

                m_Player.SendMessage(0x445, "You can use Song of Thunder again.");
            }
        }
    }
}