using FormsLoyalty.Interfaces;
using FormsLoyalty.Utils;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.Services.Loyalty.Items;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Setup;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FormsLoyalty.Models
{
    public class ItemModel  : BaseModel
    {
        private ItemService service;
        private string lastSearchKey;

       

        public async Task<List<LoyItem>> GetItemsByPage(int pageSize, int pageNumber, string categoryId, string productGroupId, string search,bool IsDesc,string sortingMethod)
        {
            List<LoyItem> items = null;

            lastSearchKey = search;

            BeginWsCall();

            try
            {
                items = await service.GetItemsByPageAsync(pageSize, pageNumber, categoryId, productGroupId, search, true, IsDesc,sortingMethod);

                if (search != lastSearchKey)
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                await HandleUIExceptionAsync(ex);
            }

            return items;
        }


        public async Task<List<LoyItem>> GetItemsByItemSearch(string search, int maxResult)
        {
            List<LoyItem> items = null;

            lastSearchKey = search;

            BeginWsCall();

            try
            {
                items = await service.GetItemsByItemSearchAsync(search, maxResult, true);

                if (search != lastSearchKey)
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            return items;
        }

        private void BeginWsCall()
        {
            service = new ItemService(new LoyItemRepository());
        }

        public async Task<List<LoyItem>> GetBestSellerItems(int count,bool includeDetails)
        {
            List<LoyItem> bestSellerItems = new List<LoyItem>();
            BeginWsCall();
            try
            {
                bestSellerItems = await service.GetBestSellerItems(count, includeDetails);
                AppData.BestSellers = bestSellerItems;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                await HandleUIExceptionAsync(ex);
            }
            return bestSellerItems;
        }

        public async Task<List<LoyItem>> GetMostViewedItems(int count,bool includeDetails)
        {
            List<LoyItem> mostViewedItems = new List<LoyItem>();
            BeginWsCall();
            try
            {
                mostViewedItems = await service.GetMostViewedItems(count, includeDetails);
                AppData.MostViewed = mostViewedItems;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                await HandleUIExceptionAsync(ex);
            }
            return mostViewedItems;
        }


        public async Task<List<ItemCategory>> GetItemCategories()
        {

            BeginWsCall();

            try
            {
                var categories = await service.GetItemCategoriesAsync();
                AppData.ItemCategories = categories;
                return categories;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                await HandleUIExceptionAsync(ex);
                return null;
            }

        }

        public async Task<List<LoyItem>> ItemsGetByPublishedOfferIdAsync(string publishedOfferId)
        {
            List<LoyItem> items = null;


            BeginWsCall();

            try
            {
                items = await service.GetItemsByPublishedOfferIdAsync(publishedOfferId, Int32.MaxValue);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                await HandleUIExceptionAsync(ex);
            }


            return items;
        }

        public async Task<LoyItem> GetItemById(string itemId, bool ShowRefreshableIndicatorOption = true)
        {
            LoyItem item = null;


            BeginWsCall();

            try
            {
                item = await service.GetItemAsync(itemId);

                if (item == null)
                   return null;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                if (ex is NullReferenceException)
                {
                    DependencyService.Get<INotify>().ShowToast(AppResources.ResourceManager.GetString( "ItemModelItemNotFound",AppResources.Culture));
                }
                else
                {

                    await HandleUIExceptionAsync(ex, showAsToast: true);
                }
            }

           

            return item;
        }

        public async Task<LoyItem> GetItemByBarcode(string barcode)
        {
            LoyItem item = null;


            BeginWsCall();

            try
            {
                item = await service.GetItemByBarcodeAsync(barcode);

                if (item == null)
                    return null;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                if (ex is NullReferenceException)
                {
                    DependencyService.Get<INotify>().ShowToast(AppResources.ResourceManager.GetString("ItemModelItemNotFound", AppResources.Culture));
                }
                else
                {
                    await HandleUIExceptionAsync(ex, showAsToast: true);
                }
            }


            return item;
        }

        

        
    }
}
