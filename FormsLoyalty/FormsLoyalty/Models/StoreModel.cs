using FormsLoyalty.Utils;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.Services.Loyalty.Stores;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Setup;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FormsLoyalty.Models
{
    public class StoreModel : BaseModel
    {
        private StoreService service;

      
        public async Task GetAllStores()
        {

            BeginWsCall();

            try
            {
                AppData.Stores = await service.GetStoresAsync();
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

        }

        public async Task<List<Store>> GetItemsInStock(string itemId, string variantId, double longitde, double latitude, double maxDistance)
        {
            List<Store> stores = null;


            BeginWsCall();

            try
            {
                stores = await service.GetItemsInStockAsync(itemId, variantId, longitde, latitude, maxDistance, Int32.MaxValue);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }


            return stores;
        }

        protected void BeginWsCall()
        {
            service = new StoreService(new StoreRepository());
        }
    }
}
