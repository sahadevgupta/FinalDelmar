using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace FormsLoyalty.iOS
{
    public class LinkerPleaseInclude
    {
        public void Include(UITabBarItem item)
        {
            item.BadgeColor = UIColor.Red;
            item.BadgeValue = "badge";
        }
    }
}