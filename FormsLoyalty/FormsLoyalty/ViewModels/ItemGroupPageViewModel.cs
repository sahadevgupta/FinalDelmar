using FormsLoyalty.Helpers;
using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using Microsoft.AppCenter.Crashes;
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

        private ObservableCollection<LoyItem> _items;
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
                
            }
        }

        private bool _isNoItemFound;
        public bool IsNoItemFound
        {
            get { return _isNoItemFound; }
            set { SetProperty(ref _isNoItemFound, value); }
        }


        private string _itemCategoryId;
        public string ItemCategoryId
        {
            get { return _itemCategoryId; }
            set { SetProperty(ref _itemCategoryId, value); }
        }
        private string _productGroupId;
        public string ProductGroupId
        {
            get { return _productGroupId; }
            set { SetProperty(ref _productGroupId, value); }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private string _arabicDescription;
        public string ArabicDescription
        {
            get { return _arabicDescription; }
            set { SetProperty(ref _arabicDescription, value); }
        }

        private string _image;
        public string Image
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }

        


        private int pageSize = 7;
        private int pageNumber = 1;

        ItemModel itemModel;
        private List<LoyItem> LoyItems;

        public DelegateCommand SearchCommand { get; set; }

        public ItemGroupPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            itemModel = new ItemModel();
            SearchCommand = new DelegateCommand(ExecuteSearchCommand);
            Items = new ObservableCollection<LoyItem>();
            
        }

        private void ExecuteSearchCommand()
        {
            GetItemOnSearch(SearchText);
        }
        #region SearchFilter
        

        private void GetItemOnSearch(string s_word)
        {
            IsNoItemFound = false;
            Device.BeginInvokeOnMainThread(async() =>
            {
                IsPageEnabled = true;
                try
                {
                    if (!string.IsNullOrEmpty(s_word) && s_word.Length >= 3)
                    {
                        var items = await itemModel.GetItemsByPage(7, 1, ItemCategoryId, ProductGroupId, s_word, false, string.Empty);
                        Items = new ObservableCollection<LoyItem>();

                        if (items.Any())
                        {
                            IsNoItemFound = false;

                            foreach (var loyItem in items)
                            {
                                if (AppData.Basket != null)
                                {
                                    var isExist = AppData.Basket.Items?.Any(x => x.ItemId == loyItem.Id);
                                    if (isExist == true)
                                    {
                                        var basketItem = AppData.Basket.Items.FirstOrDefault(x => x.ItemId == loyItem.Id);
                                        loyItem.Quantity = basketItem.Quantity;
                                    }
                                    else
                                        loyItem.Quantity = 0;
                                }

                                if (AppData.Device.UserLoggedOnToDevice?.GetWishList(AppData.Device.CardId).Items?.Any(x => x.ItemId == loyItem.Id) == true)
                                {
                                    loyItem.IsWishlisted = true;
                                }
                                else
                                    loyItem.IsWishlisted = false;

                                if (loyItem.Discount > 0)
                                {
                                    var discountedPrice = (loyItem.Discount / 100) * Convert.ToDecimal(loyItem.ItemPrice);
                                    loyItem.NewPrice = (Convert.ToDecimal(loyItem.ItemPrice) - discountedPrice).ToString("F", CultureInfo.InvariantCulture);
                                }

                                Items.Add(loyItem);

                            }
                            //TempList.ToList().AddRange(items);
                            UpdateItemQuantityAndWishlist();
                        }

                        else
                            IsNoItemFound = true;

                    }
                    else
                        Items = new ObservableCollection<LoyItem>(TempList);
                }
                catch (Exception ex)
                {

                    Crashes.TrackError(ex);
                }
                finally
                {
                    IsPageEnabled = false;
                }

                
            });
        }
        #endregion


        #region Image Function
        private void UpdateItemQuantityAndWishlist()
        {
            try
            {
                Task.Run(async () =>
                {
                    foreach (var item in Items)
                    {

                        if (AppData.Basket != null)
                        {
                            var isExist = AppData.Basket.Items?.Any(x => x.ItemId == item.Id);
                            if (isExist == true)
                            {
                                var basketItem = AppData.Basket.Items.FirstOrDefault(x => x.ItemId == item.Id);
                                item.Quantity = basketItem.Quantity;
                            }
                            else
                                item.Quantity = 0;
                        }

                        if (AppData.Device.UserLoggedOnToDevice?.GetWishList(AppData.Device.CardId).Items?.Any(x => x.ItemId == item.Id) == true)
                        {
                            item.IsWishlisted = true;
                        }
                        else
                            item.IsWishlisted = false;

                        //if (item.Images.Count > 0)
                        //{
                        //    if (string.IsNullOrEmpty(item.Images[0].Image))
                        //    {
                        //        var imgView = await ImageHelper.GetImageById(item.Images[0].Id, new LSRetail.Omni.Domain.DataModel.Base.Retail.ImageSize(396, 396));
                        //        if (imgView == null)
                        //        {
                        //            item.Images[0].Image = "noimage.png";
                        //        }
                        //        else
                        //            item.Images[0].Image = imgView.Image;
                        //    }

                        //}
                        //else
                        //  item.Images = new List<ImageView> { new ImageView { Image = "noimage.png" } };

                    }
                });

            }
            catch (Exception)
            {


            }
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



        internal async Task LoadMore()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await LoadLoyItem(ItemCategoryId, ProductGroupId);

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
                    await LoadLoyItem(ItemCategoryId, ProductGroupId);
                }
                else if(response.Equals(AppResources.txtLowHigh))
                {
                    Items = new ObservableCollection<LoyItem>();
                    IsSortedDesc = false;
                    await LoadLoyItem(ItemCategoryId, ProductGroupId);
                }
            }
            else if (text.Equals("Z < A"))
            {
                Items = new ObservableCollection<LoyItem>();
                SelectedSortOption = "name";
                IsSortedDesc = true;
                await LoadLoyItem(ItemCategoryId, ProductGroupId);
            }
            else
            {
                Items = new ObservableCollection<LoyItem>();
                SelectedSortOption = "name";
                IsSortedDesc = false;
                await LoadLoyItem(ItemCategoryId, ProductGroupId);
            }
        }

        public async Task LoadNextItems()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {

                if (product?.Items.Count > 0)
                {
                    CalculateItemPrice(product.Items);

                    UpdateItemQuantityAndWishlist();
                }

                else 
                {
                    await LoadLoyItem(ItemCategoryId,ProductGroupId);
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
                //if (loyItem.Prices.Count > 0)
                //{
                //    loyItem.Price = loyItem.PriceFromVariantsAndUOM(loyItem.SelectedVariant?.Id, loyItem.SelectedUnitOfMeasure?.Id);
                //    if (loyItem.Discount > 0)
                //    {
                //        var discountedPrice = (loyItem.Discount / 100) * Convert.ToDecimal( loyItem.Price);
                //        loyItem.NewPrice = (Convert.ToDecimal(loyItem.Price) - discountedPrice).ToString("F",CultureInfo.InvariantCulture);
                //    }
                //}

                if (loyItem.Discount > 0)
                {
                    var discountedPrice = (loyItem.Discount / 100) * Convert.ToDecimal(loyItem.ItemPrice);
                    loyItem.NewPrice = (Convert.ToDecimal(loyItem.ItemPrice) - discountedPrice).ToString("F", CultureInfo.InvariantCulture);
                }

                Items.Add(loyItem);
               
            }
            TempList = new ObservableCollection<LoyItem>(Items);
        }

        public async Task LoadLoyItem(string ItemCategoryId,string productGroupId)
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

                var items = await itemModel.GetItemsByPage(pageSize, pageNumber, ItemCategoryId, productGroupId, SearchText,IsSortedDesc,SelectedSortOption);
                if (items!=null && items.Any())
                {
                    IsNoItemFound = false;
                    CalculateItemPrice(items);
                }


                
                UpdateItemQuantityAndWishlist();
                if(product!=null)
                   product.Items = Items.ToList();

                if (Items== null || !Items.Any())
                {
                    IsNoItemFound = true;
                }
            }
        }

        public async override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            SortingOptions = new ObservableCollection<string> { AppResources.txtPrice, "A > Z", "Z < A" };

            if (parameters.TryGetValue<List<LoyItem>>("items",out List<LoyItem> items))
            {
                LoyItems = items;
                ItemCategoryId = LoyItems[0].ItemFamilyCode;
                ProductGroupId = LoyItems[0].ProductGroupId;
                Description = LoyItems[0].ProductGroupId;
                ArabicDescription = string.Empty;
                CalculateItemPrice(items);

                UpdateItemQuantityAndWishlist();
            }
            else
            {
                product = parameters.GetValue<ProductGroup>("prodGroup");
                ItemCategoryId = product.ItemCategoryId;
                ProductGroupId = product.Id;
                Description = product.Description;
                ArabicDescription = product.ArabicDescription;
                await LoadNextItems();
            }
            
           
        }
        
    }
}
