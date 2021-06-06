using FormsLoyalty.Utils;
using LSRetail.Omni.Domain.DataModel.Base.Requests;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;
using LSRetail.Omni.Domain.Services.Loyalty.Baskets;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Baskets;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FormsLoyalty.Models
{
    public class ClickCollectModel : BaseModel
    {
        private BasketService service;
        private BasketModel basketModel;

        public ClickCollectModel()
        {
            basketModel = new BasketModel();
        }

        public async Task<List<OrderLineAvailability>> OrderAvailabilityCheck(string storeId)
        {
            BeginWsCall();

            List<InventoryResponse> orderLineAvailabilities = null;
            try
            {
                List<InventoryRequest> items = new List<InventoryRequest>();
                foreach (OneListItem item in AppData.Basket.Items)
                {
                    items.Add(new InventoryRequest()
                    {
                        ItemId = item.ItemId,
                        VariantId = item.VariantId
                    });
                }
                orderLineAvailabilities = await service.ItemsInStoreGetAsync(items, storeId);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                await HandleUIExceptionAsync(ex);
            }


            List<OrderLineAvailability> list = new List<OrderLineAvailability>();
            foreach (InventoryResponse line in orderLineAvailabilities)
            {
                list.Add(new OrderLineAvailability()
                {
                    ItemId = line.ItemId,
                    VariantId = line.VariantId,
                    Quantity = line.QtyInventory
                });
            }
            return list;
        }

        public async Task<bool> ClickCollectOrderCreate(Order basket)
        {
            bool success = false;
            BeginWsCall();


            try
            {
                //success = await service.OrderCreateAsync(service.CreateOrderForCAC(basket, contactId, cardId, storeId, email));
                Order result = await service.OrderCreateAsync(basket);
                if (result != null)
                {
                    await basketModel.ClearBasket();
                    ShowToast(AppResources.ResourceManager.GetString("CheckoutViewOrderSuccess",AppResources.Culture));
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                await HandleUIExceptionAsync(ex);
            }

            return success;
        }

        public List<OneListItem> CreateBasketItems(List<OrderLineAvailability> orderLineAvailabilities)
        {
            var basketItems = new List<OneListItem>();
            var unavailableItems = new List<string>();

            foreach (var orderLineAvailability in orderLineAvailabilities)
            {
                OneListItem basketItem = AppData.Basket.ItemGetByIds(orderLineAvailability.ItemId, orderLineAvailability.VariantId, orderLineAvailability.UomId);
                if (basketItem == null)
                    continue;

                if (orderLineAvailability.Quantity > 0 && basketItem != null)
                {
                    var availableBasketItem = new OneListItem()
                    {
                        Id = basketItem.Id,
                        ItemId = basketItem.ItemId,
                        Amount = basketItem.Amount,
                        NetAmount = basketItem.NetAmount,
                        NetPrice = basketItem.NetPrice,
                        Price = basketItem.Price,
                        TaxAmount = basketItem.TaxAmount,
                        Quantity = basketItem.Quantity,
                        UnitOfMeasureId = basketItem.UnitOfMeasureId,
                        VariantId = basketItem.VariantId,
                    };

                    if (basketItem.Quantity > orderLineAvailability.Quantity)
                    {
                        unavailableItems.Add("-" + (basketItem.Quantity - orderLineAvailability.Quantity) + " " + basketItem.ItemDescription);
                        availableBasketItem.Quantity = orderLineAvailability.Quantity;
                    }

                    basketItems.Add(availableBasketItem);
                }
                else
                {
                    if (basketItem != null)
                        unavailableItems.Add("-" + (basketItem.Quantity) + " " + basketItem.ItemDescription);
                }
            }

            return basketItems;
        }

        protected  void BeginWsCall()
        {
            service = new BasketService(new BasketsRepository());
        }
    }
}
