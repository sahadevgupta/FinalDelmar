using FormsLoyalty.ViewModels;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class OfferDetailsPage : ContentPage
    {
        OfferDetailsPageViewModel _viewModel;
        public OfferDetailsPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as OfferDetailsPageViewModel;
        }

        private async void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            var stack = (StackLayout)sender;
            await stack.FadeTo(1, 250, easing: Easing.BounceIn);

            await _viewModel.NavigateToItemPage((e as TappedEventArgs).Parameter as LoyItem);
        }

        private async void Button_Clicked(object sender, System.EventArgs e)
        {
            var view = ((Grid)sender);
            view.Opacity = 0;
            await view.FadeTo(1, 250);

            _viewModel.AddRemoveOffer();

            if (_viewModel.selectedOffer.Selected)
            {
                var img = ((Grid)sender).Children[1] as Image;
                img.Source = "ic_action_remove";
            }
            else
            {
                var img = ((Grid)sender).Children[1] as Image;
                img.Source = "ic_action_new";
            }
            view.Opacity = 1;
        }
    }
}
