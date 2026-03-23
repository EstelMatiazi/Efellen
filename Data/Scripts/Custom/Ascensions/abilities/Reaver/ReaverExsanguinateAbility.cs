using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.Spells;

namespace Server.Custom.Ascensions
{
    public class ReaverExsanguinateAbility : AscensionAbility
    {
        public override AscensionType Ascension     { get { return AscensionType.Reaver; } }
        public override int           RequiredLevel { get { return 6; } }
        public override string        Name          { get { return "Exsanguinate"; } }
        public override string        DisplayName { get { return "Exsanguinate"; } }
        public override bool          IsPassive     { get { return false; } }
        public override TimeSpan      Cooldown      { get { return TimeSpan.FromMinutes(1); } }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
                return;

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            if (pm.Mana < 20 || pm.Stam < 20)
            {
                pm.SendMessage("You do not have enough mana or stamina.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            if (prog == null)
                return;

            BaseWeapon weapon = pm.Weapon as BaseWeapon;
            if (weapon == null)
            {
                pm.SendMessage("You must be wielding a weapon.");
                return;
            }

            Map map = pm.Map;
            if (map == null)
                return;

            pm.Mana    -= 20;
            pm.Stam -= 20;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x675, false, "*Exsanguinate*");
            pm.FixedParticles(0x5492, 10, 15, 0, 0x675, 0, EffectLayer.Waist);
            pm.PlaySound(0x2A1);

            int level = prog.Level;

            ArrayList list = new ArrayList();

            foreach (Mobile m in pm.GetMobilesInRange(1))
                list.Add(m);

            ArrayList targets = new ArrayList();

            for (int i = 0; i < list.Count; i++)
            {
                Mobile m = (Mobile)list[i];

                if (m == pm)
                    continue;

                if (m == null || m.Deleted || m.Map != map || !m.Alive)
                    continue;

                if (!pm.CanSee(m) || !pm.CanBeHarmful(m, false))
                    continue;

                if (!pm.InRange(m, weapon.MaxRange))
                    continue;

                if (!SpellHelper.ValidIndirectTarget(pm, m))
                    continue;

                if (pm.InLOS(m))
                    targets.Add(m);
            }

            int strBonus    = pm.Str / 15;
            int hitDamage   = Utility.RandomMinMax(18, 26) + strBonus;
            int bleedDuration = 13 + level;

            pm.RevealingAction();

            for (int i = 0; i < targets.Count; i++)
            {
                Mobile m = (Mobile)targets[i];

                pm.DoHarmful(m);

                AOS.Damage(m, pm, hitDamage, 100, 0, 0, 0, 0);

                if (level >= 12)
                {
                    pm.Hits += 9;
                    if (pm.Hits > pm.HitsMax)
                        pm.Hits = pm.HitsMax;
                }

                if (!IsExsanguinateBleeding(m))
                    new ExsanguinateBleedTimer(m, pm, bleedDuration).Start();
            }

            new CooldownNotifyTimer(pm, Cooldown).Start();
        }

        private static Hashtable m_BleedTable = new Hashtable();

        public static bool IsExsanguinateBleeding(Mobile m)
        {
            return m_BleedTable.ContainsKey(m);
        }

        public static void RegisterBleed(Mobile m, ExsanguinateBleedTimer timer)
        {
            m_BleedTable[m] = timer;
        }

        public static void UnregisterBleed(Mobile m)
        {
            m_BleedTable.Remove(m);
        }

        public sealed class ExsanguinateBleedTimer : Timer
        {
            private readonly Mobile       m_Target;
            private readonly PlayerMobile m_Caster;
            private          int          m_TicksRemaining;

            public ExsanguinateBleedTimer(Mobile target, PlayerMobile caster, int durationSeconds)
                : base(TimeSpan.FromSeconds(3.0), TimeSpan.FromSeconds(3.0))
            {
                m_Target         = target;
                m_Caster         = caster;
                m_TicksRemaining = durationSeconds / 3;
                Priority         = TimerPriority.OneSecond;

                ReaverExsanguinateAbility.RegisterBleed(target, this);
            }

            protected override void OnTick()
            {
                if (m_Target == null || m_Target.Deleted || !m_Target.Alive)
                {
                    ReaverExsanguinateAbility.UnregisterBleed(m_Target);
                    Stop();
                    return;
                }

                m_TicksRemaining--;

                int bleedDamage = Utility.RandomMinMax(26, 32);

                m_Target.FixedParticles(0x377A, 10, 15, 5030, 0x675, 0, EffectLayer.Waist);
                m_Target.PlaySound(0x1DD);

                AOS.Damage(m_Target, m_Caster, bleedDamage, 100, 0, 0, 0, 0);

                if (m_TicksRemaining <= 0)
                {
                    ReaverExsanguinateAbility.UnregisterBleed(m_Target);
                    Stop();
                }
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

                m_Player.SendMessage(0x675, "You can now use Exsanguinate again.");
            }
        }
    }
}