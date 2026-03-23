using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;

namespace Server.Custom.Ascensions
{
    public class SkaldDirgeOfTheFallenAbility : AscensionAbility
    {
        public override AscensionType Ascension     { get { return AscensionType.Skald; } }
        public override int           RequiredLevel { get { return 18; } }
        public override string        Name          { get { return "DirgeOfTheFallen"; } }
        public override bool          IsPassive     { get { return false; } }
        public override TimeSpan      Cooldown      { get { return TimeSpan.FromSeconds(300); } }

        private const int ManaCost    = 50;
        private const int MaxDuration = 90;

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
                return;

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            if (pm.Mana < ManaCost)
            {
                pm.SendMessage("You do not have enough mana.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(AscensionType.Skald);
            if (prog == null)
                return;

            int level = prog.Level;

            pm.Mana -= ManaCost;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x445, false, "*Dirge of the Fallen*");
            pm.SendMessage(0x9C2, "You call forth the heroes of legend!");

            pm.FixedParticles(0x376A, 9, 32, 5030, 0x445, 0, EffectLayer.Waist);
            pm.PlaySound(0x1F5);

            int warriorCount = 3 + (level / 4);

            List<BaseCreature> summons = SpawnHeroes(pm, warriorCount);

            new HordeTimer(summons, TimeSpan.FromSeconds(MaxDuration)).Start();
            new CooldownNotifyTimer(pm, Cooldown).Start();

            if (level >= 20 && Utility.Random(100) < level)
            {
                AscensionProgress thunderProg = pm.AscensionProfile.Get(AscensionType.Skald);

                new SkaldSongOfThunderAbility().DoSongOfThunderDirect(pm, thunderProg.Level);
            }
        }

        private static List<BaseCreature> SpawnHeroes(PlayerMobile pm, int count)
        {
            List<BaseCreature> list = new List<BaseCreature>();

            Map     map = pm.Map;
            Point3D loc = pm.Location;

            for (int i = 0; i < count; i++)
            {
                SkaldAncientHero hero = new SkaldAncientHero();
                hero.MoveToWorld(new Point3D(0, 0, 0), Map.Internal);

                hero.AddEquipment();

                Point3D spawn = Point3D.Zero;
                bool    found = false;

                for (int attempt = 0; attempt < 10; attempt++)
                {
                    Point3D candidate = new Point3D(
                        loc.X + Utility.RandomMinMax(-3, 3),
                        loc.Y + Utility.RandomMinMax(-3, 3),
                        loc.Z
                    );

                    if (map.CanFit(candidate, 16, false, false))
                    {
                        spawn = candidate;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    hero.Delete();
                    continue;
                }

                hero.MoveToWorld(spawn, map);

                hero.Summoned     = true;
                hero.SummonMaster = pm;
                hero.ControlSlots = 0;
                hero.Controlled   = false;
                hero.FightMode    = FightMode.Closest;
                hero.RangeHome    = 12;
                hero.Home         = loc;

                Effects.SendLocationParticles(
                    EffectItem.Create(hero.Location, hero.Map, EffectItem.DefaultDuration),
                    0x376A, 9, 32, 0x445, 0, 5030, 0
                );
                hero.PlaySound(0x1F5);

                list.Add(hero);
            }

            return list;
        }

        private static bool TrySpawnNear(PlayerMobile pm, BaseCreature bc)
        {
            Map     map = pm.Map;
            Point3D loc = pm.Location;

            for (int i = 0; i < 10; i++)
            {
                Point3D spawn = new Point3D(
                    loc.X + Utility.RandomMinMax(-3, 3),
                    loc.Y + Utility.RandomMinMax(-3, 3),
                    loc.Z
                );

                if (map.CanFit(spawn, 16, false, false))
                {
                    bc.MoveToWorld(spawn, map);
                    return true;
                }
            }

            return false;
        }

        private sealed class HordeTimer : Timer
        {
            private readonly List<BaseCreature> m_List;

            public HordeTimer(List<BaseCreature> list, TimeSpan duration)
                : base(duration)
            {
                m_List   = list;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                for (int i = 0; i < m_List.Count; i++)
                {
                    BaseCreature bc = m_List[i];

                    if (bc == null || bc.Deleted)
                        continue;

                    Effects.SendLocationParticles(
                        EffectItem.Create(bc.Location, bc.Map, EffectItem.DefaultDuration),
                        0x376A, 9, 32, 0x445, 0, 5030, 0
                    );
                    bc.PlaySound(0x1F5);

                    bc.Delete();
                }

                m_List.Clear();
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

                m_Player.SendMessage(0x445, "You can call the heroes of legend again.");
            }
        }
    }
}