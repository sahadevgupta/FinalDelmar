using FormsLoyalty.ViewModels;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class TransactionDetailPage : ContentPage
    {
        TransactionDetailPageViewModel _viewModel;
        public TransactionDetailPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as TransactionDetailPageViewModel;
        }

        private async void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            var frame = (Frame)sender;
            frame.Opacity = 0;
            await frame.FadeTo(1, 250);

            await _viewModel.NavigateToItemPage((e as TappedEventArgs).Parameter as SalesEntryLine);

            frame.Opacity = 1;
        }
    }
}
