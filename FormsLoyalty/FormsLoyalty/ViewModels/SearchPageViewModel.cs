using FormsLoyalty.Helpers;
using FormsLoyalty.Models;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using Microsoft.AppCenter.Crashes;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using XF.Material.Forms;
using XF.Material.Forms.UI.Dialogs;
using XF.Material.Forms.UI.Dialogs.Configurations;

namespace FormsLoyalty.ViewModels
{
    public class SearchPageViewModel : ViewModelBase
    {
        private List<SearchType> availableTypes;
        private SearchType pageSearchType;
        private string[] typeNames;

        private string _fromPage;
        public string FromPage
        {
            get { return _fromPage; }
            set { SetProperty(ref _fromPage, value); }
        }

        

        private string _SearchKey;
        private int lastSearchLength;

        public string SearchKey
        {
            get { return _SearchKey; }
            set 
            { 
                SetProperty(ref _SearchKey, value);
                
            }
        }

        

        private ObservableCollection<SearchGroup> _items;
        public ObservableCollection<SearchGroup> items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }


        public DelegateCommand FilterCommand { get; set; }

        GeneralSearchModel searchModel;
        private bool[] selectedTypes;


        public DelegateCommand<object> ItemSelectedCommand { get; set; }
        public DelegateCommand SearchCommand { get; set; }
        public SearchPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            FilterCommand = new DelegateCommand(async() => await OnFilterClicked());
            searchModel = new GeneralSearchModel();

