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
using Server.Spells;
using Server.EffectsUtil;
using Server.Custom;
using Server.Custom.DailyBosses.System;
using Server.Custom.BossSystems;
using Server.Custom.Ascensions;
namespace Server.Mobiles
{
	[CorpseName( "Herald of Fire's Corpse" )]
	public class HeraldOfCinders : BaseCreature
	{
		private const int MAGMA_DURATION = 12;
		
		private static readonly Type[] SummonTypes = new Type[] 
		{  
			typeof(Drake), 
			typeof(Wyrm), 
			typeof(Dragons),
			typeof(AncientDrake),
			typeof(AncientWyrm) 
		};

		private static readonly string[] SummonWarcries = new string[]
		{
			"Feed the wyrmlings!",
			"Come, children, it's time to feast!",
			"Thou shall not leave my lair unscathed! Come forth, my spawn!",
			"Spawn of Ashardalon, Awaken!"
		};

		private static readonly List<Type> BossDrops = new List<Type>
    	{
    	    typeof(Artifact_CinderForgedArms),
    	    typeof(Artifact_CinderForgedGloves),
    	    typeof(Artifact_CinderForgedHelm),
    	    typeof(Artifact_CinderForgedLeggings),
			typeof(Artifact_CinderForgedBreastplate),
			typeof(Artifact_CinderForgedMaul)
    	};

		private static readonly Type[] ImmuneToMagmaTypes = new Type[]
		{
			typeof(BabyDragon),
			typeof(YoungDragon),
			typeof(Drake),
			typeof(Wyrm),
			typeof(AncientDrake),
			typeof(HeraldOfCinders)
		};

		private int m_Rage = 0;
		private Mobile m_LastTarget;
		private DateTime m_NextSummonTime = DateTime.MinValue;
		private DateTime m_NextSpecialAttack = DateTime.MinValue;
		private List<BaseCreature> m_Summons = new List<BaseCreature>();
		private List<MagmaTile> m_MagmaTiles = new List<MagmaTile>();

		[Constructable]
		public HeraldOfCinders () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "The Herald of Cinders";
			Title = "The Everlasting Flame";
			Body = 713;
			BaseSoundID = 362;
			NameHue = 0x22;
			Hue = 0x81b;
			
			SetStr( 896, 985 );
			SetDex( 125, 175 );
			SetInt( 586, 675 );

			SetHits( 19000 );
			SetDamage( 11, 16 );

			SetDamageType( ResistanceType.Physical, 100 );
		
			SetResistance( ResistanceType.Physical, 60 );
			SetResistance( ResistanceType.Fire, 80 );
			SetResistance( ResistanceType.Cold, 40 );
			SetResistance( ResistanceType.Poison, 70 );
			SetResistance( ResistanceType.Energy, 70 );

			SetSkill( SkillName.Meditation, 112.5, 125.0 );
			SetSkill( SkillName.MagicResist, 125.5, 150.0 );
			SetSkill( SkillName.Tactics, 101.0, 125.0 );
			SetSkill( SkillName.FistFighting, 101.0, 125.0 );
			SetSkill( SkillName.Magery, 120.0, 125.0);

			Fame = 35000;
			Karma = -35000;
			VirtualArmor = 60;

