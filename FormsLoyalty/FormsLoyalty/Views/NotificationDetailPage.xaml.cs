﻿using FormsLoyalty.ViewModels;
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

        
    }
}
