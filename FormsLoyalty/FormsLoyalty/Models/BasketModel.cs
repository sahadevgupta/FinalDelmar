using FormsLoyalty.Interfaces;
using FormsLoyalty.Utils;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;
using LSRetail.Omni.Domain.Services.Loyalty.Baskets;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Baskets;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace FormsLoyalty.Models
{
    public class BasketModel : OneListModel
    {
        private BasketService service;

        public async Task<bool> AddItemToBasket(OneListItem item, bool openBasket = false, bool ShowIndicatorOption = true, int index = 0)
        {
            AppData.Device.UserLoggedOnToDevice.GetBasket(AppData.Device.CardId).State = BasketState.Updating;
            

            OneList newList = AppData.Device.UserLoggedOnToDevice.GetBasket(AppData.Device.CardId);
            newList.CardId = AppData.Device.CardId;

           var data =  newList.Items.FirstOrDefault(x => x.ItemId == item.ItemId);
            if(data!=null)
                data.Quantity += item.Quantity;
            
            else
               newList.AddItem(item);

            try
            {
                var list = await OneListSave(newList, true);
                if (list == null)
                {
                    return false;
                }
                else
                {
                    var msg = string.Format(AppResources.txtItemAddedToCart, list.Items[0].ItemDescription);

                    if (Device.RuntimePlatform == Device.Android)
                    {
                        DependencyService.Get<INotify>().ShowSnackBar(msg);
                    }
                    else
                    {
                       await MaterialDialog.Instance.SnackbarAsync(msg, 5000);
                    }
                    newList.Id = list.Id;

                    MessagingCenter.Send(this, "CartUpdated");
                    AppData.Device.UserLoggedOnToDevice.AddList(AppData.Device.CardId, list, ListType.Basket);
                    return true;
                }
                
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                await HandleUIExceptionAsync(ex);
                return false;
               
            }
           
          
        }

        public async Task GetBasketByCardId(string cardId)
        {

            try
            {
                List<OneList> list = await OneListGetByCardId(cardId, ListType.Basket, true);
                OneList basketList = list.FirstOrDefault();
                if (basketList != null)
                {
                    basketList.CalculateBasket();
                    AppData.Device.UserLoggedOnToDevice.AddList(AppData.Device.CardId, basketList, ListType.Basket);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                await HandleUIExceptionAsync(ex);
            }
        }

        public async Task<bool> ClearBasket()
        {
            bool success = false;


            BeginWsCall();

            try
            {
                success = await OneListDeleteById(AppData.Basket.Id);
                if (success)
                {
                    AppData.Basket.Clear();
                    MessagingCenter.Send(this, "CartUpdated");
                    
                }
                
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                await HandleUIExceptionAsync(ex);
            }

            return success;
        }

        public async Task<bool> EditItem(string basketItemId, decimal newQty, VariantRegistration newVariant)
        {
            bool IsSuccess = false;
            var newList = AppData.Device.UserLoggedOnToDevice.GetBasket(AppData.Device.CardId);

            var existingItem = newList.Items.FirstOrDefault(x => x.Id == basketItemId);
            if (existingItem == null)
                IsSuccess = false;
            else
            {
                existingItem.Quantity = newQty;
                if (newVariant != null)
                    existingItem.VariantId = newVariant.Id;


                try
                {
                    var list = await OneListSave(newList, true);
                    AppData.Device.UserLoggedOnToDevice.AddList(AppData.Device.CardId, list, ListType.Basket);
                    IsSuccess = true;
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    Crashes.TrackError(ex);
                    await HandleUIExceptionAsync(ex);
                }
            }
             return IsSuccess;
        }

        public async Task DeleteItem(string basketItemId)
        {
            await DeleteItem(AppData.Basket.Items.FirstOrDefault(x => x.Id == basketItemId));
        }

        public async Task<bool> DeleteItem(OneListItem item)
        {
            OneList newList = AppData.Device.UserLoggedOnToDevice.GetBasket(AppData.Device.CardId);

            OneListItem existingItem = newList.ItemGetByIds(item.ItemId, item.VariantId, item.UnitOfMeasureId);
            if (existingItem == null)
                return false;

            newList.Items.Remove(existingItem);

            try
            {
                var list = await OneListSave(newList, true);
                if (list!=null)
                {
                    MessagingCenter.Send(this, "CartUpdated");
                    AppData.Device.UserLoggedOnToDevice.AddList(AppData.Device.CardId, list, ListType.Basket);
                    return true;
                }
               else
                    return false;

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                await HandleUIExceptionAsync(ex);
                return false;
            }
            
        }

        public async Task<bool> SendOrder(Order basket)
        {
            var success = false;


            AppData.Basket.State = BasketState.Calculating;

            BeginWsCall();

            try
            {
                await service.OrderCreateAsync(basket);
                success = true;

                await ClearBasket();

                ShowToast(AppResources.ResourceManager.GetString("CheckoutViewOrderSuccess", AppResources.Culture));
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                await HandleUIExceptionAsync(ex);
            }


            return success;
        }

        protected void BeginWsCall()
        {
            

            service = new BasketService(new BasketsRepository());
        }
    }
}
