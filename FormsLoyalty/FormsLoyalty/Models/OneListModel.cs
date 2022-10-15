using FormsLoyalty.Utils;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;
using LSRetail.Omni.Domain.Services.Loyalty.OneLists;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.OneLists;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FormsLoyalty.Models
{
    public class OneListModel : BaseModel
    {
        private OneListService oneListService;

       

        public async Task<List<OneList>> OneListGetByCardId(string cardId, ListType listType, bool includeLines)
        {
            BeginWsCall();
            return await oneListService.OneListGetByCardIdAsync(cardId, listType, includeLines);
        }

       

        public async Task<OneList> OneListGetById(string contactId, bool includeLines)
        {
            BeginWsCall();
            return await oneListService.OneListGetByIdAsync(contactId, includeLines);
        }

        public async Task<OneList> OneListSave(OneList oneList, bool calculate)
        {
            BeginWsCall();
            oneList.StoreId = AppData.device.UserLoggedOnToDevice.StoreId;
            return await oneListService.OneListSaveAsync(oneList, calculate);
        }

        public async Task<bool> OneListDeleteById(string oneListId)
        {
            BeginWsCall();
            return await oneListService.OneListDeleteByIdAsync(oneListId);
        }

        public async Task<Order> OneListCalculate(OneList oneList)
        {
            AppData.Basket.State = BasketState.Calculating;
            oneList.StoreId = AppData.device.UserLoggedOnToDevice.StoreId;
            oneList.CardId = AppData.Device.CardId;
            BeginWsCall();
            Order order = null;
            try
            {
                order = await oneListService.OneListCalculateAsync(oneList);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
               
            }
            

            if (AppData.Basket.State == BasketState.Calculating)
            {
                AppData.Basket.State = BasketState.Normal;
            }

            return order;
        }

       
        private void BeginWsCall()
        {
            oneListService = new OneListService(new OneListRepository());
        }
    }
}
