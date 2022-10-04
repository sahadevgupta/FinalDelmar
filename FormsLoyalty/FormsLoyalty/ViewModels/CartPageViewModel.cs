using FormsLoyalty.Helpers;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using Microsoft.AppCenter.Crashes;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Unity;
using XF.Material.Forms.UI.Dialogs;

namespace FormsLoyalty.ViewModels
{
    public class CartPageViewModel : MainTabbedPageViewModel
    {

        private ObservableCollection<Basket> _baskets = new ObservableCollection<Basket>();
        public ObservableCollection<Basket> baskets
        {
            get { return _baskets; }
            set { SetProperty(ref _baskets, value); }
        }

        #region Price

        private string _totalPrice;
        public string totalPrice
        {
            get { return _totalPrice; }
            set { SetProperty(ref _totalPrice, value); }
        }

        private string _totalSubtotal;
        public string totalSubtotal
        {
            get { return _totalSubtotal; }
            set { SetProperty(ref _totalSubtotal, value); }
        }

        private string _totalShipping;
        public string totalShipping
        {
            get { return _totalShipping; }
            set { SetProperty(ref _totalShipping, value); }
        }

        private string _totalVAT;
        public string totalVAT
        {
            get { return _totalVAT; }
            set { SetProperty(ref _totalVAT, value); }
        }

        private string _totalDiscount;
        public string totalDiscount
        {
            get { return _totalDiscount; }
            set { SetProperty(ref _totalDiscount, value); }
        }

       
        #endregion

        public DelegateCommand ProceedCommand { get; set; }
        public DelegateCommand DeleteAllCommand { get; set; }

        public CartPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            IsActiveChanged += CartPageViewModel_IsActiveChanged;
            ProceedCommand = new DelegateCommand(async() => await GoToCheckOutPage());
            DeleteAllCommand = new DelegateCommand(async () => await DeleteAllItemsFromCart());
        }

       

        private void CartPageViewModel_IsActiveChanged(object sender, EventArgs e)
        {
            if (IsActive)
            {
                
                if (AppData.Device.UserLoggedOnToDevice != null)
                    LoadBasketItems();
            }
        }

