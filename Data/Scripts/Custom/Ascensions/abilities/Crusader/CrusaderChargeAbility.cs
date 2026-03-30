using System;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;
using Server.Spells;

namespace Server.Custom.Ascensions
{
    public class CrusaderChargeAbility : AscensionAbility
    {
        public override string Name { get { return "Charge"; } }
        public override string        DisplayName { get { return "Charge"; } }

        public override AscensionType Ascension
        {
            get { return AscensionType.Crusader; }
        }

        public override int RequiredLevel { get { return 6; } }

        public override bool IsPassive { get { return false; } }

        public override TimeSpan Cooldown
        {
            get { return TimeSpan.FromSeconds(9); }
        }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
            {
                pm.SendMessage("Você não pode usar seu Charge agora.");
                return;
            }

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("Esta habilidade está em cooldown.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            int level = prog.Level;

            pm.Target = new ChargeTarget(this, level);
            pm.SendMessage("Escolha um local para avançar.");
        }

        private class ChargeTarget : Target
        {
            private CrusaderChargeAbility m_Ability;
            private int m_Level;

            public ChargeTarget(CrusaderChargeAbility ability, int level)
                : base(12, true, TargetFlags.None)
            {
                m_Ability = ability;
                m_Level   = level;
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

                int maxRange = 3 + (m_Level / 5);

                if ( Server.Misc.WeightOverloading.IsOverloaded( pm ) )
			    {
			    	pm.SendLocalizedMessage( 502359, "", 0x22 ); // Thou art too encumbered to move.
			    	return;
			    }
                else if ( map == null || !map.CanSpawnMobile( p.X, p.Y, p.Z ) )
			    {
			    	pm.SendLocalizedMessage( 501942 ); // That location is blocked.
                    return;
			    }
			    else if ( SpellHelper.CheckMulti( new Point3D( p ), map ) )
			    {
			    	pm.SendLocalizedMessage( 501942 ); // That location is blocked.
                    return;
			    }

                if (pm.GetDistanceToSqrt(dest) < 2)
                {
                    pm.SendMessage("Você deve avançar pelo menos 2 tiles.");
                    return;
                }

                if (pm.GetDistanceToSqrt(dest) > maxRange)
                {
                    pm.SendMessage("Esse local está muito longe.");
                    return;
                }

                if (!map.CanFit(dest.X, dest.Y, dest.Z, 16, false, false))
                {
                    pm.SendMessage("Você não pode avançar para lá.");
                    return;
                }

                pm.MoveToWorld(dest, map);

                PlayImpactEffect(pm, m_Level);
                DoImpactEffect(pm, dest, m_Level);

                int reduction     = m_Level / 3;
                int finalCooldown = 9 - reduction;
                if (finalCooldown < 3)
                    finalCooldown = 3;

                pm.SetAbilityCooldown(m_Ability.Name, TimeSpan.FromSeconds(finalCooldown));
            }
        }

        private static void DoImpactEffect(PlayerMobile pm, Point3D center, int level)
        {
            Map map = pm.Map;
            if (map == null)
                return;

            bool doDamage      = level >= 12;
            int paralyzeChance = level;
            int strBonus       = pm.Str / 15;

            int damageBase = strBonus + (level / 3); 

            IPooledEnumerable eable = map.GetMobilesInRange(center, 1);

            try
            {
                foreach (Mobile m in eable)
                {
                    if (m == null || m.Deleted || !m.Alive || m == pm)
                        continue;

                    if (!pm.CanBeHarmful(m, false))
                        continue;

                    if (Utility.Random(100) < paralyzeChance)
                        m.Paralyze(TimeSpan.FromSeconds(3));

                    if (doDamage)
                    {
                        pm.DoHarmful(m);
                        int dmg = Utility.RandomMinMax(30, 45) + damageBase;
                        AOS.Damage(m, pm, dmg, 100, 0, 0, 0, 0);
                    }
                }
            }
            finally
            {
                eable.Free();
            }
        }

        private static void PlayImpactEffect(PlayerMobile pm, int level)
        {
            pm.FixedParticles(
                0x36BD,
                10,
                30,
                5052,
                0x498,
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
                    0x498,
                    0,
                    EffectLayer.Head
                );
            }
        }
    }
}