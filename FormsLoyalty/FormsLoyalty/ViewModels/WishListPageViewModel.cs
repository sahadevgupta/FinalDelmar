using FormsLoyalty.Helpers;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XF.Material.Forms.Models;
using XF.Material.Forms.UI.Dialogs;

namespace FormsLoyalty.ViewModels
{
    public class WishListPageViewModel : ViewModelBase
    {

        private ObservableCollection<OneListItem> _wishlist;
        public ObservableCollection<OneListItem> WishList
        {
            get { return _wishlist; }
            set { SetProperty(ref _wishlist, value); }
        }

        public MaterialMenuItem[] Actions => new MaterialMenuItem[]
        {
            new MaterialMenuItem
            {
                Text = AppResources.ResourceManager.GetString("ApplicationAddToBasket",AppResources.Culture)
            },
            new MaterialMenuItem
            {
                Text = AppResources.ResourceManager.GetString("ShoppingListDetailViewDeleteItemFromList",AppResources.Culture)
            }
        };

        public ICommand MenuCommand => new Command<MaterialMenuResult>(async (s) => await ListMenuSelected(s));

       

        public WishListPageViewModel(INavigationService navigationService) : base(navigationService)
        {
           
            
        }


        private async Task ListMenuSelected(MaterialMenuResult s)
        {
            IsPageEnabled = true;
            var onelistItem = s.Parameter as OneListItem;
            if (s.Index == 0)
            {
               await AddItemToBasket(onelistItem);
            }
            else if (s.Index == 1)
            {
                await DeleteShoppingListItem(onelistItem);
            }
            IsPageEnabled = false;
        }

        public async Task AddItemToBasket(OneListItem wishListItem)
        {
            IsPageEnabled = true;
            // Get the last data for the selected item, including its price
            try
            {
                var item = await new ItemModel().GetItemById(wishListItem.ItemId);
                if (item != null)
                {
                    wishListItem.Price = item.AmtFromVariantsAndUOM(wishListItem.VariantId, wishListItem.UnitOfMeasureId);

                    await new BasketModel().AddItemToBasket(wishListItem);
                }
            }
            catch (Exception)
            {

               
            }
            finally
            {
                IsPageEnabled = false;
            }
            


        }
        public async Task DeleteShoppingListItem(OneListItem item)
        {
            IsPageEnabled = true;
            var existinItemIndex = WishList.IndexOf(item);
            bool IsSuccess = await new ShoppingListModel().DeleteWishListLine(item.Id);
            IsPageEnabled = false;
            if (IsSuccess)
            {
                WishList.Remove(item);
                var action = await MaterialDialog.Instance.SnackbarAsync(message: AppResources.ResourceManager.GetString("ApplicatioItemDeleted", AppResources.Culture),
                                      actionButtonText: AppResources.ResourceManager.GetString("ApplicationUndo", AppResources.Culture),
                                      msDuration: 3000);
                if (action)
                {
                    WishList.Insert(existinItemIndex, item);
                    await new ShoppingListModel().AddItemToWishList(item, existinItemIndex);
                }
            }
        }


        internal async Task NavigateToItemPage(OneListItem oneListItem)
        {
            IsPageEnabled = true;
            try
            {
                var itemModel = new ItemModel();
                var item = await itemModel.GetItemById(oneListItem.ItemId);
                if (item != null)
                {
                    await NavigationService.NavigateAsync(nameof(ItemPage), new NavigationParameters { { "item", item } });
                }
                else
                  await  App.dialogService.DisplayAlertAsync("Error!!", "Unable to Proceed", "OK");
            }
            catch (Exception)
            {
                IsPageEnabled = false;
            }
            IsPageEnabled = false;
        }
        private void navigation(object obj)
        {
            
        }

        internal void LoadShoppingList()
        {
            IsPageEnabled = true;
            this.WishList = new ObservableCollection<OneListItem>();
            var  wishList = AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId);
            
            CalculateWishlistItemPrice(wishList.Items);
            
            IsPageEnabled = false;
        }

        private void CalculateWishlistItemPrice(List<OneListItem> oneListItems)
        {
            foreach (var wishlist in oneListItems)
            {
                if (wishlist.DiscountPercent > 0)
                {
                    var discountedPrice = (wishlist.DiscountPercent / 100) * Convert.ToDecimal(wishlist.Price);
                    wishlist.NewPrice = (Convert.ToDecimal(wishlist.Price) - discountedPrice).ToString("F", CultureInfo.InvariantCulture);
                }
                WishList.Add(wishlist);

            }
        }


        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            
        }
    }
}
