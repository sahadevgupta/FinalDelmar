﻿using FormsLoyalty.ViewModels;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class DemonstrationPage : ContentPage
    {
        DemonstrationPageViewModel _viewModel;
        public DemonstrationPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as DemonstrationPageViewModel;
        }

        private void Button_Clicked(object sender, System.EventArgs e)
        {
            var view = (Button)sender;
            _viewModel.SlideToNextPage(view.Text);
        }
    }
}
