using FormsLoyalty.ViewModels;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class CheckoutShippingPage : ContentPage
    {
        CheckoutShippingPageViewModel _viewModel;
        public CheckoutShippingPage()
        {
            InitializeComponent();
            
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            _viewModel = BindingContext as CheckoutShippingPageViewModel;
        }

        private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
           
            if (card.IsChecked)
            {
                _viewModel.isCreditCard = true;
            }
            else
            _viewModel.isCreditCard = false;
        }
    }
}
