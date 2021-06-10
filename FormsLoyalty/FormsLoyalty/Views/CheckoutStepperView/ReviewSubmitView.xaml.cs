using FormsLoyalty.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FormsLoyalty.Views.CheckoutStepperView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReviewSubmitView : ContentView
    {
        CheckoutPageViewModel _viewModel;
        public ReviewSubmitView(CheckoutPageViewModel viewmodel)
        {
            _viewModel = viewmodel;
            InitializeComponent();
            BindingContext = _viewModel;

        }

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
    }
}