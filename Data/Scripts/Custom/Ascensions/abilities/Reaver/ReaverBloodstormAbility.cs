using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;

namespace Server.Custom.Ascensions
{
    public class ReaverBloodstormAbility : AscensionAbility
    {
        public override AscensionType Ascension     { get { return AscensionType.Reaver; } }
        public override int           RequiredLevel { get { return 11; } }
        public override string        Name          { get { return "Bloodstorm"; } }
        public override string        DisplayName { get { return "Bloodstorm"; } }
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

            if (pm.Mana < 25 || pm.Stam < 25)
            {
                pm.SendMessage("You do not have enough mana or stamina.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            if (prog == null)
                return;

            Map map = pm.Map;
            if (map == null)
                return;

            int level = prog.Level;

            pm.Mana    -= 25;
            pm.Stam    -= 25;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x675, false, "*Bloodstorm*");
            pm.FixedParticles(0x376A, 10, 20, 5030, 0x675, 0, EffectLayer.Waist);
            pm.PlaySound(0x51D);

            int radius     = 2 + (level / 5);
            int strBonus   = pm.Str / 15;
            int dmgBonus   = (level / 2) + strBonus;
            int damage     = Utility.RandomMinMax(20, 32) + dmgBonus;

            ArrayList targets = new ArrayList();

            IPooledEnumerable eable = map.GetMobilesInRange(pm.Location, radius);

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

            ArrayList effectTiles = new ArrayList();

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (x * x + y * y > radius * radius)
                        continue;

                    int    nx = pm.Location.X + x;
                    int    ny = pm.Location.Y + y;
                    int    nz = map.GetAverageZ(nx, ny);

                    effectTiles.Add(new Point3D(nx, ny, nz));
                }
            }

            for (int i = 0; i < effectTiles.Count; i++)
            {
                Point3D tile = (Point3D)effectTiles[i];

                Effects.SendLocationParticles(
                    EffectItem.Create(tile, map, TimeSpan.FromSeconds(2.0)),
                    0x23B2, 10, 20, 0x675, 0, 0, 0
                );
            }

            Effects.PlaySound(pm.Location, map, 0x51D);

            pm.RevealingAction();

            for (int i = 0; i < targets.Count; i++)
            {
                Mobile m = (Mobile)targets[i];

                pm.DoHarmful(m);
                AOS.Damage(m, pm, damage, 100, 0, 0, 0, 0);
            }

            if (level >= 16 && Utility.Random(10000) < (level * 50))
            {
                pm.SetAbilityCooldown("Exsanguinate", TimeSpan.Zero);
                pm.SendMessage(0x675, "You can now use Exsanguinate again.");
            }

            new CooldownNotifyTimer(pm, Cooldown).Start();
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

                m_Player.SendMessage(0x675, "You can now use Bloodstorm again.");
            }
        }
    }
}