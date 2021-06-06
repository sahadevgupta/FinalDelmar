using FormsLoyalty.Utils;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using QRCoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;

namespace FormsLoyalty.ViewModels
{
    public class AccountTierPageViewModel : ViewModelBase
    {

        private string _currentPoints;
        public string CurrentPoints
        {
            get { return _currentPoints; }
            set { SetProperty(ref _currentPoints, value); }
        }

        private ImageSource _qrCode;
        public ImageSource qrCode
        {
            get { return _qrCode; }
            set { SetProperty(ref _qrCode, value); }
        }

        private string _currentTier;
        public string currentTier
        {
            get { return _currentTier; }
            set { SetProperty(ref _currentTier, value); }
        }

        private string _nextTier;
        public string nextTier
        {
            get { return _nextTier; }
            set { SetProperty(ref _nextTier, value); }
        }

        private string _nextTierPerks;
        public string nextTierPerks
        {
            get { return _nextTierPerks; }
            set { SetProperty(ref _nextTierPerks, value); }
        }

        public AccountTierPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            LoadAccountData();
        }

        private void LoadAccountData()
        {

            //Generate QrCode

            //QRCodeGenerator qrGenerator = new QRCodeGenerator();
            //QRCodeData qrCodeData = qrGenerator.CreateQrCode("The text which should be encoded.", QRCodeGenerator.ECCLevel.Q);
            //QRCode qrCode = new QRCode(qrCodeData);
            //Bitmap qrCodeImage = qrCode.GetGraphic(20);

            
           

            var qrCodeBytes =  FormsLoyalty.Utils.Utils.QrCodeUtils.GenerateQRCode(false);

            qrCode = ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));

            //Current Level
            var user = AppData.Device.UserLoggedOnToDevice;
            var userLoggedOn = user.Account;
            CurrentPoints = String.Format(AppResources.ResourceManager.GetString("AccountTierViewPointsBalance",AppResources.Culture), userLoggedOn.PointBalance.ToString("N0"));
            currentTier = string.Format(AppResources.ResourceManager.GetString("AccountTierViewCurrentTier",AppResources.Culture), userLoggedOn.Scheme.Description);
            
            //Next Level
            nextTierPerks = userLoggedOn.Scheme.Perks;
            decimal points = userLoggedOn.Scheme.PointsNeeded - userLoggedOn.PointBalance;
            if (points < 0)
                points = 0;

            nextTier = string.Format(AppResources.ResourceManager.GetString("AccountTierViewNextTier",AppResources.Culture),
                                          userLoggedOn.Scheme.PointsNeeded.ToString("N0"),
                                          points.ToString("N0"),
                                          userLoggedOn.Scheme.NextScheme.Description);
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
           
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }

        
    }
}
