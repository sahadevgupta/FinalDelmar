using FormsLoyalty.Views;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FormsLoyalty.ViewModels
{
    public class CameraPageViewModel : ViewModelBase
    {
        public DelegateCommand AcceptCommand { get; set; }

        private byte[] _capturedImage;
        public byte[] CapturedImage
        {
            get { return _capturedImage; }
            set { SetProperty(ref _capturedImage, value); }
        }

        public CameraPageViewModel(INavigationService navigationService):base(navigationService)
        {
            AcceptCommand = new DelegateCommand(async() => await OnAcceptClicked());
        }

        private async Task OnAcceptClicked()
        {
            await NavigationService.GoBackAsync(new NavigationParameters { {"image",true } });
        }

        internal async void NavigateToScanSendPage(byte[] bytes)
        {
           // var dateTimeNow = DateTime.Now;
            //var FileName = $"NewImage_{dateTimeNow:yyyyMMdd_HHmmss}";
            var imgData = new List<Tuple<byte[], string>>();
            imgData.Add(new Tuple<byte[], string>(bytes, "jpg"));
           await NavigationService.GoBackAsync(new NavigationParameters { { "images", imgData } });
        }
    }
}
