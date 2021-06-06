using FormsLoyalty.ViewModels;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class CheckoutTotalPage : ContentPage
    {
        CheckoutTotalPageViewModel _viewModel;
        public CheckoutTotalPage()
        {
            InitializeComponent();
            
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            _viewModel = BindingContext as CheckoutTotalPageViewModel;
        }
    }
}
