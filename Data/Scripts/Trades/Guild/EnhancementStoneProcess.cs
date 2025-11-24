using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
    public class GuildCraftingProcess
    {
        private int BaseCost = 100;
        private bool AttrCountAffectsCost = true;
        public int MaxAttrCount = 10;

        public Mobile Owner = null;
        public Item ItemToUpgrade = null;
        public int CurrentAttributeCount = 0;

        public GuildCraftingProcess(Mobile from, Item target)
        {
            Owner = from;
            ItemToUpgrade = target;
        }

        public void BeginProcess()
        {
            CurrentAttributeCount = 0;

            if (!(ItemToUpgrade is BaseShield || ItemToUpgrade is BaseClothing || ItemToUpgrade is BaseArmor || ItemToUpgrade is BaseWeapon || ItemToUpgrade is BaseTrinket || ItemToUpgrade is Spellbook))
            {
                Owner.SendMessage("This cannot be enhanced.");
            }
            else
            {
                int MaxedAttributes = 0;

                foreach (AttributeHandler handler in AttributeHandler.Definitions)
                {
                    int attr = handler.Upgrade(ItemToUpgrade, true);
                    
                    if (attr > 0)
                        CurrentAttributeCount++;

                    if (attr >= handler.MaxValue)
                        MaxedAttributes++;
                }

                if (CurrentAttributeCount > MaxAttrCount || MaxedAttributes >= MaxAttrCount )
                    Owner.SendMessage("This piece of equipment cannot be enhanced any further.");
                else
                    Owner.SendGump(new EnhancementGump(this));
            }
        }

        public void BeginUpgrade(AttributeHandler handler)
        {
            if (GetCostToUpgrade(handler) < 1 )
			{
				Owner.SendMessage("This piece of equipment cannot be enhanced with that any further.");
			}
            else if (SpendDust(GetCostToUpgrade(handler)))
            {
                handler.Upgrade(ItemToUpgrade, false);
                BeginProcess();
            }
        }

        private bool SpendDust(int amount)
        {
            bool bought = (Owner.AccessLevel >= AccessLevel.GameMaster);
            bool fromBank = false;

            Container cont = Owner.Backpack;
            if (!bought && cont != null)
            {
                if (cont.ConsumeTotal(typeof(ArcaneDust), amount))
                    bought = true;
                else
                {
                    cont = Owner.FindBankNoCreate();
                    if (cont != null && cont.ConsumeTotal(typeof(ArcaneDust), amount))
                    {
                        bought = true;
                        fromBank = true;
                    }
                    else
                    {
                        Owner.SendLocalizedMessage(500192);
                    }
                }
            }

            if (bought)
            {
                if (Owner.AccessLevel >= AccessLevel.GameMaster)
                    Owner.SendMessage("{0} Arcane Dust would have been withdrawn from your bank if you were not an admin.", amount);
                else if (fromBank)
                    Owner.SendMessage("The total cost of your endeavor is {0} Arcane Dust, which has been withdrawn from your bank account.", amount);
                else
                    Owner.SendMessage("The total cost of your endeavor is {0} Arcane Dust.", amount);
            }

			PlayerMobile pc = (PlayerMobile)Owner;
			if ( pc.NpcGuild == NpcGuild.TailorsGuild ){ Owner.PlaySound( 0x248 ); }
			else if ( pc.NpcGuild == NpcGuild.CarpentersGuild ){ Owner.PlaySound( 0x23D ); }
			else if ( pc.NpcGuild == NpcGuild.ArchersGuild ){ Owner.PlaySound( 0x55 ); }
			else if ( pc.NpcGuild == NpcGuild.TinkersGuild ){ Owner.PlaySound( 0x542 ); }
			else if ( pc.NpcGuild == NpcGuild.BlacksmithsGuild ){ Owner.PlaySound( 0x541 ); }

            return bought;
        }

		public bool IsCraftedByEnhancer( Item item, Mobile from )
		{
			bool crafted = false;

			if ( item is BaseClothing ){ BaseClothing cloth = (BaseClothing)item; if ( cloth.BuiltBy == from ){ crafted = true; } }
			else if ( item is BaseArmor ){ BaseArmor armor = (BaseArmor)item; if ( armor.BuiltBy == from ){ crafted = true; } }
			else if ( item is BaseWeapon ){ BaseWeapon weapon = (BaseWeapon)item; if ( weapon.BuiltBy == from ){ crafted = true; } }

			return crafted;
		}

        public int GetCostToUpgrade(AttributeHandler handler)
        {
            int attrMultiplier = 1;

			int dust = (int)( (double)BaseCost * ( 1.0 + ( (double)MySettings.S_GuildEnhanceMod / 100.0 ) ) );
				if ( IsCraftedByEnhancer( ItemToUpgrade, Owner ) ){ dust = (int)( dust / 2 ); }

            if (AttrCountAffectsCost)
            {
                foreach (AttributeHandler h in AttributeHandler.Definitions)
                    if (h.Name != handler.Name && h.Upgrade(ItemToUpgrade, true) > 0)
                        attrMultiplier++;
            }

            int cost = 0;

            int max = handler.MaxValue;
			int inc = handler.IncrementValue;
            int lvl = handler.Upgrade(ItemToUpgrade, true);

			if ( lvl < max )
				cost = ((lvl+1)*handler.Cost)*dust;
            // original cost quickly scaled up to a couple million gold coins per 1% increase in a prop. 
            // arcane dust usages should still be expensive, but not prohibitively so. 
            cost = (int)(cost * attrMultiplier)/50;

            return cost;
        }
    }
}