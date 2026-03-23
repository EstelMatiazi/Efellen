using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Spells;
using Server.Items;
using Server.EffectsUtil;

namespace Server.Custom.Ascensions
{
    public class PalemasterCircleOfDeathAbility : AscensionAbility
    {
        public override AscensionType Ascension { get { return AscensionType.Palemaster; } }
        public override int RequiredLevel { get { return 11; } }
        public override string Name { get { return "CircleofDeath"; } }
        public override string        DisplayName { get { return "Circle of Death"; } }
        public override bool IsPassive { get { return false; } }

        public override TimeSpan Cooldown
        {
            get { return TimeSpan.FromMinutes(2); }
        }

        private static readonly SlayerEntry s_Undead   = SlayerGroup.GetEntryByName(SlayerName.Silver);
        private static readonly SlayerEntry s_Exorcism = SlayerGroup.GetEntryByName(SlayerName.Exorcism);

        private static readonly int[] s_BoneGraphics = new int[]
        {
            0x1B09, 0x1B0A, 0x1B0B, 0x1B0C,
            0x1B0D, 0x1B0E, 0x1B0F, 0x1B10
        };

        private const int BoneHue = 0xB97;

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
                return;

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            if (pm.Mana < 60)
            {
                pm.SendMessage("You do not have enough mana.");
                return;
            }

            pm.Target = new CircleTarget(this);
            pm.SendMessage("Target a location for the Circle of Death.");
        }

        private class CircleTarget : Target
        {
            private PalemasterCircleOfDeathAbility m_Ability;

            public CircleTarget(PalemasterCircleOfDeathAbility ability)
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

                AscensionProgress prog = pm.AscensionProfile.Get(AscensionType.Palemaster);
                if (prog == null)
                    return;

                Point3D loc = new Point3D(p.X, p.Y, p.Z);
                m_Ability.BeginCast(pm, loc, prog.Level);
            }
        }

        private void BeginCast(PlayerMobile pm, Point3D center, int level)
        {
            pm.Mana -= 60;
            pm.SetAbilityCooldown(Name, Cooldown);
            pm.PublicOverheadMessage(MessageType.Regular, 0x48C, false, "*Circle of Death*");

            int range        = 2 + (level / 5);
            int duration     = 3 + (level / 4);
            int bonusDamage  = (level / 2) + (pm.Int / 20);
      
            List<Point3D> ring = GetRingTiles(center, range);

            List<Item> bones = SpawnBones(pm.Map, ring);

            new DamageTimer(pm, center, range, duration, bonusDamage, bones, level).Start();
            new CooldownNotifyTimer(pm, Cooldown).Start();
        }

        private static List<Point3D> GetRingTiles(Point3D center, int range)
        {
            List<Point3D> tiles = new List<Point3D>();

            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    int dist = (int)Math.Round(Math.Sqrt(x * x + y * y));
                    if (dist == range)
                        tiles.Add(new Point3D(center.X + x, center.Y + y, center.Z));
                }
            }
            return tiles;
        }

        private static List<Item> SpawnBones(Map map, List<Point3D> tiles)
        {
            List<Item> bones = new List<Item>();

            foreach (Point3D tile in tiles)
            {
                int graphic = s_BoneGraphics[Utility.Random(s_BoneGraphics.Length)];
                Item bone = new Static(graphic);
                bone.Hue = BoneHue;
                bone.MoveToWorld(tile, map);
                bones.Add(bone);
            }
            return bones;
        }

        private static void RemoveBones(List<Item> bones)
        {
            new BoneCleanupTimer(bones).Start();
        }

        private class BoneCleanupTimer : Timer
        {
            private List<Item> m_Bones;
            private int m_Index;

            public BoneCleanupTimer(List<Item> bones)
                : base(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100))
            {
                m_Bones = bones;
                m_Index = 0;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                int deletePerTick = 5;
                int count = 0;

                while (m_Index < m_Bones.Count && count < deletePerTick)
                {
                    Item bone = m_Bones[m_Index];
                    if (bone != null && !bone.Deleted)
                        bone.Delete();

                    m_Index++;
                    count++;
                }

                if (m_Index >= m_Bones.Count)
                    Stop();
            }
        }

        private class DamageTimer : Timer
        {
            private PlayerMobile   m_Caster;
            private int            m_TicksRemaining;
            private int            m_BonusDamage;
            private List<Item>     m_Bones;
            private List<Mobile>   m_Targets;
            private int            m_CasterLevel;

            public DamageTimer(PlayerMobile caster, Point3D center, int range, int duration, int bonusDamage, List<Item> bones, int level)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                m_Caster         = caster;
                m_TicksRemaining = duration;
                m_BonusDamage    = bonusDamage;
                m_Bones          = bones;
                m_Targets        = new List<Mobile>();
                m_CasterLevel    = level;

                Priority = TimerPriority.TwoFiftyMS;

                // SNAPSHOT TARGETS
                IPooledEnumerable eable = caster.Map.GetMobilesInRange(center, range);

                try
                {
                    foreach (Mobile m in eable)
                    {
                        if (m == null || m.Deleted || !m.Alive || m == caster)
                            continue;

                        if (!caster.CanBeHarmful(m))
                            continue;

                        if (s_Undead.Slays(m) || s_Exorcism.Slays(m))
                            continue;

                        m_Targets.Add(m);
                    }
                }
                finally
                {
                    eable.Free();
                }
            }

            protected override void OnTick()
            {
                if (m_Caster == null || m_Caster.Deleted)
                {
                    RemoveBones(m_Bones);
                    Stop();
                    return;
                }

                for (int i = 0; i < m_Targets.Count; i++)
                {
                    Mobile m = m_Targets[i];

                    if (m == null || m.Deleted || !m.Alive)
                        continue;

                    int damage = Utility.RandomMinMax(12, 18) + m_BonusDamage;

                    m_Caster.DoHarmful(m);
                    AOS.Damage(m, m_Caster, damage, 0, 0, 100, 0, 0);

                    if(m_CasterLevel>=16)
                    {
                        m.Stam -= damage/2;
                    }

                    m.FixedParticles(0x374A, 10, 15, 5021, 1153, 0, EffectLayer.Waist);
                }

                m_TicksRemaining--;

                if (m_TicksRemaining <= 0)
                {
                    RemoveBones(m_Bones);
                    Stop();
                }
            }
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
                    m_Player.SendMessage("You may call forth another Circle of Death.");
            }
        }
    }
}