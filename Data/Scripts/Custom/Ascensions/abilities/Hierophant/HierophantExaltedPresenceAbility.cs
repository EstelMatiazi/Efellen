using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;

namespace Server.Custom.Ascensions
{
    public class HierophantExaltedPresenceAbility : AscensionAbility
    {
        public override AscensionType Ascension     { get { return AscensionType.Hierophant; } }
        public override int           RequiredLevel { get { return 6; } }
        public override string        Name          { get { return "ExaltedPresence"; } }
        public override string        DisplayName { get { return "Exalted Presence"; } }
        public override bool          IsPassive     { get { return false; } }
        public override TimeSpan      Cooldown      { get { return TimeSpan.FromMinutes(2); } }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
                return;

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            if (pm.Mana < 45)
            {
                pm.SendMessage("You do not have enough mana.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            if (prog == null)
                return;

            int level = prog.Level;

            pm.Mana -= 45;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x439, false, "*Exalted Presence*");
            pm.FixedParticles(0x376A, 9, 32, 5030, 0x439, 0, EffectLayer.Waist);
            pm.PlaySound(0x1F5);

            int pushRange = level / 3;
            if (pushRange < 1) pushRange = 1;

            ArrayList targets = new ArrayList();

            IPooledEnumerable eable = pm.Map.GetMobilesInRange(pm.Location, 10);

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

            for (int i = 0; i < targets.Count; i++)
            {
                Mobile m = (Mobile)targets[i];

                if (m.Deleted || !m.Alive)
                    continue;

                if (m.Combatant == pm)
                    m.Combatant = null;

                m.Warmode = false;

                PushMobileAway(pm, m, pushRange);

                if (level >= 12 && m.Karma < 0)
                    m.Paralyze(TimeSpan.FromSeconds(8));

                m.FixedParticles(0x376A, 9, 32, 5030, 0x439, 0, EffectLayer.Waist);
                m.PlaySound(0x1F5);
            }

            new CooldownNotifyTimer(pm, Cooldown).Start();
        }

        private static void PushMobileAway(PlayerMobile source, Mobile target, int tiles)
        {
            Map map = target.Map;

            if (map == null || map == Map.Internal)
                return;

            int dx = target.X - source.X;
            int dy = target.Y - source.Y;

            int stepX = (dx == 0) ? 0 : (dx > 0 ? 1 : -1);
            int stepY = (dy == 0) ? 0 : (dy > 0 ? 1 : -1);

            if (stepX == 0 && stepY == 0)
                stepY = -1;

            Point3D dest = target.Location;

            for (int step = 0; step < tiles; step++)
            {
                int nx = dest.X + stepX;
                int ny = dest.Y + stepY;
                int nz = map.GetAverageZ(nx, ny);

                if (map.CanFit(nx, ny, nz, 16, false, false))
                    dest = new Point3D(nx, ny, nz);
                else
                    break;
            }

            if (dest != target.Location)
                target.MoveToWorld(dest, map);
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
                m_Player.SendMessage(0x439, "Exalted Presence can be used again.");
            }
        }
    }
}