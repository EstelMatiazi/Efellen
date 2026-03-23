using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Items;

namespace Server.Custom.Ascensions
{
    public class AssassinNoxiousCloudAbility : AscensionAbility
    {
        public override AscensionType Ascension { get { return AscensionType.Assassin; } }
        public override int RequiredLevel       { get { return 1; } }
        public override string Name             { get { return "NoxiousCloud"; } }
        public override string        DisplayName { get { return "Noxious Cloud"; } }
        public override bool IsPassive          { get { return false; } }
        public override TimeSpan Cooldown       { get { return TimeSpan.FromMinutes(1); } }

        private static readonly int[] CloudGraphics = new int[]
        {
            0x3729, 0x372B, 0x372C, 0x372D,
            0x372E, 0x372F, 0x3730, 0x3731,
            0x3733, 0x3734
        };

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
                return;

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

            if (pm.Stam < 30)
            {
                pm.SendMessage("You do not have enough stamina.");
                return;
            }

            pm.SendMessage("Target a location for the Noxious Cloud.");
            pm.Target = new NoxiousCloudTarget(this);
        }

        private class NoxiousCloudTarget : Target
        {
            private AssassinNoxiousCloudAbility m_Ability;

            public NoxiousCloudTarget(AssassinNoxiousCloudAbility ability)
                : base(12, true, TargetFlags.None)
            {
                m_Ability = ability;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                PlayerMobile pm = from as PlayerMobile;
                if (pm == null)
                    return;

                IPoint3D p = targeted as IPoint3D;
                if (p == null)
                    return;

                AscensionProgress prog = pm.AscensionProfile.Get(AscensionType.Assassin);
                if (prog == null)
                    return;

                Point3D loc = new Point3D(p.X, p.Y, p.Z);
                m_Ability.BeginCast(pm, loc, prog.Level);
            }
        }

        private void BeginCast(PlayerMobile pm, Point3D center, int level)
        {
            pm.Mana -= 30;
            pm.Stam -= 30;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x48C, false, "*Noxious Cloud*");

            Poison cloudPoison;
            if      (level >= 15) cloudPoison = Poison.Lethal;
            else if (level >= 10) cloudPoison = Poison.Deadly;
            else                  cloudPoison = Poison.Greater;

            bool applyResistDebuff = level >= 20;

            int range = 1 + (level / 6);
            Map map   = pm.Map;

            List<Point3D> areaTiles = GetAreaTiles(center, range);

            for (int i = 0; i < areaTiles.Count; i++)
            {
                Point3D tile   = areaTiles[i];
                int     gfx    = CloudGraphics[Utility.Random(CloudGraphics.Length)];
                int     hue    = 0x233;
                Effects.SendLocationParticles(
                    EffectItem.Create(tile, map, EffectItem.DefaultDuration),
                    gfx, 9, 20, hue, 0, 0, 0
                );
            }

            pm.PlaySound(0x22F);

            IPooledEnumerable eable = map.GetMobilesInRange(center, range);

            try
            {
                foreach (Mobile m in eable)
                {
                    if (m == null || m.Deleted || !m.Alive || m == pm)
                        continue;

                    if (!pm.CanBeHarmful(m, false))
                        continue;

                    pm.DoHarmful(m);
                    m.ApplyPoison(pm, cloudPoison);

                    if (applyResistDebuff)
                        new PoisonResistDebuffTimer(m, 20, TimeSpan.FromSeconds(30)).Start();
                }
            }
            finally
            {
                eable.Free();
            }

            new CooldownNotifyTimer(pm, Cooldown).Start();
        }

        private static List<Point3D> GetAreaTiles(Point3D center, int range)
        {
            List<Point3D> tiles = new List<Point3D>((int)(Math.PI * range * range) + 1);

            int rangeSq = range * range;

            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    if ((x * x + y * y) <= rangeSq)
                        tiles.Add(new Point3D(center.X + x, center.Y + y, center.Z));
                }
            }
            return tiles;
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

                m_Player.SendMessage(0x48C, "The vapors are ready to be called again.");
            }
        }

        private sealed class PoisonResistDebuffTimer : Timer
        {
            private readonly Mobile m_Target;
            private readonly int    m_Amount;

            public PoisonResistDebuffTimer(Mobile target, int amount, TimeSpan duration)
                : base(duration)
            {
                m_Target = target;
                m_Amount = amount;
                Priority = TimerPriority.OneSecond;

                m_Target.AddResistanceMod(new ResistanceMod(ResistanceType.Poison, -m_Amount));
            }

            protected override void OnTick()
            {
                if (m_Target == null || m_Target.Deleted)
                    return;

                m_Target.RemoveResistanceMod(new ResistanceMod(ResistanceType.Poison, -m_Amount));
            }
        }
    }
}