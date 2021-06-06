using FormsLoyalty.ViewModels;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class ResetPasswordPage : ContentPage
    {
        ResetPasswordPageViewModel _viewModel;
        public ResetPasswordPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as ResetPasswordPageViewModel;
        }

        protected override bool OnBackButtonPressed()
        {
            _viewModel.GoBack();
            return base.OnBackButtonPressed();
        }
    }
}
