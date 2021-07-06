using FormsLoyalty.Helpers;
using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace FormsLoyalty.ViewModels
{
    public class ItemGroupPageViewModel : ViewModelBase
    {
        private ProductGroup _product;
        public ProductGroup product
        {
            get { return _product; }
            set { SetProperty(ref _product, value); }
        }

        private ObservableCollection<LoyItem> _items = new ObservableCollection<LoyItem>();
        public ObservableCollection<LoyItem> Items
        {
            get { return _items; }
            set 
            { 
                SetProperty(ref _items, value);
               
            }
        }
        private ObservableCollection<LoyItem> _TempList = new ObservableCollection<LoyItem>();
        public ObservableCollection<LoyItem> TempList
        {
            get { return _TempList; }
            set { SetProperty(ref _TempList, value); }
        }

        private int _itemTreshold;
        public int ItemTreshold
        {
            get { return _itemTreshold; }
            set { SetProperty(ref _itemTreshold, value); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }
        #region Sort By
        private ObservableCollection<string> _sortingOptions;
        public ObservableCollection<string> SortingOptions
        {
            get { return _sortingOptions; }
            set { SetProperty(ref _sortingOptions, value); }
        }

        private string _selectedSortOption;
        public string SelectedSortOption
        {
            get { return _selectedSortOption; }
            set { SetProperty(ref _selectedSortOption, value); }
        }

        public bool IsSortedDesc { get; private set; }

        #endregion

        private string _searchTextt;
        public string SearchText
        {
            get { return this._searchTextt; }
            set
            {
                SetProperty(ref _searchTextt, value);
                if (value != null)
                {
                    SearchItems(value);
                }
            }
        }

        private bool _isNoItemFound;
        public bool IsNoItemFound
        {
            get { return _isNoItemFound; }
            set { SetProperty(ref _isNoItemFound, value); }
        }

        Timer timer;


        private int pageSize = 7;
        private int pageNumber = 1;

        ItemModel itemModel;
       
        public ItemGroupPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            itemModel = new ItemModel();
            
        }
        #region SearchFilter
        private void SearchItems(string value)
        {
            if (timer != null)
            {
                timer.Dispose();
            }
            SetUpTimer(TimeSpan.FromSeconds(1), value);
        }

        private void SetUpTimer(TimeSpan alertTime, string value)
        {
            this.timer = new Timer(x =>
            {
                this.GetMenuOnSearch(value);
            }, null, alertTime, Timeout.InfiniteTimeSpan);
        }

        private void GetMenuOnSearch(string s_word)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                IsPageEnabled = true;
                if (!string.IsNullOrEmpty(s_word))
                {
                    if (TempList != null)
                    {
                        Items = new ObservableCollection<LoyItem>(TempList.Where(x => x.Description.ToLower().Contains(s_word.ToLower())));
                    }
                }
                else
                    Items = new ObservableCollection<LoyItem>(TempList);

                

                IsPageEnabled = false;
            });
        }
        #endregion
          internal async Task NavigateToItemPage(LoyItem loyItem)
        {
            IsPageEnabled = true;
            await NavigationService.NavigateAsync(nameof(ItemPage), new NavigationParameters { { "item", loyItem } });
            IsPageEnabled = false;
        }


        internal async Task<bool> OnQtyChanged(LoyItem loyItem,decimal Qty)
        {
            IsPageEnabled = true;
            bool IsSuccess = false;

            var selectedBasket = AppData.Basket.Items.Where(x => x.ItemId == loyItem.Id).FirstOrDefault();
            if (selectedBasket != null)
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

        internal async Task AddRemoveWishList(LoyItem loyItem)
        {
            IsPageEnabled = true;
            if (!EnabledItems.ForceLogin && AppData.Device.UserLoggedOnToDevice == null)
            {
                AppData.IsLoggedIn = false;
                await NavigationService.NavigateAsync(nameof(LoginPage));

            }
            else
            {
                var existingItem = AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId).ItemGetByIds(loyItem.Id, loyItem.SelectedVariant == null ? string.Empty : loyItem.SelectedVariant.Id, loyItem.SelectedUnitOfMeasure == null ? string.Empty : loyItem.SelectedUnitOfMeasure.Id);
                if (existingItem != null)
                {
                    await new ShoppingListModel().DeleteWishListLine(existingItem.Id, true);
                    loyItem.IsWishlisted = false;
                }
                else
                {
                    if (await AddToWishList(loyItem))
                    {
                        loyItem.IsWishlisted = true;
                        MessagingCenter.Send((App)Xamarin.Forms.Application.Current, "Wishlistadded");

                    }


                }
            }



            IsPageEnabled = false;
        }

        internal async Task<bool> AddToWishList(LoyItem loyItem)
        {
            if (!EnabledItems.ForceLogin && AppData.Device.UserLoggedOnToDevice == null)
            {

                AppData.IsLoggedIn = false;
                await NavigationService.NavigateAsync(nameof(LoginPage));

                return false;
            }
            else
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
        }

        internal async Task<bool> AddItemToBasket(LoyItem loyItem)
        {
            IsPageEnabled = true;

            if (!EnabledItems.ForceLogin && AppData.Device.UserLoggedOnToDevice == null)
            {
                AppData.IsLoggedIn = false;
                await NavigationService.NavigateAsync(nameof(LoginPage));
                IsPageEnabled = false;
                return false;
            }

            OneListItem basketItem = GlobalMethods.CreateOneListItem(loyItem, 1);

            try
            {
                var basketModel = new BasketModel();
                var isSuccess = await basketModel.AddItemToBasket(basketItem);
                if (isSuccess)
                {
                    loyItem.Quantity = 1;
                    
                    IsPageEnabled = false;
                    return true;
                }


            }
            catch (Exception)
            {
                IsPageEnabled = false;
                return false;

            }
            IsPageEnabled = false;
            return false;


        }

        private void LoadItemImage()
        {
            try
            {
                Task.Run(async() =>
                {
                    foreach (var item in Items)
                    {
                        if (item.Images.Count > 0)
                        {
                            if (string.IsNullOrEmpty(item.Images[0].Image))
                            {
                                var imgView = await ImageHelper.GetImageById(item.Images[0].Id, new LSRetail.Omni.Domain.DataModel.Base.Retail.ImageSize(396, 396));
                                if (imgView == null)
                                {
                                    item.Images[0].Image = "noimage.png" ;
                                }
                                else
                                item.Images[0].Image = imgView.Image;
                            }
                           
                        }
                        //else
                          //  item.Images = new List<ImageView> { new ImageView { Image = "noimage.png" } };
                        
                    }
                });
                
            }
            catch (Exception)
            {

                
            }
        }

        internal async Task LoadMore()
        {

            if (IsBusy)
                return;

            IsBusy = true;
            await LoadLoyItem();
            IsBusy = false;
        }

        internal async Task FilterItems(string text)
        {
           
            if (text.Equals(AppResources.txtPrice))
            {
                SelectedSortOption = "price";
               var response =  await App.dialogService.DisplayActionSheetAsync(null, AppResources.ApplicationCancel,null, AppResources.txtHighLow, AppResources.txtLowHigh);
                if (response == null || response.Equals(AppResources.ApplicationCancel))
                    return;
                else if (response.Equals(AppResources.txtHighLow))
                {
                    Items = new ObservableCollection<LoyItem>();
                    IsSortedDesc = true;
                    await LoadLoyItem();
                }
                else if(response.Equals(AppResources.txtLowHigh))
                {
                    Items = new ObservableCollection<LoyItem>();
                    IsSortedDesc = false;
                    await LoadLoyItem();
                }
            }
            else if (text.Equals("Z < A"))
            {
                Items = new ObservableCollection<LoyItem>();
                SelectedSortOption = "name";
                IsSortedDesc = true;
                await LoadLoyItem();
            }
            else
            {
                Items = new ObservableCollection<LoyItem>();
                SelectedSortOption = "name";
                IsSortedDesc = false;
                await LoadLoyItem();
            }
        }

        public async Task LoadNextItems()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {

                if (product.Items.Count > 0)
                {
                    CalculateItemPrice(product.Items);

                    LoadItemImage();
                }

                else 
                {
                    await LoadLoyItem();
                } 
             
                
            }
            catch (Exception)
            {
               

            }
            finally
            {
                IsBusy = false;
            }

        }

        private void CalculateItemPrice(List<LoyItem> loyItems)
        {
            foreach (var loyItem in loyItems)
            {
                if (loyItem.Prices.Count > 0)
                {
                    loyItem.Price = loyItem.PriceFromVariantsAndUOM(loyItem.SelectedVariant?.Id, loyItem.SelectedUnitOfMeasure?.Id);
                    if (loyItem.Discount > 0)
                    {
                        var discountedPrice = (loyItem.Discount / 100) * Convert.ToDecimal( loyItem.Price);
                        loyItem.NewPrice = (Convert.ToDecimal(loyItem.Price) - discountedPrice).ToString("F",CultureInfo.InvariantCulture);
                    }
                }
                Items.Add(loyItem);
               
            }
            TempList = new ObservableCollection<LoyItem>(Items);
        }

        public async Task LoadLoyItem()
        {
            IsNoItemFound = false;
            if (Items.Count % pageSize == 0)
            {
                double num = ((double)Items.Count / (double)pageSize) + 1.00;

                if (Math.Round(num) != num)
                {
                    return;
                }

                pageNumber = Convert.ToInt32(num);


                var items = await itemModel.GetItemsByPage(pageSize, pageNumber, product.ItemCategoryId, product.Id, string.Empty,IsSortedDesc,SelectedSortOption);
                if (items.Any())
                {
                    IsNoItemFound = false;
                    CalculateItemPrice(items);
                }
                else
                    IsNoItemFound = true;
                LoadItemImage();
                product.Items = Items.ToList();
            }
        }

        public async override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            SortingOptions = new ObservableCollection<string> { AppResources.txtPrice, "A > Z", "Z < A" };
            product = parameters.GetValue<ProductGroup>("prodGroup");
            await LoadNextItems();
           
        }
        
    }
}
