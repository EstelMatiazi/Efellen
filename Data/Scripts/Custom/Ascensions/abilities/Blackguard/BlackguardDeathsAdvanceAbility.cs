using System;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public class BlackguardDeathsAdvanceAbility : AscensionAbility
    {
        public override AscensionType Ascension { get { return AscensionType.Blackguard; } }
        public override int RequiredLevel       { get { return 6; } }
        public override string Name             { get { return "DeathsAdvance"; } }
        public override string        DisplayName { get { return "Deaths Advance"; } }
        public override bool IsPassive          { get { return false; } }
        public override TimeSpan Cooldown       { get { return TimeSpan.FromSeconds(9); } }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
            {
                pm.SendMessage("You cannot use Death's Advance right now.");
                return;
            }

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);

            pm.Target = new AdvanceTarget(this, prog.Level);
            pm.SendMessage("Choose a location to advance to.");
        }

        private class AdvanceTarget : Target
        {
            private readonly BlackguardDeathsAdvanceAbility m_Ability;
            private readonly int m_Level;

            public AdvanceTarget(BlackguardDeathsAdvanceAbility ability, int level)
                : base(12, true, TargetFlags.None)
            {
                m_Ability = ability;
                m_Level   = level;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                PlayerMobile pm = from as PlayerMobile;
                if (pm == null)
                    return;

                IPoint3D p = targeted as IPoint3D;
                if (p == null)
                    return;

                Map map = pm.Map;
                Point3D dest = new Point3D(p);

                int maxRange = 3 + (m_Level / 5);

                if (pm.GetDistanceToSqrt(dest) < 2)
                {
                    pm.SendMessage("You must advance at least 2 tiles.");
                    return;
                }

                if (pm.GetDistanceToSqrt(dest) > maxRange)
                {
                    pm.SendMessage("That location is too far away.");
                    return;
                }

                if (!map.CanFit(dest.X, dest.Y, dest.Z, 16, false, false))
                {
                    pm.SendMessage("You cannot advance there.");
                    return;
                }

                pm.MoveToWorld(dest, map);

                PlayArrivalEffect(pm, m_Level);
                DoImpactEffect(pm, dest, m_Level);

                int reduction     = m_Level / 3;
                int finalCooldown = Math.Max(3, 9 - reduction);

                pm.SetAbilityCooldown(m_Ability.Name, TimeSpan.FromSeconds(finalCooldown));
                new CooldownNotifyTimer(pm, TimeSpan.FromSeconds(finalCooldown)).Start();
            }
        }

        private static void DoImpactEffect(PlayerMobile pm, Point3D center, int level)
        {
            Map map = pm.Map;
            if (map == null)
                return;

            bool    doDamage      = level >= 12;
            int     knockbackTiles = 2 + (level / 5);
            int     knockbackChance = level;        
            int     strBonus      = pm.Str / 15;
            int     damageBase    = strBonus + (level / 3);

            IPooledEnumerable eable = map.GetMobilesInRange(center, 1);

            try
            {
                foreach (Mobile m in eable)
                {
                    if (m == null || m.Deleted || !m.Alive || m == pm)
                        continue;

                    if (!pm.CanBeHarmful(m, false))
                        continue;

                    if (doDamage)
                    {
                        pm.DoHarmful(m);
                        int dmg = Utility.RandomMinMax(30, 45) + damageBase;
                        AOS.Damage(m, pm, dmg, 100, 0, 0, 0, 0);
                    }

                    if (Utility.Random(100) < knockbackChance)
                        ApplyKnockback(pm, m, knockbackTiles, map);
                }
            }
            finally
            {
                eable.Free();
            }
        }

        private static void ApplyKnockback(PlayerMobile source, Mobile target, int tiles, Map map)
        {
            int dx = target.X - source.X;
            int dy = target.Y - source.Y;

            int stepX = dx == 0 ? 0 : (dx > 0 ? 1 : -1);
            int stepY = dy == 0 ? 0 : (dy > 0 ? 1 : -1);

            if (stepX == 0 && stepY == 0)
                return; 

            Point3D landing = target.Location;

            for (int i = 1; i <= tiles; i++)
            {
                int nx = target.X + (stepX * i);
                int ny = target.Y + (stepY * i);
                int nz = map.GetAverageZ(nx, ny);

                if (map.CanFit(nx, ny, nz, 16, false, false))
                    landing = new Point3D(nx, ny, nz);
                else
                    break;
            }

            if (landing != target.Location)
            {
                target.MoveToWorld(landing, map);

                source.FixedParticles(
                    0x36BD,
                    10,
                    30,
                    5052,
                    0x47E,
                    0,
                    EffectLayer.Waist
                );
            }
        }

        private static void PlayArrivalEffect(PlayerMobile pm, int level)
        {
            pm.FixedParticles(0x374A, 10, 15, 5021, 0x47E, 0, EffectLayer.Waist);
            pm.FixedParticles(0x376A, 9,  32, 5005, 0x47E, 0, EffectLayer.Waist);
            pm.PlaySound(0x1FB);

            pm.PublicOverheadMessage(MessageType.Regular, 0x47E, false, "*Death's Advance*");

            if (level >= 12)
            {
                pm.FixedParticles(0x3709, 15, 40, 5052, 0x47E, 0, EffectLayer.Head);
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

                m_Player.SendMessage(0x47E, "You can use Death's Advance again.");
            }
        }
    }
}