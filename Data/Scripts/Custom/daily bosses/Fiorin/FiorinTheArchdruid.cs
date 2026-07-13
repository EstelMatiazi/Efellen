using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;
using Server.Commands;
using Server.Commands.Generic;
using Server.Spells;
using Server.EffectsUtil;
using Server.Custom;
using Server.Custom.DailyBosses.System;
using Server.Custom.BossSystems;
using Server.CustomSpells;
using Server.Custom.Ascensions;

namespace Server.Mobiles
{
	[CorpseName("Fiorin's Corpse")]
	public class FiorinTheArchdruid : BaseSpellCaster
	{
		private static readonly Type[] SummonTypes = new Type[]
		{
			typeof(BlackWolf),
			typeof(GuardianWolf),
			typeof(DeepWoodSniper),
			typeof(DruidOfTheHowlingOrder),
			typeof(KeeperOfTheGrove)
		};

		private static readonly string[] SummonWarcries = new string[]
		{
			"Come, my pack! Prey approaches!",
			"We shall end your vile existance!",
			"Comrades of the grove, it is time for the hunt!",
			"With the moon as our witness we shall put you down!"
		};

		private static readonly List<Type> BossDrops = new List<Type>
		{
			typeof(Artifact_GaiasMask),
			typeof(Artifact_GaiasScimitar),
			typeof(Artifact_GaiasLeggings),
			typeof(Artifact_GaiasGloves),
			typeof(Artifact_GaiasTunic),
			typeof(Artifact_GaiasArms)
		};

		private static readonly Type[] FriendlyTypes = new Type[]
		{
			typeof(GuardianPanda),
			typeof(GuardianWolf),
			typeof(BlackWolf),
			typeof(DeepWoodSniper),
			typeof(DruidOfTheHowlingOrder),
			typeof(KeeperOfTheGrove)
		};

		private int m_Rage = 0;
		private Mobile m_LastTarget;
		private DateTime m_NextSummonTime = DateTime.MinValue;
		private DateTime m_NextSpecialAttack = DateTime.MinValue;
		private DateTime m_NextLeap = DateTime.MinValue;
		private List<BaseCreature> m_Summons = new List<BaseCreature>();

		private bool m_Rage1Applied = false;
		private bool m_Rage2Applied = false;
		private bool m_Rage3Applied = false;

		[Constructable]
		public FiorinTheArchdruid() : base(AIType.AI_Mage, FightMode.Evil, 20, 1, 0.4, 0.8)
		{
			Name = "Fiorin";
			Title = "The Archdruid";
			Body = 400;
			FacialHairItemID = Utility.RandomList(0, 0, 8254, 8255, 8256, 8257, 8267, 8268, 8269);
			NameHue = 0x92E;
			Hue = Utility.RandomSkinHue();
			Utility.AssignRandomHair(this);
			HairHue = Utility.RandomHairHue();
			FacialHairHue = HairHue;

			SetStr(496, 585);
			SetDex(155, 185);
			SetInt(286, 375);

			SetHits(9000);
			SetDamage(11, 15);

			SetDamageType(ResistanceType.Physical, 100);
			SetResistance(ResistanceType.Physical, 60);
			SetResistance(ResistanceType.Fire, 45);
			SetResistance(ResistanceType.Cold, 45);
			SetResistance(ResistanceType.Poison, 60);
			SetResistance(ResistanceType.Energy, 50);

			SetSkill(SkillName.Magery, 105.0);
			SetSkill(SkillName.Psychology, 75.0);
			SetSkill(SkillName.Meditation, 85.0);
			SetSkill(SkillName.MagicResist, 75.5, 125.0);
			SetSkill(SkillName.Tactics, 105.0);
			SetSkill(SkillName.FistFighting, 115.0);
			SetSkill(SkillName.Swords, 115.0);

			Fame = 15000;
			Karma = 15000;

			VirtualArmor = 30;
			IsBoss = true;
			AddItem(new Scimitar { Hue = 267 });
			AddItem(new LeatherArms { Hue = 267 });
			AddItem(new LeatherChest { Hue = 267 });
			AddItem(new LeatherLegs { Hue = 267 });
			AddItem(new LeatherGorget { Hue = 267 });
			AddItem(new LeatherGloves { Hue = 267 });
			AddItem(new Boots { Hue = 267 });
			AddItem(new VagabondRobe { Hue = 267 });
			AddItem(new Cloak { Hue = 267 });
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.UltraRich, 2);
		}

