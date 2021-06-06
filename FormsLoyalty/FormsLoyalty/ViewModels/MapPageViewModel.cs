using FormsLoyalty.Interfaces;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.Services.Loyalty.Stores;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Custom;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Setup;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace FormsLoyalty.ViewModels
{
    public class MapPageViewModel : ViewModelBase
    {
        public List<Store> stores { get; set; }

        private ObservableCollection<Location> _locations = new ObservableCollection<Location>();
       

        public ObservableCollection<Location> locations
        {
            get { return _locations; }
            set { SetProperty(ref _locations, value); }
        }

        public MapPageViewModel(INavigationService navigationService) :base(navigationService)
        {

        }

        internal async Task NaviagteToDetail(string storeID)
        {
            var selectedStore = stores.First(x => x.Id == storeID);
           await NavigationService.NavigateAsync(nameof(StoreDetailPage), new NavigationParameters { { "store", selectedStore }});
        }

        

        private async Task AddMarkersToMap()
        {
            try
            {
                if (AppData.Stores == null || AppData.Stores.Count == 0)
                {
                    await LoadStoresFromServer();
                }
                else
                {
                    stores = AppData.Stores;
                    AddStoreMarkersToMap();
                }
            }
            catch (Exception)
            {

                Xamarin.Forms.DependencyService.Get<INotify>().ShowToast("Unable to connect the server!!");

            }

        }

        private async Task LoadStoresFromServer()
        {
            var service = new StoreService(new StoreRepository());
            stores = new List<Store>( await service.GetStoresAsync());

            AddStoreMarkersToMap();
        }
        private void AddStoreMarkersToMap()
        {
            foreach (Store store in stores)
            {
                if (store != null)
                {

                    Location location = new Location
                    {
                        position = new Position(double.Parse(store.Latitude.ToString(CultureInfo.InvariantCulture)), double.Parse(store.Longitude.ToString(CultureInfo.InvariantCulture))),
                        Title = store.Description,
                        Description = store.Address.Address1 + '\n' + store.Address.City,
                        Id = store.Id
                    };
                    locations.Add(location);
                   
                }
            }
        }

        public async override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            await AddMarkersToMap();
        }
    }
    public class Location : BindableBase
    {
        private Position _position;
        public Position position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }

        public string Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
    }
}
