using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Custom.Ascensions
{
    public class ArcaneArcherChargedArrowsAbility : AscensionAbility
    {
        public override AscensionType Ascension     { get { return AscensionType.ArcaneArcher; } }
        public override int           RequiredLevel { get { return 6; } }
        public override string        Name          { get { return "ChargedArrows"; } }

        public override string        DisplayName { get { return "Charged Arrows"; } }
        public override bool          IsPassive     { get { return false; } }
        public override TimeSpan      Cooldown      { get { return TimeSpan.FromMinutes(1); } }

        //  0=phys, 1=fire, 2=cold, 3=poison, 4=energy
        private static readonly int[] ElementPhys = new int[] { 100,   0,   0,   0,   0 };
        private static readonly int[] ElementFire = new int[] {   0, 100,   0,   0,   0 };
        private static readonly int[] ElementCold = new int[] {   0,   0, 100,   0,   0 };
        private static readonly int[] ElementPois = new int[] {   0,   0,   0, 100,   0 };
        private static readonly int[] ElementNrgy = new int[] {   0,   0,   0,   0, 100 };

        private static readonly int[][] Elements = new int[][]
        {
            ElementPhys, ElementFire, ElementCold, ElementPois, ElementNrgy
        };

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
                return;

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            if (pm.Mana < 50)
            {
                pm.SendMessage("You do not have enough mana.");
                return;
            }

            BaseWeapon weapon = pm.Weapon as BaseWeapon;
            if (weapon == null || !(weapon is BaseRanged))
            {
                pm.SendMessage("You must be wielding a ranged weapon.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            if (prog == null)
                return;

            int level = prog.Level;

            pm.SendMessage(0x48F, "Choose a direction to fire.");
            pm.Target = new ChargedArrowsTarget(pm, level);
        }

        private sealed class ChargedArrowsTarget : Target
        {
            private readonly PlayerMobile m_Caster;
            private readonly int          m_Level;

            public ChargedArrowsTarget(PlayerMobile pm, int level)
                : base(6, true, TargetFlags.None)
            {
                m_Caster = pm;
                m_Level  = level;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D p = targeted as IPoint3D;
                if (p == null) return;

                Map map = m_Caster.Map;
                if (map == null) return;

                m_Caster.Mana -= 50;
                m_Caster.SetAbilityCooldown("ChargedArrows", TimeSpan.FromMinutes(1));

                m_Caster.PublicOverheadMessage(MessageType.Regular, 0x48F, false, "*Charged Arrows*");

                int    elemIdx  = Utility.Random(5);
                int[]  elemSplit = Elements[elemIdx];

                int intBonus = m_Caster.Int / 15;
                int dexBonus = m_Caster.Dex / 15;
                int damage   = Utility.RandomMinMax(45, 75) + intBonus + dexBonus;

                Point3D origin = m_Caster.Location;
                Point3D dest   = new Point3D(p.X, p.Y, p.Z);

                int dx = dest.X - origin.X;
                int dy = dest.Y - origin.Y;

                double len = Math.Sqrt(dx * dx + dy * dy);
                if (len < 0.001) len = 1;

                double stepX = dx / len;
                double stepY = dy / len;

                m_Caster.MovingParticles(
                    new Entity(Serial.Zero, dest, map),
                    0x36D4, 7, 0, false, true, 0x48F, 0, 0x160
                );
                m_Caster.PlaySound(0x5D5);

                ArrayList targets = new ArrayList();

                for (int step = 1; step <= 6; step++)
                {
                    int tx = origin.X + (int)Math.Round(stepX * step);
                    int ty = origin.Y + (int)Math.Round(stepY * step);
                    int tz = map.GetAverageZ(tx, ty);

                    Point3D tilePos = new Point3D(tx, ty, tz);

                    if (!map.CanFit(tx, ty, tz, 1, false, false))
                        break;

                    IPooledEnumerable eable = map.GetMobilesInRange(tilePos, 0);

                    try
                    {
                        foreach (Mobile m in eable)
                        {
                            if (m == null || m.Deleted || !m.Alive || m == m_Caster)
                                continue;

                            if (!m_Caster.CanBeHarmful(m, false))
                                continue;

                            if (!targets.Contains(m))
                                targets.Add(m);
                        }
                    }
                    finally
                    {
                        eable.Free();
                    }
                }

                m_Caster.RevealingAction();

                for (int i = 0; i < targets.Count; i++)
                {
                    Mobile m = (Mobile)targets[i];

                    if (m.Deleted || !m.Alive)
                        continue;

                    m_Caster.DoHarmful(m);
                    AOS.Damage(m, m_Caster, damage,
                        elemSplit[0], elemSplit[1], elemSplit[2], elemSplit[3], elemSplit[4]);
                }

                if (m_Level >= 12 && Utility.Random(10000) < (m_Level * 50))
                {
                    m_Caster.SetAbilityCooldown("ArcaneVolley", TimeSpan.Zero);
                    m_Caster.SendMessage(0x48F, "Arcane Volley can be used again.");
                }

                new CooldownNotifyTimer(m_Caster, TimeSpan.FromMinutes(1)).Start();
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType) { }
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
                m_Player.SendMessage(0x48F, "Charged Arrows can be used again.");
            }
        }
    }
}