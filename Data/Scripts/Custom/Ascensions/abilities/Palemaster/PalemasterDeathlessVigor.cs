using System;
using Server;
using Server.Mobiles;

namespace Server.Custom.Ascensions
{
    public class PalemasterDeathlessVigor
    {
        private const string SpellDamageEffectName = "DeathlessVigorSpellDamage";

        public static void TryTrigger(PlayerMobile pm)
        {
            if (pm == null)
                return;

            if (!pm.HasActiveAscension || pm.ActiveAscension != AscensionType.Palemaster)
                return;

            AscensionProgress prog = pm.AscensionProfile.Get(AscensionType.Palemaster);

            if (prog == null || prog.Level < 2)
                return;

            int level = prog.Level;

            TryLifeRecovery(pm, level);
            TrySpellDamageBuff(pm, level);
        }

        private static void TryLifeRecovery(PlayerMobile pm, int level)
        {
            double chance = 0.015 * level;

            if (Utility.RandomDouble() > chance)
                return;

            int heal = 2 + (level / 2);

            if (pm.Hits < pm.HitsMax)
            {
                pm.Hits += heal;

                if (pm.Hits > pm.HitsMax)
                    pm.Hits = pm.HitsMax;

                PlayEffect(pm);
                pm.SendMessage("Deathless Vigor restores your vitality.");
            }
        }

        private static void TrySpellDamageBuff(PlayerMobile pm, int level)
        {
            if (level < 5)
                return;

            double chance;
            int duration;
            int bonus;

            if (level >= 13)
            {
                chance = 0.0045 * level;
                duration = 25;
                bonus = 15;
            }
            else
            {
                chance = 0.0025 * level;
                duration = 15;
                bonus = 10;
            }

            if (Utility.RandomDouble() > chance)
                return;

            if (pm.HasAscensionEffect(SpellDamageEffectName))
                return;

            TimeSpan span = TimeSpan.FromSeconds(duration);

            pm.AddAscensionEffect(
                SpellDamageEffectName,
                span,
                bonus
            );

            PlayEffect(pm);
            pm.SendMessage("Death empowers your necromancy.");

            Timer.DelayCall(span, () =>
            {
                if (pm != null && !pm.Deleted)
                    pm.SendMessage("Your deathless vigor fades.");
            });
        }

        private static void PlayEffect(PlayerMobile player)
        {
            if (player.Map == null)
                return;

            Effects.SendLocationEffect(
                player.Location,
                player.Map,
                0x3728,
                15,
                10,
                2075,
                0
            );
        }
    }
}