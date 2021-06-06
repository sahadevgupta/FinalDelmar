using FormsLoyalty.Controls.Stepper;
using FormsLoyalty.ViewModels;
using System.Linq;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class CheckoutPage : ContentPage
    {
        CheckoutPageViewModel _viewModel;
        public CheckoutPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as CheckoutPageViewModel;
            _viewModel.SubGrid = subGrid;

            StepBarComponent stepbar = new StepBarComponent(_viewModel);

            _viewModel.MainGrid = mainGrid;
            stepbar.HorizontalOptions = LayoutOptions.Center;
            stepbar.VerticalOptions = LayoutOptions.CenterAndExpand;
            _viewModel.MainGrid.Children.Add(stepbar, 0, 0);
            _viewModel.AddContentForSelectedStep();

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
           // (this.Parent.Parent as HomeMasterDetailPage).ToolbarItems
          
        }
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            
        }

        
    }
}
