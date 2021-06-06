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
            ChangeToolbarIcon();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
           
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            viewtoolbar.IconImageSource = "ic_view_list_white_24dp";
        }
        private void ChangeToolbarIcon()
        {

            try
            {
                if (Settings.ShowCard)
                {
                    viewtoolbar.IconImageSource = "ic_view_list_white_24dp";
                    _viewModel.count = 2;
                    stores.ItemTemplate = (DataTemplate)Resources["CardView"];
                    stores.Margin = new Thickness(10);
                }
                else
                {
                    _viewModel.count = 1;
                    viewtoolbar.IconImageSource = "ic_view_module_white_24dp";
                    stores.ItemTemplate = (DataTemplate)Resources["ListView"];

                    stores.Margin = new Thickness(0);

                }
            }
            catch (System.Exception)
            {

                if (Settings.ShowCard)
                {
                    viewtoolbar.IconImageSource = "ic_view_list_white_24dp";
                    _viewModel.count = 2;
                    stores.ItemTemplate = (DataTemplate)Resources["CardView"];
                    stores.Margin = new Thickness(10);
                }
                else
                {
                    _viewModel.count = 1;
                    viewtoolbar.IconImageSource = "ic_view_module_white_24dp";
                    stores.ItemTemplate = (DataTemplate)Resources["ListView"];

                    stores.Margin = new Thickness(0);

                }
            }

        }

        private void ToolbarItem_Clicked(object sender, System.EventArgs e)
        {
            Settings.ShowCard = !Settings.ShowCard;
            ChangeToolbarIcon();

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
