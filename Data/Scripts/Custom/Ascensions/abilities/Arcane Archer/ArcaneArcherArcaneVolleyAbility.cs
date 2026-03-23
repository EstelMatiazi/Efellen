using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Custom.Ascensions
{
    public class ArcaneArcherArcaneVolleyAbility : AscensionAbility
    {
        public override AscensionType Ascension     { get { return AscensionType.ArcaneArcher; } }
        public override int           RequiredLevel { get { return 11; } }
        public override string        Name          { get { return "ArcaneVolley"; } }
        public override string        DisplayName { get { return "Arcane Volley"; } }
        public override bool          IsPassive     { get { return false; } }
        public override TimeSpan      Cooldown      { get { return TimeSpan.FromSeconds(30); } }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
                return;

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            if (pm.Mana < 60)
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

            pm.SendMessage(0x48F, "Choose a target location.");
            pm.Target = new ArcaneVolleyTarget(pm, prog.Level);
        }

        public static void DoVolley(PlayerMobile pm, int level, Point3D loc, Map map)
        {
            if (map == null || map == Map.Internal)
                return;

            int elemIdx = Utility.Random(5);

            int inscrBonus = (int)(pm.Skills[SkillName.Inscribe].Value / 10);
            int damage     = Utility.RandomMinMax(55, 75) + inscrBonus;

            int phys = 0, fire = 0, cold = 0, pois = 0, nrgy = 0;

            switch (elemIdx)
            {
                case 0: phys = 100; break;
                case 1: fire = 100; break;
                case 2: cold = 100; break;
                case 3: pois = 100; break;
                case 4: nrgy = 100; break;
            }

            Effects.SendLocationParticles(
                EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                0x36D4, 10, 30, 0x48F, 0, 0x160, 0
            );
            Effects.PlaySound(loc, map, 0x5D5);

            ArrayList targets = new ArrayList();

            IPooledEnumerable eable = map.GetMobilesInRange(loc, 4);

            try
            {
                foreach (Mobile m in eable)
                {
                    if (m == null || m.Deleted || !m.Alive || m == pm)
                        continue;

                    if (!pm.CanBeHarmful(m, false))
                        continue;

                    targets.Add(m);
                }
            }
            finally
            {
                eable.Free();
            }

            pm.RevealingAction();

            for (int i = 0; i < targets.Count; i++)
            {
                Mobile m = (Mobile)targets[i];

                if (m.Deleted || !m.Alive)
                    continue;

                pm.DoHarmful(m);
                AOS.Damage(m, pm, damage, phys, fire, cold, pois, nrgy);
            }
        }

        private sealed class ArcaneVolleyTarget : Target
        {
            private readonly PlayerMobile m_Caster;
            private readonly int          m_Level;

            public ArcaneVolleyTarget(PlayerMobile pm, int level)
                : base(12, true, TargetFlags.None)
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

                m_Caster.Mana -= 60;
                m_Caster.SetAbilityCooldown("ArcaneVolley", TimeSpan.FromSeconds(30));

                m_Caster.PublicOverheadMessage(MessageType.Regular, 0x48F, false, "*Arcane Volley*");

                Point3D loc = new Point3D(p.X, p.Y, p.Z);

                DoVolley(m_Caster, m_Level, loc, map);

                if (m_Level >= 16 && Utility.Random(100) < m_Level)
                    DoVolley(m_Caster, m_Level, loc, map);

                new CooldownNotifyTimer(m_Caster, TimeSpan.FromMinutes(2)).Start();
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
                m_Player.SendMessage(0x48F, "Arcane Volley can be used again.");
            }
        }
    }
}