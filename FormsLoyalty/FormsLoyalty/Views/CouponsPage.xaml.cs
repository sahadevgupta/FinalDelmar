using FormsLoyalty.Helpers;
using FormsLoyalty.ViewModels;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class CouponsPage : ContentPage
    {
        CouponsPageViewModel _viewModel;
        public CouponsPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as CouponsPageViewModel;
        }

        

        private void ToolbarItem_Clicked(object sender, System.EventArgs e)
        {
            Settings.ShowCard = !Settings.ShowCard;
        }
        private async void Coupon_Tapped(object sender, System.EventArgs e)
        {
            var view = (Grid)sender;
            view.Opacity = 0;
            await view.FadeTo(1, 250);

            await _viewModel.NavigateToCouponDetail((e as TappedEventArgs).Parameter as PublishedOffer);

            view.Opacity = 1;
        }

        private async void addremovebtn_Clicked(object sender, System.EventArgs e)
        {

            if (sender is Button button)
            {
                var offer = button.BindingContext as PublishedOffer;
                _viewModel.AddRemoveCouponFromQrCode(offer);

                
            }
            else
            {
                var view = ((Grid)sender);
                view.Opacity = 0;
                await view.FadeTo(1, 250);
                var offer = view.BindingContext as PublishedOffer;

                _viewModel.AddRemoveCouponFromQrCode(offer);

                
                view.Opacity = 1;
            }



        }
    }
}
