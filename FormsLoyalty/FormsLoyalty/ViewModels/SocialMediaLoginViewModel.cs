using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FormsLoyalty.ViewModels
{
	public class SocialMediaLoginViewModel : ViewModelBase
	{
        private string _mobileNumber;
        public string MobileNumber
        {
            get { return _mobileNumber; }
            set { SetProperty(ref _mobileNumber, value); }
        }
        private string _errorText;
        public string ErrorText
        {
            get { return _errorText; }
            set { SetProperty(ref _errorText, value); }
        }

        private bool _IsError;
        private FacebookProfile fbProfile;
        private GoogleProfile googleProfile;

        public bool IsError
        {
            get { return _IsError; }
            set { SetProperty(ref _IsError, value); }
        }
        public DelegateCommand OnOtpCommand { get; set; }
        public SocialMediaLoginViewModel(INavigationService navigationService) : base(navigationService)
        {
            OnOtpCommand = new DelegateCommand(async () => { await NavigateToOtpView(); });
        }

        private async Task NavigateToOtpView()
        {
            IsPageEnabled = true;
            if (string.IsNullOrEmpty(MobileNumber))
            {
                ErrorText = "Mobile Number cannot be empty";
                IsError = true;
                IsPageEnabled = false;
                return;
            }

            if (MobileNumber.Length <= 11)
            {
                var _digit = MobileNumber.Substring(0, 3);
                if (!(_digit.Equals("011") || _digit.Equals("012") || _digit.Equals("015") || _digit.Equals("010")))
                {
                    await App.dialogService.DisplayAlertAsync("Error!!", "Mobile Number not in correct format.First 3 digit should be 010,011,015 or 012", AppResources.ResourceManager.GetString("ApplicationOk", AppResources.Culture));

                    IsPageEnabled = false;
                    return;
                }

            }
            try
            {
                var Otp = await new CommonModel().GenerateOTPAsync(MobileNumber);

                if (fbProfile != null )
                {
                    await NavigationService.NavigateAsync(nameof(LoginOtpView), new NavigationParameters { { "number", MobileNumber }, { "otp", Otp }, { "fb", fbProfile } });
                }
                else if (googleProfile != null)
                {
                    await NavigationService.NavigateAsync(nameof(LoginOtpView), new NavigationParameters { { "number", MobileNumber }, { "otp", Otp },{ "google", googleProfile } });
                }


            }
            catch (Exception ex)
            {

                DependencyService.Get<INotify>().ShowSnackBar(ex.Message);
            }
            finally
            {
                IsPageEnabled = false;
            }

        }
        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            if (parameters.TryGetValue<FacebookProfile>("fb", out FacebookProfile fbProfile))
            {
                this.fbProfile = fbProfile;
            }
            else if (parameters.TryGetValue("google", out GoogleProfile googleProfile))
            {
                this.googleProfile = googleProfile;
            }
        }
    }
}
