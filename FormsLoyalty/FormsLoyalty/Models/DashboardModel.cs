using FormsLoyalty.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FormsLoyalty.Models
{
    public class DashboardGroup  : List<DashboardModel>
    {
        public string Name { get; set; }

        public DashboardGroup(string name, List<DashboardModel> items) : base(items)
        {
            Name = name;
        }
    }

    public class DashboardModel
    {
        public string Description { get; set; }

        public string ArabicDescription { get; set; }
        public string ImageUrl { get; set; }

        public string NewPrice { get; set; }
        public string Price { get; set; }
        public decimal Discount { get; set; }
        public decimal Quantity { get; set; }
        public DashboardItemType ItemType { get; set; }
    }
}
