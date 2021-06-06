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
        protected override void OnAppearing()
        {
            base.OnAppearing();
            //ChangeToolbarIcon();
        }

        private void ChangeToolbarIcon()
        {
            //try
            //{
            //    if (Settings.ShowCard)
            //    {
            //        viewtoolbar.IconImageSource = "ic_view_list_white_24dp";
            //        _viewModel.SpanCount = 2;
            //        couponlist.ItemTemplate = (DataTemplate)Resources["CardView"];
            //    }
            //    else
            //    {
            //        _viewModel.SpanCount = 1;
            //        viewtoolbar.IconImageSource = "ic_view_module_white_24dp";
            //        couponlist.ItemTemplate = (DataTemplate)Resources["ListView"];
            //    }
            //}
            //catch (System.Exception)
            //{

            //    if (Settings.ShowCard)
            //    {
            //        viewtoolbar.IconImageSource = "ic_view_list_white_24dp";
            //        _viewModel.SpanCount = 2;
            //        couponlist.ItemTemplate = (DataTemplate)Resources["CardView"];
            //    }
            //    else
            //    {
            //        _viewModel.SpanCount = 1;
            //        viewtoolbar.IconImageSource = "ic_view_module_white_24dp";
            //        couponlist.ItemTemplate = (DataTemplate)Resources["ListView"];
            //    }
            //}

        }

        private void ToolbarItem_Clicked(object sender, System.EventArgs e)
        {
            Settings.ShowCard = !Settings.ShowCard;
            ChangeToolbarIcon();
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

                //if (offer.Selected)
                //{
                //    button.ImageSource = "ic_action_remove";
                //}
                //else
                //{
                //    button.ImageSource = "ic_action_new";
                //}
            }
            else
            {
                var view = ((Grid)sender);
                view.Opacity = 0;
                await view.FadeTo(1, 250);
                var offer = view.BindingContext as PublishedOffer;

                _viewModel.AddRemoveCouponFromQrCode(offer);

                //if (offer.Selected)
                //{
                //    var img = ((Grid)sender).Children[0] as Image;
                //    img.Source = "ic_action_remove";
                //}
                //else
                //{
                //    var img = ((Grid)sender).Children[0] as Image;
                //    img.Source = "ic_action_new";
                //}
                view.Opacity = 1;
            }



        }
    }
}
