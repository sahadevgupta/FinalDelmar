using FormsLoyalty.ViewModels;
using LSRetail.Omni.Domain.DataModel.Loyalty.Magazine;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class MagazinePage : ContentPage
    {
        MagazinePageViewModel _viewModel;
        public MagazinePage()
        {
            InitializeComponent();
            
            _viewModel = BindingContext as MagazinePageViewModel;
        }

        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            _viewModel.NavigateToDetail((sender as Button).CommandParameter as MagazineModel);
        }
    }
}
