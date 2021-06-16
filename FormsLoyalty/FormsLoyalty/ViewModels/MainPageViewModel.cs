using FormsLoyalty.Helpers;
using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Domain.Services.Loyalty.Items;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Setup;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;
using Microsoft.AppCenter.Crashes;
using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Unity;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using XF.Material.Forms;
using XF.Material.Forms.UI.Dialogs;
using DependencyService = Xamarin.Forms.DependencyService;
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


        private int _carouselPostion;
        public int CarouselPosition
        {
            get { return _carouselPostion; }
            set { SetProperty(ref _carouselPostion, value); }
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
                if (!string.IsNullOrEmpty(value))
                {
                    //OnSearchQuery();
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

        public Timer timer { get; set; }

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


        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService)
        {
            Title = AppResources.ResourceManager.GetString("ApplicationTitle",AppResources.Culture);
            AppData.IsLoggedIn = AppData.Device.UserLoggedOnToDevice == null ? false : true;

            App.dialogService = pageDialogService;
            searchModel = new GeneralSearchModel();
            itemModel = new ItemModel();

            LoadPoints();

            IsActiveChanged += HandleIsActiveTrue;

            DependencyService.Get<INotify>().ChangeTabBarFlowDirection(RTL);

        }

        private void LoadPoints()
        {
            if (AppData.IsLoggedIn)
            {
                if (AppData.Device.UserLoggedOnToDevice.Account != null)
                {
                    MyPoints = $"{AppData.Device.UserLoggedOnToDevice.Account.PointBalance.ToString("N0")} pts";
                }
            }
        }

       
        private void HandleIsActiveTrue(object sender, EventArgs e)
        {
            if (IsActive)
            {

                LoadPoints();
                LoadData();
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
                try
                {
                    var request = await App.dialogService.DisplayActionSheetAsync("Upload a new photo", "Cancel", null, "Camera", "Gallery");
                    if (string.IsNullOrEmpty(request) || request == "Cancel" )
                    {
                        return;
                    }
                    if (request == "Gallery")
                    {
                        IsUploadBtnClicked = true;
                        await ImageHelper.PickFromGallery(5);
                    }
                    else
                        await TakePickure();
                }
                catch (Exception)
                {


                }
                finally
                {
                    IsPageEnabled = false;
                }
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
                if (param.Contains("ar", StringComparison.OrdinalIgnoreCase))
                {

                    language = CultureInfo.GetCultures(CultureTypes.NeutralCultures).ToList().First(element => element.EnglishName.Contains("Arabic"));
                    Settings.RTL = true;
                }
                else
                {
                    language = CultureInfo.GetCultures(CultureTypes.NeutralCultures).ToList().First(element => element.EnglishName.Contains("English"));
                    Settings.RTL = false;
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

                //if(imgBytes.Length > 2097152)
                //{
                //    await MaterialDialog.Instance.AlertAsync(AppResources.txtImageSizeExceed, AppResources.txtImageSizeError, AppResources.ApplicationOk);
                //    return;
                //}
                   
                imgData.Add(new Tuple<byte[], string>(imgBytes, extension.Replace(".", "")));
                 NavigateToScanPage(imgData);
            }

           
        }

        #region Search

        public async Task OnSearchQuery(string SearchKey)
        {

            if (SearchKey.Length > 2)
            {
                IsSuggestionFound = true;
                if (SearchKey.Length > lastSearchLength)
                    await LoadSearch(SearchKey);
                else
                    searchModel.ResetSearch();
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
                var items = await model.GetItemsByPage(10, 1, string.Empty, string.Empty, SearchKey, false, string.Empty);
                Items = new ObservableCollection<LoyItem>(items);

                LoadImages();
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
                        var imgView = await ImageHelper.GetImageById(item.Images[0].Id, new LSRetail.Omni.Domain.DataModel.Base.Retail.ImageSize(110, 110));
                        item.Images[0].Image = imgView.Image;
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
            if (await CheckLogin() )
            {
                await NavigationService.NavigateAsync(nameof(AccountTierPage));
            }

            IsPageEnabled = false;
        }
        async Task<bool> CheckLogin()
        {
            
            if (!EnabledItems.ForceLogin && AppData.Device.UserLoggedOnToDevice == null)
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
            IsPageEnabled = true;
            try
            {
                if (AppData.Advertisements != null && AppData.Advertisements.Count > 0)
                {
                    LoadAdvertisements();
                    StartTimer();
                }
                else
                {
                    LoadAdvertisementsFromServer();
                }
              
                LoadCategories();
                LoadBestSellerItems();
                // LoadMostViewedItems();

                LoadOffers();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                IsPageEnabled = false;
                
            }
            IsPageEnabled = false;
        }

        private void LoadOffers()
        {
            if (AppData.Device.UserLoggedOnToDevice !=null)
            {
                var publishedOffers = AppData.Device.UserLoggedOnToDevice.PublishedOffers.Where(x => x.Code != OfferDiscountType.Coupon);
                offers = new ObservableCollection<PublishedOffer>(publishedOffers);
                LoadOfferImage();
            }
          
            
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

        int count = 0;

       

        public void StartTimer()
        {
            //Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(5), (Func<bool>)(() =>
            //{
            //    //var num = count + 10;
            //    //double point = (double)num / 50;
            //    //Progress = point;
            //    //count += 10;
            //    //if (Progress > 1)
            //    //{
            //    //    Progress = count = 0;
            //    //    CarouselPosition = (CarouselPosition + 1) % advertisements.Count;
            //    //}
            //    CarouselPosition = (CarouselPosition + 1) % advertisements.Count;
            //    return true;
            //}));

           timer = new System.Timers.Timer(7000);
            timer.AutoReset = true;
            timer.Elapsed += (s, e) =>
            {
                CarouselPosition = (CarouselPosition + 1) % advertisements.Count;

            };
            timer.Start();

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
                            LoadItemImage(MostViewedItems);
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
                LoadItemImage(MostViewedItems);
            }
        }

        private void LoadBestSellerItems(int retryCounter =3)
        {
            if (AppData.BestSellers == null || AppData.BestSellers?.Count == 0)
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    Task.Run(async () =>
                    {
                        try
                        {

                            var bestSellerItems = await itemModel.GetBestSellerItems(10,true);
                            BestSellerItems = new ObservableCollection<LoyItem>(LoadItemWithPrice(bestSellerItems));
                            if (BestSellerItems.Any())
                            {
                                LoadItemImage(BestSellerItems);
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
                                LoadBestSellerItems(--retryCounter);
                            }
                        }
                    });
                }
            }
            else
            {
                BestSellerItems = new ObservableCollection<LoyItem>(LoadItemWithPrice(AppData.BestSellers));
                LoadItemImage(BestSellerItems);
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
                            if (AppData.Basket!= null )
                            {
                                var isExist = AppData.Basket.Items?.Any(x => x.ItemId == item.Id);
                                if (isExist == true)
                                {
                                    var basketItem = AppData.Basket.Items.FirstOrDefault(x => x.ItemId == item.Id);
                                    item.Quantity = basketItem.Quantity;
                                }
                                else
                                    item.Quantity = 0;
                            }
                         

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
        private void LoadCategories(int retryCounter = 3)
        {

            if (AppData.ItemCategories == null || AppData.ItemCategories?.Count == 0)
            {
                Task.Run(async() =>
                {
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        try
                        {
                            
                            var cat = await itemModel.GetItemCategories();
                            if (cat?.Count > 0)
                            {
                                itemCategories = new ObservableCollection<ItemCategory>(cat);
                                loadCatWithImage(itemCategories);

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
                                LoadCategories(--retryCounter);

                        }
                    }
                });
            }
            else
            {
                itemCategories = new ObservableCollection<ItemCategory>(AppData.ItemCategories);
                loadCatWithImage(itemCategories);
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
        }

        private async void LoadAdvertisementsFromServer()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    var service = new SharedService(new SharedRepository());
                    var ads = await service.AdvertisementsGetByIdAsync("LOY", AppData.Device?.UserLoggedOnToDevice?.Id);

                    advertisements = new ObservableCollection<Advertisement>(ads);
                    StartTimer();
                    LoadAdsImage();

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
                            var imgview = await ImageHelper.GetImageById(ad.ImageView.Id, new ImageSize(500, 500));
                            ad.ImageView.Image = imgview.Image;
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
            timer.Stop();
            MessagingCenter.Unsubscribe<App, List<Tuple<byte[], string>>>((App)Xamarin.Forms.Application.Current, "ImagesSelected");

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
           
            if(timer!=null)
                timer.Start();
        }
        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
        }
    }
}
