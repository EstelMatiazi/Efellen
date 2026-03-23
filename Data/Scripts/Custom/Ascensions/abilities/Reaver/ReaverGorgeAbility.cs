using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;

namespace Server.Custom.Ascensions
{
    public class ReaverGorgeAbility : AscensionAbility
    {
        public override AscensionType Ascension     { get { return AscensionType.Reaver; } }
        public override int           RequiredLevel { get { return 1; } }
        public override string        Name          { get { return "Gorge"; } }
        
        public override string        DisplayName { get { return "Gorge"; } }
        public override bool          IsPassive     { get { return false; } }
        public override TimeSpan      Cooldown      { get { return TimeSpan.FromMinutes(3); } }

        private static readonly int[] BloodTiles = new int[]
        {
            0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F
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

            if (pm.Stam < 15 || pm.Mana < 15)
            {
                pm.SendMessage("You do not have enough stamina or mana.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            DoGorge(pm, prog.Level);
        }

        private void DoGorge(PlayerMobile pm, int level)
        {
            pm.Stam -= 15;
            pm.Mana    -= 15;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x675, false, "*Gorge*");

            int radius   = 2 + (level / 4);
            int duration = 10 + level;

            PlaceBloodTiles(pm, radius);

            new GorgeTimer(pm, level, radius, TimeSpan.FromSeconds(duration)).Start();
            new CooldownNotifyTimer(pm, Cooldown).Start();
        }

        private static void PlaceBloodTiles(PlayerMobile pm, int radius)
        {
            Map     map = pm.Map;
            Point3D loc = pm.Location;

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (x * x + y * y > radius * radius)
                        continue;

                    int nx = loc.X + x;
                    int ny = loc.Y + y;
                    int nz = map.GetAverageZ(nx, ny);

                    Static blood = new Static(BloodTiles[Utility.Random(BloodTiles.Length)]);
                    blood.Hue    = 0x675;
                    blood.MoveToWorld(new Point3D(nx, ny, nz), map);

                }
            }
        }

        private sealed class GorgeTimer : Timer
        {
            private readonly PlayerMobile        m_Caster;
            private readonly int                 m_Level;
            private readonly int                 m_Radius;
            private readonly Point3D             m_Origin;
            private readonly Map                 m_Map;
            private          int                 m_TicksRemaining;

            private readonly Dictionary<Mobile, GorgeMods> m_Affected;

            private readonly Dictionary<Mobile, SpeedSnapshot> m_Speeds;

            public GorgeTimer(PlayerMobile caster, int level, int radius, TimeSpan duration)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                m_Caster         = caster;
                m_Level          = level;
                m_Radius         = radius;
                m_Origin         = caster.Location;
                m_Map            = caster.Map;
                m_TicksRemaining = (int)duration.TotalSeconds;
                m_Affected       = new Dictionary<Mobile, GorgeMods>();
                m_Speeds         = new Dictionary<Mobile, SpeedSnapshot>();
                Priority         = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                m_TicksRemaining--;

                bool expiring = m_TicksRemaining <= 0;

                if (expiring)
                {
                    RemoveAllMods();
                    RemoveBloodTiles();
                    Stop();
                    return;
                }

                List<Mobile> inRange = GetHostilesInRange();

                List<Mobile> toRemove = new List<Mobile>();

                foreach (KeyValuePair<Mobile, GorgeMods> kvp in m_Affected)
                {
                    if (!inRange.Contains(kvp.Key))
                        toRemove.Add(kvp.Key);
                }

                for (int i = 0; i < toRemove.Count; i++)
                    RemoveMods(toRemove[i]);

                for (int i = 0; i < inRange.Count; i++)
                    ApplyMods(inRange[i]);

                if (m_Level >= 20)
                    CheckCorpseExplosions();
            }

            private void ApplyMods(Mobile m)
            {
                if (m_Affected.ContainsKey(m))
                    RemoveMods(m);

                GorgeMods mods = new GorgeMods();

                int physPenalty = (m_Level * 125) / 100;
                mods.PhysMod    = new ResistanceMod(ResistanceType.Physical, -physPenalty);
                m.AddResistanceMod(mods.PhysMod);

                if (m_Level >= 15)
                {
                    int elemPenalty   = (m_Level * 66) / 100;
                    mods.FireMod      = new ResistanceMod(ResistanceType.Fire,   -elemPenalty);
                    mods.ColdMod      = new ResistanceMod(ResistanceType.Cold,   -elemPenalty);
                    mods.PoisMod      = new ResistanceMod(ResistanceType.Poison, -elemPenalty);
                    mods.NrgyMod      = new ResistanceMod(ResistanceType.Energy, -elemPenalty);

                    m.AddResistanceMod(mods.FireMod);
                    m.AddResistanceMod(mods.ColdMod);
                    m.AddResistanceMod(mods.PoisMod);
                    m.AddResistanceMod(mods.NrgyMod);
                }

                if (m_Level >= 10)
                {
                    BaseCreature bc = m as BaseCreature;

                    if (bc != null && !m_Speeds.ContainsKey(m))
                    {
                        m_Speeds[m] = new SpeedSnapshot(bc.ActiveSpeed, bc.PassiveSpeed);

                        if (bc.ActiveSpeed  > 0.1) bc.ActiveSpeed  = 0.1;
                        if (bc.PassiveSpeed > 0.2) bc.PassiveSpeed = 0.2;
                    }
                }

                m_Affected[m] = mods;
            }

