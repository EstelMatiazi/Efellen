using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Items;

namespace Server.Custom.Ascensions
{
    public class AssassinCleansingAnnihilationAbility : AscensionAbility
    {
        public override AscensionType Ascension { get { return AscensionType.Assassin; } }
        public override int RequiredLevel       { get { return 18; } }
        public override string Name             { get { return "CleansingAnnihilation"; } }
        public override string        DisplayName { get { return "Cleansing Annihilation"; } }
        public override bool IsPassive          { get { return false; } }
        public override TimeSpan Cooldown       { get { return TimeSpan.FromMinutes(4); } }

        private const int  LethalMin    = 20;
        private const int  LethalMax    = 50;
        private const double LethalScalar = 0.35;
        private const int  LethalTicks  = 20;

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

            if (pm.Stam < 50)
            {
                pm.SendMessage("You do not have enough stamina.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            if (prog == null)
                return;

            pm.SendMessage("Target a creature for Cleansing Annihilation.");
            pm.Target = new CleansingTarget(this, prog.Level);
        }

        private class CleansingTarget : Target
        {
            private AssassinCleansingAnnihilationAbility m_Ability;
            private int m_Level;

            public CleansingTarget(AssassinCleansingAnnihilationAbility ability, int level)
                : base(12, false, TargetFlags.Harmful)
            {
                m_Ability = ability;
                m_Level   = level;
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

                m_Ability.DoCleansing(pm, target, m_Level);
            }
        }

        private void DoCleansing(PlayerMobile pm, Mobile primary, int level)
        {
            pm.Mana -= 50;
            pm.Stam -= 50;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x233, false, "*Cleansing Annihilation*");

            pm.DoHarmful(primary);

            primary.ApplyPoison(pm, Poison.Lethal);

            int burstDamage = ComputeBurstDamage(primary);

            primary.FixedParticles(0x374A, 10, 15, 5021, 0x233, 0, EffectLayer.Waist);
            primary.PlaySound(0x474);

            AOS.Damage(primary, pm, burstDamage, 0, 0, 0, 100, 0);

            if (primary.Poisoned)
                primary.CurePoison(pm);

            if (primary.Alive)
            {
                Map map = pm.Map;

                IPooledEnumerable eable = map.GetMobilesInRange(primary.Location, 3);

                try
                {
                    foreach (Mobile m in eable)
                    {
                        if (m == null || m.Deleted || !m.Alive || m == pm || m == primary)
                            continue;

                        if (!pm.CanBeHarmful(m, false))
                            continue;

                        pm.DoHarmful(m);
                        m.ApplyPoison(pm, Poison.Lethal);
                    }
                }
                finally
                {
                    eable.Free();
                }

                for (int x = -3; x <= 3; x++)
                {
                    for (int y = -3; y <= 3; y++)
                    {
                        if ((x * x + y * y) > 9)
                            continue;

                        Point3D tile = new Point3D(primary.X + x, primary.Y + y, primary.Z);

                        Effects.SendLocationParticles(
                            EffectItem.Create(tile, map, EffectItem.DefaultDuration),
                            0x3729, 9, 20, 0x233, 0, 0, 0
                        );
                    }
                }
            }

            if (level >= 20 && Utility.Random(100) < level)
                pm.SetAbilityCooldown("ToxicSurge", TimeSpan.Zero);

            new CooldownNotifyTimer(pm, Cooldown).Start();
        }

        private static int ComputeBurstDamage(Mobile target)
        {
            int perTick = 1 + (int)(target.Hits * LethalScalar);

            if (perTick < LethalMin) perTick = LethalMin;
            if (perTick > LethalMax) perTick = LethalMax;

            return perTick * LethalTicks;
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

                m_Player.SendMessage(0x233, "You can use Cleansing Annihilation again.");
            }
        }
    }
}