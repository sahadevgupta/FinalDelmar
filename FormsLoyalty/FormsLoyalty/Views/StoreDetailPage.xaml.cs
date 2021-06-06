using FormsLoyalty.ViewModels;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class StoreDetailPage : ContentPage
    {
        StoreDetailPageViewModel _viewModel;
        public StoreDetailPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as StoreDetailPageViewModel;
        }

        private async void Direction_Tapped(object sender, System.EventArgs e)
        {
            var view = (Grid)sender;
            view.Opacity = 0;
           await view.FadeTo(1, 250);
           await _viewModel.ShowDirections();
            view.Opacity = 1;
        }
    }
}
