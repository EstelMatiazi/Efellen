using System;
using Server;
using Server.Items;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName( "a Luminar's corpse" )]
	public class CrusaderLuminar : BaseCreature
	{
		public override bool DeleteCorpseOnDeath { get { return true; } }

		public override bool IsDispellable { get { return false; } }
		
		private static Hashtable m_BurningSkinTable = new Hashtable();
		public override double GetFightModeRanking( Mobile m, FightMode acqType, bool bPlayerOnly )
		{
			return ( m.Int + m.Skills[SkillName.Knightship].Value ) / Math.Max( GetDistanceToSqrt( m ), 1.0 );
		}

		[Constructable]
		public CrusaderLuminar() : base( AIType.AI_Melee, FightMode.Evil, 10, 1, 0.2, 0.4 )
		{
			Name = "a Luminar";
			Body = 0x9e;
			Hue = 0xf8;
			BaseSoundID = 466;
			SetStr( 250 );
			SetDex( 250 );
			SetInt( 100 );

			SetHits( 350 );
			SetStam( 150 );
			SetMana( 0 );

			SetDamage( 18, 22 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Energy, 20 );
			SetDamageType( ResistanceType.Fire, 20 );
			SetDamageType( ResistanceType.Cold, 20 );
			SetDamageType( ResistanceType.Poison, 20 );

			SetResistance( ResistanceType.Physical, 40 );
			SetResistance( ResistanceType.Fire, 40 );
			SetResistance( ResistanceType.Cold, 40 );
			SetResistance( ResistanceType.Poison, 40 );
			SetResistance( ResistanceType.Energy, 80 );	

			SetSkill( SkillName.MagicResist, 99.9 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.FistFighting, 120.0 );

			Fame = 0;
			Karma = 0;
			VirtualArmor = 50;
			ControlSlots = 0;
		}

		public override void OnGaveMeleeAttack(Mobile defender)
		{
		    base.OnGaveMeleeAttack(defender);

		    if (defender == null || defender.Deleted || !defender.Alive)
		        return;

		    if (Utility.Random(100) < 15)
		        ApplyBurningSkin(defender);
		}

		private static void ApplyBurningSkin(Mobile m)
		{
		    if (m == null || m.Deleted)
		        return;

		    BurningSkinDebuff debuff = (BurningSkinDebuff)m_BurningSkinTable[m];

		    if (debuff != null)
		    {
		        debuff.Refresh();
		    }
		    else
		    {
		        debuff = new BurningSkinDebuff(m);
		        m_BurningSkinTable[m] = debuff;
		        debuff.Start();
		    }

		    m.SendMessage(33, "*your armor burns!*");
		}

		public override bool CanRummageCorpses{ get{ return false; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }

		public override bool BleedImmune{ get{ return true; } }

		private class BurningSkinDebuff : Timer
		{
		    private Mobile m_Mobile;
		    private DateTime m_End;
		    private ResistanceMod m_Mod;

		    public BurningSkinDebuff(Mobile m)
		        : base(TimeSpan.Zero, TimeSpan.FromSeconds(1.0))
		    {
		        m_Mobile = m;
		        m_End = DateTime.UtcNow + TimeSpan.FromSeconds(12);

		        m_Mod = new ResistanceMod(ResistanceType.Physical, -12);
		        Priority = TimerPriority.TwoFiftyMS;

		        if (m_Mobile != null)
		            m_Mobile.AddResistanceMod(m_Mod);
		    }

		    public void Refresh()
		    {
		        m_End = DateTime.UtcNow + TimeSpan.FromSeconds(12);
		    }

		    protected override void OnTick()
		    {
		        if (m_Mobile == null || m_Mobile.Deleted || !m_Mobile.Alive)
		        {
		            Remove();
		            Stop();
		            return;
		        }

		        if (DateTime.UtcNow >= m_End)
		        {
		            Remove();
		            Stop();
		        }
		    }

		    private void Remove()
		    {
		        if (m_Mobile != null)
		            m_Mobile.RemoveResistanceMod(m_Mod);

		        m_BurningSkinTable.Remove(m_Mobile);
		    }
		}

		public CrusaderLuminar( Serial serial ): base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

		}
	}
}