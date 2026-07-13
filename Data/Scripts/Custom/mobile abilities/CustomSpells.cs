using System;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Spells;
using Server.Network;
using Server.Misc;
using System.Collections.Generic;
using Server.Items;
using System.ComponentModel;
using System.Runtime.CompilerServices;

/* 

This list shows the lowers level in which a spell becomes available

Level 1

Burngle
Magic Missile
Sleepning Hands
Cause Fear
Cure Light Wounds
Enta
Summon Nature’s Ally I
Summon Spore Field
Vigor, Lesser

Level 2

Aid
Bear’s Endurance
Body of the Sun
Bull’s Strength
Cat’s Grace
Cloud of Knives
Dark Bolt
Flaming Sphere
Hold Person
Melf’s Acid Arrow
Owl’s Wisdom
Scorching Ray
Summon Nature’s Ally II
Web

Level 3

Acidball
Aura of Cold, Lesser
Bestow Curse
Call Lightning
Contagion
Cure Moderate Wounds
Deafening Blast
Dirge of Discord
Dissonant Chord
Fireball
Good Hope
Harmonic Chorus
Lightningbolt
Mass Vigor, Lesser
Prayer
Protection from Energy: Acid
Protection from Energy: Cold
Protection from Energy: Electricity
Protection from Energy: Fire
Spike Growth
Summon Nature’s Ally III
Vigor

Level 4

Flamestrike
Ice Storm
Orb of Acid
Orb of Cold
Orb of Electricity
Orb of Fire
Orb of Force
Shout
Stoneskin
Summon Nature’s Ally IV
Valiant Spirit

Level 5

Call Lightning Storm
Cure Serious Wounds
Slay Living Spell
Summon Nature’s Ally V
Vigor, Greater
Wail of Doom

Level 6

Acid Fog
Bear Endurance, Mass
Bestow Curse, Greater
Bull Strength, Mass
Cacophonic Shield
Cat’s Grace, Mass
Chain Lightning
Cometfall
Disintegrate
Greater Shout
Heal
Mass Vigor
Owl’s Wisdom, Mass
Summon Nature’s Ally VI

Level 7

Aura of Cold, Greater
Firestorm
Finger of Death Spell
Hold Person, Mass
Plague
Sunbeam
Summon Nature’s Ally VII

Level 8

Black Fire
Enervation
Horrid Wilting
Polar Ray
Powerword: Fatigue
Powerword: Fear
Sunburst
Summon Nature’s Ally VIII
Valiant Spirit, mass

Level 9

Iceberg
Meteor Swarm
Powerword: Kill
Storm of Vengeance
Summon Nature’s Ally IX
 */
namespace Server.CustomSpells
{
    public enum EnergyProtectionType
    {
        Cold,
        Fire,
        Energy,
        Poison
    }

    public static class BardSpellHelpers
    {
        public static void ApplyStatBuff(
            Mobile m,
            int str, int dex, int intel,
            TimeSpan duration,
            string namePrefix)
        {
            m.AddStatMod(new StatMod(StatType.Str, namePrefix + "_Str", str, duration));
            m.AddStatMod(new StatMod(StatType.Dex, namePrefix + "_Dex", dex, duration));
            m.AddStatMod(new StatMod(StatType.Int, namePrefix + "_Int", intel, duration));
        }   
        // unused for now
        public static void ApplySkillBuff(
            Mobile m,
            SkillName skill,
            double value,
            string name)
        {
            m.AddSkillMod(new DefaultSkillMod(skill, true, value));
        }
    }


