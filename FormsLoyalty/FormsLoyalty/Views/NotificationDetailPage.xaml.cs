using FormsLoyalty.ViewModels;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class NotificationDetailPage : ContentPage
    {
        NotificationDetailPageViewModel _viewModel;
        public NotificationDetailPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as NotificationDetailPageViewModel;
        }

        private void CarouselView_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
           var currentImgView = e.CurrentItem as ImageView;

            _viewModel.LoadImage(currentImgView);
        }
    }
}
