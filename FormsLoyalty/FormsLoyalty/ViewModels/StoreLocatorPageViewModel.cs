using FormsLoyalty.Helpers;
using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.Services.Loyalty.Stores;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Setup;
using Newtonsoft.Json;
using Plugin.Settings;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace FormsLoyalty.ViewModels
{
    public class StoreLocatorPageViewModel : ViewModelBase
    {
        private ObservableCollection<Store> _stores = new ObservableCollection<Store>();

        public ObservableCollection<Store> stores
        {
            get { return _stores; }
            set { SetProperty(ref _stores, value); }
        }
        private int _count = 2;
        public int count
        {
            get { return _count; }
            set { SetProperty(ref _count, value); }
        }

        private int _horizontalSpac = 10;
        public int HorizontalSpac
        {
            get { return _horizontalSpac; }
            set { SetProperty(ref _horizontalSpac, value); }
        }

        private int _VerticalSpac = 10;
        public int VerticalSpac
        {
            get { return _VerticalSpac; }
            set { SetProperty(ref _VerticalSpac, value); }
        }

        private string _viewStockDesc;
        public string viewStockDesc
        {
            get { return _viewStockDesc; }
            set { SetProperty(ref _viewStockDesc, value); }
        }

        public ObservableCollection<Store> tempStores { get; private set; }

        public StoreLocatorPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            if (!AppData.IsViewStock)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(async() =>
                {
                   await LoadData();
                });
                
            }
            
        }

        internal async Task NavigateToMap()
        {
            IsPageEnabled = true;
            await NavigationService.NavigateAsync(nameof(MapPage));
            IsPageEnabled = false;
        }

        internal async Task NaviagteToDetail(Store store)
        {
            IsPageEnabled = true;
            await NavigationService.NavigateAsync(nameof(StoreDetailPage), new NavigationParameters { { "store", store }});
            IsPageEnabled = false;
        }
        private async Task LoadData()
        {
            IsPageEnabled = true;
            try
            {
                if (AppData.Stores == null || AppData.Stores?.Count == 0)
                {
                    await LoadStoresFromServer();
                }
                else
                {
                    stores = new ObservableCollection<Store>(AppData.Stores);
                }
            }
            catch (Exception)
            {

                IsPageEnabled = false;
            }
            
            IsPageEnabled = false;
        }


        private async Task LoadStoresFromServer()
        {
            try
            {
                var service = new StoreService(new StoreRepository());
                tempStores = new ObservableCollection<Store>(await service.GetStoresAsync());
                AppData.Stores = tempStores.ToList();

                stores = new ObservableCollection<Store>(tempStores.Take(15));
                
            }
            catch (Exception)
            {
                Xamarin.Forms.DependencyService.Get<INotify>().ShowToast("Unable to connect the server!!");
                IsPageEnabled = false;
            }
            
        }

        internal void LoadMore()
        {
            if(tempStores?.Count > 0)
            {
                foreach (var item in tempStores.Skip(stores.Count).Take(10))
                {
                    stores.Add(item);
                }
            }
               
        }

        
        private async void LoadStoresInStock(LoyItem item)
        {
            IsPageEnabled = true;
            AppData.IsViewStock = false;
            try
            {

                var storesInStock = await new StoreModel().GetItemsInStock(item.Id, item.SelectedVariant?.Id ?? string.Empty, 0, 0, 0);

                if (storesInStock != null)
                {
                    LoadStores(storesInStock, item);
                }
            }

            catch (Exception ex)
            {
                IsPageEnabled = false;
            }
            IsPageEnabled = false;
        }

        private async void LoadStores(List<Store> storesInStock,LoyItem item)
        {
            try
            {
                if (!string.IsNullOrEmpty(item.Id) && storesInStock.Count == 0)
                {

                    await App.dialogService.DisplayAlertAsync("Error!!", AppResources.ResourceManager.GetString("StorelocatorViewNotInStock", AppResources.Culture), AppResources.ResourceManager.GetString("ApplicationOk", AppResources.Culture));
                        
                    
                }
                else
                {
                   tempStores = new ObservableCollection<Store>(storesInStock);
                    
                    stores = new ObservableCollection<Store>(tempStores.Take(15));
                    viewStockDesc =$"{item.Description} - {item.SelectedVariant.ToString()}";

                    viewStockDesc = string.Format(AppResources.ResourceManager.GetString("StorelocatorViewInStockStores", AppResources.Culture), viewStockDesc);
                    AppData.Stores = stores.ToList();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            var item = parameters.GetValue<LoyItem>("viewStock");
            if (item!=null)
            {
                LoadStoresInStock(item);

            }

        }
        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
          
        }

        
    }
}