            private void RemoveMods(Mobile m)
            {
                GorgeMods mods;
                if (!m_Affected.TryGetValue(m, out mods))
                    return;

                if (m != null && !m.Deleted)
                {
                    if (mods.PhysMod != null) m.RemoveResistanceMod(mods.PhysMod);
                    if (mods.FireMod != null) m.RemoveResistanceMod(mods.FireMod);
                    if (mods.ColdMod != null) m.RemoveResistanceMod(mods.ColdMod);
                    if (mods.PoisMod != null) m.RemoveResistanceMod(mods.PoisMod);
                    if (mods.NrgyMod != null) m.RemoveResistanceMod(mods.NrgyMod);

                    SpeedSnapshot snap;
                    if (m_Speeds.TryGetValue(m, out snap))
                    {
                        BaseCreature bc = m as BaseCreature;
                        if (bc != null && !bc.Deleted)
                        {
                            bc.ActiveSpeed  = snap.Active;
                            bc.PassiveSpeed = snap.Passive;
                        }
                        m_Speeds.Remove(m);
                    }
                }

                m_Affected.Remove(m);
            }

            private void RemoveAllMods()
            {
                List<Mobile> keys = new List<Mobile>(m_Affected.Keys);
                for (int i = 0; i < keys.Count; i++)
                    RemoveMods(keys[i]);
            }

            private static bool m_ExplosionActive = false;

            private void CheckCorpseExplosions()
            {
                if (m_ExplosionActive)
                    return;

                IPooledEnumerable eable = m_Map.GetItemsInRange(m_Origin, m_Radius);

                List<Corpse> candidates = new List<Corpse>();

                try
                {
                    foreach (Item item in eable)
                    {
                        Corpse corpse = item as Corpse;
                        if (corpse == null || corpse.Deleted)
                            continue;

                        if (!(corpse.Owner is BaseCreature))
                            continue;

                        candidates.Add(corpse);
                    }
                }
                finally
                {
                    eable.Free();
                }

                for (int i = 0; i < candidates.Count; i++)
                {
                    Corpse corpse = candidates[i];

                    if (corpse.Deleted)
                        continue;

                    if (Utility.Random(10000) >= (m_Level * 150))
                        continue;

                    Point3D blastLoc = corpse.Location;
                    Map     blastMap = corpse.Map;

                    Effects.SendLocationParticles(
                        EffectItem.Create(blastLoc, blastMap, EffectItem.DefaultDuration),
                        0x23B2, 10, 30, 0x675, 0, 0, 0
                    );
                  
                    corpse.Delete();

                    int strBonus     = m_Caster.Str / 15;
                    int tacBonus     = (int)(m_Caster.Skills[SkillName.Tactics].Value / 12);
                    int blastDamage  = Utility.RandomMinMax(40, 66) + strBonus + tacBonus;

                    m_ExplosionActive = true;

                    try
                    {
                        IPooledEnumerable blastEable = blastMap.GetMobilesInRange(blastLoc, 2);

                        try
                        {
                            foreach (Mobile m in blastEable)
                            {
                                if (m == null || m.Deleted || !m.Alive || m == m_Caster)
                                    continue;

                                if (!m_Caster.CanBeHarmful(m, false))
                                    continue;

                                m_Caster.DoHarmful(m);
                                AOS.Damage(m, m_Caster, blastDamage, 100, 0, 0, 0, 0);
                            }
                        }
                        finally
                        {
                            blastEable.Free();
                        }
                    }
                    finally
                    {
                        m_ExplosionActive = false;
                    }
                }
            }

            private void RemoveBloodTiles()
            {
                IPooledEnumerable eable = m_Map.GetItemsInRange(m_Origin, m_Radius);

                List<Item> toDelete = new List<Item>();

                try
                {
                    foreach (Item item in eable)
                    {
                        if (item is Static && item.Hue == 0x675)
                            toDelete.Add(item);
                    }
                }
                finally
                {
                    eable.Free();
                }

                for (int i = 0; i < toDelete.Count; i++)
                    toDelete[i].Delete();
            }

            private List<Mobile> GetHostilesInRange()
            {
                List<Mobile> list  = new List<Mobile>();

                IPooledEnumerable eable = m_Map.GetMobilesInRange(m_Origin, m_Radius);

                try
                {
                    foreach (Mobile m in eable)
                    {
                        if (m == null || m.Deleted || !m.Alive || m == m_Caster)
                            continue;

                        if (!m_Caster.CanBeHarmful(m, false))
                            continue;

                        list.Add(m);
                    }
                }
                finally
                {
                    eable.Free();
                }

                return list;
            }
        }

        private sealed class GorgeMods
        {
            public ResistanceMod PhysMod;
            public ResistanceMod FireMod;
            public ResistanceMod ColdMod;
            public ResistanceMod PoisMod;
            public ResistanceMod NrgyMod;
        }

        private sealed class SpeedSnapshot
        {
            public readonly double Active;
            public readonly double Passive;

            public SpeedSnapshot(double active, double passive)
            {
                Active  = active;
                Passive = passive;
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

                m_Player.SendMessage(0x675, "You can now use Gorge again.");
            }
        }
    }
}