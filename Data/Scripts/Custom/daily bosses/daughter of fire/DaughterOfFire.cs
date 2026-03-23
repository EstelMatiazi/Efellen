using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Commands;
using Server.Commands.Generic;
using Server.Spells.Necromancy;
using Server.Spells;
using Server.EffectsUtil;
using Server.Custom;
using Server.Custom.DailyBosses.System;
using Server.Custom.BossSystems;
using Server.Custom.Ascensions;

namespace Server.Mobiles
{
	[CorpseName( "Daughter of Fire's Corpse" )]
	public class DaughterOfFire : BaseCreature
	{
		private static readonly Type[] SummonTypes = new Type[] 
		{ 
			typeof(Penitent), 
			typeof(FireGargoyle), 
			typeof(FireElemental), 
			typeof(Efreet),
			typeof(Succubus) 
		};

		private static readonly string[] SummonWarcries = new string[]
		{
			"Fire walks with me!",
			"Soon you shall be ash and memory!",
			"We will turn your hopes and dreams into cinder!",
			"Hellfire shall bear witness to your demise!"
		};
		
		private static readonly List<Type> BossDrops = new List<Type>
    	{
    	    typeof(Artifact_GlovesOfThePainSlave),
    	    typeof(Artifact_LegsOfThePainSlave),
    	    typeof(Artifact_ArmsOfThePainSlave),
    	    typeof(Artifact_BootsOfThePainSlave),
			typeof(Artifact_ChestOfThePainSlave),
			typeof(Artifact_ExquisiteAgony)
    	};

		private int m_Rage = 0;
		private Mobile m_LastTarget;
		private DateTime m_NextSummonTime = DateTime.MinValue;
		private DateTime m_NextSpecialAttack = DateTime.MinValue;
		private List<BaseCreature> m_Summons = new List<BaseCreature>();

		[Constructable]
		public DaughterOfFire () : base( AIType.AI_Mage, FightMode.Closest, 20, 1, 0.4, 0.8 )
		{
			Name = "Daughter of Fire";
            Title = "The Emissary of Pain";
			Body = 149;
			BaseSoundID = 0x4B0;
			NameHue = 0x22;
			Hue = 0xb73;

			SetStr( 696, 685 );
			SetDex( 185, 205 );
			SetInt( 486, 475 );

			SetHits( 7000 );
			SetDamage( 11, 15 );

			SetDamageType( ResistanceType.Fire, 100 );
			SetResistance( ResistanceType.Physical, 50 );
			SetResistance( ResistanceType.Fire, 75 );
			SetResistance( ResistanceType.Cold, 70 );
			SetResistance( ResistanceType.Poison, 70 );
			SetResistance( ResistanceType.Energy, 70 );

			SetSkill( SkillName.Magery, 100.0 );
			SetSkill( SkillName.Meditation, 115.0 );
			SetSkill( SkillName.MagicResist, 115.5, 135.0 );
			SetSkill( SkillName.Tactics, 111.0 );
			SetSkill( SkillName.FistFighting, 111.0 );
			SetSkill( SkillName.Spiritualism, 116.0);
			SetSkill( SkillName.Necromancy, 116.0);

			Fame = 25000;
			Karma = -25000;

			VirtualArmor = 40;

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 4 );
		}

