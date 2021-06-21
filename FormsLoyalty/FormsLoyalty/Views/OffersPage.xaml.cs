using FormsLoyalty.Helpers;
using FormsLoyalty.ViewModels;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class OffersPage : ContentPage
    {
        OffersPageViewModel _viewModel;
        public OffersPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as OffersPageViewModel;
           
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
           _viewModel.LoadOffers();
           // ChangeToolbarIcon();
        }

        private void ChangeToolbarIcon()
        {
            try
            {
                if (Settings.ShowCard)
                {
                    viewtoolbar.IconImageSource = "ic_view_list_white_24dp";
                    _viewModel.count = 2;
                    offerlist.ItemTemplate = (DataTemplate)Resources["CardView"];
                }
                else
                {
                    _viewModel.count = 1;
                    viewtoolbar.IconImageSource = "ic_view_module_white_24dp";
                    offerlist.ItemTemplate = (DataTemplate)Resources["ListView"];
                }
            }
            catch (System.Exception)
            {

                if (Settings.ShowCard)
                {
                    viewtoolbar.IconImageSource = "ic_view_list_white_24dp";
                    _viewModel.count = 2;
                    offerlist.ItemTemplate = (DataTemplate)Resources["CardView"];
                }
                else
                {
                    _viewModel.count = 1;
                    viewtoolbar.IconImageSource = "ic_view_module_white_24dp";
                    offerlist.ItemTemplate = (DataTemplate)Resources["ListView"];
                }
            }
           
        }

        private void ToolbarItem_Clicked(object sender, System.EventArgs e)
        {
            Settings.ShowCard = !Settings.ShowCard;
            //ChangeToolbarIcon();
        }

        private async void Offer_Tapped(object sender, System.EventArgs e)
        {
            var stack = (Grid)sender;
            stack.Opacity = 0;
            await stack.FadeTo(1, 250);

            await _viewModel.NavigateToOfferDetail((e as TappedEventArgs).Parameter as PublishedOffer);

            stack.Opacity = 1;
        }

        private async void addremovebtn_Clicked(object sender, System.EventArgs e)
        {
           
            if (sender is Button button)
            {
                var offer = button.BindingContext as PublishedOffer;
                _viewModel.AddRemoveOffer(offer);

                if (offer.Selected)
                {
                    button.ImageSource = "ic_action_remove";
                }
                else
                {
                    button.ImageSource = "ic_action_new";
                }
            }
            else
            {
                var view = ((Grid)sender);
                view.Opacity = 0;
                await view.FadeTo(1, 250);
                var offer = view.BindingContext as PublishedOffer;

                _viewModel.AddRemoveOffer(offer);

                if (offer.Selected)
                {
                    var img = ((Grid)sender).Children[0] as Image;
                    img.Source = "ic_action_remove";
                }
                else
                {
                    var img = ((Grid)sender).Children[0] as Image;
                    img.Source = "ic_action_new";
                }
                view.Opacity = 1;
            }

            

        }
    }
}
