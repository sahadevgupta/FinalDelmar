using FormsLoyalty.Utils;
using FormsLoyalty.ViewModels;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class MoreInfoPage : ContentPage
    {
        MoreInfoPageViewModel _viewModel;
        public MoreInfoPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as MoreInfoPageViewModel;
        }

        private async void menu_Tapped(object sender, System.EventArgs e)
        {
            var view = (Grid)sender;
            view.BackgroundColor = Color.LightPink;
            view.Opacity = 0;
            await view.FadeTo(1, 250);
           
            _viewModel.DrawerSelected((e as TappedEventArgs).Parameter as DrawerMenuItem);
            view.BackgroundColor = Color.Transparent;
            view.Opacity = 1;
        }
       
        private async void BarcodeScan_Tapped(object sender, System.EventArgs e)
        {
            var view = (Grid)sender;
            view.BackgroundColor = Color.LightPink;
            view.Opacity = 0;
            await view.FadeTo(1, 250);

            _viewModel.DrawerSelected((e as TappedEventArgs).Parameter as DrawerMenuItem);
            view.BackgroundColor = Color.Transparent;
            view.Opacity = 1;
        }

        private async void Profile_Tapped(object sender, System.EventArgs e)
        {
            var view = (Grid)sender;
            view.Opacity = 0;
            await view.FadeTo(1, 250);

            await _viewModel.NavigateToProfile();
            
            view.Opacity = 1;
        }
        private async void Settings_Tapped(object sender, System.EventArgs e)
        {
            var view = (StackLayout)sender;
           
            view.Opacity = 0;
            await view.FadeTo(1, 250);

            await _viewModel.OnSettingSelected((e as TappedEventArgs).Parameter.ToString());
            
            view.Opacity = 1;
        }
    }
}
