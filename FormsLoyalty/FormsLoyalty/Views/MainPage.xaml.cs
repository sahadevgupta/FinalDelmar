using FormsLoyalty.ViewModels;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class MainPage : ContentPage
    {
        MainPageViewModel _viewModel;
       
        public MainPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as MainPageViewModel;
           

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
          
            MessagingCenter.Subscribe<App, List<Tuple<byte[], string>>>((App)Xamarin.Forms.Application.Current, "ImagesSelected", GetFileData);
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _viewModel.IsSuggestionFound = false;
            if (!_viewModel.IsUploadBtnClicked)
                MessagingCenter.Unsubscribe<App, List<Tuple<byte[], string>>>((App)Xamarin.Forms.Application.Current, "ImagesSelected");
        }


        private void GetFileData(App arg1, List<Tuple<byte[], string>> arg2)
        {
            if (_viewModel.CanNavigate)
                return;

            _viewModel.CanNavigate = true;

            _viewModel.NavigateToScanPage(arg2);
            _viewModel.IsUploadBtnClicked = false;
            _viewModel.CanNavigate = false;
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var frame = (Frame)sender;
            await frame.ScaleTo(1.5, 100);
            await frame.ScaleTo(1, 100);


           await _viewModel.NavigateToCategory((e as TappedEventArgs).Parameter.ToString());
        }


        /// <summary>
        /// this method is called when you clicked camera icon.
        /// It gives an option to click or upload image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CameraImg_Tapped(object sender, EventArgs e)
        {
            var view = (Grid)sender;
            view.Opacity = 0;
            await view.FadeTo(1, 250);

          
            _viewModel.ScanSend();

            view.Opacity = 1;
        }

       

        /// <summary>
        /// this method is called when category is tapped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Category_Tapped(object sender, EventArgs e)
        {
            var stack = (StackLayout)sender;
            stack.Opacity = 0;
            await stack.FadeTo(1, 250);

            _viewModel.NavigateToItemCategory((e as TappedEventArgs).Parameter as ItemCategory);
            stack.Opacity = 1;
        }

        /// <summary>
        /// this method is called when any best seller item or most viewed item is tapped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// It Navigates to Item Page
        private async void Item_Tapped(object sender, EventArgs e)
        {
            var stack = (Grid)sender;
            stack.Opacity = 0;
            await stack.FadeTo(1, 250);

            _viewModel.NavigateToItemPage((e as TappedEventArgs).Parameter as LoyItem);
            stack.Opacity = 1;
        }

        /// <summary>
        /// this method is called when any Offer is tapped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// It Navigates to Offer detail Page
        private async void Offer_Tapped(object sender, EventArgs e)
        {
            var stack = (Grid)sender;
            stack.Opacity = 0;
            await stack.FadeTo(1, 250);

            await _viewModel.NavigateToOfferDetailsPage((e as TappedEventArgs).Parameter as PublishedOffer);
            stack.Opacity = 1;
        }

        
        private void ProgressBar_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var progress = (ProgressBar)sender;
            if (e.PropertyName == ProgressBar.ProgressProperty.PropertyName)
            {
                progress.Animate("animate",new Animation(), easing: Easing.Linear);
            }
        }

        private async void AddToCart_Clicked(object sender, EventArgs e)
        {
            var item = ((Button)sender).BindingContext as LoyItem;
            bool IsSucess = await _viewModel.AddItemToBasket(item);
            //if (IsSucess)
            //{
            //    text.Text = $"{item.Description} has been added to basket!!";
            //    snackbar.IsVisible = true;



            //    ((Button)sender).IsVisible = false;

            //    //function to fire to kill the app or the game

            //    System.Timers.Timer timer = new System.Timers.Timer(5000);
            //    timer.AutoReset = false; // the key is here so it repeats
            //    timer.Elapsed += (s,e1)=>
            //    {
            //        Device.BeginInvokeOnMainThread(() =>
            //        {
            //            snackbar.IsVisible = false;
            //        });
                    
            //    };
            //    timer.Start();
            //}
        }
        private async void minus_Tapped(object sender, EventArgs e)
        {
            var view = (Label)sender;

            view.Opacity = 0;
            await view.FadeTo(1, 250);
            var selectedItem = view.BindingContext as LoyItem;
            var qty = selectedItem.Quantity;
            if (selectedItem.Quantity > 1)
            {
                qty -= 1;
                var IsSuccess = await _viewModel.OnQtyChanged(selectedItem,qty);
                if(IsSuccess)
                     selectedItem.Quantity = qty;
               
            }
            view.Opacity = 1;
        }
        private async void plus_Tapped(object sender, EventArgs e)
        {
            var view = (Label)sender;
            view.Opacity = 0;
            await view.FadeTo(1, 250);
            var selectedItem = view.BindingContext as LoyItem;
            var qty = selectedItem.Quantity;
            if (selectedItem.Quantity >= 1)
            {
                qty += 1;

                var IsSuccess = await _viewModel.OnQtyChanged(selectedItem, qty);
                if (IsSuccess)
                    selectedItem.Quantity = qty;
            }
            
            view.Opacity = 1;
        }

        private async void WishList_Tapped(object sender, EventArgs e)
        {
            var view = (Image)sender;
            view.Opacity = 0;
            await view.FadeTo(1, 250);
            var selectedItem = view.BindingContext as LoyItem;
            if (selectedItem != null)
            {
               var IsWishlistAdded = await _viewModel.AddRemoveWishList(selectedItem);
                if (IsWishlistAdded)
                {
                   // view.Source = "ic_favorite_24dp";
                }
                
            }
            view.Opacity = 1;
            
        }

        private async void points_Tapped(object sender, EventArgs e)
        {
            var view = (Frame)sender;
            view.Opacity = 0;
            await view.FadeTo(1, 250);
            await _viewModel.NavigateToAccountTier();
            view.Opacity = 1;
        }

        

        private void itemsuggestionListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = itemsuggestionListView.SelectedItem as LoyItem;
            if (selectedItem!=null)
            {
                _viewModel.NavigateToItemPage(selectedItem);
            }
            autosuggestview.Unfocus();
            itemsuggestionListView.SelectedItem = null;
        }

        private void OnAutoSuggestionSelect(object sender, EventArgs e)
        {
            var selectedItem = (e as TappedEventArgs).Parameter as LoyItem;
            if (selectedItem != null)
            {
                _viewModel.NavigateToItemPage(selectedItem);
            }
            autosuggestview.Text = string.Empty;
            autosuggestview.Unfocus();
        }

        private async void OnLanguageSelected(object sender, EventArgs e)
        {

            var view = (Frame)sender;
            view.Opacity = 0;
            await view.FadeTo(1, 250);
             _viewModel.ChangeLanguage((e as TappedEventArgs).Parameter.ToString());
            view.Opacity = 1;

           
        }
    }
}