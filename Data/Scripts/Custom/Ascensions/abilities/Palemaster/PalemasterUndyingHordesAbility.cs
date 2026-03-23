using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;

namespace Server.Custom.Ascensions
{
    public class PalemasterUndyingHordesAbility : AscensionAbility
    {
        public override AscensionType Ascension { get { return AscensionType.Palemaster; } }
        public override int RequiredLevel       { get { return 1; } }
        public override string Name             { get { return "UndyingHordes"; } }
        public override string        DisplayName { get { return "Undying Hordes"; } }
        public override bool IsPassive          { get { return false; } }

        public override TimeSpan Cooldown
        {
            get { return TimeSpan.FromMinutes(3); }
        }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
                return;

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(AscensionType.Palemaster);
            if (prog == null)
                return;

            int level = prog.Level;

            pm.PublicOverheadMessage(MessageType.Regular, 0x48C, false, "*Undying Hordes*");
            pm.SendMessage("You call forth the undying hordes!");
            pm.SetAbilityCooldown(Name, Cooldown);

            List<BaseCreature> summons = SpawnHorde(pm, level);

            new HordeTimer(summons, TimeSpan.FromSeconds(60 + (level * 3))).Start();
            new CooldownNotifyTimer(pm, Cooldown).Start();
        }

        private static List<BaseCreature> SpawnHorde(PlayerMobile pm, int level)
        {
            List<BaseCreature> list = new List<BaseCreature>();

            if (pm.Map == null)
                return list;

            if (level <= 4)
            {
                SpawnRange(pm, list, 3, 5, typeof(PaleMasterSkeleton));
                SpawnRange(pm, list, 2, 3, typeof(PaleMasterSkeletonWarrior));
            }
            else if (level <= 8)
            {
                SpawnRange(pm, list, 3, 5, typeof(PaleMasterSkeleton));
                SpawnRange(pm, list, 2, 3, typeof(PaleMasterSkeletonWarrior));
                SpawnRange(pm, list, 1, 2, typeof(PaleMasterSkeletonKnight));
            }
            else if (level <= 12)
            {
                SpawnRange(pm, list, 3, 4, typeof(PaleMasterSkeletonWarrior));
                SpawnRange(pm, list, 2, 3, typeof(PaleMasterSkeletonKnight));
                SpawnRange(pm, list, 1, 2, typeof(PaleMasterMummy));
            }
            else if (level <= 16)
            {
                SpawnRange(pm, list, 2, 3, typeof(PaleMasterSkeletonKnight));
                SpawnRange(pm, list, 2, 3, typeof(PaleMasterMummy));
                SpawnRange(pm, list, 1, 2, typeof(PaleMasterMummyLord));
                SpawnRange(pm, list, 1, 1, typeof(PaleMasterUndeadGiant));
            }
            else if (level <= 19)
            {
                SpawnRange(pm, list, 3, 4, typeof(PaleMasterUndeadGiant));
                SpawnRange(pm, list, 2, 3, typeof(PaleMasterMummyLord));
                SpawnRange(pm, list, 1, 1, typeof(PaleMasterSkeletalDragon));
            }
            else
            {
                SpawnRange(pm, list, 4, 5, typeof(PaleMasterUndeadGiant));
                SpawnRange(pm, list, 3, 4, typeof(PaleMasterMummyLord));
                SpawnRange(pm, list, 1, 2, typeof(PaleMasterSkeletalDragon));
            }

            return list;
        }

        private static void SpawnRange(PlayerMobile pm, List<BaseCreature> list, int min, int max, Type type)
        {
            int amount = Utility.RandomMinMax(min, max);

            for (int i = 0; i < amount; i++)
            {
                BaseCreature bc = Activator.CreateInstance(type) as BaseCreature;

                if (bc == null)
                    continue;

                if (!TrySpawnNear(pm, bc))
                {
                    bc.Delete();
                    continue;
                }

                bc.Summoned     = true;
                bc.SummonMaster = pm;
                bc.IsTempEnemy  = true;
                bc.ControlSlots = 0;
                bc.Controlled   = false;
                bc.FightMode    = FightMode.Closest;
                bc.RangeHome    = 12;
                bc.Home         = pm.Location;

                Effects.SendLocationEffect(bc.Location, bc.Map, 0x3728, 15, 10, 2075, 0);

                list.Add(bc);
            }
        }

        private static bool TrySpawnNear(PlayerMobile pm, BaseCreature bc)
        {
            Map map     = pm.Map;
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

        private class HordeTimer : Timer
        {
            private List<BaseCreature> m_List;

            public HordeTimer(List<BaseCreature> list, TimeSpan duration)
                : base(duration)
            {
                m_List = list;
            }

            protected override void OnTick()
            {
                foreach (BaseCreature bc in m_List)
                {
                    if (bc != null && !bc.Deleted)
                    {
                        Effects.SendLocationEffect(bc.Location, bc.Map, 0x3728, 15, 10, 2075, 0);
                        bc.Delete();
                    }
                }

                m_List.Clear();
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
                    m_Player.SendMessage("You can call the undying hordes again.");
            }
        }
    }
}