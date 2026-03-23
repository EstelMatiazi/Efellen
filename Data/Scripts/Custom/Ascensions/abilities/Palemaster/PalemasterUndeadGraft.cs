using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom.Ascensions
{
    public class PalemasterUndeadGraft
    {
        private static readonly SlayerEntry s_Undead   = SlayerGroup.GetEntryByName(SlayerName.Silver);
        private static readonly SlayerEntry s_Exorcism = SlayerGroup.GetEntryByName(SlayerName.Exorcism);

        private static readonly Dictionary<BaseCreature, Timer> m_ActiveHeals =
            new Dictionary<BaseCreature, Timer>();

        public static void TryTrigger(PlayerMobile pm)
        {
            if (pm == null || pm.Deleted)
                return;

            if (!pm.HasActiveAscension || pm.ActiveAscension != AscensionType.Palemaster)
                return;

            AscensionProgress prog = pm.AscensionProfile.Get(AscensionType.Palemaster);

            if (prog == null || prog.Level < 8)
                return;

            int level = prog.Level;

            if (!HasFullBoneSet(pm))
                return;

            double chance = 0.025 * level;

            if (Utility.RandomDouble() > chance)
                return;

            HealControlledUndead(pm, level);
            pm.SendMessage("Your undead mastery is unmatched!");
        }

        private static bool HasFullBoneSet(PlayerMobile pm)
        {
            return IsBone(pm.FindItemOnLayer(Layer.Arms)) &&
                   IsBone(pm.FindItemOnLayer(Layer.InnerTorso)) &&
                   IsBone(pm.FindItemOnLayer(Layer.Gloves)) &&
                   IsBone(pm.FindItemOnLayer(Layer.Helm)) &&
                   IsBone(pm.FindItemOnLayer(Layer.Pants));
        }

        private static bool IsBone(Item item)
        {
            BaseArmor armor = item as BaseArmor;

            if (armor == null)
                return false;

            return armor.MaterialType == ArmorMaterialType.Bone;
        }

        private static void HealControlledUndead(PlayerMobile pm, int level)
        {
            foreach (Mobile m in pm.GetMobilesInRange(10))
            {
                BaseCreature bc = m as BaseCreature;

                if (bc == null)
                    continue;

                if (bc.SummonMaster != pm)
                    continue;

                if (!s_Undead.Slays(bc) && !s_Exorcism.Slays(bc))
                    continue;

                DoInstantHeal(bc);

                if (level >= 17)
                {
                    StartHealOverTime(bc, level);
                }
            }
        }

        private static void DoInstantHeal(BaseCreature bc)
        {
            int heal = (int)(bc.HitsMax * 0.05);

            if (heal <= 0)
                return;

            bc.Hits += heal;

            if (bc.Hits > bc.HitsMax)
                bc.Hits = bc.HitsMax;

            bc.FixedParticles(0x376A, 10, 15, 5032, 0xB97, 0, EffectLayer.Waist);
        }

        private static void StartHealOverTime(BaseCreature bc, int level)
        {
            if (m_ActiveHeals.ContainsKey(bc))
                return;

            int duration = level / 2;

            UndeadGraftTimer timer = new UndeadGraftTimer(bc, duration);
            m_ActiveHeals[bc] = timer;
            timer.Start();
        }

        private class UndeadGraftTimer : Timer
        {
            private BaseCreature m_Target;
            private int m_TicksRemaining;

            public UndeadGraftTimer(BaseCreature target, int durationSeconds)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                m_Target = target;
                m_TicksRemaining = durationSeconds;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (m_Target == null || m_Target.Deleted || !m_Target.Alive)
                {
                    Stop();
                    Cleanup();
                    return;
                }

                if (m_TicksRemaining <= 0)
                {
                    Stop();
                    Cleanup();
                    return;
                }

                int heal = (int)(m_Target.HitsMax * 0.02);

                m_Target.Hits += heal;

                if (m_Target.Hits > m_Target.HitsMax)
                    m_Target.Hits = m_Target.HitsMax;

                m_Target.FixedParticles(0x376A, 10, 15, 5032, 0xB97, 0, EffectLayer.Waist);

                m_TicksRemaining--;
            }

            private void Cleanup()
            {
                if (m_Target != null)
                    m_ActiveHeals.Remove(m_Target);
            }
        }
    }
}