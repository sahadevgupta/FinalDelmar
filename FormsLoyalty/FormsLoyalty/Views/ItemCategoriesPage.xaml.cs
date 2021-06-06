using FormsLoyalty.ViewModels;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace FormsLoyalty.Views
{
    public partial class ItemCategoriesPage : ContentPage
    {
        ItemCategoriesPageViewModel _viewModel;
        public ItemCategoriesPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as ItemCategoriesPageViewModel;
           
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }


        private async void Product_Tapped(object sender, System.EventArgs e)
        {
            var view = (Frame)sender;
            await view.ScaleTo(1.5, 100);
            await view.ScaleTo(1, 100);

            _viewModel.NavigateToItemPage((e as TappedEventArgs).Parameter as ProductGroup);
        }

       
    }
}
