using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Network;

namespace Server.Custom.Ascensions
{
    public class CrusaderHeavenlyGateAbility : AscensionAbility
    {
        public override string Name             { get { return "HeavenlyGate"; } }
        public override string        DisplayName { get { return "Heavenly Gate"; } }
        public override AscensionType Ascension { get { return AscensionType.Crusader; } }
        public override int RequiredLevel       { get { return 18; } }
        public override bool IsPassive          { get { return false; } }
        public override TimeSpan Cooldown       { get { return TimeSpan.FromSeconds(300); } }

        public override void Execute(PlayerMobile pm)
        {
            if (!CanUse(pm))
            {
                pm.SendMessage("Você não pode usar Heavenly Gate.");
                return;
            }

            if (pm.IsAbilityOnCooldown(Name))
            {
                pm.SendMessage("Esta habilidade está em cooldown.");
                return;
            }

            if (pm.Mana < 60)
            {
                pm.SendMessage("Você não tem mana suficiente.");
                return;
            }

            if (pm.Stam < 60)
            {
                pm.SendMessage("Você não tem stamina suficiente.");
                return;
            }

            if (pm.Followers + 2 > pm.FollowersMax)
            {
                pm.SendMessage("Você tem muitos seguidores para usar esta habilidade.");
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

            pm.PublicOverheadMessage(MessageType.Regular, 0x22, false, "*Heavenly Gate*");

            bool summonTwo = level >= 20
                && Utility.Random(100) < level
                && (pm.Followers + 4 <= pm.FollowersMax);

            SummonArchon(pm);

            if (summonTwo)
                SummonArchon(pm);

            Effects.SendLocationParticles(
                EffectItem.Create(pm.Location, pm.Map, EffectItem.DefaultDuration),
                0x3728, 10, 10, 0x498, 0, 2023, 0
            );
            pm.PlaySound(0x29);

            new CooldownNotifyTimer(pm, Cooldown, "Você pode abrir os portais celestes novamente.").Start();
        }

        private static void SummonArchon(PlayerMobile pm)
        {
            Map map = pm.Map;

            CrusaderArchon archon = new CrusaderArchon();

            archon.Summoned       = true;
            archon.SummonMaster   = pm;
            archon.ControlMaster  = pm;
            archon.Controlled     = true;
            archon.ControlOrder   = OrderType.Follow;
            archon.ControlTarget  = pm;

        
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

            archon.MoveToWorld(loc, map);

            Effects.SendLocationParticles(
                EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                0x3728, 10, 10, 0x498, 0, 2023, 0
            );
            archon.PlaySound(archon.GetAngerSound());
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