		public override bool AutoDispel{ get{ return !Controlled; } }
		public override int TreasureMapLevel{ get{ return 4; } }
		public override int Hides{ get{ return 38; } }
		public override HideType HideType{ get{ return HideType.Hellish; } }
		public override int Skin{ get{ return Utility.Random(9); } }
		public override SkinType SkinType{ get{ return SkinType.Lava; } }
		public override int Skeletal{ get{ return Utility.Random(9); } }
		public override SkeletalType SkeletalType{ get{ return SkeletalType.Devil; } }
		public override bool CanRummageCorpses{ get{ return false; } }
		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool BleedImmune{ get{ return true; } }
		public override bool BardImmune { get { return true; } }
		public override bool Unprovokable { get { return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }

		public override void OnThink()
		{
		    base.OnThink();

		    Mobile combatant = this.Combatant;

		    if (combatant == null || combatant.Deleted || !combatant.Alive)
		        return;

		    BossSummonSystem.TrySummonCreature(
		        this,
		        combatant,
		        SummonTypes,
		        m_Rage,
		        ref m_NextSummonTime,
		        SummonWarcries,
		        m_Summons,
		        1316,
		        GetMaxSummons(),
		        40
		    );

		    if (DateTime.UtcNow >= m_NextSpecialAttack)
		    {
		        PerformRageAttack(combatant);
		        m_NextSpecialAttack = DateTime.UtcNow + TimeSpan.FromSeconds(25 - (m_Rage * 2));
		    }

		    m_LastTarget = combatant;
		}

		private void PerformRageAttack( Mobile target )
		{
			if ( target == null || target.Deleted || !target.Alive )
				return;

			
			int attackChoice = Utility.RandomMinMax( 1, 3 );
            Map map = this.Map;

			switch ( attackChoice  )
			{
				case 1: // Flaming blast
				{
					BossSpecialAttack.PerformSlam(
                       boss: this,
                       warcry: "BURN!",
                       hue: 0xb73,
                       rage: m_Rage+1,
                       range: 6,
					   physicalDmg: 0,
                       fireDmg: 100
                   );
                   break;
				}
				case 2: // rage 2: summon fire vortex
				{
					BossSpecialAttack.SummonHonorGuard(
                        boss: this,
                        target: target,
                        warcry: "Awake the volcano!",
                        amount: 4,
                        creatureType: typeof(EvilScorchingVortex),
                        hue: 0xb73
                    );
                    break;
				}
				case 3: // Rage 3: fire whip
				{
					if (map == null)
                        return;
                    int range = 7;
                    this.PlaySound(0x208);
                    PublicOverheadMessage(Network.MessageType.Emote, 0x22, false, "*lashes a blazing whip of fire!*");
                    Point3D start = this.Location;
                    int dx = 0, dy = 0;
					int minDamage = (50+m_Rage*3);
					int maxDamage = (70+m_Rage*4);
                    GetDirectionVector(this.Direction, out dx, out dy);
                    for (int i = 1; i <= range; i++)
                    {
                        Point3D p = new Point3D(start.X + dx * i, start.Y + dy * i, start.Z);
                        Effects.SendLocationEffect(p, map, 0x36D4, 20, 10, 0xb73, 0);
                        foreach (Mobile m in map.GetMobilesInRange(p, 0))
                        {
                            if (m != null && m != this && !m.Deleted && CanBeHarmful(m))
                            {
                                DoHarmful(m);
                                m.Damage(Utility.RandomMinMax(minDamage, maxDamage), this);
                                m.PlaySound(0x15E);
                            }
                        }
                    }
                break;
			    }
			}
		}

        private void GetDirectionVector(Direction d, out int dx, out int dy)
        {
            dx = 0;
            dy = 0;

            switch (d)
            {
                case Direction.North:
                    dy = -1;
                    break;
                case Direction.Right:
                    dx = 1;
                    dy = -1;
                    break;
                case Direction.East:
                    dx = 1;
                    break;
                case Direction.Down:
                    dx = 1;
                    dy = 1;
                    break;
                case Direction.South:
                    dy = 1;
                    break;
                case Direction.Left:
                    dx = -1;
                    dy = 1;
                    break;
                case Direction.West:
                    dx = -1;
                    break;
                case Direction.Up:
                    dx = -1;
                    dy = -1;
                    break;
            }
        }
	
		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			reflect = ( Utility.Random( 100 ) < m_Rage * 12 );
		}

		private int GetMaxSummons()
		{
			switch( m_Rage )
			{
				case 0: return 8;
				case 1: return 7;
				case 2: return 6;
				case 3: return 5;
				default: return 5;
			}
		}

		public override bool OnBeforeDeath()
		{
			if ( m_Rage == 0 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "We are just getting started, honey!" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				SetDamage( 16, 20 );
				VirtualArmor += 5;
				m_Rage = 1;
				return false;
			}
			else if ( m_Rage == 1 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Feeling the heat yet?" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				SetDamage( 21, 25 );
				VirtualArmor += 5;
				
				m_Rage = 2;
				return false;
			}
			else if ( m_Rage == 2 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Hellfire shall be your eternal lover!" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				SetDamage( 26, 30 );
				VirtualArmor += 10;
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Fool! I'm fire everlasting" );
				m_Rage = 3;
				return false;
			}
			else 
			{
				Effects.SendLocationParticles( EffectItem.Create( this.Location, this.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
				this.PlaySound( 0x1FE );
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "You win...this time..." );
				Mobile killer = this.LastKiller;
				if (killer != null && killer.Player && killer.Karma > 0)
            	{
            	    int marks = Utility.RandomMinMax(103, 110);
            	    Server.Custom.DefenderOfTheRealm.MarkLootHelper.AwardMarks(killer, 1, marks);
            	}
			}
			
			return base.OnBeforeDeath();
		}

		public override void OnDelete()
		{
		    if (m_Summons != null)
		    {
		        BossSummonSystem.CleanupSummons(m_Summons);
		        m_Summons.Clear();
		        m_Summons = null;
		    }

		    base.OnDelete();
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			BossLootSystem.AwardBossSpecial(this,BossDrops, 15);
			for ( int i = 0; i < 3; i++ )
			{
				c.DropItem( Loot.RandomArty() );
				c.DropItem( new EtherealPowerScroll() );
				c.DropItem( AscensionScrollFactory.CreateRandom());
			}
			// gold explosion
		    RichesSystem.SpawnRiches( m_LastTarget, 3 );
		}

		public override void OnAfterSpawn()
		{
			base.OnAfterSpawn();
			LeechImmune = true;
		}

		public DaughterOfFire( Serial serial ) : base( serial )
		{
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			m_LastTarget = from;
			if (Utility.RandomDouble() < 0.40 )
				Server.Misc.IntelligentAction.LeapToAttacker( this, from );

			base.OnDamage( amount, from, willKill );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version

			writer.Write( m_Rage );
			writer.Write( m_NextSummonTime );
			writer.Write( m_NextSpecialAttack );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( version >= 1 )
			{
				m_Rage = reader.ReadInt();
				m_NextSummonTime = reader.ReadDateTime();
				m_NextSpecialAttack = reader.ReadDateTime();
			}

			LeechImmune = true;
			// Initialize summons list if null
			if (m_Summons == null)
				m_Summons = new List<BaseCreature>();
		}
	}
}