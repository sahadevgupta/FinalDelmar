using FormsLoyalty.ConstantValues;
using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.PopUpView;
using FormsLoyalty.Utils;
using FormsLoyalty.ViewModels;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace FormsLoyalty.Views
{
    public partial class HomeMasterDetailPage : MasterDetailPage
    {
        HomeMasterDetailPageViewModel _viewModel;
       
        public HomeMasterDetailPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as HomeMasterDetailPageViewModel;

            AddToolBar();
        }

        private void AddToolBar()
        {
            if (AppData.Device.UserLoggedOnToDevice != null)
            {
                ToolbarItem basket = new ToolbarItem
                {
                    IconImageSource = "ic_action_basket",
                    Order = ToolbarItemOrder.Primary,
                    Text = "Basket"

                };

                basket.Clicked += Basket_Clicked;

                ToolbarItem account = new ToolbarItem
                {
                    Text = AppResources.ResourceManager.GetString("MenuViewAccountManagementTitle", AppResources.Culture),
                    Order = ToolbarItemOrder.Secondary,
                    Priority = 1
                };
                account.Clicked += Account_Clicked;

                this.ToolbarItems.Add(basket);
                this.ToolbarItems.Add(account);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Subscribe<App>((App)Xamarin.Forms.Application.Current, "LoggedIn", ReloadView);
            MessagingCenter.Subscribe<App>((App)Xamarin.Forms.Application.Current, "Wishlistadded", GetWishlistCount);
            MessagingCenter.Subscribe<App>((App)Xamarin.Forms.Application.Current, "NotificationStatusChanged",GetNotificationCount);
            MessagingCenter.Subscribe<App, string>((App)Xamarin.Forms.Application.Current, "ToolBarClicked", OnToolbarClicked);
        }
       
        private void ReloadView(App obj)
        {
            _viewModel.FillDrawerList();
            AddToolBar();
        }

        private void OnToolbarClicked(App arg1, string arg2)
        {
            if (arg2.ToLower().Contains("contact".ToLower()))
            {
                ContactUs_Clicked(null, null);
            }
            else
                Account_Clicked(null, null);
        }

        private void GetNotificationCount(App obj)
        {
            _viewModel.NotificationCountChanged();
        }

        private void GetWishlistCount(App obj)
        {
            _viewModel.GetWishlistCount();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<App>(this, "Wishlistadded");
        }

        private void Account_Clicked(object sender, System.EventArgs e)
        {
            //Detail.Navigation.PushAsync(new SignUpPage(true));
        }

        private void Basket_Clicked(object sender, System.EventArgs e)
        {
            var popup = new BasketView();
            popup.CheckoutClicked += (s, e1) =>
            {
                Detail.Navigation.PushAsync(new CheckoutPage());
            };
            popup.Disappearing += (data, e2) =>
            {
                if (popup.isPlacOrder)
                {
                    Detail.Navigation.PushAsync(new ItemCategoriesPage());
                }
                
            };

            Navigation.PushPopupAsync(popup);
        }

        private async void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            var grid = (Grid)sender;
            grid.Opacity = 0;
            await grid.FadeTo(1, 250);
            IsPresented = false;
            DrawerSelected((e as TappedEventArgs).Parameter as DrawerMenuItem);
            grid.Opacity = 1;

        }

        async void DrawerSelected(DrawerMenuItem obj)
        {


            switch (obj.ActivityType)
            {
                case AppConstValues.Login:
                    AppData.IsLoggedIn = true;
                    await Detail.Navigation.PushAsync(new LoginPage());
                    break;

                case AppConstValues.Account:
                    Account_Clicked(null, null);
                     break;

                case AppConstValues.Points:
                    AppData.IsLoggedIn = AppData.Device.UserLoggedOnToDevice == null ? false : true;
                    if (AppData.IsLoggedIn)
                    {
                        await Detail.Navigation.PushAsync(new AccountTierPage());
                    }
                    else
                        await Detail.Navigation.PushAsync(new LoginPage());
                    break;

                case AppConstValues.Home:
                    App.Current.MainPage = new HomeMasterDetailPage();
                   
                    break;
                case AppConstValues.Items:
                    await Detail.Navigation.PushAsync(new ItemCategoriesPage());
                    break;
                case AppConstValues.Search:
                    await Detail.Navigation.PushAsync(new SearchPage());
                    break;

                case AppConstValues.Notifications:
                    await Detail.Navigation.PushAsync(new NotificationsPage());
                    break;

                case AppConstValues.Offer:

                    AppData.IsLoggedIn = AppData.Device.UserLoggedOnToDevice == null ? false : true;
                    if (AppData.IsLoggedIn)
                    {
                        await Detail.Navigation.PushAsync(new OffersPage());
                    }
                    else
                        await Detail.Navigation.PushAsync(new LoginPage());

                    break;
                case AppConstValues.Coupons:
                    AppData.IsLoggedIn = AppData.Device.UserLoggedOnToDevice == null ? false : true;
                    if (AppData.IsLoggedIn)
                    {
                        await Detail.Navigation.PushAsync(new CouponsPage());
                    }
                    else
                        await Detail.Navigation.PushAsync(new LoginPage());
                    break;
                case AppConstValues.Magazine:
                        await Detail.Navigation.PushAsync(new MagazinePage());
                    break;
                case AppConstValues.Reminder:
                    await Detail.Navigation.PushAsync(new RemainderPage());
                    break;

                case AppConstValues.ShoppingLists:
                    await Detail.Navigation.PushAsync(new WishListPage());
                    break;
                case AppConstValues.Locations:
                    await Detail.Navigation.PushAsync(new StoreLocatorPage());
                    break;

                case AppConstValues.Transactions:
                    await Detail.Navigation.PushAsync(new TransactionPage());
                    break;

                case AppConstValues.ScanSend:
                    await Detail.Navigation.PushAsync(new ScanSendPage());
                    break;
                case AppConstValues.AppConfiguration:
                    await Detail.Navigation.PushAsync(new AppSettingsPage());
                    break;
                case AppConstValues.Logout:

                    var response = await DisplayAlert("Logout!!", "Are you sure, you want to logout?", "Ok", "Cancel");
                    if (response)
                    {
                        var membercontact = new MemberContactModel();
                        var isSuccess = await membercontact.Logout();
                        if (true)
                        {
                            AppData.IsLoggedIn = false;
                            App.Current.MainPage = new HomeMasterDetailPage();
                        }
                    }
                    break;
            }

           

        }

        private async void BarcodeScan_Tapped(object sender, System.EventArgs e)
        {
            var img = (Image)sender;
            img.Opacity = 0;
            await img.FadeTo(1, 250);

            ScanImage();
            img.Opacity = 1;
        }

        private void ScanImage()
        {

            var options = new ZXing.Mobile.MobileBarcodeScanningOptions();
            options.PossibleFormats = new List<ZXing.BarcodeFormat>() { ZXing.BarcodeFormat.EAN_13, ZXing.BarcodeFormat.EAN_8 };
            ZXingScannerPage scan = new ZXingScannerPage(options)
            {
                DefaultOverlayTopText = AppResources.ResourceManager.GetString("ScannerViewScannerTopText", AppResources.Culture),
                DefaultOverlayBottomText = AppResources.ResourceManager.GetString("ScannerViewScannerBottomText", AppResources.Culture),
                DefaultOverlayShowFlashButton = true
            };
            
            // await Navigation.PushAsync(scan);
            Detail.Navigation.PushModalAsync(scan);
            scan.OnScanResult += (result) =>
            {
                scan.IsScanning = false;
                ZXing.BarcodeFormat barcodeFormat = result.BarcodeFormat;
                string type = barcodeFormat.ToString();
                Device.BeginInvokeOnMainThread(async () =>
                {
                    //Navigation.PopAsync();
                    await Navigation.PopAsync();
                    string barcode = result.Text;
                    _viewModel.NavigateToItemPage(barcode);
                });
            };

        }

        private void ContactUs_Clicked(object sender, EventArgs e)
        {
            Detail.Navigation.PushAsync(new ContactUsPage());
        }
    }
}