			if ( Backpack == null )
				AddItem( new Backpack() );

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 8 );
		}

		public override int TreasureMapLevel{ get{ return 5; } }
		public override bool CanRummageCorpses{ get{ return false; } }
		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool BleedImmune{ get{ return true; } }
		public override bool BardImmune { get { return true; } }
		public override bool Unprovokable { get { return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }

		public override void Damage( int amount, Mobile from )
		{
			if ( m_Rage < 3 && this.Hits - amount <= 0 )
			{
				IncreaseRage(m_Rage);
			}
			else
			{
				base.Damage( amount, from );
			}
		}

		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			reflect = ( Utility.Random( 100 ) < m_Rage * 20 );
		}

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
		        30
		    );

		    if (DateTime.UtcNow >= m_NextSpecialAttack)
		    {
		        PerformRageAttack(combatant);
		        m_NextSpecialAttack = DateTime.UtcNow + TimeSpan.FromSeconds(30 - (m_Rage * 2));
		    }

		    m_LastTarget = combatant;
		}
	
		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			m_LastTarget = from;
			if (Utility.RandomDouble() < 0.75 )
				Server.Misc.IntelligentAction.LeapToAttacker( this, from );
			
			if (Utility.RandomDouble() < 0.025 )
			{
				BossSpecialAttack.PerformConeBreath(
				    boss: this,
				    target: from,
				    warcry: "*exhales devastating flames!*",
				    hue: 1160,
				    rage: m_Rage+1,
				    range: 5,
					physicalDmg:0,
				    fireDmg: 100
				);	
			}
			
			base.OnDamage( amount, from, willKill );
		}

		private void PerformRageAttack( Mobile target )
		{
			if ( target == null || target.Deleted || !target.Alive )
				return;

			
			int attackChoice = Utility.RandomMinMax( 1, 3 );

			switch ( attackChoice )
			{
				case 1: // fire cone
					{
						BossSpecialAttack.PerformConeBreath(
				    	    boss: this,
				    	    target: target,
				    	    warcry: "*exhales devastating flames!*",
				    	    hue: 1160,
				    	    rage: m_Rage+1,
				    	    range: 8,
							physicalDmg:0,
				    	    fireDmg: 100
				    	);
				    break;
					}
				case 2: // flame burst
					{
						BossSpecialAttack.PerformTargettedAoE(
							this,
							target,
							m_Rage+1,
							"We are fire eternal!",
							1160,  // hue
							0,     // physical
							100,   // fire
							0,     // cold
							0,     // poison
							0      // energy
						);
						break;
					}
				case 3: // magma eruption
					{
						PerformMagmaEruption();
						break;
					}
			}
		}

		private void PerformMagmaEruption()
		{
			if ( Map == null )
				return;

			PublicOverheadMessage( MessageType.Regular, 0x21, false, "The ground trembles with fury!" );
			PlaySound( 0x307 );

			int tileCount = Utility.RandomMinMax( 22, 32 );
			List<Point3D> validLocations = new List<Point3D>();

			for ( int attempts = 0; attempts < tileCount * 3 && validLocations.Count < tileCount; attempts++ )
			{
				int range = Utility.RandomMinMax( 4, 16 );
				int xOffset = Utility.RandomMinMax( -range, range );
				int yOffset = Utility.RandomMinMax( -range, range );

				Point3D loc = new Point3D( X + xOffset, Y + yOffset, Z );

				if ( Map.CanFit( loc, 16, false, false ) && !IsLocationOccupied( loc ) )
				{
					validLocations.Add( loc );
				}
			}

			foreach ( Point3D loc in validLocations )
			{
				Timer.DelayCall( TimeSpan.FromSeconds( Utility.RandomDouble() * 0.5 ), delegate()
				{
					if ( Deleted || !Alive )
						return;

					Effects.SendLocationEffect( loc, Map, 0x36B0, 20, 10, 1160, 0 );
					Effects.PlaySound( loc, Map, 0x11D );

					Timer.DelayCall( TimeSpan.FromSeconds( 0.5 ), delegate()
					{
						MagmaTile magma = new MagmaTile( this, loc, Map );
						m_MagmaTiles.Add( magma );

						Timer.DelayCall( TimeSpan.FromSeconds( MAGMA_DURATION ), delegate()
						{
							if ( magma != null )
							{
								m_MagmaTiles.Remove( magma );
								magma.Delete();
							}
						});
					});
				});
			}
		}

		private bool IsLocationOccupied( Point3D loc )
		{
			IPooledEnumerable eable = Map.GetMobilesInRange( loc, 0 );
			bool occupied = false;

			foreach ( Mobile m in eable )
			{
				occupied = true;
				break;
			}

			eable.Free();
			return occupied;
		}

		public static bool IsImmuneToMagma( Mobile m )
		{
			if ( m == null )
				return false;

			Type mType = m.GetType();
			foreach ( Type immuneType in ImmuneToMagmaTypes )
			{
				if ( mType == immuneType )
					return true;
			}

			return false;
		}

		private int GetMaxSummons()
		{
			switch( m_Rage )
			{
				case 0: return 12;
				case 1: return 10;
				case 2: return 8;
				case 3: return 6;
				default: return 12;
			}
		}
		
		private Point3D GetSpawnLocation( Map map )
		{
			for ( int j = 0; j < 20; ++j )
			{
				int x = X + Utility.Random( 13 ) - 6;
				int y = Y + Utility.Random( 13 ) - 6;
				int z = map.GetAverageZ( x, y );

				if ( map.CanFit( x, y, this.Z, 16, false, false ) )
					return new Point3D( x, y, Z );
				else if ( map.CanFit( x, y, z, 16, false, false ) )
					return new Point3D( x, y, z );
			}

			return this.Location;
		}

		public void IncreaseRage(int rage)
		{
			if ( rage == 0 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "You shall burn!" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				SetDamage( 16, 20 );
				m_Rage = 1;
				return;
			}
			else if ( rage == 1 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Fire eternal shall consume you!" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				SetDamage( 21, 25 );
				VirtualArmor += 10;
				m_Rage = 2;
				return;
			}
			else if ( rage == 2 )
			{
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Ashardalon, heed my call!" );
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				SetDamage( 26, 30 );
				VirtualArmor += 10;             
				m_Rage = 3;
				return;
			}
			else
			{
				Effects.SendLocationParticles( EffectItem.Create( this.Location, this.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
				this.PlaySound( 0x1FE );
				PublicOverheadMessage( MessageType.Regular, 0x21, false, "Forgive me...My lord..." );
				Mobile killer = this.LastKiller;
				if ( killer != null && killer.Player && killer.Karma > 0 )
				{
					try
					{
						int marks = Utility.RandomMinMax( 231, 347 );
						Server.Custom.DefenderOfTheRealm.MarkLootHelper.AwardMarks( killer, 1, marks );
					}
					catch { }
				}
			}
		}
		
		public override void OnDeath( Container c )
		{
			//sanity
			if ( m_Rage < 3 )
				return;
				
			base.OnDeath( c );

			BossLootSystem.AwardBossSpecial(this,BossDrops, 15);
			for ( int i = 0; i < 5; i++ )
			{
				c.DropItem( Loot.RandomArty() );
				c.DropItem( new EtherealPowerScroll() );
				c.DropItem( AscensionScrollFactory.CreateRandom());
			}
			if ( Utility.RandomDouble() < 0.25 )
			{
				c.DropItem( new EternalPowerScroll() );
			}
			// gold explosion
			RichesSystem.SpawnRiches( m_LastTarget, 5 );
		}

		public override void OnDelete()
		{
		    if (m_Summons != null)
		    {
		        BossSummonSystem.CleanupSummons(m_Summons);
		        m_Summons.Clear();
		        m_Summons = null;
		    }
			CleanupMagmaTiles();
		    base.OnDelete();
		}

		private void CleanupMagmaTiles()
		{
			for (int i = m_MagmaTiles.Count - 1; i >= 0; i--)
			{
				MagmaTile tile = m_MagmaTiles[i];

				if (tile != null && !tile.Deleted)
					tile.Delete();
			}
			m_MagmaTiles.Clear();
		}

		public override void OnAfterSpawn()
		{
			base.OnAfterSpawn();
			LeechImmune = true;
		}

		public HeraldOfCinders( Serial serial ) : base( serial )
		{
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