using FormsLoyalty.Helpers;
using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;
using Xamarin.Essentials;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace FormsLoyalty.ViewModels
{
    public class MainTabbedPageViewModel : ViewModelBase, IActiveAware, IMyTabbedPageSelectedTab
    {

        // NOTE: Prism.Forms only sets IsActive, and does not do anything with the event.
        public event EventHandler IsActiveChanged;

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value, RaiseIsActiveChanged); }
        }

        private int _selectedTab;
        /// <summary>
        /// Binds to the View's property
        /// View-ViewModel communcation
        /// </summary>
        public int SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                SetProperty(ref _selectedTab, value);
               // Title = $"My Tabbed Page - Tab [{SelectedTab + 1}]";
            }
        }

        private bool _rtl;
        public bool RTL
        {
            get { return _rtl; }
            set { SetProperty(ref _rtl, value); }
        }

        private string _badgeCount;
        public string BadgeCount
        {
            get { return _badgeCount; }
            set { SetProperty(ref _badgeCount, value); }
        }

        public static bool _isInitialized = false;
        public Page currentTab;
       
        public MainTabbedPageViewModel(INavigationService navigationService) : base(navigationService)
        {

            LoadData();
        }

        void LoadData()
        {
            RTL = Settings.RTL;

            RefreshMemberContact();
            CheckCartCount(null);
            MessagingCenter.Subscribe<BasketModel>(this, "CartUpdated", CheckCartCount);
            MessagingCenter.Subscribe<App>((App)Xamarin.Forms.Application.Current, "LoggedIn", ReloadView);


            
            DependencyService.Get<INotify>().ChangeTabBarFlowDirection(RTL);
            

        }

        private void ReloadView(App obj)
        {
            RefreshMemberContact();
        }

        internal async void CheckCartCount(BasketModel obj)
        {
            if ((AppData.Basket is null || !AppData.Basket.Items.Any()) &&  AppData.Device is object && !string.IsNullOrEmpty(AppData.Device.CardId))
            {
                var items = await new ShoppingListModel().GetOneListItemsByCardId(AppData.Device?.CardId, ListType.Basket);
                if (items is object)
                {
                    BadgeCount = items.Items.Count().ToString();
                }
                else
                {
                    BadgeCount = null;
                }
                
            }
            else
            {
                if (AppData.Basket is object)
                {
                    BadgeCount = AppData.Basket?.Items.Count == 0 ? null : AppData.Basket.Items.Count.ToString();
                }
                else
                {
                    BadgeCount = null;
                }
            }
                
            
        }

        /// <summary>
        /// Get User info 
        /// </summary>
        void RefreshMemberContact()
        {

            if (_isInitialized) return;

            _isInitialized = true;

            AppData.IsFirstTimeMemberRefresh = true;


            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    Task.Run(async () =>
                    {


                        if (AppData.Device?.UserLoggedOnToDevice != null && !AppData.Device.UserLoggedOnToDevice.OneLists.Any())
                        {
                            var memberContactModel = new MemberContactModel();
                            await memberContactModel.UserGetByCardId(AppData.Device.CardId);



                            //GetPoints();
                            //NotificationCountChanged();
                            //GetWishlistCount();
                            //CouponsCountChanged();
                        }
                        
                       
                        

                        // await  loading.DismissAsync();
                    }).ConfigureAwait(false);

                }
                catch (Exception)
                {


                }
            }

        }

        internal async void SetTab(int selectedTabIndex)
        {
            switch (selectedTabIndex)
            {
                case 0:
                    await NavigationService.NavigateAsync("app:///MainTabbedPage?selectedTab=MainPage");
                    break;
                case 1:
                    await NavigationService.NavigateAsync("app:///MainTabbedPage?selectedTab=ItemCategoriesPage");
                    break;
                case 2:
                    await NavigationService.NavigateAsync("app:///MainTabbedPage?selectedTab=CartPage");
                    break;
                case 3:
                    await NavigationService.NavigateAsync("app:///MainTabbedPage?selectedTab=TransactionPage");
                    break;
                case 4:
                    await NavigationService.NavigateAsync("app:///MainTabbedPage?selectedTab=MoreInfoPage");
                    break;
                default:
                    break;
            }
        }

        protected virtual void RaiseIsActiveChanged()
        {
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetSelectedTab(int tabIndex)
        {
            SelectedTab = tabIndex;
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }
        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
        }
    }
}
