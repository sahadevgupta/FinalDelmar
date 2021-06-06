using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing.Common.ReedSolomon;

namespace FormsLoyalty.ViewModels
{
    public class ResetPasswordPageViewModel : ViewModelBase
    {
        private string _Username;
        public string Username
        {
            get { return _Username; }
            set { SetProperty(ref _Username, value); }
        }

        public string _serverResetCode { get; private set; }

        private string _resetCode;
        public string resetCode
        {
            get { return _resetCode; }
            set { SetProperty(ref _resetCode, value); }
        }

        private string _newPassword;
        public string NewPassword
        {
            get { return _newPassword; }
            set { SetProperty(ref _newPassword, value); }
        }

        private string _confPassword;
        public string ConfPassword
        {
            get { return _confPassword; }
            set { SetProperty(ref _confPassword, value); }
        }

        public DelegateCommand ResetCommand { get; set; }
        public ResetPasswordPageViewModel(INavigationService navigationService): base(navigationService)
        {
            ResetCommand = new DelegateCommand(async() => await ResetPassword());
        }
        internal async void GoBack()
        {
            DependencyService.Get<INotify>().ShowToast("Password changed successfully!!");
            await NavigationService.NavigateAsync("../../");
        }

        private async Task ResetPassword()
        {
            IsPageEnabled = true;
            if (await Validate())
            {
                var success = await new MemberContactModel().ResetPassword(Username, resetCode, NewPassword);

                if (success)
                {
                    GoBack();
                }
            }
              IsPageEnabled = true;
        }


        private async Task<bool> Validate()
        {
            if (string.IsNullOrEmpty(Username))
            {

               await App.dialogService.DisplayAlertAsync("Error!!", AppResources.ResourceManager.GetString("AccountViewUserNameEmpty", AppResources.Culture), AppResources.ResourceManager.GetString("ApplicationOk", AppResources.Culture));
                return false;
            }

            if (string.IsNullOrEmpty(resetCode))
            {
                await App.dialogService.DisplayAlertAsync("Error!!", AppResources.ResourceManager.GetString("ResetPasswordResetCodeEmpty", AppResources.Culture), AppResources.ResourceManager.GetString("ApplicationOk", AppResources.Culture));
                return false;
                
            }
            else
            {
                if (!_serverResetCode.Equals(resetCode))
                {
                    await App.dialogService.DisplayAlertAsync("Error!!", "Code does not match", AppResources.ResourceManager.GetString("ApplicationOk", AppResources.Culture));
                    return false;
                }
            }

            if (string.IsNullOrEmpty(NewPassword))
            {
                await App.dialogService.DisplayAlertAsync("Error!!", AppResources.ResourceManager.GetString("AccountViewPasswordEmpty", AppResources.Culture), AppResources.ResourceManager.GetString("ApplicationOk", AppResources.Culture));
                return false;
                
            }

            if (NewPassword != ConfPassword)
            {
                await App.dialogService.DisplayAlertAsync("Error!!", AppResources.ResourceManager.GetString("ChangePasswordPasswordsDontMatch", AppResources.Culture), AppResources.ResourceManager.GetString("ApplicationOk", AppResources.Culture));
                return false;
                
            }

            return true;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            Username = parameters.GetValue<string>("username");
            _serverResetCode = parameters.GetValue<string>("code");

        }
    }
}