		public override int TreasureMapLevel { get { return 3; } }
		public override bool CanRummageCorpses { get { return false; } }
		public override bool ReacquireOnMovement { get { return !Controlled; } }
		public override bool BleedImmune { get { return true; } }
		public override bool BardImmune { get { return true; } }
		public override bool Unprovokable { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Lethal; } }
		public override bool AlwaysAttackable { get { return true; } }
		public override bool AlwaysMurderer { get { return false; } }
		public override bool ClickTitle { get { return false; } }
		public override bool ShowFameTitle { get { return false; } }

		private bool IsFriendlyCreature(Mobile m)
		{
			if (m == null)
				return false;

			Type t = m.GetType();
			for (int i = 0; i < FriendlyTypes.Length; i++)
			{
				if (t == FriendlyTypes[i])
					return true;
			}
			return false;
		}

		public override bool IsEnemy(Mobile m)
		{
			if (m == null || m.Deleted)
				return false;

			if (IsFriendlyCreature(m))
				return false;

			if (m.Player && m.Karma >= 0 && m.Combatant != this)
				return false;

			if (!IntelligentAction.GetMyEnemies(m, this, true))
				return false;

			if (m.Region != this.Region)
				return false;

			return true;
		}

		public override void AggressiveAction(Mobile m, bool criminal)
		{
			if (IsFriendlyCreature(m))
				return;

			base.AggressiveAction(m, criminal);
		}

		public override bool CanBeHarmful(Mobile m, bool message, bool ignoreOurBlessedness)
		{
			if (IsFriendlyCreature(m))
				return false;

			return base.CanBeHarmful(m, message, ignoreOurBlessedness);
		}

		public override bool CanBeBeneficial(Mobile m, bool message, bool allowDead)
		{
			if (IsFriendlyCreature(m))
				return true;

			return base.CanBeBeneficial(m, message, allowDead);
		}

		public override void OnDamage(int amount, Mobile from, bool willKill)
		{
			if (from != null && from.Player && !from.Criminal)
				from.Criminal = true;

			Server.Misc.IntelligentAction.TryToLeapToAttacker( this, from, 0.07, ref m_NextLeap );

			base.OnDamage(amount, from, willKill);

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
			PublicOverheadMessage(MessageType.Regular, 0x21, false, "Stand together, for Gaia!");
			this.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
			this.PlaySound(0x202);
			SetDamage(16, 20);
		}

		private void ApplyRage2()
		{
			PublicOverheadMessage(MessageType.Regular, 0x21, false, "We shall hunt you down!");
			this.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
			this.PlaySound(0x202);
			SetDamage(21, 25);
			VirtualArmor += 5;
		}

		private void ApplyRage3()
		{
			PublicOverheadMessage(MessageType.Regular, 0x21, false, "Mother Gaia, I call thee!");
			this.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
			this.PlaySound(0x202);
			SetStr(Str + 100);
			SetDex(Dex + 95);
			SetDamage(21, 25);
			VirtualArmor += 5;
			Body = 0x5E;
			Hue = 0;
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
				45
			);

			if (DateTime.UtcNow >= m_NextSpecialAttack)
			{
				PerformRageAttack(combatant);
				m_NextSpecialAttack = DateTime.UtcNow + TimeSpan.FromSeconds(30 - (m_Rage * 2));
			}

			m_LastTarget = combatant;
		}

		private void PerformRageAttack(Mobile target)
		{
			if (target == null || target.Deleted || !target.Alive)
				return;

			int attackChoice = Utility.RandomMinMax(1, 3);

			switch (attackChoice)
			{
				case 1:
				{
					BossSpecialAttack.PerformEntangle(
						boss: this,
						warcry: "*Gaia, guide my hand!*",
						hue: 0x4F6,
						rage: m_Rage + 1,
						range: 6,
						bleedLevel: 4
					);
					break;
				}
				case 2:
				{
					BossSpecialAttack.PerformSmite(
						this,
						target,
						m_Rage + 1,
						"The Hour of the Wolf is upon thee!",
						669,
						50,
						0,
						0,
						50,
						0
					);
					break;
				}
				case 3:
				{
					BossSpecialAttack.SummonHonorGuard(
						boss: this,
						target: target,
						warcry: "Spirits, aid me!",
						amount: 4,
						creatureType: typeof(GuardianPanda),
						hue: 0xb73
					);
					break;
				}
			}
		}

		public override void CheckReflect(Mobile caster, ref bool reflect)
		{
			reflect = (Utility.Random(100) < m_Rage * 10);
		}

		private int GetMaxSummons()
		{
			switch (m_Rage)
			{
				case 0: return 8;
				case 1: return 6;
				case 2: return 4;
				case 3: return 4;
				default: return 4;
			}
		}

		public override bool OnBeforeDeath()
		{
			BossLootSystem.AwardBossMarks(this, this.LastKiller, 80, 110, "I...I return...To Gaia...");
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

		public override void OnDeath(Container c)
		{
			base.OnDeath(c);

			BossLootSystem.AwardBossSpecial(this, BossDrops, 30);
			for (int i = 0; i < 2; i++)
			{
				c.DropItem(Loot.RandomArty());
				c.DropItem(new EtherealPowerScroll());
				c.DropItem(AscensionScrollFactory.CreateRandom());
			}

			RichesSystem.SpawnRiches(m_LastTarget, 2);
		}

		public override void OnAfterSpawn()
		{
			this.MobileMagics(6, SpellType.Druid, 0x92E);
			base.OnAfterSpawn();
			LeechImmune = true;
		}

		public FiorinTheArchdruid(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)3);
			writer.Write(m_Rage);
			writer.Write(m_NextSummonTime);
			writer.Write(m_NextSpecialAttack);
			writer.Write(m_Rage1Applied);
			writer.Write(m_Rage2Applied);
			writer.Write(m_Rage3Applied);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			if (version >= 1)
			{
				m_Rage = reader.ReadInt();
				m_NextSummonTime = reader.ReadDateTime();
				m_NextSpecialAttack = reader.ReadDateTime();
			}

			if (version >= 3)
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
			if (m_Summons == null)
				m_Summons = new List<BaseCreature>();
		}
	}
}