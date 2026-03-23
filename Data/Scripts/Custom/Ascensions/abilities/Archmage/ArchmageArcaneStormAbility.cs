using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public class ArchmageArcaneStormAbility : AscensionAbility
    {
        public override string Name { get { return "ArcaneStorm"; } }
        public override string DisplayName { get { return "Arcane Storm"; } }
        public override AscensionType Ascension { get { return AscensionType.Archmage; } }
        public override int RequiredLevel { get { return 1; } }
        public override bool IsPassive { get { return false; } }

        public override TimeSpan Cooldown
        {
            get { return TimeSpan.FromSeconds(45); }
        }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
            {
                pm.SendMessage("You cannot use Arcane Storm.");
                return;
            }

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            if (pm.Mana < 45)
            {
                pm.SendMessage("You do not have enough mana.");
                return;
            }

            pm.SendMessage("Select your target.");
            pm.Target = new ArcaneStormTarget(this);
        }

        private class ArcaneStormTarget : Target
        {
            private ArchmageArcaneStormAbility m_Ability;

            public ArcaneStormTarget(ArchmageArcaneStormAbility ability)
                : base(12, false, TargetFlags.Harmful)
            {
                m_Ability = ability;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                PlayerMobile pm = from as PlayerMobile;
                if (pm == null)
                    return;

                Mobile target = targeted as Mobile;

                if (target == null || !target.Alive || !pm.CanBeHarmful(target))
                {
                    pm.SendMessage("That is not a valid target.");
                    return;
                }

                m_Ability.PerformStorm(pm, target);
            }
        }

        public void PerformStorm(PlayerMobile pm, Mobile target)
        {
            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            if (prog == null)
                return;

            int level = prog.Level;

            pm.PublicOverheadMessage(MessageType.Regular, 0x22, false, "*Missile Storm*");
            pm.DoHarmful(target);
            pm.Mana -= 45;

            int missiles   = 6 + (level / 3);
            int bonusDamage = (pm.Int / 25) + (level / 3);

            for (int i = 0; i < missiles; i++)
            {
                new MissileVisualTimer(pm, target, TimeSpan.FromSeconds(i * 0.3)).Start();
                new MissileDamageTimer(this, pm, target, level, bonusDamage, TimeSpan.FromSeconds(i * 0.25)).Start();
            }

            double cd = 45.0 - (1.0 / level);
            pm.SetAbilityCooldown(Name, TimeSpan.FromSeconds(cd));
        }

        private class MissileVisualTimer : Timer
        {
            private PlayerMobile m_Caster;
            private Mobile m_Target;

            public MissileVisualTimer(PlayerMobile caster, Mobile target, TimeSpan delay)
                : base(delay)
            {
                m_Caster = caster;
                m_Target = target;
            }

            protected override void OnTick()
            {
                if (m_Target == null || !m_Target.Alive || m_Target.Deleted)
                    return;

                Effects.SendMovingEffect(m_Caster, m_Target, 0x379F, 7, 0, false, false, 0x0213, 0);
                Effects.PlaySound(m_Caster.Location, m_Caster.Map, 0x1F5);

                new MissileImpactTimer(m_Target, TimeSpan.FromSeconds(0.5)).Start();
            }
        }

        private class MissileImpactTimer : Timer
        {
            private Mobile m_Target;

            public MissileImpactTimer(Mobile target, TimeSpan delay)
                : base(delay)
            {
                m_Target = target;
            }

            protected override void OnTick()
            {
                if (m_Target == null || !m_Target.Alive || m_Target.Deleted)
                    return;

                Effects.SendLocationEffect(m_Target.Location, m_Target.Map, 0x3709, 10, 30, 0x0213, 0);
                m_Target.FixedParticles(0x36BD, 6, 10, 5044, 0x0213, 0, EffectLayer.Waist);
            }
        }

        private class MissileDamageTimer : Timer
        {
            private ArchmageArcaneStormAbility m_Ability;
            private PlayerMobile m_Caster;
            private Mobile m_Target;
            private int m_Level;
            private int m_BonusDamage;

            public MissileDamageTimer(ArchmageArcaneStormAbility ability, PlayerMobile caster, Mobile target, int level, int bonusDamage, TimeSpan delay)
                : base(delay)
            {
                m_Ability     = ability;
                m_Caster      = caster;
                m_Target      = target;
                m_Level       = level;
                m_BonusDamage = bonusDamage;
            }

            protected override void OnTick()
            {
                if (m_Caster == null || m_Target == null || m_Target.Deleted || !m_Target.Alive)
                    return;

                int damage = Utility.RandomMinMax(10, 18) + m_BonusDamage;
                AOS.Damage(m_Target, m_Caster, damage, 0, 0, 0, 0, 100);

                m_Ability.HandleProcs(m_Caster, m_Target, m_Level);

                if (m_Level >= 20 && !m_Target.Alive)
                {
                    m_Caster.SetAbilityCooldown("Conflux", TimeSpan.Zero);
                    m_Caster.SendMessage("Your Conflux cooldown has been reset!");
                }
            }
        }

        private void HandleProcs(PlayerMobile caster, Mobile target, int level)
        {
            if (level >= 10)
            {
                if (Utility.Random(100) < level * 3)
                {
                    int resistDrop = 8 + (level / 4);
                    int duration   = 12 + (level / 2);

                    ResistanceMod mod = new ResistanceMod(ResistanceType.Energy, -resistDrop);
                    target.AddResistanceMod(mod);

                    new ResistRestoreTimer(target, mod, TimeSpan.FromSeconds(duration)).Start();
                }
            }

            if (level >= 15)
            {
                int stamLoss = 34 + (level / 2);
                target.Stam  = Math.Max(target.Stam - stamLoss, 0);
            }
        }

        private class ResistRestoreTimer : Timer
        {
            private Mobile m_Target;
            private ResistanceMod m_Mod;

            public ResistRestoreTimer(Mobile target, ResistanceMod mod, TimeSpan delay)
                : base(delay)
            {
                m_Target = target;
                m_Mod    = mod;
            }

            protected override void OnTick()
            {
                if (m_Target != null && m_Mod != null)
                    m_Target.RemoveResistanceMod(m_Mod);
            }
        }
    }
}