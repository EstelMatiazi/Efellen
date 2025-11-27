using System;
using Server;
using Server.Mobiles;
using Server.Spells;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
	public class PoolOfBlood : Item
	{
		private TimeSpan m_Duration;
		private int m_MinDamage;
		private int m_MaxDamage;
		private DateTime m_Created;
		private bool m_Drying;
		private Timer m_Timer;

		[Constructable]
		public PoolOfBlood() : this( TimeSpan.FromSeconds( 10.0 ), 2, 5 )
		{
		}

		public override string DefaultName { get { return "a pool of blood"; } }

		[Constructable]
		public PoolOfBlood( TimeSpan duration, int minDamage, int maxDamage )
			: base( 0x122A )
		{
			Hue = 0x25;
			Movable = false;

			m_MinDamage = minDamage;
			m_MaxDamage = maxDamage;
			m_Created = DateTime.Now;
			m_Duration = duration;

			m_Timer = Timer.DelayCall( TimeSpan.Zero, TimeSpan.FromSeconds( 1 ), new TimerCallback( OnTick ) );
		}

		public override void OnAfterDelete()
		{
			if( m_Timer != null )
				m_Timer.Stop();
		}

		private void OnTick()
		{
			DateTime now = DateTime.Now;
			TimeSpan age = now - m_Created;

			if( age > m_Duration ) {
				Delete();
			} else {
				if( !m_Drying && age > (m_Duration - age) )
				{
					m_Drying = true;
					ItemID = 0x122B;
				}

				List<Mobile> target = new List<Mobile>();

				foreach( Mobile m in GetMobilesInRange( 0 ) )
				{
					BaseCreature bc = m as BaseCreature;

					if( m.Alive && !m.IsDeadBondedPet && (bc == null || bc.Controlled || bc.Summoned) )
					{
						target.Add( m );
					}
				}

				for ( int i = 0; i < target.Count; i++ )
                {
                    if ( target[i] is PlayerMobile && TransformationSpellHelper.UnderTransformation( target[i], typeof( Spells.Necromancy.VampiricEmbraceSpell ) ) )
			        {
			        	Heal(target[i]);
			        }
                    else
                    {
    		        	Damage( target[i] );
                    }
			    }
		    }
        }
		public override bool OnMoveOver( Mobile m )
		{
            if ( m is PlayerMobile && TransformationSpellHelper.UnderTransformation( m, typeof( Spells.Necromancy.VampiricEmbraceSpell ) ) )
			{
				Heal(m);
			}
            else
            {
    			Damage( m );
            }

			return true;
		}
        public void Heal ( Mobile m )
		{
            m.Heal( Utility.RandomMinMax( m_MinDamage, m_MaxDamage ) );
		}

		public void Damage ( Mobile m )
		{
			m.Damage( Utility.RandomMinMax( m_MinDamage, m_MaxDamage ) );
		}

		public PoolOfBlood( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			//Don't serialize these
		}

		public override void Deserialize( GenericReader reader )
		{
		}
	}
}
