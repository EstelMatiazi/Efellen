using System;
using System.Collections;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public class HeartOfTheWilds : BaseTrinket
	{
		private string m_OwnerName;
		private static Hashtable m_Table = new Hashtable();
		private ArrayList m_UnlockedForms;
		// heart comes with wolfy form unlocked as default
		private const string DefaultFormId = "Direwolf";
		public string OwnerName { get { return m_OwnerName; } set { m_OwnerName = value; InvalidateProperties(); } }


		[Constructable]
		public HeartOfTheWilds(Mobile from) : base(0x6730, Layer.Neck)
		{
			Name = from.Name+"'s Heart of the Wilds";
			Hue = 669;
			Weight = 1.0;
			LootType = LootType.Blessed;
			m_UnlockedForms = new ArrayList();
		    m_UnlockedForms.Add(DefaultFormId);
			m_OwnerName = from.Name;
		}

		public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add("Pertence a {0}", m_OwnerName);
        }

		public override string DefaultDescription{ get{ return "Um Coração das selvas contém conhecimento druídico adquirido de um dos servos da natureza. Pode ser usado por druidas habilidosos para se transformar nas várias criaturas com as quais se vincularam."; } }

		public HeartOfTheWilds(Serial serial) : base(serial)
		{
		}

		public bool IsFormUnlocked(string id)
		{
		    return m_UnlockedForms.Contains(id);
		}

		public void UnlockForm(string id)
		{
		    if (!m_UnlockedForms.Contains(id))
		        m_UnlockedForms.Add(id);
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (from.FindItemOnLayer(Layer.Neck) != this)
			{
				from.SendMessage("Você deve equipar o amuleto para usar seu poder.");
				return;
			}

			if (from.Mounted)
			{
				from.SendMessage("Você não pode transmorfar enquanto está montado.");
				return;
			}

			if (from.Skills[SkillName.Druidism].Value < 70.0 || from.Skills[SkillName.Druidism].Value < 70.0 )
			{
				from.SendMessage("Você não é habilidoso o suficiente para transmorfar. Precisa de pelo menos 70 em Druidismo e Espiritualismo.");
				return;
			}
			
			if (from.Skills[SkillName.Necromancy].Base > 0.1)
			{
				from.SendMessage("Os espíritos se recusam a se comunicar com necromantes.");
				return;
			}

			if (IsWearingMetalArmor(from))
			{
				from.SendMessage("Você não pode transmorfar enquanto estiver usando armadura de metal.");
				return;
			}

			SpectralFormContext context = GetContext(from);

			if (context != null)
			{
				RemoveContext(from, true);
				from.SendMessage("Você retorna à sua forma natural.");
			}
			else
			{
				from.CloseGump(typeof(ShapeshiftGump));
				from.SendGump(new ShapeshiftGump(from, this));
			}
		}

		private bool IsWearingMetalArmor(Mobile from)
		{
			for (int i = 0; i < from.Items.Count; i++)
			{
				Item item = from.Items[i];

				if (item is BaseArmor && !(item is BaseShield))
				{
					BaseArmor armor = (BaseArmor)item;

					// Druids don't wear metal armor
					if ((armor.MaterialType == ArmorMaterialType.Plate ||
						armor.MaterialType == ArmorMaterialType.Chainmail ||
						armor.MaterialType == ArmorMaterialType.Ringmail) 
							&& armor.DefaultResource == CraftResource.Iron
								&& armor.ArmorAttributes.MageArmor == 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override bool OnEquip(Mobile from)
		{
			return base.OnEquip(from);
		}

		public override void OnRemoved(object parent)
		{
			base.OnRemoved(parent);

			if (parent is Mobile)
			{
				Mobile m = (Mobile)parent;
				if (m == null || m.Deleted) return;
				SpectralFormContext context = GetContext(m);

				if (context != null)
				{
					RemoveContext(m, true);
					m.SendMessage("Você retorna à sua forma natural à medida que o poder do amuleto desaparece.");
				}
			}
		}

		public static void AddContext(Mobile m, SpectralFormContext context)
		{
			m_Table[m] = context;
			m.CheckStatTimers();
		}

		public static void RemoveContext(Mobile m, bool resetGraphics)
		{
			SpectralFormContext context = GetContext(m);

			if (context != null)
			{
				m_Table.Remove(m);

				if (context.SpeedBoost && context.SpeedBoostActive)
				{
					m.Send(SpeedControl.Disable);
					context.SpeedBoostActive = false;
				}

				if (context.StrMod != null)
					m.RemoveStatMod(context.StrMod.Name);
				if (context.DexMod != null)
					m.RemoveStatMod(context.DexMod.Name);
				if (context.IntMod != null)
					m.RemoveStatMod(context.IntMod.Name);

				if (context.SkillMod != null)
					m.RemoveSkillMod(context.SkillMod);

				context.RemoveResistMods(m);

				context.RemoveRegenAttributes(m);

				if (resetGraphics)
				{
					m.HueMod = -1;
					m.BodyMod = 0;
					m.RaceBody();
				}

				m.FixedParticles(0x3728, 10, 13, 2023, EffectLayer.Waist);
				m.PlaySound(0x1F8);

				context.Timer.Stop();

				m.CheckStatTimers();
				SpectralFormCombat.ClearCooldown(m);
			}
		}

		public static SpectralFormContext GetContext(Mobile m)
		{
			return m_Table[m] as SpectralFormContext;
		}

		public static bool Shapeshift(Mobile m, SpectralFormEntry entry, HeartOfTheWilds amulet)
		{

			if (amulet.m_OwnerName != m.Name) 
			{
				m.SendMessage("Este amuleto não pertence a você");
				return false;
			}
			
			if (m.FindItemOnLayer(Layer.Neck) != amulet)
			{
				m.SendMessage("O amuleto deve ser equipado para usar seu poder.");
				return false;
			}

			if (!amulet.IsFormUnlocked(entry.Id))
			{
			    m.SendMessage("Você ainda não aprendeu esta forma.");
			    return false;
			}


			double druidism = m.Skills[SkillName.Druidism].Value;
			double spiritualism = m.Skills[SkillName.Spiritualism].Value;

			if (druidism < entry.RequiredSkill || spiritualism < entry.RequiredSkill)
			{
				m.SendMessage("Você precisa de pelo menos {0} em ambos Druidismo e Espiritualismo para assumir esta forma.", entry.RequiredSkill);
				return false;
			}

			if (entry.ThirdSkill != SkillName.Alchemy && m.Skills[entry.ThirdSkill].Value < entry.ThirdSkillRequired)
			{
				m.SendMessage("Você precisa de pelo menos {0} {1} para assumir esta forma.", entry.ThirdSkillRequired, entry.ThirdSkill);
				return false;
			}

			if (m.Mana < entry.ManaCost)
			{
				m.SendMessage("Você precisa de pelo menos {0} de mana para transmorfar.", entry.ManaCost);
				return false;
			}

			if (m.FindItemOnLayer(Layer.Neck) != amulet)
			{
				m.SendMessage("O amuleto deve ser equipado.");
				return false;
			}

			if (m.Mounted)
			{
				IMount mount = m.Mount;
				if (mount != null)
					mount.Rider = null;
			}

			m.Mana -= entry.ManaCost;

			// transformation happens here
			m.BodyMod = entry.BodyValue;
			if (entry.Hue > 0)
				m.HueMod = entry.Hue;

			int strBonus = entry.StrBonus;
			int dexBonus = entry.DexBonus;
			int intBonus = entry.IntBonus;

			if (m.Str + strBonus > 150)
				strBonus = 150 - m.Str;
			if (m.Dex + dexBonus > 150)
				dexBonus = 150 - m.Dex;
			if (m.Int + intBonus > 150)
				intBonus = 150 - m.Int;

			StatMod strMod = null, dexMod = null, intMod = null;

			if (strBonus > 0)
			{
				strMod = new StatMod(StatType.Str, "SpectralForm_Str", strBonus, TimeSpan.Zero);
				m.AddStatMod(strMod);
			}

			if (dexBonus > 0)
			{
				dexMod = new StatMod(StatType.Dex, "SpectralForm_Dex", dexBonus, TimeSpan.Zero);
				m.AddStatMod(dexMod);
			}

			if (intBonus > 0)
			{
				intMod = new StatMod(StatType.Int, "SpectralForm_Int", intBonus, TimeSpan.Zero);
				m.AddStatMod(intMod);
			}

			SkillMod skillMod = null;
			if (entry.SkillBonus != SkillName.Alchemy)
			{
				double totalSkill = druidism + spiritualism;
				double skillBonusAmount = Math.Min(totalSkill / 15.0, 15.0);

				skillMod = new DefaultSkillMod(entry.SkillBonus, true, skillBonusAmount);
				m.AddSkillMod(skillMod);
			}

			Timer timer = new SpectralFormTimer(m, entry);
			timer.Start();

			SpectralFormContext context = new SpectralFormContext(timer, strMod, dexMod, intMod, entry, amulet, skillMod);
			AddContext(m, context);

			context.ApplyResistMods(m);
			context.ApplyRegenAttributes(m, druidism, spiritualism);

			if (entry.SpeedBoost)
			{
				Timer.DelayCall(TimeSpan.FromSeconds(0.25), delegate()
				{
					SpectralFormContext ctx = GetContext(m);
					if (ctx != null && ctx.SpeedBoost && !ctx.SpeedBoostActive)
					{
						m.Send(SpeedControl.MountSpeed);
						ctx.SpeedBoostActive = true;
					}
				});
			}

			m.FixedEffect(0x37C4, 10, 14, 1153, 4);
			m.PlaySound(0x1F7);

			m.CheckStatTimers();

			return true;
		}
		
		public override void Serialize(GenericWriter writer)
		{
		    base.Serialize(writer);
		    writer.Write((int)1); // version
			writer.Write(m_OwnerName);
		    writer.Write(m_UnlockedForms.Count);
		    for (int i = 0; i < m_UnlockedForms.Count; i++)
		        writer.Write((string)m_UnlockedForms[i]);
		}


		public override void Deserialize(GenericReader reader)
		{
		    base.Deserialize(reader);
		    int version = reader.ReadInt();
		    m_UnlockedForms = new ArrayList();
		    if (version >= 1)
		    {
				m_OwnerName = reader.ReadString();
		        int count = reader.ReadInt();
		        for (int i = 0; i < count; i++)
		            m_UnlockedForms.Add(reader.ReadString());
				
				if (!m_UnlockedForms.Contains(DefaultFormId))
				    m_UnlockedForms.Add(DefaultFormId);
		    }
		}

	}

	public class SpectralFormEntry
	{
	 	private string m_Id;
		private string m_Name;
		private int m_BodyValue;
		private int m_Hue;
		private int m_StrBonus;
		private int m_DexBonus;
		private int m_IntBonus;
		private int m_PhysResist;
		private int m_FireResist;
		private int m_ColdResist;
		private int m_EnergyResist;
		private int m_PoisonResist;
		private double m_RequiredSkill;
		private int m_ManaCost;
		private bool m_SpeedBoost;
		private bool m_BleedOnHit;
		private bool m_HealthRegen;
		private bool m_StaminaRegen;
		private bool m_ManaRegen;
		private bool m_LightningOnHit;
		private bool m_ParalyzeOnHit;
		private bool m_PoisonOnHit;
		private bool m_CleaveOnHit;
		private SkillName m_SkillBonus;
		private SkillName m_ThirdSkill;
		private double m_ThirdSkillRequired;


		public string Id { get { return m_Id; } }

		public string Name { get { return m_Name; } }
		public int BodyValue { get { return m_BodyValue; } }
		public int Hue { get { return m_Hue; } }
		public int StrBonus { get { return m_StrBonus; } }
		public int DexBonus { get { return m_DexBonus; } }
		public int IntBonus { get { return m_IntBonus; } }
		public int PhysResist { get { return m_PhysResist; } }
		public int FireResist { get { return m_FireResist; } }
		public int ColdResist { get { return m_ColdResist; } }
		public int EnergyResist { get { return m_EnergyResist; } }
		public int PoisonResist { get { return m_PoisonResist; } }
		public double RequiredSkill { get { return m_RequiredSkill; } }
		public int ManaCost { get { return m_ManaCost; } }
		public bool SpeedBoost { get { return m_SpeedBoost; } }
		public bool BleedOnHit { get { return m_BleedOnHit; } }
		public bool HealthRegen { get { return m_HealthRegen; } }
		public bool StaminaRegen { get { return m_StaminaRegen; } }
		public bool ManaRegen { get { return m_ManaRegen; } }
		public bool LightningOnHit { get { return m_LightningOnHit; } }
		public bool ParalyzeOnHit { get { return m_ParalyzeOnHit; } }
		public bool PoisonOnHit { get { return m_PoisonOnHit; } }
		public bool CleaveOnHit {get {return m_CleaveOnHit; } }
		public SkillName SkillBonus { get { return m_SkillBonus; } }
		public SkillName ThirdSkill { get { return m_ThirdSkill; } }
		public double ThirdSkillRequired { get { return m_ThirdSkillRequired; } }

		public SpectralFormEntry(string id, string name, int bodyValue, int hue, int strBonus, int dexBonus, int intBonus,
			int physResist, int fireResist, int coldResist, int energyResist, int poisonResist,
			double requiredSkill, int manaCost, bool speedBoost, bool bleedOnHit, bool healthRegen,
			bool staminaRegen, bool manaRegen, bool lightningOnHit, bool paralyzeOnHit,
			bool poisonOnHit, bool cleaveOnHit, SkillName skillBonus, SkillName thirdSkill, double thirdSkillRequired)
		{
			
			m_Id = id;
			m_Name = name;
			m_BodyValue = bodyValue;
			m_Hue = hue;
			m_StrBonus = strBonus;
			m_DexBonus = dexBonus;
			m_IntBonus = intBonus;
			m_PhysResist = physResist;
			m_FireResist = fireResist;
			m_ColdResist = coldResist;
			m_EnergyResist = energyResist;
			m_PoisonResist = poisonResist;
			m_RequiredSkill = requiredSkill;
			m_ManaCost = manaCost;
			m_SpeedBoost = speedBoost;
			m_BleedOnHit = bleedOnHit;
			m_HealthRegen = healthRegen;
			m_StaminaRegen = staminaRegen;
			m_ManaRegen = manaRegen;
			m_LightningOnHit = lightningOnHit;
			m_ParalyzeOnHit = paralyzeOnHit;
			m_PoisonOnHit = poisonOnHit;
			m_CleaveOnHit = cleaveOnHit;
			m_SkillBonus = skillBonus;
			m_ThirdSkill = thirdSkill;
			m_ThirdSkillRequired = thirdSkillRequired;
		}

		public static SpectralFormEntry[] Entries = new SpectralFormEntry[]
		{
			// Direwolf, Fast movement, Tracking bonus
			new SpectralFormEntry("Direwolf","Direwolf", 277, 0, 0, 10, 0, 0, 0, 0, 0, 0, 70.0, 25,
				true, false, false, false, false, false, false, false, false, 
				SkillName.Tracking, SkillName.Alchemy, 0),
			// Boa -  Poison on hit, Requires Poisoning 80
			new SpectralFormEntry("Anaconda","Anaconda", 21, 0, 10, 10, 0, 0, 0, 0, 0, 10, 80.0, 25,
				false, false, false, false, false, false, false, true, false, 
				SkillName.Alchemy, SkillName.Poisoning, 80.0),
            // Direbear -  Bleed + Health regen
			new SpectralFormEntry("Direbear","Direbear", 0xBE, 0, 15, 0, 0, 5, 0, 0, 0, 0, 85.0, 35,
				false, true, true, false, false, false, false, false, false, 
				SkillName.Focus, SkillName.Alchemy, 0),
			// 74 stalker - fast, paralyzing, requires hiding 85
            new SpectralFormEntry("Stalker","Stalker", 74, 0, 0, 15, 0, 0, 0, 0, 0, 5, 85.0, 35, 
                true, false, false, false, false, false, true, false, false,
                SkillName.Stealth, SkillName.Hiding,85),
			// giant scorpion - Poison + paralyze on hit, Requires Poisoning 90
			new SpectralFormEntry("Giant Scorpion","Giant Scorpion", 0x13b, 0, 10, 10, 0, 10, 0, 0, 0, 0, 90.0, 30,
				false, false, false, false, false, false, true, true, false, 
				SkillName.Fencing, SkillName.Poisoning, 90.0),
			// Gorakong - Poison on hit + Mana Regen, Requires Poisoning 100
			new SpectralFormEntry("Gorakong","Gorakong", 0x1d0, 0, 10, 0, 5, 5, 0, 0, 0, 0, 100.0, 40,
				false, false, true, false, true, false, false, false, false, 
				SkillName.FistFighting, SkillName.Alchemy, 100.0),
			// Worg -  Fast movement + Stamina Regen, Tactics bonus
			new SpectralFormEntry("Worg","Worg", 967, 0, 10, 10, 0, 0, 0, 0, 5, 0, 105.0, 40,
				true, false, false, true, false, false, false, false, false, 
				SkillName.Tactics, SkillName.Alchemy, 0),
			 // Griffon - bleed on hit + Fast movement, Requires 110 druidism/spiritualism
            new SpectralFormEntry("Griffon","Griffon", 0x31F, 0, 10, 10, 0, 5, 0, 0, 0, 0, 110.0, 45, 
                true, true, false, false, false, false, false, false, false,
                SkillName.Tactics, SkillName.Alchemy, 0),
			 // stegosaurus - health and stam regen
            new SpectralFormEntry("Stegosaurus","Stegosaurus", 0x2C1, 0, 20, 0, 0, 15, 0, 0, 0, 0, 115.0, 45, 
                false, false, true, true, false, false, false, false, false,
                SkillName.Parry, SkillName.Alchemy, 0),
			// Monstrous Spider - Poison on hit + Fast movement, Requires Poisoning 120
			new SpectralFormEntry("Monstrous Spider","Monstrous Spider", 173, 0, 10, 15, 0, 0, 0, 0, 0, 20, 120.0, 50,
				true, false, false, false, false, false, false, true, false, 
				SkillName.Alchemy, SkillName.Poisoning, 120.0),
				// Tyrannosaurus -  bleed + cleave, Magic Resist bonus
			new SpectralFormEntry("Tyrannosaurus","Tyrannosaurus", 665, 0, 25, 0, 0, 10, 10, 0, 0, 0, 120.0, 50,
				false, true, false, false, false, false, false, false, true,
				SkillName.MagicResist, SkillName.Alchemy, 0)
		};
	}

	public class SpectralFormContext
	{
		private Timer m_Timer;
		private StatMod m_StrMod;
		private StatMod m_DexMod;
		private StatMod m_IntMod;
		private SpectralFormEntry m_Entry;
		private HeartOfTheWilds m_Amulet;
		private ResistanceMod[] m_ResistMods;
		private int m_RegenAmount;
		private SkillMod m_SkillMod;
		private bool m_SpeedBoostActive;
		private DateTime m_LastSpeedCheck;

		public Timer Timer { get { return m_Timer; } }
		public StatMod StrMod { get { return m_StrMod; } }
		public StatMod DexMod { get { return m_DexMod; } }
		public StatMod IntMod { get { return m_IntMod; } }
		public SpectralFormEntry Entry { get { return m_Entry; } }
		public HeartOfTheWilds Amulet { get { return m_Amulet; } }
		public bool SpeedBoost { get { return m_Entry.SpeedBoost; } }
		public int RegenAmount { get { return m_RegenAmount; } set { m_RegenAmount = value; } }
		public SkillMod SkillMod { get { return m_SkillMod; } }
		public bool SpeedBoostActive { get { return m_SpeedBoostActive; } set { m_SpeedBoostActive = value; } }

		public SpectralFormContext(Timer timer, StatMod strMod, StatMod dexMod, StatMod intMod,
			SpectralFormEntry entry, HeartOfTheWilds amulet, SkillMod skillMod)
		{
			m_Timer = timer;
			m_StrMod = strMod;
			m_DexMod = dexMod;
			m_IntMod = intMod;
			m_Entry = entry;
			m_Amulet = amulet;
			m_RegenAmount = 0;
			m_SkillMod = skillMod;
			m_SpeedBoostActive = false;
			m_LastSpeedCheck = DateTime.MinValue;

			m_ResistMods = new ResistanceMod[5];
		}
		
		public void CheckSpeedBoost(Mobile m)
		{
			if ((DateTime.UtcNow - m_LastSpeedCheck).TotalSeconds < 3.5)
				return;

			m_LastSpeedCheck = DateTime.UtcNow;

			if (m_Entry.SpeedBoost && !m_SpeedBoostActive)
			{
				m.Send(SpeedControl.MountSpeed);
				m_SpeedBoostActive = true;
			}
		}

		public void ApplyResistMods(Mobile m)
		{
			if (m_Entry.PhysResist > 0)
			{
				m_ResistMods[0] = new ResistanceMod(ResistanceType.Physical, m_Entry.PhysResist);
				m.AddResistanceMod(m_ResistMods[0]);
			}

			if (m_Entry.FireResist > 0)
			{
				m_ResistMods[1] = new ResistanceMod(ResistanceType.Fire, m_Entry.FireResist);
				m.AddResistanceMod(m_ResistMods[1]);
			}

			if (m_Entry.ColdResist > 0)
			{
				m_ResistMods[2] = new ResistanceMod(ResistanceType.Cold, m_Entry.ColdResist);
				m.AddResistanceMod(m_ResistMods[2]);
			}

			if (m_Entry.EnergyResist > 0)
			{
				m_ResistMods[3] = new ResistanceMod(ResistanceType.Energy, m_Entry.EnergyResist);
				m.AddResistanceMod(m_ResistMods[3]);
			}

			if (m_Entry.PoisonResist > 0)
			{
				m_ResistMods[4] = new ResistanceMod(ResistanceType.Poison, m_Entry.PoisonResist);
				m.AddResistanceMod(m_ResistMods[4]);
			}
		}

		public void RemoveResistMods(Mobile m)
		{
			for (int i = 0; i < m_ResistMods.Length; i++)
			{
				if (m_ResistMods[i] != null)
					m.RemoveResistanceMod(m_ResistMods[i]);
			}
		}

		public void ApplyRegenAttributes(Mobile m, double druidism, double spiritualism)
		{
			m_RegenAmount = (int)((druidism + spiritualism) / 39.0);
		}

		public void RemoveRegenAttributes(Mobile m)
		{
			m_RegenAmount = 0;
		}
	}

	public class SpectralFormTimer : Timer
	{
		private Mobile m_Mobile;
		private SpectralFormEntry m_Entry;
		private int m_Counter;

		public SpectralFormTimer(Mobile from, SpectralFormEntry entry)
			: base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
		{
			m_Mobile = from;
			m_Entry = entry;
			m_Counter = 0;

			Priority = TimerPriority.FiftyMS;
		}

		protected override void OnTick()
		{
			SpectralFormContext context = HeartOfTheWilds.GetContext(m_Mobile);

			if (context == null || m_Mobile.Deleted || !m_Mobile.Alive || m_Mobile.BodyMod != m_Entry.BodyValue)
			{
				HeartOfTheWilds.RemoveContext(m_Mobile, true);
				Stop();
				return;
			}

			if (m_Mobile.FindItemOnLayer(Layer.Neck) != context.Amulet)
			{
				HeartOfTheWilds.RemoveContext(m_Mobile, true);
				m_Mobile.SendMessage("Você retorna à sua forma natural enquanto o poder do amuleto desaparece.");
				Stop();
				return;
			}

			context.CheckSpeedBoost(m_Mobile);

			double druidism = m_Mobile.Skills[SkillName.Druidism].Value;
			double spiritualism = m_Mobile.Skills[SkillName.Spiritualism].Value;

			double regenChance = (druidism + spiritualism) / 10.0;

			if (regenChance > 25.0)
			    regenChance = 25.0;

			m_Counter++;

			if (context.RegenAmount > 0)
			{
				if (Utility.RandomDouble() * 100.0 > regenChance)
			        return;
				
				if (m_Entry.HealthRegen && (m_Mobile.Hits < m_Mobile.HitsMax) && ( (m_Mobile.Hits + context.RegenAmount) < m_Mobile.HitsMax ) )
				{
					m_Mobile.Hits += context.RegenAmount;
				}	

				if (m_Entry.StaminaRegen && (m_Mobile.Stam < m_Mobile.StamMax) && ( (m_Mobile.Stam + context.RegenAmount) < m_Mobile.StamMax ) )
				{
					m_Mobile.Stam += context.RegenAmount;
				}
			
				if (m_Entry.ManaRegen && (m_Mobile.Mana < m_Mobile.ManaMax)  && ( (m_Mobile.Mana + context.RegenAmount) < m_Mobile.ManaMax ))
				{
					m_Mobile.Mana += context.RegenAmount;
				}
			}
		}
	}
}