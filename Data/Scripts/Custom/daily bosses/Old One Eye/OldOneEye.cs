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
using Server.Custom.Ascensions;

namespace Server.Mobiles
{
	[CorpseName( "Old One Eye's Corpse" )]
	public class OldOneEye : BaseCreature
	{
		private static readonly List<Type> BossDrops = new List<Type>
    	{
    	    typeof(Artifact_PrimalTunic),
            typeof(Artifact_PrimalLegs),
            typeof(Artifact_PrimalArms),
            typeof(Artifact_PrimalCrown),
            typeof(Artifact_PrimalClaws),
    	};

		private int m_Rage = 0;
		private Mobile m_LastTarget;
		private DateTime m_NextSpecialAttack = DateTime.MinValue;
		private DateTime m_NextTailSwipe = DateTime.MinValue;
		private DateTime m_NextLeap = DateTime.MinValue;
		private bool m_IsStunned = false;
		private DateTime m_StunEndTime = DateTime.MinValue;

		private bool m_Rage1Applied = false;
		private bool m_Rage2Applied = false;
		private bool m_Rage3Applied = false;

		[Constructable]
		public OldOneEye () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Old One Eye";

			Body = 665;
			BaseSoundID = 362;
			NameHue = 0x22;
			Hue = 0x995;
            Title = "The Primal Terror";
			
			SetStr( 796, 885 );
			SetDex( 265, 325 );
			SetInt( 106, 205 );

			SetHits( 33000 );
			SetDamage( 11, 15 );

			SetDamageType( ResistanceType.Physical, 100 );
			SetResistance( ResistanceType.Physical, 75 );
			SetResistance( ResistanceType.Fire, 75 );
			SetResistance( ResistanceType.Cold, 40 );
			SetResistance( ResistanceType.Poison, 60 );
			SetResistance( ResistanceType.Energy, 60 );

			SetSkill( SkillName.MagicResist, 145.5, 165.0 );
			SetSkill( SkillName.Tactics, 121.0, 125.0 );
			SetSkill( SkillName.FistFighting, 121.0, 125.0 );
			SetSkill( SkillName.Anatomy, 121.0, 125.0 );

			Fame = 30000;
			Karma = -30000;

