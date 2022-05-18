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
            stepbar.HorizontalOptions = LayoutOptions.FillAndExpand;
            stepbar.VerticalOptions = LayoutOptions.StartAndExpand;
            _viewModel.MainGrid.Children.Add(stepbar, 0, 0);
            _viewModel.AddContentForSelectedStep();

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
           // (this.Parent.Parent as HomeMasterDetailPage).ToolbarItems
          
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            
        }

        protected override bool OnBackButtonPressed()
        {
            
            Device.BeginInvokeOnMainThread(async () =>
            {
                var result = await this.DisplayAlert("Alert!", AppResources.txtExist, AppResources.ApplicationYes, AppResources.ApplicationNo);
                if (result)
                {
                    MessagingCenter.Send((App)Xamarin.Forms.Application.Current, "RegisterHardwareBackPressed");
                    await Navigation.PopAsync();
                }// or anything else
            });
            return true;
        }

    }
}