            ItemSelectedCommand = new DelegateCommand<object>(async (data) => await NaviagteToNextPage(data));
            SearchCommand = new DelegateCommand(async() => await ExecuteSearchCommand());


        }

       

        private async Task NaviagteToNextPage(object data)
        {
            IsPageEnabled = true;
            if (data is LoyItem item)
            {
                await NavigationService.NavigateAsync(nameof(ItemPage), new NavigationParameters { { "item", item } },animated:false);
            }
            else if (data is Store store)
            {
                await NavigationService.NavigateAsync(nameof(StoreDetailPage), new NavigationParameters { { "store", store } }, animated: false);
            }
            IsPageEnabled = false;
        }

        private async Task OnFilterClicked()
        {

            var indexes = selectedTypes.Select((element, index) => element ? index : -1).Where(i => i >= 0).ToArray();


            var simpleDialogConfiguration = new MaterialConfirmationDialogConfiguration
            {
                // BackgroundColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.PRIMARY),
                // TitleTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY),


                TitleTextColor = Color.Black,
                ButtonAllCaps = false,


                CornerRadius = 8,
                ControlSelectedColor = Color.FromHex("#b72228"),
                ControlUnselectedColor = Color.Black.MultiplyAlpha(0.66),
                TintColor = Color.FromHex("#b72228")
                // ScrimColor = Color.FromHex("#232F34").MultiplyAlpha(0.32)
            };

            var result = await MaterialDialog.Instance.SelectChoicesAsync(title: AppResources.ResourceManager.GetString("GeneralSearchViewChooseCategories",AppResources.Culture),
                                                                          selectedIndices: indexes,
                                                                          choices: typeNames,
                                                                          confirmingText: AppResources.ApplicationOk, 
                                                                          dismissiveText: AppResources.ApplicationCancel, configuration: simpleDialogConfiguration
                                                                           );


            if (result!=null)
            {
                selectedTypes = new bool[availableTypes.Count];

                foreach (var index in result)
                {
                    selectedTypes[index] = true;
                }
            }
        }

        #region Search

        private async Task ExecuteSearchCommand()
        {
           await OnSearchQuery().ConfigureAwait(false);
        }

        public async Task OnSearchQuery()
        {
           
            if (SearchKey.Length > 2)
            {
               await SearchAsync().ConfigureAwait(false);
               
            }
 
        }

        private async Task SearchAsync()
        {
            IsPageEnabled = true;

            try
            {
                SearchType searchType = availableTypes.Where((t, i) => selectedTypes[i]).Aggregate<SearchType, SearchType>(0, (current, t) => current | t);

                var results = await searchModel.Search(SearchKey, searchType).ConfigureAwait(false);

                if (results != null)
                {
                    LoadResults(results);
                }
            }
            catch (Exception ex) 
            {

                Crashes.TrackError(ex) ;
            }
            finally
            {
                IsPageEnabled = false;
            }
           
        }

        private void LoadResults(SearchRs results)
        {
          
           var tempItems = new ObservableCollection<SearchGroup>();

            foreach (var availableType in availableTypes.Where((t, i) => selectedTypes[i]))
            {
                if (availableType == SearchType.Item)
                {
                    tempItems.Add(new SearchGroup
                            (
                            string.Format(AppResources.ResourceManager.GetString("GeneralSearchViewItemsGroupHeader", AppResources.Culture), results.Items.Take(15).Count()),
                            new List<object>(results.Items.Take(15))
                            ));
                   

                }
                else if (availableType == SearchType.Notification)
                {

                    tempItems.Add(new SearchGroup
                            (
                            string.Format(AppResources.ResourceManager.GetString("GeneralSearchViewNotificationsGroupHeader", AppResources.Culture), results.Notifications.Take(15).Count()),
                            new List<object>(results.Notifications.Take(15))
                            ));

                }
                else if (availableType == SearchType.SalesEntry)
                {

                    tempItems.Add(new SearchGroup
                           (
                           string.Format(AppResources.ResourceManager.GetString("GeneralSearchViewTransactionsGroupHeader", AppResources.Culture), results.SalesEntries.Take(15).Count()),
                           new List<object>(results.SalesEntries.Take(15))
                           ));

                       
                   
                }
                else if (availableType == SearchType.OneList)
                {

                    tempItems.Add(new SearchGroup
                           (
                           string.Format(AppResources.ResourceManager.GetString("GeneralSearchViewWishListGroupHeader", AppResources.Culture), results.OneLists.Take(15).Count()),
                           new List<object>(results.OneLists.Take(15))
                           ));

                       
                    
                }
                else if (availableType == SearchType.Store)
                {

                    tempItems.Add(new SearchGroup
                           (
                           string.Format(AppResources.ResourceManager.GetString("GeneralSearchViewStoresGroupHeader", AppResources.Culture), results.Stores.Take(15).Count()),
                           new List<object>(results.Stores.Take(15))
                           ));

                       
                    
                }
            }

           items = new ObservableCollection<SearchGroup>(tempItems);

           
        }
        #endregion

        internal void LoadData()
        {
            if (availableTypes == null)
            {
                availableTypes = new List<SearchType>();

                if (!string.IsNullOrEmpty(FromPage))
                {
                    availableTypes.Add(pageSearchType);
                }
                else
                {
                    if (EnabledItems.HasItemCatalog)
                    {
                        availableTypes.Add(SearchType.Item);
                    }

                    if (EnabledItems.HasOffers)
                    {
                        availableTypes.Add(SearchType.Offer);
                    }

                    if (EnabledItems.HasCoupons)
                    {
                        availableTypes.Add(SearchType.Coupon);
                    }

                    if (EnabledItems.HasNotifications)
                    {
                        availableTypes.Add(SearchType.Notification);
                    }

                    if (EnabledItems.HasHistory)
                    {
                        availableTypes.Add(SearchType.SalesEntry);
                    }

                    if (EnabledItems.HasWishLists)
                    {
                        availableTypes.Add(SearchType.OneList);
                    }

                    if (EnabledItems.HasStoreLocator)
                    {
                        availableTypes.Add(SearchType.Store);
                    }
                }
            }

            if (selectedTypes == null)
            {
                selectedTypes = new bool[availableTypes.Count];

                for (int i = 0; i < availableTypes.Count; i++)
                {
                    selectedTypes[i] = true;
                }
            }

            if (typeNames == null)
            {
                typeNames = new string[availableTypes.Count];

                for (int i = 0; i < availableTypes.Count; i++)
                {
                    if (availableTypes[i] == SearchType.Item)
                    {
                        //typeNames[i] = AppResources.ResourceManager.GetString("GeneralSearchViewItem",AppResources.Culture);
                        typeNames[i] = "Item(Description & Active Ingredient)";
                    }
                    else if (availableTypes[i] == SearchType.Offer)
                    {
                        typeNames[i] = AppResources.ResourceManager.GetString("GeneralSearchViewOffer", AppResources.Culture);
                    }
                    else if (availableTypes[i] == SearchType.Coupon)
                    {
                        typeNames[i] = AppResources.ResourceManager.GetString("GeneralSearchViewCoupon", AppResources.Culture);
                    }
                    else if (availableTypes[i] == SearchType.Notification)
                    {
                        typeNames[i] = AppResources.ResourceManager.GetString("GeneralSearchViewNotification", AppResources.Culture);
                    }
                    else if (availableTypes[i] == SearchType.SalesEntry)
                    {
                        typeNames[i] = AppResources.ResourceManager.GetString("GeneralSearchViewTransaction", AppResources.Culture);
                    }
                    else if (availableTypes[i] == SearchType.OneList)
                    {
                        typeNames[i] = AppResources.ResourceManager.GetString("ShoppingListDetailViewWishlist", AppResources.Culture);
                    }
                    else if (availableTypes[i] == SearchType.Store)
                    {
                        typeNames[i] = AppResources.ResourceManager.GetString("GeneralSearchViewStore", AppResources.Culture);
                    }
                }
            }

        }


        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            pageSearchType = parameters.GetValue<SearchType>("searchType");
            FromPage = parameters.GetValue<string>("page");

            
        }

        
    }

    public class SearchGroup: List<object> 
    {
        public string Name { get; set; }
        public SearchGroup(string name, List<object>  loyItems) : base(loyItems)
        {
            Name = name;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    
}
