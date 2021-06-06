using FormsLoyalty.ViewModels;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class CouponDetailsPage : ContentPage
    {
        CouponDetailsPageViewModel _viewModel;

        public CouponDetailsPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as CouponDetailsPageViewModel;
        }

        private void CarouselView_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            var currentImgView = e.CurrentItem as ImageView;

            Task.Run(async () =>
            {
                currentImgView.Image = await _viewModel.GetImageById(currentImgView.Id);
            });


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
            await view.FadeTo(1, 250,easing: Easing.BounceIn);

            _viewModel.AddRemoveOffer();
            view.Opacity = 1;
        }

    }
}
