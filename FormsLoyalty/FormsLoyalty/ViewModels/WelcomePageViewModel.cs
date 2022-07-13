using FormsLoyalty.Interfaces;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using LSRetail.Omni.Domain.Services.Loyalty.Devices;
using FormsLoyalty.Utils;
using System.Threading.Tasks;
using FormsLoyalty.Views;

namespace FormsLoyalty.ViewModels
{
    public class WelcomePageViewModel : ViewModelBase
    {
        public DelegateCommand onNextCommand { get; set; }
        public WelcomePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            onNextCommand = new DelegateCommand(async()=> await GoToTourPage());
        }

        private async Task GoToTourPage()
        {
            IsPageEnabled = true;
            SaveDevice();
            await NavigationService.NavigateAsync(nameof(DemonstrationPage));
            IsPageEnabled = false;
        }

        private void SaveDevice()
        {
            var uuid = Xamarin.Forms.DependencyService.Get<INotify>().getDeviceUuid();
            var deviceRepo =  PrismApplicationBase.Current.Container.Resolve<IDeviceLocalRepository>();
            Device device = new Device();
            device.Id = uuid;
            FormsLoyalty.Utils.Utils.FillDeviceInfo(device);

            deviceRepo.SaveDevice(device);

            AppData.Device = device;
        }
    }
}