    public static class NatureSpellHelper
    {
        public static void ApplyVigor(
            Mobile m,
            int healPerTick,
            int durationSeconds,
            int level)
        {
            int ticks = durationSeconds;

            for (int i = 0; i < ticks; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i + 1), delegate
                {
                    if (m == null || m.Deleted || !m.Alive)
                        return;

                    m.Heal(healPerTick);

                    m.FixedParticles(0x376A, 9, 32, 5005, 0x3F, 0, EffectLayer.Waist);
                    m.PlaySound(0x202);
                });
            }
        }

        public static void SpawnSporeTile(Point3D loc, Map map, Mobile caster, int level, TimeSpan duration)
        {
            InternalSporeTile tile = new InternalSporeTile(caster, level, duration);
            tile.MoveToWorld(loc, map);

            Effects.SendLocationEffect(loc, map, 0x375A, 20, 10, 0x48, 0);
        }

        private class InternalSporeTile : Item
        {
            private Mobile m_Caster;
            private int m_Level;

            public InternalSporeTile(Mobile caster, int level, TimeSpan duration)
                : base(0x12B7) // mushroom-ish static
            {
                m_Caster = caster;
                m_Level = level;
                Movable = false;
                Hue = 0x48;

                Timer.DelayCall(duration, Delete);

                StartTick();
            }

            private void StartTick()
            {
                Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(2), Tick);
            }

            private void Tick()
            {
                if (Deleted)
                    return;

                foreach (Mobile m in GetMobilesInRange(0))
                {
                    if (!m.Alive)
                        continue;

                    m.ApplyPoison(m_Caster, Poison.Regular);

                    int loss = 3 + m_Level;
                    m.Mana = Math.Max(0, m.Mana - loss);
                    m.Stam = Math.Max(0, m.Stam - loss);

                    m.FixedParticles(0x374A, 10, 15, 5032, 0x48, 0, EffectLayer.Waist);
                }
            }

            public InternalSporeTile(Serial serial) : base(serial) { }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write(0);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                reader.ReadInt();
            }
        }
    }

    public static class DeathSpellHelper
    {
        public static bool IsUndead(Mobile target)
        {
            SlayerEntry silver = SlayerGroup.GetEntryByName(SlayerName.Silver);
            SlayerEntry exorcism = SlayerGroup.GetEntryByName(SlayerName.Exorcism);

            if (target is BaseCreature)
            {
                if (silver != null && silver.Slays(target))
                    return true;

                if (exorcism != null && exorcism.Slays(target))
                    return true;
            }

            return false;
        }
    }


    public static class MassBuffHelper
    {
        public static void ApplyStatBuff(
            Mobile caster,
            int range,
            int amount,
            TimeSpan duration,
            Action<Mobile, int> apply,
            Action<Mobile, int> remove,
            int effect,
            int hue,
            string message)
        {
            IPooledEnumerable eable = caster.GetMobilesInRange(range);

            foreach (Mobile m in eable)
            {
                if(SpellHelpers.isValidHostileTarget(caster, m))
                    continue;

                apply(m, amount);

                m.FixedEffect(effect, 10, 20, hue, 0);
                m.SendMessage(hue, message);

                Timer.DelayCall(duration, delegate
                {
                    if (m != null && !m.Deleted)
                        remove(m, amount);
                });
            }
            eable.Free();
        }
    }

    public static class FieldSpellHelper
    {
        public static void SpawnField(
            Mobile caster,
            Point3D center,
            Map map,
            int count,
            int radius,
            Func<Item> factory)
        {
            for (int i = 0; i < count; i++)
            {
                int x = Utility.RandomMinMax(-radius, radius);
                int y = Utility.RandomMinMax(-radius, radius);

                Item item = factory();
                item.MoveToWorld(new Point3D(center.X + x, center.Y + y, center.Z), map);
            }
        }
    }

    public abstract class SummonNatureAllySpell : CustomSpell
    {
        protected Type CreatureType;
        protected int SpellLevel;

        public SummonNatureAllySpell(
            string name,
            int icon,
            int level,
            Type creatureType)
            : base(name, icon)
        {
            SpellLevel = level;
            CreatureType = creatureType;
            AddLevel(SpellType.Druid, level);
            AddTag(SpellTag.Summon);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            SpellHelpers.SummonNatureAlly(caster, CreatureType, SpellLevel, hue);
        }
    }


    public static class SpellHelpers
    {
        public static bool IsValidCaster(Mobile m)
        {
            return m != null && !m.Deleted && m.Alive && m.Map != null;
        }

        public static void SafeTimer(Mobile caster, Action action)
        {
            if (!IsValidCaster(caster))
                return;
            action();
        }

        public static void SummonNatureAlly(
            Mobile caster,
            Type creatureType,
            int level,
            int hue)
        {
            Map map = caster.Map;
            if (map == null)
                return;     

            Point3D spawnLoc = Point3D.Zero;        

            // Find closest empty tile
            for (int r = 1; r <= 2; r++)
            {
                for (int x = -r; x <= r; x++)
                for (int y = -r; y <= r; y++)
                {
                    Point3D p = new Point3D(caster.X + x, caster.Y + y, caster.Z);      

                    if (map.CanSpawnMobile(p))
                    {
                        spawnLoc = p;
                        goto FOUND;
                    }
                }
            }       

            return;     

        FOUND:      

            Effects.SendLocationEffect(spawnLoc, map, 0x1A5E, 16, 10, hue, 0);
            Timer.DelayCall(TimeSpan.FromSeconds(0.8), delegate
            {
                Effects.SendLocationEffect(spawnLoc, map, 0x1A5E, 16, 10, hue, 0);
            });     

            BaseCreature bc = null;     

            try
            {
                bc = (BaseCreature)Activator.CreateInstance(creatureType);
            }
            catch
            {
                return;
            }       

            bc.MoveToWorld(spawnLoc, map);
            bc.Controlled = true;
            bc.ControlMaster = caster;
            bc.ControlOrder = OrderType.Guard;
            bc.FightMode = FightMode.Closest;
            bc.Summoned = true;
            bc.SummonMaster = caster;

            bc.IsBonded = false;
            bc.CantWalk = false;

            TimeSpan duration = TimeSpan.FromSeconds(20 + level * 2);
            Timer.DelayCall(duration, delegate
            {
                if (bc != null && !bc.Deleted)
                {
                    Effects.SendLocationEffect(bc.Location, bc.Map, 0x3728, 13, 20, hue, 0);
                    bc.Delete();
                }
            });
        }

        public static bool isValidHostileTarget(Mobile caster, Mobile m)
        {
            if (m == null || m.Deleted || !m.Alive)
                return false;
            if (!caster.InLOS(m))
                return false;
            bool valid = false;
            if (m is PlayerMobile)
            {
                valid = true;
            }
            else
            {
                BaseCreature bc = m as BaseCreature;
                if (bc != null && (bc.Controlled || bc.Summoned))
                    valid = true;
            }
            if (!valid)
                return false;
            if (!caster.CanBeHarmful(m))
                return false;
            return valid;
        }

        public static void DamageHostilesAtPoint(
            Mobile caster,
            Point3D center,
            Map map,
            int range,
            Func<int> damageRoll,
            int phys, int fire, int cold, int poison, int energy,
            int effectID,
            int hue)
        {
            if (map == null || caster == null || caster.Deleted)
                return;

            Effects.SendLocationEffect(center, map, effectID, 20, 10, hue, 0);

            IPooledEnumerable eable = map.GetMobilesInRange(center, range);

            foreach (Mobile m in eable)
            {
                if(!isValidHostileTarget(caster,m))
                    continue;

                caster.DoHarmful(m);

                int dmg = damageRoll();
                AOS.Damage(m, caster, dmg, phys, fire, cold, poison, energy);

                m.PlaySound(0x64E);
            }
            eable.Free();
        }

        public static void DoTimedStorm(
            Mobile caster,
            Point3D center,
            Map map,
            int radius,
            int ticks,
            int secondsBetweenTicks,
            Func<int> damageRoll,
            int phys, int fire, int cold, int poison, int energy,
            int effectID,
            int hue,
            string message)
        {
            for (int i = 0; i < ticks; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i * secondsBetweenTicks), () =>
                {
                    if (!SpellHelpers.IsValidCaster(caster))
                        return;
                    if (map == null)
                        return;
                    if (caster.Map != map)
                        return;

                    Effects.SendLocationEffect(center, map, effectID, 30, 10, hue, 0);

                    for(int j = 0; j < 8; j++)
                    {
                        int ox = Utility.RandomMinMax(-radius, radius);
                        int oy = Utility.RandomMinMax(-radius, radius);
                        Point3D boltLoc = new Point3D(center.X + ox, center.Y + oy, center.Z);
                        Effects.SendLocationEffect(boltLoc, map, 0x3818, 20, 10, hue, 0);
                    }

                    Effects.PlaySound(center, map, 0x29);

                    IPooledEnumerable eable = map.GetMobilesInRange(center, radius);
                    foreach (Mobile m in eable)
                    {
                        if(!isValidHostileTarget(caster,m))
                            continue;

                        caster.DoHarmful(m);

                        m.FixedEffect(0x3818, 10, 20, hue, 0);
                        m.FixedEffect(0x37CC, 10, 15, hue, 0);

                        int dmg = damageRoll();
                        AOS.Damage(m, caster, dmg, phys, fire, cold, poison, energy);

                        m.PlaySound(0x64E);
                        m.PlaySound(0x5CF);

                        m.SendMessage(message);
                    }
                    eable.Free();
                });
            }
        }

        public static void ApplyRepeatedStatDrain(
            Mobile target,
            StatType stat,
            int minLoss,
            int maxLoss,
            int ticks,
            int tickDelaySeconds,
            int restoreAfterSeconds)
        {
        for (int i = 0; i < ticks; i++)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(i * tickDelaySeconds), delegate
            {
                if (target == null || target.Deleted)
                    return; 

                int loss = Utility.RandomMinMax(minLoss, maxLoss);  

                if (stat == StatType.Str)
                    target.RawStr -= loss;
                else if (stat == StatType.Dex)
                    target.RawDex -= loss;
                else if (stat == StatType.Int)
                    target.RawInt -= loss;  

                Timer.DelayCall(TimeSpan.FromSeconds(restoreAfterSeconds), delegate
                {
                    if (target != null && !target.Deleted)
                    {
                        if (stat == StatType.Str)
                            target.RawStr += loss;
                        else if (stat == StatType.Dex)
                            target.RawDex += loss;
                        else if (stat == StatType.Int)
                            target.RawInt += loss;
                    }
                });
            });
        }
    }

    public static void ForEachHostileInRange(
        Point3D center,
        Map map,
        Mobile caster,
        int range,
        Action<Mobile> action)
    {
        if (map == null)
            return;

        IPooledEnumerable eable = map.GetMobilesInRange(center, range);

        foreach (Mobile m in eable)
        {
            if (!isValidHostileTarget(caster, m))
                continue;

            action(m);
        }

        eable.Free();
    }


    public static void ForEachHostileInRange(
        Mobile center,
        Mobile caster,
        int range,
        Action<Mobile> action)
    {
        IPooledEnumerable eable = center.GetMobilesInRange(range);

        foreach (Mobile m in eable)
        {
            if(!isValidHostileTarget(caster,m))
                    continue;
            action(m);
        }
        eable.Free();
    }

    public static void DoConeDamage(
        Mobile caster,
        int range,
        int damageMin,
        int damageMax,
        int level,
        int phys, int fire, int cold, int poison, int energy,
        int effectID,
        int hue,
        string message)
    {
        Direction d = caster.Direction & Direction.Mask;
    
        for (int i = 1; i <= range; i++)
        {
            Point3D loc = GetPointInDirection(caster, d, i);
    
            Effects.SendLocationEffect(
                loc,
                caster.Map,
                effectID,
                20,
                10,
                hue,
                0
            );
    
            int damage = Utility.RandomMinMax(damageMin, damageMax) + level;
    
            DamageAtLocation(
                caster,
                loc,
                damage,
                phys, fire, cold, poison, energy
            );
        }
    }


        public static Point3D GetPointInDirection(Mobile m, Direction d, int distance)
        {
            switch (d & Direction.Mask)
            {
                case Direction.North: return new Point3D(m.X, m.Y - distance, m.Z);
                case Direction.South: return new Point3D(m.X, m.Y + distance, m.Z);
                case Direction.West: return new Point3D(m.X - distance, m.Y, m.Z);
                case Direction.East: return new Point3D(m.X + distance, m.Y, m.Z);
                default: return m.Location;
            }
        }

        public static void DamageAtLocation(Mobile caster, Point3D loc, int damage, int phys, int fire, int cold, int poison, int energy)
        {
            IPooledEnumerable eable = caster.Map.GetMobilesInRange(loc, 0);
            foreach (Mobile m in eable)
            {
                if(!isValidHostileTarget(caster,m))
                    continue;

                if (m != caster)
                {
                    caster.DoHarmful(m);
                    AOS.Damage(m, caster, damage, phys, fire, cold, poison, energy);
                    m.PlaySound(0x208);
                }
            }
            eable.Free();
        }

        public static void CreateAreaEffect(Point3D center, Map map, int range, double chance, int effectID, int speed, int duration, int hue)
        {
            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    if (Utility.RandomDouble() < chance)
                    {
                        Point3D loc = new Point3D(center.X + x, center.Y + y, center.Z);
                        Effects.SendLocationEffect(loc, map, effectID, speed, duration, hue, 0);
                    }
                }
            }
        }

        public static void CreateExplosion(Point3D center, Map map, int effectID, int hue)
        {
            Effects.SendLocationEffect(center, map, effectID, 20, 10, hue, 0);
            Effects.PlaySound(center, map, 0x307);

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Point3D loc = new Point3D(center.X + x, center.Y + y, center.Z);
                    Effects.SendLocationEffect(loc, map, effectID, 20, 10, hue, 0);
                }
            }
        }

        public static List<Mobile> GetMobilesInRange(Mobile center, Mobile caster, int range)
        {
            List<Mobile> list = new List<Mobile>();
            IPooledEnumerable eable = center.GetMobilesInRange(range);
            foreach (Mobile m in eable)
            {
                if(!isValidHostileTarget(caster,m))
                    continue;

                list.Add(m);
            }
            eable.Free();
            return list;
        }

        public static Mobile FindNearbyEnemy(Mobile caster, int range)
        {
            IPooledEnumerable eable = caster.GetMobilesInRange(range);
            Mobile target = null;
            foreach (Mobile m in eable)
            {
                if(!isValidHostileTarget(caster,m))
                    continue;
                if (m != caster && caster.CanBeHarmful(m))
                {
                    target = m;
                    break;
                }
            }
            eable.Free();
            return target;
        }
    }

    // ===== LEVEL 1 =====
    public class BurningHandsSpell : CustomSpell
    {
        public BurningHandsSpell() : base("Burning Hands", 0x3709)
        {
            AddLevel(SpellType.Wizard, 1);
            AddLevel(SpellType.Sorcerer, 1);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            caster.FixedParticles(0x3709, 10, 15, 5052, hue != 0 ? hue : 1160, 0, EffectLayer.RightHand);
            caster.FixedParticles(0x3709, 10, 15, 5052, hue != 0 ? hue : 1160, 0, EffectLayer.LeftHand);
            
            SpellHelpers.DoConeDamage(
                caster,
                3,
                15, 20,
                level,
                0, 100, 0, 0, 0, 
                0x3709,
                hue != 0 ? hue : 1160,
                "You are burned by scorching flames!"
            );
        }
    }

    public class CauseFearSpell : CustomSpell
    {
        public CauseFearSpell() : base("Cause Fear", 0x3709)
        {
            AddLevel(SpellType.Wizard, 1);
            AddLevel(SpellType.Sorcerer, 1);
            AddLevel(SpellType.Bard, 1);
            AddLevel(SpellType.Cleric, 1);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.CC);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;

            if(!SpellHelpers.isValidHostileTarget(caster,target))
                return;

            if (target.Skills[SkillName.Knightship].Value > 50 + level * 2)
            {
                target.SendMessage("Your bravery shields you from fear!");
                target.FixedEffect(0x375A, 10, 20, hue != 0 ? hue : 0x47E, 0);
                target.PlaySound(0x1F7);
                return;
            }

            caster.DoHarmful(target);

            target.FixedParticles(0x374A, 10, 15, 5013, hue != 0 ? hue : 0x455, 0, EffectLayer.Waist);
            target.FixedParticles(0x376A, 9, 20, 5044, hue != 0 ? hue : 0x455, 0, EffectLayer.Head);
            target.PlaySound(0x1F8);

            Direction dir = target.GetDirectionTo(caster);
            int distance = level + 1;
            
            for (int i = 0; i < distance; i++)
            {
                target.Move(dir);
                
                if (i % 2 == 0)
                {
                    Effects.SendLocationEffect(target.Location, target.Map, 0x3728, 10, 10, hue != 0 ? hue : 0x455, 0);
                }
            }

            target.FixedParticles(0x376A, 9, 32, 5008, hue != 0 ? hue : 0x455, 0, EffectLayer.Waist);
            target.SendMessage("You flee in terror!");
        }
    }

    public class LesserVigorSpell : CustomSpell
    {
        public LesserVigorSpell() : base("Vigor, Lesser", 0x376A)
        {
            AddLevel(SpellType.Cleric, 1);
            AddLevel(SpellType.Druid, 1);
            AddTag(SpellTag.Heal);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.HoT);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            int duration = 6 + (level / 2);
            
            caster.FixedParticles(0x376A, 9, 32, 5030, hue != 0 ? hue : 0x47D, 0, EffectLayer.Waist);
            caster.PlaySound(0x1F7);
            caster.SendMessage("You feel vitality flowing through your body.");
            
            NatureSpellHelper.ApplyVigor(caster, 3, duration, level);
        }
    }


    public class CureLightWoundsSpell : CustomSpell
    {
        public CureLightWoundsSpell() : base("Cure Light Wounds", 0x376A)
        {
            AddLevel(SpellType.Cleric, 1);
            AddLevel(SpellType.Druid, 1);
            AddLevel(SpellType.Bard, 1);
            AddTag(SpellTag.Heal);
            AddTag(SpellTag.SingleTarget);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            int healAmount = Utility.RandomMinMax(4, 12) * level;
            caster.Heal(healAmount);
            
            caster.FixedParticles(0x376A, 9, 32, 5030, hue != 0 ? hue : 0x47D, 0, EffectLayer.Waist);
            caster.FixedParticles(0x375A, 10, 15, 5018, hue != 0 ? hue : 0x47D, 0, EffectLayer.Head);
            caster.PlaySound(0x202);
            
            caster.SendMessage("You feel healing energy flow through you.");
        }
    }

    public class EntangleSpell : CustomSpell
    {
        public EntangleSpell() : base("Entangle", 0x3728)
        {
            AddLevel(SpellType.Druid, 1);
            AddTag(SpellTag.CC);
            AddTag(SpellTag.AoE);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
            if(!SpellHelpers.isValidHostileTarget(caster,target))
                return;

            SpellHelpers.CreateAreaEffect(
                target.Location, 
                target.Map, 
                4, 
                0.5, 
                0x3735, 
                20, 
                10, 
                hue != 0 ? hue : 0x47E
            );

            Effects.PlaySound(target.Location, target.Map, 0x5C6);

            SpellHelpers.ForEachHostileInRange(target, caster, 4, delegate(Mobile m)
            {
                caster.DoHarmful(m);
                
                double duration = Math.Max(3.0, (3.0 + level) - (m.Skills[SkillName.MagicResist].Value / 100.0 * 6.0));
                m.Paralyze(TimeSpan.FromSeconds(duration));
                
                m.FixedEffect(0x376A, 9, 32);
                m.FixedParticles(0x3735, 10, 20, 5052, hue != 0 ? hue : 0x47E, 0, EffectLayer.Waist);
                m.PlaySound(0x204);
                m.SendMessage("Magical vines entangle you!");
            });
        }
    }

    public class MagicMissileSpell : CustomSpell
    {
        public MagicMissileSpell() : base("Magic Missile", 0x36E4)
        {
            AddLevel(SpellType.Wizard, 1);
            AddLevel(SpellType.Sorcerer, 1);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.SingleTarget);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
            if(!SpellHelpers.isValidHostileTarget(caster,target))
                return;

            caster.DoHarmful(target);
            
            int missiles = (level >= 9) ? 5 : (level >= 7) ? 4 : (level >= 5) ? 3 : (level >= 3) ? 2 : 1;

            for (int i = 0; i < missiles; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i * 0.3), delegate ()
                {
                    if (target == null || !target.Alive || target.Deleted)
                        return;

                    Effects.SendMovingEffect(
                        caster, 
                        target, 
                        0x379F, 
                        7, 
                        0, 
                        false, 
                        false, 
                        hue != 0 ? hue : 0x0213, 
                        0
                    );
                    Effects.PlaySound(caster.Location, caster.Map, 0x1F5);

                    Timer.DelayCall(TimeSpan.FromSeconds(0.5), delegate ()
                    {
                        if (target == null || !target.Alive || target.Deleted)
                            return;

                        int damage = Utility.RandomMinMax(3, 7) + level;
                        AOS.Damage(target, caster, damage, 0, 0, 0, 0, 100);
                        
                        Effects.SendLocationEffect(
                            target.Location, 
                            target.Map, 
                            0x3709, 
                            10, 
                            30, 
                            hue != 0 ? hue : 0x0213, 
                            0
                        );
                        target.FixedParticles(0x36BD, 6, 10, 5044, hue != 0 ? hue : 0x0213, 0, EffectLayer.Waist);
                    });
                });
            }
        }
    }

    public class SleepSpell : CustomSpell
    {
        public SleepSpell() : base("Sleep", 0x376A)
        {
            AddLevel(SpellType.Wizard, 1);
            AddLevel(SpellType.Sorcerer, 1);
            AddLevel(SpellType.Bard, 1);
            AddTag(SpellTag.CC);
            AddTag(SpellTag.AoE);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
            if (target == null || !target.Alive)
                return;

            double baseDuration = Math.Min(9.0, 3.0 + level);
            
            target.FixedEffect(0x375A, 10, 20, hue != 0 ? hue : 0x47E, 0);
            target.FixedParticles(0x373A, 10, 15, 5036, hue != 0 ? hue : 0x47E, 0, EffectLayer.Head);
            target.PlaySound(0x1F2);

            SpellHelpers.ForEachHostileInRange(target, caster, 1, delegate(Mobile m)
            {
                caster.DoHarmful(m);
                
                double duration = Math.Max(1.0, baseDuration - (m.Skills[SkillName.MagicResist].Value / 100.0 * baseDuration));
                m.Paralyze(TimeSpan.FromSeconds(duration));
                
                m.FixedEffect(0x376A, 9, 32, hue != 0 ? hue : 0x47E, 0);
                m.FixedParticles(0x375A, 9, 20, 5044, hue != 0 ? hue : 0x47E, 0, EffectLayer.Head);
                m.PlaySound(0x204);
                m.SendMessage("You fall into a magical slumber!");
            });
        }
    }

    public class SporeFieldSpell : CustomSpell
    {
        public SporeFieldSpell() : base("Spore Field", 0x375A)
        {
            AddLevel(SpellType.Druid, 1);
            AddTag(SpellTag.CC);
            AddTag(SpellTag.Debuff);
            AddTag(SpellTag.AoE);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
            if(!SpellHelpers.isValidHostileTarget(caster,target))
                return;

            int range = 1;
            TimeSpan duration = TimeSpan.FromSeconds(8 + level);

            Effects.SendLocationEffect(target.Location, target.Map, 0x3728, 20, 10, 0x48, 0);
            caster.PlaySound(0x222);

            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    Point3D loc = new Point3D(
                        target.X + x,
                        target.Y + y,
                        target.Z
                    );

                    NatureSpellHelper.SpawnSporeTile(loc, target.Map, caster, level, duration);
                }
            }
        }
    }


    public class SummonNatureAllyISpell : SummonNatureAllySpell
    {
        public SummonNatureAllyISpell()
            : base("Summon Nature's Ally I", 0x2156, 1, typeof(GiantRat))
        {
        }
    }


    // ===== LEVEL 2 =====

    public class AidSpell : CustomSpell
    {
        public AidSpell() : base("Aid", 0x376A)
        {
            AddLevel(SpellType.Cleric, 2);
            AddTag(SpellTag.Buff);
            AddTag(SpellTag.Heal);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            int strBonus = 13 + level;
            int healAmount = Utility.RandomMinMax(12, 22) + level;

            caster.AddStatMod(new StatMod(StatType.Str, "AidStr", strBonus, TimeSpan.FromSeconds(30)));
            caster.Hits += healAmount;

            caster.FixedParticles(0x376A, 10, 32, 5030, hue != 0 ? hue : 0x47D, 0, EffectLayer.Waist);
            caster.FixedParticles(0x375A, 9, 20, 5030, hue != 0 ? hue : 0x47D, 0, EffectLayer.Head);
            caster.FixedParticles(0x373A, 10, 15, 5018, hue != 0 ? hue : 0x47D, 0, EffectLayer.RightHand);
            caster.PlaySound(0x1F2);
        }
    }

    public class BearsEnduranceSpell : CustomSpell
    {
        public BearsEnduranceSpell() : base("Bear's Endurance", 0x480)
        {
            AddLevel(SpellType.Wizard, 2);
            AddLevel(SpellType.Sorcerer, 2);
            AddLevel(SpellType.Druid, 2);
            AddLevel(SpellType.Cleric, 2);
            AddTag(SpellTag.Buff);
            AddTag(SpellTag.SingleTarget);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            int bonusHP = 35 + level;
            int strBonus = Math.Max(1, bonusHP / 2);

            caster.AddStatMod(new StatMod(StatType.Str, "BearsEndurance", strBonus, TimeSpan.FromSeconds(60)));
            caster.Hits += bonusHP;

            caster.FixedEffect(0x376A, 10, 20, hue != 0 ? hue : 0x21, 0);
            caster.FixedParticles(0x373A, 10, 15, 5018, hue != 0 ? hue : 0x21, 0, EffectLayer.Waist);
            caster.FixedParticles(0x375A, 9, 20, 5044, hue != 0 ? hue : 0x21, 0, EffectLayer.Head);
            caster.PlaySound(0x1EA);
            
        }
    }



    public class BodyOfTheSunSpell : CustomSpell
    {
        public BodyOfTheSunSpell() : base("Body of the Sun", 0x3709)
        {
            AddLevel(SpellType.Druid, 2);
            AddLevel(SpellType.Wizard, 2);
            AddLevel(SpellType.Sorcerer, 2);
            AddTag(SpellTag.DoT);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            int duration = level * 2;

            caster.FixedEffect(0x3709, 10, 30, hue != 0 ? hue : 0x501, 0);
            caster.FixedParticles(0x376A, 9, 32, 5008, hue != 0 ? hue : 0x501, 0, EffectLayer.Waist);
            caster.PlaySound(0x208);
            caster.SendMessage("Your body radiates scorching heat!");

            new BodyOfTheSunTimer(caster, level, duration, hue).Start();
        }

        private class BodyOfTheSunTimer : Timer
        {
            private Mobile m_Caster;
            private int m_Level;
            private int m_Ticks;
            private int m_Hue;

            public BodyOfTheSunTimer(Mobile caster, int level, int duration, int hue)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(2))
            {
                m_Caster = caster;
                m_Level = level;
                m_Ticks = duration / 2;
                m_Hue = hue;
            }

            protected override void OnTick()
            {
                if (m_Caster == null || m_Caster.Deleted || !m_Caster.Alive || m_Ticks-- <= 0)
                {
                    Stop();
                    return;
                }

                m_Caster.FixedParticles(0x3709, 5, 10, 5052, m_Hue != 0 ? m_Hue : 0x501, 0, EffectLayer.Waist);

                SpellHelpers.ForEachHostileInRange(m_Caster, m_Caster, 1, delegate(Mobile m)
                {
                    m_Caster.DoHarmful(m);
                    
                    int dmg = Utility.RandomMinMax(8, 13) + m_Level;
                    AOS.Damage(m, m_Caster, dmg, 0, 100, 0, 0, 0);

                    m.FixedEffect(0x3709, 10, 20, m_Hue != 0 ? m_Hue : 0x501, 0);
                    m.PlaySound(0x208);
                });
            }
        }
    }


    public class BullsStrengthSpell : CustomSpell
    {
        public BullsStrengthSpell() : base("Bull's Strength", 0x375A)
        {
            AddLevel(SpellType.Wizard, 2);
            AddLevel(SpellType.Sorcerer, 2);
            AddLevel(SpellType.Cleric, 2);
            AddLevel(SpellType.Druid, 2);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Buff);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            int bonus = 15 + level;
            caster.RawStr += bonus;

            Timer.DelayCall(TimeSpan.FromSeconds(30), delegate
            {
                if (caster != null && !caster.Deleted)
                    caster.RawStr -= bonus;
            });

            caster.FixedEffect(0x375A, 10, 20, hue != 0 ? hue : 0x21, 0);
            caster.FixedParticles(0x373A, 10, 15, 5018, hue != 0 ? hue : 0x21, 0, EffectLayer.Waist);
            caster.FixedParticles(0x376A, 9, 20, 5044, hue != 0 ? hue : 0x21, 0, EffectLayer.LeftHand);
            caster.FixedParticles(0x376A, 9, 20, 5044, hue != 0 ? hue : 0x21, 0, EffectLayer.RightHand);
            caster.PlaySound(0x1E9);
            
            caster.SendMessage("You feel the strength of a raging bull!");
        }
    }

    public class CatsGraceSpell : CustomSpell
    {
        public CatsGraceSpell() : base("Cat's Grace", 0x375A)
        {
            AddLevel(SpellType.Wizard, 2);
            AddLevel(SpellType.Sorcerer, 2);
            AddLevel(SpellType.Bard, 2);
            AddLevel(SpellType.Druid, 2);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Buff);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            int bonus = 15 + level;
            caster.RawDex += bonus;

            Timer.DelayCall(TimeSpan.FromSeconds(30), delegate
            {
                if (caster != null && !caster.Deleted)
                    caster.RawDex -= bonus;
            });

            caster.FixedEffect(0x375A, 10, 20, hue != 0 ? hue : 0x47E, 0);
            caster.FixedParticles(0x376A, 9, 32, 5008, hue != 0 ? hue : 0x47E, 0, EffectLayer.Waist);
            caster.FixedParticles(0x373A, 10, 15, 5036, hue != 0 ? hue : 0x47E, 0, EffectLayer.Head);
            caster.PlaySound(0x1E9);
            
            caster.SendMessage("You move with feline grace and agility!");
        }
    }

    public class CloudOfKnivesSpell : CustomSpell
    {
        public CloudOfKnivesSpell() : base("Cloud of Knives", 0x36CB)
        {
            AddLevel(SpellType.Cleric, 2);
            AddLevel(SpellType.Wizard, 2);
            AddLevel(SpellType.Sorcerer, 2);
            AddTag(SpellTag.DoT);
            AddTag(SpellTag.Offensive);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
            if (target == null)
                return;

            Point3D loc = target.Location;
            Map map = target.Map;

            int duration = 6 + (level / 3) * 2;

            // Enhanced visuals: Spinning blade cloud
            Effects.SendLocationEffect(loc, map, 0x36CB, 20, 10, hue != 0 ? hue : 0x481, 0);
            Effects.PlaySound(loc, map, 0x23B);

            new CloudOfKnivesTimer(caster, loc, map, level, duration, hue).Start();
        }

        private class CloudOfKnivesTimer : Timer
        {
            private Mobile m_Caster;
            private Point3D m_Loc;
            private Map m_Map;
            private int m_Level;
            private int m_Ticks;
            private int m_Hue;

            public CloudOfKnivesTimer(Mobile caster, Point3D loc, Map map, int level, int duration, int hue)
                : base(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2))
            {
                m_Caster = caster;
                m_Loc = loc;
                m_Map = map;
                m_Level = level;
                m_Ticks = duration / 2;
                m_Hue = hue;
            }

            protected override void OnTick()
            {
                if (m_Caster == null || m_Caster.Deleted || m_Ticks-- <= 0)
                {
                    Stop();
                    return;
                }
                Effects.SendLocationEffect(m_Loc, m_Map, 0x36CB, 10, 10, m_Hue != 0 ? m_Hue : 0x481, 0);
                Effects.PlaySound(m_Loc, m_Map, 0x23B);

                IPooledEnumerable eable = m_Map.GetMobilesInRange(m_Loc, 0);
                foreach (Mobile m in eable)
                {
                    if(!SpellHelpers.isValidHostileTarget(m_Caster,m))
                        continue;
                    m_Caster.DoHarmful(m);
                        
                    int dmg = Utility.RandomMinMax(8, 13) + m_Level;
                    AOS.Damage(m, m_Caster, dmg, 100, 0, 0, 0, 0);
                        
                    m.SendMessage("Whirling blades slice into you!");
                }
                eable.Free();
            }
        }
    }


     public class DarkBoltSpell : CustomSpell
    {
        public DarkBoltSpell() : base("Dark Bolt", 0x36BD)
        {
            AddLevel(SpellType.Cleric, 2);
            AddLevel(SpellType.Wizard, 2);
            AddLevel(SpellType.Sorcerer, 2);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.CC);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
            if(!SpellHelpers.isValidHostileTarget(caster,target))
                return;

            caster.DoHarmful(target);

            caster.MovingEffect(target, 0x36BD, 7, 0, false, false, hue != 0 ? hue : 0x497, 0);
            caster.FixedParticles(0x374A, 10, 15, 5013, hue != 0 ? hue : 0x497, 0, EffectLayer.RightHand);

            Timer.DelayCall(TimeSpan.FromSeconds(0.5), delegate()
            {
                if (target == null || target.Deleted || !target.Alive)
                    return;

                int dmg = Utility.RandomMinMax(17, 22) + level;
                AOS.Damage(target, caster, dmg, 100, 0, 0, 0, 0);

                target.FixedParticles(0x374A, 10, 15, 5013, hue != 0 ? hue : 0x497, 0, EffectLayer.Waist);
                target.FixedParticles(0x36BD, 6, 10, 5044, hue != 0 ? hue : 0x497, 0, EffectLayer.Head);
                target.PlaySound(0x1F2);

                double resist = target.Skills[SkillName.MagicResist].Value;
                int roll = Utility.RandomMinMax(6, 26) + level;

                if (resist < roll)
                {
                    target.Paralyze(TimeSpan.FromSeconds(4));
                    target.SendMessage("Dark energy freezes you in place!");
                }
                else
                {
                    target.SendMessage("You resist the paralyzing energy!");
                }
            });
        }
    }

    public class FlamingSphereSpell : CustomSpell
    {
        public FlamingSphereSpell() : base("Flaming Sphere", 0x36BD)
        {
            AddLevel(SpellType.Druid, 2);
            AddLevel(SpellType.Wizard, 2);
            AddLevel(SpellType.Sorcerer, 2);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.DoT);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
            if (target == null)
                return;

            Point3D loc = target.Location;
            Map map = target.Map;

            Effects.SendLocationEffect(loc, map, 0x36BD, 20, 10, hue != 0 ? hue : 1160, 0);
            Effects.PlaySound(loc, map, 0x208);

            int duration = 6 + (level / 3) * 2;
            new FlamingSphereTimer(caster, loc, map, level, duration, hue).Start();
        }

        private class FlamingSphereTimer : Timer
        {
            private Mobile m_Caster;
            private Point3D m_Loc;
            private Map m_Map;
            private int m_Level;
            private int m_Ticks;
            private int m_Hue;

            public FlamingSphereTimer(Mobile caster, Point3D loc, Map map, int level, int duration, int hue)
                : base(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2))
            {
                m_Caster = caster;
                m_Loc = loc;
                m_Map = map;
                m_Level = level;
                m_Ticks = duration / 2;
                m_Hue = hue;
            }

            protected override void OnTick()
            {
                if (m_Caster == null || m_Caster.Deleted || m_Ticks-- <= 0)
                {
                    Stop();
                    return;
                }

                Effects.SendLocationEffect(m_Loc, m_Map, 0x36BD, 20, 10, m_Hue != 0 ? m_Hue : 1160, 0);
                Effects.PlaySound(m_Loc, m_Map, 0x208);

                IPooledEnumerable eable = m_Map.GetMobilesInRange(m_Loc, 1);
                foreach (Mobile m in eable)
                {
                    if(!SpellHelpers.isValidHostileTarget(m_Caster,m))
                        continue;
                    
                    m_Caster.DoHarmful(m);
                        
                    int dmg = Utility.RandomMinMax(12, 21) + m_Level;
                    AOS.Damage(m, m_Caster, dmg, 0, 100, 0, 0, 0);
                        
                    m.FixedParticles(0x3709, 10, 15, 5052, m_Hue != 0 ? m_Hue : 1160, 0, EffectLayer.Waist);
                    m.SendMessage("The flaming sphere burns you!");
                }
                eable.Free();
            }
        }
    }



    public class HoldPersonSpell : CustomSpell
    {
        public HoldPersonSpell() : base("Hold Person", 0x376A)
        {
            AddLevel(SpellType.Cleric, 2);
            AddLevel(SpellType.Wizard, 3);
            AddLevel(SpellType.Sorcerer, 3);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.CC);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
            if(!SpellHelpers.isValidHostileTarget(caster,target))
                return;

            caster.DoHarmful(target);
            
            double duration = Math.Max(1.0, (6.0 + level) - (target.Skills[SkillName.MagicResist].Value / 100.0 * 6.0));
            target.Paralyze(TimeSpan.FromSeconds(duration));

            target.FixedParticles(0x376A, 9, 32, 5030, hue != 0 ? hue : 0x0, 0, EffectLayer.Waist);
            target.FixedParticles(0x3779, 10, 15, 5009, hue != 0 ? hue : 0x0, 0, EffectLayer.Head);
            target.FixedParticles(0x375A, 10, 20, 5044, hue != 0 ? hue : 0x0, 0, EffectLayer.LeftHand);
            target.FixedParticles(0x375A, 10, 20, 5044, hue != 0 ? hue : 0x0, 0, EffectLayer.RightHand);
            target.PlaySound(0x204);
            target.SendMessage("Magical bonds hold you completely still!");
        }
    }

    public class MelfsAcidArrowSpell : CustomSpell
    {
        public MelfsAcidArrowSpell() : base("Melf's Acid Arrow", 0x36D4)
        {
            AddLevel(SpellType.Wizard, 2);
            AddLevel(SpellType.Sorcerer, 2);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.DoT);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
            if(!SpellHelpers.isValidHostileTarget(caster,target))
                return;

            caster.DoHarmful(target);

            Effects.SendMovingEffect(caster, target, 0x36D4, 10, 0, false, false, hue > 0 ? hue : 0x48E, 0);
            Effects.PlaySound(caster.Location, caster.Map, 0x5D3);

            Timer.DelayCall(TimeSpan.FromSeconds(0.5), delegate()
            {
                if (target == null || target.Deleted || !target.Alive)
                    return;

                int initialDamage = Utility.RandomMinMax(20, 25) + level;
                AOS.Damage(target, caster, initialDamage, 0, 0, 0, 100, 0);

                target.FixedParticles(0x36BD, 10, 20, 5044, 0x48E, 0, EffectLayer.Waist);
                target.SendMessage("Acid sears your flesh!");

                Timer.DelayCall(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3), level,
                    new TimerStateCallback(DoTick), new object[] { caster, target });
            });
        }

        private void DoTick(object state)
        {
            object[] data = (object[])state;
            Mobile caster = (Mobile)data[0];
            Mobile target = (Mobile)data[1];

            if (target == null || target.Deleted || !target.Alive)
                return;

            int dotDamage = Utility.RandomMinMax(12, 17);
            AOS.Damage(target, caster, dotDamage, 0, 0, 0, 100, 0);
            
            target.FixedEffect(0x36BD, 10, 20, 0x48E, 0);
            target.SendMessage("The acid continues to burn!");
        }
    }

    public class OwlsWisdomSpell : CustomSpell
    {
        public OwlsWisdomSpell() : base("Owl's Wisdom", 0x375A)
        {
            AddLevel(SpellType.Wizard, 2);
            AddLevel(SpellType.Sorcerer, 2);
            AddLevel(SpellType.Cleric, 2);
            AddLevel(SpellType.Druid, 2);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Buff);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            int bonus = 15 + level;
            caster.RawInt += bonus;

            Timer.DelayCall(TimeSpan.FromSeconds(30), delegate
            {
                if (caster != null && !caster.Deleted)
                    caster.RawInt -= bonus;
            });

            caster.FixedEffect(0x375A, 10, 20, hue != 0 ? hue : 0x480, 0);
            caster.FixedParticles(0x373A, 10, 15, 5018, hue != 0 ? hue : 0x480, 0, EffectLayer.Waist);
            caster.FixedParticles(0x376A, 9, 32, 5008, hue != 0 ? hue : 0x480, 0, EffectLayer.Head);
            caster.PlaySound(0x1E9);
            
            caster.SendMessage("Ancient wisdom fills your mind!");
        }
    }

    public class ScorchingRaySpell : CustomSpell
    {
        public ScorchingRaySpell() : base("Scorching Ray", 0x36D4)
        {
            AddLevel(SpellType.Wizard, 2);
            AddLevel(SpellType.Sorcerer, 2);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Offensive);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
            if(!SpellHelpers.isValidHostileTarget(caster,target))
                return;

            caster.DoHarmful(target);
            
            int rays = Math.Min(3, 1 + ((level - 1) / 4));

            for (int i = 0; i < rays; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i * 0.2), delegate()
                {
                    if (target == null || target.Deleted || !target.Alive)
                        return;

                    caster.MovingParticles(target, 0x36D4, 7, 0, false, true, hue != 0 ? hue : 1160, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
                    
                    int damage = Utility.RandomMinMax(10, 18) + level;
                    AOS.Damage(target, caster, damage, 0, 100, 0, 0, 0);
                    
                    target.FixedParticles(0x3709, 10, 15, 5052, hue != 0 ? hue : 1160, 0, EffectLayer.Waist);
                });
            }
            
            target.PlaySound(0x208);
        }
    }

    public class SummonNatureAllyIISpell : SummonNatureAllySpell
    {
        public SummonNatureAllyIISpell()
            : base("Summon Nature's Ally II", 0x2156, 2, typeof(BlackBear))
        {
        }
    }


    public class WebSpell : CustomSpell
    {
        public WebSpell() : base("Web", 0x376a)
        {
            AddLevel(SpellType.Wizard, 2);
            AddLevel(SpellType.Sorcerer, 2);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.CC);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
            if(!SpellHelpers.isValidHostileTarget(caster,target))
                return;

            List<Mobile> targets = SpellHelpers.GetMobilesInRange(target, caster, 2);

            foreach (Mobile m in targets)
            {
                double duration = Math.Max(3.0, (8 + level) - (m.Dex / 10.0));
                m.Paralyze(TimeSpan.FromSeconds(duration));
                Effects.SendLocationEffect(m.Location, m.Map, 0x23AF, 30, 10, hue != 0 ? hue : 0x0, 0);
                m.FixedParticles(0x376A, 9, 32, 5008, EffectLayer.Waist);
                Effects.PlaySound(m.Location, m.Map, 0x5D2);
                m.SendMessage("You are trapped in a magical web!");
            }
        }
    }
    // ===== LEVEL 3 =====
    public class AcidballSpell : CustomSpell
    {
        public AcidballSpell() : base("Acidball", 0x36D4)
        {
            AddLevel(SpellType.Wizard, 3);
            AddLevel(SpellType.Sorcerer, 3);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;

            if (!SpellHelpers.isValidHostileTarget(caster, target))
                return;

            int damage = Utility.RandomMinMax(20, 25) + level;
            int effectHue = hue != 0 ? hue : 0x4F6;

            caster.FixedParticles(0x36BD, 10, 15, 5044, effectHue, 0, EffectLayer.RightHand);
            caster.PlaySound(0x5D3);

            caster.MovingEffect(target, 0x36D4, 10, 0, false, false, effectHue, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(0.5), delegate()
            {
                if (!SpellHelpers.IsValidCaster(caster))
                    return;

                if (!SpellHelpers.isValidHostileTarget(caster, target))
                    return;

                SpellHelpers.CreateExplosion(target.Location, target.Map, 0x36BD, effectHue);

                for (int i = 0; i < 4; i++)
                {
                    Point3D sprayLoc = new Point3D(
                        target.X + Utility.RandomMinMax(-1, 1),
                        target.Y + Utility.RandomMinMax(-1, 1),
                        target.Z
                    );
                    Effects.SendLocationEffect(sprayLoc, target.Map, 0x36BD, 15, 10, effectHue, 0);
                }

                Effects.PlaySound(target.Location, target.Map, 0x231);

                SpellHelpers.ForEachHostileInRange(target, caster, 1, delegate(Mobile m)
                {
                    caster.DoHarmful(m);

                    AOS.Damage(m, caster, damage, 0, 0, 0, 100, 0);

                    m.FixedParticles(0x374A, 10, 15, 5013, effectHue, 0, EffectLayer.Waist);
                    m.FixedParticles(0x36BD, 8, 12, 5044, effectHue, 0, EffectLayer.Head);

                    for (int i = 1; i <= 2; i++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(i * 0.4), delegate()
                        {
                            if (!SpellHelpers.IsValidCaster(caster))
                                return;

                            if (m != null && !m.Deleted && m.Alive)
                            {
                                m.FixedParticles(0x374A, 5, 10, 5013, effectHue, 0, EffectLayer.Waist);
                                Effects.SendLocationEffect(m.Location, m.Map, 0x36BD, 8, 8, effectHue, 0);
                            }
                        });
                    }

                    m.SendMessage("Acid burns through your armor and flesh!");
                });

                Timer.DelayCall(TimeSpan.FromSeconds(0.6), delegate()
                {
                    if (!SpellHelpers.IsValidCaster(caster))
                        return;

                    if (target != null && !target.Deleted)
                    {
                        Effects.SendLocationEffect(target.Location, target.Map, 0x3728, 20, 10, effectHue, 0);
                    }
                });
            });
        }
    }

    public abstract class AuraOfColdSpell : CustomSpell
    {
        protected int Min;
        protected int Max;

        protected AuraOfColdSpell(string name, int hue, int min, int max) : base(name, hue)
        {
            Min = min;
            Max = max;

            AddTag(SpellTag.DoT);
            AddTag(SpellTag.AoE);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            int ticks = (level * 2) / 2;

            caster.FixedParticles(0x374A, 10, 30, 5032, hue, 3, EffectLayer.Waist);

            Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(2), ticks, () =>
            {
                if(!SpellHelpers.IsValidCaster(caster))
                    return;

                foreach (Mobile m in caster.GetMobilesInRange(1))
                {
                    if (!SpellHelpers.isValidHostileTarget(caster, m))
                        continue;

                    int dmg = Utility.RandomMinMax(Min, Max) + level;

                    SpellHelper.Damage(
                        TimeSpan.Zero,
                        m,
                        caster,
                        dmg,
                        0, 0, 0, 100, 0
                    );
                }
            });
        }
    }

    public class AuraOfColdLesserSpell : AuraOfColdSpell
    {
        public AuraOfColdLesserSpell()
            : base("Aura of Cold, Lesser", 0x480, 16, 21)
        {
            AddLevel(SpellType.Cleric, 3);
            AddLevel(SpellType.Druid, 3);
        }
    }



    public class BestowCurseSpell : CustomSpell
    {
        public BestowCurseSpell() : base("Bestow Curse", 0x455)
        {
            AddLevel(SpellType.Bard, 3);
            AddLevel(SpellType.Cleric, 3);
            AddLevel(SpellType.Wizard, 4);
            AddLevel(SpellType.Sorcerer, 4);

            AddTag(SpellTag.Debuff);
            AddTag(SpellTag.SingleTarget);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
            if (!SpellHelpers.isValidHostileTarget(caster, target))
                return;

            caster.DoHarmful(target);

            int strLoss = (int)(target.RawStr * (0.12 + level * 0.01));
            TimeSpan dur = TimeSpan.FromSeconds(30 + level);

            target.AddStatMod(new StatMod(StatType.Str, "BestowCurse", -strLoss, dur));

            target.FixedEffect(0x3728, 10, 20);
            target.PlaySound(0x1F8);
        }
    }


    public class CallLightningSpell : CustomSpell
    {
        public CallLightningSpell() : base("Call Lightning", 0x379F)
        {
            AddLevel(SpellType.Druid, 3);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.DoT);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            DateTime end = DateTime.UtcNow + TimeSpan.FromSeconds(6 + level);

            Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(3), delegate
            {
               if(!SpellHelpers.IsValidCaster(caster))
                    return;

                Mobile target = SpellHelpers.FindNearbyEnemy(caster, 3);
                if (target != null)
                {
                    caster.DoHarmful(target);
                    target.BoltEffect(hue != 0 ? hue : 0);
                    AOS.Damage(target, caster, Utility.RandomMinMax(16, 21) + level, 0, 0, 0, 0, 100);
                    target.PlaySound(0x29);
                }
            });
        }
    }

    public class CureModerateWoundsSpell : CustomSpell
    {
        public CureModerateWoundsSpell() : base("Cure Moderate Wounds", 0x376A)
        {
            AddLevel(SpellType.Cleric, 3);
            AddLevel(SpellType.Druid, 3);
            AddLevel(SpellType.Bard, 4);
            AddTag(SpellTag.Heal);
            AddTag(SpellTag.SingleTarget);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            caster.Heal(Utility.RandomMinMax(3, 7) * level);
            caster.FixedParticles(0x376A, 9, 32, 5030, hue != 0 ? hue : 0x47D, 0, EffectLayer.Waist);
            caster.FixedParticles(0x375A, 10, 15, 5018, hue != 0 ? hue : 0x47D, 0, EffectLayer.Head);
            caster.PlaySound(0x202);
        }
    }

    public class ContagionSpell : CustomSpell
    {
        public ContagionSpell() : base("Contagion", 0x36BD)
        {
            AddLevel(SpellType.Cleric, 3);
            AddLevel(SpellType.Druid, 3);
            AddLevel(SpellType.Wizard, 4);
            AddLevel(SpellType.Sorcerer, 4);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.Debuff);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;

            if (!SpellHelpers.isValidHostileTarget(caster, target))
                return;

            caster.DoHarmful(target);

            target.FixedParticles(0x374A, 10, 15, 5013, hue != 0 ? hue : 0x48E, 0, EffectLayer.Waist);
            target.FixedParticles(0x36BD, 10, 20, 5044, hue != 0 ? hue : 0x48E, 0, EffectLayer.Head);
             Effects.SendLocationEffect(target.Location, target.Map, 0x3728, 20, 10, hue != 0 ? hue : 0x48E, 0);
            target.PlaySound(0x205);

            target.ApplyPoison(caster, Poison.Greater);

            int stamLoss = 12 + level * 3;
            target.Stam = Math.Max(0, target.Stam - stamLoss);

            int strLoss = 6 + level;
            target.AddStatMod(new StatMod(StatType.Str, "ContagionStr", -strLoss, TimeSpan.FromMinutes(2)));

            target.SendMessage("A virulent disease weakens your body!");

            Timer.DelayCall(TimeSpan.FromSeconds(2), delegate()
            {
                if (target != null && !target.Deleted && target.Alive)
                {
                    target.FixedParticles(0x374A, 5, 10, 5013, hue != 0 ? hue : 0x48E, 0, EffectLayer.Waist);
                }
            });

            Timer.DelayCall(TimeSpan.FromSeconds(4), delegate()
            {
                if (target != null && !target.Deleted && target.Alive)
                {
                    target.FixedParticles(0x374A, 5, 10, 5013, hue != 0 ? hue : 0x48E, 0, EffectLayer.Waist);
                }
            });
        }
    }


   public class DeafeningBlastSpell : CustomSpell
    {
        public DeafeningBlastSpell() : base("Deafening Blast", 0x3728)
        {
            AddLevel(SpellType.Bard, 3);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.Debuff);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;

            if (!SpellHelpers.isValidHostileTarget(caster, target))
                return;

            int damage = Utility.RandomMinMax(20, 25) + level;
            int effectHue = hue != 0 ? hue : 0x47E;

            caster.FixedParticles(0x36BD, 10, 15, 5044, effectHue, 0, EffectLayer.Waist);
            caster.PlaySound(0x307);

            target.FixedEffect(0x37C4, 10, 30, effectHue, 0);
            Effects.SendLocationEffect(target.Location, target.Map, 0x37C4, 20, 10, effectHue, 0);

            SpellHelpers.ForEachHostileInRange(target, caster, 1, delegate(Mobile m)
            {
                caster.DoHarmful(m);

                AOS.Damage(m, caster, damage, 50, 0, 0, 0, 50);

                m.Stam = Math.Max(0, m.Stam - damage);

                m.FixedParticles(0x376A, 9, 32, 5008, effectHue, 0, EffectLayer.Waist);
                m.FixedParticles(0x3728, 10, 15, 5052, effectHue, 0, EffectLayer.Head);
                m.FixedParticles(0x36BD, 6, 10, 5044, effectHue, 0, EffectLayer.CenterFeet);

                for (int i = 0; i < 3; i++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(i * 0.15), delegate()
                    {
                        if (m != null && !m.Deleted && m.Alive)
                        {
                            Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 8, 10, effectHue, 0);
                        }
                    });
                }

                m.PlaySound(0x2F3);
                m.SendMessage("A deafening sonic blast assaults your senses!");
            });

            Timer.DelayCall(TimeSpan.FromSeconds(0.3), delegate()
            {
                if (target != null && !target.Deleted)
                {
                    Effects.SendLocationEffect(target.Location, target.Map, 0x37C4, 15, 10, effectHue, 0);
                }
            });
        }
    }

    public class DirgeOfDiscordSpell : CustomSpell
    {
        public DirgeOfDiscordSpell() : base("Dirge of Discord", 0x455)
        {
            AddLevel(SpellType.Bard, 3);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Debuff);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;

            if (!SpellHelpers.isValidHostileTarget(caster, target))
                return;

            int dexLoss = 20 + level * 2;
            int strLoss = (int)(25 + level * 1.5);
            TimeSpan duration = TimeSpan.FromSeconds(30 + level);
            int effectHue = hue != 0 ? hue : 0x455;

            caster.FixedParticles(0x374A, 10, 15, 5013, effectHue, 0, EffectLayer.Waist);
            caster.PlaySound(0x1F8);

            Effects.SendLocationEffect(target.Location, target.Map, 0x3728, 20, 10, effectHue, 0);

            SpellHelpers.ForEachHostileInRange(target, caster, 2, delegate(Mobile m)
            {
                caster.DoHarmful(m);

                m.AddStatMod(new StatMod(StatType.Dex, "DirgeDex", -dexLoss, duration));
                m.AddStatMod(new StatMod(StatType.Str, "DirgeStr", -strLoss, duration));

                m.FixedEffect(0x376A, 10, 25, effectHue, 0);
                m.FixedParticles(0x374A, 10, 15, 5013, effectHue, 0, EffectLayer.Waist);
                m.FixedParticles(0x3728, 9, 20, 5044, effectHue, 0, EffectLayer.Head);

                for (int i = 1; i <= 3; i++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(i * 0.5), delegate()
                    {
                        if (m != null && !m.Deleted && m.Alive)
                        {
                            m.FixedParticles(0x374A, 5, 10, 5013, effectHue, 0, EffectLayer.Waist);
                            Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 8, effectHue, 0);
                        }
                    });
                }

                m.PlaySound(0x204);
                m.SendMessage("A discordant dirge saps your strength and agility!");
            });

            for (int i = 1; i <= 2; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i * 0.8), delegate()
                {
                    if (target != null && !target.Deleted)
                    {
                        Effects.SendLocationEffect(target.Location, target.Map, 0x3728, 15, 10, effectHue, 0);
                    }
                });
            }
        }
    }

    public class DissonantChordSpell : CustomSpell
    {
        public DissonantChordSpell() : base("Dissonant Chord", 0x481)
        {
            AddLevel(SpellType.Bard, 3);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            int damage = Utility.RandomMinMax(20, 25) + level;
            int effectHue = hue != 0 ? hue : 0x481;

            caster.FixedParticles(0x375A, 10, 15, 5018, effectHue, 0, EffectLayer.Waist);
            caster.FixedParticles(0x376A, 9, 20, 5044, effectHue, 0, EffectLayer.Head);
            caster.PlaySound(0x22F);

            Effects.SendLocationEffect(caster.Location, caster.Map, 0x37C4, 20, 10, effectHue, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(0.3), delegate()
            {
                if (!SpellHelpers.IsValidCaster(caster))
                    return;

                SpellHelpers.ForEachHostileInRange(caster, caster, 2, delegate(Mobile m)
                {
                    caster.DoHarmful(m);

                    AOS.Damage(m, caster, damage, 0, 0, 0, 100, 0);

                    m.FixedEffect(0x36BD, 10, 25, effectHue, 0);
                    m.FixedParticles(0x374A, 10, 15, 5013, effectHue, 0, EffectLayer.Waist);
                    m.FixedParticles(0x36CB, 8, 12, 5044, effectHue, 0, EffectLayer.Head);

                    Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 15, 10, effectHue, 0);

                    for (int i = 0; i < 2; i++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(i * 0.2), delegate()
                        {
                            if (m != null && !m.Deleted && m.Alive)
                            {
                                Effects.SendLocationEffect(m.Location, m.Map, 0x37C4, 8, 8, effectHue, 0);
                            }
                        });
                    }

                    m.PlaySound(0x1F2); 
                    m.SendMessage("A dissonant chord tears through you!");
                });

                Timer.DelayCall(TimeSpan.FromSeconds(0.2), delegate()
                {
                    if (SpellHelpers.IsValidCaster(caster))
                    {
                        Effects.SendLocationEffect(caster.Location, caster.Map, 0x37C4, 25, 10, effectHue, 0);
                    }
                });
            });
        }
    }

    public class FireballSpell : CustomSpell
    {
        public FireballSpell() : base("Fireball", 0x36D4)
        {
            AddLevel(SpellType.Wizard, 3);
            AddLevel(SpellType.Sorcerer, 3);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;

            if (!SpellHelpers.isValidHostileTarget(caster, target))
                return;

            int damage = Utility.RandomMinMax(16, 25) + level;
            int effectHue = hue != 0 ? hue : 1160;

            caster.FixedParticles(0x3709, 10, 15, 5052, effectHue, 0, EffectLayer.RightHand);
            caster.FixedParticles(0x3709, 10, 15, 5052, effectHue, 0, EffectLayer.LeftHand);
            caster.PlaySound(0x15E); // Fire whoosh sound

            caster.MovingEffect(target, 0x36D4, 10, 0, false, false, effectHue, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(0.5), delegate()
            {
                if (!SpellHelpers.IsValidCaster(caster))
                    return;

                if (!SpellHelpers.isValidHostileTarget(caster, target))
                    return;

                SpellHelpers.CreateExplosion(target.Location, target.Map, 0x36BD, effectHue);

                for (int i = 0; i < 6; i++)
                {
                    Point3D flameLoc = new Point3D(
                        target.X + Utility.RandomMinMax(-2, 2),
                        target.Y + Utility.RandomMinMax(-2, 2),
                        target.Z
                    );
                    Timer.DelayCall(TimeSpan.FromSeconds(i * 0.1), delegate()
                    {
                        if (!SpellHelpers.IsValidCaster(caster))
                            return;

                        if (target != null && !target.Deleted)
                        {
                            Effects.SendLocationEffect(flameLoc, target.Map, 0x3709, 15, 10, effectHue, 0);
                        }
                    });
                }

                Effects.PlaySound(target.Location, target.Map, 0x307);

                SpellHelpers.ForEachHostileInRange(target, caster, 1, delegate(Mobile m)
                {
                    caster.DoHarmful(m);

                    AOS.Damage(m, caster, damage, 0, 100, 0, 0, 0);

                    m.FixedParticles(0x3709, 10, 30, 5052, effectHue, 0, EffectLayer.Waist);
                    m.FixedParticles(0x36BD, 10, 15, 5044, effectHue, 0, EffectLayer.Head);
                    m.FixedParticles(0x374A, 8, 12, 5013, effectHue, 0, EffectLayer.CenterFeet);

                    for (int i = 1; i <= 3; i++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(i * 0.3), delegate()
                        {
                            if (!SpellHelpers.IsValidCaster(caster))
                                return;

                            if (m != null && !m.Deleted && m.Alive)
                            {
                                m.FixedParticles(0x3709, 5, 10, 5052, effectHue, 0, EffectLayer.Waist);
                                Effects.SendLocationEffect(m.Location, m.Map, 0x3709, 8, 8, effectHue, 0);
                            }
                        });
                    }

                    m.PlaySound(0x208);
                    m.SendMessage("You are engulfed in a massive fireball!");
                });

                for (int i = 1; i <= 2; i++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(i * 0.5), delegate()
                    {
                        if (!SpellHelpers.IsValidCaster(caster))
                            return;

                        if (target != null && !target.Deleted)
                        {
                            Effects.SendLocationEffect(target.Location, target.Map, 0x3735, 20, 10, effectHue, 0);
                        }
                    });
                }
            });
        }
    }

    public class GoodHopeSpell : CustomSpell
    {
        public GoodHopeSpell() : base("Good Hope", 0x480)
        {
            AddLevel(SpellType.Bard, 3);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Buff);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            int statBonus = 15 + level;
            int skillBonus = 10 + level;
            TimeSpan duration = TimeSpan.FromSeconds(30 + level * 2);
            int effectHue = hue != 0 ? hue : 0x480;

            caster.FixedParticles(0x375A, 10, 15, 5018, effectHue, 0, EffectLayer.Waist);
            caster.FixedParticles(0x376A, 9, 20, 5044, effectHue, 0, EffectLayer.Head);
            caster.PlaySound(0x1F7);

            Effects.SendLocationEffect(caster.Location, caster.Map, 0x375A, 20, 10, effectHue, 0);

            IPooledEnumerable eable = caster.GetMobilesInRange(3);
            foreach (Mobile m in eable)
            {
                if (m == null || m.Deleted || !m.Alive)
                    continue;

                if (SpellHelpers.isValidHostileTarget(caster, m))
                    continue;

                BardSpellHelpers.ApplyStatBuff(m, statBonus, statBonus, statBonus, duration, "GoodHope");
                m.AddSkillMod(new DefaultSkillMod(SkillName.Tactics, true, skillBonus));
                m.AddSkillMod(new DefaultSkillMod(SkillName.Parry, true, skillBonus));

                m.FixedParticles(0x373A, 10, 15, 5012, effectHue, 0, EffectLayer.Waist);
                m.FixedParticles(0x376A, 9, 20, 5008, effectHue, 0, EffectLayer.Head);
                m.FixedParticles(0x375A, 8, 15, 5044, effectHue, 0, EffectLayer.RightHand);

                for (int i = 1; i <= 2; i++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(i * 0.4), delegate()
                    {
                        if (!SpellHelpers.IsValidCaster(caster))
                            return;

                        if (m != null && !m.Deleted && m.Alive)
                        {
                            m.FixedParticles(0x373A, 5, 10, 5012, effectHue, 0, EffectLayer.Waist);
                            Effects.SendLocationEffect(m.Location, m.Map, 0x375A, 10, 8, effectHue, 0);
                        }
                    });
                }

                m.PlaySound(0x1ED);
            }
            eable.Free();

            for (int i = 1; i <= 3; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i * 0.3), delegate()
                {
                    if (!SpellHelpers.IsValidCaster(caster))
                        return;

                    Effects.SendLocationEffect(caster.Location, caster.Map, 0x375A, 15, 10, effectHue, 0);
                });
            }
        }
    }

    public class HarmonicChorusSpell : CustomSpell
    {
        public HarmonicChorusSpell() : base("Harmonic Chorus", 0x480)
        {
            AddLevel(SpellType.Bard, 3);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Buff);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            int skillBonus = 10 + level;
            TimeSpan duration = TimeSpan.FromSeconds(30 + level * 2);
            int effectHue = hue != 0 ? hue : 0x480;

            caster.FixedParticles(0x375A, 10, 15, 5018, effectHue, 0, EffectLayer.Head);
            caster.FixedParticles(0x376A, 9, 20, 5044, effectHue, 0, EffectLayer.Waist);
            caster.PlaySound(0x1F2);

            Effects.SendLocationEffect(caster.Location, caster.Map, 0x37C4, 20, 10, effectHue, 0);

            IPooledEnumerable eable = caster.GetMobilesInRange(2);
            foreach (Mobile m in eable)
            {
                if (m == null || m.Deleted || !m.Alive)
                    continue;

                if (SpellHelpers.isValidHostileTarget(caster, m))
                    continue;

                m.AddStatMod(new StatMod(StatType.Int, "ChorusInt", 25, duration));
                m.AddSkillMod(new DefaultSkillMod(SkillName.Meditation, true, skillBonus));
                m.AddSkillMod(new DefaultSkillMod(SkillName.Focus, true, skillBonus));

                m.FixedParticles(0x373A, 10, 15, 5012, effectHue, 0, EffectLayer.Waist);
                m.FixedParticles(0x375A, 9, 20, 5008, effectHue, 0, EffectLayer.Head);
                m.FixedParticles(0x376A, 8, 15, 5044, effectHue, 0, EffectLayer.RightHand);

                for (int i = 1; i <= 2; i++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(i * 0.3), delegate()
                    {
                        if (!SpellHelpers.IsValidCaster(caster))
                            return;

                        if (m != null && !m.Deleted && m.Alive)
                        {
                            m.FixedParticles(0x376A, 5, 10, 5008, effectHue, 0, EffectLayer.Head);
                            Effects.SendLocationEffect(m.Location, m.Map, 0x37C4, 10, 8, effectHue, 0);
                        }
                    });
                }

                m.PlaySound(0x1ED);
            }
            eable.Free();

            for (int i = 1; i <= 3; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i * 0.25), delegate()
                {
                    if (!SpellHelpers.IsValidCaster(caster))
                        return;

                    Effects.SendLocationEffect(caster.Location, caster.Map, 0x37C4, 15, 10, effectHue, 0);
                });
            }
        }
    }



    public class LightningBoltSpell : CustomSpell
    {
        public LightningBoltSpell() : base("Lightning Bolt", 0x29)
        {
            AddLevel(SpellType.Wizard, 3);
            AddLevel(SpellType.Sorcerer, 3);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            Mobile target = caster.Combatant as Mobile;
            if (!SpellHelpers.isValidHostileTarget(caster, target))
                return;

            int damage = Utility.RandomMinMax(20, 25) + level;
            int effectHue = hue != 0 ? hue : 0x480;

            Direction d = caster.GetDirectionTo(target);
            int dx = 0;
            int dy = 0;

            switch (d & Direction.Mask)
            {
                case Direction.North: dy = -1; break;
                case Direction.South: dy = 1; break;
                case Direction.West: dx = -1; break;
                case Direction.East: dx = 1; break;
                case Direction.Up: dx = -1; dy = -1; break;
                case Direction.Down: dx = 1; dy = 1; break;
                case Direction.Left: dx = -1; dy = 1; break;
                case Direction.Right: dx = 1; dy = -1; break;
            }

            caster.FixedParticles(0x3818, 10, 15, 5052, effectHue, 0, EffectLayer.RightHand);
            caster.PlaySound(0x29);

            Point3D p = caster.Location;
            Map map = caster.Map;

            for (int i = 0; i < 6; i++)
            {
                p = new Point3D(p.X + dx, p.Y + dy, p.Z);

                Effects.SendLocationEffect(p, map, 0x3818, 10, 10, effectHue, 0);

                Timer.DelayCall(TimeSpan.FromSeconds(i * 0.1), delegate()
                {
                    if (!SpellHelpers.IsValidCaster(caster))
                        return;

                    Effects.SendLocationEffect(p, map, 0x3818, 8, 8, effectHue, 0);
                });

                IPooledEnumerable eable = map.GetMobilesInRange(p, 0);
                foreach (Mobile m in eable)
                {
                    if (!SpellHelpers.isValidHostileTarget(caster, m))
                        continue;

                    caster.DoHarmful(m);

                    AOS.Damage(m, caster, damage, 0, 0, 0, 0, 100);

                    m.FixedParticles(0x3818, 10, 15, 5044, effectHue, 0, EffectLayer.Waist);
                    m.FixedParticles(0x374A, 8, 12, 5013, effectHue, 0, EffectLayer.Head);
                    m.BoltEffect(effectHue);

                    m.PlaySound(0x28);
                    m.SendMessage("Lightning strikes through you!");
                }
                eable.Free();
            }
        }
    }



    public class MassLesserVigorSpell : CustomSpell
    {
        public MassLesserVigorSpell() : base("Mass Vigor, Lesser", 0x376A)
        {
            AddLevel(SpellType.Cleric, 3);
            AddLevel(SpellType.Druid, 3);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Heal);
            AddTag(SpellTag.HoT);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            int duration = 6 + (level / 2);

            foreach (Mobile m in caster.GetMobilesInRange(2))
            {
                if (SpellHelpers.isValidHostileTarget(caster, m))
                {
                    NatureSpellHelper.ApplyVigor(m, 3, duration, level);
                }
            }

            caster.PlaySound(0x202);
        }
    }


    public class PrayerSpell : CustomSpell
    {
        public PrayerSpell() : base("Prayer", 0x376A)
        {
            AddLevel(SpellType.Cleric, 3);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Buff);
            AddTag(SpellTag.Debuff);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            int effect = 25 + level;
            TimeSpan duration = TimeSpan.FromSeconds(20 + level);
            int buffHue = hue != 0 ? hue : 0x47D;
            int debuffHue = hue != 0 ? hue : 0x455;

            caster.FixedParticles(0x375A, 10, 30, 5052, buffHue, 0, EffectLayer.Waist);
            caster.FixedParticles(0x376A, 9, 20, 5044, buffHue, 0, EffectLayer.Head);
            caster.PlaySound(0x1F2);

            Effects.SendLocationEffect(caster.Location, caster.Map, 0x375A, 25, 10, buffHue, 0);

            IPooledEnumerable eable = caster.GetMobilesInRange(3);
            foreach (Mobile m in eable)
            {
                if (m == null || m.Deleted || !m.Alive)
                    continue;

                bool isHostile = SpellHelpers.isValidHostileTarget(caster, m);

                if (isHostile)
                {
                    caster.DoHarmful(m);

                    m.AddStatMod(new StatMod(StatType.Str, "PrayerStrDebuff", -effect, duration));
                    m.AddStatMod(new StatMod(StatType.Dex, "PrayerDexDebuff", -effect, duration));

                    m.FixedEffect(0x374A, 10, 32, debuffHue, 0);
                    m.FixedParticles(0x374A, 10, 15, 5013, debuffHue, 0, EffectLayer.Waist);
                    m.FixedParticles(0x3728, 8, 12, 5044, debuffHue, 0, EffectLayer.Head);

                    for (int i = 1; i <= 2; i++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(i * 0.4), delegate()
                        {
                            if (!SpellHelpers.IsValidCaster(caster))
                                return;

                            if (m != null && !m.Deleted && m.Alive)
                            {
                                m.FixedParticles(0x374A, 5, 10, 5013, debuffHue, 0, EffectLayer.Waist);
                            }
                        });
                    }

                    m.PlaySound(0x1F8);
                    m.SendMessage("A divine curse weakens your body!");
                }
                else
                {
                    m.AddStatMod(new StatMod(StatType.Str, "PrayerStr", effect, duration));
                    m.AddStatMod(new StatMod(StatType.Dex, "PrayerDex", effect, duration));

                    m.FixedEffect(0x376A, 10, 32, buffHue, 0);
                    m.FixedParticles(0x376A, 9, 20, 5044, buffHue, 0, EffectLayer.Waist);
                    m.FixedParticles(0x375A, 8, 15, 5018, buffHue, 0, EffectLayer.Head);

                    for (int i = 1; i <= 2; i++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(i * 0.4), delegate()
                        {
                            if (!SpellHelpers.IsValidCaster(caster))
                                return;

                            if (m != null && !m.Deleted && m.Alive)
                            {
                                m.FixedParticles(0x376A, 5, 10, 5018, buffHue, 0, EffectLayer.Waist);
                            }
                        });
                    }

                    m.PlaySound(0x202);
                }
            }
            eable.Free();

            for (int i = 1; i <= 3; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i * 0.3), delegate()
                {
                    if (!SpellHelpers.IsValidCaster(caster))
                        return;

                    Effects.SendLocationEffect(caster.Location, caster.Map, 0x375A, 20, 10, buffHue, 0);
                });
            }
        }
    }

    public abstract class ProtectionFromEnergySpell : CustomSpell
    {
        private ResistanceType m_ResistType;
        private EnergyProtectionType m_Type;

        protected ProtectionFromEnergySpell(
            string name,
            int hue,
            ResistanceType resistType,
            EnergyProtectionType type
        ) : base(name, hue)
        {
            m_ResistType = resistType;
            m_Type = type;

            AddTag(SpellTag.Buff);
            AddTag(SpellTag.SingleTarget);

            AddLevel(SpellType.Cleric, 3);
            AddLevel(SpellType.Druid, 3);
            AddLevel(SpellType.Wizard, 3);
            AddLevel(SpellType.Sorcerer, 3);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            int amount = 12 + level;

            if (GetResist(caster) >= 70)
            {
                return;
            }

            if (GetResist(caster) + amount > 70)
                amount = 70 - GetResist(caster);

            TimeSpan duration = TimeSpan.FromSeconds(40 + level * 2);

            ResistanceMod mod = new ResistanceMod(m_ResistType, amount);

            caster.AddResistanceMod(mod);

            caster.FixedParticles(0x373A, 10, 15, 5012, hue, 3, EffectLayer.Waist);
            caster.PlaySound(0x1EA);

            new ExpireTimer(caster, mod, duration, m_Type).Start();
        }

        private int GetResist(Mobile m)
        {
            switch (m_ResistType)
            {
                case ResistanceType.Cold: return m.ColdResistance;
                case ResistanceType.Fire: return m.FireResistance;
                case ResistanceType.Energy: return m.EnergyResistance;
                case ResistanceType.Poison: return m.PoisonResistance;
            }
            return 0;
        }

        private class ExpireTimer : Timer
        {
            private Mobile m_Mobile;
            private ResistanceMod m_Mod;
            private EnergyProtectionType m_Type;

            public ExpireTimer(Mobile m, ResistanceMod mod, TimeSpan delay, EnergyProtectionType type)
                : base(delay)
            {
                m_Mobile = m;
                m_Mod = mod;
                m_Type = type;
            }

            protected override void OnTick()
            {
                if (m_Mobile == null)
                    return;

                m_Mobile.RemoveResistanceMod(m_Mod);

                switch (m_Type)
                {
                    case EnergyProtectionType.Cold:
                        m_Mobile.SendMessage("Your protection from cold fades.");
                        break;
                    case EnergyProtectionType.Fire:
                        m_Mobile.SendMessage("Your protection from fire fades.");
                        break;
                    case EnergyProtectionType.Energy:
                        m_Mobile.SendMessage("Your protection from electricity fades.");
                        break;
                    case EnergyProtectionType.Poison:
                        m_Mobile.SendMessage("Your protection from acid fades.");
                        break;
                }

                Stop();
            }
        }
    }

    public class ProtectionFromColdSpell : ProtectionFromEnergySpell
    {
        public ProtectionFromColdSpell()
            : base("Protection from Energy: Cold", 0x480, ResistanceType.Cold, EnergyProtectionType.Cold)
        {
        }
    }

    public class ProtectionFromFireSpell : ProtectionFromEnergySpell
    {
        public ProtectionFromFireSpell()
            : base("Protection from Energy: Fire", 1160, ResistanceType.Fire, EnergyProtectionType.Fire)
        {
        }
    }

    public class ProtectionFromElectricitySpell : ProtectionFromEnergySpell
    {
        public ProtectionFromElectricitySpell()
            : base("Protection from Energy: Electricity", 0x4A0, ResistanceType.Energy, EnergyProtectionType.Energy)
        {
        }
    }
    public class ProtectionFromAcidSpell : ProtectionFromEnergySpell
    {
        public ProtectionFromAcidSpell()
            : base("Protection from Energy: Acid", 0x455, ResistanceType.Poison, EnergyProtectionType.Poison)
        {
        }
    }



    
    public class SpikeGrowthSpell : CustomSpell
    {
        public SpikeGrowthSpell() : base("Spike Growth", 0x36BD)
        {
            AddLevel(SpellType.Druid, 3);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.Debuff);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            Mobile target = caster.Combatant as Mobile;
            if (!SpellHelpers.isValidHostileTarget(caster, target))
                return;

            Point3D center = target.Location;
            Map map = target.Map;
            int effectHue = hue != 0 ? hue : 0x59B;
            int damage = Utility.RandomMinMax(20, 25) + level;

            caster.FixedParticles(0x3735, 10, 15, 5052, effectHue, 0, EffectLayer.Waist);
            caster.PlaySound(0x5C6);

            Effects.SendLocationEffect(center, map, 0x36BD, 20, 10, effectHue, 0);

            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    if (Utility.RandomDouble() < 0.4)
                    {
                        Point3D spikeLoc = new Point3D(center.X + x, center.Y + y, center.Z);
                        Effects.SendLocationEffect(spikeLoc, map, 0x3735, 15, 10, effectHue, 0);
                    }
                }
            }

            int duration = level + 1;
            for (int i = 0; i < duration; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i * 2), delegate
                {
                    if (!SpellHelpers.IsValidCaster(caster))
                        return;

                    Effects.SendLocationEffect(center, map, 0x36BD, 15, 10, effectHue, 0);

                    SpellHelpers.ForEachHostileInRange(caster, caster, 2, delegate(Mobile m)
                    {
                        caster.DoHarmful(m);

                        AOS.Damage(m, caster, damage, 100, 0, 0, 0, 0);
                        m.Stam = Math.Max(0, m.Stam - damage);

                        m.FixedEffect(0x36BD, 10, 10, effectHue, 0);
                        m.FixedParticles(0x3735, 10, 15, 5044, effectHue, 0, EffectLayer.CenterFeet);
                        m.FixedParticles(0x374A, 8, 12, 5013, effectHue, 0, EffectLayer.Waist);

                        m.PlaySound(0x22F);
                        m.SendMessage("Sharp spikes pierce through you!");
                    });
                });
            }
        }
    }

    public class SummonNatureAllyIIISpell : SummonNatureAllySpell
    {
        public SummonNatureAllyIIISpell()
            : base("Summon Nature's Ally III", 0x2156, 3, typeof(WolfDire))
        {
        }
    }
    public class VigorSpell : CustomSpell
    {
        public VigorSpell() : base("Vigor", 0x376A)
        {
            AddLevel(SpellType.Cleric, 3);
            AddLevel(SpellType.Druid, 3);
            AddTag(SpellTag.Heal);
            AddTag(SpellTag.HoT);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            int duration = 9 + (level / 2);
            NatureSpellHelper.ApplyVigor(caster, 6, duration, level);
        }
    }


    // ===== LEVEL 4 =====
    public class FlameStrikeSpell : CustomSpell
    {
        public FlameStrikeSpell() : base("Flamestrike", 0x3709)
        {
            AddLevel(SpellType.Wizard, 4);
            AddLevel(SpellType.Sorcerer, 4);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Offensive);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            Mobile target = caster.Combatant as Mobile;
            if (!SpellHelpers.isValidHostileTarget(caster, target))
                return;

            int damage = Utility.RandomMinMax(30, 35) + level;
            int effectHue = hue != 0 ? hue : 1160;

            caster.FixedParticles(0x3709, 10, 15, 5052, effectHue, 0, EffectLayer.RightHand);
            caster.PlaySound(0x15E);

            Point3D above = new Point3D(target.X, target.Y, target.Z + 20);
            Effects.SendLocationEffect(above, target.Map, 0x3709, 30, 10, effectHue, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(0.4), delegate()
            {
                if (!SpellHelpers.IsValidCaster(caster))
                    return;

                if (!SpellHelpers.isValidHostileTarget(caster, target))
                    return;

                caster.DoHarmful(target);

                target.FixedParticles(0x3709, 10, 30, 5052, effectHue, 0, EffectLayer.Waist);
                target.FixedParticles(0x36BD, 10, 20, 5044, effectHue, 0, EffectLayer.Head);
                target.FixedParticles(0x374A, 8, 15, 5013, effectHue, 0, EffectLayer.CenterFeet);

                SpellHelpers.CreateExplosion(target.Location, target.Map, 0x3709, effectHue);

                AOS.Damage(target, caster, damage, 0, 100, 0, 0, 0);

                for (int i = 1; i <= 3; i++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(i * 0.2), delegate()
                    {
                        if (!SpellHelpers.IsValidCaster(caster))
                            return;

                        if (target != null && !target.Deleted && target.Alive)
                        {
                            target.FixedParticles(0x3709, 5, 10, 5052, effectHue, 0, EffectLayer.Waist);
                            Effects.SendLocationEffect(target.Location, target.Map, 0x3709, 10, 8, effectHue, 0);
                        }
                    });
                }

                target.SendMessage("A pillar of divine fire engulfs you!");
            });
        }
    }

    public class IceStormSpell : CustomSpell
    {
        public IceStormSpell() : base("Ice Storm", 0x480)
        {
            AddLevel(SpellType.Druid, 4);
            AddLevel(SpellType.Wizard, 4);
            AddLevel(SpellType.Sorcerer, 4);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            Mobile target = caster.Combatant as Mobile;
            if (!SpellHelpers.isValidHostileTarget(caster, target))
                return;

            Point3D loc = target.Location;
            Map map = target.Map;
            int damage = Utility.RandomMinMax(24, 29) + level;
            int effectHue = hue != 0 ? hue : 0x480;

            caster.FixedParticles(0x376A, 10, 15, 5044, effectHue, 0, EffectLayer.Waist);
            caster.FixedParticles(0x375A, 9, 20, 5018, effectHue, 0, EffectLayer.Head);
            caster.PlaySound(0x64F);

            Effects.SendLocationEffect(loc, map, 0x376A, 30, 10, effectHue, 0);

            for (int x = -3; x <= 3; x++)
            {
                for (int y = -3; y <= 3; y++)
                {
                    if (Utility.RandomDouble() < 0.3)
                    {
                        Point3D iceLoc = new Point3D(loc.X + x, loc.Y + y, loc.Z);
                        Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomDouble() * 0.5), delegate()
                        {
                            if (!SpellHelpers.IsValidCaster(caster))
                                return;

                            Effects.SendLocationEffect(iceLoc, map, 0x36BD, 15, 10, effectHue, 0);
                        });
                    }
                }
            }

            Timer.DelayCall(TimeSpan.FromSeconds(0.3), delegate()
            {
                if (!SpellHelpers.IsValidCaster(caster))
                    return;

                SpellHelpers.ForEachHostileInRange(target, caster, 3, delegate(Mobile m)
                {
                    caster.DoHarmful(m);

                    AOS.Damage(m, caster, damage, 0, 0, 100, 0, 0);

                    m.FixedParticles(0x374A, 10, 15, 5032, effectHue, 3, EffectLayer.Waist);
                    m.FixedParticles(0x376A, 9, 20, 5044, effectHue, 0, EffectLayer.Head);
                    m.FixedParticles(0x36BD, 8, 12, 5013, effectHue, 0, EffectLayer.CenterFeet);

                    int slowDuration = 3 + (level / 2);
                    m.Paralyze(TimeSpan.FromSeconds(slowDuration));

                    for (int i = 1; i <= 2; i++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(i * 0.5), delegate()
                        {
                            if (!SpellHelpers.IsValidCaster(caster))
                                return;

                            if (m != null && !m.Deleted && m.Alive)
                            {
                                m.FixedParticles(0x374A, 5, 10, 5032, effectHue, 0, EffectLayer.Waist);
                                Effects.SendLocationEffect(m.Location, m.Map, 0x36BD, 10, 8, effectHue, 0);
                            }
                        });
                    }

                    m.PlaySound(0x64F);
                    m.SendMessage("Freezing ice shards rain down upon you!");
                });
            });
        }
    }


    public abstract class BaseOrbSpell : CustomSpell
    {
        protected abstract int Phys { get; }
        protected abstract int Fire { get; }
        protected abstract int Cold { get; }
        protected abstract int Poison { get; }
        protected abstract int Energy { get; }
        protected abstract int Hue { get; }
        protected abstract string HitMessage { get; }

        public BaseOrbSpell(string name) : base(name, 0x36D4) { }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
            if (target == null || !caster.CanBeHarmful(target))
                return;

            caster.DoHarmful(target);

            caster.MovingParticles(
                target,
                0x36D4,
                7,
                0,
                false,
                true,
                Hue,
                0,
                9502,
                1,
                0,
                (EffectLayer)255,
                0
            );

            int damage = Utility.RandomMinMax(30, 35) + level;

            AOS.Damage(
                target,
                caster,
                damage,
                Phys, Fire, Cold, Poison, Energy
            );

            target.SendMessage(HitMessage);
        }
    }

    public class OrbOfAcidSpell : BaseOrbSpell
    {
        public OrbOfAcidSpell() : base("Orb of Acid")
        {
            AddLevel(SpellType.Wizard, 4);
            AddLevel(SpellType.Sorcerer, 4);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Offensive);
        }

        protected override int Phys { get { return 0; } }
        protected override int Fire { get { return 0; } }
        protected override int Cold { get { return 0; } }
        protected override int Poison { get { return 100; } }
        protected override int Energy { get { return 0; } }
        protected override int Hue { get { return 0x3F; } }
        protected override string HitMessage { get { return "Acid burns your flesh!"; } }
    }

    public class OrbOfColdSpell : BaseOrbSpell
    {
        public OrbOfColdSpell() : base("Orb of Cold")
        {
            AddLevel(SpellType.Wizard, 4);
            AddLevel(SpellType.Sorcerer, 4);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Offensive);
        }
    
        protected override int Phys { get { return 0; } }
        protected override int Fire { get { return 0; } }
        protected override int Cold { get { return 100; } }
        protected override int Poison { get { return 0; } }
        protected override int Energy { get { return 0; } }
        protected override int Hue { get { return 0x3F; } }
        protected override string HitMessage { get { return "Cold scars your flesh!"; } }
    }

    public class OrbOfElectricitySpell : BaseOrbSpell
    {
        public OrbOfElectricitySpell() : base("Orb of Electricity")
        {
            AddLevel(SpellType.Wizard, 4);
            AddLevel(SpellType.Sorcerer, 4);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Offensive);
        }
    
        protected override int Phys { get { return 0; } }
        protected override int Fire { get { return 0; } }
        protected override int Cold { get { return 0; } }
        protected override int Poison { get { return 0; } }
        protected override int Energy { get { return 100; } }
        protected override int Hue { get { return 0x3F; } }
        protected override string HitMessage { get { return "Electricity runs through your flesh!"; } }
    }

    public class OrbOfFireSpell : BaseOrbSpell
    {
        public OrbOfFireSpell() : base("Orb of Fire")
        {
            AddLevel(SpellType.Wizard, 4);
            AddLevel(SpellType.Sorcerer, 4);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Offensive);
        }
    
        protected override int Phys { get { return 0; } }
        protected override int Fire { get { return 100; } }
        protected override int Cold { get { return 0; } }
        protected override int Poison { get { return 0; } }
        protected override int Energy { get { return 0; } }
        protected override int Hue { get { return 0x3F; } }
        protected override string HitMessage { get { return "Fire gnaws at your flesh!"; } }
    }

    public class OrbOfForceSpell : BaseOrbSpell
    {
        public OrbOfForceSpell() : base("Orb of Force")
        {
            AddLevel(SpellType.Wizard, 4);
            AddLevel(SpellType.Sorcerer, 4);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Offensive);
        }
    
        protected override int Phys { get { return 100; } }
        protected override int Fire { get { return 0; } }
        protected override int Cold { get { return 0; } }
        protected override int Poison { get { return 0; } }
        protected override int Energy { get { return 0; } }
        protected override int Hue { get { return 0x3F; } }
        protected override string HitMessage { get { return "A powerful impact hits you!"; } }
    }
    
    public class ShoutSpell : CustomSpell
    {
        public ShoutSpell() : base("Shout", 0x375A)
        {
            AddLevel(SpellType.Wizard, 4);
            AddLevel(SpellType.Sorcerer, 4);
            AddLevel(SpellType.Bard, 4);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            int effectHue = hue != 0 ? hue : 0x480;

            caster.FixedParticles(0x37C4, 10, 15, 5044, effectHue, 0, EffectLayer.Head);
            caster.FixedParticles(0x375A, 9, 20, 5018, effectHue, 0, EffectLayer.Waist);
            caster.PlaySound(0x2A1);

            Effects.SendLocationEffect(caster.Location, caster.Map, 0x37C4, 25, 10, effectHue, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(0.2), delegate()
            {
                if (!SpellHelpers.IsValidCaster(caster))
                    return;

                Direction d = caster.Direction & Direction.Mask;

                for (int i = 1; i <= 6; i++)
                {
                    Point3D loc = SpellHelpers.GetPointInDirection(caster, d, i);

                    Effects.SendLocationEffect(loc, caster.Map, 0x37C4, 20, 10, effectHue, 0);
                    Effects.SendLocationEffect(loc, caster.Map, 0x3728, 15, 10, effectHue, 0);

                    int damage = Utility.RandomMinMax(24, 29) + level;

                    IPooledEnumerable eable = caster.Map.GetMobilesInRange(loc, 0);
                    foreach (Mobile m in eable)
                    {
                        if (!SpellHelpers.isValidHostileTarget(caster, m))
                            continue;

                        caster.DoHarmful(m);

                        AOS.Damage(m, caster, damage, 30, 0, 0, 0, 70);

                        m.FixedParticles(0x37C4, 10, 15, 5044, effectHue, 0, EffectLayer.Waist);
                        m.FixedParticles(0x3728, 8, 12, 5013, effectHue, 0, EffectLayer.Head);

                        int pushBack = 2;
                        Direction pushDir = caster.GetDirectionTo(m);
                        Direction opposite = (Direction)(((int)pushDir + 4) % 8);

                        for (int p = 0; p < pushBack; p++)
                        {
                            m.Move(opposite);
                        }

                        m.PlaySound(0x2F3);
                        m.SendMessage("A thunderous shout blasts you backwards!");
                    }
                    eable.Free();
                }

                for (int i = 1; i <= 3; i++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(i * 0.15), delegate()
                    {
                        if (!SpellHelpers.IsValidCaster(caster))
                            return;

                        Point3D midLoc = SpellHelpers.GetPointInDirection(caster, d, 3);
                        Effects.SendLocationEffect(midLoc, caster.Map, 0x37C4, 20, 10, effectHue, 0);
                    });
                }
            });
        }
    }

    public class StoneskinSpell : CustomSpell
    {
        public StoneskinSpell() : base("Stoneskin", 0x375A)
        {
            AddLevel(SpellType.Wizard, 4);
            AddLevel(SpellType.Sorcerer, 4);
            AddLevel(SpellType.Druid, 5);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Buff);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            int bonus = 25 * level;

            caster.Hits += bonus;

            caster.FixedEffect(0x375A, 10, 30);
            caster.PlaySound(0x1E9);

        }
    }
    public class SummonNatureAllyIVSpell : SummonNatureAllySpell
    {
        public SummonNatureAllyIVSpell()
            : base("Summon Nature's Ally IV", 0x2156, 4, typeof(FireSalamander))
        {
        }
    }

    public class ValiantSpiritSpell : CustomSpell
    {
        public ValiantSpiritSpell() : base("Valiant Spirit", 0x480)
        {
            AddLevel(SpellType.Bard, 4);
            AddLevel(SpellType.Cleric, 4);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Buff);
            AddTag(SpellTag.Heal);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            int heal = Utility.RandomMinMax(20, 32) + level;
            int str = 5 + level;
            int resist = Math.Min(70, 6 + level);

            caster.Hits += heal;
            caster.AddStatMod(new StatMod(StatType.Str, "ValiantStr", str, TimeSpan.FromSeconds(60 + level * 10)));

            TimeSpan dur = TimeSpan.FromSeconds(60 + level * 10);
            caster.AddResistanceMod(new ResistanceMod(ResistanceType.Fire, resist));
            caster.AddResistanceMod(new ResistanceMod(ResistanceType.Cold, resist));
            caster.AddResistanceMod(new ResistanceMod(ResistanceType.Energy, resist));
            caster.AddResistanceMod(new ResistanceMod(ResistanceType.Poison, resist));
            caster.AddResistanceMod(new ResistanceMod(ResistanceType.Physical, resist));

            caster.FixedParticles(0x375A, 10, 15, 5012, hue, 0, EffectLayer.Waist);
            caster.PlaySound(0x1F7);
        }
    }


    // ===== LEVEL 5 =====
    public class CallLightningStormSpell : CustomSpell
    {
        public CallLightningStormSpell() : base("Call Lightning Storm", 0x379F)
        {
            AddLevel(SpellType.Druid, 5);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.DoT);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            TimeSpan duration = TimeSpan.FromSeconds(12 + level);
            DateTime end = DateTime.UtcNow + duration;
            int damage = Utility.RandomMinMax(24, 29) + level;
            int effectHue = hue != 0 ? hue : 0x480;

            caster.FixedParticles(0x376A, 10, 15, 5044, effectHue, 0, EffectLayer.Waist);
            caster.FixedParticles(0x3818, 9, 20, 5018, effectHue, 0, EffectLayer.Head);
            caster.PlaySound(0x28);

            caster.PublicOverheadMessage(
                Server.Network.MessageType.Emote,
                0x3B2,
                false,
                "*dark storm clouds gather overhead*"
            );

            Effects.SendLocationEffect(caster.Location, caster.Map, 0x3728, 30, 10, effectHue, 0);

            Timer.DelayCall(
                TimeSpan.Zero,
                TimeSpan.FromSeconds(3),
                delegate
                {
                    if (!SpellHelpers.IsValidCaster(caster) || DateTime.UtcNow > end)
                        return;

                    Mobile target = SpellHelpers.FindNearbyEnemy(caster, 6);
                    if (target == null)
                        return;

                    Point3D above = new Point3D(target.X, target.Y, target.Z + 20);
                    Effects.SendLocationEffect(above, target.Map, 0x3818, 25, 10, effectHue, 0);

                    Timer.DelayCall(TimeSpan.FromSeconds(0.3), delegate()
                    {
                        if (!SpellHelpers.IsValidCaster(caster))
                            return;

                        if (!SpellHelpers.isValidHostileTarget(caster, target))
                            return;

                        caster.DoHarmful(target);

                        target.BoltEffect(effectHue);
                        target.FixedParticles(0x3818, 10, 15, 5044, effectHue, 0, EffectLayer.Waist);
                        target.FixedParticles(0x374A, 8, 12, 5013, effectHue, 0, EffectLayer.Head);

                        Effects.SendLocationEffect(target.Location, target.Map, 0x3818, 20, 10, effectHue, 0);

                        AOS.Damage(target, caster, damage, 0, 0, 0, 0, 100);

                        for (int i = 0; i < 3; i++)
                        {
                            Point3D flashLoc = new Point3D(
                                target.X + Utility.RandomMinMax(-1, 1),
                                target.Y + Utility.RandomMinMax(-1, 1),
                                target.Z
                            );
                            Effects.SendLocationEffect(flashLoc, target.Map, 0x3818, 10, 8, effectHue, 0);
                        }

                        target.PlaySound(0x29);
                        target.SendMessage("A lightning bolt crashes down upon you!");
                    });
                }
            );
        }
    }

    public class CureSeriousWoundsSpell : CustomSpell
    {
        public CureSeriousWoundsSpell() : base("Cure Serious Wounds", 0x376A)
        {
            AddLevel(SpellType.Cleric, 5);
            AddLevel(SpellType.Druid, 5);
            AddLevel(SpellType.Bard, 6);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Heal);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            int heal = Utility.RandomMinMax(12, 22) + level;

            caster.Heal(heal);

            caster.FixedParticles(0x376A, 9, 32, 5030, hue, 0, EffectLayer.Waist);
            caster.PlaySound(0x202);

        }
    }

    public class SlayLivingSpell : CustomSpell
    {
        public SlayLivingSpell() : base("Slay Living", 0x22C5)
        {
            AddLevel(SpellType.Cleric, 5);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Offensive);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            Mobile target = caster.Combatant as Mobile;
            if (!SpellHelpers.isValidHostileTarget(caster, target))
                return;

            if (DeathSpellHelper.IsUndead(target))
                return;

            int effectHue = hue != 0 ? hue : 0x497;

            caster.FixedParticles(0x374A, 10, 15, 5013, effectHue, 0, EffectLayer.RightHand);
            caster.FixedParticles(0x3709, 9, 20, 5044, effectHue, 0, EffectLayer.Waist);
            caster.PlaySound(0x1FC);

            caster.MovingEffect(target, 0x36D4, 10, 0, false, false, effectHue, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(0.5), delegate()
            {
                if (!SpellHelpers.IsValidCaster(caster))
                    return;

                if (!SpellHelpers.isValidHostileTarget(caster, target))
                    return;

                caster.DoHarmful(target);

                target.FixedEffect(0x3709, 10, 30, effectHue, 0);
                target.FixedParticles(0x374A, 10, 15, 5013, effectHue, 0, EffectLayer.Waist);
                target.FixedParticles(0x36BD, 10, 20, 5044, effectHue, 0, EffectLayer.Head);
                target.PlaySound(0x211);

                double resist = target.Skills[SkillName.MagicResist].Value;
                int roll = Utility.Random(40 + level * 2);

                if (roll > resist)
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(i * 0.2), delegate()
                        {
                            if (target != null && !target.Deleted)
                            {
                                target.FixedParticles(0x374A, 10, 15, 5013, effectHue, 0, EffectLayer.Waist);
                                Effects.SendLocationEffect(target.Location, target.Map, 0x3709, 15, 10, effectHue, 0);
                            }
                        });
                    }

                    Timer.DelayCall(TimeSpan.FromSeconds(0.8), delegate()
                    {
                        if (target != null && !target.Deleted && target.Alive)
                        {
                            target.Kill();
                            target.SendMessage("Your life force is violently ripped from your body!");
                        }
                    });
                }
                else
                {
                    int damage = Utility.RandomMinMax(17, 22) + level;
                    AOS.Damage(target, caster, damage, 100, 0, 0, 0, 0);

                    target.FixedParticles(0x376A, 9, 20, 5044, effectHue, 0, EffectLayer.CenterFeet);
                    target.PlaySound(0x1F2);
                    target.SendMessage("You resist the death magic but suffer terrible pain!");
                }
            });
        }
    }
    public class SummonNatureAllyVSpell : SummonNatureAllySpell
    {
        public SummonNatureAllyVSpell()
            : base("Summon Nature's Ally V", 0x2156, 5, typeof(AnyElemental))
        {
        }
    }
    public class GreaterVigorSpell : CustomSpell
    {
        public GreaterVigorSpell() : base("Vigor, Greater", 0x376A)
        {
            AddLevel(SpellType.Cleric, 5);
            AddLevel(SpellType.Druid, 5);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Heal);
        }
    
        public override void Cast(Mobile caster, int hue, int level)
        {
            int duration = 12 + (level / 2);
            NatureSpellHelper.ApplyVigor(caster, 12, duration, level);
        }
    }

    public class WailOfDoomSpell : CustomSpell
    {
        public WailOfDoomSpell() : base("Wail of Doom", 0x482)
        {
            AddLevel(SpellType.Bard, 5);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.CC);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            int range = 4;
            int angle = 60;
            int damage = Utility.RandomMinMax(35, 40) + level;
            int effectHue = hue != 0 ? hue : 0x482;
            Direction dir = caster.Direction;

            caster.FixedParticles(0x374A, 10, 15, 5013, effectHue, 0, EffectLayer.Head);
            caster.FixedParticles(0x36BD, 10, 20, 5044, effectHue, 0, EffectLayer.Waist);
            caster.PlaySound(0x1FB);

            Effects.SendLocationEffect(caster.Location, caster.Map, 0x37C4, 30, 10, effectHue, 0);

            for (int i = 1; i <= 4; i++)
            {
                Point3D coneLoc = SpellHelpers.GetPointInDirection(caster, dir, i);
                Effects.SendLocationEffect(coneLoc, caster.Map, 0x3728, 20, 10, effectHue, 0);
            }

            Timer.DelayCall(TimeSpan.FromSeconds(0.3), delegate()
            {
                if (!SpellHelpers.IsValidCaster(caster))
                    return;

                IPooledEnumerable eable = caster.GetMobilesInRange(range);
                foreach (Mobile m in eable)
                {
                    if (!SpellHelpers.isValidHostileTarget(caster, m))
                        continue;

                    if (!InCone(caster, m, dir, angle))
                        continue;

                    caster.DoHarmful(m);

                    AOS.Damage(m, caster, damage, 0, 0, 0, 100, 0);

                    m.FixedEffect(0x36BD, 10, 25, effectHue, 0);
                    m.FixedParticles(0x374A, 10, 15, 5013, effectHue, 0, EffectLayer.Waist);
                    m.FixedParticles(0x3728, 8, 12, 5044, effectHue, 0, EffectLayer.Head);

                    int save = Utility.RandomMinMax(40, 52) + level * 2;
                    if (m.Int < save)
                    {
                        int stunDuration = 6 + level;
                        m.Paralyze(TimeSpan.FromSeconds(stunDuration));

                        for (int i = 1; i <= 3; i++)
                        {
                            Timer.DelayCall(TimeSpan.FromSeconds(i * 0.4), delegate()
                            {
                                if (!SpellHelpers.IsValidCaster(caster))
                                    return;

                                if (m != null && !m.Deleted && m.Alive)
                                {
                                    m.FixedParticles(0x374A, 5, 10, 5013, effectHue, 0, EffectLayer.Head);
                                    Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 8, effectHue, 0);
                                }
                            });
                        }

                        m.SendMessage("The wail of doom overwhelms your mind!");
                    }
                    else
                    {
                        m.SendMessage("You resist the terrifying wail!");
                    }

                    m.PlaySound(0x204);
                }
                eable.Free();
            });
        }

        private bool InCone(Mobile caster, Mobile target, Direction dir, int angle)
        {
            double dx = target.X - caster.X;
            double dy = target.Y - caster.Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            if (dist == 0)
                return true;

            double targetAngle = Math.Atan2(dy, dx) * 180 / Math.PI;
            double facingAngle = GetFacingAngle(dir);
            double diff = Math.Abs(NormalizeAngle(targetAngle - facingAngle));

            return diff <= angle / 2;
        }

        private double GetFacingAngle(Direction dir)
        {
            switch (dir & Direction.Mask)
            {
                case Direction.North: return -90;
                case Direction.Right: return 0;
                case Direction.East: return 0;
                case Direction.Down: return 90;
                case Direction.South: return 90;
                case Direction.Left: return 180;
                case Direction.West: return 180;
                case Direction.Up: return -90;
            }
            return 0;
        }

        private double NormalizeAngle(double angle)
        {
            while (angle < -180) angle += 360;
            while (angle > 180) angle -= 360;
            return angle;
        }
    }



    // ===== LEVEL 6 =====
    public class AcidFogSpell : CustomSpell
    {
        public AcidFogSpell() : base("Acid Fog", 0x36D4)
        {
            AddLevel(SpellType.Wizard, 6);
            AddLevel(SpellType.Sorcerer, 6);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.DoT);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            Mobile target = caster.Combatant as Mobile;
            if (!SpellHelpers.isValidHostileTarget(caster, target))
                return;

            int effectHue = hue != 0 ? hue : 0x48E;

            caster.FixedParticles(0x374A, 10, 15, 5013, effectHue, 0, EffectLayer.RightHand);
            caster.FixedParticles(0x36BD, 9, 20, 5044, effectHue, 0, EffectLayer.Waist);
            caster.PlaySound(0x231);

            Effects.SendLocationEffect(target.Location, target.Map, 0x3728, 30, 10, effectHue, 0);

            for (int i = 0; i < 4; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i * 0.2), delegate()
                {
                    if (!SpellHelpers.IsValidCaster(caster))
                        return;

                    Point3D fogLoc = new Point3D(
                        target.X + Utility.RandomMinMax(-2, 2),
                        target.Y + Utility.RandomMinMax(-2, 2),
                        target.Z
                    );
                    Effects.SendLocationEffect(fogLoc, target.Map, 0x3728, 20, 10, effectHue, 0);
                });
            }

            FieldSpellHelper.SpawnField(
                caster,
                target.Location,
                target.Map,
                6,
                1,
                delegate { return new AcidFogTile(caster, level, effectHue); }
            );
        }
    }

    public class AcidFogTile : Item
    {
        private Mobile m_Caster;
        private int m_Level;
        private int m_Hue;
        private Timer m_TickTimer;

        public AcidFogTile(Mobile caster, int level, int hue) : base(0x398C)
        {
            Movable = false;
            Hue = hue;
            m_Caster = caster;
            m_Level = level;
            m_Hue = hue;

            m_TickTimer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1.0), Tick);
            Timer.DelayCall(TimeSpan.FromSeconds(2 + level), Delete);
        }

        private void Tick()
        {
            if (Deleted || m_Caster == null || m_Caster.Deleted)
            {
                if (m_TickTimer != null)
                    m_TickTimer.Stop();
                return;
            }

            Effects.SendLocationEffect(Location, Map, 0x3728, 10, 10, m_Hue, 0);

            IPooledEnumerable eable = GetMobilesInRange(0);
            foreach (Mobile m in eable)
            {
                if (!SpellHelpers.isValidHostileTarget(m_Caster, m))
                    continue;

                m_Caster.DoHarmful(m);

                int dmg = Utility.RandomMinMax(28, 33) + m_Level;
                AOS.Damage(m, m_Caster, dmg, 0, 0, 0, 100, 0);

                m.FixedEffect(0x36BD, 10, 20, m_Hue, 0);
                m.FixedParticles(0x374A, 8, 12, 5013, m_Hue, 0, EffectLayer.Waist);
                m.FixedParticles(0x36BD, 6, 10, 5044, m_Hue, 0, EffectLayer.CenterFeet);

                m.PlaySound(0x231);
                m.SendMessage("Corrosive acid fog burns your flesh!");
            }
            eable.Free();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_TickTimer != null)
            {
                m_TickTimer.Stop();
                m_TickTimer = null;
            }

            Effects.SendLocationEffect(Location, Map, 0x3728, 15, 10, m_Hue, 0);
        }

        public AcidFogTile(Serial serial) : base(serial) { }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }
    }

    public class MassBearsEnduranceSpell : CustomSpell
    {
        public MassBearsEnduranceSpell() : base("Mass Bear's Endurance", 0x480)
        {
            AddLevel(SpellType.Wizard, 6);
            AddLevel(SpellType.Sorcerer, 6);
            AddLevel(SpellType.Druid, 6);
            AddLevel(SpellType.Cleric, 6);
            AddTag(SpellTag.Buff);
            AddTag(SpellTag.AoE);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            int bonusHP = 45 + level * 2;
            int strBonus = Math.Max(1, bonusHP / 2);
            int effectHue = hue != 0 ? hue : 0x21;

            caster.FixedParticles(0x375A, 10, 15, 5018, effectHue, 0, EffectLayer.Waist);
            caster.FixedParticles(0x376A, 9, 20, 5044, effectHue, 0, EffectLayer.Head);
            caster.PlaySound(0x1EA);

            Effects.SendLocationEffect(caster.Location, caster.Map, 0x375A, 30, 10, effectHue, 0);

            for (int i = 1; i <= 3; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i * 0.2), delegate()
                {
                    if (!SpellHelpers.IsValidCaster(caster))
                        return;

                    Effects.SendLocationEffect(caster.Location, caster.Map, 0x376A, 20, 10, effectHue, 0);
                });
            }

            IPooledEnumerable eable = caster.GetMobilesInRange(6);
            foreach (Mobile m in eable)
            {
                if (m == null || m.Deleted || !m.Alive)
                    continue;

                if (SpellHelpers.isValidHostileTarget(caster, m))
                    continue;

                caster.DoBeneficial(m);

                m.AddStatMod(new StatMod(StatType.Str, "MassBearsEndurance", strBonus, TimeSpan.FromSeconds(60)));
                m.Hits += bonusHP;

                m.FixedEffect(0x376A, 10, 20, effectHue, 0);
                m.FixedParticles(0x373A, 10, 15, 5018, effectHue, 0, EffectLayer.Waist);
                m.FixedParticles(0x375A, 8, 12, 5044, effectHue, 0, EffectLayer.Head);
                m.FixedParticles(0x376A, 9, 20, 5008, effectHue, 0, EffectLayer.CenterFeet);

                for (int i = 1; i <= 2; i++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(i * 0.3), delegate()
                    {
                        if (!SpellHelpers.IsValidCaster(caster))
                            return;

                        if (m != null && !m.Deleted && m.Alive)
                        {
                            m.FixedParticles(0x373A, 5, 10, 5018, effectHue, 0, EffectLayer.Waist);
                            Effects.SendLocationEffect(m.Location, m.Map, 0x375A, 10, 8, effectHue, 0);
                        }
                    });
                }

                m.PlaySound(0x1EA);
            }
            eable.Free();
        }
    }

   public class GreaterBestowCurseSpell : CustomSpell
    {
        public GreaterBestowCurseSpell() : base("Bestow Curse, Greater", 0x455)
        {
            AddLevel(SpellType.Bard, 6);
            AddLevel(SpellType.Cleric, 7);
            AddLevel(SpellType.Wizard, 8);
            AddLevel(SpellType.Sorcerer, 8);
            AddTag(SpellTag.Debuff);
            AddTag(SpellTag.SingleTarget);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            Mobile target = caster.Combatant as Mobile;
            if (!SpellHelpers.isValidHostileTarget(caster, target))
                return;

            int effectHue = hue != 0 ? hue : 0x455;

            caster.FixedParticles(0x374A, 10, 15, 5013, effectHue, 0, EffectLayer.RightHand);
            caster.FixedParticles(0x3728, 9, 20, 5044, effectHue, 0, EffectLayer.Waist);
            caster.PlaySound(0x1FC);

            caster.MovingEffect(target, 0x374A, 10, 0, false, false, effectHue, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(0.5), delegate()
            {
                if (!SpellHelpers.IsValidCaster(caster))
                    return;

                if (!SpellHelpers.isValidHostileTarget(caster, target))
                    return;

                caster.DoHarmful(target);

                int strLoss = (int)(target.RawStr * (0.24 + level * 0.02));
                TimeSpan dur = TimeSpan.FromSeconds(30 + level);

                target.AddStatMod(new StatMod(StatType.Str, "GreaterBestowCurse", -strLoss, dur));

                target.FixedEffect(0x3728, 10, 20, effectHue, 0);
                target.FixedParticles(0x374A, 10, 15, 5013, effectHue, 0, EffectLayer.Waist);
                target.FixedParticles(0x3728, 10, 20, 5044, effectHue, 0, EffectLayer.Head);
                target.FixedParticles(0x36BD, 8, 12, 5013, effectHue, 0, EffectLayer.CenterFeet);

                for (int i = 1; i <= 4; i++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(i * 0.4), delegate()
                    {
                        if (!SpellHelpers.IsValidCaster(caster))
                            return;

                        if (target != null && !target.Deleted && target.Alive)
                        {
                            target.FixedParticles(0x374A, 5, 10, 5013, effectHue, 0, EffectLayer.Waist);
                            Effects.SendLocationEffect(target.Location, target.Map, 0x3728, 10, 8, effectHue, 0);
                        }
                    });
                }

                target.PlaySound(0x1F8);
                target.SendMessage("A terrible curse weakens your body!");
            });
        }
    }


    public class BullsStrengthMassSpell : CustomSpell
    {
        public BullsStrengthMassSpell() : base("Bull's Strength, Mass", 0x375A)
        {
            AddLevel(SpellType.Cleric, 6);
            AddLevel(SpellType.Druid, 6);
            AddLevel(SpellType.Wizard, 6);
            AddLevel(SpellType.Sorcerer, 6);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Buff);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            int bonus = 30 + level * 2;
            int effectHue = hue != 0 ? hue : 0x21;

            caster.FixedParticles(0x375A, 10, 15, 5018, effectHue, 0, EffectLayer.Waist);
            caster.FixedParticles(0x373A, 10, 20, 5044, effectHue, 0, EffectLayer.Head);
            caster.PlaySound(0x1E9);

            Effects.SendLocationEffect(caster.Location, caster.Map, 0x375A, 30, 10, effectHue, 0);

            for (int i = 1; i <= 3; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i * 0.2), delegate()
                {
                    if (!SpellHelpers.IsValidCaster(caster))
                        return;

                    Effects.SendLocationEffect(caster.Location, caster.Map, 0x373A, 20, 10, effectHue, 0);
                });
            }

            IPooledEnumerable eable = caster.GetMobilesInRange(6);
            foreach (Mobile m in eable)
            {
                if (m == null || m.Deleted || !m.Alive)
                    continue;

                if (SpellHelpers.isValidHostileTarget(caster, m))
                    continue;

                caster.DoBeneficial(m);

                m.RawStr += bonus;

                Timer.DelayCall(TimeSpan.FromMinutes(1), delegate
                {
                    if (m != null && !m.Deleted)
                        m.RawStr -= bonus;
                });

                m.FixedEffect(0x375A, 10, 20, effectHue, 0);
                m.FixedParticles(0x373A, 10, 15, 5018, effectHue, 0, EffectLayer.Waist);
                m.FixedParticles(0x376A, 9, 20, 5044, effectHue, 0, EffectLayer.LeftHand);
                m.FixedParticles(0x376A, 9, 20, 5044, effectHue, 0, EffectLayer.RightHand);

                for (int i = 1; i <= 2; i++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(i * 0.3), delegate()
                    {
                        if (!SpellHelpers.IsValidCaster(caster))
                            return;

                        if (m != null && !m.Deleted && m.Alive)
                        {
                            m.FixedParticles(0x373A, 5, 10, 5018, effectHue, 0, EffectLayer.Waist);
                            Effects.SendLocationEffect(m.Location, m.Map, 0x375A, 10, 8, effectHue, 0);
                        }
                    });
                }

                m.PlaySound(0x1E9);
            }
            eable.Free();
        }
    }

    public class CacophonicShieldSpell : CustomSpell
    {
        public CacophonicShieldSpell() : base("Cacophonic Shield", 0x481)
        {
            AddLevel(SpellType.Bard, 6);
            AddLevel(SpellType.Wizard, 7);
            AddLevel(SpellType.Sorcerer, 7);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Buff);
            AddTag(SpellTag.DoT);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            int resist = 6 + level;
            TimeSpan dur = TimeSpan.FromSeconds(21 + level);
            int damage = Utility.RandomMinMax(28, 33);
            int effectHue = hue != 0 ? hue : 0x481;

            ResistanceMod mod = new ResistanceMod(ResistanceType.Energy, resist);
            caster.AddResistanceMod(mod);

            caster.FixedParticles(0x375A, 10, 15, 5012, effectHue, 0, EffectLayer.Waist);
            caster.FixedParticles(0x376A, 9, 20, 5044, effectHue, 0, EffectLayer.Head);
            caster.FixedParticles(0x37C4, 10, 15, 5018, effectHue, 0, EffectLayer.CenterFeet);
            caster.PlaySound(0x1F7);

            Effects.SendLocationEffect(caster.Location, caster.Map, 0x37C4, 25, 10, effectHue, 0);

            int ticks = (int)(dur.TotalSeconds / 2);

            Timer.DelayCall(
                TimeSpan.Zero,
                TimeSpan.FromSeconds(2),
                ticks,
                delegate
                {
                    if (!SpellHelpers.IsValidCaster(caster))
                        return;

                    caster.FixedParticles(0x37C4, 5, 10, 5012, effectHue, 0, EffectLayer.Waist);
                    Effects.SendLocationEffect(caster.Location, caster.Map, 0x37C4, 15, 10, effectHue, 0);

                    SpellHelpers.ForEachHostileInRange(caster, caster, 1, delegate(Mobile m)
                    {
                        caster.DoHarmful(m);

                        AOS.Damage(m, caster, damage, 0, 0, 0, 100, 0);

                        m.FixedParticles(0x37C4, 10, 15, 5044, effectHue, 0, EffectLayer.Waist);
                        m.FixedParticles(0x3728, 8, 12, 5013, effectHue, 0, EffectLayer.Head);

                        m.PlaySound(0x2F3);
                        m.SendMessage("The cacophonic shield's discord strikes you!");
                    });
                }
            );

            Timer.DelayCall(dur, delegate()
            {
                if (caster != null && !caster.Deleted)
                {
                    caster.RemoveResistanceMod(mod);
                    Effects.SendLocationEffect(caster.Location, caster.Map, 0x37C4, 20, 10, effectHue, 0);
                }
            });
        }
    }


    public class CatsGraceMassSpell : CustomSpell
    {
        public CatsGraceMassSpell() : base("Cat's Grace, Mass", 0x375A)
        {
            AddLevel(SpellType.Bard, 6);
            AddLevel(SpellType.Druid, 6);
            AddLevel(SpellType.Wizard, 6);
            AddLevel(SpellType.Sorcerer, 6);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Buff);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            int bonus = 30 + level * 2;
            int effectHue = hue != 0 ? hue : 0x47E;

            caster.FixedParticles(0x375A, 10, 15, 5018, effectHue, 0, EffectLayer.Waist);
            caster.FixedParticles(0x376A, 9, 20, 5044, effectHue, 0, EffectLayer.Head);
            caster.PlaySound(0x1E9);

            Effects.SendLocationEffect(caster.Location, caster.Map, 0x375A, 30, 10, effectHue, 0);

            for (int i = 1; i <= 3; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i * 0.15), delegate()
                {
                    if (!SpellHelpers.IsValidCaster(caster))
                        return;

                    Effects.SendLocationEffect(caster.Location, caster.Map, 0x376A, 20, 10, effectHue, 0);
                });
            }

            IPooledEnumerable eable = caster.GetMobilesInRange(6);
            foreach (Mobile m in eable)
            {
                if (m == null || m.Deleted || !m.Alive)
                    continue;

                if (SpellHelpers.isValidHostileTarget(caster, m))
                    continue;

                caster.DoBeneficial(m);

                m.RawDex += bonus;

                Timer.DelayCall(TimeSpan.FromMinutes(1), delegate
                {
                    if (m != null && !m.Deleted)
                        m.RawDex -= bonus;
                });

                m.FixedEffect(0x375A, 10, 20, effectHue, 0);
                m.FixedParticles(0x376A, 10, 15, 5008, effectHue, 0, EffectLayer.Waist);
                m.FixedParticles(0x373A, 9, 20, 5036, effectHue, 0, EffectLayer.Head);
                m.FixedParticles(0x375A, 8, 12, 5044, effectHue, 0, EffectLayer.LeftHand);
                m.FixedParticles(0x375A, 8, 12, 5044, effectHue, 0, EffectLayer.RightHand);

                for (int i = 1; i <= 2; i++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(i * 0.25), delegate()
                    {
                        if (!SpellHelpers.IsValidCaster(caster))
                            return;

                        if (m != null && !m.Deleted && m.Alive)
                        {
                            m.FixedParticles(0x376A, 5, 10, 5008, effectHue, 0, EffectLayer.Waist);
                            Effects.SendLocationEffect(m.Location, m.Map, 0x375A, 10, 8, effectHue, 0);
                        }
                    });
                }

                m.PlaySound(0x1E9);
            }
            eable.Free();
        }
    }

    public class ChainLightningSpell : CustomSpell
    {
        public ChainLightningSpell() : base("Chain Lightning", 0x379F)
        {
            AddLevel(SpellType.Wizard, 6);
            AddLevel(SpellType.Sorcerer, 6);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            int damage = Utility.RandomMinMax(32, 37) + level;
            int effectHue = hue != 0 ? hue : 0x480;

            caster.FixedParticles(0x3818, 10, 15, 5052, effectHue, 0, EffectLayer.RightHand);
            caster.FixedParticles(0x3818, 10, 15, 5052, effectHue, 0, EffectLayer.LeftHand);
            caster.PlaySound(0x29);

            Effects.SendLocationEffect(caster.Location, caster.Map, 0x3818, 30, 10, effectHue, 0);

            List<Mobile> targets = new List<Mobile>();
            IPooledEnumerable eable = caster.GetMobilesInRange(3);
            foreach (Mobile m in eable)
            {
                if (SpellHelpers.isValidHostileTarget(caster, m))
                    targets.Add(m);
            }
            eable.Free();

            if (targets.Count == 0)
                return;

            Mobile lastTarget = caster;

            for (int i = 0; i < targets.Count; i++)
            {
                Mobile currentTarget = targets[i];
                Mobile chainFrom = lastTarget;

                Timer.DelayCall(TimeSpan.FromSeconds(i * 0.2), delegate()
                {
                    if (!SpellHelpers.IsValidCaster(caster))
                        return;

                    if (!SpellHelpers.isValidHostileTarget(caster, currentTarget))
                        return;

                    if (chainFrom != null && !chainFrom.Deleted)
                    {
                        chainFrom.MovingEffect(currentTarget, 0x3818, 7, 0, false, false, effectHue, 0);
                    }

                    Timer.DelayCall(TimeSpan.FromSeconds(0.3), delegate()
                    {
                        if (!SpellHelpers.IsValidCaster(caster))
                            return;

                        if (!SpellHelpers.isValidHostileTarget(caster, currentTarget))
                            return;

                        caster.DoHarmful(currentTarget);

                        currentTarget.BoltEffect(effectHue);
                        currentTarget.FixedParticles(0x3818, 10, 15, 5044, effectHue, 0, EffectLayer.Waist);
                        currentTarget.FixedParticles(0x374A, 8, 12, 5013, effectHue, 0, EffectLayer.Head);

                        AOS.Damage(currentTarget, caster, damage, 0, 0, 0, 0, 100);

                        for (int j = 0; j < 2; j++)
                        {
                            Point3D sparkLoc = new Point3D(
                                currentTarget.X + Utility.RandomMinMax(-1, 1),
                                currentTarget.Y + Utility.RandomMinMax(-1, 1),
                                currentTarget.Z
                            );
                            Effects.SendLocationEffect(sparkLoc, currentTarget.Map, 0x3818, 10, 8, effectHue, 0);
                        }

                        currentTarget.PlaySound(0x28);
                        currentTarget.SendMessage("Chain lightning surges through your body!");
                    });
                });

                lastTarget = currentTarget;
            }
        }
    }

    public class CometfallSpell : CustomSpell
    {
        public CometfallSpell() : base("Cometfall", 1160)
        {
            AddLevel(SpellType.Cleric, 6);
            AddLevel(SpellType.Druid, 6);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.CC);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            Mobile target = caster.Combatant as Mobile;
            if (!SpellHelpers.isValidHostileTarget(caster, target))
                return;

            int damage = Utility.RandomMinMax(30, 35) + level;
            int effectHue = hue != 0 ? hue : 1160;

            caster.FixedParticles(0x375A, 10, 15, 5018, effectHue, 0, EffectLayer.RightHand);
            caster.FixedParticles(0x36BD, 9, 20, 5044, effectHue, 0, EffectLayer.Waist);
            caster.PlaySound(0x5C);

            Point3D above = new Point3D(target.X, target.Y, target.Z + 30);
            Effects.SendLocationEffect(above, target.Map, 0x36CB, 30, 10, effectHue, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(0.8), delegate()
            {
                if (!SpellHelpers.IsValidCaster(caster))
                    return;

                if (!SpellHelpers.isValidHostileTarget(caster, target))
                    return;

                caster.DoHarmful(target);

                target.FixedParticles(0x36BD, 20, 10, 5044, effectHue, 3, EffectLayer.Head);
                target.FixedParticles(0x36CB, 15, 15, 5052, effectHue, 0, EffectLayer.Waist);
                target.FixedParticles(0x3709, 10, 20, 5044, effectHue, 0, EffectLayer.CenterFeet);

                Effects.SendLocationEffect(target.Location, target.Map, 0x36BD, 25, 10, effectHue, 0);

                for (int i = 0; i < 6; i++)
                {
                    Point3D debrisLoc = new Point3D(
                        target.X + Utility.RandomMinMax(-2, 2),
                        target.Y + Utility.RandomMinMax(-2, 2),
                        target.Z
                    );
                    Effects.SendLocationEffect(debrisLoc, target.Map, 0x36CB, 15, 10, effectHue, 0);
                }

                AOS.Damage(target, caster, damage, 50, 0, 50, 0, 0);

                target.PlaySound(0x160);

                int save = Utility.RandomMinMax(20, 60) + level;
                if (target.Dex < save)
                {
                    DoKnockback(target, caster, 3, effectHue);

                    target.Paralyze(TimeSpan.FromSeconds(4));

                    for (int i = 1; i <= 3; i++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(i * 0.4), delegate()
                        {
                            if (!SpellHelpers.IsValidCaster(caster))
                                return;

                            if (target != null && !target.Deleted && target.Alive)
                            {
                                target.FixedParticles(0x36BD, 5, 10, 5044, effectHue, 0, EffectLayer.Waist);
                                Effects.SendLocationEffect(target.Location, target.Map, 0x3728, 10, 8, effectHue, 0);
                            }
                        });
                    }

                    target.SendMessage("The comet's impact sends you flying!");
                }
                else
                {
                    target.SendMessage("You brace yourself against the comet's impact!");
                }
            });
        }

        private void DoKnockback(Mobile target, Mobile caster, int tiles, int hue)
        {
            Direction d = caster.GetDirectionTo(target);
            Point3D loc = target.Location;
            Map map = target.Map;

            for (int i = 0; i < tiles; i++)
            {
                Point3D next = GetOffset(loc, d);
                if (map == null || !map.CanFit(next, 16, false, false))
                    break;

                Effects.SendLocationEffect(loc, map, 0x3728, 10, 10, hue, 0);
                loc = next;
            }

            target.MoveToWorld(loc, map);
            Effects.SendLocationEffect(loc, map, 0x36BD, 15, 10, hue, 0);
        }

        private Point3D GetOffset(Point3D p, Direction d)
        {
            switch (d)
            {
                case Direction.North: return new Point3D(p.X, p.Y - 1, p.Z);
                case Direction.South: return new Point3D(p.X, p.Y + 1, p.Z);
                case Direction.West: return new Point3D(p.X - 1, p.Y, p.Z);
                case Direction.East: return new Point3D(p.X + 1, p.Y, p.Z);
                case Direction.Up: return new Point3D(p.X - 1, p.Y - 1, p.Z);
                case Direction.Down: return new Point3D(p.X + 1, p.Y + 1, p.Z);
                case Direction.Left: return new Point3D(p.X - 1, p.Y + 1, p.Z);
                case Direction.Right: return new Point3D(p.X + 1, p.Y - 1, p.Z);
            }
            return p;
        }
    }

    public class DisintegrateSpell : CustomSpell
    {
        public DisintegrateSpell() : base("Disintegrate", 0x36D4)
        {
            AddLevel(SpellType.Wizard, 6);
            AddLevel(SpellType.Sorcerer, 6);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Offensive);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            Mobile target = caster.Combatant as Mobile;
            if (!SpellHelpers.isValidHostileTarget(caster, target))
                return;

            int effectHue = hue != 0 ? hue : 0x47E;

            caster.FixedParticles(0x374A, 10, 15, 5013, effectHue, 0, EffectLayer.RightHand);
            caster.FixedParticles(0x36D4, 9, 20, 5044, effectHue, 0, EffectLayer.Waist);
            caster.PlaySound(0x1FC);

            Effects.SendMovingEffect(caster, target, 0x36D4, 10, 0, false, false, effectHue, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(0.5), delegate()
            {
                if (!SpellHelpers.IsValidCaster(caster))
                    return;

                if (!SpellHelpers.isValidHostileTarget(caster, target))
                    return;

                caster.DoHarmful(target);

                bool weakened = target.Hits < (target.HitsMax / 2);
                int damage = weakened
                    ? Utility.RandomMinMax(70, 80) + level * 2
                    : Utility.RandomMinMax(35, 40) + level * 2;

                target.FixedEffect(0x3709, 10, 30, effectHue, 0);
                target.FixedParticles(0x36D4, 10, 20, 5044, effectHue, 0, EffectLayer.Waist);
                target.FixedParticles(0x374A, 10, 15, 5013, effectHue, 0, EffectLayer.Head);
                target.FixedParticles(0x36BD, 8, 12, 5013, effectHue, 0, EffectLayer.CenterFeet);

                Effects.SendLocationEffect(target.Location, target.Map, 0x36D4, 20, 10, effectHue, 0);

                AOS.Damage(target, caster, damage, 100, 0, 0, 0, 0);

                if (weakened)
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(i * 0.2), delegate()
                        {
                            if (!SpellHelpers.IsValidCaster(caster))
                                return;

                            if (target != null && !target.Deleted && target.Alive)
                            {
                                target.FixedParticles(0x36D4, 5, 10, 5013, effectHue, 0, EffectLayer.Waist);
                                Effects.SendLocationEffect(target.Location, target.Map, 0x3709, 10, 8, effectHue, 0);
                            }
                        });
                    }
                    target.SendMessage("The disintegration ray tears through your weakened body!");
                }
                else
                {
                    for (int i = 1; i <= 2; i++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(i * 0.3), delegate()
                        {
                            if (!SpellHelpers.IsValidCaster(caster))
                                return;

                            if (target != null && !target.Deleted && target.Alive)
                            {
                                target.FixedParticles(0x36D4, 5, 10, 5013, effectHue, 0, EffectLayer.Waist);
                            }
                        });
                    }
                    target.SendMessage("A disintegration ray strikes you!");
                }

                target.PlaySound(0x211);
            });
        }
    }

    public class GreaterShoutSpell : ShoutSpell
    {
        public GreaterShoutSpell() : base()
        {
            AddLevel(SpellType.Wizard, 8);
            AddLevel(SpellType.Sorcerer, 8);
            AddLevel(SpellType.Bard, 6);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
        }
    
        public override void Cast(Mobile caster, int hue, int level)
        {
            int range = 9;
            int min = 32;
            int max = 37;
           SpellHelpers.DoConeDamage(caster, range, min, max, level,50,0,0,0,50,0x375A,hue != 0 ? hue : 0x480, "A powerful scream pierces your ears!");
        }
    }

    public class MassVigorSpell : CustomSpell
    {
        public MassVigorSpell() : base("Mass Vigor", 0x376A)
        {
            AddLevel(SpellType.Cleric, 6);
            AddLevel(SpellType.Druid, 6);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Heal);
            AddTag(SpellTag.HoT);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            int duration = 9 + (level / 2);

            foreach (Mobile m in caster.GetMobilesInRange(2))
            {
                if(SpellHelpers.isValidHostileTarget(caster, m))
                    continue;
                NatureSpellHelper.ApplyVigor(m, 6, duration, level);
            }

            caster.PlaySound(0x202);
        }
    }

    public class OwlsWisdomMassSpell : CustomSpell
    {
        public OwlsWisdomMassSpell() : base("Owl's Wisdom, Mass", 0x375A)
        {
            AddLevel(SpellType.Cleric, 6);
            AddLevel(SpellType.Druid, 6);
            AddLevel(SpellType.Wizard, 6);
            AddLevel(SpellType.Sorcerer, 6);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Buff);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            int bonus = 30 + level * 2;
            int effectHue = hue != 0 ? hue : 0x480;

            caster.FixedParticles(0x375A, 10, 15, 5018, effectHue, 0, EffectLayer.Waist);
            caster.FixedParticles(0x376A, 10, 20, 5044, effectHue, 0, EffectLayer.Head);
            caster.FixedParticles(0x373A, 9, 15, 5008, effectHue, 0, EffectLayer.Head);
            caster.PlaySound(0x1E9);

            Effects.SendLocationEffect(caster.Location, caster.Map, 0x375A, 30, 10, effectHue, 0);

            for (int i = 1; i <= 3; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i * 0.2), delegate()
                {
                    if (!SpellHelpers.IsValidCaster(caster))
                        return;

                    Effects.SendLocationEffect(caster.Location, caster.Map, 0x376A, 20, 10, effectHue, 0);
                });
            }

            IPooledEnumerable eable = caster.GetMobilesInRange(6);
            foreach (Mobile m in eable)
            {
                if (m == null || m.Deleted || !m.Alive)
                    continue;

                if (SpellHelpers.isValidHostileTarget(caster, m))
                    continue;

                caster.DoBeneficial(m);

                m.RawInt += bonus;

                Timer.DelayCall(TimeSpan.FromMinutes(1), delegate
                {
                    if (m != null && !m.Deleted)
                        m.RawInt -= bonus;
                });

                m.FixedEffect(0x375A, 10, 20, effectHue, 0);
                m.FixedParticles(0x376A, 10, 15, 5008, effectHue, 0, EffectLayer.Waist);
                m.FixedParticles(0x373A, 10, 20, 5018, effectHue, 0, EffectLayer.Head);
                m.FixedParticles(0x375A, 8, 12, 5044, effectHue, 0, EffectLayer.Head);

                for (int i = 1; i <= 2; i++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(i * 0.3), delegate()
                    {
                        if (!SpellHelpers.IsValidCaster(caster))
                            return;

                        if (m != null && !m.Deleted && m.Alive)
                        {
                            m.FixedParticles(0x376A, 5, 10, 5008, effectHue, 0, EffectLayer.Head);
                            Effects.SendLocationEffect(m.Location, m.Map, 0x375A, 10, 8, effectHue, 0);
                        }
                    });
                }

                m.PlaySound(0x1E9);
            }
            eable.Free();
        }
    }


    public class HealSpell : CustomSpell
    {
        public HealSpell() : base("Heal", 0x376A)
        {
            AddLevel(SpellType.Cleric, 6);
            AddLevel(SpellType.Druid, 7);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Heal);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            int heal = level * 12;

            caster.Heal(heal);
        
            caster.FixedParticles(0x376A, 9, 32, 5030, hue != 0 ? hue : 0x376A, 0, EffectLayer.Waist);
            caster.PlaySound(0x202);
        }
    }
    public class SummonNatureAllyVISpell : SummonNatureAllySpell
    {
        public SummonNatureAllyVISpell()
            : base("Summon Nature's Ally VI", 0x2156, 6, typeof(AnyGemElemental))
        {
        }
    }

    public class MissileStormSpell: CustomSpell
    {
        public MissileStormSpell(): base("Missile Storm", 0x36E4)
        {
            AddLevel(SpellType.Wizard, 6);
            AddLevel(SpellType.Sorcerer,6);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Offensive);
        }
        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
            if(!SpellHelpers.isValidHostileTarget(caster,target))
                return;

            caster.DoHarmful(target);
            
            int missiles = (level == 9) ? 10 : (level == 8) ? 8 : (level == 7) ? 7 : 6;

            for (int i = 0; i < missiles; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i * 0.3), delegate ()
                {
                    if (target == null || !target.Alive || target.Deleted)
                        return;

                    Effects.SendMovingEffect(
                        caster, 
                        target, 
                        0x379F, 
                        7, 
                        0, 
                        false, 
                        false, 
                        hue != 0 ? hue : 0x0213, 
                        0
                    );
                    Effects.PlaySound(caster.Location, caster.Map, 0x1F5);

                    Timer.DelayCall(TimeSpan.FromSeconds(0.5), delegate ()
                    {
                        if (target == null || !target.Alive || target.Deleted)
                            return;

                        int damage = Utility.RandomMinMax(6, 9) + level;
                        AOS.Damage(target, caster, damage, 0, 0, 0, 0, 100);
                        
                        Effects.SendLocationEffect(
                            target.Location, 
                            target.Map, 
                            0x3709, 
                            10, 
                            30, 
                            hue != 0 ? hue : 0x0213, 
                            0
                        );
                        target.FixedParticles(0x36BD, 6, 10, 5044, hue != 0 ? hue : 0x0213, 0, EffectLayer.Waist);
                    });
                });
            }
        }
    }

    // ===== LEVEL 7 =====
    public class AuraOfColdGreaterSpell : AuraOfColdSpell
    {
        public AuraOfColdGreaterSpell()
            : base("Aura of Cold, Greater", 0x480, 32, 37)
        {
            AddLevel(SpellType.Cleric, 7);
            AddLevel(SpellType.Druid, 7);
        }
    }

    public class HoldPersonMassSpell : CustomSpell
    {
        public HoldPersonMassSpell() : base("Hold Person, Mass", 0x376A)
        {
            AddLevel(SpellType.Wizard, 7);
            AddLevel(SpellType.Sorcerer, 7);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.CC);
        }   

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return; 

            Mobile center = caster.Combatant as Mobile;
            if (!SpellHelpers.isValidHostileTarget(caster, center))
                return; 

            int effectHue = hue != 0 ? hue : 0x0;   

            caster.FixedParticles(0x376A, 10, 15, 5030, effectHue, 0, EffectLayer.RightHand);
            caster.FixedParticles(0x3779, 10, 20, 5009, effectHue, 0, EffectLayer.Waist);
            caster.PlaySound(0x204);    

            Effects.SendLocationEffect(center.Location, center.Map, 0x376A, 30, 10, effectHue, 0);  

            for (int i = 1; i <= 3; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i * 0.2), delegate()
                {
                    if (!SpellHelpers.IsValidCaster(caster))
                        return; 

                    if (center != null && !center.Deleted)
                    {
                        Effects.SendLocationEffect(center.Location, center.Map, 0x3779, 20, 10, effectHue, 0);
                    }
                });
            }   

            Timer.DelayCall(TimeSpan.FromSeconds(0.4), delegate()
            {
                if (!SpellHelpers.IsValidCaster(caster))
                    return; 

                SpellHelpers.ForEachHostileInRange(center, caster, 4, delegate(Mobile m)
                {
                    caster.DoHarmful(m);    

                    double resist = m.Skills[SkillName.MagicResist].Value;
                    int duration = (int)(6 + level - resist / 10);  

                    m.FixedEffect(0x376A, 10, 30, effectHue, 0);
                    m.FixedParticles(0x3779, 10, 15, 5009, effectHue, 0, EffectLayer.Waist);
                    m.FixedParticles(0x375A, 10, 20, 5044, effectHue, 0, EffectLayer.Head);
                    m.FixedParticles(0x376A, 8, 12, 5030, effectHue, 0, EffectLayer.LeftHand);
                    m.FixedParticles(0x376A, 8, 12, 5030, effectHue, 0, EffectLayer.RightHand); 

                    if (duration > 0)
                    {
                        m.Paralyze(TimeSpan.FromSeconds(duration)); 

                        for (int i = 1; i <= 3; i++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(i * 0.5), delegate()
                        {
                            if (!SpellHelpers.IsValidCaster(caster))
                                return;

                            if (m != null && !m.Deleted && m.Alive)
                            {
                                m.FixedParticles(0x3779, 5, 10, 5009, effectHue, 0, EffectLayer.Waist);
                                Effects.SendLocationEffect(m.Location, m.Map, 0x376A, 10, 8, effectHue, 0);
                            }
                        });
                    }

                    m.SendMessage("Magical bonds freeze you completely in place!");
                }
                else
                {
                    m.FixedParticles(0x373A, 10, 15, 5036, effectHue, 0, EffectLayer.Waist);
                    m.SendMessage("You resist the paralyzing magic!");
                }

                m.PlaySound(0x204);
            });
        });
    }
}

    public class FingerOfDeathSpell : CustomSpell
    {
        public FingerOfDeathSpell() : base("Finger of Death", 0x22C5)
        {
            AddLevel(SpellType.Druid, 8);
            AddLevel(SpellType.Sorcerer, 7);
            AddLevel(SpellType.Wizard, 7);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Offensive);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            Mobile target = caster.Combatant as Mobile;
            if (!SpellHelpers.isValidHostileTarget(caster, target))
                return;

            if (DeathSpellHelper.IsUndead(target))
                return;

            int effectHue = hue != 0 ? hue : 0x497;

            caster.FixedParticles(0x374A, 10, 15, 5013, effectHue, 0, EffectLayer.RightHand);
            caster.FixedParticles(0x3709, 10, 20, 5044, effectHue, 0, EffectLayer.Waist);
            caster.PlaySound(0x1FC);

            Point3D handLoc = new Point3D(caster.X, caster.Y, caster.Z + 5);
            Effects.SendLocationEffect(handLoc, caster.Map, 0x374A, 20, 10, effectHue, 0);

            caster.MovingEffect(target, 0x36D4, 10, 0, false, false, effectHue, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(0.6), delegate()
            {
                if (!SpellHelpers.IsValidCaster(caster))
                    return;

                if (!SpellHelpers.isValidHostileTarget(caster, target))
                    return;

                caster.DoHarmful(target);

                target.FixedParticles(0x374A, 10, 15, 5013, effectHue, 0, EffectLayer.Waist);
                target.FixedParticles(0x3709, 10, 20, 5044, effectHue, 0, EffectLayer.Head);
                target.FixedParticles(0x36BD, 8, 12, 5013, effectHue, 0, EffectLayer.CenterFeet);

                Effects.SendLocationEffect(target.Location, target.Map, 0x3709, 20, 10, effectHue, 0);

                int roll = Utility.Random(50 + level * 2);
                double resist = target.Skills[SkillName.MagicResist].Value;

                if (roll > resist)
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(i * 0.25), delegate()
                        {
                            if (target != null && !target.Deleted)
                            {
                                target.FixedParticles(0x374A, 10, 15, 5013, effectHue, 0, EffectLayer.Waist);
                                target.FixedParticles(0x3709, 8, 12, 5044, effectHue, 0, EffectLayer.Head);
                                Effects.SendLocationEffect(target.Location, target.Map, 0x36BD, 15, 10, effectHue, 0);
                            }
                        });
                    }

                    Timer.DelayCall(TimeSpan.FromSeconds(1.2), delegate()
                    {
                        if (target != null && !target.Deleted && target.Alive)
                        {
                            target.Kill();
                            target.FixedEffect(0x3709, 10, 30, effectHue, 0);
                            target.PlaySound(0x211);
                            target.SendMessage("Your life force is violently ripped away!");

                            Effects.SendLocationEffect(target.Location, target.Map, 0x36BD, 30, 10, effectHue, 0);
                        }
                    });
                }
                else
                {
                    int damage = Utility.RandomMinMax(32, 37) + level;
                    AOS.Damage(target, caster, damage, 100, 0, 0, 0, 0);

                    target.FixedParticles(0x376A, 9, 20, 5044, effectHue, 0, EffectLayer.CenterFeet);
                    target.PlaySound(0x1F2);
                    target.SendMessage("You resist the death magic but suffer terrible pain!");

                    for (int i = 1; i <= 2; i++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(i * 0.4), delegate()
                        {
                            if (!SpellHelpers.IsValidCaster(caster))
                                return;

                            if (target != null && !target.Deleted && target.Alive)
                            {
                                target.FixedParticles(0x374A, 5, 10, 5013, effectHue, 0, EffectLayer.Waist);
                            }
                        });
                    }
                }
            });
        }
    }

    public class FirestormSpell : CustomSpell
    {
        public FirestormSpell() : base("Firestorm", 1160)
        {
            AddLevel(SpellType.Cleric, 8);
            AddLevel(SpellType.Druid, 7);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.DoT);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            int effectHue = hue != 0 ? hue : 1160;

            caster.FixedParticles(0x3709, 10, 15, 5052, effectHue, 0, EffectLayer.RightHand);
            caster.FixedParticles(0x36BD, 10, 20, 5044, effectHue, 0, EffectLayer.Waist);
            caster.PlaySound(0x208);

            Effects.SendLocationEffect(caster.Location, caster.Map, 0x36BD, 30, 10, effectHue, 0);

            for (int i = 0; i < 5; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i * 0.15), delegate()
                {
                    if (!SpellHelpers.IsValidCaster(caster))
                        return;

                    Point3D burstLoc = new Point3D(
                        caster.X + Utility.RandomMinMax(-2, 2),
                        caster.Y + Utility.RandomMinMax(-2, 2),
                        caster.Z
                    );
                    Effects.SendLocationEffect(burstLoc, caster.Map, 0x3709, 20, 10, effectHue, 0);
                });
            }

            for (int i = 0; i < level; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i * 0.1), delegate()
                {
                    if (!SpellHelpers.IsValidCaster(caster))
                        return;

                    Direction dir = (Direction)Utility.Random(8);
                    Point3D loc = caster.Location;

                    for (int j = 0; j < Utility.RandomMinMax(2, 4); j++)
                    {
                        loc = GetOffset(loc, dir);
                    }

                    FirestormField field = new FirestormField(loc, caster, level, effectHue);
                    field.MoveToWorld(loc, caster.Map);
                });
            }
        }

        private Point3D GetOffset(Point3D p, Direction d)
        {
            switch (d)
            {
                case Direction.North: return new Point3D(p.X, p.Y - 1, p.Z);
                case Direction.South: return new Point3D(p.X, p.Y + 1, p.Z);
                case Direction.West: return new Point3D(p.X - 1, p.Y, p.Z);
                case Direction.East: return new Point3D(p.X + 1, p.Y, p.Z);
                case Direction.Up: return new Point3D(p.X - 1, p.Y - 1, p.Z);
                case Direction.Down: return new Point3D(p.X + 1, p.Y + 1, p.Z);
                case Direction.Left: return new Point3D(p.X - 1, p.Y + 1, p.Z);
                case Direction.Right: return new Point3D(p.X + 1, p.Y - 1, p.Z);
            }
            return p;
        }
    }

    public class FirestormField : Item
    {
        private Timer m_Timer;
        private Mobile m_Caster;
        private int m_Level;
        private int m_Hue;

        public FirestormField(Point3D loc, Mobile caster, int level, int hue) : base(0x398C)
        {
            Movable = false;
            Hue = hue;
            m_Caster = caster;
            m_Level = level;
            m_Hue = hue;

            Effects.SendLocationEffect(loc, caster.Map, 0x36BD, 25, 10, hue, 0);

            m_Timer = Timer.DelayCall(
                TimeSpan.Zero,
                TimeSpan.FromSeconds(2),
                new TimerCallback(Tick)
            );

            Timer.DelayCall(TimeSpan.FromSeconds(12 + level), Delete);
        }

        private void Tick()
        {
            if (Deleted || m_Caster == null || m_Caster.Deleted)
            {
                if (m_Timer != null)
                    m_Timer.Stop();
                return;
            }

            Effects.SendLocationEffect(Location, Map, 0x3709, 15, 10, m_Hue, 0);

            SpellHelpers.ForEachHostileInRange(
                Location,
                Map,
                m_Caster,
                0,
                delegate(Mobile m)
                {
                    m_Caster.DoHarmful(m);

                    int damage = Utility.RandomMinMax(32, 37) + m_Level;
                    AOS.Damage(m, m_Caster, damage, 0, 100, 0, 0, 0);

                    m.FixedParticles(0x3709, 10, 15, 5052, m_Hue, 0, EffectLayer.Waist);
                    m.FixedParticles(0x36BD, 8, 12, 5044, m_Hue, 0, EffectLayer.Head);

                    m.PlaySound(0x208);
                    m.SendMessage("Raging flames engulf you!");
                }
            );

        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            Effects.SendLocationEffect(Location, Map, 0x3735, 15, 10, m_Hue, 0);
        }

        public FirestormField(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_Caster);
            writer.Write(m_Level);
            writer.Write(m_Hue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Caster = reader.ReadMobile();
            m_Level = reader.ReadInt();
            m_Hue = reader.ReadInt();
            Delete();
        }
    }
    
    public class PlagueSpell : CustomSpell
    {
        public PlagueSpell() : base("Plague", 0x36BD)
        {
            AddLevel(SpellType.Cleric, 7);
            AddLevel(SpellType.Druid, 7);
            AddLevel(SpellType.Wizard, 8);
            AddLevel(SpellType.Sorcerer, 8);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.DoT);
            AddTag(SpellTag.Debuff);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            if (!SpellHelpers.IsValidCaster(caster))
                return;

            Mobile center = caster.Combatant as Mobile;
            if (!SpellHelpers.isValidHostileTarget(caster, center))
                return;

            int debuff = 12 + level * 3;
            int stamLoss = 40 + level * 3;
            int effectHue = hue != 0 ? hue : 0x48E;

            caster.FixedParticles(0x374A, 10, 15, 5013, effectHue, 0, EffectLayer.RightHand);
            caster.FixedParticles(0x36BD, 10, 20, 5044, effectHue, 0, EffectLayer.Waist);
            caster.PlaySound(0x205);

            Effects.SendLocationEffect(center.Location, center.Map, 0x3728, 30, 10, effectHue, 0);

            for (int i = 0; i < 6; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i * 0.2), delegate()
                {
                    if (!SpellHelpers.IsValidCaster(caster))
                        return;

                    if (center != null && !center.Deleted)
                    {
                        Point3D plagueCloud = new Point3D(
                            center.X + Utility.RandomMinMax(-3, 3),
                            center.Y + Utility.RandomMinMax(-3, 3),
                            center.Z
                        );
                        Effects.SendLocationEffect(plagueCloud, center.Map, 0x3728, 15, 10, effectHue, 0);
                    }
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds(0.4), delegate()
            {
                if (!SpellHelpers.IsValidCaster(caster))
                    return;

                SpellHelpers.ForEachHostileInRange(center, caster, 4, delegate(Mobile m)
                {
                    caster.DoHarmful(m);

                    m.ApplyPoison(caster, Poison.Deadly);

                    m.AddStatMod(new StatMod(StatType.Str, "PlagueStr", -debuff, TimeSpan.FromSeconds(20 + level)));
                    m.Stam = Math.Max(0, m.Stam - stamLoss);

                    m.FixedEffect(0x36BD, 10, 20, effectHue, 0);
                    m.FixedParticles(0x374A, 10, 15, 5013, effectHue, 0, EffectLayer.Waist);
                    m.FixedParticles(0x3728, 10, 20, 5044, effectHue, 0, EffectLayer.Head);
                    m.FixedParticles(0x36BD, 8, 12, 5013, effectHue, 0, EffectLayer.CenterFeet);

                    for (int i = 1; i <= 5; i++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(i * 0.5), delegate()
                        {
                            if (!SpellHelpers.IsValidCaster(caster))
                                return;

                            if (m != null && !m.Deleted && m.Alive)
                            {
                                m.FixedParticles(0x374A, 5, 10, 5013, effectHue, 0, EffectLayer.Waist);
                                m.FixedParticles(0x3728, 5, 10, 5044, effectHue, 0, EffectLayer.Head);
                                Effects.SendLocationEffect(m.Location, m.Map, 0x36BD, 10, 8, effectHue, 0);
                            }
                        });
                    }

                    m.PlaySound(0x205);
                    m.SendMessage("Virulent plague ravages your body!");
                });
            });
        }
    }

    public class SummonNatureAllyVIISpell : SummonNatureAllySpell
    {
        public SummonNatureAllyVIISpell()
            : base("Summon Nature's Ally VII", 0x2156, 7, typeof(Tyranasaur))
        {
        }
    }


    public class SunbeamSpell : CustomSpell
    {
        public SunbeamSpell() : base("Sunbeam", 0x3709)
        {
            AddLevel(SpellType.Druid, 7);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Offensive);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
           if(!SpellHelpers.isValidHostileTarget(caster, target))
                return;

            int beams = Math.Max(1, level / 3);

            caster.PlaySound(0x29);
            
            for (int i = 0; i < beams; i++)
            {
                if(!SpellHelpers.isValidHostileTarget(caster, target))
                    break;

                caster.DoHarmful(target);

                target.BoltEffect(hue != 0 ? hue : 0x480);

                int dmg = Utility.RandomMinMax(45, 50)+level;
                AOS.Damage(target, caster, dmg, 0, 100, 0, 0, 0);

                double resist = target.Skills[SkillName.MagicResist].Value;
                int para = (int)(2 + level - resist / 10);

                if (para > 0)
                    target.Paralyze(TimeSpan.FromSeconds(para));
            }
        }
    }
    // ===== LEVEL 8 =====
    public class BlackfireSpell : CustomSpell
    {
        public BlackfireSpell() : base("Blackfire", 0x36D4)
        {
            AddLevel(SpellType.Wizard, 8);
            AddLevel(SpellType.Sorcerer, 8);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.Debuff);
        }
        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
            if(!SpellHelpers.isValidHostileTarget(caster, target))
                return;

            caster.DoHarmful(target);
            target.SendMessage("Black flames burn into your soul!");

            caster.MovingParticles(target, 0x36D4, 7, 0, false, true,
                hue != 0 ? hue : 0x455, 0, 9502, 1, 0, (EffectLayer)255, 0);

            int dmg = Utility.RandomMinMax(50,55)+level;

            AOS.Damage(target, caster, dmg, 0, 0, 100, 0, 0);

            SpellHelpers.ApplyRepeatedStatDrain(
                target,
                StatType.Str,
                5,
                8,
                6,
                2,
                22 + level
            );
            target.PlaySound(0x208);
        }
    }
    public class EnervationSpell : CustomSpell
    {
        public EnervationSpell() : base("Enervation", 0x374A)
        {
            AddLevel(SpellType.Wizard, 8);
            AddLevel(SpellType.Sorcerer, 8);
            AddLevel(SpellType.Cleric, 8);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.Debuff);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
            if(!SpellHelpers.isValidHostileTarget(caster, target))
                return;


            SpellHelpers.ForEachHostileInRange(target, caster, 1, m =>
            {
                caster.DoHarmful(m);

                int debuff = 32 + level * 2;
                int loss = 40 + level * 3;
                TimeSpan dur = TimeSpan.FromSeconds(42 + level * 2);

                m.AddStatMod(new StatMod(StatType.Str, "EnervationStr", -debuff, dur));
                m.Stam = Math.Max(0, m.Stam - loss);
                m.Mana = Math.Max(0, m.Mana - loss);

                m.FixedEffect(0x374A, 10, 20);
                m.PlaySound(0x1FB);
                m.SendMessage("Your strength and energy are drained!");
            });
        }
    }

    public class HorridWiltingSpell : CustomSpell
    {
        public HorridWiltingSpell() : base("Horrid Wilting", 0x3709)
        {
            AddLevel(SpellType.Wizard, 8);
            AddLevel(SpellType.Sorcerer, 8);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
            if(!SpellHelpers.isValidHostileTarget(caster, target))
                return;


            SpellHelpers.ForEachHostileInRange(target, caster, 3, m =>
            {
                int dmg = Utility.RandomMinMax(36, 39) + level;
                AOS.Damage(m, caster, dmg, 0, 50, 0, 50, 0);

                m.FixedEffect(0x3709, 10, 30);
                m.PlaySound(0x1FB);
                m.SendMessage("Your body shrivels from magical drought!");
            });
        }
    }

    public class SummonNatureAllyVIIISpell : SummonNatureAllySpell
    {
        public SummonNatureAllyVIIISpell()
            : base("Summon Nature's Ally VIII", 0x2156, 8, typeof(YoungRoc))
        {
        }
    }


    public class SunburstSpell : CustomSpell
    {
        public SunburstSpell() : base("Sunburst", 0x3709)
        {
            AddLevel(SpellType.Druid, 8);
            AddLevel(SpellType.Wizard, 8);
            AddLevel(SpellType.Sorcerer, 8);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.CC);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            caster.PlaySound(0x15E);

            Effects.SendLocationEffect(caster.Location, caster.Map, 0x3709, 30, 10, 0x480, 0);

            Effects.SendLocationEffect(caster.Location, caster.Map, 0x3728, 20, 10, 0x480, 0);

            for (int i = 0; i < 8; i++)
            {
                int xOffset = 0;
                int yOffset = 0;

                switch(i)
                {
                    case 0: xOffset = 3; yOffset = 0; break;   // East
                    case 1: xOffset = -3; yOffset = 0; break;  // West
                    case 2: xOffset = 0; yOffset = 3; break;   // South
                    case 3: xOffset = 0; yOffset = -3; break;  // North
                    case 4: xOffset = 2; yOffset = 2; break;   // SE
                    case 5: xOffset = -2; yOffset = 2; break;  // SW
                    case 6: xOffset = 2; yOffset = -2; break;  // NE
                    case 7: xOffset = -2; yOffset = -2; break; // NW
                }

                Point3D targetLoc = new Point3D(
                    caster.X + xOffset, 
                    caster.Y + yOffset, 
                    caster.Z
                );

                Effects.SendLocationEffect(targetLoc, caster.Map, 0x3818, 15, 10, 0x480, 0);
            }

            Timer.DelayCall(TimeSpan.FromSeconds(0.3), () => 
            {
                caster.PlaySound(0x307);
            });

            SpellHelpers.ForEachHostileInRange(caster, caster, 4, m =>
            {
                caster.DoHarmful(m);

                int dmg = Utility.RandomMinMax(40, 45) + level;
                AOS.Damage(m, caster, dmg, 0, 100, 0, 0, 0);

                m.FixedEffect(0x3709, 10, 30, 0x480, 0);

                m.FixedEffect(0x375A, 10, 15, 0x480, 0);

                m.PlaySound(0x208);

                double resist = m.Skills[SkillName.MagicResist].Value;
                int para = (int)(8 + level - resist / 10);

                if (para > 0)
                {
                    m.Paralyze(TimeSpan.FromSeconds(para));

                    m.FixedEffect(0x376A, 9, 32, 0x480, 0);
                }

                m.SendMessage("You are blinded by searing light!");
            });

            Timer.DelayCall(TimeSpan.FromSeconds(0.5), () =>
            {
                Effects.SendLocationEffect(caster.Location, caster.Map, 0x3735, 15, 10, 0x480, 0);
            });
        }
    }

    public class PolarRaySpell : CustomSpell
    {
        public PolarRaySpell() : base("Polar Ray", 0x36D4)
        {
            AddLevel(SpellType.Wizard, 8);
            AddLevel(SpellType.Sorcerer, 8);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.CC);
        }   

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
            if(!SpellHelpers.isValidHostileTarget(caster, target))
                return;

            caster.PlaySound(0x64D);

            caster.DoHarmful(target);
            target.SendMessage("You are struck by absolute cold!"); 

            caster.MovingParticles(target, 0x36D4, 7, 0, false, true,
                hue != 0 ? hue : 0x481, 0, 9502, 1, 0, (EffectLayer)255, 0);

            Effects.SendTargetEffect(target, 0x37CC, 10, 32, 0x481, 0);

            caster.MovingParticles(target, 0x1363, 10, 0, false, false, 0x481, 0, 9502, 1, 0, (EffectLayer)255, 0);

            int dmg = Utility.RandomMinMax(50,55)+level;   
            AOS.Damage(target, caster, dmg, 0, 0, 100, 0, 0);   

            target.PlaySound(0x64F);
            target.PlaySound(0x28);

            target.FixedEffect(0x376A, 10, 16, 0x481, 0);

            double resist = target.Skills[SkillName.MagicResist].Value;
            int para = (int)(12 + level - resist / 10); 
            if (para > 0)
            {
                target.Paralyze(TimeSpan.FromSeconds(para));
                target.FixedEffect(0x3779, 10, 30, 0x481, 0);
            }

            for(int i = 0; i < 3; i++)
            {
                int xOffset = Utility.RandomMinMax(-1, 1);
                int yOffset = Utility.RandomMinMax(-1, 1);
                Point3D iceLoc = new Point3D(target.X + xOffset, target.Y + yOffset, target.Z);
                Effects.SendLocationEffect(iceLoc, target.Map, 0x352D, 16, 10, 0x481, 0);
            }
        }
    }

    public class PowerWordFearSpell : CustomSpell
    {
        public PowerWordFearSpell() : base("Powerword: Fear", 0x22C5)
        {
            AddLevel(SpellType.Wizard, 8);
            AddLevel(SpellType.Sorcerer, 8);
            AddLevel(SpellType.Cleric, 8);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.CC);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            caster.PlaySound(0x204);
            caster.PlaySound(0x1FB);

            Effects.SendLocationEffect(caster.Location, caster.Map, 0x37C4, 20, 10, 0x496, 0);

            for(int i = 0; i < 8; i++)
            {
                int xOffset = 0;
                int yOffset = 0;

                switch(i)
                {
                    case 0: xOffset = 4; yOffset = 0; break;
                    case 1: xOffset = -4; yOffset = 0; break;
                    case 2: xOffset = 0; yOffset = 4; break;
                    case 3: xOffset = 0; yOffset = -4; break;
                    case 4: xOffset = 3; yOffset = 3; break;
                    case 5: xOffset = -3; yOffset = 3; break;
                    case 6: xOffset = 3; yOffset = -3; break;
                    case 7: xOffset = -3; yOffset = -3; break;
                }

                Point3D waveLoc = new Point3D(caster.X + xOffset, caster.Y + yOffset, caster.Z);
                Effects.SendLocationEffect(waveLoc, caster.Map, 0x3709, 15, 10, 0x496, 0);
            }

            SpellHelpers.ForEachHostileInRange(caster, caster, 5, m =>
            {
                caster.DoHarmful(m);

                m.FixedEffect(0x374A, 10, 16, 0x496, 0);
                m.FixedEffect(0x37C4, 10, 30, 0x496, 0);

                m.PlaySound(0x213);

                double resist = m.Skills[SkillName.MagicResist].Value;
                double duration = Math.Max(2, (10 + level) - resist / 15.0);
                m.Paralyze(TimeSpan.FromSeconds(duration));

                int dist = Utility.RandomMinMax(5, 8);
                Point3D flee = SpellHelpers.GetPointInDirection(m, m.GetDirectionTo(caster), -dist);

                Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 13, 10, 0x496, 0);

                m.MoveToWorld(flee, m.Map);

                m.FixedEffect(0x376A, 9, 32, 0x496, 0);

                Effects.SendLocationEffect(flee, m.Map, 0x3728, 10, 10, 0x496, 0);

                m.SendMessage("You flee in absolute terror!");
            });
        }
    }
    
    public class PowerWordFatigueSpell : CustomSpell
    {
        public PowerWordFatigueSpell() : base("Powerword: Fatigue", 0x22C5)
        {
            AddLevel(SpellType.Wizard, 8);
            AddLevel(SpellType.Sorcerer, 8);
            AddLevel(SpellType.Cleric, 8);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.Debuff);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            caster.PlaySound(0x1F8);
            caster.PlaySound(0x1E1);

            Effects.SendLocationEffect(caster.Location, caster.Map, 0x37CC, 20, 10, 0x21, 0);

            for(int i = 0; i < 8; i++)
            {
                int xOffset = 0;
                int yOffset = 0;

                switch(i)
                {
                    case 0: xOffset = 4; yOffset = 0; break;
                    case 1: xOffset = -4; yOffset = 0; break;
                    case 2: xOffset = 0; yOffset = 4; break;
                    case 3: xOffset = 0; yOffset = -4; break;
                    case 4: xOffset = 3; yOffset = 3; break;
                    case 5: xOffset = -3; yOffset = 3; break;
                    case 6: xOffset = 3; yOffset = -3; break;
                    case 7: xOffset = -3; yOffset = -3; break;
                }

                Point3D waveLoc = new Point3D(caster.X + xOffset, caster.Y + yOffset, caster.Z);
                Effects.SendLocationEffect(waveLoc, caster.Map, 0x3779, 15, 10, 0x21, 0);
            }

            SpellHelpers.ForEachHostileInRange(caster, caster, 5, m =>
            {
                caster.DoHarmful(m);

                m.FixedEffect(0x376A, 10, 30, 0x21, 0);
                m.FixedEffect(0x37C4, 10, 20, 0x21, 0);

                m.PlaySound(0x1F1);

                double resist = m.Skills[SkillName.MagicResist].Value;
                double remaining = resist / 20.0;
                if (remaining > 5)
                    remaining = 5;

                int newStam = (int)(m.StamMax * (remaining / 100.0));
                m.Stam = Math.Max(0, newStam);

                m.FixedEffect(0x3709, 10, 30, 0x21, 0);

                for(int i = 0; i < 2; i++)
                {
                    int xOffset = Utility.RandomMinMax(-1, 1);
                    int yOffset = Utility.RandomMinMax(-1, 1);
                    Point3D exhaustLoc = new Point3D(m.X + xOffset, m.Y + yOffset, m.Z);
                    Effects.SendLocationEffect(exhaustLoc, m.Map, 0x3735, 10, 10, 0x21, 0);
                }

                m.SendMessage("Your limbs feel unbearably heavy!");
            });
        }
    }

    public class MassValiantSpiritSpell : CustomSpell
    {
        public MassValiantSpiritSpell() : base("Valiant Spirit, Mass", 0x480)
        {
            AddLevel(SpellType.Cleric, 8);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Heal);
            AddTag(SpellTag.Buff);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            IPooledEnumerable eable = caster.GetMobilesInRange(3);

            foreach (Mobile m in eable)
            {
                if(SpellHelpers.isValidHostileTarget(caster, m))
                    continue;

                int heal = Utility.RandomMinMax(40, 52) + level;
                int str = 10 + level;
                int resist = Math.Min(70, 11 + level);

                m.Hits += heal;
                m.AddStatMod(new StatMod(StatType.Str, "ValiantMassStr", str, TimeSpan.FromSeconds(60 + level * 10)));

                m.AddResistanceMod(new ResistanceMod(ResistanceType.Fire, resist));
                m.AddResistanceMod(new ResistanceMod(ResistanceType.Cold, resist));
                m.AddResistanceMod(new ResistanceMod(ResistanceType.Energy, resist));
                m.AddResistanceMod(new ResistanceMod(ResistanceType.Poison, resist));
                m.AddResistanceMod(new ResistanceMod(ResistanceType.Physical, resist));

                m.FixedParticles(0x375A, 10, 15, 5012, hue, 0, EffectLayer.Waist);
            }

            eable.Free();
        }
    }

    // ===== LEVEL 9 =====
    public class IcebergSpell : CustomSpell
    {
        public IcebergSpell() : base("Iceberg", 0x36BD)
        {
            AddLevel(SpellType.Wizard, 9);
            AddLevel(SpellType.Sorcerer, 9);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.Debuff);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
            if (target == null)
                return;

            caster.PlaySound(0x64E);
            caster.PlaySound(0x1FB);

            Effects.SendLocationEffect(target.Location, target.Map, 0x3779, 20, 10, 0x481, 0);

            for(int i = 0; i < 12; i++)
            {
                int xOffset = Utility.RandomMinMax(-3, 3);
                int yOffset = Utility.RandomMinMax(-3, 3);
                Point3D iceLoc = new Point3D(target.X + xOffset, target.Y + yOffset, target.Z);
                Effects.SendLocationEffect(iceLoc, target.Map, 0x352D, 20, 10, 0x481, 0);
            }

            SpellHelpers.ForEachHostileInRange(target, caster, 3, m =>
            {
                Effects.SendLocationEffect(m.Location, m.Map, 0x36BD, 20, 10, 0x481, 0);

                m.FixedEffect(0x37CC, 10, 30, 0x481, 0);

                m.PlaySound(0x664);
                m.PlaySound(0x208);

                for(int i = 0; i < 4; i++)
                {
                    int xOffset = Utility.RandomMinMax(-1, 1);
                    int yOffset = Utility.RandomMinMax(-1, 1);
                    Point3D shardLoc = new Point3D(m.X + xOffset, m.Y + yOffset, m.Z);
                    Effects.SendLocationEffect(shardLoc, m.Map, 0x36D4, 15, 10, 0x481, 0);
                }

                int damage = Utility.RandomMinMax(44, 49)+level;
                int stamLoss = damage / 3;
                AOS.Damage(m, caster, damage, 0, 0, 100, 0, 0);
                m.Stam = Math.Max(0, m.Stam - stamLoss);

                m.FixedEffect(0x376A, 10, 16, 0x481, 0);

                m.SendMessage("You are crushed beneath falling ice!");
            });
        }
    }

    public class MeteorSwarmSpell : CustomSpell
    {
        public MeteorSwarmSpell() : base("Meteor Swarm", 0x36CB)
        {
            AddLevel(SpellType.Wizard, 9);
            AddLevel(SpellType.Sorcerer, 9);
            AddTag(SpellTag.AoE);
            AddTag(SpellTag.Offensive);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
            if(!SpellHelpers.isValidHostileTarget(caster, target))
                return;

            int effectHue = hue != 0 ? hue : 0x4A5;

            caster.PlaySound(0x307);
            caster.PlaySound(0x15E);

            Effects.SendLocationEffect(target.Location, target.Map, 0x3709, 20, 10, effectHue, 0);

            for (int i = 0; i < 4; i++)
            {
                int ox = Utility.RandomMinMax(-3, 3);
                int oy = Utility.RandomMinMax(-3, 3);
                Point3D center = new Point3D(target.X + ox, target.Y + oy, target.Z);

                Point3D skyPoint = new Point3D(center.X, center.Y, center.Z + 20);
                Effects.SendLocationEffect(skyPoint, target.Map, 0x36D4, 30, 10, effectHue, 0);

                Effects.SendLocationEffect(center, target.Map, 0x36CB, 20, 10, effectHue, 0);
                Effects.SendLocationEffect(center, target.Map, 0x3709, 18, 10, effectHue, 0);

                for (int x = -1; x <= 0; x++)
                for (int y = -1; y <= 0; y++)
                {
                    Point3D loc = new Point3D(center.X + x, center.Y + y, center.Z);
                    Effects.SendLocationEffect(loc, target.Map, 0x36CB, 16, 10, effectHue, 0);
                }

                for(int j = 0; j < 6; j++)
                {
                    int debrisX = Utility.RandomMinMax(-2, 2);
                    int debrisY = Utility.RandomMinMax(-2, 2);
                    Point3D debrisLoc = new Point3D(center.X + debrisX, center.Y + debrisY, center.Z);
                    Effects.SendLocationEffect(debrisLoc, target.Map, 0x36BD, 12, 10, effectHue, 0);
                }

                SpellHelpers.DamageHostilesAtPoint(
                    caster,
                    center,
                    target.Map,
                    1,
                    () => Utility.RandomMinMax(44, 49)+level,
                    0, 100, 0, 0, 0,
                    0x36CB,
                    effectHue
                );
            }

            caster.PlaySound(0x11D);
        }
    }
    public class PowerWordKillSpell : CustomSpell
    {
        public PowerWordKillSpell() : base("Powerword: Kill", 0x22C5)
        {
            AddLevel(SpellType.Wizard, 9);
            AddLevel(SpellType.Sorcerer, 9);
            AddTag(SpellTag.SingleTarget);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.Debuff);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
            if(!SpellHelpers.isValidHostileTarget(caster, target))
                return;

            caster.PlaySound(0x1FB);
            caster.PlaySound(0x208);

            caster.FixedEffect(0x37C4, 10, 20, 0x1, 0);

            caster.MovingParticles(target, 0x374A, 9, 0, false, true, 0x1, 0, 9502, 1, 0, (EffectLayer)255, 0);

            caster.DoHarmful(target);

            if (target.Hits < 101)
            {
                target.FixedEffect(0x37C4, 10, 30, 0x1, 0);
                target.FixedEffect(0x376A, 10, 30, 0x1, 0);

                for(int i = 0; i < 8; i++)
                {
                    int xOffset = Utility.RandomMinMax(-2, 2);
                    int yOffset = Utility.RandomMinMax(-2, 2);
                    Point3D deathLoc = new Point3D(target.X + xOffset, target.Y + yOffset, target.Z);
                    Effects.SendLocationEffect(deathLoc, target.Map, 0x3709, 10, 10, 0x1, 0);
                }

                target.PlaySound(0x211);
                target.PlaySound(0x1F1);

                target.Kill();

                Effects.SendLocationEffect(target.Location, target.Map, 0x3728, 13, 10, 0x1, 0);
            }
            else
            {
                target.FixedEffect(0x3779, 10, 15, 0x1, 0);
                target.SendMessage("The word of death fails to claim you!");
                target.PlaySound(0x1F2);
            }
        }
    }

    public class StormOfVengeanceSpell : CustomSpell
    {
        public StormOfVengeanceSpell() : base("Storm of Vengeance", 0x36BD)
        {
            AddLevel(SpellType.Druid, 9);
            AddLevel(SpellType.Cleric, 9);
            AddTag(SpellTag.Offensive);
            AddTag(SpellTag.AoE);
        }

        public override void Cast(Mobile caster, int hue, int level)
        {
            Mobile target = caster.Combatant as Mobile;
            if(!SpellHelpers.isValidHostileTarget(caster, target))
                return;

            Point3D center = target.Location;
            Map map = target.Map;
            int effectHue = hue != 0 ? hue : 0x480;

            caster.PlaySound(0x5CF);
            caster.PlaySound(0x5CE);

            Effects.SendLocationEffect(center, map, 0x3709, 30, 10, effectHue, 0);

            for(int i = 0; i < 12; i++)
            {
                int ox = Utility.RandomMinMax(-7, 7);
                int oy = Utility.RandomMinMax(-7, 7);
                Point3D cloudLoc = new Point3D(center.X + ox, center.Y + oy, center.Z);
                Effects.SendLocationEffect(cloudLoc, map, 0x3779, 25, 10, effectHue, 0);
            }

            SpellHelpers.DoTimedStorm(
                caster,
                center,
                map,
                7,
                5,
                3,
                () => Utility.RandomMinMax(44, 49)+level,
                25, 25, 25, 25, 0,
                0x36BD,
                effectHue,
                "The storm tears into you!"
            );
        }
    }

    public class SummonNatureAllyIXSpell : SummonNatureAllySpell
    {
        public SummonNatureAllyIXSpell()
            : base("Summon Nature's Ally IX", 0x2156, 9, typeof(Roc))
        {
        }
    }

}