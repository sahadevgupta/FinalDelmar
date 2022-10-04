using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using LSRetail.Omni.Domain.DataModel.Base;
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
    public class ChangePasswordPageViewModel : ViewModelBase
    {
        private string _currentPassword;
        public string CurrentPassword
        {
            get { return _currentPassword; }
            set { SetProperty(ref _currentPassword, value); }
        }
        private string _newPassword;
        public string NewPassword
        {
            get { return _newPassword; }
            set { SetProperty(ref _newPassword, value); }
        }

        private string _newConfPassword;
        public string NewConfPassword
        {
            get { return _newConfPassword; }
            set { SetProperty(ref _newConfPassword, value); }
        }
        private string _passwordPolicy;
        public string passwordPolicy
        {
            get { return _passwordPolicy; }
            set { SetProperty(ref _passwordPolicy, value); }
        }

        private bool _isError;
        public bool IsError
        {
            get { return _isError; }
            set { SetProperty(ref _isError, value); }
        }

        public DelegateCommand ChangePassCommand { get; set; }
        public ChangePasswordPageViewModel(INavigationService navigationService):base(navigationService)
        {
            ChangePassCommand = new DelegateCommand(async() => await ChangePassword());
        }

        public async Task ChangePassword()
        {
            IsPageEnabled = true;
            if (!ValidateData())
            {
               
                var success = await new MemberContactModel().ChangePassword(AppData.Device.UserLoggedOnToDevice.UserName, NewPassword, CurrentPassword);

                if (success)
                {
                    DependencyService.Get<INotify>().ShowToast(AppResources.ResourceManager.GetString("ChangePasswordChangePasswordSuccess", AppResources.Culture));
                    await NavigationService.GoBackAsync();
                }
            }
            IsPageEnabled = true;
        }
        private bool ValidateData()
        {
            if (string.IsNullOrEmpty(CurrentPassword))
            {
                IsError = true;
            }
            else if (string.IsNullOrEmpty(NewPassword))
            {
                IsError = true;
            }
            else if (NewPassword != NewConfPassword)
            {
                IsError = true;
            }

            

            return IsError;
        }

        private async Task LoadAppsettings()
        {
            var results = await new GeneralSearchModel().AppSettings(ConfigKey.Password_Policy);

            passwordPolicy = results;
        }
        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
            parameters.Add("edit", true);
        }
        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
           await LoadAppsettings();
        }
    }
}
