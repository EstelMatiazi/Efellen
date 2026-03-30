using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.EffectsUtil;
using Server.Spells.Necromancy;
using Server.Spells;
using Server.Items;

namespace Server.Custom.DailyBosses.System
{
    public static class BossSpecialAttack
    {
        private static ArrayList m_BossBleeds = new ArrayList();
        private static Dictionary<Mobile, Dictionary<ResistanceType, ResistanceMod>> m_ActiveResistBreaches = new Dictionary<Mobile, Dictionary<ResistanceType, ResistanceMod>>();
        private const double TELEGRAPH_DELAY = 2;
        #region slam
        /// <summary>
        /// Performs a telegraphed AoE slam attack centered on the boss
        /// </summary>
        /// <param name="boss">The boss performing the attack</param>
        /// <param name="warcry">Message displayed overhead</param>
        /// <param name="hue">Color hue for visual effects</param>
        /// <param name="rage">Boss rage level (affects damage)</param>
        /// <param name="range">Attack radius (default: 6)</param>
        /// <param name="physicalDmg">Physical damage percentage (0-100)</param>
        /// <param name="fireDmg">Fire damage percentage (0-100)</param>
        /// <param name="coldDmg">Cold damage percentage (0-100)</param>
        /// <param name="poisonDmg">Poison damage percentage (0-100)</param>
        /// <param name="energyDmg">Energy damage percentage (0-100)</param>
        public static void PerformSlam(
            BaseCreature boss,
            string warcry,
            int hue,
            int rage,
            int range = 6,
            int physicalDmg = 100,
            int fireDmg = 0,
            int coldDmg = 0,
            int poisonDmg = 0,
            int energyDmg = 0
        )
        {
            if (boss == null || boss.Deleted || !boss.Alive)
                return;

            // Validate damage percentages -- must equal to 100
            int totalDamage = physicalDmg + fireDmg + coldDmg + poisonDmg + energyDmg;
            if (totalDamage != 100)
            {
                Console.WriteLine("Warning: Damage percentages for " + boss.Name + " slam total " + totalDamage + "%, expected 100%");
            }

            if (!string.IsNullOrEmpty(warcry))
            {
                boss.PublicOverheadMessage(MessageType.Regular, hue, false, warcry);
            }

            boss.FixedParticles(0x3709, 10, 30, 5052, hue, 0, EffectLayer.Waist);
            boss.PlaySound(0x208);

            Point3D bossLocation = boss.Location;
            Map bossMap = boss.Map;

            Timer.DelayCall(TimeSpan.FromSeconds(TELEGRAPH_DELAY), delegate()
            {
                if (boss.Deleted || !boss.Alive || bossMap == null)
                    return;

                boss.PlaySound(0x64F);
                Effects.SendLocationEffect(bossLocation, bossMap, 0x36B0, 30, 10, hue, 0);

                int minDamage = 50 + (int)(rage * 4);//50-66
                int maxDamage = 70 + (rage * 6);//70-88

                IPooledEnumerable eable = bossMap.GetMobilesInRange(bossLocation, range);
                
                foreach (Mobile m in eable)
                {
                    if (m == null || m == boss || !m.Player || !m.Alive)
                        continue;

                    if (!boss.CanBeHarmful(m))
                        continue;

                    boss.DoHarmful(m);
                    
                    int damage = Utility.RandomMinMax(minDamage, maxDamage);
                    AOS.Damage(m, boss, damage, physicalDmg, fireDmg, coldDmg, poisonDmg, energyDmg);

                    m.FixedParticles(0x36B0, 1, 10, 5013, hue, 0, EffectLayer.Waist);
                }
                eable.Free();
                SlamVisuals.SlamVisual(boss, range, 0x36B0, hue);
            });
        }
        #endregion
        #region rampage
        /// <summary>
        /// Performs a telegraphed directional charge attack
        /// </summary>
        /// <param name="boss">The boss performing the attack</param>
        /// <param name="warcry">Message displayed overhead</param>
        /// <param name="hue">Color hue for visual effects</param>
        /// <param name="rage">Boss rage level (affects damage and charge count)</param>
        /// <param name="stunDuration">Duration boss is stunned after charge (seconds)</param>
        public static void PerformRampage(
            BaseCreature boss,
            string warcry,
            int hue,
            int rage,
            double stunDuration = 4.0
        )
        {
            if (boss == null || boss.Deleted || !boss.Alive)
                return;

            if (!string.IsNullOrEmpty(warcry))
            {
                boss.PublicOverheadMessage(MessageType.Regular, hue, false, warcry);
            }

            boss.Frozen = true;

            boss.FixedParticles(0x375A, 10, 30, 5052, hue, 0, EffectLayer.CenterFeet);
            boss.FixedParticles(0x376A, 9, 32, 5030, hue, 0, EffectLayer.Waist);
            boss.PlaySound(0x15E);

            Effects.SendLocationEffect(boss.Location, boss.Map, 0x3728, 30, 10, hue, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(TELEGRAPH_DELAY), delegate()
            {
                if (boss.Deleted || !boss.Alive)
                {
                    boss.Frozen = false;
                    return;
                }

                boss.Frozen = false;
                boss.PlaySound(0x51D);

                int chargeCount = Utility.RandomMinMax(3, 5) + rage;
                int minDamage = 40 + (int)(rage * 4);//40-52
                int maxDamage = 55 + (rage * 4);//55-67

                List<Point3D> path = BuildRampagePath(boss, chargeCount);
                List<Mobile> damagedMobiles = new List<Mobile>();

                foreach (Point3D loc in path)
                {
                    boss.MoveToWorld(loc, boss.Map);

                    Effects.SendLocationEffect(loc, boss.Map, 0x3728, 15, 10, hue, 0);
                    Effects.SendLocationEffect(loc, boss.Map, 0x36B0, 10, 10, hue, 0);

                    IPooledEnumerable eable = boss.Map.GetMobilesInRange(loc, 1);
                    foreach (Mobile m in eable)
                    {
                        if (m != boss && m.Player && m.Alive && boss.CanBeHarmful(m) && !damagedMobiles.Contains(m))
                        {
                            boss.DoHarmful(m);
                            int damage = Utility.RandomMinMax(minDamage, maxDamage);
                            AOS.Damage(m, boss, damage, 100, 0, 0, 0, 0);
                            m.FixedParticles(0x36B0, 1, 10, 5013, hue, 0, EffectLayer.Waist);
                            damagedMobiles.Add(m);
                        }
                    }
                    eable.Free();
                }

                // Post-rampage stun
                boss.Frozen = true;
                Timer.DelayCall(TimeSpan.FromSeconds(stunDuration), delegate()
                {
                    if (boss.Deleted)
                        return;

                    boss.Frozen = false;
                    boss.PublicOverheadMessage(MessageType.Regular, hue, false, "*recovers balance*");
                    boss.FixedParticles(0x375A, 10, 15, 5013, hue, 0, EffectLayer.Waist);
                });
            });
        }
        #endregion
        #region summon honor guard
        /// <summary>
        /// Summons elite guard creatures with telegraphed spawn locations
        /// </summary>
        /// <param name="boss">The boss performing the summon</param>
        /// <param name="target">Target for summoned creatures to attack</param>
        /// <param name="warcry">Message displayed overhead</param>
        /// <param name="amount">Number of creatures to summon</param>
        /// <param name="creatureType">Type of creature to summon (must inherit from BaseCreature)</param>
        /// <param name="hue">Color hue for visual effects</param>
        public static void SummonHonorGuard(
            BaseCreature boss,
            Mobile target,
            string warcry,
            int amount,
            Type creatureType,
            int hue
        )
        {
            if (boss == null || boss.Deleted || !boss.Alive)
                return;

            if (creatureType == null || !creatureType.IsSubclassOf(typeof(BaseCreature)))
            {
                Console.WriteLine("Warning: Invalid creature type passed to SummonHonorGuard");
                return;
            }

            Map map = boss.Map;
            if (map == null)
                return;

            if (!string.IsNullOrEmpty(warcry))
            {
                boss.PublicOverheadMessage(MessageType.Regular, hue, false, warcry);
            }

            boss.PlaySound(0x133);
            boss.FixedParticles(0x3728, 1, 13, 9912, hue, 7, EffectLayer.Head);

            List<Point3D> spawnLocations = new List<Point3D>();
            for (int i = 0; i < amount; i++)
            {
                Point3D loc = GetSpawnLocation(boss, map);
                spawnLocations.Add(loc);

                // Warning visual at each spawn location
                Effects.SendLocationEffect(loc, map, 0x3709, 30, 10, hue, 0);
                Effects.PlaySound(loc, map, 0x20F);
            }

            Timer.DelayCall(TimeSpan.FromSeconds(TELEGRAPH_DELAY), delegate()
            {
                if (boss.Deleted || !boss.Alive || map == null)
                    return;

                foreach (Point3D loc in spawnLocations)
                {
                    Effects.SendLocationEffect(loc, map, 0x3728, 20, 10, hue, 0);
                    Effects.PlaySound(loc, map, 0x307);

                    BaseCreature monster = null;
                    try
                    {
                        monster = (BaseCreature)Activator.CreateInstance(creatureType);
                    }
                    catch
                    {
                        Console.WriteLine("Error: Failed to create instance of " + creatureType.Name);
                        continue;
                    }

                    if (monster == null)
                        continue;

                    monster.Team = boss.Team;
                    monster.IsTempEnemy = true;
                    monster.MoveToWorld(loc, map);
                    
                    if (target != null && target.Alive)
                        monster.Combatant = target;
                }
            });
        }
        #endregion
        #region fear
        /// <summary>
        /// Performs an AoE fear effect that paralyzes nearby players
        /// </summary>
        /// <param name="boss">The boss performing the fear</param>
        /// <param name="warcry">Message displayed overhead</param>
        /// <param name="range">Fear effect radius</param>
        /// <param name="rage">Boss rage level (increases duration)</param>
        /// <param name="terror">Knightship skill level required to be immune (1-125)</param>
        public static void PerformFear(
            BaseCreature boss,
            string warcry,
            int range,
            int rage,
            int terror
        )
        {
            if (boss == null || boss.Deleted || !boss.Alive)
                return;

            if (!string.IsNullOrEmpty(warcry))
            {
                boss.PublicOverheadMessage(MessageType.Regular, 0x21, false, warcry);
            }

            boss.PlaySound(0x64D);

            IPooledEnumerable eable = boss.GetMobilesInRange(range);
            
            foreach (Mobile m in eable)
            {
                if (m == boss || !m.Alive || !m.Player || !boss.CanBeHarmful(m))
                    continue;

                if (m.Combatant != boss)
                    continue;

                if (m.Skills.Knightship.Value >= terror)
                {
                    m.SendMessage("Your bravery protects you from fear!");
                    m.FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);
                    continue;
                }

                boss.DoHarmful(m);

                m.Paralyze(TimeSpan.FromSeconds(GetParalyzeDuration(m,rage)));
                m.SendMessage("You are frozen in terror!");
                m.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Head);
            }
            eable.Free();
        }
        #endregion
        #region parahax + bleed
        /// <summary>
        /// Performs an AoE entangling attack that paralyzes and causes bleed damage over time
        /// </summary>
        /// <param name="boss">The boss performing the attack</param>
        /// <param name="warcry">Message displayed overhead when attack executes</param>
        /// <param name="hue">Color hue for visual effects</param>
        /// <param name="rage">Boss rage level (affects paralyze duration)</param>
        /// <param name="range">Attack radius</param>
        /// <param name="bleedLevel">Bleed damage multiplier (damage per tick = level * 2-3)</param>
        public static void PerformEntangle(
            BaseCreature boss,
            string warcry,
            int hue,
            int rage,
            int range,
            int bleedLevel
        )
        {
            if (boss == null || boss.Deleted || !boss.Alive)
                return;

            // Telegraph
            boss.PublicOverheadMessage(MessageType.Regular, hue, false, "*gathering energy*");
            boss.PlaySound(0x1F5);
            boss.FixedParticles(0x375A, 10, 15, 5037, hue, 0, EffectLayer.Waist);

            // Store location and map for the delayed attack
            Point3D bossLocation = boss.Location;
            Map bossMap = boss.Map;

            Timer.DelayCall(TimeSpan.FromSeconds(2.0), delegate()
            {
                if (boss.Deleted || !boss.Alive || bossMap == null)
                    return;

                if (!string.IsNullOrEmpty(warcry))
                {
                    boss.PublicOverheadMessage(MessageType.Regular, hue, false, warcry);
                }

                boss.PlaySound(0x133);
                boss.FixedParticles(0x3728, 1, 13, 9912, hue, 7, EffectLayer.Head);

                Effects.SendLocationEffect(bossLocation, bossMap, 0x36B0, 30, 10, hue, 0);
                SlamVisuals.SlamVisual(boss, range, 0x36B0, hue);

                IPooledEnumerable eable = bossMap.GetMobilesInRange(bossLocation, range);

                foreach (Mobile m in eable)
                {
                    if (m == boss || !m.Player || !m.Alive || !boss.CanBeHarmful(m))
                        continue;

                    boss.DoHarmful(m);

                    int paralyzeDuration = GetParalyzeDuration(m, rage);
                    m.Paralyze(TimeSpan.FromSeconds(paralyzeDuration));
                    m.SendMessage("You are paralyzed by the attack!");

                    if (!Server.Items.BaseRace.IsBleeder(m))
                        continue;

                    if (IsBleedImmune(m))
                        continue;

                    m.PlaySound(0x133);
                    m.FixedParticles(0x377A, 244, 25, 9950, 31, 0, EffectLayer.Waist);

                    if (m is PlayerMobile)
                    {
                        m.LocalOverheadMessage(MessageType.Regular, 0x982, false, "You are bleeding profusely!");
                    }

                    BeginBossBleed(m, boss, bleedLevel);
                }

                eable.Free();
            });
        }
        #endregion
        #region cross explosion
        /// <summary>
        /// Performs a targeted cross-pattern explosion that damages everything it touches
        /// </summary>
        /// <param name="boss">The boss performing the attack</param>
        /// <param name="target">Primary target for the cross pattern</param>
        /// <param name="warcry">Message displayed overhead</param>
        /// <param name="hue">Color hue for visual effects</param>
        /// <param name="rage">Boss rage level (affects damage and range)</param>
        /// <param name="physicalDmg">Physical damage percentage (0-100)</param>
        /// <param name="fireDmg">Fire damage percentage (0-100)</param>
        /// <param name="coldDmg">Cold damage percentage (0-100)</param>
        /// <param name="poisonDmg">Poison damage percentage (0-100)</param>
        /// <param name="energyDmg">Energy damage percentage (0-100)</param>
        public static void PerformCrossExplosion(
            BaseCreature boss,
            Mobile target,
            string warcry,
            int hue,
            int rage,
            int physicalDmg = 100,
            int fireDmg = 0,
            int coldDmg = 0,
            int poisonDmg = 0,
            int energyDmg = 0
        )
        {
            if (boss == null || boss.Deleted || !boss.Alive)
                return;

            if (target == null || target.Deleted || !target.Alive)
                return;

            int totalDamage = physicalDmg + fireDmg + coldDmg + poisonDmg + energyDmg;
            if (totalDamage != 100)
            {
                Console.WriteLine("Warning: Damage percentages for " + boss.Name + " cross explosion total " + totalDamage + "%, expected 100%");
            }

            if (!string.IsNullOrEmpty(warcry))
            {
                boss.PublicOverheadMessage(MessageType.Regular, hue, false, warcry);
            }

            target.FixedParticles(0x3709, 10, 30, 5052, hue, 0, EffectLayer.Waist);
            Effects.PlaySound(target.Location, target.Map, 0x208);

            Point3D targetLocation = target.Location;
            Map targetMap = target.Map;

            Timer.DelayCall(TimeSpan.FromSeconds(TELEGRAPH_DELAY * 2), delegate()
            {
                if (boss.Deleted || !boss.Alive || targetMap == null)
                    return;

                int minDamage = 55 + (rage * 3);//55-64
                int maxDamage = 75 + (rage * 6);//75-93

                Point3D[] points = CrossPoints(target.Location, rage*2);

                List<Mobile> damagedMobiles = new List<Mobile>();

                foreach (Point3D point in points)
                {
                    Effects.SendLocationEffect(point, targetMap, Utility.RandomList(0x33E5, 0x33F5), 85, 10, hue, 0);

                    IPooledEnumerable eable = targetMap.GetMobilesInRange(point, 0);
                    foreach (Mobile m in eable)
                    {
                        if (damagedMobiles.Contains(m) || m == boss || !m.Alive || !m.Player)
                            continue;

                        if (!boss.CanBeHarmful(m))
                            continue;

                        boss.DoHarmful(m);
                        
                        int damage = Utility.RandomMinMax(minDamage, maxDamage);
                        AOS.Damage(m, boss, damage, physicalDmg, fireDmg, coldDmg, poisonDmg, energyDmg);

                        m.FixedParticles(0x36B0, 1, 10, 5013, hue, 0, EffectLayer.Waist);
                        damagedMobiles.Add(m);
                    }
                    eable.Free();
                }
                Effects.PlaySound(targetLocation, targetMap, 0x15F);
            });
        }
        #endregion
        #region targetted AoE explosion
        /// <summary>
        /// Performs a targeted AoE explosion that marks tiles
        /// </summary>
        /// <param name="boss">The boss performing the attack</param>
        /// <param name="target">Primary target for the attack</param>
        /// <param name="warcry">Message displayed overhead</param>
        /// <param name="hue">Color hue for visual effects</param>
        /// <param name="rage">Boss rage level (affects damage and radius)</param>
        /// <param name="physicalDmg">Physical damage percentage (0-100)</param>
        /// <param name="fireDmg">Fire damage percentage (0-100)</param>
        /// <param name="coldDmg">Cold damage percentage (0-100)</param>
        /// <param name="poisonDmg">Poison damage percentage (0-100)</param>
        /// <param name="energyDmg">Energy damage percentage (0-100)</param>
        public static void PerformTargettedAoE(
            BaseCreature boss,
        	Mobile target,
        	int rage,
        	string warcry,
        	int hue,
        	int physicalDmg,
        	int fireDmg,
        	int coldDmg,
        	int poisonDmg,
        	int energyDmg
        )
        {
        	if ( target == null || target.Deleted || target.Map == null )
        		return;

        	int radius = 4 + rage;

        	int minDamage = (int)(45 + (rage * 2));//45-51
        	int maxDamage = 55 + (rage * 4);//55-77

        	if (!string.IsNullOrEmpty(warcry))
            {
                boss.PublicOverheadMessage(MessageType.Regular, hue, false, warcry);
            }

        	boss.MovingParticles(
        		target,
        		0x1C19,
        		1,
        		0,
        		false,
        		true,
        		hue,
        		0,
        		9502,
        		6014,
        		0x11D,
        		EffectLayer.Waist,
        		0
        	);

        	Timer.DelayCall( TimeSpan.FromSeconds( TELEGRAPH_DELAY ), delegate()
        	{
        		if ( target == null || target.Deleted || target.Map == null )
        			return;

        		Map map = target.Map;

        		Effects.SendLocationParticles(
        			EffectItem.Create( target.Location, map, EffectItem.DefaultDuration ),
        			0x36BD,
        			20,
        			10,
        			hue,
        			0,
        			5044,
        			0
        		);

        		Effects.PlaySound( target.Location, map, 0x307 );

        		IPooledEnumerable eable = target.GetMobilesInRange( radius );
        		foreach ( Mobile m in eable )
        		{
        			if ( m == null || m.Deleted || !m.Alive || m == boss )
        				continue;

        			if ( m is BaseCreature )
        			{
        				BaseCreature bc = (BaseCreature)m;
        				if ( bc.Team == boss.Team )
        					continue;
        			}

        			int damage = Utility.RandomMinMax( minDamage, maxDamage );

        			AOS.Damage(
        				m,
        				boss,
        				damage,
        				physicalDmg,
        				fireDmg,
        				coldDmg,
        				poisonDmg,
        				energyDmg
        			);

        			SetOnFire( m, hue );
        		}
        		eable.Free();
        		LightTilesOnFire( target.Location, map, radius, hue );
        	});
        }
        #endregion
        #region smite
        /// <summary>
        /// Performs a single target attack
        /// </summary>
        /// <param name="boss">The boss performing the attack</param>
        /// <param name="target">Primary target for the attack</param>
        /// <param name="warcry">Message displayed overhead</param>
        /// <param name="hue">Color hue for visual effects</param>
        /// <param name="rage">Boss rage level (affects damage and radius)</param>
        /// <param name="physicalDmg">Physical damage percentage (0-100)</param>
        /// <param name="fireDmg">Fire damage percentage (0-100)</param>
        /// <param name="coldDmg">Cold damage percentage (0-100)</param>
        /// <param name="poisonDmg">Poison damage percentage (0-100)</param>
        /// <param name="energyDmg">Energy damage percentage (0-100)</param>
        public static void PerformSmite(
            BaseCreature boss,
        	Mobile target,
        	int rage,
        	string warcry,
        	int hue,
        	int physicalDmg,
        	int fireDmg,
        	int coldDmg,
        	int poisonDmg,
        	int energyDmg
        )
        {
           if (boss == null || boss.Deleted || !boss.Alive || target == null || target.Deleted || !target.Alive)
                return;

            int totalDamage = physicalDmg + fireDmg + coldDmg + poisonDmg + energyDmg;
            if (totalDamage != 100)
            {
                Console.WriteLine("Warning: Damage percentages for " + boss.Name + " smite total " + totalDamage + "%, expected 100%");
            }
            if (!string.IsNullOrEmpty(warcry))
            {
                boss.PublicOverheadMessage(MessageType.Regular, hue, false, warcry);
            }
            boss.FixedParticles(0x3709, 10, 30, 5052, hue, 0, EffectLayer.Waist);
            boss.PlaySound(0x208);
            Point3D bossLocation = boss.Location;
            Map bossMap = boss.Map;
            Timer.DelayCall(TimeSpan.FromSeconds(TELEGRAPH_DELAY), delegate()
            {
                if (boss.Deleted || !boss.Alive || bossMap == null || target == null || target.Deleted || !target.Alive)
                    return;
                
                boss.PlaySound(0x64F);
                Effects.SendLocationEffect(bossLocation, bossMap, 0x36B0, 30, 10, hue, 0);
                int minDamage = 65 + (int)(rage * 3);//65-77
                int maxDamage = 80 + (rage * 4);//80-92
                boss.DoHarmful(target);
                int damage = Utility.RandomMinMax(minDamage, maxDamage);
	            target.BoltEffect(0);
	            target.PlaySound(0x1FB);
                AOS.Damage(target, boss, damage, physicalDmg, fireDmg, coldDmg, poisonDmg, energyDmg);
                target.FixedParticles(0x36B0, 1, 10, 5013, hue, 0, EffectLayer.Waist);
            });
        }
        #endregion
        #region cone breath
        /// <summary>
        /// Performs a cone-shaped breath attack towards the target
        /// </summary>
        /// <param name="boss">The boss performing the attack</param>
        /// <param name="target">Primary target for the cone direction</param>
        /// <param name="warcry">Message displayed overhead</param>
        /// <param name="hue">Color hue for visual effects</param>
        /// <param name="rage">Boss rage level</param>
        /// <param name="range">Cone range (3-5)</param>
        /// <param name="physicalDmg">Physical damage percentage (0-100)</param>
        /// <param name="fireDmg">Fire damage percentage (0-100)</param>
        /// <param name="coldDmg">Cold damage percentage (0-100)</param>
        /// <param name="poisonDmg">Poison damage percentage (0-100)</param>
        /// <param name="energyDmg">Energy damage percentage (0-100)</param>
        public static void PerformConeBreath(
            BaseCreature boss,
            Mobile target,
            string warcry,
            int hue,
            int rage,
            int range,
            int physicalDmg = 100,
            int fireDmg = 0,
            int coldDmg = 0,
            int poisonDmg = 0,
            int energyDmg = 0
        )
        {
            if (boss == null || boss.Deleted || !boss.Alive)
                return;

            if (target == null || target.Deleted || !target.Alive)
                return;

            int totalDamage = physicalDmg + fireDmg + coldDmg + poisonDmg + energyDmg;
            if (totalDamage != 100)
            {
                Console.WriteLine("Warning: Damage percentages for " + boss.Name + " cone breath total " + totalDamage + "%, expected 100%");
            }

            if (range < 3)
                range = 3;
            if (range > 8)
                range = 8;

            boss.PublicOverheadMessage(MessageType.Regular, hue, false, "*" + boss.Name + " is preparing an attack!*");
            boss.PlaySound(0x227);
            boss.FixedParticles(0x375A, 10, 15, 5037, hue, 0, EffectLayer.Head);

            boss.Frozen = true;

            Point3D bossLocation = boss.Location;
            Point3D targetLocation = target.Location;
            Map map = boss.Map;

            Direction direction = GetDirection(bossLocation, targetLocation);

            Timer.DelayCall(TimeSpan.FromSeconds(2.0), delegate()
            {
                if (boss.Deleted || !boss.Alive || map == null)
                    return;
                
                boss.Frozen = false;

                if (!string.IsNullOrEmpty(warcry))
                {
                    boss.PublicOverheadMessage(MessageType.Regular, hue, false, warcry);
                }

                boss.PlaySound(0x227);

                int minDamage = 55 + (rage * 3);
                int maxDamage = 65 + (rage * 3);

                List<Mobile> damagedMobiles = new List<Mobile>();

                for (int currentRange = 1; currentRange <= range; currentRange++)
                {
                    int delay = currentRange - 1;
                    int capturedRange = currentRange;

                    Timer.DelayCall(TimeSpan.FromMilliseconds(delay * 150), delegate()
                    {
                        if (boss.Deleted || !boss.Alive || map == null)
                            return;

                        List<Point3D> coneTiles = GetConeTiles(bossLocation, direction, capturedRange);

                        foreach (Point3D tile in coneTiles)
                        {
                            Effects.SendLocationParticles(
                                EffectItem.Create(tile, map, TimeSpan.FromSeconds(0.5)),
                                0x3709,
                                10,
                                30,
                                hue,
                                0,
                                5052,
                                0
                            );

                            if (capturedRange == range)
                            {
                                IPooledEnumerable eable = map.GetMobilesInRange(tile, 0);
                                foreach (Mobile m in eable)
                                {
                                    if (damagedMobiles.Contains(m) || m == boss || !m.Alive || !m.Player)
                                        continue;

                                    if (!boss.CanBeHarmful(m))
                                        continue;

                                    boss.DoHarmful(m);

                                    int damage = Utility.RandomMinMax(minDamage, maxDamage);
                                    AOS.Damage(m, boss, damage, physicalDmg, fireDmg, coldDmg, poisonDmg, energyDmg);

                                    m.FixedParticles(0x36B0, 1, 10, 5013, hue, 0, EffectLayer.Waist);
                                    damagedMobiles.Add(m);
                                }
                                eable.Free();
                            }
                        }
                        Effects.PlaySound(bossLocation, map, 0x208);
                    });
                }
            });
        }

        #endregion
        // Add this to BossSpecialAttack.cs

        #region delayed explosion
        /// <summary>
        /// Performs a delayed explosion attack that marks tiles with explosive runes
        /// </summary>
        /// <param name="boss">The boss performing the attack</param>
        /// <param name="warcry">Message displayed overhead</param>
        /// <param name="hue">Color hue for visual effects</param>
        /// <param name="radius">Radius around boss to place explosive tiles</param>
        /// <param name="rage">Boss rage level (affects damage, delay, and tile count)</param>
        /// <param name="physicalDmg">Physical damage percentage (0-100)</param>
        /// <param name="fireDmg">Fire damage percentage (0-100)</param>
        /// <param name="coldDmg">Cold damage percentage (0-100)</param>
        /// <param name="poisonDmg">Poison damage percentage (0-100)</param>
        /// <param name="energyDmg">Energy damage percentage (0-100)</param>
        public static void PerformDelayedExplosion(
            BaseCreature boss,
            string warcry,
            int hue,
            int radius,
            int rage,
            int physicalDmg = 0,
            int fireDmg = 100,
            int coldDmg = 0,
            int poisonDmg = 0,
            int energyDmg = 0
        )
        {
            if (boss == null || boss.Deleted || !boss.Alive || boss.Map == null)
                return;
        
            int totalDamage = physicalDmg + fireDmg + coldDmg + poisonDmg + energyDmg;
            if (totalDamage != 100)
            {
                Console.WriteLine("Warning: Damage percentages for " + boss.Name + " delayed explosion total " + totalDamage + "%, expected 100%");
            }
        
            int tilesToMark = Utility.RandomMinMax(8, 12) + (rage * 3);
        
            if (!string.IsNullOrEmpty(warcry))
            {
                boss.PublicOverheadMessage(MessageType.Regular, hue, false, warcry);
            }
        
            boss.PlaySound(0x233);
        
            List<Point3D> validLocations = new List<Point3D>();
        
            // Find valid spawn locations
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    Point3D loc = new Point3D(boss.X + x, boss.Y + y, boss.Z);
                    
                    if (!boss.Map.CanSpawnMobile(loc.X, loc.Y, loc.Z))
                        continue;
        
                    bool occupied = false;
                    IPooledEnumerable eable = boss.Map.GetMobilesInRange(loc, 0);
                    foreach (Mobile m in eable)
                    {
                        occupied = true;
                        break;
                    }
                    eable.Free();
        
                    if (!occupied)
                        validLocations.Add(loc);
                }
            }
        
            int marked = 0;
            while (marked < tilesToMark && validLocations.Count > 0)
            {
                int index = Utility.Random(validLocations.Count);
                Point3D loc = validLocations[index];
                validLocations.RemoveAt(index);
        
                CreateDelayedExplosiveTile(boss, loc, hue, rage, physicalDmg, fireDmg, coldDmg, poisonDmg, energyDmg);
                marked++;
            }
        }
        
        /// <summary>
        /// Creates a single explosive tile that detonates after a delay
        /// </summary>
        private static void CreateDelayedExplosiveTile(
            BaseCreature boss,
            Point3D location,
            int hue,
            int rage,
            int physicalDmg,
            int fireDmg,
            int coldDmg,
            int poisonDmg,
            int energyDmg
        )
        {
            if (boss.Map == null)
                return;
        
            new DelayedExplosiveTile(boss, location, boss.Map, hue, rage, physicalDmg, fireDmg, coldDmg, poisonDmg, energyDmg);
        }
        
        /// <summary>
        /// Represents an explosive tile that detonates after a delay
        /// </summary>
        private class DelayedExplosiveTile
        {
            private BaseCreature m_Boss;
            private Point3D m_Location;
            private Map m_Map;
            private Item m_Visual;
            private Timer m_Timer;
            private int m_Hue;
            private int m_PhysicalDmg;
            private int m_FireDmg;
            private int m_ColdDmg;
            private int m_PoisonDmg;
            private int m_EnergyDmg;
            private int m_Damage;
        
            public DelayedExplosiveTile(
                BaseCreature boss,
                Point3D location,
                Map map,
                int hue,
                int rage,
                int physicalDmg,
                int fireDmg,
                int coldDmg,
                int poisonDmg,
                int energyDmg
            )
            {
                m_Boss = boss;
                m_Location = location;
                m_Map = map;
                m_Hue = hue;
                m_PhysicalDmg = physicalDmg;
                m_FireDmg = fireDmg;
                m_ColdDmg = coldDmg;
                m_PoisonDmg = poisonDmg;
                m_EnergyDmg = energyDmg;
                m_Damage = 150 + (rage * 10);
        
                // Create visual marker
                m_Visual = new ExplosiveRuneItem(hue);
                m_Visual.MoveToWorld(location, map);
        
                // Show warning effect
                Effects.SendLocationParticles(
                    EffectItem.Create(location, map, TimeSpan.FromSeconds(0.5)),
                    0x3728, 
                    5,
                    10,
                    0,
                    0,
                    0,
                    0
                );
                Effects.PlaySound(location, map, 0x44D);
        
                // Calculate delay: base 5-9 seconds, -1 min and max per rage
                int minDelay = Math.Max(1, 5 - rage);
                int maxDelay = Math.Max(2, 9 - rage);
                double delay = Utility.RandomMinMax(minDelay * 10, maxDelay * 10) / 10.0;
                
                m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(delay), new TimerCallback(Explode));
            }
        
            public void Stop()
            {
                if (m_Timer != null && m_Timer.Running)
                    m_Timer.Stop();
        
                if (m_Visual != null && !m_Visual.Deleted)
                    m_Visual.Delete();
            }
        
            private void Explode()
            {
                if (m_Map == null || m_Boss == null || m_Boss.Deleted)
                {
                    Stop();
                    return;
                }
        
                // Explosion effect
                Effects.SendLocationParticles(
                    EffectItem.Create(m_Location, m_Map, EffectItem.DefaultDuration),
                    0x3709,
                    10,
                    30,
                    0,     
                    0,
                    5052,
                    0
                );
                
                // Smoke after explosion
                Effects.SendLocationParticles(
                    EffectItem.Create(m_Location, m_Map, TimeSpan.FromSeconds(1.5)),
                    0x3728,
                    10,
                    15,
                    0,    
                    0,
                    0,
                    0
                );
                
                Effects.PlaySound(m_Location, m_Map, 0x307);
        
                // Damage anyone standing on this tile
                IPooledEnumerable eable = m_Map.GetMobilesInRange(m_Location, 0);
                foreach (Mobile m in eable)
                {
                    if (m == null || m == m_Boss || !m.Alive)
                        continue;
        
                    if (m is BaseCreature)
                    {
                        BaseCreature bc = m as BaseCreature;
                        if (bc != null && bc.Team == m_Boss.Team)
                            continue;
                    }
        
                    AOS.Damage(m, m_Boss, m_Damage, m_PhysicalDmg, m_FireDmg, m_ColdDmg, m_PoisonDmg, m_EnergyDmg);
                    m.FixedParticles(0x3709, 10, 30, 5052, m_Hue, 0, EffectLayer.Waist);
                }
                eable.Free();
        
                if (m_Visual != null && !m_Visual.Deleted)
                    m_Visual.Delete();
            }
        
            private class ExplosiveRuneItem : Item
            {
                public ExplosiveRuneItem(int hue) : base(0x1B1F) // Fire runes
                {
                    Movable = false;
                    Hue = hue;
                    Name = "Explosive Runes";
                }
        
                public ExplosiveRuneItem(Serial serial) : base(serial)
                {
                }
        
                public override void Serialize(GenericWriter writer)
                {
                    base.Serialize(writer);
                    writer.Write((int)0);
                }
        
                public override void Deserialize(GenericReader reader)
                {
                    base.Deserialize(reader);
                    int version = reader.ReadInt();
                    
                    Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Delete));
                }
            }
        }
        #endregion
        #region degen aura
        /// <summary>
        /// Creates a degenerative aura around the boss that drains player attributes over time
        /// </summary>
        /// <param name="boss">The boss performing the attack</param>
        /// <param name="warcry">Message displayed overhead when activated</param>
        /// <param name="radius">Effect radius in tiles</param>
        /// <param name="rage">Boss rage level</param>
        /// <param name="duration">Base duration in seconds, which is further modified by rage</param>
        /// <param name="intensity">Base damage per tick, which is further modified by rage</param>
        /// <param name="attribute">Which attribute to drain: "health", "mana", or "stamina"</param>
        /// <param name="hue">Color hue for visual effects</param>
        public static void PerformDegenAura(
            BaseCreature boss,
            string warcry,
            int radius,
            int rage,
            int duration,
            int intensity,
            string attribute,
            int hue
        )
        {
            if (boss == null || boss.Deleted || !boss.Alive)
                return;

            attribute = attribute.ToLower();
            if (attribute != "health" && attribute != "mana" && attribute != "stamina")
            {
                Console.WriteLine("Warning: Invalid attribute '" + attribute + "' for DegenAura. Must be health, mana, or stamina.");
                return;
            }

            if (!string.IsNullOrEmpty(warcry))
            {
                boss.PublicOverheadMessage(MessageType.Regular, hue, false, warcry);
            }

           boss.FixedParticles(0x375A, 10, 15, 5037, hue, 0, EffectLayer.Waist);
            boss.PlaySound(0x1F5);

            int finalDuration = duration + rage*2;
            int damagePerTick = intensity + rage;

            new DegenAura(boss, radius, finalDuration, damagePerTick, attribute, hue);
        }

        /// <summary>
        /// Represents an active degenerative aura around a boss
        /// </summary>
        private class DegenAura
        {
            private BaseCreature m_Boss;
            private int m_Radius;
            private int m_DamagePerTick;
            private string m_Attribute;
            private int m_Hue;
            private Timer m_TickTimer;
            private Timer m_DurationTimer;
            private Timer m_VisualTimer;
            private int m_TicksRemaining;
            private bool m_Active;

            private const double TICK_INTERVAL = 2.0; // Damage tick every 2 seconds

            public DegenAura(
                BaseCreature boss,
                int radius,
                int durationSeconds,
                int damagePerTick,
                string attribute,
                int hue
            )
            {
                m_Boss = boss;
                m_Radius = radius;
                m_DamagePerTick = damagePerTick;
                m_Attribute = attribute;
                m_Hue = hue;
                m_TicksRemaining = (int)(durationSeconds / TICK_INTERVAL);
                m_Active = true;

                m_TickTimer = Timer.DelayCall(
                    TimeSpan.FromSeconds(TICK_INTERVAL),
                    TimeSpan.FromSeconds(TICK_INTERVAL),
                    new TimerCallback(OnTick)
                );

                m_VisualTimer = Timer.DelayCall(
                    TimeSpan.FromSeconds(0.5),
                    TimeSpan.FromSeconds(0.5),
                    new TimerCallback(ShowVisualEffect)
                );

                m_DurationTimer = Timer.DelayCall(
                    TimeSpan.FromSeconds(durationSeconds),
                    new TimerCallback(End)
                );
            }

            private void OnTick()
            {
                if (!m_Active || m_Boss == null || m_Boss.Deleted || !m_Boss.Alive)
                {
                    End();
                    return;
                }

                m_TicksRemaining--;

                if (m_TicksRemaining <= 0)
                {
                    End();
                    return;
                }

                Map map = m_Boss.Map;
                if (map == null)
                {
                    End();
                    return;
                }

                IPooledEnumerable eable = map.GetMobilesInRange(m_Boss.Location, m_Radius);

                foreach (Mobile m in eable)
                {
                    if (m == null || m == m_Boss || !m.Alive || !m.Player)
                        continue;

                    if (!m_Boss.CanBeHarmful(m))
                        continue;

                    ApplyDegen(m);
                }

                eable.Free();
            }

            private void ApplyDegen(Mobile target)
            {
                if (target == null || target.Deleted || !target.Alive)
                    return;

                switch (m_Attribute)
                {
                    case "health":
                        AOS.Damage(target, m_Boss, m_DamagePerTick, 100, 0, 0, 0, 0);
                        target.FixedParticles(0x374A, 10, 15, 5013, m_Hue, 0, EffectLayer.Waist);
                        target.PlaySound(0x1F1);
                        break;

                    case "mana":
                        int manaDrain = Math.Min(m_DamagePerTick, target.Mana);
                        target.Mana -= manaDrain;
                        target.FixedParticles(0x374A, 10, 15, 5032, m_Hue, 0, EffectLayer.Head);
                        target.PlaySound(0x1F8);
                        break;

                    case "stamina":
                        int stamDrain = Math.Min(m_DamagePerTick, target.Stam);
                        target.Stam -= stamDrain;
                        target.FixedParticles(0x374A, 10, 15, 5028, m_Hue, 0, EffectLayer.CenterFeet);
                        target.PlaySound(0x1F2);
                        break;
                }
            }

            private void ShowVisualEffect()
            {
                // Sanity
                if (!m_Active || m_Boss == null || m_Boss.Deleted || !m_Boss.Alive)
                {
                    End();
                    return;
                }

                m_Boss.FixedParticles(0x375A, 1, 10, 5037, m_Hue, 0, EffectLayer.Waist);

                if (m_Boss.Map != null && Utility.RandomBool())
                {
                    Effects.SendLocationParticles(
                        EffectItem.Create(m_Boss.Location, m_Boss.Map, TimeSpan.FromSeconds(0.5)),
                        0x3728,
                        10,
                        10,
                        m_Hue,
                        0,
                        5042,
                        0
                    );
                }
            }

            public void End()
            {
                if (!m_Active)
                    return;

                m_Active = false;

                if (m_TickTimer != null)
                {
                    m_TickTimer.Stop();
                    m_TickTimer = null;
                }

                if (m_VisualTimer != null)
                {
                    m_VisualTimer.Stop();
                    m_VisualTimer = null;
                }

                if (m_DurationTimer != null)
                {
                    m_DurationTimer.Stop();
                    m_DurationTimer = null;
                }

                if (m_Boss != null && !m_Boss.Deleted && m_Boss.Map != null)
                {
                    m_Boss.FixedParticles(0x3735, 1, 30, 9902, m_Hue, 0, EffectLayer.Waist);
                    m_Boss.PlaySound(0x1F3);
                }
            }
        }
        #endregion
        #region demonweb ritual
          /// <summary>
        /// Te'els ultimate - spawns spider webs, brings forth lightning, roots and summons nasty spiders
        /// </summary>
        /// <param name="boss">The boss performing the attack</param>
        /// <param name="hue">Color hue for visual effects</param>
        public static void PerformDemonWebRitual(
            Mobile boss,
            int hue)
        {
            if (boss == null || boss.Deleted || !boss.Alive || boss.Map == null)
                return;

            BaseCreature bc = boss as BaseCreature;

            boss.PublicOverheadMessage(
                MessageType.Regular,
                hue,
                false,
                "Lolth, I give thee my everything! Destroy these interlopers!"
            );

            boss.PlaySound(0x20F);
            boss.FixedParticles(0x376A, 10, 30, 5030, hue, 0, EffectLayer.Waist);

            Map map = boss.Map;
            Point3D center = boss.Location;

            Timer.DelayCall(TimeSpan.FromSeconds(4.0), delegate
            {
                if (boss == null || boss.Deleted || !boss.Alive || boss.Map == null)
                    return;

                List<DemonWebTile> webs = new List<DemonWebTile>();

                for (int i = 0; i < 16; i++)
                {
                    Point3D p = GetRandomEmptyTileNear(center, map, 6);

                    if (p == Point3D.Zero)
                        continue;

                    DemonWebTile web = new DemonWebTile(p, map, boss, hue);
                    web.MoveToWorld(p, map);
                    webs.Add(web);
                }

                Timer.DelayCall(TimeSpan.FromSeconds(6.0), delegate
                {
                    if (boss == null || boss.Deleted || !boss.Alive || boss.Map == null)
                        return;

                    IPooledEnumerable eable = map.GetMobilesInRange(center, 6);

                    foreach (Mobile m in eable)
                    {
                        if (m == null || m.Deleted || !m.Alive || !m.Player)
                            continue;

                        for (int i = 0; i < 2; i++)
                        {
                            Point3D spawn = FindAdjacentTile(m, map);
                            if (spawn == Point3D.Zero)
                                continue;
                            BaseCreature s = new DemonwebSpinner();
				            Point3D loc = GetSpawnLocation( bc,boss.Map );
				            s.IsTempEnemy = true;
				            s.MoveToWorld( loc, boss.Map );
				         }
                    }

                    eable.Free();
                });
            });
        }

        #endregion
        #region perform pull
         /// <summary>
        /// Pulls players close to the boss, paralyzes and optionally poisons them
        /// </summary>
        /// <param name="boss">The boss performing the attack</param>
        /// <param name="warcry">Message displayed overhead when activated</param>
        /// <param name="hue">Color hue for visual effects</param>
        /// <param name="rage">Boss rage level, determines paralyze duration</param>
        /// <param name="applyPoison">if true, deadly poison is applied</param>
        public static void PerformPull(
            Mobile boss,
            string warcry,
            int hue,
            int rage,
            bool applyPoison)
        {
            if (boss == null || boss.Deleted || !boss.Alive || boss.Map == null)
                return;

            boss.PublicOverheadMessage(MessageType.Regular, hue, false, warcry);
            boss.PlaySound(0x1F3);
            boss.FixedParticles(0x376A, 9, 32, 5030, hue, 0, EffectLayer.Waist);

            Map map = boss.Map;
            Point3D center = boss.Location;

            Timer.DelayCall(TimeSpan.FromSeconds(4.0), delegate
            {
                if (boss == null || boss.Deleted || !boss.Alive || boss.Map == null)
                    return;

                IPooledEnumerable eable = map.GetMobilesInRange(center, 6);

                foreach (Mobile m in eable)
                {
                    if (m == null || m.Deleted || !m.Alive)
                        continue;

                    if (!m.Player)
                        continue;

                    if (!boss.CanBeHarmful(m))
                        continue;

                    Point3D dest = FindAdjacentTile(boss, map);

                    if (dest == Point3D.Zero)
                        continue;

                    Effects.SendMovingEffect(
                        m,
                        boss,
                        0x3728,
                        10,
                        0,
                        false,
                        false,
                        hue,
                        0
                    );

                    m.MoveToWorld(dest, map);
                    m.Paralyze(TimeSpan.FromSeconds(rage * 3));

                    if (applyPoison)
                        m.ApplyPoison(boss, Poison.Deadly);

                    m.FixedParticles(0x3728, 10, 15, 5044, hue, 0, EffectLayer.Waist);
                    m.PlaySound(0x204);
                }
                eable.Free();
            });
        }

        #endregion

        #region resist breach
        /// <summary>
        /// Performs a telegraphed AoE attack that reduces a specific resistance on all nearby players
        /// </summary>
        /// <param name="boss">The boss performing the attack</param>
        /// <param name="target">The mobile that will be the center of the effect</param>
        /// <param name="warcry">Message displayed overhead</param>
        /// <param name="hue">Color hue for visual effects</param>
        /// <param name="rage">Boss rage level (affects sap amount and duration)</param>
        /// <param name="radius">Radius around the target in which players are affected</param>
        /// <param name="resist">Resistance type to sap: "physical", "fire", "cold", "poison", or "energy"</param>
        public static void PerformResistBreach(
            BaseCreature boss,
            Mobile target,
            string warcry,
            int hue,
            int rage,
            int radius,
            string resist
        )
        {
            if (boss == null || boss.Deleted || !boss.Alive)
                return;

            if (target == null || target.Deleted || !target.Alive)
                return;

            resist = resist.ToLower();

            ResistanceType resistType;
            if (!TryParseResistType(resist, out resistType))
            {
                Console.WriteLine("Warning: Invalid resist type '" + resist + "' passed to PerformResistBreach. Must be physical, fire, cold, poison, or energy.");
                return;
            }

            if (!string.IsNullOrEmpty(warcry))
            {
                boss.PublicOverheadMessage(MessageType.Regular, hue, false, warcry);
            }

            boss.FixedParticles(0x3709, 10, 30, 5052, hue, 0, EffectLayer.Waist);
            boss.PlaySound(0x208);

            Point3D targetLocation = target.Location;
            Map targetMap = target.Map;

            Timer.DelayCall(TimeSpan.FromSeconds(TELEGRAPH_DELAY), delegate()
            {
                if (boss.Deleted || !boss.Alive || targetMap == null)
                    return;

                int sapAmount  = 30 + rage * 3;
                int duration   = 10 + rage * 3;

                Effects.PlaySound(targetLocation, targetMap, 0x208);

                IPooledEnumerable eable = targetMap.GetMobilesInRange(targetLocation, radius);

                foreach (Mobile m in eable)
                {
                    if (m == null || m == boss || !m.Player || !m.Alive)
                        continue;

                    if (!boss.CanBeHarmful(m))
                        continue;

                    boss.DoHarmful(m);

                    m.FixedParticles(0x3709, 10, 30, 5052, hue, 0, EffectLayer.Waist);
                    m.PlaySound(0x208);

                    ApplyResistBreach(m, boss, resistType, sapAmount, duration, resist);
                }
                eable.Free();
            });
        }

        /// <summary>
        /// Applies the resist sap modifier to a single mobile and schedules its removal
        /// </summary>
        private static void ApplyResistBreach(Mobile m, BaseCreature boss, ResistanceType resistType, int sapAmount, int duration, string resistLabel)
        {
            RemoveResistBreach(m, resistType);

            ResistanceMod mod = new ResistanceMod(resistType, -sapAmount);
            m.AddResistanceMod(mod);

            if (!m_ActiveResistBreaches.ContainsKey(m))
                m_ActiveResistBreaches[m] = new Dictionary<ResistanceType, ResistanceMod>();

            m_ActiveResistBreaches[m][resistType] = mod;

            m.SendMessage("Your " + resistLabel + " resistance has been sapped by " + sapAmount + " for " + duration + " seconds!");

            Timer.DelayCall(TimeSpan.FromSeconds(duration), delegate()
            {
                if (m == null || m.Deleted)
                    return;

                RemoveResistBreach(m, resistType);

                if (m.Alive)
                    m.SendMessage("Your " + resistLabel + " resistance has returned to normal.");
            });
        }

        /// <summary>
        /// Removes a tracked resist breach mod from a mobile if one exists
        /// </summary>
        private static void RemoveResistBreach(Mobile m, ResistanceType resistType)
        {
            if (m == null || !m_ActiveResistBreaches.ContainsKey(m))
                return;

            Dictionary<ResistanceType, ResistanceMod> mods = m_ActiveResistBreaches[m];

            if (!mods.ContainsKey(resistType))
                return;

            m.RemoveResistanceMod(mods[resistType]);
            mods.Remove(resistType);

            if (mods.Count == 0)
                m_ActiveResistBreaches.Remove(m);
        }

        /// <summary>
        /// Converts a resist string to a ResistanceType enum value
        /// </summary>
        private static bool TryParseResistType(string resist, out ResistanceType result)
        {
            switch (resist)
            {
                case "physical": result = ResistanceType.Physical; return true;
                case "fire":     result = ResistanceType.Fire;     return true;
                case "cold":     result = ResistanceType.Cold;     return true;
                case "poison":   result = ResistanceType.Poison;   return true;
                case "energy":   result = ResistanceType.Energy;   return true;
                default:         result = ResistanceType.Physical; return false;
            }
        }
        #endregion
        #region helpers
        private static Point3D GetRandomEmptyTileNear(Point3D center, Map map, int range)
        {
            for (int i = 0; i < 20; i++)
            {
                int x = center.X + Utility.RandomMinMax(-range, range);
                int y = center.Y + Utility.RandomMinMax(-range, range);
                int z = center.Z;

                Point3D p = new Point3D(x, y, z);

                if (map.CanFit(p, 16, false, false))
                    return p;
            }

            return Point3D.Zero;
        }

        public class DemonWebTile : Item
        {
            private Mobile m_Boss;
            private int m_Hue;
            private Timer m_LightningTimer;

            public DemonWebTile(Point3D loc, Map map, Mobile boss, int hue)
                : base(0x10D3)
            {
                Movable = false;
                Hue = hue;

                m_Boss = boss;
                m_Hue = hue;

                MoveToWorld(loc, map);

                m_LightningTimer = Timer.DelayCall(
                    TimeSpan.FromSeconds(3.0),
                    TimeSpan.FromSeconds(3.0),
                    StrikeLightning
                );
                Timer.DelayCall(TimeSpan.FromSeconds(12.0), Delete);
            }

            public override bool OnMoveOver(Mobile m)
            {
                if (m == null || m.Deleted || !m.Alive || !m.Player)
                    return true;

                m.Paralyze(TimeSpan.FromSeconds(1));
                return true;
            }

            private void StrikeLightning()
            {
                if (Deleted || Map == null)
                {
                    StopTimer();
                    return;
                }

                IPooledEnumerable eable = GetMobilesInRange(0);

                foreach (Mobile m in eable)
                {
                    if (m == null || m.Deleted || !m.Alive || !m.Player)
                        continue;

                    Effects.SendBoltEffect(m, true, m_Hue);

                    int damage = Utility.RandomMinMax(75, 95);
                    AOS.Damage(m, m_Boss, damage, 0, 0, 0, 50, 50);
                }

                eable.Free();
            }

            private void StopTimer()
            {
                if (m_LightningTimer != null)
                {
                    m_LightningTimer.Stop();
                    m_LightningTimer = null;
                }
            }

            public override void OnDelete()
            {
                StopTimer();
                base.OnDelete();
            }

            public DemonWebTile(Serial serial) : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write((int)1); // version

                writer.Write(m_Boss);
                writer.Write(m_Hue);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();

                if (version >= 1)
                {
                    m_Boss = reader.ReadMobile();
                    m_Hue = reader.ReadInt();
                }

                m_LightningTimer = Timer.DelayCall(
                    TimeSpan.FromSeconds(3.0),
                    TimeSpan.FromSeconds(3.0),
                    StrikeLightning
                );

                Timer.DelayCall(TimeSpan.FromSeconds(12.0), Delete);
            }
        }


        private static Point3D FindAdjacentTile(Mobile boss, Map map)
        {
            int x = boss.X;
            int y = boss.Y;
            int z = boss.Z;

            for (int i = 0; i < 8; i++)
            {
                int dx = 0, dy = 0;

                switch (i)
                {
                    case 0: dx = 1; break;
                    case 1: dx = -1; break;
                    case 2: dy = 1; break;
                    case 3: dy = -1; break;
                    case 4: dx = 1; dy = 1; break;
                    case 5: dx = -1; dy = 1; break;
                    case 6: dx = 1; dy = -1; break;
                    case 7: dx = -1; dy = -1; break;
                }

                Point3D p = new Point3D(x + dx, y + dy, z);

                if (map.CanFit(p, 16, false, false))
                    return p;
            }

            return Point3D.Zero;
        }

        /// <summary>
        /// Cross shaped AoE attack
        /// </summary>
        /// <param name="center">origin point</param>
        /// <param name="radius">how far the cross goes</param>
        public static Point3D[] CrossPoints(Point3D center, int radius)
        {
            List<Point3D> points = new List<Point3D>();
            points.Add(center);

            for (int i = 1; i <= radius; i++)
            {
                points.Add(new Point3D(center.X + i, center.Y, center.Z)); // East
                points.Add(new Point3D(center.X - i, center.Y, center.Z)); // West
                points.Add(new Point3D(center.X, center.Y + i, center.Z)); // South
                points.Add(new Point3D(center.X, center.Y - i, center.Z)); // North
            }
            return points.ToArray();
        }
        /// <summary>
        /// Builds a random rampage path for the boss
        /// </summary>
        private static List<Point3D> BuildRampagePath(BaseCreature boss, int chargeCount)
        {
            List<Point3D> path = new List<Point3D>();
            Point3D currentLoc = boss.Location;

            for (int charge = 0; charge < chargeCount; charge++)
            {
                Direction chargeDir = (Direction)Utility.Random(8);
                int chargeDist = Utility.RandomMinMax(6, 8);

                for (int step = 0; step < chargeDist; step++)
                {
                    Point3D nextLoc = GetPointInDirection(currentLoc, chargeDir, 1);

                    if (boss.Map.CanSpawnMobile(nextLoc))
                    {
                        path.Add(nextLoc);
                        currentLoc = nextLoc;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return path;
        }
        /// <summary>
        /// Gets a point in a specific direction from a location
        /// </summary>
        private static Point3D GetPointInDirection(Point3D from, Direction d, int distance)
        {
            int x = from.X;
            int y = from.Y;

            switch (d & Direction.Mask)
            {
                case Direction.North: y -= distance; break;
                case Direction.South: y += distance; break;
                case Direction.West: x -= distance; break;
                case Direction.East: x += distance; break;
                case Direction.Right: x += distance; y -= distance; break;
                case Direction.Left: x -= distance; y += distance; break;
                case Direction.Down: x += distance; y += distance; break;
                case Direction.Up: x -= distance; y -= distance; break;
            }

            return new Point3D(x, y, from.Z);
        }
        /// <summary>
        /// Gets a valid spawn location near the boss
        /// </summary>
        private static Point3D GetSpawnLocation(BaseCreature boss, Map map)
        {
            for (int j = 0; j < 20; ++j)
            {
                int x = boss.X + Utility.Random(13) - 6;
                int y = boss.Y + Utility.Random(13) - 6;
                int z = map.GetAverageZ(x, y);

                if (map.CanFit(x, y, boss.Z, 16, false, false))
                    return new Point3D(x, y, boss.Z);
                else if (map.CanFit(x, y, z, 16, false, false))
                    return new Point3D(x, y, z);
            }

            return boss.Location;
        }
        private static int GetParalyzeDuration(Mobile m, int rage)
		{
			int resist = (int)(m.Skills.MagicResist.Value);
			// base 2s at 125, caps 8s at 0 magic resist, adds rage afterwards
			int duration = (8 - (int)(resist * (6.0 / 125.0))) + rage;
            if (duration < 2 )
                duration = 2;
			return duration;
		}

        private class BossBleedEntry
        {
            public Mobile Mobile;
            public Timer Timer;

            public BossBleedEntry(Mobile m, Timer t)
            {
                Mobile = m;
                Timer = t;
            }
        }

        private static BossBleedEntry FindBleedEntry(Mobile m)
        {
            for (int i = 0; i < m_BossBleeds.Count; i++)
            {
                BossBleedEntry entry = (BossBleedEntry)m_BossBleeds[i];

                if (entry.Mobile == m)
                    return entry;
            }        
            return null;
        }
        private static bool IsBleedImmune(Mobile m)
        {
            TransformContext context = TransformationSpellHelper.GetContext(m);
            if (context != null && (context.Type == typeof(LichFormSpell) || context.Type == typeof(WraithFormSpell)))
                return true;

            if (m is BaseCreature && ((BaseCreature)m).BleedImmune)
                return true;

            return false;
        }

        public static void BeginBossBleed(Mobile m, Mobile from, int totalTicks)
        {
            BossBleedEntry entry = FindBleedEntry(m);

            if (entry != null)
            {
                if (entry.Timer != null)
                    entry.Timer.Stop();

                m_BossBleeds.Remove(entry);
            }

            Timer t = new BossBleedTimer(from, m, totalTicks);
            m_BossBleeds.Add(new BossBleedEntry(m, t));
            t.Start();
        }


        public static void DoBossBleed(Mobile m, Mobile from, int level)
        {
            if (!m.Alive || !Server.Items.BaseRace.IsBleeder(m) || m == null || m.Deleted )
            {
                EndBossBleed(m, false);
                return;
            }

            int damage = Utility.RandomMinMax(level * 3, level * 5);
            
            if (!m.Player)
                damage *= 2;

            m.PlaySound(0x133);
            AOS.Damage(m, from, damage, 100, 0, 0, 0, 0);

            Blood blood = new Blood();
            blood.ItemID = Utility.Random(0x122A, 5);
            blood.MoveToWorld(m.Location, m.Map);
            m.FixedParticles(0x377A, 1, 15, 9502, 67, 7, EffectLayer.Waist);
        }

        public static void EndBossBleed(Mobile m, bool message)
        {
            BossBleedEntry entry = FindBleedEntry(m);

            if (entry == null)
                return;

            if (entry.Timer != null)
                entry.Timer.Stop();

            m_BossBleeds.Remove(entry);

            if (message && m is PlayerMobile)
                m.SendMessage("The bleeding has stopped.");
        }


        private class BossBleedTimer : Timer
        {
            private Mobile m_From;
            private Mobile m_Mobile;
            private int m_Count;
            private int m_MaxTicks;

            public BossBleedTimer(Mobile from, Mobile m, int maxTicks) 
                : base(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0))
            {
                m_From = from;
                m_Mobile = m;
                m_MaxTicks = maxTicks;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                DoBossBleed(m_Mobile, m_From, m_MaxTicks - m_Count);
                
                if (++m_Count == m_MaxTicks)
                    EndBossBleed(m_Mobile, true);
            }
        }

        private static void LightTilesOnFire( Point3D center, Map map, int radius, int hue )
        {
        	if ( map == null )
        		return;

        	for ( int x = -radius; x <= radius; x++ )
        	{
        		for ( int y = -radius; y <= radius; y++ )
        		{
        			Point3D loc = new Point3D( center.X + x, center.Y + y, center.Z );

        			int dx = center.X - loc.X;
        			int dy = center.Y - loc.Y;
        			double distance = Math.Sqrt( (dx * dx) + (dy * dy) );

        			if ( distance > radius )
        				continue;

        			Effects.SendLocationParticles(
        				EffectItem.Create( loc, map, TimeSpan.FromSeconds( 2.0 ) ),
        				0x3709,
        				10,
        				30,
        				hue,
        				0,
        				5052,
        				0
        			);
        		}
        	}
        }

        private static void SetOnFire( Mobile m, int hue )
        {
        	if ( m == null || m.Deleted )
        		return;

        	m.FixedParticles(
        		0x3709,
        		10,
        		30,
        		5052,
        		hue,
        		0,
        		EffectLayer.Waist
        	);
        }

        private static List<Point3D> GetConeTiles(Point3D origin, Direction direction, int range)
        {
            List<Point3D> tiles = new List<Point3D>();

            int width = range;

            int dx = 0, dy = 0;
            GetDirectionOffset(direction, ref dx, ref dy);

            int perpX = 0, perpY = 0;
            GetPerpendicularOffset(direction, ref perpX, ref perpY);

            for (int offset = -(width - 1); offset <= (width - 1); offset++)
            {
                // Skip corners based on range and offset to create cone shape
                int absOffset = Math.Abs(offset);
                if (absOffset > range)
                    continue;

                int x = origin.X + (dx * range) + (perpX * offset);
                int y = origin.Y + (dy * range) + (perpY * offset);

                tiles.Add(new Point3D(x, y, origin.Z));
            }

            return tiles;
        }

        /// <summary>
        /// Gets the primary direction from origin to target
        /// </summary>
        private static Direction GetDirection(Point3D from, Point3D to)
        {
            int dx = to.X - from.X;
            int dy = to.Y - from.Y;

            // Determine primary direction
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                // horizontal
                if (dx > 0)
                    return Direction.East;
                else
                    return Direction.West;
            }
            else
            {
                // vertical
                if (dy > 0)
                    return Direction.South;
                else
                    return Direction.North;
            }
        }

        /// <summary>
        /// Gets the X and Y offset for a direction
        /// </summary>
        private static void GetDirectionOffset(Direction direction, ref int dx, ref int dy)
        {
            switch (direction & Direction.Mask)
            {
                case Direction.North: dx = 0; dy = -1; break;
                case Direction.South: dx = 0; dy = 1; break;
                case Direction.East: dx = 1; dy = 0; break;
                case Direction.West: dx = -1; dy = 0; break;
                case Direction.Right: dx = 1; dy = -1; break;
                case Direction.Down: dx = 1; dy = 1; break;
                case Direction.Left: dx = -1; dy = 1; break;
                case Direction.Up: dx = -1; dy = -1; break;
            }
        }

        /// <summary>
        /// Gets perpendicular offsets for cone spread
        /// </summary>
        private static void GetPerpendicularOffset(Direction direction, ref int px, ref int py)
        {
            switch (direction & Direction.Mask)
            {
                case Direction.North:
                case Direction.South:
                    px = 1; py = 0; // Horizontal spread for vertical breath
                    break;
                case Direction.East:
                case Direction.West:
                    px = 0; py = 1; // Vertical spread for horizontal breath
                    break;
                case Direction.Right:
                case Direction.Left:
                case Direction.Down:
                case Direction.Up:
                    px = 1; py = 0; // Default for diagonal
                    break;
            }
        }
        #endregion
    }
}