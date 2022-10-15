using FormsLoyalty.Helpers;
using FormsLoyalty.ViewModels;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using System.Linq;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class StoreLocatorPage : ContentPage
    {
        StoreLocatorPageViewModel _viewModel;
        public StoreLocatorPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as StoreLocatorPageViewModel;
        }
        
        
        private async void Button_Clicked(object sender, System.EventArgs e)
        {
            var grid = (Grid)sender;
            grid.Opacity = 0;
            await grid.FadeTo(1, 250);
            await _viewModel.NavigateToMap();
            grid.Opacity = 1;
        }

        private async void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            var grid = (Grid)sender;
            grid.Opacity = 0;
            await grid.FadeTo(1, 250);

            await _viewModel.NaviagteToDetail((e as TappedEventArgs).Parameter as Store);

            grid.Opacity = 1;
        }

        private  void stores_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            if (!Settings.ShowCard)
            {
                if (e.LastVisibleItemIndex > _viewModel.stores.Count - 1)
                {
                     _viewModel.LoadMore();
                }
            }

            else if (e.LastVisibleItemIndex == _viewModel.stores.Count - 1)
            {
                 _viewModel.LoadMore();
            }
        }
    }
}
