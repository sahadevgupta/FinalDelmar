using FormsLoyalty.ConstantValues;
using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.Repos;
using FormsLoyalty.Resources;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using Microsoft.AppCenter.Crashes;
using Plugin.StoreReview;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Unity;
using Xamarin.Essentials;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;
using ZXing.Net.Mobile.Forms;

namespace FormsLoyalty.ViewModels
{
    public class MoreInfoPageViewModel : MainTabbedPageViewModel
    {

        private ObservableCollection<DrawerMenuItem> _drawerMenuItems;
        public ObservableCollection<DrawerMenuItem> drawerMenuItems
        {
            get { return _drawerMenuItems; }
            set { SetProperty(ref _drawerMenuItems, value); }
        }

        private ObservableCollection<MenuGroup> _menuItems;
        public ObservableCollection<MenuGroup> MenuItems
        {
            get { return _menuItems; }
            set { SetProperty(ref _menuItems, value); }
        }

        private string _profileName;
        public string ProfileName
        {
            get { return _profileName; }
            set { SetProperty(ref _profileName, value); }
        }

        private string _mobileNo;
        public string MobileNumber
        {
            get { return _mobileNo; }
            set { SetProperty(ref _mobileNo, value); }
        }
        public INavigation navigation;
        public DelegateCommand RateAppCommand { get; set; }

        IGenericDatabaseRepo<PublishedOffer> _offerRepo;
        public MoreInfoPageViewModel(INavigationService navigationService, IGenericDatabaseRepo<PublishedOffer> offerRepo) : base(navigationService)
        {
            _offerRepo = offerRepo;
            IsActiveChanged += MoreInfoPageViewModel_IsActiveChanged;

            RateAppCommand = new DelegateCommand(RateApp);
        }
        private void MoreInfoPageViewModel_IsActiveChanged(object sender, EventArgs e)
        {
            if (IsActive)
            {
                if (AppData.Device.UserLoggedOnToDevice != null)
                {
                    ProfileName = AppData.Device.UserLoggedOnToDevice.Name;
                    MobileNumber = AppData.Device.UserLoggedOnToDevice.MobilePhone;
                }

               LoadData();

            }

        }

