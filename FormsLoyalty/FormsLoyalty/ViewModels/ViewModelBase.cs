using FormsLoyalty.Controls.Stepper;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using XF.Material.Forms.Dialogs;

namespace FormsLoyalty.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible
    {
        protected INavigationService NavigationService { get; private set; }
        public int StepListCount { get; set; }
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private bool _isSnackBarVisible;
        public bool IsSnackBarVisible
        {
            get { return _isSnackBarVisible; }
            set { SetProperty(ref _isSnackBarVisible, value); }
        }

        private bool _isPageEnabled;
        public bool IsPageEnabled
        {
            get { return _isPageEnabled; }
            set { SetProperty(ref _isPageEnabled, value); }
        }
        private bool _isNotConnected;
        public bool IsNotConnected
        {
            get { return this._isNotConnected; }
            set { SetProperty(ref _isNotConnected, value); }
        }
        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;

            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            IsNotConnected = Connectivity.NetworkAccess != NetworkAccess.Internet;
        }
        ~ViewModelBase()
        {
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        }
        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            IsNotConnected = e.NetworkAccess != NetworkAccess.Internet;
            ConnectionChanged(IsNotConnected);
        }
       
        public virtual void ConnectionChanged(bool IsNotConnected)
        {

        }

        public virtual void OnStepTapped(StepBarModel stepBarModel)
        {

        }
        public virtual void Initialize(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {

        }
    }
}
