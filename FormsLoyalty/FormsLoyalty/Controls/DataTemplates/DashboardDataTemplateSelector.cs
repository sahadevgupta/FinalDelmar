using FormsLoyalty.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace FormsLoyalty.Controls.DataTemplates
{
    public class DashboardDataTemplateSelector : DataTemplateSelector
    {
        //public DataTemplate AdsTemplate { get; set; }
        public DataTemplate CategoriesTemplate { get; set; }
        public DataTemplate BestSellerTemplate { get; set; }
        public DataTemplate OffersTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            DataTemplate dataTemplate = default;
            var dashboardData = (DashboardModel)item;
            switch (dashboardData.ItemType)
            {
                case Models.Enums.DashboardItemType.Categories:
                    dataTemplate = CategoriesTemplate;
                    break;
                case Models.Enums.DashboardItemType.BestSeller:
                    dataTemplate = BestSellerTemplate;
                    break;
                case Models.Enums.DashboardItemType.Offers:
                    dataTemplate = OffersTemplate;
                    break;
            }
            return dataTemplate;
        }

        
    }
}
