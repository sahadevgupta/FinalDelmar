using FormsLoyalty.Models;
using FormsLoyalty.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsLoyalty.ViewModels
{
    public class ForgotPasswordPageViewModel : ViewModelBase
    {
        private string _username;
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }
        public DelegateCommand CodeCommand { get; set; }
        public ForgotPasswordPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            CodeCommand = new DelegateCommand(async() => await CodeConfirmation());
        }

        private async Task CodeConfirmation()
        {
            IsPageEnabled = true;
            try
            {
                if (Validate())
                {
                    var success = await new MemberContactModel().ForgotPasswordForDeviceAsync(Username);
                    if (!string.IsNullOrEmpty(success))
                    {
                        await NavigationService.NavigateAsync(nameof(ResetPasswordPage), new NavigationParameters { { "username", Username },{"code", success } });

                    }
                }
            }
            catch (Exception)
            {

                IsPageEnabled = false;
            }
            IsPageEnabled = false;
        }

        private bool Validate()
        {
            if (string.IsNullOrEmpty(Username))
            {
               App.dialogService.DisplayAlertAsync("Error!!", AppResources.ResourceManager.GetString("AccountViewUserNameEmpty", AppResources.Culture), "OK");

                return false;
            }

            return true;
        }


    }
}
