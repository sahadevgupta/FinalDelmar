using FormsLoyalty.Utils;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XF.Material.Forms.UI.Dialogs;

namespace FormsLoyalty.Models
{
    public class ShoppingListModel : OneListModel
    {
       

        public bool ItemIsInWishList(LoyItem item, VariantRegistration variant, UnitOfMeasure uom)
        {
            if (AppData.Device.UserLoggedOnToDevice == null)
                return false;

            var existingItem = AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId).ItemGetByIds(item.Id, variant == null?string.Empty :variant.Id, uom == null ? string.Empty :uom.Id);

            return existingItem != null;
        }

        public async Task WishListSave(OneList wishList)
        {

          
            try
            {
                var list = await OneListSave(wishList, false);

                AppData.Device.UserLoggedOnToDevice.AddList(AppData.Device.CardId, list, ListType.Wish);

               // SendBroadcast(Utils.BroadcastUtils.ShoppingListUpdated); need to configure
            }
            catch (Exception ex)
            {
               await HandleUIExceptionAsync(ex);
            }

        }

        

        public async Task AddItemToWishList(OneListItem line, int index = 0)
        {

            var newList = AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId).Clone();
            newList.Items.Insert(index, line);

            try
            {
                var list = await OneListSave(newList, false);

                AppData.Device.UserLoggedOnToDevice.AddList(AppData.Device.CardId, list, ListType.Wish);

              //  SendBroadcast(Utils.BroadcastUtils.ShoppingListUpdated);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

        }

        public async Task GetShoppingListsByCardId(string cardId)
        {

            try
            {
                var list = await OneListGetByCardId(cardId, ListType.Wish, true);

                AppData.Device.UserLoggedOnToDevice.AddList(AppData.Device.CardId, list.FirstOrDefault(), ListType.Wish);

                //SendBroadcast(Utils.BroadcastUtils.ShoppingListsUpdated);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }
            finally
            {
               // ShowIndicator(false);
            }
        }

        public async Task<bool> DeleteWishListLine(string wishListLineId,bool fromItemPage = false)
        {

            var newList = AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId).Clone();

            var deletedItem = newList.Items.FirstOrDefault(x => x.Id == wishListLineId);
            var deletedItemIndex = newList.Items.IndexOf(deletedItem);

            newList.Items.RemoveAll(x => x.Id == wishListLineId);

            try
            {
                var list = await OneListSave(newList, false);
                if (list != null)
                {
                    AppData.Device.UserLoggedOnToDevice.AddList(AppData.Device.CardId, list, ListType.Wish);
                    return true;
                }
                else
                    return false;

               // SendBroadcast(Utils.BroadcastUtils.ShoppingListUpdated);

            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
                return false;
            }
            finally
            {
               // ShowIndicator(false);
            }
        }

        public void ClearWishListWithConfirmation()
        {
            var message =AppResources.ResourceManager.GetString("ShoppingListViewClearConfirmation",AppResources.Culture);
              throw new FileNotFoundException();
            //var dialog = new WarningDialog(Context, Context.GetString(Resource.String.ShoppingListDetailViewWishlist))
            //                    .SetPositiveButton(Context.GetString(Resource.String.ApplicationYes),
            //                                        () => DeleteWishList());
            //dialog.Message = message;
            //dialog.SetNegativeButton(Context.GetString(Resource.String.ApplicationNo), delegate () { });
            //dialog.Show();
        }

        public async void DeleteWishList()
        {
            if (string.IsNullOrEmpty(AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId).Id))
            {
                AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId).Items.Clear();
                return;
            }



            try
            {
                var success = await this.OneListDeleteById(AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId).Id);

                if (success)
                {
                   // SendBroadcast(Utils.BroadcastUtils.ShoppingListUpdated);
                }
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

        }
    }
}