        private async Task DeleteAllItemsFromCart()
        {
            IsPageEnabled = true;
            try
            {
                var response = await App.dialogService.DisplayAlertAsync("Alert!!", AppResources.ResourceManager.GetString("BasketViewClearBasketPrompt", AppResources.Culture), AppResources.ResourceManager.GetString("ApplicationYes", AppResources.Culture), AppResources.ResourceManager.GetString("ApplicationNo", AppResources.Culture));
                if (response)
                {
                    var result = await new BasketModel().ClearBasket();
                    if (result)
                    {
                        LoadBasketItems();
                        totalPrice = string.Empty;
                    }
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

        private async Task GoToCheckOutPage()
        {
            if (IsPageEnabled)
                return;
            IsPageEnabled = true;
           await NavigationService.NavigateAsync("CheckoutPage");
            IsPageEnabled = false;
        }

        internal async Task<int> OnQtyChanged(Basket basket,decimal Qty)
        {
            IsPageEnabled = true;
           
            var existinItemIndex = baskets.IndexOf(basket);
            try
            {
               var response = await new BasketModel().EditItem(basket.Id, Qty, null);
                if (response)
                {
                   

                    var onelistitems = await new ShoppingListModel().GetOneListItemsByCardId(AppData.Device?.CardId, ListType.Basket);
                    if (onelistitems is object)
                    {
                        var temp = new ObservableCollection<Basket>();
                        foreach (var basketItem in onelistitems?.Items)
                        {

                            var item = new Basket();

                            item.Id = basketItem.Id;
                            item.ItemDescription = basketItem.ItemDescription;
                            //item.DiscountAmount = basketItem.DiscountAmount;


                            item.VariantDescription = basketItem.VariantDescription;
                            item.PriceWithoutDiscount = ((basketItem.NetPrice > 0 ? basketItem.NetPrice : basketItem.Amount) * basketItem.Quantity);
                            item.ItemId = basketItem.ItemId;
                            item.Quantity = basketItem.Quantity;
                            item.Price = basketItem.Price;

                            if (basketItem.DiscountAmount != 0 && basketItem.DiscountPercent == 0)
                            {
                                item.DiscountPercent = (basketItem.DiscountAmount / item.PriceWithoutDiscount) * 100;
                            }
                            else
                            {
                                item.DiscountPercent = basketItem.DiscountPercent;
                            }

                            if (item.DiscountPercent > 0)
                            {
                                var discountedPrice = (item.DiscountPercent / 100) * Convert.ToDecimal(basketItem.Price);
                                item.NewPrice = ((Convert.ToDecimal(item.Price) - discountedPrice) * item.Quantity).ToString("F", CultureInfo.InvariantCulture);

                                item.DiscountAmount = item.PriceWithoutDiscount - Convert.ToDecimal(item.NewPrice);
                            }

                            if (item.DiscountAmount != 0)
                            {
                                item.PriceWithDiscount = item.PriceWithoutDiscount - item.DiscountAmount;
                            }

                            item.UnitOfMeasureDescription = basketItem.UnitOfMeasureDescription;
                            item.UnitOfMeasureId = basketItem.UnitOfMeasureId;
                            item.VariantId = basketItem.VariantId;
                            //if (string.IsNullOrEmpty(basketItem.UnitOfMeasureId) == false)
                            //{
                            //    item.Qty = string.Format(AppResources.ResourceManager.GetString("ApplicationQtyN", AppResources.Culture), basketItem.Quantity.ToString() + " " + basketItem.UnitOfMeasureId);
                            //}
                            //else
                            //{
                            //    item.Qty = string.Format(AppResources.ResourceManager.GetString("ApplicationQtyN", AppResources.Culture), basketItem.Quantity.ToString("N0"));
                            //}
                            item.Image = basketItem.Image;
                            temp.Add(item);


                        }

                        baskets = new ObservableCollection<Basket>(temp);

                        
                    }
                    CalculateBasketPrice();

                    return existinItemIndex;
                }
                else
                    return -1;
            }
            catch (Exception)
            {

                return -1;
            }
            finally
            {
                IsPageEnabled = false;
            }
        }

       
        internal async Task OnDelete(Basket basket)
        {
            IsPageEnabled = true;
            var existinItemIndex = baskets.IndexOf(basket);
            var IsSucess = await new BasketModel().DeleteItem(basket);
            IsPageEnabled = false;
            if (IsSucess)
            {
                baskets.Remove(basket);
                LoadBasketItems();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(async() =>
                {
                    var action = await MaterialDialog.Instance.SnackbarAsync(message: AppResources.ResourceManager.GetString("ApplicatioItemDeleted", AppResources.Culture),
                                           actionButtonText: AppResources.ResourceManager.GetString("ApplicationUndo", AppResources.Culture),
                                           msDuration: 3000);
                    if (action)
                    {
                        baskets.Insert(existinItemIndex, basket);
                        IsPageEnabled = true;
                        await new BasketModel().AddItemToBasket(basket, index: existinItemIndex);
                        IsPageEnabled = false;
                    }
                    CalculateBasketPrice();
                });
               
            }

            
        }
        private void LoadBasketItems()
        {
            Task.Run(async () =>
            {
                try
                {
                    IsPageEnabled = true;

                    baskets = new ObservableCollection<Basket>();

                    var onelistitems = await new ShoppingListModel().GetOneListItemsByCardId(AppData.Device?.CardId, ListType.Basket);
                    if (onelistitems is object)
                    {
                        var temp = new ObservableCollection<Basket>();
                        foreach (var basketItem in onelistitems?.Items)
                        {

                            var item = new Basket();

                            item.Id = basketItem.Id;
                            item.ItemDescription = basketItem.ItemDescription;
                            //item.DiscountAmount = basketItem.DiscountAmount;


                            item.VariantDescription = basketItem.VariantDescription;
                            item.PriceWithoutDiscount = ((basketItem.NetPrice > 0 ? basketItem.NetPrice : basketItem.Amount) * basketItem.Quantity);
                            item.ItemId = basketItem.ItemId;
                            item.Quantity = basketItem.Quantity;
                            item.Price = basketItem.Price;

                            if (basketItem.DiscountAmount != 0 && basketItem.DiscountPercent == 0)
                            {
                                item.DiscountPercent = (basketItem.DiscountAmount / item.PriceWithoutDiscount) * 100;
                            }
                            else
                            {
                                item.DiscountPercent = basketItem.DiscountPercent;
                            }

                            if (item.DiscountPercent > 0)
                            {
                                var discountedPrice = (item.DiscountPercent / 100) * Convert.ToDecimal(basketItem.Price);
                                item.NewPrice = ((Convert.ToDecimal(item.Price) - discountedPrice) * item.Quantity).ToString("F", CultureInfo.InvariantCulture);

                                item.DiscountAmount = item.PriceWithoutDiscount - Convert.ToDecimal(item.NewPrice);
                            }

                            if (item.DiscountAmount != 0)
                            {
                                item.PriceWithDiscount = item.PriceWithoutDiscount - item.DiscountAmount;
                            }

                            item.UnitOfMeasureDescription = basketItem.UnitOfMeasureDescription;
                            item.UnitOfMeasureId = basketItem.UnitOfMeasureId;
                            item.VariantId = basketItem.VariantId;
                            //if (string.IsNullOrEmpty(basketItem.UnitOfMeasureId) == false)
                            //{
                            //    item.Qty = string.Format(AppResources.ResourceManager.GetString("ApplicationQtyN", AppResources.Culture), basketItem.Quantity.ToString() + " " + basketItem.UnitOfMeasureId);
                            //}
                            //else
                            //{
                            //    item.Qty = string.Format(AppResources.ResourceManager.GetString("ApplicationQtyN", AppResources.Culture), basketItem.Quantity.ToString("N0"));
                            //}
                            item.Image = basketItem.Image;
                            temp.Add(item);


                        }
                        baskets = new ObservableCollection<Basket>(temp);
                        
                    }
                    CalculateBasketPrice();

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
        private void CalculateBasketPrice()
        {
            try
            {
                if (baskets.Any())
                {

                    var discAmount = baskets.Sum(x => x.DiscountAmount);
                    decimal totalMRP = baskets.Sum(x => x.PriceWithoutDiscount);
                    totalSubtotal = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(totalMRP);

                    totalShipping = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(AppData.Basket.ShippingAmount);
                    totalVAT = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(AppData.Basket.TotalAmount - AppData.Basket.TotalNetAmount);


                    totalDiscount = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(discAmount);


                    if (AppData.Basket.State == BasketState.Dirty)
                    {
                        totalPrice = "~" + AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(totalMRP - discAmount);
                    }
                    else
                    {
                        totalPrice = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(totalMRP - discAmount);
                    }

                }
            }
            catch (Exception ex)
            {

                Crashes.TrackError(ex);
            }

        }
 
    }
}
