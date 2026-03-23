using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public class BlackguardChainsOfIceAbility : AscensionAbility
    {
        public override AscensionType Ascension { get { return AscensionType.Blackguard; } }
        public override int RequiredLevel       { get { return 11; } }
        public override string Name             { get { return "ChainsOfIce"; } }
        public override string        DisplayName { get { return "Chains Of Ice"; } }
        public override bool IsPassive          { get { return false; } }
        public override TimeSpan Cooldown       { get { return TimeSpan.FromSeconds(90); } }

        private const int ChainRadius = 6;

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
            DoChainsOfIce(pm, prog.Level);
        }

        private void DoChainsOfIce(PlayerMobile pm, int level)
        {
            pm.Mana -= 30;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x47E, false, "*Chains of Ice*");

            pm.FixedParticles(0x374A, 10, 30, 5013, 0x47E, 0, EffectLayer.Waist);
            pm.FixedParticles(0x376A, 9,  32, 5013, 0x47E, 0, EffectLayer.Waist);
            pm.PlaySound(0x10B);

            int  freezeDuration = 3 + (level / 4);
            int  damage         = Utility.RandomMinMax(30, 43) + (pm.Str / 15);
            bool chainExplosion = level >= 16;
            Map  map            = pm.Map;

            // Snapshot targets at cast time
            List<Mobile> frozen = new List<Mobile>();

            IPooledEnumerable eable = map.GetMobilesInRange(pm.Location, ChainRadius);

            try
            {
                foreach (Mobile m in eable)
                {
                    if (m == null || m.Deleted || !m.Alive || m == pm)
                        continue;

                    if (!pm.CanBeHarmful(m, false))
                        continue;

                    pm.DoHarmful(m);
                    AOS.Damage(m, pm, damage, 0, 0, 100, 0, 0);
                    m.Paralyze(TimeSpan.FromSeconds(freezeDuration));

                    m.FixedParticles(0x374A, 10, 15, 5013, 0x47E, 0, EffectLayer.Waist);
                    m.PlaySound(0x10B);

                    if (m.Alive)
                        frozen.Add(m);
                }
            }
            finally
            {
                eable.Free();
            }

            if (chainExplosion && frozen.Count > 0)
                new ChainExplosionTimer(pm, frozen, freezeDuration, pm.Str).Start();

            new CooldownNotifyTimer(pm, Cooldown).Start();
        }

        // ── Level 16: Chain Explosion timer ──────────────────────────────────
        private sealed class ChainExplosionTimer : Timer
        {
            private readonly PlayerMobile  m_Caster;
            private readonly List<Mobile>  m_Frozen;
            private readonly int           m_Str;

            public ChainExplosionTimer(PlayerMobile caster, List<Mobile> frozen, int delay, int str)
                : base(TimeSpan.FromSeconds(delay))
            {
                m_Caster = caster;
                m_Frozen = frozen;
                m_Str    = str;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (m_Caster == null || m_Caster.Deleted)
                    return;

                Map map = m_Caster.Map;
                if (map == null)
                    return;

                // Build origin set and bounding box in one pass
                Dictionary<int, Mobile> originSet = new Dictionary<int, Mobile>();

                int minX = int.MaxValue, minY = int.MaxValue;
                int maxX = int.MinValue, maxY = int.MinValue;

                for (int i = 0; i < m_Frozen.Count; i++)
                {
                    Mobile origin = m_Frozen[i];

                    if (origin == null || origin.Deleted || !origin.Alive)
                        continue;

                    originSet[origin.Serial] = origin;

                    if (origin.X - 1 < minX) minX = origin.X - 1;
                    if (origin.Y - 1 < minY) minY = origin.Y - 1;
                    if (origin.X + 1 > maxX) maxX = origin.X + 1;
                    if (origin.Y + 1 > maxY) maxY = origin.Y + 1;
                }

                if (originSet.Count == 0)
                    return;

                int centerX  = (minX + maxX) / 2;
                int centerY  = (minY + maxY) / 2;
                int centerZ  = map.GetAverageZ(centerX, centerY);
                int queryRad = ((maxX - minX) + (maxY - minY)) / 2 + 1;

                int explosionDamage = Utility.RandomMinMax(30, 43) + (m_Str / 15);

                Dictionary<int, bool>  alreadyHit  = new Dictionary<int, bool>();
                IPooledEnumerable      eable        = map.GetMobilesInRange(new Point3D(centerX, centerY, centerZ), queryRad);

                try
                {
                    foreach (Mobile m in eable)
                    {
                        if (m == null || m.Deleted || !m.Alive || m == m_Caster)
                            continue;

                        if (alreadyHit.ContainsKey(m.Serial))
                            continue;

                        if (!IsAdjacentToAnyOrigin(m, originSet))
                            continue;

                        if (!m_Caster.CanBeHarmful(m, false))
                            continue;

                        m_Caster.DoHarmful(m);
                        AOS.Damage(m, m_Caster, explosionDamage, 0, 0, 100, 0, 0);

                        m.FixedParticles(0x374A, 10, 15, 5013, 0x47E, 0, EffectLayer.Waist);
                        m.PlaySound(0x10B);

                        alreadyHit[m.Serial] = true;
                    }
                }
                finally
                {
                    eable.Free();
                }
            }

            // Chebyshev distance check — four integer comparisons with no floating point
            private static bool IsAdjacentToAnyOrigin(Mobile candidate, Dictionary<int, Mobile> origins)
            {
                foreach (KeyValuePair<int, Mobile> kvp in origins)
                {
                    Mobile origin = kvp.Value;
                    int dx = candidate.X - origin.X;
                    int dy = candidate.Y - origin.Y;

                    if (dx >= -1 && dx <= 1 && dy >= -1 && dy <= 1)
                        return true;
                }

                return false;
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

                m_Player.SendMessage(0x47E, "You can use Chains of Ice again.");
            }
        }
    }
}