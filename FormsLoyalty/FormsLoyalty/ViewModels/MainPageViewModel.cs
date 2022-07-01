﻿using FormsLoyalty.Helpers;
using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;
using Microsoft.AppCenter.Crashes;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;
using DependencyService = Xamarin.Forms.DependencyService;
using NavigationMode = Prism.Navigation.NavigationMode;
using Timer = System.Timers.Timer;

namespace FormsLoyalty.ViewModels
{
    public class MainPageViewModel : MainTabbedPageViewModel
    {
        private ObservableCollection<Advertisement> _advertisements;
        public ObservableCollection<Advertisement> advertisements
        {
            get { return _advertisements; }
            set { SetProperty(ref _advertisements, value); }
        }

        private ObservableCollection<ItemCategory> _itemCategories;
        public ObservableCollection<ItemCategory> itemCategories
        {
            get { return _itemCategories; }
            set { SetProperty(ref _itemCategories, value); }
        }

        private ObservableCollection<LoyItem> _bestSellerItems;
        public ObservableCollection<LoyItem> BestSellerItems
        {
            get { return _bestSellerItems; }
            set { SetProperty(ref _bestSellerItems, value); }
        }

        private ObservableCollection<LoyItem> _mostViewedItems;
        public ObservableCollection<LoyItem> MostViewedItems
        {
            get { return _mostViewedItems; }
            set { SetProperty(ref _mostViewedItems, value); }
        }

        private LayoutState _adsCurrentState;
        public LayoutState AdsCurrentState
        {
            get { return _adsCurrentState; }
            set { SetProperty(ref _adsCurrentState, value); }
        }

        private LayoutState _categoryCurrentState;
        public LayoutState CategoryCurrentState
        {
            get { return _categoryCurrentState; }
            set { SetProperty(ref _categoryCurrentState, value); }
        }

        private LayoutState _bestSellerCurrentState;
        public LayoutState BestSellerCurrentState
        {
            get { return _bestSellerCurrentState; }
            set { SetProperty(ref _bestSellerCurrentState, value); }
        }

        private LayoutState _offerCurrentState;
        public LayoutState OfferCurrentState
        {
            get { return _offerCurrentState; }
            set { SetProperty(ref _offerCurrentState, value); }
        }


