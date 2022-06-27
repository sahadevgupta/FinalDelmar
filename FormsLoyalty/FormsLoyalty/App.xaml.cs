using Prism;
using Prism.Ioc;
using FormsLoyalty.ViewModels;
using FormsLoyalty.Views;
using Xamarin.Essentials.Interfaces;
using Xamarin.Essentials.Implementation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LSRetail.Omni.Domain.Services.Loyalty.Items;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Setup;
using System.Threading;
using System.Globalization;
using System.Linq;
using Infrastructure.Data.SQLite.Devices;
using LSRetail.Omni.Domain.Services.Loyalty.MemberContacts;
using Infrastructure.Data.SQLite.MemberContacts;
using LSRetail.Omni.Domain.Services.Loyalty.Devices;
using LSRetail.Omni.Domain.Services.Base.Image;
using Prism.Services;
using System;
using Xamarin.Essentials;
using FormsLoyalty.Interfaces;
using DependencyService = Xamarin.Forms.DependencyService;
using FormsLoyalty.Utils;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup.SpecialCase;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.Domain.Services.Loyalty.Transactions;
using Infrastructure.Data.SQLite.Transactions;
using LSRetail.Omni.Domain.Services.Loyalty.Notifications;
using Infrastructure.Data.SQLite.Notifications;
using Infrastructure.Data.SQLite.Addresses;
using FormsLoyalty.Helpers;
using Plugin.Settings;
using FormsLoyalty.Services;
using FormsLoyalty.Repos;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Collections.Generic;
using Plugin.FirebasePushNotification;
using static LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils;
using FormsLoyalty.PopUpView;
using FormsLoyalty.Models;
using Prism.Navigation;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using System.Net.Http;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace FormsLoyalty
{
    public partial class App
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */

        public static IPageDialogService dialogService;
        public static INavigationService navigationService;
        public static List<string> choices = new List<string> { AppResources.txtSunday, AppResources.txtMonday, AppResources.txtTuesday, AppResources.txtWednesday, AppResources.txtThrusday,
                                                                AppResources.txtFriday, AppResources.txtSaturday };

        public static async Task CallPCLMethod()
        {
            DbHelper.InitializeDatabase();
            var serverTasks = new List<Task>();
            
            //serverTasks.Add(memberConatctTask);
            var offerTask = MemberConatcts.LoadOfferFromServer();
            serverTasks.Add(offerTask);
            var adsTask = MemberConatcts.LoadAdvertisementsFromServer();
            serverTasks.Add(adsTask);
            var categoriesTask = MemberConatcts.LoadCategories();
            serverTasks.Add(categoriesTask);

            var bestSellerItemsTask = MemberConatcts.LoadBestSellerItems();
            serverTasks.Add(bestSellerItemsTask);

            await Task.WhenAll(serverTasks).ConfigureAwait(false);
        }

        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override void OnInitialized()
        {
           // Xamarin.Forms.Device.SetFlags(new string[] { "CarouselView_Experimental", "RadioButton_Experimental", "IndicatorView_Experimental", "Expander_Experimental", "Shapes_Experimental", "SwipeView_Experimental","Brush_Experimental" });
            InitializeComponent();
            navigationService = NavigationService;
            
            XF.Material.Forms.Material.Init(this);
            LoadStyles();
            Init();

            
        }

        private void LoadStyles()
        {
            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                mergedDictionaries.Add(new Styles.colors());
                if(Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
                    mergedDictionaries.Add(new Styles.iosStyle());
            }
        }

        private void Init()
        {
            CultureInfo language;
            if (Settings.RTL)
            {
                language = CultureInfo.GetCultures(CultureTypes.NeutralCultures).ToList().First(element => element.EnglishName.Contains("Arabic"));
            }
            else
                language = CultureInfo.GetCultures(CultureTypes.NeutralCultures).ToList().First(element => element.EnglishName.Contains("English"));

            Thread.CurrentThread.CurrentUICulture = language;
            AppResources.Culture = Thread.CurrentThread.CurrentUICulture;

            #region Image API Call for SSL

            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => true;

            var client = new HttpClient(handler);
            FFImageLoading.ImageService.Instance.Initialize(new FFImageLoading.Config.Configuration
            {
                HttpClient = client
            });

            #endregion

            var deviceId = DependencyService.Get<INotify>().getDeviceUuid();

             LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils.InitWebService( deviceId, AppType.Loyalty, DefaultUrlLoyalty, AppResources.Culture.TwoLetterISOLanguageName);

            if (string.IsNullOrEmpty(AppData.Device.Id) && !(AppData.Device is UnknownDevice))
            {
                var deviceRepo = PrismApplicationBase.Current.Container.Resolve<IDeviceLocalRepository>();
                LSRetail.Omni.Domain.DataModel.Loyalty.Setup.Device deviceData = new LSRetail.Omni.Domain.DataModel.Loyalty.Setup.Device(deviceId);
                FormsLoyalty.Utils.Utils.FillDeviceInfo(deviceData);

                deviceRepo.SaveDevice(deviceData);

                AppData.Device = deviceData;
            }

            
            // await NavigationService.NavigateAsync(nameof(WelcomePage));
            if (AppData.Device is UnknownDevice)
            {
                NavigationService.NavigateAsync(nameof(WelcomePage));
            }
            else
            {
                 Xamarin.Essentials.MainThread.InvokeOnMainThreadAsync(() =>
                {
                    //Current.MainPage = new Page1();
                    NavigationService.NavigateAsync("app:///NavigationPage/MainTabbedPage");
                });
                // Current.MainPage = new TabbedPage1();
                
            }
        }

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            base.OnAppLinkRequestReceived(uri);
        }
        protected override void OnStart()
        {
            base.OnStart();
            AppCenter.Start("ios=e608da8d-5a04-495b-8987-5c86b5b64dfd;" +
                  "android=4e565f82-65bd-4dfa-9a14-4af9f9af03e2;",
                  typeof(Analytics), typeof(Crashes));

            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine($"TOKEN : {p.Token}");
                Settings.FCM_Token = p.Token;
            };

            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {

                System.Diagnostics.Debug.WriteLine("Received");
                foreach (var data in p.Data)
                {
                    System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
                }
                if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
                    DependencyService.Get<INotify>().ShowLocalNotification("titleforeground", "test");

            };
            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine("Opened");
                foreach (var data in p.Data)
                {
                    System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
                }


            };

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
            containerRegistry.RegisterSingleton<ItemService>();
            containerRegistry.RegisterSingleton<LoyItemRepository>();
            containerRegistry.RegisterSingleton<ITransactionLocalRepository,TransactionRepository>();
            containerRegistry.RegisterSingleton<IDeviceLocalRepository, DeviceRepository>();
            containerRegistry.RegisterSingleton<IMemberContactLocalRepository, MemberContactRepository>();
            containerRegistry.RegisterSingleton<INotificationLocalRepository, NotificationRepository>();
            containerRegistry.RegisterSingleton<IAddressRepository, AddressRepository>();
            containerRegistry.RegisterSingleton<IMagazineRepo, MagazineRepo>();
            containerRegistry.RegisterSingleton<IReminderRepo, ReminderRepo>();
            containerRegistry.RegisterSingleton<IScanSendManager, ScanSendManager>();
            containerRegistry.RegisterSingleton<IScanSendRepo, ScanSendRepo>();
            containerRegistry.RegisterSingleton<IGenericDatabaseRepo<PublishedOffer>, GenericDatabaseRepo<PublishedOffer>>();


            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<ItemCategoriesPage, ItemCategoriesPageViewModel>();
            containerRegistry.RegisterForNavigation<WelcomePage, WelcomePageViewModel>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<ForgotPasswordPage, ForgotPasswordPageViewModel>();
            containerRegistry.RegisterForNavigation<ResetPasswordPage, ResetPasswordPageViewModel>();
            containerRegistry.RegisterForNavigation<AppSettingsPage, AppSettingsPageViewModel>();
            containerRegistry.RegisterForNavigation<SignUpPage, SignUpPageViewModel>();
            containerRegistry.RegisterForNavigation<AccountTierPage, AccountTierPageViewModel>();
            containerRegistry.RegisterForNavigation<OffersPage, OffersPageViewModel>();
            containerRegistry.RegisterForNavigation<OfferDetailsPage, OfferDetailsPageViewModel>();
            containerRegistry.RegisterForNavigation<StoreLocatorPage, StoreLocatorPageViewModel>();
            containerRegistry.RegisterForNavigation<MapPage, MapPageViewModel>();
            containerRegistry.RegisterForNavigation<StoreDetailPage, StoreDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<TransactionPage, TransactionPageViewModel>();
            containerRegistry.RegisterForNavigation<ContactUsPage, ContactUsPageViewModel>();
            containerRegistry.RegisterForNavigation<TransactionDetailPage, TransactionDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<SearchPage, SearchPageViewModel>();
            containerRegistry.RegisterForNavigation<NotificationsPage, NotificationsPageViewModel>();
            containerRegistry.RegisterForNavigation<NotificationDetailPage, NotificationDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<WishListPage, WishListPageViewModel>();
            containerRegistry.RegisterForNavigation<ItemGroupPage, ItemGroupPageViewModel>();
            containerRegistry.RegisterForNavigation<ItemPage, ItemPageViewModel>();
            containerRegistry.RegisterForNavigation<ScanSendPage, ScanSendPageViewModel>();

            containerRegistry.RegisterForNavigation<QRCodePage, QRCodePageViewModel>();
            containerRegistry.RegisterForNavigation<AccountProfilePage, AccountProfilePageViewModel>();
            containerRegistry.RegisterForNavigation<CheckoutPage, CheckoutPageViewModel>();
            containerRegistry.RegisterForNavigation<ItemSearchPage, ItemSearchPageViewModel>();
            containerRegistry.RegisterForNavigation<CouponsPage, CouponsPageViewModel>();
            containerRegistry.RegisterForNavigation<ChangePasswordPage, ChangePasswordPageViewModel>();
            containerRegistry.RegisterForNavigation<MagazinePage, MagazinePageViewModel>();
            containerRegistry.RegisterForNavigation<MagazineDetail, MagazineDetailViewModel>();
            containerRegistry.RegisterForNavigation<RemainderPage, RemainderPageViewModel>();
            containerRegistry.RegisterForNavigation<AddReminderPage, AddReminderPageViewModel>();
            containerRegistry.RegisterForNavigation<CouponDetailsPage, CouponDetailsPageViewModel>();
            containerRegistry.RegisterForNavigation<DemonstrationPage, DemonstrationPageViewModel>();
            containerRegistry.RegisterForNavigation<MainTabbedPage, MainTabbedPageViewModel>();
            containerRegistry.RegisterForNavigation<CartPage, CartPageViewModel>();
            containerRegistry.RegisterForNavigation<MoreInfoPage, MoreInfoPageViewModel>();
            containerRegistry.RegisterForNavigation<HelpPage, HelpPageViewModel>();
            containerRegistry.RegisterForNavigation<TermsConditionPage, TermsConditionPageViewModel>();
            containerRegistry.RegisterForNavigation<ImagePreviewPage, ImagePreviewPageViewModel>();
            containerRegistry.RegisterForNavigation<LoginOtpView, LoginOtpViewViewModel>();
            containerRegistry.RegisterForNavigation<CameraPage, CameraPageViewModel>();
            containerRegistry.RegisterForNavigation<SocialMediaLogin, SocialMediaLoginViewModel>();

        }
    }
}
