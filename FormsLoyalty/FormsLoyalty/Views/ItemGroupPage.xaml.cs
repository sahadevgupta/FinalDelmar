using FormsLoyalty.Helpers;
using FormsLoyalty.Utils;
using FormsLoyalty.ViewModels;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;

namespace FormsLoyalty.Views
{
    public partial class ItemGroupPage : ContentPage
    {
        ItemGroupPageViewModel _viewModel;
        public ItemGroupPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as ItemGroupPageViewModel;
            
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

       

        private async void Item_Tapped(object sender, System.EventArgs e)
        {
            var view = (Grid)sender;
            view.Opacity = 0;
            await view.FadeTo(1, 250);
            
            await _viewModel.NavigateToItemPage((e as TappedEventArgs).Parameter as LoyItem);

            view.Opacity = 1;
        }

       

        bool _firstInstance = false;
        private async void collectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            var view = (CollectionView)sender;
            Debug.WriteLine("HorizontalDelta: " + e.HorizontalDelta);
            Debug.WriteLine("VerticalDelta: " + e.VerticalDelta);
            Debug.WriteLine("HorizontalOffset: " + e.HorizontalOffset);
            Debug.WriteLine("VerticalOffset: " + e.VerticalOffset);
            Debug.WriteLine("FirstVisibleItemIndex: " + e.FirstVisibleItemIndex);
            Debug.WriteLine("CenterItemIndex: " + e.CenterItemIndex);
            Debug.WriteLine("LastVisibleItemIndex: " + e.LastVisibleItemIndex);

            if (!Settings.ShowCard)
            {
                if (e.LastVisibleItemIndex >_viewModel.Items.Count - 1)
                {
                    await _viewModel.LoadMore();
                }
            }

           else if (e.LastVisibleItemIndex == _viewModel.Items.Count - 1)
            {
               await _viewModel.LoadMore();
            }

           

        }

       

        private void SelectSortingOption(object sender, EventArgs e)
        {
            var view = sender as View;
            var parent = view.Parent as StackLayout;

            foreach (var child in parent.Children)
            {
                VisualStateManager.GoToState(child, "Normal");
                ChangeTextColor(child, "#707070", false);
            }

            VisualStateManager.GoToState(view, "Selected");
            var panckake = view as PancakeView;
            ChangeTextColor(view, "#FFFFFF", true);
        }
        private async void ChangeTextColor(View child, string hexColor, bool isTheSelectedSortOption)
        {
            var txtCtrl = child.FindByName<Label>("sortingOption");
            if (isTheSelectedSortOption)
            {
               await _viewModel.FilterItems(txtCtrl.Text);
            }
            if (txtCtrl != null)
                txtCtrl.TextColor = Color.FromHex(hexColor);
        }

        private async void AddToCart_Clicked(object sender, EventArgs e)
        {
            var item = ((Button)sender).BindingContext as LoyItem;
            bool IsSucess = await _viewModel.AddItemToBasket(item);
            if (IsSucess)
            {
                ((Button)sender).IsVisible = false;
            }
        }
        private async void minus_Tapped(object sender, EventArgs e)
        {
            var view = (Label)sender;

            view.Opacity = 0;
            var selectedItem = view.BindingContext as LoyItem;
            var qty = selectedItem.Quantity;
            if (selectedItem.Quantity > 1)
            {
                qty -= 1;
                var IsSuccess = await _viewModel.OnQtyChanged(selectedItem,qty);
                if (IsSuccess)
                    selectedItem.Quantity = qty;

            }
            await view.FadeTo(1, 250);
        }
        private async void plus_Tapped(object sender, EventArgs e)
        {
            var view = (Label)sender;
            var selectedItem = view.BindingContext as LoyItem;
            var qty = selectedItem.Quantity;
            if (selectedItem.Quantity >= 1)
            {
                qty += 1;
                var IsSuccess = await _viewModel.OnQtyChanged(selectedItem,qty);
                if (IsSuccess)
                    selectedItem.Quantity = qty;
            }
            view.Opacity = 0;
            await view.FadeTo(1, 250);
        }

        private async void WishList_Tapped(object sender, EventArgs e)
        {
            var view = (Image)sender;
            var selectedItem = view.BindingContext as LoyItem;
            if (selectedItem != null)
            {
                await _viewModel.AddRemoveWishList(selectedItem);
               
            }

            view.Opacity = 0;
            await view.FadeTo(1, 250);
        }
    }
}
