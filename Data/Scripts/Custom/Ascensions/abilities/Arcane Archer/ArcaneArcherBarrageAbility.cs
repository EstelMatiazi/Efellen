using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;

namespace Server.Custom.Ascensions
{
    public class ArcaneArcherBarrageAbility : AscensionAbility
    {
        public override AscensionType Ascension     { get { return AscensionType.ArcaneArcher; } }
        public override int           RequiredLevel { get { return 18; } }
        public override string        Name          { get { return "Barrage"; } }
        public override string        DisplayName { get { return "Arcane Barrage"; } }
        public override bool          IsPassive     { get { return false; } }
        public override TimeSpan      Cooldown      { get { return TimeSpan.FromMinutes(3); } }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
                return;

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            if (pm.Mana < 70)
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

            Mobile target = pm.Combatant;
            if (target == null || target.Deleted || !target.Alive)
            {
                pm.SendMessage("You have no target.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            if (prog == null)
                return;

            int level = prog.Level;

            pm.Mana -= 70;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x48F, false, "*Barrage*");

            DoBarrage(pm, target, level, weapon);

            // Level 20: 0.25%/level chance to fire again
            if (level >= 20 && Utility.Random(10000) < (level * 25))
                DoBarrage(pm, target, level, weapon);

            new CooldownNotifyTimer(pm, Cooldown).Start();
        }

        public static void DoBarrage(PlayerMobile pm, Mobile target, int level, BaseWeapon weapon)
        {
            int shots = level / 4;
            if (shots < 1) shots = 1;

            // Each shot is a deferred swing via timer to avoid nesting spatial queries
            for (int i = 0; i < shots; i++)
            {
                new BarrageShotTimer(pm, target, weapon, TimeSpan.FromSeconds(i * 0.25)).Start();
            }
        }

        private sealed class BarrageShotTimer : Timer
        {
            private readonly PlayerMobile m_Caster;
            private readonly Mobile       m_Target;
            private readonly BaseWeapon   m_Weapon;

            public BarrageShotTimer(PlayerMobile pm, Mobile target, BaseWeapon weapon, TimeSpan delay)
                : base(delay)
            {
                m_Caster  = pm;
                m_Target  = target;
                m_Weapon  = weapon;
                Priority  = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (m_Caster == null || m_Caster.Deleted || !m_Caster.Alive)
                    return;

                if (m_Target == null || m_Target.Deleted || !m_Target.Alive)
                    return;

                if (!m_Caster.CanBeHarmful(m_Target, false))
                    return;

                // Fire a single shot via OnSwing — bypasses stamina cost
                // by calling OnHit directly with the weapon's normal damage flow
                m_Caster.MovingEffect(m_Target, ((BaseRanged)m_Weapon).EffectID, 18, 1, false, false);

                if (m_Weapon.CheckHit(m_Caster, m_Target))
                    m_Weapon.OnHit(m_Caster, m_Target, 1.0);
                else
                    m_Weapon.OnMiss(m_Caster, m_Target);
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
                if (m_Player == null || m_Player.Deleted) return;
                m_Player.SendMessage(0x48F, "Barrage can be used again.");
            }
        }
    }
}