using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;

namespace Server.Custom.Ascensions
{
    public class KensaiTempestAbility : AscensionAbility
    {
        public override AscensionType Ascension     { get { return AscensionType.Kensai; } }
        public override int           RequiredLevel { get { return 18; } }
        public override string        Name          { get { return "Tempest"; } }
        public override string        DisplayName { get { return "Tempest"; } }
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

            if (pm.Stam < 75)
            {
                pm.SendMessage("You do not have enough stamina.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            if (prog == null)
                return;

            pm.Stam -= 75;
            pm.SetAbilityCooldown(Name, Cooldown);

            DoTempest(pm, prog.Level, false);

            new CooldownNotifyTimer(pm, Cooldown).Start();
        }

        public static void DoTempest(PlayerMobile pm, int level, bool freeRetrigger)
        {
            if (pm == null || pm.Deleted || !pm.Alive)
                return;

            Map map = pm.Map;

            if (map == null || map == Map.Internal)
                return;

            pm.PublicOverheadMessage(MessageType.Regular, 0x448, false, "*Tempest*");

            pm.FixedParticles(0x5492, 10, 20, 0, 0x448, 0, EffectLayer.Waist);
            pm.PlaySound(0x2A1);

            int dexBonus = pm.Dex / 15;
            int damage   = Utility.RandomMinMax(70, 85) + dexBonus;

            ArrayList targets = new ArrayList();

            IPooledEnumerable eable = map.GetMobilesInRange(pm.Location, 6);

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

            bool retriggered = false;

            for (int i = 0; i < targets.Count; i++)
            {
                Mobile m = (Mobile)targets[i];

                if (m.Deleted || !m.Alive)
                    continue;

                bool wasFullHealth = (m.Hits >= m.HitsMax);

                pm.DoHarmful(m);

                m.FixedParticles(0x5492, 10, 15, 0, 0x448, 0, EffectLayer.Waist);
                AOS.Damage(m, pm, damage, 100, 0, 0, 0, 0);

                if (!freeRetrigger && !retriggered && level >= 20 && !m.Alive)
                {
                    if (Utility.Random(10000) < (level * 50))
                    {
                        retriggered = true;
                        new TempestRetriggerTimer(pm, level).Start();
                    }
                }
            }
        }

        private sealed class TempestRetriggerTimer : Timer
        {
            private readonly PlayerMobile m_Player;
            private readonly int          m_Level;

            public TempestRetriggerTimer(PlayerMobile pm, int level)
                : base(TimeSpan.Zero)
            {
                m_Player = pm;
                m_Level  = level;
                Priority = TimerPriority.TwoFiftyMS ;
            }

            protected override void OnTick()
            {
                if (m_Player == null || m_Player.Deleted || !m_Player.Alive)
                    return;

                DoTempest(m_Player, m_Level, true);
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

                m_Player.SendMessage(0x448, "Tempest can be used again.");
            }
        }
    }
}