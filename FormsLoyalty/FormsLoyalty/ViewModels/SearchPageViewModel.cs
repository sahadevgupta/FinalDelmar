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
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

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
                if (!string.IsNullOrEmpty(value))
                {
                    OnSearchQuery();
                }
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
        public SearchPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            FilterCommand = new DelegateCommand(async() => await OnFilterClicked());
            searchModel = new GeneralSearchModel();

            ItemSelectedCommand = new DelegateCommand<object>(async (data) => await NaviagteToNextPage(data));

           
        }

        private async Task NaviagteToNextPage(object data)
        {
            IsPageEnabled = true;
            if (data is LoyItem item)
            {
                await NavigationService.NavigateAsync(nameof(ItemPage), new NavigationParameters { { "item", item } });
            }
            else if (data is Store store)
            {
                await NavigationService.NavigateAsync(nameof(StoreDetailPage), new NavigationParameters { { "store", store } });
            }
            IsPageEnabled = false;
        }

        private async Task OnFilterClicked()
        {

            var indexes = selectedTypes.Select((element, index) => element ? index : -1).Where(i => i >= 0).ToArray();

            var result = await MaterialDialog.Instance.SelectChoicesAsync(title: AppResources.ResourceManager.GetString("GeneralSearchViewChooseCategories",AppResources.Culture),
                                                                          selectedIndices: indexes,
                                                                          choices: typeNames
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

        public void OnSearchQuery()
        {
            IsPageEnabled = true;
            if (SearchKey.Length > 2)
            {
                if (SearchKey.Length > lastSearchLength)
                    Search(false);
                else
                    searchModel.ResetSearch();
            }

            lastSearchLength = SearchKey.Length;

            IsPageEnabled = false;
        }

        private async void Search(bool resetSearch)
        {
            IsPageEnabled = true;
            if (resetSearch)
                searchModel.ResetSearch();

            SearchType searchType = availableTypes.Where((t, i) => selectedTypes[i]).Aggregate<SearchType, SearchType>(0, (current, t) => current | t);

            var results = await searchModel.Search(SearchKey, searchType);

            if (results != null)
            {
               await LoadResults(results);
            }
            IsPageEnabled = false;
        }

        private async Task LoadResults(SearchRs results)
        {
            IsPageEnabled = true;
            items = new ObservableCollection<SearchGroup>();

           var result = await  LoadDataImage( results);

            foreach (var availableType in availableTypes.Where((t, i) => selectedTypes[i]))
            {
                if (availableType == SearchType.Item)
                {
                        items.Add(new SearchGroup
                            (
                            string.Format(AppResources.ResourceManager.GetString("GeneralSearchViewItemsGroupHeader", AppResources.Culture), result.Items.Count),
                            new List<object>(results.Items)
                            ));
                   

                }
                else if (availableType == SearchType.Notification)
                {
                    
                        items.Add(new SearchGroup
                            (
                            string.Format(AppResources.ResourceManager.GetString("GeneralSearchViewNotificationsGroupHeader", AppResources.Culture), result.Notifications.Count),
                            new List<object>(results.Notifications)
                            ));

                }
                else if (availableType == SearchType.SalesEntry)
                {
                   
                        items.Add(new SearchGroup
                           (
                           string.Format(AppResources.ResourceManager.GetString("GeneralSearchViewTransactionsGroupHeader", AppResources.Culture), result.SalesEntries.Count),
                           new List<object>(results.SalesEntries)
                           ));

                       
                   
                }
                else if (availableType == SearchType.OneList)
                {
                    
                        items.Add(new SearchGroup
                           (
                           string.Format(AppResources.ResourceManager.GetString("GeneralSearchViewWishListGroupHeader", AppResources.Culture), result.OneLists.Count),
                           new List<object>(results.OneLists)
                           ));

                       
                    
                }
                else if (availableType == SearchType.Store)
                {
                   
                        items.Add(new SearchGroup
                           (
                           string.Format(AppResources.ResourceManager.GetString("GeneralSearchViewStoresGroupHeader", AppResources.Culture), result.Stores.Count),
                           new List<object>(results.Stores)
                           ));

                       
                    
                }
            }

            IsPageEnabled = false;
        }

        private async Task<SearchRs> LoadDataImage(SearchRs result)
        {
            foreach (var item in result.Items)
            {
                if (item.Images != null && item.Images.Count > 0)
                {
                    item.Images[0].Image = await GetImag(item.Images[0].Id);
                }
            }

            foreach (var item in result.Stores)
            {
                if (item.Images != null && item.Images.Count > 0)
                {
                    item.Images[0].Image = await GetImag(item.Images[0].Id);
                }
            }

            foreach (var item in result.Notifications)
            {
                if (item.Images != null && item.Images.Count > 0)
                {
                    item.Images[0].Image = await GetImag(item.Images[0].Id);
                }
            }

            return result;
        }

       

        public async Task<string> GetImag(string imageId)
        {
            ImageView imageView = new ImageView();
            try
            {
               await Task.Run(async() =>
                {
                    imageView = await ImageHelper.GetImageById(imageId, new ImageSize(110, 110));
                });
               
            }
            catch (Exception)
            {

                
            }
            return imageView.Image;
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
                        typeNames[i] = AppResources.ResourceManager.GetString("GeneralSearchViewItem",AppResources.Culture);
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
