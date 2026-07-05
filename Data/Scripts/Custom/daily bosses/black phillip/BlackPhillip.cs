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
using Server.CustomSpells;

namespace Server.Mobiles
{
    [CorpseName("Black Phillip's Corpse")]
    public class BlackPhillip : BaseCreature
    {
        private static readonly Type[] SummonTypes = new Type[]
        {
            typeof(NativeWitchDoctor),
            typeof(WitchOfTheDreadHost),
            typeof(Demon)
        };

        private static readonly string[] SummonWarcries = new string[]
        {
            "Will you sign my contract?",
            "We are covenant!",
            "Wouldst thou like to live deliciously?",
            "Does thou crave the taste of butter?"
        };

        private static readonly List<Type> BossDrops = new List<Type>
        {
            typeof(Artifact_HelmOfTheDreadHost),
            typeof(Artifact_RobeOfTheDreadHost),
            typeof(Artifact_StaffOfTheDreadHost),
            typeof(Artifact_EmbraceOfTheDreadHost),
            typeof(Artifact_TemptationOfTheDreadHost),
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
        public BlackPhillip() : base(AIType.AI_Mage, FightMode.Closest, 20, 1, 0.4, 0.8)
        {
            Name = "Black Phillip";
            Title = "Harbinger from Beyond";
            Body = 380;
            NameHue = 0x22;
            Hue = 1109;
            BaseSoundID = 0x99;

            SetStr(496, 585);
            SetDex(155, 235);
            SetInt(206, 275);

            SetHits(9000);

            SetDamage(11, 15);

            SetDamageType(ResistanceType.Fire, 50);
            SetDamageType(ResistanceType.Physical, 50);

            SetResistance(ResistanceType.Physical, 45);
            SetResistance(ResistanceType.Fire, 55);
            SetResistance(ResistanceType.Cold, 40);
            SetResistance(ResistanceType.Poison, 50);
            SetResistance(ResistanceType.Energy, 60);

            SetSkill(SkillName.Magery, 92.5, 115.0);
            SetSkill(SkillName.Psychology, 62.5, 85.0);
            SetSkill(SkillName.Meditation, 72.5, 85.0);
            SetSkill(SkillName.MagicResist, 75.5, 115.0);
            SetSkill(SkillName.Tactics, 81.0, 95.0);
            SetSkill(SkillName.FistFighting, 101.0, 115.0);

            Fame = 15000;
            Karma = -15000;

            VirtualArmor = 30;
            IsBoss = true;
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
        public override Poison PoisonImmune { get { return Poison.Greater; } }

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
                m_NextSpecialAttack = DateTime.UtcNow + TimeSpan.FromSeconds(25 - (m_Rage * 2));
            }

            m_LastTarget = combatant;
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            m_LastTarget = from;

            Server.Misc.IntelligentAction.TryToLeapToAttacker( this, from, 0.05, ref m_NextLeap );

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
            PublicOverheadMessage(MessageType.Regular, 0x21, false, "I shall enjoy your bloodletting!");
            this.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
            this.PlaySound(0x202);
            SetDamage(16, 20);
        }

        private void ApplyRage2()
        {
            PublicOverheadMessage(MessageType.Regular, 0x21, false, "Come closer...");
            this.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
            this.PlaySound(0x202);
            SetDamage(21, 25);
            VirtualArmor += 5;
        }

        private void ApplyRage3()
        {
            PublicOverheadMessage(MessageType.Regular, 0x21, false, "You bore me, mortal!");
            this.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
            this.PlaySound(0x202);
            SetDamage(26, 30);
            VirtualArmor += 5;
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
                    BossSpecialAttack.PerformTargettedAoE(
                        this,
                        target,
                        m_Rage + 1,
                        "WOUDST THOU SING MY BOOK?",
                        0x845,
                        0,
                        50,
                        0,
                        0,
                        50
                    );
                    break;
                }
                case 2:
                {
                    BossSpecialAttack.SummonHonorGuard(
                        boss: this,
                        target: target,
                        warcry: "Come the little children unto me!",
                        amount: 6,
                        creatureType: typeof(WitchOfTheDreadHost),
                        hue: 0x845
                    );
                    break;
                }
                case 3:
                {
                    BossSpecialAttack.PerformDelayedExplosion(
                        this,
                        "YOUR SOUL IS MINE!",
                        0x845,
                        16,
                        m_Rage + 1,
                        0,
                        100,
                        0,
                        0,
                        0
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
                case 0: return 9;
                case 1: return 8;
                case 2: return 7;
                case 3: return 6;
                default: return 6;
            }
        }

        public override bool OnBeforeDeath()
        {
            BossLootSystem.AwardBossMarks(this, this.LastKiller, 79, 90, "I shall return!");
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
            base.OnAfterSpawn();
            this.MobileMagics(5, SpellType.Wizard, 0);
            LeechImmune = true;
        }

        public BlackPhillip(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)2);

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

            if (version >= 2)
            {
                m_Rage1Applied = reader.ReadBool();
                m_Rage2Applied = reader.ReadBool();
                m_Rage3Applied = reader.ReadBool();
                this.MobileMagics(5, SpellType.Wizard, 0);
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