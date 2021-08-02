using FormsLoyalty.Helpers;
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
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private ObservableCollection<PublishedOffer> _relatedPublishedOffers;
        public ObservableCollection<PublishedOffer> relatedPublishedOffers
        {
            get { return _relatedPublishedOffers; }
            set { SetProperty(ref _relatedPublishedOffers, value); }
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

        ShoppingListModel shoppingListModel;
        ItemModel itemModel;

        #region Command
        public DelegateCommand BasketCommand { get; set; }
        public DelegateCommand StockCommand { get; set; }
        public DelegateCommand ShareCommand { get; set; }

        public DelegateCommand<ImageView> ShowPreviewCommand => new DelegateCommand<ImageView>(async(data) =>
        {
            if (string.IsNullOrEmpty(data.Image) || data.Image.ToLower().Contains("noimage".ToLower()))
                return;

            await NavigationService.NavigateAsync(nameof(ImagePreviewPage), new NavigationParameters { {"previewImage", data.Image },{"images",Item.Images } });
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

                var filePath = DependencyService.Get<INotify>().GetImageUri(Item.Images[0].Image, Item.Description);
                shareContent.File = new ShareFile(filePath);

                await Share.RequestAsync(shareContent);
            }
            catch (Exception)
            {

                
            }
            
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

            if (!EnabledItems.ForceLogin && AppData.Device.UserLoggedOnToDevice == null)
            {
                AppData.IsLoggedIn = false;
               await NavigationService.NavigateAsync(nameof(LoginPage),new NavigationParameters { { "itemPage",true } });
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
                //SelectVariant();
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
                       // DependencyService.Get<INotify>().ShowSnackBar($"{basketItem.ItemDescription} has been added to basket!!");
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
                else
                {

                }

                if (Item.Prices.Count > 0)
                    itemPrice = Item.PriceFromVariantsAndUOM(Item.SelectedVariant?.Id, Item.SelectedUnitOfMeasure?.Id);

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
            catch (Exception)
            {

                
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
            catch (Exception)
            {

                
            }
          

            return imgview == null ? null : imgview.Image;
        }


        /// <summary>
        /// Generate's items related offer
        /// </summary>
        private async void LoadRelatedPublishedOffers()
        {
            try
            {
                if (AppData.Device.UserLoggedOnToDevice == null)
                {
                    //ShowIndicator(false);
                    return;
                }
                var service = new SharedService(new SharedRepository());
                relatedPublishedOffers = new ObservableCollection<PublishedOffer>(await GetPublishedOffer(service));

                if (relatedPublishedOffers != null && relatedPublishedOffers.Count > 0)
                {
                    LoadOfferImages();
                }
            }
            catch (Exception)
            {

               // await MaterialDialog.Instance.SnackbarAsync(message: "Unable to load related offers",
               //                              msDuration: MaterialSnackbar.DurationShort);
            }

        }


        /// <summary>
        /// Get Item's Related offer image.
        /// </summary>
        private void LoadOfferImages()
        {
            try
            {
                Task.Run(async() =>
                {
                    foreach (var item in relatedPublishedOffers)
                    {
                        if (item.Images.Count > 0)
                        {
                            item.Images[0].Image = await GetImageById(item.Images[0].Id);
                        }
                        else
                            item.Images = new List<ImageView> { new ImageView { Image = "noimage.png" } };
                        
                    }
                });
                

            }
            catch (Exception)
            {


            }
        }

        private async Task<List<PublishedOffer>> GetPublishedOffer(SharedService service)
        {
            try
            {
                return await service.GetPublishedOffersByItemIdAsync(Item.Id, AppData.Device.CardId);
            }
            catch (Exception)
            {

                return null;
            }
           
        }

        /// <summary>
        /// Add or Remove item from wishlist.
        /// </summary>
        /// <returns></returns>
        internal async Task AddRemoveWishList()
        {
            IsPageEnabled = true;
            if (!EnabledItems.ForceLogin && AppData.Device.UserLoggedOnToDevice == null)
            {
                AppData.IsLoggedIn = false;
                await NavigationService.NavigateAsync(nameof(LoginPage));
                
            }
            else
            {
                var existingItem = AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId).ItemGetByIds(Item.Id, Item.SelectedVariant == null ? string.Empty : Item.SelectedVariant.Id, Item.SelectedUnitOfMeasure == null ? string.Empty : Item.SelectedUnitOfMeasure.Id);
                if (existingItem != null)
                {
                    await shoppingListModel.DeleteWishListLine(existingItem.Id, true);
                    WishListIcon = "ic_favorite_outline_24dp";
                }
                else
                {
                    if (await AddToWishList())
                    {
                        WishListIcon = "ic_favorite_24dp";
                        MessagingCenter.Send((App)Xamarin.Forms.Application.Current, "Wishlistadded");

                    }


                }
            }

           

            IsPageEnabled = false;
        }


      
        /// <summary>
        /// Add item to wishlist
        /// </summary>
        /// <returns></returns>
        private async Task<bool> AddToWishList()
        {
           
            if (!EnabledItems.ForceLogin && AppData.Device.UserLoggedOnToDevice == null)
            {

                AppData.IsLoggedIn = false;
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
        private async void GetItemByBarCode(string barcode)
        {
            IsPageEnabled = true;
            try
            {
                
                var item = await itemModel.GetItemByBarcode(barcode);
                if (item == null)
                {
                    NavigationService.GoBackAsync();
                  // await MaterialDialog.Instance.SnackbarAsync(AppResources.ItemModelItemNotFound);
                    return;
                }
                Item = item;
                LoadItem();
                LoadRelatedPublishedOffers();
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
        private async void GetItemById(string Id)
        {
            IsPageEnabled = true;
            try
            {
                Item = await itemModel.GetItemById(Id);
                LoadItem();
                LoadRelatedPublishedOffers();
            }
            catch (Exception)
            {
                IsPageEnabled = false;

            }
            IsPageEnabled = false;

        }


        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            if (parameters.TryGetValue("barcode", out string barcode))
            {
                GetItemByBarCode(barcode);

            }
            else if (parameters.TryGetValue("itemId", out string Id))
            {
                GetItemById(Id);
            }
            else if (parameters.TryGetValue("item", out LoyItem item))
            {
                Item = item;
                LoadItem();
                LoadRelatedPublishedOffers();
            }
           
           
        }

        
    }
}
