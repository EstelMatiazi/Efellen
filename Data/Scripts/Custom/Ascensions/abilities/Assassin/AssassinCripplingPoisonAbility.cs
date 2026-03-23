using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Items;

namespace Server.Custom.Ascensions
{
    public class AssassinCripplingPoisonAbility : AscensionAbility
    {
        public override AscensionType Ascension { get { return AscensionType.Assassin; } }
        public override int RequiredLevel       { get { return 6; } }
        public override string Name             { get { return "CripplingPoison"; } }
        public override string        DisplayName { get { return "Crippling Poison"; } }
        public override bool IsPassive          { get { return false; } }
        public override TimeSpan Cooldown       { get { return TimeSpan.FromMinutes(1); } }

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

            if (pm.Stam < 40)
            {
                pm.SendMessage("You do not have enough stamina.");
                return;
            }

            pm.SendMessage("Target a creature to cripple.");
            pm.Target = new CripplingPoisonTarget(this);
        }

        private class CripplingPoisonTarget : Target
        {
            private AssassinCripplingPoisonAbility m_Ability;

            public CripplingPoisonTarget(AssassinCripplingPoisonAbility ability)
                : base(12, false, TargetFlags.Harmful)
            {
                m_Ability = ability;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                PlayerMobile pm = from as PlayerMobile;
                if (pm == null)
                    return;

                Mobile target = targeted as Mobile;
                if (target == null)
                    return;

                if (!pm.CanBeHarmful(target, true))
                    return;

                AscensionProgress prog = pm.AscensionProfile.Get(AscensionType.Assassin);
                if (prog == null)
                    return;

                m_Ability.DoThrow(pm, target, prog.Level);
            }
        }

        private void DoThrow(PlayerMobile pm, Mobile primary, int level)
        {
            pm.Mana -= 40;
            pm.Stam -= 40;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x233, false, "*Crippling Poison*");

            bool aoe     = level >= 12;
            Poison venom = aoe ? Poison.Lethal : Poison.Deadly;
            int crippleSecs = level / 2;

            pm.MovingParticles(primary, 0x1C19, 1, 0, false, true, 0, 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0);

            pm.DoHarmful(primary);
            ApplyCripple(pm, primary, venom, crippleSecs);

            if (aoe)
            {
                Map map = pm.Map;

                IPooledEnumerable eable = map.GetMobilesInRange(primary.Location, 2);

                try
                {
                    foreach (Mobile m in eable)
                    {
                        if (m == null || m.Deleted || !m.Alive || m == pm || m == primary)
                            continue;

                        if (!pm.CanBeHarmful(m, false))
                            continue;

                        pm.DoHarmful(m);
                        ApplyCripple(pm, m, venom, crippleSecs);
                    }
                }
                finally
                {
                    eable.Free();
                }

                for (int x = -2; x <= 2; x++)
                {
                    for (int y = -2; y <= 2; y++)
                    {
                        if ((x * x + y * y) > 4)
                            continue;

                        Point3D tile = new Point3D(primary.X + x, primary.Y + y, primary.Z);

                        Effects.SendLocationParticles(
                            EffectItem.Create(tile, map, EffectItem.DefaultDuration),
                            0x3729, 9, 20, 0x233, 0, 0, 0
                        );
                    }
                }
            }

            primary.PlaySound(0x474);
            new CooldownNotifyTimer(pm, Cooldown).Start();
        }

        private static void ApplyCripple(PlayerMobile attacker, Mobile target, Poison venom, int seconds)
        {
            target.ApplyPoison(attacker, venom);

            BaseCreature bc = target as BaseCreature;
            if (bc != null && seconds > 0)
                new CrippledTimer(bc, seconds).Start();
        }

        private sealed class CrippledTimer : Timer
        {
            private readonly BaseCreature m_Target;

            public CrippledTimer(BaseCreature target, int seconds)
                : base(TimeSpan.FromSeconds(seconds))
            {
                m_Target          = target;
                m_Target.CantWalk = true;
                Priority          = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (m_Target == null || m_Target.Deleted)
                    return;

                m_Target.CantWalk = false;
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

                m_Player.SendMessage(0x48C, "The can use crippling poison again.");
            }
        }
    }
}