﻿using FormsLoyalty.Helpers;
using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;
using Microsoft.AppCenter.Crashes;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace FormsLoyalty.ViewModels
{
    public class ItemPageViewModel : ViewModelBase
    {
        private LoyItem _Item;
        public LoyItem Item
        {
            get { return _Item; }
            set { SetProperty(ref _Item, value); }
        }

        private string _wishlistIcon = "ic_favorite_outline_24dp";

        

        public string WishListIcon
        {
            get { return _wishlistIcon; }
            set { SetProperty(ref _wishlistIcon, value); }
        }

        private string _itemPrice;
        public string itemPrice
        {
            get { return _itemPrice; }
            set { SetProperty(ref _itemPrice, value); }
        }

        private decimal _quantity = 1;
        public decimal Quantity
        {
            get { return _quantity; }
            set { SetProperty(ref _quantity, value); }
        }

        private ObservableCollection<LoyItem> _relatedItems;
        public ObservableCollection<LoyItem> RelatedItems
        {
            get { return _relatedItems; }
            set { SetProperty(ref _relatedItems, value); }
        }

        private string _bastetBtn = AppResources.ResourceManager.GetString("ApplicationAddToBasket", AppResources.Culture);
        public string BasketBtn
        {
            get { return _bastetBtn; }
            set { SetProperty(ref _bastetBtn, value); }
        }


        private string _selectVariant;
        public string selectVariant
        {
            get { return _selectVariant; }
            set { SetProperty(ref _selectVariant, value); }
        }

        readonly ShoppingListModel shoppingListModel;
        readonly ItemModel itemModel;

        #region Command
        public DelegateCommand BasketCommand { get; set; }
        public DelegateCommand StockCommand { get; set; }
        public DelegateCommand ShareCommand { get; set; }

        public DelegateCommand<ImageView> ShowPreviewCommand => new DelegateCommand<ImageView>(async(data) =>
        {
            if (string.IsNullOrEmpty(data.Location))
                return;

            await NavigationService.NavigateAsync(nameof(ImagePreviewPage), new NavigationParameters { {"previewImage", data.Location },{"images",Item.Images } });
        });

        #endregion


        public ItemPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            shoppingListModel = new ShoppingListModel();
            itemModel = new ItemModel();
           

            BasketCommand = new DelegateCommand(async() => await AddToBasket());
            StockCommand = new DelegateCommand(async() => await ViewAvailability());
            ShareCommand = new DelegateCommand(async () => await ShareItem());
        }

        /// <summary>
        /// This method is used to share the item to another app.
        /// <ex>Whatsapp,Hike,WeChat</ex>
        /// </summary>
        /// <returns></returns>
        internal async Task ShareItem()
        {
            try
            {

                var shareContent = new ShareFileRequest();
                shareContent.Title = Item.Description;

                var filePath = DependencyService.Get<INotify>().GetImageUri(Item.Images[0].Location, Item.Description);
                shareContent.File = new ShareFile(filePath);

                await Share.RequestAsync(shareContent);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

        }

        /// <summary>
        /// Change selected Item based on related item selection
        /// </summary>
        /// <param name="selectedItem"></param>
        internal async Task ExecuteChangeSelectedItem(LoyItem selectedItem)
        {
            if (IsPageEnabled)
                return;

            IsPageEnabled = true;
            Item = selectedItem;

            await LoadRelatedItems();

            IsPageEnabled = false;
        }


        /// <summary>
        /// Navigates to offer Detail page
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        internal async Task NavigateToOfferDetail(PublishedOffer data)
        {
            IsPageEnabled = true;
            await NavigationService.NavigateAsync(nameof(OfferDetailsPage), new NavigationParameters { { "offer", data } });
            IsPageEnabled = false;
        }

        /// <summary>
        /// This method is used to check the stock availability in nearby stores.
        /// </summary>
        /// <returns></returns>
        private async Task ViewAvailability()
        {
            IsPageEnabled = true;

           
                if (Item == null)
                    return;

                var itemDescription = Item.Description;
                var variantId = "";
                if (Item.SelectedVariant != null)
                {
                    itemDescription += " - " + Item.SelectedVariant.ToString();
                    variantId = Item.SelectedVariant.Id;
                }

                if (string.IsNullOrEmpty(variantId) && Item.VariantsRegistration != null && Item.VariantsRegistration.Count > 0)
                {
                      await MaterialDialog.Instance.SnackbarAsync(message: AppResources.ResourceManager.GetString("ItemViewPickVariant", AppResources.Culture),
                                           msDuration: MaterialSnackbar.DurationLong);
               
                }
                else
                {
                AppData.IsViewStock = true;
                    
                   await NavigationService.NavigateAsync(nameof(StoreLocatorPage),new NavigationParameters { {"viewStock", Item } });
                }
            IsPageEnabled = false;
        }

        /// <summary>
        /// This method is used to add item in the basket.
        /// </summary>
        /// <returns></returns>
        private async Task AddToBasket()
        {
            IsPageEnabled = true;

            if (!AppData.IsLoggedIn)
            {
                await NavigationService.NavigateAsync(nameof(LoginPage), new NavigationParameters { { "itemPage", true } });
                IsPageEnabled = false;
                return;
            }

            
            if (BasketBtn == AppResources.ResourceManager.GetString("ApplicationAddedToBasket", AppResources.Culture))
            {
                BasketBtn = AppResources.ResourceManager.GetString("ApplicationAddToBasket", AppResources.Culture);
                IsPageEnabled = false;
                return;
            }

            OneListItem basketItem = GlobalMethods.CreateOneListItem(Item, Quantity);

            if (string.IsNullOrEmpty(basketItem.VariantId) && Item.VariantsRegistration != null && Item.VariantsRegistration.Count > 0)
            {
                await MaterialDialog.Instance.SnackbarAsync(message: AppResources.ResourceManager.GetString("ItemViewPickVariant", AppResources.Culture),
                                           msDuration: MaterialSnackbar.DurationLong);
                throw new FileNotFoundException();
            }
            else
            {
                try
                {
                    var basketModel = new BasketModel();
                    var isSuccess = await basketModel.AddItemToBasket(basketItem);
                    if (isSuccess)
                    {
                        BasketBtn = AppResources.ResourceManager.GetString("ApplicationAddedToBasket", AppResources.Culture);
                    }
                    
                   
                }
                catch (Exception)
                {

                    IsPageEnabled = false;
                }
                
            }



            IsPageEnabled = false;
        }

        /// <summary>
        /// This method is used to set any variant change in the item.
        /// </summary>
        /// <param name="newVariantRegistration"></param>
        internal void changeVariant(VariantRegistration newVariantRegistration)
        {
            Item.SelectedVariant = newVariantRegistration;
            selectVariant = Item.SelectedVariant.ToString();

            CalculatePrice();
        }
        private void CalculatePrice()
        {
            itemPrice = Item.PriceFromVariantsAndUOM(Item.SelectedVariant?.Id, Item.SelectedUnitOfMeasure?.Id);
            if (string.IsNullOrEmpty(itemPrice))
            {
                itemPrice = Item.ItemPrice.ToString();
            }
        }

        /// <summary>
        /// This method is used to load Items
        /// </summary>
        internal void LoadItem()
        {
            try
            {


                string selectedVariantId = string.Empty;

                if (Item.VariantsRegistration.Count > 0)
                {
                    if (Item.SelectedVariant == null)
                    {
                        if (string.IsNullOrEmpty(selectedVariantId) && Item.VariantsRegistration?.Count > 0)
                        {
                            selectedVariantId = Item.VariantsRegistration[0].Id;
                        }

                        Item.SelectedVariant = Item.VariantsRegistration.FirstOrDefault(x => x.Id == selectedVariantId);
                    }

                    if (Item.SelectedVariant != null)
                    {
                        VariantExt.SetIsSelectedFromVariantReg(Item.VariantsExt, Item.SelectedVariant);
                        selectVariant = Item.SelectedVariant.ToString();
                    }
                }

                itemPrice = Item.PriceFromVariantsAndUOM(Item.SelectedVariant?.Id, Item.SelectedUnitOfMeasure?.Id);
                if (string.IsNullOrEmpty(itemPrice))
                {
                    itemPrice = Item.ItemPrice.ToString();
                }

                if (Item.Discount > 0)
                {
                    var discountedPrice = (Item.Discount / 100) * Convert.ToDecimal(Item.ItemPrice);
                    Item.NewPrice = (Convert.ToDecimal(Item.ItemPrice) - discountedPrice).ToString("F", CultureInfo.InvariantCulture);
                }


                if (AppData.Device.UserLoggedOnToDevice != null)
                {
                    var existingItem = AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId).ItemGetByIds(Item.Id, Item.SelectedVariant == null ? string.Empty : Item.SelectedVariant.Id, Item.SelectedUnitOfMeasure == null ? string.Empty : Item.SelectedUnitOfMeasure.Id);
                    if (existingItem == null)
                    {
                        WishListIcon = "ic_favorite_outline_24dp";
                    }
                    else
                    {
                        WishListIcon = "ic_favorite_24dp";
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

        }


        /// <summary>
        /// Get item image
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Image in base64 format</returns>
        internal async Task<string> GetImageById(string id)
        {
            ImageView imgview = new ImageView();
            try
            {
               
                imgview = await ImageHelper.GetImageById(id, new ImageSize(396, 396));
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }


            return imgview == null ? null : imgview.Image;
        }


        /// <summary>
        /// Generate's items related offer
        /// </summary>
        private async Task LoadRelatedItems()
        {
            try
            {
                var relatedItem = await itemModel.ItemsGetByRelatedItemIdAsync(Item.Id, 10);
                if (relatedItem.Any(x => x.Id == Item.Id))
                {
                    var index = relatedItem.IndexOf(relatedItem.FirstOrDefault(x => x.Id == Item.Id));
                    relatedItem.RemoveAt(index);
                }
                RelatedItems = new ObservableCollection<LoyItem>(relatedItem);

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

        }

        /// <summary>
        /// Add or Remove item from wishlist.
        /// </summary>
        /// <returns></returns>
        internal async Task AddRemoveWishList()
        {
            IsPageEnabled = true;

            if(!AppData.IsLoggedIn)
            {
                await NavigationService.NavigateAsync(nameof(LoginPage));
                
            }
            else
            {
                var existingItem = AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId).ItemGetByIds(Item.Id, Item.SelectedVariant == null ? string.Empty : Item.SelectedVariant.Id, Item.SelectedUnitOfMeasure == null ? string.Empty : Item.SelectedUnitOfMeasure.Id);
                if (existingItem != null)
                {
                    await shoppingListModel.DeleteWishListLine(existingItem.Id, true);
                    WishListIcon = "ic_favorite_outline_24dp";
                    Item.IsWishlisted = false;
                }
                else
                {
                    if (await AddToWishList())
                    {
                        Item.IsWishlisted = true;
                        WishListIcon = "ic_favorite_24dp";
                        MessagingCenter.Send((App)Xamarin.Forms.Application.Current, "Wishlistadded");

                    }


                }
                MessagingCenter.Send(Item, "WhistlistChange");
            }

           

            IsPageEnabled = false;
        }


      
        /// <summary>
        /// Add item to wishlist
        /// </summary>
        /// <returns></returns>
        private async Task<bool> AddToWishList()
        {
           
            if (!AppData.IsLoggedIn)
            {

                await NavigationService.NavigateAsync(nameof(LoginPage));

                return false;
            }
            else
            {
                OneListItem line = GlobalMethods.CreateOneListItem(Item, 1);
                

                if (string.IsNullOrEmpty(line.VariantId) && Item.VariantsRegistration != null && Item.VariantsRegistration.Count > 0)
                {
                    await MaterialDialog.Instance.SnackbarAsync(message: AppResources.ResourceManager.GetString("ItemViewPickVariant", AppResources.Culture),
                                           msDuration: MaterialSnackbar.DurationLong);
                    return false;
                }
                else
                {
                    try
                    {
                        await shoppingListModel.AddItemToWishList(line);
                    }
                    catch (Exception)
                    {

                        return false;
                    }
                    
                }

                return true;
            }
        }


        /// <summary>
        /// Get item using barcode
        /// </summary>
        /// <param name="barcode"></param>
        private async Task GetItemByBarCode(string barcode)
        {
            IsPageEnabled = true;
            try
            {
                
                var item = await itemModel.GetItemByBarcode(barcode);
                if (item == null)
                {
                   await NavigationService.GoBackAsync();
                    return;
                }
                Item = item;
                LoadItem();
               await LoadRelatedItems();
            }
            catch (Exception)
            {
                IsPageEnabled = false;

            }
            IsPageEnabled = false;
        }

        /// <summary>
        /// Get item using Id
        /// </summary>
        /// <param name="Id"></param>
        /// return's Item
        private async Task GetItemById(string Id)
        {
            IsPageEnabled = true;
            try
            {
                Item = await itemModel.GetItemById(Id);
                LoadItem();
               await LoadRelatedItems();
            }
            catch (Exception)
            {
                IsPageEnabled = false;

            }
            IsPageEnabled = false;

        }


        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            if (parameters.TryGetValue("barcode", out string barcode))
            {
               await GetItemByBarCode(barcode);

            }
            else if (parameters.TryGetValue("itemId", out string Id))
            {
               await GetItemById(Id);
            }
            else if (parameters.TryGetValue("item", out LoyItem item))
            {
                Item = item;
                LoadItem();
               await LoadRelatedItems();
            }
           
           
        }

        
    }
}
