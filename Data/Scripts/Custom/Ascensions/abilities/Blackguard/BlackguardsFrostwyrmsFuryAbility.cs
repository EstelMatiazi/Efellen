using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public class BlackguardsFrostwyrmsFuryAbility : AscensionAbility
    {
        public override string Name             { get { return "FrostwyrmsFury"; } }
        public override string        DisplayName { get { return "Frostwyrms Fury"; } }
        public override AscensionType Ascension { get { return AscensionType.Blackguard; } }
        public override int RequiredLevel       { get { return 18; } }
        public override bool IsPassive          { get { return false; } }
        public override TimeSpan Cooldown       { get { return TimeSpan.FromSeconds(300); } }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
            {
                pm.SendMessage("You cannot use Frostwyrm's Fury.");
                return;
            }

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("That ability is on cooldown.");
                return;
            }

            if (pm.Mana < 60)
            {
                pm.SendMessage("You do not have enough mana.");
                return;
            }

            if (pm.Stam < 60)
            {
                pm.SendMessage("You do not have enough stamina.");
                return;
            }

            if (pm.Followers + 2 > pm.FollowersMax)
            {
                pm.SendMessage("You have too many followers to use this ability.");
                return;
            }

            AscensionProgress prog = pm.AscensionProfile.Get(Ascension);
            DoHeavenlyGate(pm, prog.Level);
        }

        private void DoHeavenlyGate(PlayerMobile pm, int level)
        {
            pm.Mana -= 60;
            pm.Stam -= 60;
            pm.SetAbilityCooldown(Name, Cooldown);

            pm.PublicOverheadMessage(MessageType.Regular, 0x22, false, "*Frostwyrm's Fury*");

            bool summonTwo = level >= 20
                && Utility.Random(100) < level
                && (pm.Followers + 4 <= pm.FollowersMax);

            SummonCreature(pm);

            if (summonTwo)
                SummonCreature(pm);

            Effects.SendLocationParticles(
                EffectItem.Create(pm.Location, pm.Map, EffectItem.DefaultDuration),
                0x3728, 10, 10, 0x498, 0, 2023, 0
            );
            pm.PlaySound(0x29);

            new CooldownNotifyTimer(pm, Cooldown, "You can call forth the Frostwyrm's Fury once again.").Start();
        }

        private static void SummonCreature(PlayerMobile pm)
        {
            Map map = pm.Map;

            BlackguardsFrostWyrm creature = new BlackguardsFrostWyrm();

            creature.Summoned       = true;
            creature.SummonMaster   = pm;
            creature.ControlMaster  = pm;
            creature.Controlled     = true;
            creature.ControlOrder   = OrderType.Follow;
            creature.ControlTarget  = pm;

        
            Point3D loc = pm.Location;

            for (int i = 0; i < 10; i++)
            {
                int x = pm.X + Utility.RandomMinMax(-1, 1);
                int y = pm.Y + Utility.RandomMinMax(-1, 1);
                int z = map.GetAverageZ(x, y);

                if (map.CanFit(x, y, z, 16, false, false))
                {
                    loc = new Point3D(x, y, z);
                    break;
                }
            }

            creature.MoveToWorld(loc, map);

            Effects.SendLocationParticles(
                EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                0x3728, 10, 10, 0x498, 0, 2023, 0
            );
            creature.PlaySound(creature.GetAngerSound());
        }

        private sealed class CooldownNotifyTimer : Timer
        {
            private readonly PlayerMobile m_Player;
            private readonly string       m_Message;

            public CooldownNotifyTimer(PlayerMobile player, TimeSpan delay, string message)
                : base(delay)
            {
                m_Player  = player;
                m_Message = message;
                Priority  = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (m_Player == null || m_Player.Deleted)
                    return;

                m_Player.SendMessage(0x482, m_Message);
            }
        }
    }
}