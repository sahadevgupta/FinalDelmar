using FormsLoyalty.Controls.Stepper;
using FormsLoyalty.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class CheckoutPage : ContentPage
    {
        CheckoutPageViewModel _viewModel;
        public CheckoutPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as CheckoutPageViewModel;
            

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
           // (this.Parent.Parent as HomeMasterDetailPage).ToolbarItems
          
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            
        }

        protected override bool OnBackButtonPressed()
        {
            
            Device.BeginInvokeOnMainThread(async () =>
            {
                var result = await this.DisplayAlert("Alert!", AppResources.txtExist, AppResources.ApplicationYes, AppResources.ApplicationNo);
                if (result)
                {
                    MessagingCenter.Send((App)Xamarin.Forms.Application.Current, "RegisterHardwareBackPressed");
                    await Navigation.PopAsync();
                }// or anything else
            });
            return true;
        }

        #region Review Stack
        private async void viewOfferBtn_Clicked(object sender, EventArgs e)
        {
            await _viewModel.ShowCouponsPopUpView();
        }

        private async void removeOfferBtn_Tapped(object sender, EventArgs e)
        {
            await _viewModel.RemoveCoupon();
        }

        private async void placeOrderbtn_Clicked(object sender, EventArgs e)
        {
            await _viewModel.PlaceOrder();
        }
        #endregion

        #region Address Stack
        private void NextBtn_Clicked(object sender, EventArgs e)
        {
            var result = _viewModel.NavigateToNextStep();
            if (result)
            {
                addresstrackergridimage.Source = ImageSource.FromFile("check.png");
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await progress.ProgressTo(1, 800, Easing.Linear);
                    progress.Progress = 1;

                    reviewstackview.IsVisible = true;
                    addressStackView.IsVisible = false;
                    reviewtrackergrid.BackgroundColor = Color.FromHex("#127ABF");
                });
            }
        }
        #endregion

        private void StepTapped_Tapped(object sender, EventArgs e)
        {
            

            Device.BeginInvokeOnMainThread(async () =>
            {
                await progress.ProgressTo(0, 800, Easing.Linear);
                progress.Progress = 0;

                reviewstackview.IsVisible = false;
                addressStackView.IsVisible = true;
            });
        }
    }
}
