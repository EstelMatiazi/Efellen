using System;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;
using Server.Spells;

namespace Server.Custom.Ascensions
{
    public class BerserkerLeapSlamAbility : AscensionAbility
    {
        public override string Name { get { return "LeapSlam"; } }
        public override string        DisplayName { get { return "Leap Slam"; } }

        public override AscensionType Ascension
        {
            get { return AscensionType.Berserker; }
        }

        public override int RequiredLevel { get { return 6; } }

        public override bool IsPassive { get { return false; } }

        public override TimeSpan Cooldown
        {
            get { return TimeSpan.FromSeconds(9); } // base
        }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
            {
                pm.SendMessage("You cannot use your Leap Slam now.");
                return;                
            }

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            int level = prog.Level;

            pm.Target = new LeapTarget(this, level);
            pm.SendMessage("Choose a location to leap to.");
        }

        private class LeapTarget : Target
        {
            private BerserkerLeapSlamAbility m_Ability;
            private int m_Level;

            public LeapTarget(BerserkerLeapSlamAbility ability, int level)
                : base(12, true, TargetFlags.None)
            {
                m_Ability = ability;
                m_Level = level;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!(from is PlayerMobile))
                    return;

                PlayerMobile pm = (PlayerMobile)from;

                IPoint3D p = targeted as IPoint3D;
                if (p == null)
                    return;

                Map map = pm.Map;
                Point3D dest = new Point3D(p);

                int maxRange = 3 + (m_Level / 4);

                if (pm.GetDistanceToSqrt(dest) < 2)
                {
                    pm.SendMessage("You must leap at least 2 tiles away.");
                    return;
                }

                if (pm.GetDistanceToSqrt(dest) > maxRange)
                {
                    pm.SendMessage("That location is too far away.");
                    return;
                }

                if (!map.CanFit(dest.X, dest.Y, dest.Z, 16, false, false))
                {
                    pm.SendMessage("You cannot land there.");
                    return;
                }

                pm.MoveToWorld(dest, map);

                PlayImpactEffect(pm, m_Level);

                DoImpactDamage(pm, dest, m_Level);

                int reduction = m_Level / 3;
                int finalCooldown = 9 - reduction;
                if (finalCooldown < 3)
                    finalCooldown = 3;

                pm.SetAbilityCooldown(m_Ability.Name, TimeSpan.FromSeconds(finalCooldown));
            }
        }

        private static void DoImpactDamage(PlayerMobile pm, Point3D center, int level)
        {
            Map map = pm.Map;

            int min = 20;
            int max = 35;

            int damage = Utility.RandomMinMax(min, max);
            damage += (pm.Str / 15);
            damage += (level / 3);

            foreach (Mobile m in pm.GetMobilesInRange(1))
            {
                if (m == pm)
                    continue;

                if (!pm.CanBeHarmful(m))
                    continue;

                pm.DoHarmful(m);

                AOS.Damage(m, pm, damage, 100, 0, 0, 0, 0);

                if (level >= 12)
                {
                    m.Freeze(TimeSpan.FromSeconds(3.0));
                }
            }
        }

        private static void PlayImpactEffect(PlayerMobile pm, int level)
        {
            pm.FixedParticles(
                0x36BD,
                10,
                30,
                5052,
                0x0F1,
                0,
                EffectLayer.Waist
            );

            pm.PlaySound(0x307);

            if (level >= 12)
            {
                pm.FixedParticles(
                    0x3709,
                    15,
                    40,
                    5052,
                    0x0F1,
                    0,
                    EffectLayer.Head
                );
            }
        }
    }
}