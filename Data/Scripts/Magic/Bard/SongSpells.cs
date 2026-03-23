using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Misc;
using Server.Items;
using Server.Network;
using Server.Commands;
using Server.Commands.Generic;
using Server.Mobiles;
using Server.Accounting;
using Server.Regions;

namespace Server.Misc
{
    class BardFunctions
    {
		public static void UseBardInstrument( Item instrument, bool succeed, Mobile singer )
		{
			if ( instrument == null )
				return;

			if ( !(instrument is BaseInstrument) )
				return;

			if ( singer == null )
				return;

			BaseInstrument harp = (BaseInstrument)instrument;

			if ( succeed )
				singer.PlaySound(harp.SuccessSound);
			else
				singer.PlaySound(harp.FailureSound);

			if ( harp.UsesRemaining > 1 )
				harp.UsesRemaining--;
			else
			{
				if ( singer != null )
					singer.SendLocalizedMessage( 502079 ); // The instrument played its last tune.

				instrument.Delete();
			}
		}
	}
}

namespace Server.Spells.Song
{
	public abstract class Song : Spell 
	{
		public SongBook m_Book;

		public abstract double RequiredSkill{ get; } 
		public abstract int RequiredMana{ get; } 

		public override SkillName CastSkill{ get{ return SkillName.Musicianship; } } 
		public override SkillName DamageSkill{ get{ return SkillName.Musicianship; } } 

		public override bool ClearHandsOnCast{ get{ return false; } } 
		public override bool BlocksMovement{ get{ return false; } }

		public override TimeSpan GetCastDelay()
		{
			return CastDelayBase;
		}

		public Song( Mobile caster, Item scroll, SpellInfo info ) : base( caster, scroll, info ) 
		{ 
		}

		public static int MusicSkill( Mobile m )
        {
            int base_skill = (int)(m.Skills[SkillName.Musicianship].Value
                           +      m.Skills[SkillName.Provocation].Value
                           +      m.Skills[SkillName.Discordance].Value
                           +      m.Skills[SkillName.Peacemaking].Value);

            // ── Resonance (Skald, level 14+)
            PlayerMobile resonancePm = m as PlayerMobile;

            if ( resonancePm != null && resonancePm.ActiveAscension == AscensionType.Skald )
            {
                AscensionProgress resonanceProg = resonancePm.AscensionProfile.Get( AscensionType.Skald );
                int resonanceLevel = resonanceProg.Level;

                if ( resonanceLevel >= 19 )
                    base_skill = (base_skill * 125) / 100; // +25%
                else if ( resonanceLevel >= 14 )
                    base_skill = (base_skill * 115) / 100; // +15%
            }
            // ── End Resonance ─────────────────────────────────────────────────

            return base_skill;
        }

		public override bool CheckCast()
		{	
			if ( !base.CheckCast() )
				return false;
			if ( Caster.Skills[CastSkill].Value < RequiredSkill )
			{
				string args = String.Format( "{0}\t{1}\t ", RequiredSkill.ToString( "F1" ), CastSkill.ToString() );
				Caster.SendLocalizedMessage( 1063013, args ); // You need at least ~1_SKILL_REQUIREMENT~ ~2_SKILL_NAME~ skill to use that ability.
				return false;
			}
            Spellbook book = Spellbook.Find(Caster, -1, SpellbookType.Song);

			if ( book is SongBook )
				m_Book = (SongBook)book;

            if ( m_Book == null )
            {
                Caster.SendMessage("Parece que você está sem seu livro de canções.");
                return false;
            }
			return true;
		}

		public override void OnCast()
		{
		}

		public override void DoFizzle()
		{
			Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1115710 ); // Your spell song has been interrupted.
			// 1115737 You feel invigorated by the bard's spellsong
			// 1115758 The bard's song fills you with resilience
			// 1115759 The bard's song fills you with invincible
			// 1115774 You halt your spellsong
			// 1115938 Your spellsong has finished
			// 1149722 Your spellsong has ended
		}

		public override void GetCastSkills( out double min, out double max ) 
		{ 
			min = RequiredSkill; 
			max = RequiredSkill + 30.0; 
		} 

		public override int GetMana() 
		{ 
			return RequiredMana; 
		}
	} 
}
