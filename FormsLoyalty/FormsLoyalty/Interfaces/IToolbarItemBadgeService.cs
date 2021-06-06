using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace FormsLoyalty.Interfaces
{
    public interface IToolbarItemBadgeService
    {
        void SetBadge(Page page, ToolbarItem toolbarItem, string value, Color backgroundColor, Color textColor);
        void SetTabBadge(TabbedPage page, Page BadgePage, string value);
    }
}
