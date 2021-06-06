using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using System;
using System.Collections.Generic;
using System.Text;

namespace FormsLoyalty.Models
{
    public class Basket : OneListItem
    {

        private string _qty;

        public string Qty
        {
            get { return _qty; }
            set { _qty = value; }
        }

        private string _total;

        public string total
        {
            get { return _total; }
            set { _total = value; }
        }

        private decimal _discountedPrice;

        public decimal DiscountedPrice
        {
            get { return _discountedPrice; }
            set { _discountedPrice = value; }
        }

        private decimal _priceWithDiscount;
        public decimal PriceWithDiscount
        {
            get { return _priceWithDiscount; }
            set { _priceWithDiscount = value; }
        }

        private decimal _priceWithoutDiscount;
        public decimal PriceWithoutDiscount
        {
            get { return _priceWithoutDiscount; }
            set { _priceWithoutDiscount = value; }
        }

    }
}
