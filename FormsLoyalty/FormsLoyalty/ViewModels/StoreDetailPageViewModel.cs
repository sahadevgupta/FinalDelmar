using FormsLoyalty.Helpers;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FormsLoyalty.ViewModels
{
    public class StoreDetailPageViewModel : ViewModelBase
    {
        private Store _selectedStore;
        public Store selectedStore
        {
            get { return _selectedStore; }
            set { SetProperty(ref _selectedStore, value); }
        }
        private string _opening;
        public string opening
        {
            get { return _opening; }
            set { SetProperty(ref _opening, value); }
        }
        private string _storeAddress;
        public string storeAddress
        {
            get { return _storeAddress; }
            set { SetProperty(ref _storeAddress, value); }
        }
        private string _distanceText;
        public string distanceText
        {
            get { return _distanceText; }
            set { SetProperty(ref _distanceText, value); }
        }
        public Xamarin.Essentials.Location CurrentLocation { get; private set; }

        #region Command
        public DelegateCommand ShowPreviewCommand => new DelegateCommand(async () =>
        {
            if (string.IsNullOrEmpty(selectedStore.Images[0].Image) || selectedStore.Images[0].Image.ToLower().Contains("noimage".ToLower()))
                return;
           
            await NavigationService.NavigateAsync(nameof(ImagePreviewPage), new NavigationParameters { { "previewImage", selectedStore.Images[0].Image }, { "images", selectedStore.Images } });
        });
        #endregion


        public StoreDetailPageViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        internal async Task ShowDirections()
        {
            IsPageEnabled = true;
           await Launcher.OpenAsync("http://maps.google.com/maps?saddr=&daddr=" + selectedStore.Latitude.ToString(CultureInfo.InvariantCulture) + "," + selectedStore.Longitude.ToString(CultureInfo.InvariantCulture));
            IsPageEnabled = false;
        }

        private async void LoadData(Store Store)
        {
            IsPageEnabled = true;

            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }


            CurrentLocation = await Geolocation.GetLastKnownLocationAsync();

            if (Store.Images!=null && Store.Images.Count>0)
            {
                try
                {
                    if (string.IsNullOrEmpty(Store.Images[0].Image))
                    {
                        Task.Run(async () =>
                        {
                            if (Store.Images.Count>0)
                            {
                                var imagview = await ImageHelper.GetImageById(Store.Images[0].Id, new LSRetail.Omni.Domain.DataModel.Base.Retail.ImageSize(396, 396));
                                Store.Images[0].Image = imagview.Image;
                            }
                            else
                                Store.Images = new List<ImageView> { new ImageView { Image = "noimage.png" } };
                           
                        });
                    }
                    
                }
                catch (Exception)
                {

                   
                }
                
            }


            storeAddress = Store.Address.FormatAddress;
            if (Store.StoreHours.Count > 0)
            {


                foreach (var storeHour in Store.StoreHours)
                {
                    opening += string.Format(AppResources.ResourceManager.GetString("StorelocatorDetailViewOpeningHourDay", AppResources.Culture), storeHour.OpenFrom.ToString("t"), storeHour.OpenTo.ToString("t"), GetStoreHourDayName(storeHour.DayOfWeek)) + System.Environment.NewLine;
                }
                opening = opening.TrimEnd(System.Environment.NewLine.ToCharArray());
            }

            if (CurrentLocation!=null)
            {
                SetDistance(Store.Latitude, Store.Longitude);
            }
           

            selectedStore = Store;
            IsPageEnabled = false;
        }

       
        private void SetDistance(double destLatitude,double destLongitude)
        {
            

            Xamarin.Essentials.Location sourceCoordinates = new Xamarin.Essentials.Location(CurrentLocation.Latitude, CurrentLocation.Longitude);
            Xamarin.Essentials.Location destinationCoordinates = new Xamarin.Essentials.Location(destLatitude, destLongitude);
            double distance = Xamarin.Essentials.Location.CalculateDistance(sourceCoordinates, destinationCoordinates, DistanceUnits.Kilometers);

            distanceText = string.Format(AppResources.ResourceManager.GetString("StorelocatorDetailViewKilometersAway",AppResources.Culture), distance.ToString("N1"));

        }

        public Command PhoneCommand => new Command(()=>
        {
            try
            {
                if (string.IsNullOrEmpty(selectedStore.Phone)) return;

                PhoneDialer.Open(selectedStore.Phone);
            }
            catch (Exception)
            {

               
            }
        });

        private string GetStoreHourDayName(int dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case 0:   //Sunday
                    return AppResources.ResourceManager.GetString("StorelocatorDetailViewSundays",AppResources.Culture);
                case 1:   //Monday
                    return AppResources.ResourceManager.GetString("StorelocatorDetailViewMonday",AppResources.Culture);
                case 2:   //Tuesday
                    return AppResources.ResourceManager.GetString("StorelocatorDetailViewTuesday",AppResources.Culture);
                case 3:   //Wednesday
                    return AppResources.ResourceManager.GetString("StorelocatorDetailViewWednesday",AppResources.Culture);
                case 4:   //Thursday
                    return AppResources.ResourceManager.GetString("StorelocatorDetailViewThursday",AppResources.Culture);
                case 5:   //Friday
                    return AppResources.ResourceManager.GetString("StorelocatorDetailViewFriday",AppResources.Culture);
                case 6:   //Saturday
                    return AppResources.ResourceManager.GetString("StorelocatorDetailViewSaturday",AppResources.Culture);
                default:
                    return dayOfWeek.ToString();

            }
        }
        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            var Store = parameters.GetValue<Store>("store");
            // need to assign here
            LoadData(Store);
        }
    }
}
