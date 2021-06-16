using FormsLoyalty.Helpers;
using FormsLoyalty.Utils;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace FormsLoyalty.ViewModels
{
    public class AppSettingsPageViewModel : ViewModelBase
    {
        private bool _isEnLan;
        public bool IsEnLan
        {
            get { return _isEnLan; }
            set { SetProperty(ref _isEnLan, value); }
        }

        private bool _isArLan;
        public bool IsArLan
        {
            get { return _isArLan; }
            set { SetProperty(ref _isArLan, value); }
        }

        public DelegateCommand changeLangCommand { get; set; }
        public AppSettingsPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            changeLangCommand = new DelegateCommand(ChangeLanguage);
            LoadData();
        }

        private void LoadData()
        {
            if (!Settings.RTL)
            {
                IsEnLan = true;
            }
            else
                IsArLan = true;
        }

        private async void ChangeLanguage()
        {
            IsPageEnabled = true;
            AppData.IsLanguageChanged = true;
            CultureInfo language;
            if (IsEnLan)
            {
                 language = CultureInfo.GetCultures(CultureTypes.NeutralCultures).ToList().First(element => element.EnglishName.Contains("English"));
                Settings.RTL = false;
            }
            else
            {
                language = CultureInfo.GetCultures(CultureTypes.NeutralCultures).ToList().First(element => element.EnglishName.Contains("Arabic"));
                Settings.RTL = true;
            }
           
            Thread.CurrentThread.CurrentUICulture = language;
            AppResources.Culture = language;

            await NavigationService.NavigateAsync("app:///MainTabbedPage?selectedTab=MainPage");

            IsPageEnabled = false;

        }
        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

           
        }
    }
}
