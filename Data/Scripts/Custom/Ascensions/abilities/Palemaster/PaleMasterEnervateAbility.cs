using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Network;
using Server.EffectsUtil;

namespace Server.Custom.Ascensions
{
    public class PaleMasterEnervateAbility : AscensionAbility
    {
        public override string Name        { get { return "Enervate"; } }
        public override string        DisplayName { get { return "Enervate"; } }
        public override AscensionType Ascension { get { return AscensionType.Palemaster; } }
        public override int RequiredLevel  { get { return 6; } }
        public override bool IsPassive     { get { return false; } }
        public override TimeSpan Cooldown  { get { return TimeSpan.FromSeconds(60); } }

        private static readonly SlayerEntry s_Undead   = SlayerGroup.GetEntryByName(SlayerName.Silver);
        private static readonly SlayerEntry s_Exorcism = SlayerGroup.GetEntryByName(SlayerName.Exorcism);

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
            {
                pm.SendMessage("You cannot use Enervate.");
                return;
            }

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            if (pm.Mana < 30)
            {
                pm.SendMessage("You do not have enough mana.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            DoEnervate(pm, prog.Level, true);
            new CooldownNotifyTimer(pm, Cooldown).Start();
        }

        public void ForceExecute(PlayerMobile pm, int level)
        {
            DoEnervate(pm, level, false);
        }

        private void DoEnervate(PlayerMobile pm, int level, bool applyCost)
        {
            if (applyCost)
            {
                pm.Mana -= 30;
                pm.SetAbilityCooldown(Name, Cooldown);
            }

            pm.PublicOverheadMessage(MessageType.Regular, 0x22, false, "*Enervate*");

            int radius      = 3 + (level / 5);
            int damage      = 40 + (level / 3);
            int maxLeech    = level >= 12 ? 100 : 50;
            int debuffPercent = 5 + (level / 5);

            int totalHeal = 0;
            int totalMana = 0;

            SlamVisuals.SlamVisual(pm, radius, 0x36B0, 1153);

            IPooledEnumerable eable = pm.Map.GetMobilesInRange(pm.Location, radius);

            try
            {
                foreach (Mobile m in eable)
                {
                    if (m == null || m.Deleted || !m.Alive || m == pm)
                        continue;

                    if (!pm.CanBeHarmful(m))
                        continue;

                    if (s_Undead.Slays(m) || s_Exorcism.Slays(m))
                        continue;

                    pm.DoHarmful(m);
                    AOS.Damage(m, pm, damage, 0, 0, 100, 0, 0);

                    m.Mana = (m.Mana >= damage) ? m.Mana - damage : 0;

                    totalHeal += 5;
                    totalMana += 5;

                    ApplyStatDebuff(m, debuffPercent);
                }
            }
            finally
            {
                eable.Free();
            }

            totalHeal = Math.Min(totalHeal, maxLeech);
            totalMana = Math.Min(totalMana, maxLeech);

            pm.Hits = Math.Min(pm.Hits + totalHeal, pm.HitsMax);
            pm.Mana = Math.Min(pm.Mana + totalMana, pm.ManaMax);

            pm.FixedParticles(0x374A, 10, 15, 5021, 2075, 0, EffectLayer.Waist);
            pm.PlaySound(0x1F8);
        }

        private static void ApplyStatDebuff(Mobile m, int percent)
        {
            int strLoss = Math.Max((m.RawStr * percent) / 100, 1);
            int dexLoss = Math.Max((m.RawDex * percent) / 100, 1);

            m.AddStatMod(new StatMod(StatType.Str, "EnervateStr", -strLoss, TimeSpan.FromSeconds(30)));
            m.AddStatMod(new StatMod(StatType.Dex, "EnervateDex", -dexLoss, TimeSpan.FromSeconds(30)));
        }
        private class CooldownNotifyTimer : Timer
        {
            private PlayerMobile m_Player;

            public CooldownNotifyTimer(PlayerMobile pm, TimeSpan delay)
                : base(delay)
            {
                m_Player = pm;
            }

            protected override void OnTick()
            {
                if (m_Player != null)
                    m_Player.SendMessage("You can enervate again.");
            }
        }
    }
}