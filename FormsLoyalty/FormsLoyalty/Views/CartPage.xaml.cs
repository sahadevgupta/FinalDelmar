using FormsLoyalty.Models;
using FormsLoyalty.ViewModels;
using System;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class CartPage : ContentPage
    {
        CartPageViewModel _viewModel;
        public CartPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as CartPageViewModel;
        }

        private async void Delete_Tapped(object sender, System.EventArgs e)
        {
            var view = (Label)sender;
           
            view.Opacity = 0;
            await view.FadeTo(1, 250);

            await _viewModel.OnDelete((e as TappedEventArgs).Parameter as Basket);
           
            view.Opacity = 1;
        }

        private async void minus_Tapped(object sender, EventArgs e)
        {
            var view = (Label)sender;
            
            view.Opacity = 0;
            var selectedItem = view.BindingContext as Basket;
            var qty = selectedItem.Quantity;
            if (selectedItem.Quantity > 1)
            {
                qty -= 1;
                var index = await _viewModel.OnQtyChanged(selectedItem, qty);
                if (index > 0)
                {
                    selectedItem.Quantity = qty;
                    basketlist.ScrollTo(index,ScrollToPosition.Center,true);
                }
            }
            await view.FadeTo(1, 250);
        }
        private async void plus_Tapped(object sender, EventArgs e)
        {
            var view = (Label)sender;
            var selectedItem = view.BindingContext as Basket;
            var qty = selectedItem.Quantity;
            if (selectedItem.Quantity >= 1)
            {
                qty += 1;
                var index = await _viewModel.OnQtyChanged(selectedItem,qty);
                if (index > 0)
                {
                    selectedItem.Quantity = qty;
                    basketlist.ScrollTo(index, ScrollToPosition.Center, true);
                }
            }
            view.Opacity = 0;
            await view.FadeTo(1, 250);
        }
    }
}
