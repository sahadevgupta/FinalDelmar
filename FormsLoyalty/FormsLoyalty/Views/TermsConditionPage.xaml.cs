using FormsLoyalty.ViewModels;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class TermsConditionPage : ContentPage
    {
        TermsConditionPageViewModel _viewModel;
        public TermsConditionPage()
        {
            InitializeComponent();

            _viewModel = BindingContext as TermsConditionPageViewModel;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
        }

        private void WebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            //_viewModel.IsPageEnabled = true;
        }

        private void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            //_viewModel.IsPageEnabled = false;
        }
    }
}
