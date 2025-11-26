using System;
using Server;
using Server.Items;
using System.Collections;

namespace Server.Misc
{
	public class Arty
	{
		public static void ArtySetup(Item item, string extra)
		{
		    if (item is BaseGiftArmor)
		    {
		        ((BaseGiftArmor)item).m_Gifter = null;
		        ((BaseGiftArmor)item).m_How = "Unearthed by";
		    }
		    else if (item is BaseGiftClothing)
		    {
		        ((BaseGiftClothing)item).m_Gifter = null;
		        ((BaseGiftClothing)item).m_How = "Unearthed by";
		    }
		    else if (item is BaseGiftJewel)
		    {
		        ((BaseGiftJewel)item).m_Gifter = null;
		        ((BaseGiftJewel)item).m_How = "Unearthed by";
		    }
		    else if (item is BaseGiftShield)
		    {
		        ((BaseGiftShield)item).m_Gifter = null;
		        ((BaseGiftShield)item).m_How = "Unearthed by";
		    }
		    else if (item is BaseGiftAxe)
		    {
		        ((BaseGiftAxe)item).m_Gifter = null;
		        ((BaseGiftAxe)item).m_How = "Unearthed by";
		    }
		    else if (item is BaseGiftKnife)
		    {
		        ((BaseGiftKnife)item).m_Gifter = null;
		        ((BaseGiftKnife)item).m_How = "Unearthed by";
		    }
		    else if (item is BaseGiftBashing)
		    {
		        ((BaseGiftBashing)item).m_Gifter = null;
		        ((BaseGiftBashing)item).m_How = "Unearthed by";
		    }
		    else if (item is BaseGiftWhip)
		    {
		        ((BaseGiftWhip)item).m_Gifter = null;
		        ((BaseGiftWhip)item).m_How = "Unearthed by";
		    }
		    else if (item is BaseGiftPoleArm)
		    {
		        ((BaseGiftPoleArm)item).m_Gifter = null;
		        ((BaseGiftPoleArm)item).m_How = "Unearthed by";
		    }
		    else if (item is BaseGiftRanged)
		    {
		        ((BaseGiftRanged)item).m_Gifter = null;
		        ((BaseGiftRanged)item).m_How = "Unearthed by";
		    }
		    else if (item is BaseGiftSpear)
		    {
		        ((BaseGiftSpear)item).m_Gifter = null;
		        ((BaseGiftSpear)item).m_How = "Unearthed by";
		    }
		    else if (item is BaseGiftStaff)
		    {
		        ((BaseGiftStaff)item).m_Gifter = null;
		        ((BaseGiftStaff)item).m_How = "Unearthed by";
		    }
		    else if (item is BaseGiftSword)
		    {
		        ((BaseGiftSword)item).m_Gifter = null;
		        ((BaseGiftSword)item).m_How = "Unearthed by";
		    }	

		      if (extra != null)
    		    extra = extra.Trim();

    		if (!string.IsNullOrEmpty(extra))
    		{
    		    extra = extra.Trim();
				try
    		    {
    		        item.GetType().GetProperty("ArtifactExtra").SetValue(item, extra, null);
    		    }
    		    catch { }
    		}
		}


		public static void setArtifact( Item item )
		{
			if ( item.ArtifactLevel > 0 )
			{
				Type itemType = item.GetType();
				Item arty = null;

				if ( itemType != null )
				{
					arty = (Item)Activator.CreateInstance(itemType);
					item.Name = arty.Name;
					if ( !(item is BaseQuiver) && !MySettings.S_ChangeArtyLook ){ item.ItemID = arty.ItemID; }
					if ( !MySettings.S_ChangeArtyLook ){ item.Hue = arty.Hue; }

					arty.Delete();
				}
			}
		}
	}
}