using FormsLoyalty.ConstantValues;
using FormsLoyalty.Helpers;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.Domain.Services.Loyalty.Items;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FormsLoyalty.ViewModels
{
    public class HomeMasterDetailPageViewModel : ViewModelBase
    {
        private ItemService service;

        private ObservableCollection<DrawerMenuItem> _drawerMenuItems;
        public ObservableCollection<DrawerMenuItem> drawerMenuItems
        {
            get { return _drawerMenuItems; }
            set { SetProperty(ref _drawerMenuItems, value); }
        }

        private bool _rtl;
        public bool RTL
        {
            get { return _rtl; }
            set { SetProperty(ref _rtl, value); }
        }

       

        public DelegateCommand<string> OnDrawerSelectedCommand { get; set; }
        IDialogService _dialogService;

        public HomeMasterDetailPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IDialogService dialogService) : base(navigationService)
        {
            RTL = Settings.RTL;
            App.dialogService = pageDialogService;
            _dialogService = dialogService;
            
            FillDrawerList();

        }

       
        /// <summary>
        /// This method is used to generate points.
        /// </summary>
        private void GetPoints()
        {
            AppData.MyPoints = AppData.Device.UserLoggedOnToDevice.Account.PointBalance.ToString("N0");
        }

        /// <summary>
        /// This method is hepls to set the wishlist count.
        /// </summary>
        internal void GetWishlistCount()
        {
            var itemCount = string.Empty;
              itemCount = AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId).Items.Count.ToString();

                var wishlisttab = drawerMenuItems.Where(x => x.ActivityType == (int)AppConstValues.ShoppingLists).FirstOrDefault();
                wishlisttab.SubTitle = itemCount;
            
        }

        /// <summary>
        /// This method is triggered when the notification status is changed from notification page.
        /// </summary>
        internal void NotificationCountChanged()
        {
            var count = GetNotificationCount();
            var notificationtab = drawerMenuItems.Where(x => x.ActivityType == (int)AppConstValues.Notifications).FirstOrDefault();
            notificationtab.SubTitle = count;
        }


        /// <summary>
        /// This method is triggered when the notification status is changed from notification page.
        /// </summary>
        internal void CouponsCountChanged()
        {
            var count = GetCouponsCount();
            var Couponstab = drawerMenuItems.Where(x => x.ActivityType == (int)AppConstValues.Coupons).FirstOrDefault();
            Couponstab.SubTitle = count;
        }

        /// <summary>
        /// This method used to fill the drawer with title and respective icon
        /// </summary>
        internal void FillDrawerList()
        {
            drawerMenuItems = new ObservableCollection<DrawerMenuItem>();

            if (!EnabledItems.ForceLogin && AppData.Device.UserLoggedOnToDevice == null)
            {
                drawerMenuItems.Add(new DrawerMenuItem()
                {
                    ActivityType = AppConstValues.Login,
                    IsVisible = true,
                    Image = "ic_action_person_colored",
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("LoginViewLoginButton", AppResources.Culture)

                });
            }
            else if (AppData.Device != null && AppData.Device.UserLoggedOnToDevice != null && AppData.Device.UserLoggedOnToDevice.Account != null)
            {
                drawerMenuItems.Add(new DrawerMenuItem()
                {
                    ActivityType = AppConstValues.Account,
                    IsVisible = true,
                    Image = "ic_action_person_colored",
                    IsLoading = false,
                    Title = AppData.Device.UserLoggedOnToDevice.UserName,
                    //SubTitle = AppData.Device.UserLoggedOnToDevice.Account.PointBalance.ToString("N0")
                });
            }

           

            


          

            if (EnabledItems.HasHome)
            {
                drawerMenuItems.Add(new DrawerMenuItem()
                {
                    ActivityType = AppConstValues.Home,
                    IsVisible = true,
                    Image = "ic_action_home",
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("ActionbarHome", AppResources.Culture),
                });
            }

            #region Points Tab
            var pointsTab = new DrawerMenuItem();
            pointsTab.ActivityType = AppConstValues.Points;
            pointsTab.IsVisible = true;
            pointsTab.Image = "hand";
            pointsTab.IsLoading = false;
            pointsTab.Title = AppResources.ResourceManager.GetString("txtpoints", AppResources.Culture);

            if (EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null)
            {

                pointsTab.SubTitle = AppData.Device.UserLoggedOnToDevice.Account.PointBalance.ToString("N0");

            }
            drawerMenuItems.Add(pointsTab);

            #endregion

            if (EnabledItems.HasItemCatalog)
            {
               
                    drawerMenuItems.Add(new DrawerMenuItem()
                    {
                        ActivityType = AppConstValues.Items,
                        IsVisible = true,
                        Image = "ic_action_items_colored",
                        IsLoading = false,
                        Title = AppResources.ResourceManager.GetString("ActionbarItems", AppResources.Culture),
                    });
                
            }

            if (EnabledItems.HasSearch)
            {
                drawerMenuItems.Add(new DrawerMenuItem()
                {
                    ActivityType = AppConstValues.Search,
                    IsVisible = true,
                    Image = "ic_search_24dp",
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("ActionbarSearch", AppResources.Culture),
                });
            }

            if ((EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null) && EnabledItems.HasNotifications)
            {
                string notificationCount = GetNotificationCount();

                drawerMenuItems.Add(new SecondaryTextDrawerMenuItem()
                {
                    ActivityType = AppConstValues.Notifications,
                    IsVisible = true,
                    Image = "ic_action_notifications_colored",
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("ActionbarNotifications", AppResources.Culture),
                    SubTitle = notificationCount
                });
            }

            if (EnabledItems.HasOffers)
            {
                var offerCount = string.Empty;

                if (EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null)
                {
                    if (AppData.Device.UserLoggedOnToDevice.PublishedOffers.Count(x => x.Code != OfferDiscountType.Coupon) > 0)
                    {
                        offerCount = AppData.Device.UserLoggedOnToDevice.PublishedOffers.Count(x => x.Code != OfferDiscountType.Coupon).ToString();
                    }
                }

                drawerMenuItems.Add(new SecondaryTextDrawerMenuItem()
                {
                    ActivityType = AppConstValues.Offer,
                    IsVisible = true,
                    Image = "ic_action_offers_colored",
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("ActionbarOffers", AppResources.Culture),
                    SubTitle = offerCount
                });
            }

            if (EnabledItems.HasCoupons)
            {
                string couponCount = GetCouponsCount();

                drawerMenuItems.Add(new SecondaryTextDrawerMenuItem()
                {
                    ActivityType = AppConstValues.Coupons,
                    IsVisible = true,
                    Image = "ic_action_coupons_colored",
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("ActionbarCoupons", AppResources.Culture),
                    SubTitle = couponCount
                });
            }

            drawerMenuItems.Add(new DrawerMenuItem()
            {
                ActivityType = AppConstValues.Magazine,
                IsVisible = true,
                Image = "ic_action_magazine",
                IsLoading = false,
                Title = AppResources.ResourceManager.GetString("ActionbarMagazines", AppResources.Culture),
            });
            drawerMenuItems.Add(new DrawerMenuItem()
            {
                ActivityType = AppConstValues.Reminder,
                IsVisible = true,
                Image = "ic_aciton_reminder",
                IsLoading = false,
                Title = AppResources.ResourceManager.GetString("ActionbarReminders", AppResources.Culture),
            });
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
                    Image = "ic_action_shoplists_colored",
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("ShoppingListDetailViewWishlist", AppResources.Culture),
                    SubTitle = itemCount
                });
            }

            if (EnabledItems.HasStoreLocator)
            {
                drawerMenuItems.Add(new DrawerMenuItem()
                {
                    ActivityType = AppConstValues.Locations,
                    IsVisible = true,
                    Image = "ic_action_map",
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("ActionbarStores", AppResources.Culture),
                });
            }

            if ((EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null) && EnabledItems.HasHistory)
            {
                drawerMenuItems.Add(new DrawerMenuItem()
                {
                    ActivityType = AppConstValues.Transactions,
                    IsVisible = true,
                    Image = "ic_action_history_colored",
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("ActionbarTransactions", AppResources.Culture),
                });
            }

            // Scan & Send Option
            if ((EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null))
            {
                drawerMenuItems.Add(new DrawerMenuItem()
                {
                    ActivityType = AppConstValues.ScanSend,
                    IsVisible = true,
                    Image = "camera",
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("ActionbarScanSend", AppResources.Culture),
                });
            }

            drawerMenuItems.Add(new DrawerMenuItem()
            {
                ActivityType = AppConstValues.AppConfiguration,
                IsVisible = true,
                Image = "ic_action_settings",
                IsLoading = false,
                Title = AppResources.ResourceManager.GetString("AppConfigurationappbar", AppResources.Culture),
            });

            if (EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null)
            {
                drawerMenuItems.Add(new DrawerMenuItem()
                {
                    ActivityType = AppConstValues.Logout,
                    IsVisible = true,
                    Image = "ic_action_logout_colored",
                    IsLoading = false,
                    Title = AppResources.ResourceManager.GetString("ActionbarLogout", AppResources.Culture),

                });
            }
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
        /// This method is triggered whenever code is scanned through bar code scanner.
        /// Navigates to item page with barcode string as parameter. 
        /// </summary>
        /// <param name="barcode"></param>
        internal async void NavigateToItemPage(string barcode)
        {

            await NavigationService.NavigateAsync(nameof(ItemPage), new NavigationParameters { { "barcode", barcode } });
        }

        /// <summary>
        /// Navigate to Account Page
        /// </summary>
        internal async void NavigateToAccountManagement()
        {
            await NavigationService.NavigateAsync(nameof(SignUpPage), new NavigationParameters { { "edit", true } });
        }

        //internal async void DrawerSelected(DrawerMenuItem obj)
        //{


        //    switch (obj.ActivityType)
        //    {
        //        case AppConstValues.Login:
        //            AppData.IsLoggedIn = true;
        //           await NavigationService.NavigateAsync("NavigationPage/LoginPage");
        //            break;

        //        case AppConstValues.Account:

        //           await NavigationService.NavigateAsync("HomeMasterDetailPage/NavigationPage/AccountTierPage",null,false);

        //           // await NavigationService.NavigateAsync("AccountTierPage");
        //            break;
        //        case AppConstValues.Home:
        //            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        //            break;
        //        case AppConstValues.Items:
        //            await NavigationService.NavigateAsync("NavigationPage/ItemCategoriesPage");
        //            break;

        //        case AppConstValues.Search:

        //            await NavigationService.NavigateAsync("NavigationPage/SearchPage");
        //            break;

        //        case AppConstValues.Notifications:
        //            await NavigationService.NavigateAsync("NavigationPage/NotificationsPage",null,null,true);
        //            break;

        //        case AppConstValues.Offer:

        //            AppData.IsLoggedIn = AppData.Device.UserLoggedOnToDevice == null ? false : true;
        //            if (AppData.IsLoggedIn)
        //            {
        //                await NavigationService.NavigateAsync("NavigationPage/OffersPage");
        //            }
        //            else
        //                await NavigationService.NavigateAsync("NavigationPage/LoginPage");

        //            break;
        //        case AppConstValues.Coupons:
        //            AppData.IsLoggedIn = AppData.Device.UserLoggedOnToDevice == null ? false : true;
        //            if (AppData.IsLoggedIn)
        //            {
        //                await NavigationService.NavigateAsync("NavigationPage/CouponsPage");
        //            }
        //            else
        //                await NavigationService.NavigateAsync("NavigationPage/LoginPage");
        //            break;
        //        case AppConstValues.ShoppingLists:
        //            await NavigationService.NavigateAsync("NavigationPage/WishListPage");
        //            break;
        //        case AppConstValues.Locations:
        //            await NavigationService.NavigateAsync("NavigationPage/StoreLocatorPage");
        //            break;

        //        case AppConstValues.Transactions:
        //            await NavigationService.NavigateAsync("NavigationPage/TransactionPage");
        //            break;

        //        case AppConstValues.ScanSend:
        //            await NavigationService.NavigateAsync("NavigationPage/ScanSendPage");
        //            break;
        //        case AppConstValues.AppConfiguration:
        //            await NavigationService.NavigateAsync("NavigationPage/AppSettingsPage");
        //            break;
        //        case AppConstValues.Logout:

        //            var membercontact = new MemberContactModel();
        //            await membercontact.Logout();

        //            if (EnabledItems.ForceLogin)
        //            {
        //                await NavigationService.NavigateAsync("NavigationPage/LoginPage");
        //            }
        //            else
        //            {
        //                await NavigationService.NavigateAsync("app:///HomeMasterDetailPage");
        //            }
        //            break;
        //    }


        //}

    }
}