			VirtualArmor = 50;
			IsBoss = true;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 6 );
		}

		public override int TreasureMapLevel{ get{ return 4; } }
		public override bool CanRummageCorpses{ get{ return false; } }
		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool BleedImmune{ get{ return true; } }
		public override bool BardImmune { get { return true; } }
		public override bool Unprovokable { get { return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			m_LastTarget = from;

			Server.Misc.IntelligentAction.TryToLeapToAttacker( this, from, 0.15, ref m_NextLeap );
			
			if ( !m_IsStunned && DateTime.UtcNow >= m_NextSpecialAttack )
			{
				PerformRageAttack( from );
				m_NextSpecialAttack = DateTime.UtcNow + TimeSpan.FromSeconds( 20 - (m_Rage * 2) );
			}

			base.OnDamage( amount, from, willKill );

			CheckRageThresholds();
		}

		private void CheckRageThresholds()
		{
			if (this.HitsMax <= 0)
				return;

			double hpPercent = (double)this.Hits / (double)this.HitsMax;

			if (!m_Rage1Applied && hpPercent <= 0.75)
			{
				m_Rage1Applied = true;
				m_Rage = 1;
				ApplyRage1();
			}
			else if (!m_Rage2Applied && hpPercent <= 0.50)
			{
				m_Rage2Applied = true;
				m_Rage = 2;
				ApplyRage2();
			}
			else if (!m_Rage3Applied && hpPercent <= 0.25)
			{
				m_Rage3Applied = true;
				m_Rage = 3;
				ApplyRage3();
			}
		}

		private void ApplyRage1()
		{
			PublicOverheadMessage( MessageType.Regular, 0x21, false, "*Screeches defiantly*" );
			this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
			this.PlaySound( 0x63F );
			SetStr( Str + 30 );
			SetDamage( 16, 20 );
			VirtualArmor += 5;
		}

		private void ApplyRage2()
		{
			PublicOverheadMessage( MessageType.Regular, 0x21, false, "*Screeches in Pain*" );
			this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
			this.PlaySound( 0x63F );
			SetDamage( 21, 25 );
			VirtualArmor += 10;
		}

		private void ApplyRage3()
		{
			PublicOverheadMessage( MessageType.Regular, 0x21, false, "*Roars terrifyingly*" );
			this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
			this.PlaySound( 0x63F );
			SetDamage( 25, 30 );
			VirtualArmor += 10;
		}

      	private void PerformRageAttack( Mobile target )
		{
			if ( target == null || target.Deleted || !target.Alive )
				return;

			int attackChoice = Utility.RandomMinMax( 1, 3 );

			switch ( attackChoice )
			{
				case 1:
				{
					BossSpecialAttack.PerformFear(
				      boss: this,
				      warcry: "*The Old One Eye unleashes a soul-freezing roar!*",
				      range: 6,
				      rage: m_Rage+1,
				      terror: 110
				  );
				  break;
				}
				case 2:
				{
					BossSpecialAttack.PerformSlam(
                    	boss: this,
                    	warcry: "*The ground quakes!*",
                    	hue: 0x995,
                    	rage: m_Rage+1,
                    	range: 6,
                    	physicalDmg: 100
              		);
                	break;
				}
                case 3:
				{
                    BossSpecialAttack.PerformRampage(
                       boss: this,
                       warcry: "*The Old One Eye charges wildly!*",
                       hue: 0x995,
                       rage: m_Rage+1,
                       stunDuration: 3.0
                   );
                   break;
				}
			}
		}

		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			reflect = ( Utility.Random( 100 ) < m_Rage * 16 );
		}

		public override void OnThink()
		{
		    base.OnThink();

		    Mobile combatant = this.Combatant;

		    if (combatant == null || combatant.Deleted || !combatant.Alive)
		        return;

		    if (DateTime.UtcNow >= m_NextSpecialAttack)
		    {
		        PerformRageAttack(combatant);
		        m_NextSpecialAttack = DateTime.UtcNow + TimeSpan.FromSeconds(35 - (m_Rage * 2));
		    }

		    m_LastTarget = combatant;
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack(attacker);

			Server.Misc.IntelligentAction.TryToLeapToAttacker( this, attacker, 0.1, ref m_NextLeap );

			if (Utility.Random(100) < 15 && DateTime.UtcNow >= m_NextTailSwipe)
			{
				PerformTailSwipe();
				m_NextTailSwipe = DateTime.UtcNow + TimeSpan.FromSeconds(30 - (m_Rage * 2));
			}
		}

		private void PerformTailSwipe()
		{
			PublicOverheadMessage(
				MessageType.Regular,
				0x21,
				false,
				"*Swings its massive tail!*"
			);
			PlaySound(0x64E);
			IPooledEnumerable eable = GetMobilesInRange(1);
			foreach (Mobile m in eable)
			{
				if (m == this || !m.Player || m.Deleted || !m.Alive || !CanBeHarmful(m))
					continue;

				DoHarmful(m);
				bool wasMounted = false;
				IMount mount = m.Mount;
				if (mount != null)
				{
					wasMounted = true;
					m.SendMessage("The massive tail swipe knocks you off your mount!");
					m.PlaySound(0x140);
					m.FixedParticles(0x3728, 10, 15, 9955, EffectLayer.Waist);
					Server.Mobiles.EtherealMount.EthyDismount(m);
					mount.Rider = null;
					BaseMount.SetMountPrevention(m, BlockMountType.Dazed, TimeSpan.FromSeconds(10.0));
				}
				int damage;
				if (wasMounted)
				{
					damage = Utility.RandomMinMax(33, 40) + Utility.RandomMinMax(15, 25);
					m.SendMessage("You are struck by a devastating tail swipe and take additional damage from the fall!");
				}
				else
				{
					damage = Utility.RandomMinMax(33, 40);
					m.SendMessage("You are struck by a devastating tail swipe!");
				}
				AOS.Damage(m, this, damage, 100, 0, 0, 0, 0);
				m.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
			}
			eable.Free();
		}

		public override bool OnBeforeDeath()
		{
			BossLootSystem.AwardBossMarks(this, this.LastKiller, 156, 223, "*Screeches one last time*");
			return base.OnBeforeDeath();
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			BossLootSystem.AwardBossSpecial(this, BossDrops, 30);
			for ( int i = 0; i < 4; i++ )
			{
				c.DropItem( Loot.RandomArty() );
				c.DropItem( new EtherealPowerScroll() );
				c.DropItem( AscensionScrollFactory.CreateRandom());
			}
			RichesSystem.SpawnRiches( m_LastTarget, 4 );
		}

		public override void OnAfterSpawn()
		{
			base.OnAfterSpawn();
			LeechImmune = true;
		}

		public OldOneEye( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 2 );

			writer.Write( m_Rage );
			writer.Write( m_NextSpecialAttack );
			writer.Write( m_NextTailSwipe );
			writer.Write( m_Rage1Applied );
			writer.Write( m_Rage2Applied );
			writer.Write( m_Rage3Applied );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( version >= 1 )
			{
				m_Rage = reader.ReadInt();
				m_NextSpecialAttack = reader.ReadDateTime();
				m_NextTailSwipe = reader.ReadDateTime();
			}

			if ( version >= 2 )
			{
				m_Rage1Applied = reader.ReadBool();
				m_Rage2Applied = reader.ReadBool();
				m_Rage3Applied = reader.ReadBool();
			}
			else
			{
				m_Rage1Applied = m_Rage >= 1;
				m_Rage2Applied = m_Rage >= 2;
				m_Rage3Applied = m_Rage >= 3;
			}

			LeechImmune = true;
		}
	}
}