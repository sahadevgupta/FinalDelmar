using FormsLoyalty.Helpers;
using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
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

        private readonly IUnityContainer _unityContainer;
        public Page currentTab;
       
        public MainTabbedPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            RTL = Settings.RTL;
            //if (!AppData.IsLanguageChanged)
            //{
                RefreshMemberContact();
            //}
               

            MessagingCenter.Subscribe<BasketModel>(this, "CartUpdated", CheckCartCount);
            MessagingCenter.Subscribe<App>((App)Xamarin.Forms.Application.Current, "LoggedIn", ReloadView);

            
            
        }

        private void ReloadView(App obj)
        {
            RefreshMemberContact();
        }

        internal void CheckCartCount(BasketModel obj)
        {
            if (AppData.Basket!=null)
            {
                BadgeCount = AppData.Basket.Items.Count == 0 ? null : AppData.Basket.Items.Count.ToString();
            }
        }

        /// <summary>
        /// Get User info 
        /// </summary>
        void RefreshMemberContact()
        {
            AppData.IsFirstTimeMemberRefresh = true;
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    Task.Run(async() =>
                    {
                        if (EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null)
                        {
                            var memberContactModel = new MemberContactModel();
                            await memberContactModel.UserGetByCardId(AppData.Device.CardId);

                            CheckCartCount(null);
                           
                            //GetPoints();
                            //NotificationCountChanged();
                            //GetWishlistCount();
                            //CouponsCountChanged();
                        }
                    });
                   
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
