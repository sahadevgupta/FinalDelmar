using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using XF.Material.Forms.UI.Dialogs;

namespace FormsLoyalty.Helpers
{
    public static class GlobalMethods
    {
        public static OneListItem CreateOneListItem(LoyItem loyItem,decimal Quantity)
        {
            OneListItem basketItem = new OneListItem()
            {
                ItemId = loyItem.Id,
                ItemDescription = loyItem.Description,
                Image = loyItem.DefaultImage,
                Quantity = Quantity,
                Price = loyItem.AmtFromVariantsAndUOM(loyItem.SelectedVariant?.Id, loyItem.SelectedUnitOfMeasure?.Id),
                
               
            };

            if (loyItem.Discount > 0)
            {
                basketItem.DiscountAmount = loyItem.ItemPrice - Convert.ToDecimal(loyItem.NewPrice);
                basketItem.DiscountPercent = loyItem.Discount;
            }

            if (loyItem.SelectedVariant != null)
            {
                basketItem.VariantId = loyItem.SelectedVariant.Id;
                basketItem.VariantDescription = loyItem.SelectedVariant.ToString();
            }

            if (loyItem.SelectedUnitOfMeasure != null)
            {
                basketItem.UnitOfMeasureId = loyItem.SelectedUnitOfMeasure.Id;
                basketItem.UnitOfMeasureDescription = loyItem.SelectedUnitOfMeasure.Description;
            }

            return basketItem;
        }




    }
}
