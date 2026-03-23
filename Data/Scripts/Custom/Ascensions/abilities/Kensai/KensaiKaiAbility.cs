using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Custom.Ascensions
{
    public class KensaiKaiAbility : AscensionAbility
    {
        public override AscensionType Ascension     { get { return AscensionType.Kensai; } }
        public override int           RequiredLevel { get { return 6; } }
        public override string        Name          { get { return "Kai"; } }
        public override string        DisplayName { get { return "Kai"; } }
        public override bool          IsPassive     { get { return false; } }

        public override TimeSpan Cooldown
        {
            get { return TimeSpan.FromSeconds(1); }
        }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
                return;

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            if (pm.Stam < 30)
            {
                pm.SendMessage("You do not have enough stamina.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            if (prog == null)
                return;

            int level    = prog.Level;
            int maxRange = 3 + (level / 4);

            pm.SendMessage(0x448, "Choose a destination.");
            pm.Target = new KaiTarget(pm, level, maxRange);
        }

        private sealed class KaiTarget : Target
        {
            private readonly PlayerMobile m_Caster;
            private readonly int          m_Level;
            private readonly int          m_MaxRange;

            public KaiTarget(PlayerMobile pm, int level, int maxRange)
                : base(maxRange, true, TargetFlags.None)
            {
                m_Caster   = pm;
                m_Level    = level;
                m_MaxRange = maxRange;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D p = targeted as IPoint3D;

                if (p == null)
                    return;

                Map map = m_Caster.Map;

                if (map == null)
                    return;

                Point3D dest = new Point3D(p.X, p.Y, p.Z);

                if (m_Caster.GetDistanceToSqrt(dest) < 2)
                {
                    m_Caster.SendMessage("You must leap at least 2 tiles away.");
                    return;
                }

                if (!map.CanFit(dest.X, dest.Y, dest.Z, 16, false, false))
                {
                    dest = new Point3D(dest.X, dest.Y, map.GetAverageZ(dest.X, dest.Y));

                    if (!map.CanFit(dest.X, dest.Y, dest.Z, 16, false, false))
                    {
                        m_Caster.SendMessage("You cannot end your strike there.");
                        return;
                    }
                }

                int cooldownSecs = 9 - (m_Level / 3);
                if (cooldownSecs < 1) cooldownSecs = 1;

                m_Caster.Stam -= 30;
                m_Caster.SetAbilityCooldown("Kai", TimeSpan.FromSeconds(cooldownSecs));

                m_Caster.PublicOverheadMessage(MessageType.Regular, 0x448, false, "*Kai*");

                m_Caster.FixedParticles(0x376A, 9, 32, 5030, 0x448, 0, EffectLayer.Waist);
                m_Caster.PlaySound(0x214);

                m_Caster.MoveToWorld(dest, map);

                Effects.SendLocationParticles(
                    EffectItem.Create(dest, map, EffectItem.DefaultDuration),
                    0x5492, 10, 20, 0x448, 0, 0, 0
                );
                Effects.PlaySound(dest, map, 0x14F);

                int    dexBonus    = m_Caster.Dex / 15;
                int    levelBonus  = m_Level / 3;
                int    damage      = Utility.RandomMinMax(20, 35) + dexBonus + levelBonus;

                ArrayList targets = new ArrayList();

                IPooledEnumerable eable = map.GetMobilesInRange(dest, 1);

                try
                {
                    foreach (Mobile m in eable)
                    {
                        if (m == null || m.Deleted || !m.Alive || m == m_Caster)
                            continue;

                        if (!m_Caster.CanBeHarmful(m, false))
                            continue;

                        targets.Add(m);
                    }
                }
                finally
                {
                    eable.Free();
                }

                m_Caster.RevealingAction();

                for (int i = 0; i < targets.Count; i++)
                {
                    Mobile m = (Mobile)targets[i];

                    if (m.Deleted || !m.Alive)
                        continue;

                    m_Caster.DoHarmful(m);

                    m.FixedParticles(0x5492, 10, 15, 0, 0x448, 0, EffectLayer.Waist);
                    AOS.Damage(m, m_Caster, damage, 100, 0, 0, 0, 0);

                    if (m_Level >= 12)
                    {
                        BaseWeapon kaiWeapon = m_Caster.Weapon as BaseWeapon;

                        if (kaiWeapon != null && KensaiHelpers.IsSword(kaiWeapon)
                            && Utility.Random(100) < m_Level)
                        {
                            KensaiHelpers.ApplyCullingStrikeProc(m_Caster, m, m_Level);
                        }
                    }
                }

                new CooldownNotifyTimer(m_Caster, TimeSpan.FromSeconds(cooldownSecs)).Start();
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
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

                m_Player.SendMessage(0x448, "Kai can be used again.");
            }
        }
    }
}