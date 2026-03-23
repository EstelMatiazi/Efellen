using System;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom.Ascensions
{
    public static class KensaiHelpers
    {
        public static bool IsSword(BaseWeapon weapon)
        {
            if (weapon == null)
                return false;

            return weapon.Type is BaseSword;
        }

        public static void ApplyCullingStrikeProc(PlayerMobile pm, Mobile target, int level)
        {
            if (target == null || target.Deleted || !target.Alive)
                return;

            if (!pm.CanBeHarmful(target, false))
                return;

            BaseWeapon weapon = pm.Weapon as BaseWeapon;

            if (weapon == null || !IsSword(weapon))
                return;

            int minDmg    = weapon.MinDamage;
            int maxDmg    = weapon.MaxDamage;
            int baseDmg   = Utility.RandomMinMax(minDmg, maxDmg);
            int bonusDmg  = (baseDmg * 80) / 100;

            if (bonusDmg < 1)
                bonusDmg = 1;

            pm.DoHarmful(target);
            target.FixedParticles(0x376A, 9, 32, 5030, 0x448, 0, EffectLayer.Waist);
            AOS.Damage(target, pm, bonusDmg, 100, 0, 0, 0, 0);
        }
    }
}