using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;
using LSRetail.Omni.Domain.Services.Loyalty.Transactions;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Orders;
using Prism;
using Prism.Commands;
using Prism.Ioc;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Unity;

namespace FormsLoyalty.ViewModels
{
    public class TransactionPageViewModel : MainTabbedPageViewModel
    {
        private ObservableCollection<SalesEntry> _salesEntries =new ObservableCollection<SalesEntry>();
        public ObservableCollection<SalesEntry> transactions
        {
            get { return _salesEntries; }
            set { SetProperty(ref _salesEntries, value); }
        }


        public DelegateCommand<SalesEntry> onSelectedCommand { get; set; }
        public TransactionPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            onSelectedCommand = new DelegateCommand<SalesEntry>(async (data) => await NavigateToDetail(data));

            IsActiveChanged += TransactionPageViewModel_IsActiveChanged;

           
        }

        private void TransactionPageViewModel_IsActiveChanged(object sender, EventArgs e)
        {
            if (IsActive)
            {
               LoadData();
            }
        }

        private async Task NavigateToDetail(SalesEntry data)
        {
            IsPageEnabled = true;
            await NavigationService.NavigateAsync(nameof(TransactionDetailPage), new NavigationParameters { { "transaction", data } });
            IsPageEnabled = false;
        }

        void LoadOrders(List<SalesEntry> entries)
        {
            transactions = new ObservableCollection<SalesEntry>();
            foreach (var order in entries)
            {

                order.OrderDate = order.DocumentRegTime.ToLocalTime();
                transactions.Add(order);
            }

            
        }

        private async void LoadData()
        {
            IsPageEnabled = true;

            if (AppData.Device.UserLoggedOnToDevice != null)
            {
                await GetOrderAsync();
                
            }
            IsPageEnabled = false;
        }
        public async Task GetOrderAsync()
        {
            try
            {
                var service = new TransactionService(new TransactionRepository());

                var loadedTransactions = await service.GetSalesEntriesAsync(AppData.Device.CardId, Int32.MaxValue);
               
                if (loadedTransactions != null)
                {
                    SaveLocalTransactions(loadedTransactions);

                    AppData.Device.UserLoggedOnToDevice.SalesEntries = loadedTransactions;
                    LoadOrders(loadedTransactions);
                }
            }
            catch (Exception ex)
            {
                LoadCacheOrder();
                IsPageEnabled = false;
            }
          
        }

        private void LoadCacheOrder()
        {
            if (AppData.Device.UserLoggedOnToDevice.SalesEntries?.Count > 0)
            {
                var entries = AppData.Device.UserLoggedOnToDevice.TransactionOrderedByDate;
                LoadOrders(entries);

            }
        }

        private void SaveLocalTransactions(List<SalesEntry> transactions)
        {
           // var service = new TransactionLocalService(new Infrastructure.Data.SQLite.Transactions.TransactionRepository());
            var localService = PrismApplicationBase.Current.Container.Resolve<ITransactionLocalRepository>();
            localService.SaveTransactions(transactions);
        }

       

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            
        }

        
    }
}
