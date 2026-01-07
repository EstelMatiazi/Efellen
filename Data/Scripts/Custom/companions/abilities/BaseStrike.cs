using System;
using Server;
using Server.Mobiles;
using Server.Companions.Core;
using Server.Companions.Abilities;

public abstract class BaseStrike : BaseFeat
{
    protected BaseStrike(int tier) : base(tier)
    {
    }

    public override FeatType Type
    {
        get { return FeatType.Strike; }
    }

    public override bool IsMartialSpecial
    {
        get { return Category == FeatCategory.Martial; }
    }

    public override void Use(CompanionMobile companion, Mobile target)
    {
        if (companion == null || target == null)
            return;

        if (!CheckCooldown())
            return;

        OnStrike(companion, target);
    }

    protected abstract void OnStrike(
        CompanionMobile companion,
        Mobile target
    );
}
