using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Network;
using Server.EffectsUtil;

namespace Server.Custom.Ascensions
{
    public class CrusaderSmiteAbility : AscensionAbility
    {
        public override string Name             { get { return "Smite"; } }
        public override string        DisplayName { get { return "Smite"; } }
        public override AscensionType Ascension { get { return AscensionType.Crusader; } }
        public override int RequiredLevel       { get { return 1; } }
        public override bool IsPassive          { get { return false; } }
        public override TimeSpan Cooldown       { get { return TimeSpan.FromSeconds(60); } }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
            {
                pm.SendMessage("You cannot use Smite.");
                return;
            }

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            if (pm.Mana < 20)
            {
                pm.SendMessage("You do not have enough mana.");
                return;
            }

            if (pm.Stam < 20)
            {
                pm.SendMessage("You do not have enough Stamina.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            DoSmite(pm, prog.Level);
        }

        private void DoSmite(PlayerMobile pm, int level)
        {
            pm.Mana -= 20;
            pm.Stam -= 20;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x22, false, "*Smite*");

            int radius      = 2 + (level / 5);
            int damage      = Utility.RandomMinMax(20, 32) + (level / 2) + (pm.Str / 15);
            bool paralyze   = level >= 10;
            bool healFriends = level >= 15;
            bool searingFlames = level >= 20;

            SlamVisuals.SlamVisual(pm, radius, 0x36B0, 0x498);

            IPooledEnumerable eable = pm.Map.GetMobilesInRange(pm.Location, radius);

            try
            {
                foreach (Mobile m in eable)
                {
                    if (m == null || m.Deleted || !m.Alive || m == pm)
                        continue;

                    if (healFriends && m.Karma > 0)
                    {
                        int healAmt = 20 + (level / 2);
                        m.Hits = Math.Min(m.HitsMax, m.Hits + healAmt);
                        m.FixedParticles(0x374A, 10, 15, 5021, 0x3F, 0, EffectLayer.Waist);
                        m.PlaySound(0x1F2);
                        continue;
                    }

                    if (!pm.CanBeHarmful(m))
                        continue;

                    if (m.Karma >= 0)
                        continue;

                    pm.DoHarmful(m);
                    AOS.Damage(m, pm, damage, 0, 0, 100, 0, 0);

                    if (paralyze && m.Karma <= -10000)
                    {
                        int duration = 4 + (level / 6);
                        m.Paralyze(TimeSpan.FromSeconds(duration));
                    }

                    if (searingFlames && m.Karma <= -10000)
                    {
                        new SearingFlamesTimer(pm, m, pm.Str).Start();
                    }
                }
            }
            finally
            {
                eable.Free();
            }

            pm.FixedParticles(0x374A, 10, 15, 5021, 2075, 0, EffectLayer.Waist);
            pm.PlaySound(0x1F8);
        }

        private sealed class SearingFlamesTimer : Timer
        {
            private static readonly TimeSpan Interval = TimeSpan.FromSeconds(2.0);
            private const int MaxTicks = 6;

            private readonly Mobile  _attacker;
            private readonly Mobile  _target;
            private readonly int     _str;
            private int              _ticksRemaining;

            public SearingFlamesTimer(Mobile attacker, Mobile target, int str)
                : base(Interval, Interval)
            {
                _attacker       = attacker;
                _target         = target;
                _str            = str;
                _ticksRemaining = MaxTicks;
                Priority        = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (_target == null || _target.Deleted || !_target.Alive)
                {
                    Stop();
                    return;
                }

                int fireDmg = Utility.RandomMinMax(10, 20) + (_str / 15);

                _target.FixedParticles(0x3709, 10, 30, 5052, 0x498, 0, EffectLayer.LeftFoot);
                _target.PlaySound(0x208);

                if (_attacker != null && !_attacker.Deleted)
                    AOS.Damage(_target, _attacker, fireDmg, 0, 100, 0, 0, 0);
                else
                    _target.Damage(fireDmg);

                if (--_ticksRemaining <= 0)
                    Stop();
            }
        }
    }
}