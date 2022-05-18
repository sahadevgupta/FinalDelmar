using FormsLoyalty.ConstantValues;
using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.Repos;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
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

        private void MoreInfoPageViewModel_IsActiveChanged(object sender, EventArgs e)
        {
            if (IsActive)
            {
                if (AppData.Device.UserLoggedOnToDevice != null)
                {
                    ProfileName = AppData.Device.UserLoggedOnToDevice.Name;
                    MobileNumber = AppData.Device.UserLoggedOnToDevice.MobilePhone;
                }

                FillDrawerList();
                
            }
            
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
        internal async void FillDrawerList()
        {
            drawerMenuItems = new ObservableCollection<DrawerMenuItem>();

            if (!EnabledItems.ForceLogin && AppData.Device.UserLoggedOnToDevice == null)
            {
                drawerMenuItems.Add(new DrawerMenuItem()
                {
                    ActivityType = AppConstValues.Login,
                    IsVisible = true,
                    Image = "user",
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("LoginViewLoginButton", AppResources.Culture)

                });
            }
            //else if (AppData.Device != null && AppData.Device.UserLoggedOnToDevice != null && AppData.Device.UserLoggedOnToDevice.Account != null)
            //{
            //    drawerMenuItems.Add(new DrawerMenuItem()
            //    {
            //        ActivityType = AppConstValues.Account,
            //        IsVisible = true,
            //        Image = "ic_action_person_colored",
            //        IsLoading = false,
            //        Title = AppData.Device.UserLoggedOnToDevice.UserName,
            //        //SubTitle = AppData.Device.UserLoggedOnToDevice.Account.PointBalance.ToString("N0")
            //    });
            //}

            #region Home Tab
            //drawerMenuItems.Add(new DrawerMenuItem()
            //    {
            //        ActivityType = AppConstValues.Home,
            //        IsVisible = true,
            //        Image = "ic_action_home",
            //        IsLoading = false,
            //        Title = AppResources.ResourceManager.GetString("ActionbarHome", AppResources.Culture),
            //    });

            #endregion

            #region Points Tab
            //var pointsTab = new DrawerMenuItem();
            //pointsTab.ActivityType = AppConstValues.Points;
            //pointsTab.IsVisible = true;
            //pointsTab.Image = "points";
            //pointsTab.IsLoading = false;
            //pointsTab.Title = AppResources.ResourceManager.GetString("txtpoints", AppResources.Culture);

            //if (EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null)
            //{

            //    pointsTab.SubTitle = AppData.Device.UserLoggedOnToDevice.Account.PointBalance.ToString("N0");

            //}
            //drawerMenuItems.Add(pointsTab);

            #endregion

            #region Item Tab

            drawerMenuItems.Add(new DrawerMenuItem()
            {
                ActivityType = AppConstValues.Items,
                IsVisible = true,
                Image = "items",
                IsLoading = false,
                Title = AppResources.ResourceManager.GetString("ActionbarItems", AppResources.Culture),
            });

            #endregion

            #region Search tab
            drawerMenuItems.Add(new DrawerMenuItem()
                {
                    ActivityType = AppConstValues.Search,
                    IsVisible = true,
                    Image = "search",
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
                    Image = "notification",
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
                Image = "offers",
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
                    Image = "coupon",
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
                Image = "magazine",
                IsLoading = false,
                Title = AppResources.ResourceManager.GetString("ActionbarMagazines", AppResources.Culture),
            });
            #endregion

            #region Reminder Tab
            drawerMenuItems.Add(new DrawerMenuItem()
            {
                ActivityType = AppConstValues.Reminder,
                IsVisible = true,
                Image = "reminder",
                IsLoading = false,
                Title = AppResources.ResourceManager.GetString("ActionbarReminders", AppResources.Culture),
            });
            #endregion

            #region Wishlist Tab

            if ((EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null) && EnabledItems.HasWishLists)
            {
                var itemCount = string.Empty;
                if (AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId).Items.Count > 0)
                {
                    itemCount = AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId).Items.Count.ToString();
                }

                drawerMenuItems.Add(new SecondaryTextDrawerMenuItem()
                {
                    ActivityType = AppConstValues.ShoppingLists,
                    IsVisible = true,
                    Image = "wishlist",
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("ShoppingListDetailViewWishlist", AppResources.Culture),
                    SubTitle = itemCount
                });
            }
            #endregion


            #region Store Tab
            drawerMenuItems.Add(new DrawerMenuItem()
                {
                    ActivityType = AppConstValues.Locations,
                    IsVisible = true,
                    Image = "store",
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("ActionbarStores", AppResources.Culture),
                });
            #endregion

            #region Order Tab
            //if ((EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null) && EnabledItems.HasHistory)
            //{
            //    drawerMenuItems.Add(new DrawerMenuItem()
            //    {
            //        ActivityType = AppConstValues.Transactions,
            //        IsVisible = true,
            //        Image = "ic_action_history_colored",
            //        IsLoading = false,
            //        Title = AppResources.ResourceManager.GetString("ActionbarTransactions", AppResources.Culture),
            //    });
            //}
            #endregion

            #region ScanSend Tab
            // Scan & Send Option
            if ((EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null))
            {
                drawerMenuItems.Add(new DrawerMenuItem()
                {
                    ActivityType = AppConstValues.ScanSend,
                    IsVisible = true,
                    Image = "icon_camera",
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("ActionbarScanSend", AppResources.Culture),
                });
            }

            #endregion

            #region AppSetting Tab
            //drawerMenuItems.Add(new DrawerMenuItem()
            //{
            //    ActivityType = AppConstValues.AppConfiguration,
            //    IsVisible = true,
            //    Image = "ic_action_settings",
            //    IsLoading = false,
            //    Title = AppResources.ResourceManager.GetString("AppConfigurationappbar", AppResources.Culture),
            //});
            #endregion

            #region LogOut Tab

            if (EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null)
            {
                drawerMenuItems.Add(new DrawerMenuItem()
                {
                    ActivityType = AppConstValues.Logout,
                    IsVisible = true,
                    Image = "logout",
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("ActionbarLogout", AppResources.Culture),

                });
            }
            #endregion
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

                case AppConstValues.Account:
                   // Account_Clicked(null, null);
                    break;

                case AppConstValues.Points:
                    AppData.IsLoggedIn = AppData.Device.UserLoggedOnToDevice == null ? false : true;
                    if (AppData.IsLoggedIn)
                    {
                        await NavigationService.NavigateAsync(nameof(AccountTierPage));
                    }
                    else
                        await NavigationService.NavigateAsync(nameof(LoginPage));
                    break;

                
                case AppConstValues.Items:
                    
                        IsPageEnabled = true;

                    try
                    {
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

                        IsPageEnabled = false;
                    }
                    catch (Exception)
                    {

                        
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
}
