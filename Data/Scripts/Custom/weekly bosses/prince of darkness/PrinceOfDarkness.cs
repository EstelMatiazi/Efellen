using System;
using Server;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Commands;
using Server.Commands.Generic;

namespace Server.Mobiles
{
	[CorpseName( "Prince of Darkness Corpse" )]
	public class PrinceOfDarkness : BaseCreature
	{
		[Constructable]
		public PrinceOfDarkness () : base( AIType.AI_Mage, FightMode.Closest, 20, 1, 0.4, 0.8 )
		{
			Name = "Prince of Darkness";
			Title = "Pure Evil Incarnate";

			Body = 0x58;
			BaseSoundID = 838;
			NameHue = 0x22;
			Hue = 0x982;

			SetStr( 896, 985 );
			SetDex( 125, 175 );
			SetInt( 586, 675 );

			SetHits( 30000 );
			SetDamage( 70, 80 );

			SetDamageType( ResistanceType.Physical, 75 );
			SetResistance( ResistanceType.Fire, 70, 90 );
			SetResistance( ResistanceType.Cold, 60, 70 );
			SetResistance( ResistanceType.Poison, 60, 70 );
			SetResistance( ResistanceType.Energy, 60, 70 );

			SetSkill( SkillName.Psychology, 80.1, 100.0 );
			SetSkill( SkillName.Magery, 100.1, 125.0 );
			SetSkill( SkillName.Meditation, 52.5, 75.0 );
			SetSkill( SkillName.MagicResist, 120.5, 150.0 );
			SetSkill( SkillName.Tactics, 97.6, 111.0 );
			SetSkill( SkillName.FistFighting, 97.6, 111.0 );
			SetSkill( SkillName.Musicianship, 111.6, 125.0);
			SetSkill( SkillName.Discordance, 111.6, 125.0);
			SetSkill( SkillName.Spiritualism, 110.0,125.0);
			SetSkill (SkillName.Necromancy, 100.1,125.0);

			Fame = 35000;
			Karma = -35000;

			VirtualArmor = 60;

			PackItem (Loot.RandomArty());
			PackItem (Loot.RandomArty());
			PackItem (Loot.RandomArty());

		}


		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 8 );
		}

		public override bool AutoDispel{ get{ return !Controlled; } }
		public override int TreasureMapLevel{ get{ return 5; } }
		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 18; } }
		public override HideType HideType{ get{ return HideType.Hellish; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override int Skin{ get{ return Utility.Random(5); } }
		public override SkinType SkinType{ get{ return SkinType.Demon; } }
		public override int Skeletal{ get{ return Utility.Random(5); } }
		public override SkeletalType SkeletalType{ get{ return SkeletalType.Devil; } }
        public override bool CanRummageCorpses{ get{ return false; } }
		public override int BreathPhysicalDamage{ get{ return 0; } }
		public override int BreathFireDamage{ get{ return 0; } }
		public override int BreathColdDamage{ get{ return 0; } }
		public override int BreathPoisonDamage{ get{ return 100; } }
		public override int BreathEnergyDamage{ get{ return 0; } }
		public override int BreathEffectHue{ get{ return 0x3F; } }
		public override int BreathEffectSound{ get{ return 0x658; } }
		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool HasBreath{ get{ return true; } }
		public override double BreathEffectDelay{ get{ return 0.1; } }
		public override void BreathDealDamage( Mobile target, int form ){ base.BreathDealDamage( target, 33 ); }
        public override bool BleedImmune{ get{ return true; } }
		public override bool BardImmune { get { return !Core.SE; } }
		public override bool Unprovokable { get { return Core.SE; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }
		
		int CanDie = 0;

		private Mobile lastTarget;

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			lastTarget = from;
			Server.Misc.IntelligentAction.LeapToAttacker( this, from );
			base.OnDamage( amount, from, willKill );
		}

		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			if ( Utility.RandomMinMax( 1, 4 ) == 1 ){ reflect = true; } // 25% spells are reflected back to the caster
			else { reflect = false; }
		}

		public void SpawnCreature( Mobile target )
		{
			Map map = this.Map;

			if ( map == null )
				return;

			int monsters = 0;

			foreach ( Mobile m in this.GetMobilesInRange( 6 ) )
			{
				if ( m is MetalHead)
					++monsters;
			}
            int metalheadSummons = CanDie > 0  ? 18 : 9;
			if ( monsters < metalheadSummons )
			{
				PlaySound( 0x216 );
                Say("All aboard! Hahahaha!");
				int newmonsters = Utility.RandomMinMax( 2, 4 );

				for ( int i = 0; i < newmonsters; ++i )
				{
					BaseCreature monster = new MetalHead();

					monster.Team = this.Team;

					bool validLocation = false;
					Point3D loc = this.Location;

					for ( int j = 0; !validLocation && j < 10; ++j )
					{
						int x = X + Utility.Random( 3 ) - 1;
						int y = Y + Utility.Random( 3 ) - 1;
						int z = map.GetAverageZ( x, y );

						if ( validLocation = map.CanFit( x, y, this.Z, 16, false, false ) )
							loc = new Point3D( x, y, Z );
						else if ( validLocation = map.CanFit( x, y, z, 16, false, false ) )
							loc = new Point3D( x, y, z );
					}

					monster.IsTempEnemy = true;
					monster.MoveToWorld( loc, map );
					monster.Combatant = target;
				}
			}
		}

		public void SummonMetalhead( Mobile target )
		{
			if ( target == null || target.Deleted )
				return;

			if ( 0.1 >= Utility.RandomDouble() ) // 10% chance
				SpawnCreature( target );
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			//base.OnGotMeleeAttack( attacker );
			SummonMetalhead( attacker );
		}

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			//base.OnGaveMeleeAttack( defender );
			SummonMetalhead( defender );
		}

		public PrinceOfDarkness( Serial serial ) : base( serial )
		{
		}

		public override bool OnBeforeDeath()
		{
			

			if ( CanDie == 0 )
			{
				Say("No more tears!");
				this.Hits = this.HitsMax/2;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				CanDie = 1;
             	return false;
			}
			else 
			{
				Effects.SendLocationParticles( EffectItem.Create( this.Location, this.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
				this.PlaySound( 0x1FE );
				Say("Mama...I'm coming home.");
			}


			return base.OnBeforeDeath();
		}

		public override void OnDeath( Container c )
		{

			base.OnDeath( c );

			if (Utility.RandomDouble() < 0.15)
			{
					c.DropItem(new EternalPowerScroll());
			}
			if (Utility.RandomDouble() < 0.25)
			{
				PowerScroll ps = PowerScroll.CreateRandom(20, 25);
				if (ps != null)
					c.DropItem(ps);
			}
			if (Utility.RandomDouble() < 0.45)
			{
				int amt = Utility.RandomMinMax(1, 3);
				for (int i = 0; i < amt; i++)
				{
					PowerScroll ps = PowerScroll.CreateRandom(10, 20);
					if (ps != null)
						c.DropItem(ps);
				}
			}
			if (Utility.RandomDouble() < 0.65)
			{
				int amt = Utility.RandomMinMax(2, 6);
				for (int i = 0; i < amt; i++)
				{
					PowerScroll ps = PowerScroll.CreateRandom(5, 15);
					if (ps != null)
						c.DropItem(ps);
				}
			}
			if(Utility.RandomDouble() < 0.85)
			{
				int amt = Utility.RandomMinMax(1,4);
				for (int i = 0; i < amt; i++)
				{
					c.DropItem(new EtherealPowerScroll());
				}
			}
			
			TitanRiches( lastTarget );
		}

		public static void TitanRiches( Mobile m )
		{
			Map map = m.Map;

			if ( map != null )
			{
				for ( int x = -12; x <= 12; ++x )
				{
					for ( int y = -12; y <= 12; ++y )
					{
						double dist = Math.Sqrt(x*x+y*y);

						if ( dist <= 12 )
							new GoodiesTimer( map, m.X + x, m.Y + y ).Start();
					}
				}
			}
		}

		public class GoodiesTimer : Timer
		{
			private Map m_Map;
			private int m_X, m_Y;

			public GoodiesTimer( Map map, int x, int y ) : base( TimeSpan.FromSeconds( Utility.RandomDouble() * 5.0 ) )
			{
				m_Map = map;
				m_X = x;
				m_Y = y;
			}

			protected override void OnTick()
			{
				int z = m_Map.GetAverageZ( m_X, m_Y );
				bool canFit = m_Map.CanFit( m_X, m_Y, z, 6, false, false );

				for ( int i = -3; !canFit && i <= 3; ++i )
				{
					canFit = m_Map.CanFit( m_X, m_Y, z + i, 6, false, false );

					if ( canFit )
						z += i;
				}

				if ( !canFit )
					return;

				Item g = new Gold( 100, 200 ); g.Delete();

				int r1 = (int)( Utility.RandomMinMax( 80, 160 ) * (MyServerSettings.GetGoldCutRate() * .01) );
				int r2 = (int)( Utility.RandomMinMax( 200, 400 ) * (MyServerSettings.GetGoldCutRate() * .01) );
				int r3 = (int)( Utility.RandomMinMax( 400, 800 ) * (MyServerSettings.GetGoldCutRate() * .01) );
				int r4 = (int)( Utility.RandomMinMax( 800, 1200 ) * (MyServerSettings.GetGoldCutRate() * .01) );
				int r5 = (int)( Utility.RandomMinMax( 1200, 1600 ) * (MyServerSettings.GetGoldCutRate() * .01) );

				switch ( Utility.Random( 21 ) )
				{
					case 0: g = new Crystals( r1 ); break;
					case 1: g = new DDGemstones( r2 ); break;
					case 2: g = new DDJewels( r2 ); break;
					case 3: g = new DDGoldNuggets( r3 ); break;
					case 4: g = new Gold( r3 ); break;
					case 5: g = new Gold( r3 ); break;
					case 6: g = new Gold( r3 ); break;
					case 7: g = new DDSilver( r4 ); break;
					case 8: g = new DDSilver( r4 ); break;
					case 9: g = new DDSilver( r4 ); break;
					case 10: g = new DDSilver( r4 ); break;
					case 11: g = new DDSilver( r4 ); break;
					case 12: g = new DDSilver( r4 ); break;
					case 13: g = new DDCopper( r5 ); break;
					case 14: g = new DDCopper( r5 ); break;
					case 15: g = new DDCopper( r5 ); break;
					case 16: g = new DDCopper( r5 ); break;
					case 17: g = new DDCopper( r5 ); break;
					case 18: g = new DDCopper( r5 ); break;
					case 19: g = new DDCopper( r5 ); break;
					case 20: g = new DDCopper( r5 ); break;
				}

				g.MoveToWorld( new Point3D( m_X, m_Y, z ), m_Map );

				if ( 0.5 >= Utility.RandomDouble() )
				{
					switch ( Utility.Random( 3 ) )
					{
						case 0: // Fire column
						{
							Effects.SendLocationParticles( EffectItem.Create( g.Location, g.Map, EffectItem.DefaultDuration ), 0x3709, 10, 30, 5052 );
							Effects.PlaySound( g, g.Map, 0x208 );

							break;
						}
						case 1: // Explosion
						{
							Effects.SendLocationParticles( EffectItem.Create( g.Location, g.Map, EffectItem.DefaultDuration ), 0x36BD, 20, 10, 5044 );
							Effects.PlaySound( g, g.Map, 0x307 );

							break;
						}
						case 2: // Ball of fire
						{
							Effects.SendLocationParticles( EffectItem.Create( g.Location, g.Map, EffectItem.DefaultDuration ), 0x36FE, 10, 10, 5052 );

							break;
						}
					}
				}
			}
		}

		public override void OnAfterSpawn()
		{
			base.OnAfterSpawn();
			LeechImmune = true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
		
	}
}