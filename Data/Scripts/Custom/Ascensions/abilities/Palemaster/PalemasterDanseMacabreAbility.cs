using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Items;

namespace Server.Custom.Ascensions
{
    public class PalemasterDanseMacabreAbility : AscensionAbility
    {
        public override AscensionType Ascension { get { return AscensionType.Palemaster; } }
        public override int RequiredLevel       { get { return 18; } }
        public override string Name             { get { return "DanseMacabre"; } }
        public override string        DisplayName { get { return "Danse Macabre"; } }
        public override bool IsPassive          { get { return false; } }

        public override TimeSpan Cooldown
        {
            get { return TimeSpan.FromMinutes(4); }
        }

        private static readonly SlayerEntry s_Undead   = SlayerGroup.GetEntryByName(SlayerName.Silver);
        private static readonly SlayerEntry s_Exorcism = SlayerGroup.GetEntryByName(SlayerName.Exorcism);

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
                return;

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            if (pm.Mana < 80)
            {
                pm.SendMessage("You do not have enough mana.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(AscensionType.Palemaster);
            if (prog == null)
                return;

            pm.Mana -= 80;
            pm.SetAbilityCooldown(Name, Cooldown);
            pm.PublicOverheadMessage(MessageType.Regular, 0xB97, false, "*Danse Macabre*");

            Effects.SendLocationEffect(pm.Location, pm.Map, 0x3728, 30, 15, 0xB97, 0);

            new DanseTimer(pm, prog.Level).Start();
            new CooldownNotifyTimer(pm, Cooldown).Start();
        }

        private class DanseTimer : Timer
        {
            private PlayerMobile m_Caster;
            private int m_Level;
            private int m_TicksRemaining;

            public DanseTimer(PlayerMobile caster, int level)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                m_Caster         = caster;
                m_Level          = level;
                m_TicksRemaining = level / 3;
                Priority         = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (m_Caster == null || m_Caster.Deleted)
                {
                    Stop();
                    return;
                }

                DoRandomEffect();

                m_TicksRemaining--;

                if (m_TicksRemaining <= 0)
                    Stop();
            }

            private void DoRandomEffect()
            {
                int maxRoll = (m_Level >= 20) ? 7 : 4;

                switch (Utility.RandomMinMax(1, maxRoll))
                {
                    case 1: SpawnUndeadGiants();        break;
                    case 2: SpawnMummyLords();           break;
                    case 3: HealControlledUndead();      break;
                    case 4: HealCaster();                break;
                    case 5: ExecuteLowHealthEnemies();   break;
                    case 6: StrengthDebuff();            break;
                    case 7: SpawnSkeletalDragon();       break;
                }
            }

            private void SpawnUndeadGiants()
            {
                List<BaseCreature> list = new List<BaseCreature>();
                SpawnRange(m_Caster, list, 1, 2, typeof(PaleMasterUndeadGiant));
                new HordeTimer(list, TimeSpan.FromSeconds(20)).Start();
            }

            private void SpawnMummyLords()
            {
                List<BaseCreature> list = new List<BaseCreature>();
                SpawnRange(m_Caster, list, 2, 3, typeof(PaleMasterMummyLord));
                new HordeTimer(list, TimeSpan.FromSeconds(20)).Start();
            }

            private void SpawnSkeletalDragon()
            {
                List<BaseCreature> list = new List<BaseCreature>();
                SpawnRange(m_Caster, list, 1, 1, typeof(PaleMasterSkeletalDragon));
                new HordeTimer(list, TimeSpan.FromSeconds(20)).Start();
            }

            private void HealControlledUndead()
            {
                IPooledEnumerable eable = m_Caster.Map.GetMobilesInRange(m_Caster.Location, 10);

                try
                {
                    foreach (Mobile m in eable)
                    {
                        BaseCreature bc = m as BaseCreature;

                        if (bc == null || bc.Deleted || !bc.Alive)
                            continue;

                        if (bc.SummonMaster != m_Caster)
                            continue;

                        if (!s_Undead.Slays(bc) && !s_Exorcism.Slays(bc))
                            continue;

                        bc.Hits = Math.Min(bc.Hits + (bc.HitsMax / 2), bc.HitsMax);
                        bc.FixedParticles(0x376A, 10, 15, 5032, 0xB97, 0, EffectLayer.Waist);
                    }
                }
                finally
                {
                    eable.Free();
                }
            }

            private void HealCaster()
            {
                m_Caster.Hits = m_Caster.HitsMax;
                m_Caster.FixedParticles(0x376A, 10, 15, 5032, 0xB97, 0, EffectLayer.Waist);
            }

            private void ExecuteLowHealthEnemies()
            {
                IPooledEnumerable eable = m_Caster.Map.GetMobilesInRange(m_Caster.Location, 3);

                try
                {
                    foreach (Mobile m in eable)
                    {
                        if (m == null || m.Deleted || !m.Alive || m == m_Caster)
                            continue;

                        if (!m_Caster.CanBeHarmful(m))
                            continue;

                        if (s_Undead.Slays(m) || s_Exorcism.Slays(m))
                            continue;

                        if (m.Hits < (m.HitsMax / 2))
                        {
                            m_Caster.DoHarmful(m);
                            AOS.Damage(m, m_Caster, m.HitsMax / 10, 0, 0, 100, 0, 0);
                            m.FixedParticles(0x374A, 10, 15, 5021, 0xB97, 0, EffectLayer.Waist);
                        }
                    }
                }
                finally
                {
                    eable.Free();
                }
            }

            private void StrengthDebuff()
            {
                IPooledEnumerable eable = m_Caster.Map.GetMobilesInRange(m_Caster.Location, 4);

                try
                {
                    foreach (Mobile m in eable)
                    {
                        if (m == null || m.Deleted || !m.Alive || m == m_Caster)
                            continue;

                        if (!m_Caster.CanBeHarmful(m))
                            continue;

                        if (s_Undead.Slays(m) || s_Exorcism.Slays(m))
                            continue;

                        m.AddStatMod(new StatMod(StatType.Str, "DanseStrDebuff", -(m.RawStr / 5), TimeSpan.FromSeconds(20)));
                        m.FixedParticles(0x3779, 10, 15, 5032, 0xB97, 0, EffectLayer.Waist);
                    }
                }
                finally
                {
                    eable.Free();
                }
            }
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
                bc.ControlSlots = 0;
                bc.Controlled   = false;
                bc.IsTempEnemy  = true;

                list.Add(bc);
            }
        }

        private static bool TrySpawnNear(PlayerMobile pm, BaseCreature bc)
        {
            Map map     = pm.Map;
            Point3D loc = pm.Location;

            for (int i = 0; i < 10; i++)
            {
                int x        = loc.X + Utility.RandomMinMax(-3, 3);
                int y        = loc.Y + Utility.RandomMinMax(-3, 3);
                Point3D spawn = new Point3D(x, y, loc.Z);

                if (map.CanFit(spawn, 16, false, false))
                {
                    bc.MoveToWorld(spawn, map);
                    Effects.SendLocationEffect(bc.Location, bc.Map, 0x3728, 15, 10, 2075, 0);
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
                    m_Player.SendMessage("The dead can dance again.");
            }
        }
    }
}