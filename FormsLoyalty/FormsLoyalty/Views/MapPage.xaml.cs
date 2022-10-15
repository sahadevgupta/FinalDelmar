using FormsLoyalty.ViewModels;
using Prism.Mvvm;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace FormsLoyalty.Views
{
    public partial class MapPage : ContentPage
    {
        MapPageViewModel _viewModel;
        public Xamarin.Essentials.Location CurrentLocation { get; private set; }
        public MapPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as MapPageViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Task.Run(async () => await GetCurrentLocation());
           
        }
        private async Task GetCurrentLocation()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }
                CurrentLocation = await Geolocation.GetLastKnownLocationAsync();
                if (CurrentLocation == null)
                {
                    CurrentLocation = await Geolocation.GetLocationAsync();
                }

                MapSpan mapSpan = MapSpan.FromCenterAndRadius(new Position(CurrentLocation.Latitude, CurrentLocation.Longitude), Distance.FromKilometers(0.444));
                map.MoveToRegion(mapSpan);
            }
            catch (Exception)
            {

               await DisplayAlert("Error!!","Location Permission is not granted","OK");
            }
           

        }

        private async void Pin_Clicked(object sender, EventArgs e)
        {
            var location = ((Pin)sender).BindingContext as ViewModels.Location;
           await _viewModel.NaviagteToDetail(location.Id);
        }
    }

   
}
