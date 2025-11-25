using System;
using Server;
using Server.Misc;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.Regions;

namespace Server.Mobiles 
{ 
	[CorpseName( "a lycanthrope corpse" )] 
	public class Ozzy_WereWolf : BaseCreature 
	{
		private static Dictionary<Mobile, DateTime> m_LastHealTime = new Dictionary<Mobile, DateTime>();
		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.BleedAttack;
		}

		[Constructable] 
		public Ozzy_WereWolf() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 ) 
		{
			SpeechHue = Utility.RandomTalkHue();
			Hue = Utility.RandomSkinColor();
            Body = Utility.RandomList( 708, 94 );
            BaseSoundID = 0xE5;
			Name = "a WereWolf";
			Title = null;

			SetStr( 200, 250 );
			SetDex( 200, 250 );
			SetInt( 30, 60 );
            SetHits( 200, 250 );
			SetDamage( 11, 16 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 55 );
			SetResistance( ResistanceType.Fire, 25, 40 );
			SetResistance( ResistanceType.Cold, 25, 40 );
			SetResistance( ResistanceType.Poison, 10, 30 );
			SetResistance( ResistanceType.Energy, 10, 30 );

			SetSkill( SkillName.Searching, 80.0 );
			SetSkill( SkillName.Anatomy, 125.0 );
			SetSkill( SkillName.Poisoning, 60.0, 82.5 );
			SetSkill( SkillName.MagicResist, 83.5, 92.5 );
			SetSkill( SkillName.Swords, 125.0 );
			SetSkill( SkillName.FistFighting, 125.0 );
			SetSkill( SkillName.Tactics, 125.0 );

			Fame = 8000;
			Karma = -8000;
			VirtualArmor = 10;
		}

		public override bool AlwaysAttackable{ get{ return true; } }
        public override bool CanRummageCorpses{ get{ return false; } }
		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 7; } }
		public override int Cloths{ get{ return 4; } }
		public override ClothType ClothType{ get{ return ClothType.Furry; } }
		public override int Skeletal{ get{ return Utility.Random(4); } }
		public override SkeletalType SkeletalType{ get{ return SkeletalType.Lycan; } }

		public override bool OnBeforeDeath()
		{
		    if (this.Map != null && this.Map != Map.Internal)
		    {
		        IPooledEnumerable eable = this.Map.GetMobilesInRange(this.Location, 6);
		        int mobileCount = 0;
		        foreach (Mobile m in eable)
		        {
		            mobileCount++;
		            if (m is PrinceOfDarkness && m.Alive)
		            {
		                PrinceOfDarkness prince = (PrinceOfDarkness)m;
		                DateTime lastHeal;
		                if (!m_LastHealTime.TryGetValue(prince, out lastHeal) || 
		                    DateTime.UtcNow >= lastHeal + TimeSpan.FromMinutes(2.0))
		                {
		                    prince.PublicOverheadMessage(
		                        Network.MessageType.Regular, 
		                        0x982, 
		                        false, 
		                        "The Prince of Darkness chops the werewolf's head off!"
		                    );
		                    int healAmount = Utility.RandomMinMax(275, 625);
			                prince.Heal(healAmount);
		                    m_LastHealTime[prince] = DateTime.UtcNow;
		                }
		                break;
		            }
		        }
		        eable.Free();
		    }
		    return base.OnBeforeDeath();
		}

		public Ozzy_WereWolf( Serial serial ) : base( serial ) 
		{ 
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