using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.Domain.Services.Loyalty.Transactions;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Orders;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace FormsLoyalty.ViewModels
{
    public class TransactionDetailPageViewModel : ViewModelBase
    {
        private SalesEntry _transaction;
        public SalesEntry transaction
        {
            get { return _transaction; }
            set { SetProperty(ref _transaction, value); }
        }

        private bool _reOrderBtnVisible = true;
        public bool IsReOrderBtnVisible
        {
            get { return _reOrderBtnVisible; }
            set { SetProperty(ref _reOrderBtnVisible, value); }
        }

        #region Order Detail Price

        private string _totalSubtotal;
        public string TotalSubtotal
        {
            get { return _totalSubtotal; }
            set { SetProperty(ref _totalSubtotal, value); }
        }

        private string _totalShipping;
        public string TotalShipping
        {
            get { return _totalShipping; }
            set { SetProperty(ref _totalShipping, value); }
        }

        private string _totalVAT;
        public string TotalVat
        {
            get { return _totalVAT; }
            set { SetProperty(ref _totalVAT, value); }
        }

        private string _totalDiscount;
        public string TotalDiscount
        {
            get { return _totalDiscount; }
            set { SetProperty(ref _totalDiscount, value); }
        }

        private string _total;
        public string Total
        {
            get { return _total; }
            set { SetProperty(ref _total, value); }
        }

        #endregion

        public DelegateCommand ReOrderCommand { get; set; }
        public TransactionDetailPageViewModel(INavigationService navigationService) : base (navigationService)
        {
            ReOrderCommand = new DelegateCommand(async()=> await AddToBasket());
        }

        private async Task AddToBasket()
        {
            IsPageEnabled = true;
            try
            {
                if (!AppData.IsLoggedIn)
                {
                   await NavigationService.NavigateAsync(nameof(LoginPage));
                    IsPageEnabled = false;
                    return;
                }

               

                foreach (var line in transaction.Lines)
                {
                    if (line.ItemId == AppResources.ResourceManager.GetString("ShipmentItemID",AppResources.Culture))
                        continue;

                    var Item = await new ItemModel().GetItemById(line.ItemId, false);
                    if (Item != null)
                    {

                        OneListItem basketItem = new OneListItem()
                        {
                            ItemId = Item.Id,
                            ItemDescription = Item.Description,
                            Image = Item.DefaultImage,
                            Quantity = line.Quantity,
                            Price = Item.AmtFromVariantsAndUOM(Item.SelectedVariant?.Id, Item.SelectedUnitOfMeasure?.Id)
                        };

                        if (Item.SelectedVariant != null)
                        {
                            basketItem.VariantId = Item.SelectedVariant.Id;
                            basketItem.VariantDescription = Item.SelectedVariant.ToString();
                        }

                        if (Item.SelectedUnitOfMeasure != null)
                        {
                            basketItem.UnitOfMeasureId = Item.SelectedUnitOfMeasure.Id;
                            basketItem.UnitOfMeasureDescription = Item.SelectedUnitOfMeasure.Description;
                        }

                        if (string.IsNullOrEmpty(basketItem.VariantId) && Item.VariantsRegistration != null && Item.VariantsRegistration.Count > 0)
                        {
                            Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                            {
                                await MaterialDialog.Instance.SnackbarAsync(message: AppResources.ResourceManager.GetString("ItemViewPickVariant", AppResources.Culture),
                                                       msDuration: MaterialSnackbar.DurationLong);
                            });
                        }
                        else
                        {
                            await new BasketModel().AddItemToBasket(basketItem, openBasket: true, ShowIndicatorOption: false);
                           
                            IsReOrderBtnVisible = false;

                        }


                    }


                }

                
            }
            catch (Exception)
            {

                IsReOrderBtnVisible = true; 

                IsPageEnabled = false;
            }
            IsPageEnabled = false;
        }

        public async Task LoadTransaction(SalesEntry Transaction)
        {
            IsPageEnabled = true;
            try
            {
                var service = new TransactionService(new TransactionRepository());
                var loadedTransaction = await service.SalesEntryGetByIdAsync(Transaction.Id, Transaction.IdType);
                if (loadedTransaction == null)
                {
                    await NavigationService.GoBackAsync();
                }
                else
                {
                    transaction = loadedTransaction;

                    TotalSubtotal = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(transaction.TotalNetAmount + transaction.TotalDiscount);
                    TotalShipping = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(AppData.Basket.ShippingAmount);
                    TotalVat = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(transaction.TotalAmount - transaction.TotalNetAmount);
                    TotalDiscount = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(transaction.TotalDiscount);
                    Total = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(transaction.TotalAmount);
                }
                    
                
            }
            catch (Exception)
            {

                IsPageEnabled = false;
            }
            IsPageEnabled = false;
        }
        internal async Task NavigateToItemPage(SalesEntryLine salesEntryLine)
        {
            if (IsPageEnabled)
                return;

            IsPageEnabled = true;
            
            await NavigationService.NavigateAsync(nameof(ItemPage), new NavigationParameters { { "itemId", salesEntryLine.ItemId } });
           
            IsPageEnabled = false;
        }
        public async override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

           var Transaction = parameters.GetValue<SalesEntry>("transaction");
           await LoadTransaction(Transaction);
        }

        
    }
}
