using FormsLoyalty.ViewModels;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class SearchPage : ContentPage
    {
        SearchPageViewModel _viewModel;
        public SearchPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as SearchPageViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _viewModel.LoadData();
        }
    }
}