        private async void LoadData()
        {
            try
            {
                IsPageEnabled = true;
                var menuoptionItems = await GetMenuDrawerList();
                var temp = new List<MenuGroup>();
                temp.Add(new MenuGroup(AppResources.txtMenu, menuoptionItems));
                temp.Add(new MenuGroup(AppResources.txtSettings, GetSettingsDrawerList()));

                MenuItems = new ObservableCollection<MenuGroup>(temp);
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

        private void RateApp()
        {
            try
            {

                Xamarin.Forms.Device.BeginInvokeOnMainThread( () =>
                {
                     DependencyService.Get<IAppRating>().RateAppFromStore();
                    //await CrossStoreReview.Current.RequestReview(true);
                });



            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }

        internal async Task NavigateToProfile()
        {
            IsPageEnabled = true;
            if (AppData.Device.UserLoggedOnToDevice != null)
            {
                await NavigationService.NavigateAsync(nameof(SignUpPage), new NavigationParameters { { "edit", true } });
            }
            else
            {
               var response =  await App.dialogService.DisplayAlertAsync(null, AppResources.txtLoginFirst, AppResources.ActionbarLogin, AppResources.ApplicationCancel);
                if (response)
                {
                    await NavigationService.NavigateAsync(nameof(LoginPage));
                }
            }
                 

            IsPageEnabled = false;
        }

        

        internal async Task OnSettingSelected(string obj)
        {
            if (obj.ToLower().Contains("Help".ToLower()))
            {
               await NavigationService.NavigateAsync(nameof(DemonstrationPage),new NavigationParameters { {"FromHelp",true } });
            }
            else if(obj.ToLower().Contains("Terms".ToLower()))
                await NavigationService.NavigateAsync(nameof(TermsConditionPage));
            else
               await NavigationService.NavigateAsync(nameof(AppSettingsPage));
        }

        /// <summary>
        /// This method used to fill the drawer with title and respective icon
        /// </summary>
        internal async Task<List<DrawerMenuItem>> GetMenuDrawerList()
        {
            var drawerMenuItems = new List<DrawerMenuItem>();

            if (!EnabledItems.ForceLogin && AppData.Device.UserLoggedOnToDevice == null)
            {
                drawerMenuItems.Add(new DrawerMenuItem()
                {
                    ActivityType = AppConstValues.Login,
                    IsVisible = true,
                    Image = FontAwesomeIcons.User,
                    IsFontImage = true,
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("LoginViewLoginButton", AppResources.Culture)

                });
            }
            

            #region Item Tab

            drawerMenuItems.Add(new DrawerMenuItem()
            {
                ActivityType = AppConstValues.Items,
                IsVisible = true,
                Image = FontAwesomeIcons.ShoppingBag,
                IsFontImage = true,
                IsLoading = false,
                Title = AppResources.ResourceManager.GetString("ActionbarItems", AppResources.Culture),
            });

            #endregion

            #region Search tab
            drawerMenuItems.Add(new DrawerMenuItem()
                {
                    ActivityType = AppConstValues.Search,
                    IsVisible = true,
                    Image = FontAwesomeIcons.Search,
                    IsFontImage = true,
                    IsLoading = false,
                    Title = AppResources.txtAdvancedSearch,
                });
            #endregion

            #region Notification Tab

            if ((EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null) && EnabledItems.HasNotifications)
            {
                string notificationCount = GetNotificationCount();

                drawerMenuItems.Add(new SecondaryTextDrawerMenuItem()
                {
                    ActivityType = AppConstValues.Notifications,
                    IsVisible = true,
                    Image = FontAwesomeIcons.Bell,
                    IsFontImage = true,
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("ActionbarNotifications", AppResources.Culture),
                    SubTitle = notificationCount
                });
            }

            #endregion

            #region Offer Tab

             var offerCount = string.Empty;

            if (AppData.PublishedOffers is object)
            {
                var data = await _offerRepo.GetItemsAsync();
                
                if (data.Count(x => x.Code != OfferDiscountType.Coupon) > 0)
                {
                    offerCount = data.Count(x => x.Code != OfferDiscountType.Coupon && !x.IsViewed).ToString();
                }  
            }

            drawerMenuItems.Add(new SecondaryTextDrawerMenuItem()
            {
                ActivityType = AppConstValues.Offer,
                IsVisible = true,
                Image = FontAwesomeIcons.Star,
                IsFontImage = true,
                IsLoading = false,
                Title = AppResources.ResourceManager.GetString("ActionbarOffers", AppResources.Culture),
                SubTitle = offerCount
            });

            #endregion

            #region Coupons Tab

            if (EnabledItems.HasCoupons)
            {
                string couponCount = GetCouponsCount();

                drawerMenuItems.Add(new SecondaryTextDrawerMenuItem()
                {
                    ActivityType = AppConstValues.Coupons,
                    IsVisible = true,
                    Image = FontAwesomeIcons.Gift,
                    IsFontImage = true,
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("ActionbarCoupons", AppResources.Culture),
                    SubTitle = couponCount
                });
            }
            #endregion

            #region Magazine Tab
            drawerMenuItems.Add(new DrawerMenuItem()
            {
                ActivityType = AppConstValues.Magazine,
                IsVisible = true,
                Image = FontAwesomeIcons.BookReader,
                IsFontImage = true,
                IsLoading = false,
                Title = AppResources.ResourceManager.GetString("ActionbarMagazines", AppResources.Culture),
            });
            #endregion

            #region Reminder Tab
            drawerMenuItems.Add(new DrawerMenuItem()
            {
                ActivityType = AppConstValues.Reminder,
                IsVisible = true,
                Image = FontAwesomeIcons.Clock,
                IsFontImage = true,
                IsLoading = false,
                Title = AppResources.ResourceManager.GetString("ActionbarReminders", AppResources.Culture),
            });
            #endregion

            #region Wishlist Tab

            if ((EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null) && EnabledItems.HasWishLists)
            {
                //var itemCount = string.Empty;
                //var oneListItem = await new ShoppingListModel().GetOneListItemsByCardId(AppData.Device.CardId, LSRetail.Omni.Domain.DataModel.Loyalty.Baskets.ListType.Wish);
                //if (oneListItem?.Items.Count > 0)
                //{
                //    itemCount = oneListItem.Items.Count.ToString();
                //}

                drawerMenuItems.Add(new SecondaryTextDrawerMenuItem()
                {
                    ActivityType = AppConstValues.ShoppingLists,
                    IsVisible = true,
                    Image = FontAwesomeIcons.Bookmark,
                    IsFontImage = true,
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("ShoppingListDetailViewWishlist", AppResources.Culture)
                    //SubTitle = itemCount
                });
            }
            #endregion


            #region Store Tab
            drawerMenuItems.Add(new DrawerMenuItem()
                {
                    ActivityType = AppConstValues.Locations,
                    IsVisible = true,
                    Image = FontAwesomeIcons.Store,
                    IsFontImage = true,
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("ActionbarStores", AppResources.Culture),
                });
            #endregion

            

            #region ScanSend Tab
            // Scan & Send Option
            if ((EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null))
            {
                drawerMenuItems.Add(new DrawerMenuItem()
                {
                    ActivityType = AppConstValues.ScanSend,
                    IsVisible = true,
                    Image = FontAwesomeIcons.Camera,
                    IsFontImage = true,
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("ActionbarScanSend", AppResources.Culture),
                });
            }

            #endregion

            

            #region LogOut Tab

            if (EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null)
            {
                drawerMenuItems.Add(new DrawerMenuItem()
                {
                    ActivityType = AppConstValues.Logout,
                    IsVisible = true,
                    Image = FontAwesomeIcons.PowerOff,
                    IsFontImage = true,
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("ActionbarLogout", AppResources.Culture),

                });
            }
            #endregion

            return drawerMenuItems;
        }

        private List<DrawerMenuItem> GetSettingsDrawerList()
        {
            var drawerMenuItems = new List<DrawerMenuItem>();
            drawerMenuItems.Add(new DrawerMenuItem()
            {
                ActivityType = AppConstValues.ContactUs,
                IsVisible = true,
                Image = "IconContactUs",
                IsLoading = false,
                Title = AppResources.ActionbarContactUs
            });

            drawerMenuItems.Add(new DrawerMenuItem()
            {
                ActivityType = AppConstValues.RateApp,
                IsVisible = true,
                Image = "iconRate",
                IsLoading = false,
                Title = AppResources.txtRateApp
            });

            drawerMenuItems.Add(new DrawerMenuItem()
            {
                ActivityType = AppConstValues.TermsAndConditions,
                IsVisible = true,
                Image = FontAwesomeIcons.WindowRestore,
                IsFontImage = true,
                IsLoading = false,
                Title = AppResources.ActionbarTerm
            });

            drawerMenuItems.Add(new DrawerMenuItem()
            {
                ActivityType = AppConstValues.Settings,
                IsVisible = true,
                Image = FontAwesomeIcons.Cog,
                IsFontImage = true,
                IsLoading = false,
                Title = AppResources.AppConfigurationappbar
            });

            drawerMenuItems.Add(new DrawerMenuItem()
            {
                ActivityType = AppConstValues.Help,
                IsVisible = true,
                Image = FontAwesomeIcons.QuestionCircle,
                IsFontImage = true,
                IsLoading = false,
                Title = AppResources.MenuViewHelp
            });
            return drawerMenuItems;
        }

        /// <summary>
        /// Get Coupons count to display on the screen
        /// </summary>
        /// <returns></returns>
        private static string GetCouponsCount()
        {
            var couponCount = string.Empty;

            if (EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null)
            {
                if (AppData.Device.UserLoggedOnToDevice.PublishedOffers.Count(x => x.Code == OfferDiscountType.Coupon) > 0)
                {
                    couponCount = AppData.Device.UserLoggedOnToDevice.PublishedOffers.Count(x => x.Code == OfferDiscountType.Coupon).ToString();
                }
            }

            return couponCount;
        }

        /// <summary>
        /// This method is triggered whenever code is scanned through bar code scanner.
        /// Navigates to item page with barcode string as parameter. 
        /// </summary>
        /// <param name="barcode"></param>
        internal async void NavigateToItemPage(string barcode)
        {

            await NavigationService.NavigateAsync(nameof(ItemPage), new NavigationParameters { { "barcode", barcode } });
        }


        /// <summary>
        /// Get Notification count to display on the screen
        /// </summary>
        /// <returns></returns>
        private static string GetNotificationCount()
        {
            var notificationCount = string.Empty;

            var unreadNotifications = AppData.Device.UserLoggedOnToDevice.Notifications.Count(x => x.Status == NotificationStatus.New);
            if (unreadNotifications > 0)
            {

                notificationCount = unreadNotifications.ToString();
            }

            return notificationCount;
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

                var item = await new ItemModel().GetItemByBarcode(barcode);
                if (item != null)
                {
                    await NavigationService.NavigateAsync(nameof(ItemPage), new NavigationParameters { { "item", item } });
                }  
                else
                {
                    DependencyService.Get<INotify>().ShowToast($"{AppResources.ResourceManager.GetString("ItemModelItemNotFound", AppResources.Culture)} with barcode {barcode}");
                }
            }
            catch (Exception)
            {
                IsPageEnabled = false;

            }
            IsPageEnabled = false;
        }

        internal async void DrawerSelected(DrawerMenuItem obj)
        {
            switch (obj.ActivityType)
            {
                case AppConstValues.Login:
                    AppData.IsLoggedIn = true;
                    await NavigationService.NavigateAsync(nameof(LoginPage));
                    break;

                case AppConstValues.Items:
                    
                    try
                    {
                        IsPageEnabled = true;
                        var options = new ZXing.Mobile.MobileBarcodeScanningOptions();
                        options.PossibleFormats = new List<ZXing.BarcodeFormat>() { ZXing.BarcodeFormat.EAN_13, ZXing.BarcodeFormat.EAN_8 };

                        var overlay = new ZXingDefaultOverlay
                        {
                            ShowFlashButton = false,
                            TopText = AppResources.ScannerViewScannerTopText,
                            BottomText = AppResources.ScannerViewScannerBottomText,

                        };
                        overlay.BindingContext = overlay;


                        ZXingScannerPage scan = new ZXingScannerPage(options, overlay);
                        scan.DefaultOverlayTopText = "Title";
                        scan.DefaultOverlayBottomText = "TEXT";
                        scan.AutoFocus();
                        scan.DefaultOverlayShowFlashButton = true;
                        scan.HasTorch = true;
                        scan.Title = "SCAN";


                        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                        if (status != PermissionStatus.Granted)
                        {
                            status = await Permissions.RequestAsync<Permissions.Camera>();
                        }
                        if (status == PermissionStatus.Granted)
                        {
                            var storageStatus = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                            if (storageStatus != PermissionStatus.Granted)
                            {
                                storageStatus = await Permissions.RequestAsync<Permissions.Camera>();
                            }
                        }
                        if (status != PermissionStatus.Granted)
                        {
                            await App.dialogService.DisplayAlertAsync("Error!!", "Camera Permission not granted, please go to settings to enable it", "OK");
                            return;
                        }

                        await navigation.PushAsync(scan);
                        //Navigation.PushAsync(scan);
                        scan.OnScanResult += (result) =>
                        {
                            scan.IsScanning = false;
                            ZXing.BarcodeFormat barcodeFormat = result.BarcodeFormat;
                            string type = barcodeFormat.ToString();


                           Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                            {
                                //DependencyService.Get<INotify>().ShowToast($"Scan Successful!!, Code : {result.Text}");
                                //Navigation.PopAsync();
                                navigation.PopAsync();
                                string barcode = result.Text;
                               
                                GetItemByBarCode(barcode);
                            });
                        };

                        
                    }
                    catch (Exception)
                    {

                        
                    }
                    finally
                    {
                        IsPageEnabled = false;
                    }
                    break;
                case AppConstValues.Search:
                    await NavigationService.NavigateAsync(nameof(SearchPage));
                    break;

                case AppConstValues.Notifications:
                    await NavigationService.NavigateAsync(nameof(NotificationsPage));
                    break;

                case AppConstValues.Offer:

                  
                        await NavigationService.NavigateAsync(nameof(OffersPage));
                   

                    break;
                case AppConstValues.Coupons:
                    AppData.IsLoggedIn = AppData.Device.UserLoggedOnToDevice == null ? false : true;
                    if (AppData.IsLoggedIn)
                    {
                        await NavigationService.NavigateAsync(nameof(CouponsPage));
                    }
                    else
                        await NavigationService.NavigateAsync(nameof(LoginPage));
                    break;
                case AppConstValues.Magazine:
                    await NavigationService.NavigateAsync(nameof(MagazinePage));
                    break;
                case AppConstValues.Reminder:
                    await NavigationService.NavigateAsync(nameof(RemainderPage));
                    break;

                case AppConstValues.ShoppingLists:
                    await NavigationService.NavigateAsync(nameof(WishListPage));
                    break;
                case AppConstValues.Locations:
                    await NavigationService.NavigateAsync(nameof(StoreLocatorPage));
                    break;

                //case AppConstValues.Transactions:
                //    await Detail.Navigation.PushAsync(new TransactionPage());
                //    break;

                case AppConstValues.ScanSend:
                    await NavigationService.NavigateAsync(nameof(ScanSendPage));
                    break;
                //case AppConstValues.AppConfiguration:
                //    await Detail.Navigation.PushAsync(new AppSettingsPage());
                //    break;

                case AppConstValues.ContactUs:
                    await NavigationService.NavigateAsync(nameof(ContactUsPage), useModalNavigation: true, animated:false);
                    break;
                case AppConstValues.RateApp:
                      RateApp();
                    break;
                case AppConstValues.TermsAndConditions:
                    await NavigationService.NavigateAsync(nameof(TermsConditionPage),animated:false);
                    break;
                case AppConstValues.Settings:
                    await NavigationService.NavigateAsync(nameof(AppSettingsPage), animated: false);
                    break;
                case AppConstValues.Help:
                    await NavigationService.NavigateAsync(nameof(DemonstrationPage), new NavigationParameters { { "FromHelp", true } },animated:false);
                    break;

                case AppConstValues.Logout:

                    await LogOut();
                    break;
            }



        }

        private async Task LogOut()
        {
            try
            {
                var response = await App.dialogService.DisplayAlertAsync(AppResources.ActionbarLogout, AppResources.logoutText, AppResources.ApplicationOk, AppResources.ApplicationCancel);
                if (response)
                {
                    IsPageEnabled = true;
                    var membercontact = new MemberContactModel();
                    var isSuccess = await membercontact.Logout();
                    if (isSuccess)
                    {
                        await NavigationService.NavigateAsync("app:///MainTabbedPage?selectedTab=MainPage");
                    }

                }
            }
            catch (Exception)
            {

                await App.dialogService.DisplayAlertAsync(AppResources.ActionbarLogout, "Unable to Logout", AppResources.ApplicationOk);
            }
            IsPageEnabled = false;
            
        }
    }

    public class MenuGroup : List<DrawerMenuItem>
    {
        public string Name { get; private set; }

        public MenuGroup(string name, List<DrawerMenuItem> menuItems) : base(menuItems)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
