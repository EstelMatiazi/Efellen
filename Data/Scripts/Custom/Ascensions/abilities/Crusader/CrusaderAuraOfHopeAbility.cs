using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Network;
using Server.EffectsUtil;

namespace Server.Custom.Ascensions
{
    public class CrusaderAuraOfHopeAbility : AscensionAbility
    {
        public override string Name             { get { return "AuraOfHope"; } }
        public override string        DisplayName { get { return "Aura Of Hope"; } }
        public override AscensionType Ascension { get { return AscensionType.Crusader; } }
        public override int RequiredLevel       { get { return 11; } }
        public override bool IsPassive          { get { return false; } }
        public override TimeSpan Cooldown       { get { return TimeSpan.FromSeconds(180); } }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
            {
                pm.SendMessage("You cannot use Aura of Hope.");
                return;
            }

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

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            DoAuraOfHope(pm, prog.Level);
        }

        private void DoAuraOfHope(PlayerMobile pm, int level)
        {
            pm.Mana -= 40;
            pm.Stam -= 40;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x22, false, "*Aura of Hope*");

            List<Mobile> targets = new List<Mobile>();
            targets.Add(pm);

            IPooledEnumerable eable = pm.Map.GetMobilesInRange(pm.Location, 4);
            try
            {
                foreach (Mobile m in eable)
                {
                    if (m == null || m.Deleted || !m.Alive || m == pm)
                        continue;

                    if (m.Karma <= 0)
                        continue;

                    if (pm.CanBeHarmful(m) && m.Combatant == pm)
                        continue;

                    targets.Add(m);
                }
            }
            finally
            {
                eable.Free();
            }

            pm.FixedParticles(0x374A, 10, 15, 5021, 0x498, 0, EffectLayer.Waist);
            pm.PlaySound(0x1F2);

            new AuraOfHopeTimer(pm, targets, level).Start();
        }

        private sealed class AuraOfHopeTimer : Timer
        {
            private static readonly TimeSpan TickInterval  = TimeSpan.FromSeconds(4);
            private static readonly TimeSpan ExpiryDelay   = TimeSpan.FromSeconds(30);
            private const int TotalTicks = 7;

            private readonly PlayerMobile  m_Caster;
            private readonly List<Mobile>  m_Targets;
            private readonly int           m_Level;
            private readonly int           m_HpRegen;
            private readonly int           m_ManaRegen;
            private readonly int           m_StamRegen;
            private readonly bool          m_FullHealOnExpiry;
            private int                    m_TicksRemaining;

            public AuraOfHopeTimer(PlayerMobile caster, List<Mobile> targets, int level)
                : base(TickInterval, TickInterval)
            {
                m_Caster           = caster;
                m_Targets          = targets;
                m_Level            = level;
                m_HpRegen          = 12 + (level / 2);
                m_ManaRegen        = 6  + (level / 2);
                m_StamRegen        = 6  + (level / 2);
                m_FullHealOnExpiry = level >= 16;
                m_TicksRemaining   = TotalTicks;
                Priority           = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                for (int i = 0; i < m_Targets.Count; i++)
                {
                    Mobile m = m_Targets[i];

                    if (m == null || m.Deleted || !m.Alive)
                        continue;

                    m.Hits = Math.Min(m.HitsMax, m.Hits + m_HpRegen);
                    m.Mana = Math.Min(m.ManaMax, m.Mana + m_ManaRegen);
                    m.Stam = Math.Min(m.StamMax, m.Stam + m_StamRegen);

                    m.FixedParticles(0x374A, 10, 15, 5021, 0x498, 0, EffectLayer.Waist);
                }

                m_TicksRemaining--;

                if (m_TicksRemaining <= 0)
                {
                    Stop();

                    if (m_FullHealOnExpiry)
                    {
                        for (int i = 0; i < m_Targets.Count; i++)
                        {
                            Mobile m = m_Targets[i];

                            if (m == null || m.Deleted || !m.Alive)
                                continue;

                            m.Hits = m.HitsMax;

                            m.FixedParticles(0x374A, 20, 20, 5021, 0x498, 0, EffectLayer.Waist);
                            m.PlaySound(0x1F2);
                        }
                    }
                }
            }
        }
    }
}