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
    public partial class AddressDetailsView : ContentView
    {
        CheckoutPageViewModel _viewModel;
        public AddressDetailsView(CheckoutPageViewModel viewmodel)
        {
            InitializeComponent();
            BindingContext = viewmodel;
            _viewModel = viewmodel;
        }

        public AddressDetailsView()
        {

        }

        private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {

            //if (card.IsChecked)
            //{
            //    _viewModel.isCreditCard = true;
            //}
            //else
            //    _viewModel.isCreditCard = false;
        }

        private void NextBtn_Clicked(object sender, EventArgs e)
        {
            _viewModel.NavigateToNextStep();
        }
    }
}