        private double _height;
        public double Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value); }
        }


        #region Search
        private int lastSearchLength;

        private string _searchKey;
        public string SearchKey
        {
            get { return _searchKey; }
            set
            {
                SetProperty(ref _searchKey, value);
                if (string.IsNullOrEmpty(value))
                {
                    IsSuggestionFound = false;
                }
            }
        }

        private ObservableCollection<LoyItem> _items =new ObservableCollection<LoyItem>();
        public ObservableCollection<LoyItem> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        private string _myPoints;
        public string MyPoints
        {
            get { return _myPoints; }
            set { SetProperty(ref _myPoints, value); }
        }

       

        GeneralSearchModel searchModel;
        ItemModel itemModel;
        #endregion

        #region Offer
        private ObservableCollection<PublishedOffer> _offers = new ObservableCollection<PublishedOffer>();
        public ObservableCollection<PublishedOffer> offers
        {
            get { return _offers; }
            set { SetProperty(ref _offers, value); }
        }

        private bool _isSuggestionFound;
        public bool IsSuggestionFound
        {
            get { return _isSuggestionFound; }
            set { SetProperty(ref _isSuggestionFound, value); }
        }

        #endregion

        public bool CanNavigate;
        public bool IsUploadBtnClicked;

        #region Commands
        public DelegateCommand<Advertisement> OnAdTappedCommand { get; set; }
        public DelegateCommand SearchCommand { get; set; }
        #endregion

        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService)
        {
            AdsCurrentState = CategoryCurrentState = BestSellerCurrentState = OfferCurrentState = LayoutState.Loading;
            Height = DeviceDisplay.MainDisplayInfo.Height;
            Title = AppResources.ResourceManager.GetString("ApplicationTitle",AppResources.Culture);
            AppData.IsLoggedIn = AppData.Device.UserLoggedOnToDevice == null ? false : true;

            App.dialogService = pageDialogService;
            searchModel = new GeneralSearchModel();
            itemModel = new ItemModel();

            LoadPoints();

            IsActiveChanged += HandleIsActiveTrue;

            

            OnAdTappedCommand = new DelegateCommand<Advertisement>(async(data) => await OnAdSelected(data));
            SearchCommand = new DelegateCommand(async() => await ExecuteSearchCommandAsync());
        }

       

        private async Task OnAdSelected(Advertisement data)
        {
            IsPageEnabled = true;
            try
            {
                switch (data.AdsType)
                {
                    case AdsType.Item:
                        if(!string.IsNullOrEmpty(data.AppValue))
                             await NavigationService.NavigateAsync(nameof(ItemPage), new NavigationParameters { { "itemId", data.AppValue } });

                        break;
                    case AdsType.Offer:
                        if (!string.IsNullOrEmpty(data.AppValue))
                        {

                            var offer = AppData.PublishedOffers.FirstOrDefault(x => x.Id == data.AppValue);
                            if (offer != null)
                            {

                                await NavigationService.NavigateAsync(nameof(OfferDetailsPage), new NavigationParameters { { "offer", offer } });

                            }
                        }
                        break;
                    case AdsType.Magazine:
                        if (!string.IsNullOrEmpty(data.AppValue))
                            await Launcher.TryOpenAsync(new Uri(data.AppValue));
                        break;
                    case AdsType.Search:
                        if (!string.IsNullOrEmpty(data.AppValue))
                        {


                            var result = data.AppValue.Split(',');

                            var items = await itemModel.GetItemsByPage(7, 1, result[0], result[1], result[2], false, string.Empty);
                            if (items.Any())
                            {
                                await NavigationService.NavigateAsync(nameof(ItemGroupPage), new NavigationParameters { { "items", items } });

                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

                Crashes.TrackError(ex);
            }
            finally
            {

                IsPageEnabled = false;
            }
        }

        private void LoadPoints()
        {
            if (AppData.IsLoggedIn)
            {
                if (AppData.Device.UserLoggedOnToDevice?.Account != null)
                {
                    MyPoints = $"{AppData.Device.UserLoggedOnToDevice.Account.PointBalance.ToString("N0")} {AppResources.txtpoints}";
                }
            }
        }

        bool IsInitialized = false;
        private void HandleIsActiveTrue(object sender, EventArgs e)
        {
            if (IsActive)
            {

                LoadPoints();

                if (!IsInitialized)
                {
                    IsInitialized = true;
                    LoadData();
                }
                


            }
        }


        internal async Task NavigateToOfferDetailsPage(PublishedOffer publishedOffer)
        {
            IsPageEnabled = true;

            await NavigationService.NavigateAsync(nameof(OfferDetailsPage), new NavigationParameters { { "offer", publishedOffer } });


            IsPageEnabled = false;
        }


        internal async void ScanSend()
        {
            IsPageEnabled = true;
            if (await CheckLogin())
            {

                await NavigationService.NavigateAsync(nameof(ScanSendPage));

                //try
                //{
                //    var request = await App.dialogService.DisplayActionSheetAsync(AppResources.txtUploadPhoto, AppResources.ApplicationCancel, null, AppResources.txtCamera, AppResources.txtGallery);
                //    if (string.IsNullOrEmpty(request) || request.Equals(AppResources.ApplicationCancel) )
                //    {
                //        return;
                //    }
                //    if (request.Equals(AppResources.txtGallery))
                //    {
                //        IsUploadBtnClicked = true;
                //        await ImageHelper.PickFromGallery(5);
                //    }
                //    else
                //    {
                //        if (Device.RuntimePlatform == Device.Android)
                //        {
                //            await NavigationService.NavigateAsync(nameof(CameraPage),null,true,false);
                //        }
                //        else
                //            await TakePickure();
                //    }
                       
                //}
                //catch (Exception)
                //{


                //}
                //finally
                //{
                //    IsPageEnabled = false;
                //}
            }
            else
            {
                IsPageEnabled = false;
            }
           
        }

        internal async Task<bool> OnQtyChanged(LoyItem loyItem,decimal Qty)
        {
            IsPageEnabled = true;
            bool IsSuccess = false;

            var selectedBasket = AppData.Basket.Items.Where(x => x.ItemId == loyItem.Id).FirstOrDefault();
            if (selectedBasket!=null)
            {
                try
                {
                    IsSuccess = await new BasketModel().EditItem(selectedBasket.Id, Qty, null);
                   
                }
                catch (Exception)
                {

                    IsSuccess = false;
                }
                
            }
            IsPageEnabled = false;
            return IsSuccess;
        }


        internal async Task<bool> AddRemoveWishList(LoyItem loyItem)
        {
            IsPageEnabled = true;

            if (await CheckLogin())
            {
                var existingItem = AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId).ItemGetByIds(loyItem.Id, loyItem.SelectedVariant == null ? string.Empty : loyItem.SelectedVariant.Id, loyItem.SelectedUnitOfMeasure == null ? string.Empty : loyItem.SelectedUnitOfMeasure.Id);
                if (existingItem != null)
                {
                    await new ShoppingListModel().DeleteWishListLine(existingItem.Id, true);
                    loyItem.IsWishlisted = false;
                    IsPageEnabled = false;
                    return false;
                }
                else
                {
                    if (await AddToWishList(loyItem))
                    {
                        loyItem.IsWishlisted = true;
                        MessagingCenter.Send((App)Xamarin.Forms.Application.Current, "Wishlistadded");
                        IsPageEnabled = false;
                        return true;
                    }
                    else
                    {
                        IsPageEnabled = false;
                        return false;
                    }
                       

                }
            }
            else
            {
                IsPageEnabled = false;
                return false;
            }
                

            
        }

        internal async Task<bool> AddToWishList(LoyItem loyItem)
        {
            if (await CheckLogin())
            {
                OneListItem wishlist = GlobalMethods.CreateOneListItem(loyItem, 1);

                if (string.IsNullOrEmpty(wishlist.VariantId) && loyItem.VariantsRegistration != null && loyItem.VariantsRegistration.Count > 0)
                {
                    await MaterialDialog.Instance.SnackbarAsync(message: AppResources.ResourceManager.GetString("ItemViewPickVariant", AppResources.Culture),
                                           msDuration: MaterialSnackbar.DurationLong);
                    return false;
                }
                else
                {
                    try
                    {
                        await new ShoppingListModel().AddItemToWishList(wishlist);
                        loyItem.IsWishlisted = true;
                    }
                    catch (Exception)
                    {

                        return false;
                    }

                }

                return true;
            }
            else
                return false;
        }

        internal void ChangeLanguage(string param)
        {
            IsPageEnabled = true;
            AppData.IsLanguageChanged = true;
            Xamarin.Forms.Device.BeginInvokeOnMainThread(async() =>
            {
               
                CultureInfo language;
                if (param.Equals("en", StringComparison.OrdinalIgnoreCase))
                {
                    language = CultureInfo.GetCultures(CultureTypes.NeutralCultures).ToList().First(element => element.EnglishName.Contains("English"));
                    Settings.RTL = false;
                    
                }
                else
                {
                    language = CultureInfo.GetCultures(CultureTypes.NeutralCultures).ToList().First(element => element.EnglishName.Contains("Arabic"));
                    Settings.RTL = true;
                }

                Thread.CurrentThread.CurrentUICulture = language;
                AppResources.Culture = language;

                await NavigationService.NavigateAsync("app:///MainTabbedPage?selectedTab=MainPage");

               
            });

            IsPageEnabled = false;
        }

        internal async Task<bool> AddItemToBasket(LoyItem loyItem)
        {
            IsPageEnabled = true;

            if (await CheckLogin())
            {
                OneListItem basketItem = GlobalMethods.CreateOneListItem(loyItem, 1);

                try
                {
                    var basketModel = new BasketModel();
                    var isSuccess = await basketModel.AddItemToBasket(basketItem);
                    if (isSuccess)
                    {
                        loyItem.Quantity = 1;
                        //DependencyService.Get<INotify>().ShowSnackBar($"{basketItem.ItemDescription} has been added to basket!!");
                        IsPageEnabled = false;
                        return true;
                    }


                }
                catch (Exception)
                {
                    IsPageEnabled = false;
                    return false;

                }
            }
            IsPageEnabled = false;
            return false;
            

        }

        private async Task TakePickure()
        {
           
            var imgData = new List<Tuple<byte[], string>>();
            var file = await ImageHelper.TakePictureAsync();
            if (file != null)
            {
                var extension = Path.GetExtension(file.Path);
                byte[] imgBytes;
                using (var memoryStream = new MemoryStream())
                {
                    file.GetStream().CopyTo(memoryStream);
                    imgBytes =  memoryStream.ToArray();
                }

               var bytes =  DependencyService.Get<IMediaService>().CompressImage(imgBytes);


                //if(imgBytes.Length > 2097152)
                //{
                //    await MaterialDialog.Instance.AlertAsync(AppResources.txtImageSizeExceed, AppResources.txtImageSizeError, AppResources.ApplicationOk);
                //    return;
                //}
                if (bytes != null)
                {
                    imgData.Add(new Tuple<byte[], string>(bytes, extension.Replace(".", "")));
                   
                }
                NavigateToScanPage(imgData);
            }


        }

        #region Search

        private async Task ExecuteSearchCommandAsync()
        {
            if (IsPageEnabled) return;
            IsPageEnabled = true;

            if (!string.IsNullOrEmpty(SearchKey))
            {
                await OnSearchQuery(SearchKey);

            }
            else
               IsSuggestionFound = false;

            IsPageEnabled = false;
        }
       

        public async Task OnSearchQuery(string SearchKey)
        {

            if (SearchKey.Length > 2)
            {
                IsSuggestionFound = true;
                await LoadSearch(SearchKey).ConfigureAwait(false);
            }
            else
            IsSuggestionFound = false;

            lastSearchLength = SearchKey.Length;

           
        }

        private async Task LoadSearch(string SearchKey)
        {
            try
            {
                var model = new ItemModel();
                var items = await model.GetItemsByPage(10, 1, string.Empty, string.Empty, SearchKey, false, string.Empty).ConfigureAwait(false);
                Items = new ObservableCollection<LoyItem>(items);

                //LoadImages();
            }
            catch (Exception)
            {
                IsPageEnabled = false;

            }
        }

        private void LoadImages()
        {
            try
            {
                Task.Run(async () =>
                {
                    foreach (var item in Items)
                    {
                        if (item.Images.Any())
                        {
                            var imgView = await ImageHelper.GetImageById(item.Images[0].Id, new LSRetail.Omni.Domain.DataModel.Base.Retail.ImageSize(110, 110));
                            item.Images[0].Image = imgView.Image;
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


        #endregion

        internal async Task NavigateToAccountTier()
        {
            IsPageEnabled = true;
            await CheckLogin();

            IsPageEnabled = false;
        }
        async Task<bool> CheckLogin()
        {
            
            if (AppData.Device.UserLoggedOnToDevice == null)
            {
                AppData.IsLoggedIn = false;
                await NavigationService.NavigateAsync(nameof(LoginPage));

            }
            return AppData.IsLoggedIn;
        }

        /// <summary>
        /// this method is called when an image is uploaded or clicked.
        /// It navigates to scan page to preview the image.
        /// </summary>
        /// <param name="imgData">It takes bytes and extension</param>
        internal async void NavigateToScanPage(List<Tuple<byte[], string>> imgData)
        {
            IsPageEnabled = true;
            await NavigationService.NavigateAsync(nameof(ScanSendPage), new NavigationParameters { { "images", imgData } });
            IsPageEnabled = false;
        }

       


        internal async Task NavigateToCategory(string obj)
        {
            IsPageEnabled = true;
            if (obj == "Items")
            {
                await NavigationService.NavigateAsync("NavigationPage/ItemCategoriesPage");
            }
            else
                await NavigationService.NavigateAsync(nameof(StoreLocatorPage));

            IsPageEnabled = false;
        }

        internal void NavigateToItemPage(LoyItem loyItem)
        {
            IsPageEnabled = true;
            NavigationService.NavigateAsync(nameof(ItemPage), new NavigationParameters { { "item", loyItem } });
            IsPageEnabled = false;
        }

        /// <summary>
        /// This method is used to load data on page startup
        /// </summary>
        private void LoadData()
        {
            
            
            try
            {

                #region Ads View
                Task.Run(async() =>
                {
                    AdsCurrentState = LayoutState.Loading;
                    if (AppData.Advertisements != null && AppData.Advertisements.Count > 0)
                    {
                        LoadAdvertisements();

                    }
                    else
                    {
                       await LoadAdvertisementsFromServer();
                    }
                    AdsCurrentState = LayoutState.Success;
                });
                #endregion


                #region Category View
                Task.Run(async () =>
                {
                    CategoryCurrentState = LayoutState.Loading;
                    
                        await LoadCategoriesAsync();

                    CategoryCurrentState = LayoutState.Success;
                });
                #endregion

                #region Offer View
                Task.Run(async () =>
                {
                    OfferCurrentState = LayoutState.Loading;

                    await LoadOffersAsync();

                    OfferCurrentState = LayoutState.Success;
                });
                #endregion

                #region Best Seller View
                Task.Run(async () =>
                {
                    BestSellerCurrentState = LayoutState.Loading;

                    await LoadBestSellerItemsAsync();

                    BestSellerCurrentState = LayoutState.Success;
                });
                #endregion


                //var serverTasks = new List<Task>();
                //if (AppData.Advertisements != null && AppData.Advertisements.Count > 0)
                //{
                //    LoadAdvertisements();

                //}
                //else
                //{
                //    var adsTask = LoadAdvertisementsFromServer();
                //    //await LoadAdvertisementsFromServer().ConfigureAwait(false);
                //    serverTasks.Add(adsTask);

                //}

                //if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
                //{
                //    var socialMediaTask = GetSocialMediaStatusForIos();
                //    serverTasks.Add(socialMediaTask);

                //}

                //var categoriesTask = LoadCategoriesAsync();
                //serverTasks.Add(categoriesTask);

                //var bestSellerTask = LoadBestSellerItems();
                //serverTasks.Add(bestSellerTask);

                //var offersTask = LoadOffersAsync();
                //serverTasks.Add(offersTask);

                ////await LoadCategories().ConfigureAwait(false);
                ////await LoadBestSellerItems().ConfigureAwait(false);
                ////await LoadOffersAsync().ConfigureAwait(false);


                //if (!(advertisements is object && advertisements.Any() && itemCategories is object && itemCategories.Any()
                //    && offers is object && offers.Any()))
                //{
                //    await Task.WhenAll(serverTasks).ConfigureAwait(false);
                //}

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                IsPageEnabled = false;
                //await loading.DismissAsync();
            }
            //CurrentState = LayoutState.None;
        }

        private async Task GetSocialMediaStatusForIos()
        {
            try
            {
                 AppData.GetSocialMediaStatusResult = await new CommonModel().GetSocialMediaDisplayStatusAsync();
            }
            catch (Exception ex)
            {

                Crashes.TrackError(ex);
            }
        }

        private async Task LoadOffersAsync()
        {
            if (AppData.PublishedOffers == null || !AppData.PublishedOffers.Any())
            {
                await new OfferModel().GetOffersByCardId(string.Empty).ConfigureAwait(false);
            }
           
            offers = new ObservableCollection<PublishedOffer>(AppData.PublishedOffers.Where(x => x.Code != OfferDiscountType.Coupon).Take(10));
            //LoadOfferImage();  
        }

        private void LoadOfferImage()
        {
            try
            {
                Task.Run(async () =>
                {
                    foreach (var offer in offers)
                    {
                        if (offer.Images.Count > 0)
                        {
                            var imageView = await ImageHelper.GetImageById(offer.Images[0].Id, new ImageSize(396, 396));
                            offer.Images[0].Image = imageView.Image;
                        }
                        else
                            offer.Images = new List<ImageView> { new ImageView { Image = "noimage.png" } };
                    }
                });
            }
            catch (Exception)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                {
                    await MaterialDialog.Instance.SnackbarAsync(message: "Unable to fetch image.",
                                           msDuration: MaterialSnackbar.DurationLong);
                });

            }
        }

      

       

       
        private void LoadMostViewedItems(int retryCounter = 3)
        {
            if (AppData.MostViewed == null || AppData.MostViewed?.Count == 0)
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    Task.Run(async () =>
                {
                    try
                    {

                        var mostViewedItems = await itemModel.GetMostViewedItems(10,true);
                        MostViewedItems = new ObservableCollection<LoyItem>(LoadItemWithPrice(mostViewedItems));
                        if (MostViewedItems.Any())
                        {
                            //LoadItemImage(MostViewedItems);
                            retryCounter = 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                        if (retryCounter == 0)
                        {
                            DependencyService.Get<INotify>().ShowToast(ex.Message);
                        }
                        else
                        {
                            LoadMostViewedItems(--retryCounter);
                        }
                    }
                });
                }
            }
            else
            {
                MostViewedItems = new ObservableCollection<LoyItem>(LoadItemWithPrice( AppData.MostViewed));
                //LoadItemImage(MostViewedItems);
            }
        }

        private async Task LoadBestSellerItemsAsync(int retryCounter =3)
        {
            if (AppData.BestSellers == null || AppData.BestSellers?.Count == 0)
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                   
                        try
                        {

                            var bestSellerItems = await itemModel.GetBestSellerItems(10,true);
                            BestSellerItems = new ObservableCollection<LoyItem>(LoadItemWithPrice(bestSellerItems));
                            if (BestSellerItems.Any())
                            {
                                //LoadItemImage(BestSellerItems);
                                retryCounter = 0;
                            }

                        }
                        catch (Exception ex)
                        {
                            Crashes.TrackError(ex);
                            if (retryCounter == 0)
                            {
                                DependencyService.Get<INotify>().ShowToast(ex.Message);
                            }
                            else
                            {
                               await LoadBestSellerItemsAsync(--retryCounter);
                            }
                        }
                   
                }
            }
            else
            {
                BestSellerItems = new ObservableCollection<LoyItem>(LoadItemWithPrice(AppData.BestSellers));
                //LoadItemImage(BestSellerItems);
            }



               
        }


        private List<LoyItem> LoadItemWithPrice(List<LoyItem> loyItems)
        {
            foreach (var loyItem in loyItems)
            {
                if (loyItem.Prices.Count > 0)
                {
                    loyItem.Price = loyItem.PriceFromVariantsAndUOM(loyItem.SelectedVariant?.Id, loyItem.SelectedUnitOfMeasure?.Id);
                    if (loyItem.Discount > 0)
                    {
                        var discountedPrice = (loyItem.Discount / 100) * Convert.ToDecimal(loyItem.Price);
                        loyItem.NewPrice = (Convert.ToDecimal(loyItem.Price) - discountedPrice).ToString("F", CultureInfo.InvariantCulture);
                    }
                }

                if (AppData.Basket != null)
                {
                    var isExist = AppData.Basket.Items?.Any(x => x.ItemId == loyItem.Id);
                    if (isExist == true)
                    {
                        var basketItem = AppData.Basket.Items.FirstOrDefault(x => x.ItemId == loyItem.Id);
                        loyItem.Quantity = basketItem.Quantity;
                    }
                    else
                        loyItem.Quantity = 0;
                }

                if (AppData.Device.UserLoggedOnToDevice?.GetWishList(AppData.Device.CardId).Items?.Any(x => x.ItemId == loyItem.Id) == true)
                {
                    loyItem.IsWishlisted = true;
                }
                else
                    loyItem.IsWishlisted = false;

            }
            return loyItems;
        }

        private void LoadItemImage(ObservableCollection<LoyItem> loyItems)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    Task.Run(async () =>
                    {
                        foreach (var item in loyItems)
                        {
                           

                            if (item.Images.Count > 0)
                            {
                                if (string.IsNullOrEmpty(item.Images[0].Image))
                                {
                                    var image = await ImageHelper.GetImageById(item.Images[0].Id, new ImageSize(396, 396));
                                    item.Images[0].Image = image.Image;
                                }

                            }
                            else
                                item.Images = new List<ImageView> { new ImageView { Image = "noimage.png" } };

                        }
                    });
                }
                catch (Exception ex)
                {
                    DependencyService.Get<INotify>().ShowToast(ex.Message);
                }
            }
        }

        #region Categories

        /// <summary>
        /// This method is used for loading all categories
        /// </summary>
        private async Task LoadCategoriesAsync(int retryCounter = 3)
        {

            if (AppData.ItemCategories == null || AppData.ItemCategories?.Count == 0)
            {
               
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        try
                        {
                            
                            var cat = await itemModel.GetItemCategories();
                            if (cat?.Count > 0)
                            {
                                itemCategories = new ObservableCollection<ItemCategory>(cat);
                                //loadCatWithImage(itemCategories);

                                retryCounter = 0;
                            }
                            else
                                itemCategories = new ObservableCollection<ItemCategory>();

                        }
                        catch (Exception ex)
                        {
                            Crashes.TrackError(ex);
                            if (retryCounter == 0)
                            {
                                DependencyService.Get<INotify>().ShowToast(ex.Message);

                            }
                            else
                               await LoadCategoriesAsync(--retryCounter);

                        }
                    }
               
            }
            else
            {
                itemCategories = new ObservableCollection<ItemCategory>(AppData.ItemCategories);
                //loadCatWithImage(itemCategories);
            }
                
        }

        internal async void NavigateToItemCategory(ItemCategory itemCategory)
        {
            IsPageEnabled = true;
            await NavigationService.NavigateAsync(nameof(ItemCategoriesPage), new NavigationParameters { { "item", itemCategory }, { "fromPage", true } });
            IsPageEnabled = false;
        }
       

        private void loadCatWithImage(ObservableCollection<ItemCategory> itemCategories)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    Task.Run(async () =>
                    {
                        foreach (var itemCategory in itemCategories)
                        {
                            if (itemCategory.Images.Count > 0)
                            {
                                if (string.IsNullOrEmpty(itemCategory.Images[0].Image))
                                {
                                    var image = await ImageHelper.GetImageById(itemCategory.Images[0].Id, new ImageSize(396, 396));
                                    itemCategory.Images[0].Image = image.Image;
                                }

                            }
                            else
                                itemCategory.Images = new List<ImageView> { new ImageView { Image = "noimage.png" } };

                        }
                    });
                }
                catch (Exception ex)
                {

                    DependencyService.Get<INotify>().ShowToast(ex.Message);
                }
            }
        }

        #endregion

        /// <summary>
        /// This method is use to show ads
        /// </summary>
        public void LoadAdvertisements()
        {
            advertisements = new ObservableCollection<Advertisement>(AppData.Advertisements);
            //LoadAdsImage();
        }

        private async Task LoadAdvertisementsFromServer()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    var service = new SharedService(new SharedRepository());
                    var ads = await service.AdvertisementsGetByIdAsync("LOY", AppData.Device?.UserLoggedOnToDevice?.Id);

                    advertisements = new ObservableCollection<Advertisement>(ads);
                    
                    //LoadAdsImage();

                    AppData.Advertisements = advertisements.ToList();

                }
                catch (Exception ex)
                {

                    DependencyService.Get<INotify>().ShowToast(ex.Message);
                }

            }
        }

        /// <summary>
        /// Get Ads Image
        /// </summary>
        private void LoadAdsImage()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    Task.Run(async () =>
                    {
                        foreach (var ad in advertisements)
                        {
                            if (ad.ImageView != null)
                            {
                                var imgview = await ImageHelper.GetImageById(ad.ImageView.Id, new ImageSize(500, 500));
                                ad.ImageView.Image = imgview.Image;
                            }
                            else
                            {
                                ad.ImageView = new ImageView { Image = "noimage" };
                            }
                           
                        }

                    });
                }
                catch (Exception ex)
                {

                    DependencyService.Get<INotify>().ShowToast(ex.Message);
                }

            }
        }

        public override void ConnectionChanged(bool IsNotConnected)
        {
            base.ConnectionChanged(IsNotConnected);
            if (IsNotConnected)
            {
                DependencyService.Get<INotify>().ShowToast("Oops, looks like you don't have internet connection");
            }
            else
            {
                LoadData();
            }
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
            
            MessagingCenter.Unsubscribe<App, List<Tuple<byte[], string>>>((App)Xamarin.Forms.Application.Current, "ImagesSelected");

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);


            var navigationMode = parameters.GetNavigationMode();
            switch (navigationMode)
            {
                case NavigationMode.Back:
                    var imgData = parameters.GetValue<List<Tuple<byte[], string>>>("images");
                    if(imgData !=null)
                    NavigateToScanPage(imgData);

                    break;
                    //IsBackNavigation = true;
            }
        }
        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

        }
    }
}
